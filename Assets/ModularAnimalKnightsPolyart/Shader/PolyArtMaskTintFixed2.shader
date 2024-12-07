Shader "PolyArtMaskTintFixed2"
{
    Properties
    {
        _Smoothness("Smoothness", Range(0, 1)) = 0
        _Metallic("Metallic", Range(0, 1)) = 0
        _Color01("Color01", Color) = (0.7205882, 0.08477508, 0.08477508, 0)
        _Color02("Color02", Color) = (0.02649222, 0.3602941, 0.09785674, 0)
        _Color03("Color03", Color) = (0.07628676, 0.2567445, 0.6102941, 0)
        _Color04("Color04", Color) = (0.6207737, 0.1119702, 0.8014706, 0)
        _Color05("Color05", Color) = (0.9056604, 0.5051349, 0.09825563, 0)
        _Color06("Color06", Color) = (1, 0.7848822, 0, 0)
        _PolyArtAlbedo("PolyArtAlbedo", 2D) = "white" {}
        _Mask01("Mask01", 2D) = "white" {}
        _Mask02("Mask02", 2D) = "white" {}
        _Color01Power("Color01Power", Range(0, 6)) = 1
        _Color02Power("Color02Power", Range(0, 6)) = 1
        _Color03Power("Color03Power", Range(0, 6)) = 1
        _Color04Power("Color04Power", Range(0, 6)) = 1
        _Color05Power("Color05Power", Range(0, 6)) = 1
        _Color06Power("Color06Power", Range(0, 6)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        
        struct Input
        {
            float2 uv_PolyArtAlbedo;
            float2 uv_Mask01;
            float2 uv_Mask02;
        };

        sampler2D _PolyArtAlbedo;
        sampler2D _Mask01;
        sampler2D _Mask02;

        half _Metallic;
        half _Smoothness;

        float4 _Color01;
        float _Color01Power;
        float4 _Color02;
        float _Color02Power;
        float4 _Color03;
        float _Color03Power;
        float4 _Color04;
        float _Color04Power;
        float4 _Color05;
        float _Color05Power;
        float4 _Color06;
        float _Color06Power;

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 albedoTex = tex2D(_PolyArtAlbedo, IN.uv_PolyArtAlbedo);
            float4 mask1 = tex2D(_Mask01, IN.uv_Mask01);
            float4 mask2 = tex2D(_Mask02, IN.uv_Mask02);

            float4 blendedColor = 
                (min(mask1.r, _Color01) * _Color01Power) +
                (min(mask1.g, _Color02) * _Color02Power) +
                (min(mask1.b, _Color03) * _Color03Power) +
                (min(mask2.r, _Color04) * _Color04Power) +
                (min(mask2.g, _Color05) * _Color05Power) +
                (min(mask2.b, _Color06) * _Color06Power);

            // finalColor = lerp(albedoTex, saturate(albedoTex * blendedColor), 
             //                        mask1.r + mask1.g + mask1.b + mask2.r + mask2.g + mask2.b);

            float4 finalColor = (1, 1, 1, 1);
            o.Albedo = finalColor.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}