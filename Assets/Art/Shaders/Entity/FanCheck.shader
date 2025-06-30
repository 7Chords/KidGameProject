Shader "KidGame/FanCheck"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _Angle("Angle", Range(0, 360)) = 90
        _Radius("Radius", Range(0, 1)) = 0.5
        _Smoothness("Smoothness", Range(0, 0.1)) = 0.02
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _Angle;
                float _Radius;
                float _Smoothness;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv * 2.0 - 1.0; // 将UV从[0,1]转换到[-1,1]
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 计算当前像素到中心的距离
                float distanceFromCenter = length(IN.uv);

            // 计算当前像素的角度（从正右方向开始，逆时针方向）
            float angle = atan2(IN.uv.y, IN.uv.x) * (180.0 / PI);
            angle = angle < 0 ? angle + 360 : angle; // 转换为0-360度范围

            // 计算扇形边界
            float halfAngle = _Angle * 0.5;
            float angleDiff = abs(angle - 180); // 以180度为中心

            // 扇形区域判断
            float inSector = (distanceFromCenter <= _Radius) && (angleDiff <= halfAngle);

            // 边缘平滑处理
            float radiusFade = smoothstep(_Radius, _Radius - _Smoothness, distanceFromCenter);
            float angleFade = smoothstep(halfAngle, halfAngle - (_Smoothness * 10), angleDiff);

            // 组合所有因素
            float sectorMask = radiusFade * angleFade;

            // 应用颜色和透明度
            half4 color = _Color;
            color.a *= sectorMask;

            return color;
        }
        ENDHLSL
    }
    }
}