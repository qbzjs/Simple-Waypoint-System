﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/My First Lighting Shader singleLight" {

	Properties {
		_Tint ("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo", 2D) = "white" {}
		[Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0.1
	}

	SubShader {

		Pass {
			Tags {
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM

			#pragma target 3.0

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#include "UnityPBSLighting.cginc"

			float4 _Tint;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			float _Metallic;
			float _Smoothness;

			struct VertexData {
				float4 position : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct Interpolators {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};

			Interpolators MyVertexProgram (VertexData v) {
				Interpolators i;
				i.position = UnityObjectToClipPos(v.position);
				i.worldPos = mul(unity_ObjectToWorld, v.position);
				/*
					 // mul(IT_M, norm) => mul(norm, I_M) => {dot(norm, I_M.col0), dot(norm, I_M.col1), dot(norm, I_M.col2)}
    				return normalize(mul(norm, (float3x3)unity_WorldToObject));

    				是为了解决因为当曲面沿一个纬度拉伸时，其法线不会以相同的方式拉伸。需要使用逆转置矩阵
				*/
				i.normal = UnityObjectToWorldNormal(v.normal); //法线需要逆转置矩阵
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return i;
			}

			float4 MyFragmentProgram (Interpolators i) : SV_TARGET {
				i.normal = normalize(i.normal);
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

				float3 lightColor = _LightColor0.rgb;
				float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;

				float3 specularTint;
				float oneMinusReflectivity;
				albedo = DiffuseAndSpecularFromMetallic(
					albedo, _Metallic, specularTint, oneMinusReflectivity
				);


				//直接光
				UnityLight light;
				light.color = lightColor;
				light.dir = lightDir;
				light.ndotl = DotClamped(i.normal, lightDir);

				//间接光
				UnityIndirect indirectLight;
				indirectLight.diffuse = 0;
				indirectLight.specular = 0;

				return UNITY_BRDF_PBS(
					albedo, specularTint,
					oneMinusReflectivity, _Smoothness,
					i.normal, viewDir,
					light, indirectLight
				);
			}

			ENDCG
		}
	}
}