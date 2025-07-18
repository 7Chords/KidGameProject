Shader "KidGame/PlacePreview"
{
    Properties
    {
        _MainColor("Main Color", Color) = (0,1,0,0.5)
        _OccludedColor("Occluded Color", Color) = (1,0,0,0.7)
        _MainTex("Main Texture", 2D) = "white" {}
        _EdgeThickness("Edge Thickness", Range(0, 0.1)) = 0.02
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent+100"
                "RenderType" = "Transparent"
                "IgnoreProjector" = "True"
                "ForceNoShadowCasting" = "True"
            }

            // 正常渲染Pass（可见部分）
            Pass
            {
                Name "FORWARD"
                Tags { "LightMode" = "UniversalForward" }

                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite Off
                ZTest LEqual
                Cull Back

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float2 uv : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct Varyings
                {
                    float4 positionHCS : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                half4 _MainColor;

                Varyings vert(Attributes IN)
                {
                    Varyings OUT;
                    UNITY_SETUP_INSTANCE_ID(IN);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                    OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                    return OUT;
                }

                half4 frag(Varyings IN) : SV_Target
                {
                    half4 texColor = tex2D(_MainTex, IN.uv);
                    return texColor * _MainColor;
                }
                ENDHLSL
            }

            // 被遮挡部分渲染Pass
            Pass
            {
                Name "OCCLUDED"
                Tags { "LightMode" = "SRPDefaultUnlit" }

                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite Off
                ZTest Greater
                Cull Back

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct Varyings
                {
                    float4 positionHCS : SV_POSITION;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                half4 _OccludedColor;
                float _EdgeThickness;

                Varyings vert(Attributes IN)
                {
                    Varyings OUT;
                    UNITY_SETUP_INSTANCE_ID(IN);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                    // 沿法线方向稍微膨胀，确保轮廓可见
                    float3 pos = IN.positionOS.xyz + IN.normalOS * _EdgeThickness;
                    OUT.positionHCS = TransformObjectToHClip(pos);
                    return OUT;
                }

                half4 frag(Varyings IN) : SV_Target
                {
                    return _OccludedColor;
                }
                ENDHLSL
            }
        }

            FallBack "Hidden/Universal Render Pipeline/Transparent"
}