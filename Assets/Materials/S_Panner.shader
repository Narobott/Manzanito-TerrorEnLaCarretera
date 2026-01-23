Shader "Custom/TexturePanner2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PanDirection ("Pan Direction (XY)", Vector) = (1, 0, 0, 0)
        _PanSpeed ("Pan Speed", Float) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }

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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float2 _PanDirection;
                float _PanSpeed;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 pannedUV = IN.uv + (_PanDirection * _PanSpeed * _Time.y);
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pannedUV);
            }
            ENDHLSL
        }
    }
}
