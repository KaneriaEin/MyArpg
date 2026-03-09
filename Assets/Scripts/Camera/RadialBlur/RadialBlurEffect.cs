using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System;

[Serializable]
[PostProcess(typeof(RadialBlurRenderer), PostProcessEvent.AfterStack, "Custom/RadialBlur")]
public sealed class RadialBlur : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("模糊强度")]
    public FloatParameter blurStrength = new FloatParameter { value = 0.5f };

    [Range(1, 100), Tooltip("采样次数（影响质量）")]
    public IntParameter samples = new IntParameter { value = 10 };

    [Tooltip("模糊中心点 (0-1)")]
    public Vector2Parameter center = new Vector2Parameter { value = new Vector2(0.5f, 0.5f) };

    [Range(0f, 5f), Tooltip("距离衰减")]
    public FloatParameter falloff = new FloatParameter { value = 1.0f };

    [Tooltip("模糊模式")]
    public BoolParameter zoomMode = new BoolParameter { value = true };
}

public sealed class RadialBlurRenderer : PostProcessEffectRenderer<RadialBlur>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Custom/RadialBlur"));

        sheet.properties.SetFloat("_BlurStrength", settings.blurStrength);
        sheet.properties.SetInt("_Samples", settings.samples);
        sheet.properties.SetVector("_Center", settings.center);
        sheet.properties.SetFloat("_Falloff", settings.falloff);
        sheet.properties.SetInt("_ZoomMode", settings.zoomMode ? 1 : 0);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}