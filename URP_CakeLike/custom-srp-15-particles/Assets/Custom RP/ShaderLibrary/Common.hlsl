﻿#ifndef CUSTOM_COMMON_INCLUDED
#define CUSTOM_COMMON_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "UnityInput.hlsl"

#define UNITY_MATRIX_M unity_ObjectToWorld
#define UNITY_MATRIX_I_M unity_WorldToObject
#define UNITY_MATRIX_V unity_MatrixV
#define UNITY_MATRIX_VP unity_MatrixVP
#define UNITY_MATRIX_P glstate_matrix_projection

#if defined(_SHADOW_MASK_ALWAYS) || defined(_SHADOW_MASK_DISTANCE)
	#define SHADOWS_SHADOWMASK
#endif

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"

//内联采样器状态
/*
采样器名称被识别为“内联”采样器状态（全都不区分大小写）：

*“Point”、“Linear”或“Trilinear”（必需）设置纹理过滤模式。

*“Clamp”、“Repeat”、“Mirror”或“MirrorOnce”（必需）设置纹理包裹模式

sampler_point_clamp // filter:point, wrap: clamp u & v
sampler_linear_clamp // filter:linear, wrap: clamp u & v
*/
SAMPLER(sampler_linear_clamp);
SAMPLER(sampler_point_clamp);

bool IsOrthographicCamera () {
	return unity_OrthoParams.w;
}

float OrthographicDepthBufferToLinear (float rawDepth) {
	#if UNITY_REVERSED_Z
		rawDepth = 1.0 - rawDepth;
	#endif
	return (_ProjectionParams.z - _ProjectionParams.y) * rawDepth + _ProjectionParams.y;
}

#include "Fragment.hlsl"

float Square (float x) {
	return x * x;
}

float DistanceSquared(float3 pA, float3 pB) {
	return dot(pA - pB, pA - pB);
}

void ClipLOD (Fragment fragment, float fade) {
	#if defined(LOD_FADE_CROSSFADE)
		float dither = InterleavedGradientNoise(fragment.positionSS, 0);
		clip(fade + (fade < 0.0 ? dither : -dither));
	#endif
}

float3 DecodeNormal (float4 sample, float scale) {
	#if defined(UNITY_NO_DXT5nm)
	    return UnpackNormalRGB(sample, scale);
	#else
	    return UnpackNormalmapRGorAG(sample, scale);
	#endif
}

float3 NormalTangentToWorld (float3 normalTS, float3 normalWS, float4 tangentWS) {
	float3x3 tangentToWorld =
		CreateTangentToWorld(normalWS, tangentWS.xyz, tangentWS.w);
	return TransformTangentToWorld(normalTS, tangentToWorld);
}

#endif