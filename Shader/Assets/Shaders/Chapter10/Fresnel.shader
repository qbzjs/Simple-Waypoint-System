﻿Shader "Shaders/Chapter10/Fresnel"
{
	//菲涅尔效应
	Properties
	{
		_Color ("Color Tint", Color) = (1, 1, 1, 1)
		_FresnelScale ("Fresnel Scale", Range(0, 1)) = 0.5
		_Cubemap ("Reflection Cubemap", Cube) = "_Skybox" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry"}
		Pass
		{
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM
			
			#pragma multi_compile_fwdbase

			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			fixed4 _Color;
			fixed _FresnelScale;
			samplerCUBE _Cubemap;

			struct a2v{
				float4 vertex: POSITION;
				float3 normal: NORMAL;
			};

			struct v2f{
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
  				fixed3 worldNormal : TEXCOORD1;
  				fixed3 worldViewDir : TEXCOORD2;
  				fixed3 worldRefl : TEXCOORD3;
 	 			SHADOW_COORDS(4)
			};

			v2f vert(a2v v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.worldViewDir = UnityWorldSpaceViewDir(o.worldPos);

				//计算反射向量 reflect(-视线方向，法线方向)
				o.worldRefl = reflect(-o.worldViewDir, o.worldNormal);

				//使用TRANSFER_SHADOW 注意：
					// 1 必须保证a2v中顶点坐标名为vertex 
					// 2 顶点着色器的输入形参名必须为v
					// 3 v2f的顶点变量名必须为pos

					//总结下：a2v中必须要有vertex表示顶点位置 v2f中必须有pos表是裁剪空间的位置 形参必须得是v

				TRANSFER_SHADOW(o);
				return o;

			}

			fixed4 frag(v2f o): SV_Target{
				fixed3 worldNormal = normalize(o.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(o.worldPos));	
				fixed3 worldViewDir = normalize(o.worldViewDir);

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				
				fixed3 diffuse = _LightColor0.rgb * _Color.rgb * max(0, dot(worldNormal, worldLightDir));


				// Use the reflect dir in world space to access the cubemap
				fixed3 reflection = texCUBE(_Cubemap, o.worldRefl).rgb;
				
				fixed fresnel = _FresnelScale + (1 - _FresnelScale) * pow(1 - dot(worldViewDir, worldNormal), 5);

				//计算衰减以及阴影值
				UNITY_LIGHT_ATTENUATION(atten, o, o.worldPos);

				// Mix the diffuse color with the reflected color
				fixed3 color = ambient + lerp(diffuse, reflection, saturate(fresnel)) * atten;

				return fixed4(color, 1.0);

			}
			
			ENDCG
		}
	}
	FallBack "Reflective/VertexLit"
}
