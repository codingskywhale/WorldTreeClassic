Shader "Unlit/GradientShader"
{
     Properties
    {
        _Color1 ("Color 1", Color) = (0.88,0.96,0.99,1) // #E1F5FE
        _Color2 ("Color 2", Color) = (0.51,0.83,0.98,1) // #81D4FA
        _Speed ("Speed", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _Speed;
            float4 _Color1;
            float4 _Color2;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float lerpValue = (sin(_Time.y * _Speed * 2.0) + 1.0) / 2.0; // 시간에 따라 일렁이는 값 생성
                return lerp(_Color1, _Color2, lerpValue);
            }
            ENDCG
        }
    }
}