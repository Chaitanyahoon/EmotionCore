Shader "Hidden/DigitalGlitch"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Intensity ("Bypass", Float) = 0
        _ColorSplit ("Color Split", Float) = 0
    }
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform float _Intensity;
            uniform float _ColorSplit;
            uniform float _TimeX;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (_Intensity == 0) return tex2D(_MainTex, i.uv);

                float2 uv = i.uv;
                
                // Scanline Jitter
                float glitchTime = _TimeX * 10;
                float jitter = rand(float2(uv.y, glitchTime)) * 2 - 1;
                jitter *= step(0.98, rand(float2(uv.y * 5, glitchTime))); // Threshold
                uv.x += jitter * _Intensity * 0.1;

                // Color Split (Chromatic Aberration)
                float split = _ColorSplit * _Intensity * 0.05;
                
                half4 r = tex2D(_MainTex, uv + float2(split, 0));
                half4 g = tex2D(_MainTex, uv);
                half4 b = tex2D(_MainTex, uv - float2(split, 0));

                return half4(r.r, g.g, b.b, 1);
            }
            ENDCG
        }
    }
    Fallback Off
}
