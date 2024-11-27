Shader "PolyArtMaskTintFixed"
{
    Properties
    {
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0.0
        _Color01("Color01", Color) = (0.7205882, 0.08477508, 0.08477508, 1)
        _Color02("Color02", Color) = (0.02649222, 0.3602941, 0.09785674, 1)
        _Color03("Color03", Color) = (0.07628676, 0.2567445, 0.6102941, 1)
        _Color04("Color04", Color) = (0.6207737, 0.1119702, 0.8014706, 1)
        _PolyArtAlbedo("PolyArtAlbedo", 2D) = "white" {}
        _Mask01("Mask01", 2D) = "white" {}
        _Mask02("Mask02", 2D) = "white" {}
        _Color01Power("Color01Power", Range(0, 6)) = 1
        _Color02Power("Color02Power", Range(0, 6)) = 1
        _Color03Power("Color03Power", Range(0, 6)) = 1
        _Color04Power("Color04Power", Range(0, 6)) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" "RenderType" = "Opaque" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" "Queue" = "Overlay" }
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Properties
            TEXTURE2D(_PolyArtAlbedo);
            SAMPLER(sampler_PolyArtAlbedo);
            TEXTURE2D(_Mask01);
            SAMPLER(sampler_Mask01);
            TEXTURE2D(_Mask02);
            SAMPLER(sampler_Mask02);

            float4 _Color01;
            float4 _Color02;
            float4 _Color03;
            float4 _Color04;

            float _Color01Power;
            float _Color02Power;
            float _Color03Power;
            float _Color04Power;

            float _Metallic;
            float _Smoothness;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.worldPos = TransformObjectToWorld(v.positionOS);
                o.worldNormal = normalize(mul(v.normalOS, (float3x3)unity_WorldToObject));
                o.uv = v.uv;
                return o;
            }

            // Input Data Structure
            struct IData
            {
                half3 worldPos;
                half3 worldNormal;
                half3 viewDir;
            };

            void InitializeInputData(Varyings v, inout IData inputData)
            {
                inputData.worldPos = v.worldPos;
                inputData.worldNormal = normalize(v.worldNormal);
                inputData.viewDir = normalize(_WorldSpaceCameraPos - v.worldPos);
            }

            half4 SampleMasks(float2 uv)
            {
                half4 mask1 = SAMPLE_TEXTURE2D(_Mask01, sampler_Mask01, uv);
                half4 mask2 = SAMPLE_TEXTURE2D(_Mask02, sampler_Mask02, uv);
                return mask1 + mask2;
            }

            // URP Lighting Function
            half3 CalculateLighting(IData inputData)
            {
                // Convert worldPos to float4 (add w-component)
                float4 worldPos4 = float4(inputData.worldPos, 1.0);

                // Get the main directional light using URP's built-in function
                Light mainLight = GetMainLight(worldPos4); // Pass the float4 world position

                // Compute lighting based on light direction and surface normal
                half3 lightDir = normalize(mainLight.direction);
                half NdotL = max(0.0, dot(inputData.worldNormal, lightDir));

                // Diffuse reflection using the light intensity and normal
                half3 diffuse = mainLight.color.rgb * NdotL;

                // Get the ambient light from the environment (URP)
                half3 ambient = unity_AmbientSky.rgb; // Using URP's ambient light

                return diffuse + ambient; // Combine diffuse and ambient lighting
            }

            half4 frag(Varyings i) : SV_Target
            {
                // Sample the albedo texture
                half4 albedo = SAMPLE_TEXTURE2D(_PolyArtAlbedo, sampler_PolyArtAlbedo, i.uv);
                half4 masks = SampleMasks(i.uv);

                // Combine colors based on masks and power
                half3 finalColor = 0;
                finalColor += masks.r * _Color01.rgb * _Color01Power;
                finalColor += masks.g * _Color02.rgb * _Color02Power;
                finalColor += masks.b * _Color03.rgb * _Color03Power;
                finalColor += masks.a * _Color04.rgb * _Color04Power;

                // Add additional blending
                finalColor = lerp(albedo.rgb, finalColor, 0.5);

                // Compute lighting
                IData inputData;
                InitializeInputData(i, inputData);

                half3 lighting = CalculateLighting(inputData);

                // Apply metallic and smoothness using URP's lighting model
                half3 specular = lighting * _Smoothness;
                finalColor = (finalColor + specular) * (1.0 - _Metallic);

                return half4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }
}