Shader "Custom/RimLightStandardFade"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        
        // Rim Light 属性
        _RimColor ("Rim Color", Color) = (0, 0.5, 1, 1)
        _RimPower ("Rim Power", Range(0.1, 10)) = 3.0
        _RimIntensity ("Rim Intensity", Range(0, 10)) = 1.0
        _HitStrength ("Hit Strength", Range(0, 1)) = 0.0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldNormal;
        };
        
        sampler2D _MainTex;
        float4 _Color;
        float _Metallic;
        float _Smoothness;
        
        // Rim Light
        float4 _RimColor;
        float _RimPower;
        float _RimIntensity;
        float _HitStrength;
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // 基础材质
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = c.a;
            
            // 计算边缘光
            float rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            rim = pow(rim, _RimPower) * _RimIntensity;
            
            // 受击时增强
            rim *= (1.0 + _HitStrength * 2.0);
            
            // 应用到自发光
            o.Emission = _RimColor.rgb * rim;
            // _RimColor.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
