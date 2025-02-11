﻿Shader "UI/CirCleProgressBar"
{
    Properties
    {
        [hideinInspector]
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Angle("Angle", range(0,361)) = 360
        _Center("Center", vector) = (.5,.5,0,0)
        [hideinInspector]
        _Width("Width", float) = 1
    }

        SubShader
            {
                Tags
                {
                    "Queue" = "Transparent"
                    "IgnoreProjector" = "True"
                    "RenderType" = "Transparent"
                    "PreviewType" = "Plane"
                    "CanUseSpriteAtlas" = "True"
                }


                Cull Off
                Lighting Off
                ZWrite Off
                ZTest[unity_GUIZTestMode]
                Blend SrcAlpha OneMinusSrcAlpha

                Pass
                {
                    CGPROGRAM
                        #pragma vertex vert
                        #pragma fragment frag
                        #include "UnityCG.cginc"

                        float _Angle;
                        float4 _Center;
                        half _Width;

                        struct appdata_t
                        {
                            float4 vertex   : POSITION;
                            float4 color    : COLOR;
                            float2 texcoord : TEXCOORD0;
                        };

                        struct v2f
                        {
                            float4 vertex   : SV_POSITION;
                            fixed4 color : COLOR;
                            half2 texcoord  : TEXCOORD0;
                        };

                        fixed4 _Color;
                        sampler2D _MainTex;
                        float4 _MainTex_ST;
                        float4 _UVRect;
                        float4 _UVScale;

                        v2f vert(appdata_t IN)
                        {
                            v2f OUT;
                            OUT.vertex = UnityObjectToClipPos(IN.vertex);

                            //非运行时，_MainTex 是sprite对应的texture, texcoord对应的就是原始sprite的uv坐标
                            //运行时，MainTex 是sprite对应图集的texture,texcoord对应的就是图集的uv坐标
                            float2 texcoord = TRANSFORM_TEX(IN.texcoord,_MainTex);
                            OUT.texcoord = texcoord;
                            OUT.color = IN.color * _Color;
                            return OUT;
                        }


                        fixed4 frag(v2f IN) : SV_Target
                        {

                            half4 color = tex2D(_MainTex, IN.texcoord)* IN.color;
                            float2 pos = IN.texcoord - _Center;
                            float ang = degrees(atan2(pos.x, -pos.y)) + 180;

                            _Angle = 360 - _Angle;
                            color.a = color.a * saturate((ang - _Angle) / _Width);
                            return color;
                        }
                    ENDCG
                 }
            }
}
