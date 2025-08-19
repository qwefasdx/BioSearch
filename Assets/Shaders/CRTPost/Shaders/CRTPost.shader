Shader "CRT/CRTPost"
{
    Properties
    {
        // 기본 CRT
        _ScanlineIntensity ("Scanline Intensity", Range(0,2)) = 1.0
        _ScanlineCount     ("Scanlines per Screen Height", Range(100,2000)) = 900
        _MaskStrength      ("RGB Mask Strength", Range(0,1)) = 0.35
        _MaskPixelSize     ("RGB Triad Pixel Size", Range(1,4)) = 1.0
        _Curvature         ("Barrel Curvature", Range(0,1)) = 0.25
        _CornerRadius      ("Corner Roundness", Range(0,1)) = 0.08
        _Vignette          ("Vignette Strength", Range(0,2)) = 0.75
        _Aberration        ("Chromatic Aberration", Range(0,3)) = 0.6
        _Gamma             ("Gamma", Range(0.5,3)) = 1.2

        // 가끔 지지직(버스트) 파라미터
        _JitterBase        ("Base Jitter",   Range(0,2))   = 0.02
        _JitterBurst       ("Burst Jitter",  Range(0,3))   = 0.18
        _FlickerBase       ("Base Flicker",  Range(0,0.2)) = 0.005
        _FlickerBurst      ("Burst Flicker", Range(0,0.3)) = 0.06
        _BurstLen          ("Glitch Burst Length (s)", Range(0.05,0.35)) = 0.14
        _BurstChance       ("Burst Chance per Segment", Range(0,0.5)) = 0.08
        _Seed              ("Glitch Seed", Range(0,100)) = 13
        _TimeScale         ("Flicker/Jitter Speed (Hz)", Range(0,5)) = 1.2

        // 롤링 밴드(Hum Bar)
        _BandStrength      ("Hum Bar Strength", Range(0,1)) = 0.25
        _BandThickness     ("Hum Bar Thickness", Range(0,0.5)) = 0.10
        _BandSpeed         ("Hum Bar Speed (-=up, +=down)", Range(-2,2)) = 0.20
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

            // URP core + fullscreen triangle
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            // ⚠ _BlitTexture / sampler_LinearClamp 선언 금지 (URP가 제공)

            // ---------------- Params ----------------
            float _ScanlineIntensity, _MaskStrength, _Curvature, _CornerRadius, _Vignette, _Aberration, _Gamma;
            float _ScanlineCount, _MaskPixelSize;

            float _JitterBase, _JitterBurst, _FlickerBase, _FlickerBurst;
            float _BurstLen, _BurstChance, _Seed, _TimeScale;

            float _BandStrength, _BandThickness, _BandSpeed;

            // ---------------- VS structs -------------
            struct CRTAttrib   { uint vertexID : SV_VertexID; };
            struct CRTVaryings { float4 positionHCS: SV_POSITION; float2 uv: TEXCOORD0; float2 uvNdc: TEXCOORD1; };

            CRTVaryings Vert (CRTAttrib v)
            {
                CRTVaryings o;
                o.positionHCS = GetFullScreenTriangleVertexPosition(v.vertexID);
                o.uv          = GetFullScreenTriangleTexCoord(v.vertexID);
                o.uvNdc       = o.uv * 2.0 - 1.0;
                return o;
            }

            // ---------------- Helpers ----------------
            static const float TAU = 6.2831853;
            float2 SrcTexelSize() { return 1.0 / _ScreenParams.xy; }
            float4 SampleSrc(float2 uv) { return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv); }

            float Hash11(float x) { return frac(sin(x * 12.9898 + 78.233) * 43758.5453); }

            // 확률적 버스트 게이트(세그먼트별로 on/off)
            float BurstEnvelope(float t)
            {
                float segLen = max(_BurstLen, 1e-3);
                float segId  = floor(t / segLen);
                float h      = Hash11(segId + _Seed);                // 0~1
                float inBurst= step(1.0 - saturate(_BurstChance), h); // 확률적으로 on

                float ph   = frac(t / segLen);                       // 0~1
                float env  = smoothstep(0.00, 0.25, ph) * (1.0 - smoothstep(0.75, 1.00, ph));
                float amp  = lerp(0.8, 1.2, Hash11(segId * 3.17 + _Seed * 1.11));

                return inBurst * env * amp; // 0~1.2
            }

            float2 BarrelDistort(float2 uv, float k)
            {
                float2 cc = uv * 2.0 - 1.0;
                float r2 = dot(cc, cc);
                cc *= (1.0 + k * r2);
                return saturate((cc + 1.0) * 0.5);
            }

            float CornerMask(float2 uvNdc, float corner)
            {
                float2 a = abs(uvNdc);
                float m = max(a.x, a.y);
                float edge = smoothstep(1.0 - corner, 1.0, m);
                return 1.0 - edge;
            }

            // 픽셀 고정 RGB triad (해상도/스케일 안정)
            float3 ApertureMask(float2 uv, float strength)
            {
                float px = floor(uv.x * (_ScreenParams.x / max(_MaskPixelSize, 1.0)));
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

            // 가끔만 흔들리는 지터 (버스트 시 세기↑)
            float2 JitteredUV(float2 uv, float t)
            {
                float burst = BurstEnvelope(t);
                float amp    = _JitterBase + _JitterBurst * burst;
                float freq   = max(_TimeScale, 1e-4);

                float baseWave = sin(t * TAU * freq);
                float wobble   = baseWave * (0.6 + 0.4 * sin(uv.y * 40.0 + burst * 3.14));

                uv.x += wobble * amp * SrcTexelSize().x * 60.0;
                return uv;
            }

            // 롤링 밴드(Hum Bar): 아래/위로 흐르는 밝기·채널 변화
            void ApplyHumBar(inout float3 col, float2 uv, float t)
            {
                float sp = _BandSpeed;
                float by = frac(t * abs(sp));     // 0~1 내려가는 기준
                by = (sp >= 0.0) ? by : (1.0 - by); // speed<0면 위로

                float dist = abs(uv.y - by);
                // 중심=1, 바깥=0로 부드러운 띠
                float m = 1.0 - smoothstep(_BandThickness*0.5, _BandThickness, dist);

                // 띠 내부에서만 살짝 채널 당김/밝기 보강
                float2 texel = SrcTexelSize();
                float shift = 1.25 * texel.x; // 매우 작은 수평 시프트
                float3 shifted;
                shifted.r = SampleSrc(uv + float2( shift, 0)).r;
                shifted.g = col.g;
                shifted.b = SampleSrc(uv + float2(-shift, 0)).b;

                float3 mixed = lerp(col, shifted * 1.08, m * _BandStrength);
                col = mixed;
            }

            float4 Frag (CRTVaryings i) : SV_Target
            {
                float t  = _TimeParameters.x;

                // 렌즈 왜곡
                float2 uv = BarrelDistort(i.uv, _Curvature);

                // 기본 CRT 룩
                float2 uvJ = JitteredUV(uv, t);
                float3 col = SampleCA(uvJ, _Aberration);
                col *= Scanline(uvJ, _ScanlineCount, _ScanlineIntensity) * ApertureMask(uvJ, _MaskStrength);

                // 플리커(밝기 요동): 버스트에만 강해짐
                float burst  = BurstEnvelope(t);
                float freq   = max(_TimeScale, 1e-4);
                float baseFlick =
                      0.63 * sin(t * TAU * (1.00 * freq) + 0.1)
                    + 0.27 * sin(t * TAU * (2.17 * freq) + 1.3)
                    + 0.10 * sin(t * TAU * (3.73 * freq) + 2.0);
                baseFlick *= 0.5;
                baseFlick *= (0.85 + 0.15 * sin(i.uv.y * 20.0));
                float flickAmp = _FlickerBase + _FlickerBurst * burst;
                col += baseFlick * flickAmp;

                // 롤링 밴드 적용(감마 전)
                ApplyHumBar(col, uv, t);

                // 감마 및 마스크
                col = pow(saturate(col), 1.0 / _Gamma);
                float corner = CornerMask(i.uvNdc, _CornerRadius);
                float vign   = 1.0 - _Vignette * smoothstep(0.3, 1.0, length(i.uvNdc));
                col *= corner * vign;

                return float4(col, 1);
            }
            ENDHLSL
        }
    }
}
