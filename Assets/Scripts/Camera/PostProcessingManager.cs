using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using UnityEngine.Rendering.PostProcessing;
using static UnityEditor.AddressableAssets.Build.Layout.BuildLayout;

public class PostProcessingManager : SingletonMono<PostProcessingManager>
{
    [SerializeField] private PostProcessVolume postProcessVolume;
    [SerializeField] private PostProcessProfile postProcessProfile;

    public void PerfectDodgeEffect()
    {
        // TODO: ����ʱ���ɾ��effect����������

        // ���VignetteЧ��
        Vignette vignette = postProcessProfile.AddSettings<Vignette>();
        vignette.enabled.Override(true);
        vignette.mode.Override(VignetteMode.Classic);
        vignette.intensity.Override(0.254f); // default:0
        vignette.smoothness.Override(1f);
        vignette.roundness.Override(1f);

        // ���AmbientOcclusionЧ��
        AmbientOcclusion ambientOcclusion = postProcessProfile.AddSettings<AmbientOcclusion>();
        ambientOcclusion.enabled.Override(true);
        ambientOcclusion.intensity.Override(0.71f); // default:0

        // ���BloomЧ��
        Bloom bloom = postProcessProfile.AddSettings<Bloom>();
        bloom.enabled.Override(true);
        bloom.intensity.Override(1.5f); // default:0

        // ���motionBlurЧ��
        MotionBlur motionBlur = postProcessProfile.AddSettings<MotionBlur>();
        motionBlur.enabled.Override(true);
        motionBlur.shutterAngle.Override(270f);
        motionBlur.sampleCount.Override(20);

        // ���motionBlurЧ��
        LensDistortion lensDistortion = postProcessProfile.AddSettings<LensDistortion>();
        lensDistortion.enabled.Override(true);
        lensDistortion.intensity.Override(-10f); // default:0

        // ���motionBlurЧ��
        DepthOfField depthOfField = postProcessProfile.AddSettings<DepthOfField>();
        depthOfField.enabled.Override(true);
        depthOfField.focusDistance.Override(5f); // default:10
        depthOfField.aperture.Override(0.3f); // default:5.6
        depthOfField.focalLength.Override(17f); // default:50
    }

    public void RemoveAllEffect()
    {
        postProcessProfile.settings.Clear();
    }
}
