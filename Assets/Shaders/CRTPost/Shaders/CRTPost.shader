Shader "CRT/CRTPost"
{
    Properties
    {
        _ScanlineIntensity ("Scanline Intensity", Range(0,2)) = 1.0
        _ScanlineCount     ("Scanlines per Screen Height", Range(100,2000)) = 900
        _MaskStrength      ("RGB Mask Strength", Range(0,1)) = 0.35
        _Curvature         ("Barrel Curvature", Range(0,1)) = 0.25
        _CornerRadius      ("Corner Roundness", Range(0,1)) = 0.08
        _Vignette          ("Vignette Strength", Range(0,2)) = 0.75
        _Aberration        ("Chromatic Aberration", Range(0,3)) = 0.6
        _NoiseAmount       ("Subtle Noise", Range(0,0.2)) = 0.03
        _Jitter            ("Horizontal Jitter", Range(0,2)) = 0.15
        _TimeScale         ("Flicker/Jitter Speed", Range(0,5)) = 1.2
        _Gamma             ("Gamma", Range(0.5,3)) = 1.2
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "CRTPost"
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex   Vert
            #pragma fragment Frag

            // URP core + fullscreen triangle utilities
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            // 중요: 여기서 _BlitTexture / sampler 선언을 직접 하지 않는다.
            // Blit 경로에서 TEXTURE2D_X(_BlitTexture)와 sampler_LinearClamp가 제공됨.

            // Parameters
            float _ScanlineIntensity, _MaskStrength, _Curvature, _CornerRadius, _Vignette, _Aberration, _NoiseAmount, _Jitter, _TimeScale, _Gamma;
            float _ScanlineCount;

            // Unique structs to avoid name collisions
            struct CRTAttrib   { uint vertexID : SV_VertexID; };
            struct CRTVaryings { float4 positionHCS: SV_POSITION; float2 uv: TEXCOORD0; float2 uvNdc: TEXCOORD1; };

            CRTVaryings Vert (CRTAttrib v)
            {
                CRTVaryings o;
                o.positionHCS = GetFullScreenTriangleVertexPosition(v.vertexID);
                o.uv          = GetFullScreenTriangleTexCoord(v.vertexID);
                o.uvNdc       = o.uv * 2.0 - 1.0; // [-1,1]
                return o;
            }

            // Helpers
            float2 SrcTexelSize() { return 1.0 / _ScreenParams.xy; }
            float4 SampleSrc(float2 uv) { return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv); }

            float2 BarrelDistort(float2 uv, float k)
            {
                float2 cc = uv * 2.0 - 1.0;
                float r2 = dot(cc, cc);
                cc *= (1.0 + k * r2);
                return saturate((cc + 1.0) * 0.5);
            }

            float CornerMask(float2 uvNdc, float corner)
            {
                // 0(중앙) → 1(모서리)로 증가하는 값 m
                float2 a = abs(uvNdc);
                float m = max(a.x, a.y);
                // 경계가 (1-corner)에서 시작해 1에서 완전히 꺼지도록 스무스
                float edge = smoothstep(1.0 - corner, 1.0, m);
                return 1.0 - edge; // 중앙=1, 모서리=0
            }

            float3 ApertureMask(float2 uv, float strength)
            {
                float px = floor(uv.x * _ScreenParams.x);

                float stripe = fmod(px, 3.0);

                float3 mask = (stripe < 0.5) ? float3(1.0, 0.45, 0.45) :
                  (stripe < 1.5) ? float3(0.45, 1.0, 0.45) :
                                   float3(0.45, 0.45, 1.0);

                return lerp(float3(1.0,1.0,1.0), mask, saturate(strength));
            }

            float Scanline(float2 uv, float lines, float intensity)
            {
                float s = 0.5 + 0.5 * cos(uv.y * lines * 3.14159265);
                return lerp(1.0, s, intensity);
            }

            float3 SampleCA(float2 uv, float amt)
            {
                float2 d = (uv - 0.5) * SrcTexelSize() * (120.0 * amt);
                float3 c;
                c.r = SampleSrc(uv + d).r;
                c.g = SampleSrc(uv).g;
                c.b = SampleSrc(uv - d).b;
                return c;
            }

            float2 JitteredUV(float2 uv)
            {
                float t = _TimeParameters.x * _TimeScale;
                float n = frac(sin(dot(uv + t, float2(12.9898,78.233))) * 43758.5453);
                uv.x += (n - 0.5) * _Jitter * SrcTexelSize().x * 100.0;
                return uv;
            }

            float4 Frag (CRTVaryings i) : SV_Target
            {
                float2 uv = BarrelDistort(i.uv, _Curvature);

                // CRT look
                float2 uvJ = JitteredUV(uv);
                float3 col = SampleCA(uvJ, _Aberration);
                col *= Scanline(uvJ, _ScanlineCount, _ScanlineIntensity) * ApertureMask(uvJ, _MaskStrength);

                // Noise
                float t = _TimeParameters.x * _TimeScale;
                col += (frac(sin(dot(uv + t, float2(127.1,311.7)) * 43758.5453)) - 0.5) * _NoiseAmount;

                // Post shaping
                col = pow(saturate(col), 1.0 / _Gamma);

                // Masks
                float corner = CornerMask(i.uvNdc, _CornerRadius);
                float vign   = 1.0 - _Vignette * smoothstep(0.3, 1.0, length(i.uvNdc));
                col *= corner * vign;

                return float4(col, 1);
            }
            ENDHLSL
        }
    }
}
