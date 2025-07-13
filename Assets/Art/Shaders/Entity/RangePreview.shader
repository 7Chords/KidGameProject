Shader "KidGame/RangePreview"
{
    Properties
    {
        _MainColor("Fill Color (RGB)", Color) = (0.2, 0.6, 1, 1) // Fixed RGB
        _BaseAlpha("Base Transparency", Range(0.1, 0.4)) = 0.2 // Base alpha
        _PulseStrength("Pulse Strength", Range(0.05, 0.5)) = 0.25 // Alpha variation amount
        _PulseSpeed("Pulse Speed", Range(0.5, 3)) = 1.5 // Cycles per second

        _BorderColor("Edge Color (RGB)", Color) = (0.8, 1, 1, 1)
        _BorderAlpha("Edge Transparency", Range(0.6, 1)) = 0.8

        _Range("Range Radius", Float) = 5.0
        _BorderThickness("Edge Thickness", Float) = 0.2
    }

        SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _MainColor;
            float _BaseAlpha;
            float _PulseStrength;
            float _PulseSpeed;

            float4 _BorderColor;
            float _BorderAlpha;

            float _Range;
            float _BorderThickness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float3 positionWS   : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionHCS = TransformWorldToHClip(output.positionWS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Trap center in world space
                float3 trapCenterWS = TransformObjectToWorld(float3(0,0,0));
                float distanceToCenter = distance(input.positionWS, trapCenterWS);

                // Check if within range
                if (distanceToCenter > _Range)
                    discard;

                // Alpha pulse calculation (sine wave for smooth variation)
                float alphaPulse = 0.5 + 0.5 * sin(_Time.y * _PulseSpeed); // 0.5~1.5 range
                float currentAlpha = _BaseAlpha * (1 + alphaPulse * _PulseStrength);

                // Border detection
                bool isBorder = distanceToCenter > _Range - _BorderThickness;

                // Base fill color with pulsing alpha
                half4 fillColor = half4(_MainColor.rgb, currentAlpha);

                // Enhance border with separate pulse (subtler)
                if (isBorder)
                {
                    float borderPulse = 0.5 + 0.5 * sin(_Time.y * _PulseSpeed);
                    float borderCurrentAlpha = _BorderAlpha * (1 + borderPulse * _PulseStrength * 0.5);
                    fillColor = half4(_BorderColor.rgb, borderCurrentAlpha);
                }

                return fillColor;
            }
            ENDHLSL
        }
    }
        FallBack "Hidden/Universal Render Pipeline/FallbackError"
}