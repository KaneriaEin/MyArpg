using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using UnityEngine.Rendering.PostProcessing;
using static UnityEditor.AddressableAssets.Build.Layout.BuildLayout;
using UnityEditor;

public class PostProcessingManager : SingletonMono<PostProcessingManager>
{
    [SerializeField] private PostProcessVolume postProcessVolume;
    [SerializeField] private PostProcessProfile postProcessProfile;

    #region 完美闪避效果
    public void SetPerfectDodgeEffect()
    {
        // 设置对应参数
        // 设置Vignette效果
        if (postProcessProfile.TryGetSettings(out Vignette vignette))
        {
            vignette.mode.Override(VignetteMode.Classic);
            vignette.intensity.Override(0.254f); // default:0
            vignette.smoothness.Override(1f);
            vignette.roundness.Override(1f);
            vignette.enabled.Override(true);
        }

        // 设置AmbientOcclusion效果
        if (postProcessProfile.TryGetSettings(out AmbientOcclusion ambientOcclusion))
        {
            ambientOcclusion.intensity.Override(1.6f); // default:0
            ambientOcclusion.enabled.Override(true);
        }

        // 设置Bloom效果
        if (postProcessProfile.TryGetSettings(out Bloom bloom))
        {
            bloom.intensity.Override(1.5f); // default:0
            bloom.enabled.Override(true);
        }

        // 设置motionBlur效果
        if (postProcessProfile.TryGetSettings(out MotionBlur motionBlur))
        {
            motionBlur.shutterAngle.Override(270f);
            motionBlur.sampleCount.Override(20);
            motionBlur.enabled.Override(true);
        }

        // 设置LensDistortion效果
        if (postProcessProfile.TryGetSettings(out LensDistortion lensDistortion))
        {
            lensDistortion.intensity.Override(-10f); // default:0
            lensDistortion.enabled.Override(true);
        }

        // 设置DepthOfField效果
        if (postProcessProfile.TryGetSettings(out DepthOfField depthOfField))
        {
            depthOfField.focusDistance.Override(5f); // default:10
            depthOfField.aperture.Override(0.3f); // default:5.6
            depthOfField.focalLength.Override(17f); // default:50
            depthOfField.enabled.Override(true);
        }
    }

    public void RemovePerfectDodgeEffect()
    {
        // 设置对应参数
        // 设置Vignette效果
        if (postProcessProfile.TryGetSettings(out Vignette vignette))
        {
            vignette.enabled.Override(false);
        }

        // 设置AmbientOcclusion效果
        if (postProcessProfile.TryGetSettings(out AmbientOcclusion ambientOcclusion))
        {
            ambientOcclusion.enabled.Override(false);
        }

        // 设置Bloom效果
        if (postProcessProfile.TryGetSettings(out Bloom bloom))
        {
            bloom.enabled.Override(false);
        }

        // 设置motionBlur效果
        if (postProcessProfile.TryGetSettings(out MotionBlur motionBlur))
        {
            motionBlur.enabled.Override(false);
        }

        // 设置LensDistortion效果
        if (postProcessProfile.TryGetSettings(out LensDistortion lensDistortion))
        {
            lensDistortion.enabled.Override(false);
        }

        // 设置DepthOfField效果
        if (postProcessProfile.TryGetSettings(out DepthOfField depthOfField))
        {
            depthOfField.enabled.Override(false);
        }
    }
    #endregion
}
