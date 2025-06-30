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
                OUT.uv = IN.uv * 2.0 - 1.0; // ��UV��[0,1]ת����[-1,1]
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // ���㵱ǰ���ص����ĵľ���
                float distanceFromCenter = length(IN.uv);

            // ���㵱ǰ���صĽǶȣ������ҷ���ʼ����ʱ�뷽��
            float angle = atan2(IN.uv.y, IN.uv.x) * (180.0 / PI);
            angle = angle < 0 ? angle + 360 : angle; // ת��Ϊ0-360�ȷ�Χ

            // �������α߽�
            float halfAngle = _Angle * 0.5;
            float angleDiff = abs(angle - 180); // ��180��Ϊ����

            // ���������ж�
            float inSector = (distanceFromCenter <= _Radius) && (angleDiff <= halfAngle);

            // ��Եƽ������
            float radiusFade = smoothstep(_Radius, _Radius - _Smoothness, distanceFromCenter);
            float angleFade = smoothstep(halfAngle, halfAngle - (_Smoothness * 10), angleDiff);

            // �����������
            float sectorMask = radiusFade * angleFade;

            // Ӧ����ɫ��͸����
            half4 color = _Color;
            color.a *= sectorMask;

            return color;
        }
        ENDHLSL
    }
    }
}