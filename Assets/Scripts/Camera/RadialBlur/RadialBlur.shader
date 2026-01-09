Shader "Custom/RadialBlur"
{
HLSLINCLUDE
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float _BlurStrength;
    int _Samples;
    float2 _Center;
    float _Falloff;
    int _ZoomMode;
    
    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float4 color = 0;
        float2 dir = i.texcoord - _Center;
        float dist = length(dir);
        
        // 计算基于距离的强度
        float strength = _BlurStrength * pow(dist, _Falloff);
        
        for (int j = 0; j < _Samples; j++)
        {
            float scale = 1.0;
            
            if (_ZoomMode == 1) // 缩放模式
            {
                // 远离中心
                scale = 1.0 + strength * (j / float(_Samples - 1));
            }
            else // 旋转模式（简单模拟）
            {
                // 添加一些旋转偏移
                float angle = strength * (j / float(_Samples - 1)) * 0.1;
                float2x2 rot = float2x2(cos(angle), -sin(angle), sin(angle), cos(angle));
                float2 rotatedUV = _Center + mul(rot, dir);
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, rotatedUV);
                continue;
            }
            
            float2 sampleUV = _Center + dir * scale;
            color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, sampleUV);
        }
        
        color /= _Samples;
        return color;
    }
    ENDHLSL
    
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag
            ENDHLSL
        }
    }}
