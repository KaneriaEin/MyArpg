using JKFrame;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : SingletonMono<PostProcessingManager>
{
    [SerializeField] private PostProcessVolume postProcessVolume;
    [SerializeField] private PostProcessProfile postProcessProfile;

    public void Init()
    {
        radialBlurEffect = postProcessProfile.GetSetting<RadialBlur>();
        radialBlurEffect.blurStrength.value = 0;
        isPulsing = false;

    }

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

    #region RadialBlur相关
    [Header("径向模糊引用")]
    public RadialBlur radialBlurEffect;

    public bool isPulsing = false;
    private float localTimeScale = 1f;
    public float LocalTimeScale { set { localTimeScale = value; }  get { return localTimeScale; } }



    public void SetRadialBlurTimeScale(float timescale)
    {
        localTimeScale = timescale;
    }

    public void TriggerRadialBlur(float risetime, float holdtime, float falltime, Vector2 screenCenter = default, float strength = 0.5f)
    {
        if (isPulsing) return;
        if (screenCenter == default) screenCenter = new Vector2(0.5f, 0.5f);

        if (radialBlurEffect != null)
        {
            radialBlurEffect.center.value = screenCenter;
        }

        StartCoroutine(PulsedRadialBlur(risetime, holdtime, falltime, strength));
    }

    public void TriggerRadialBlur(Vector2? screenCenter = null)
    {
        if (isPulsing) return;

        if (screenCenter.HasValue && radialBlurEffect != null)
        {
            radialBlurEffect.center.value = screenCenter.Value;
        }

        StartCoroutine(PulsedRadialBlur(0.01f, 0.01f, 0.1f));
    }
    /// <summary>
    /// 触发径向模糊
    /// </summary>
    /// <param name="risetime">上升时间</param>
    /// <param name="holdtime">保持时间</param>
    /// <param name="falltime">下降时间</param>
    /// <returns></returns>
    public IEnumerator PulsedRadialBlur(float risetime, float holdtime, float falltime, float strength = 0.25f)
    {
        isPulsing = true;

        // 第一阶段：快速增强 (0 → 1)
        float riseTime = risetime;   // 上升时间
        float holdTime = holdtime;   // 保持时间
        float fallTime = falltime;   // 下降时间

        // 阶段1：0 → 1 (0.05秒)
        for (float t = 0; t < riseTime; t += Time.deltaTime * localTimeScale)
        {
            radialBlurEffect.blurStrength.value = Mathf.Lerp(0, strength, t / riseTime);
            yield return null;
        }
        radialBlurEffect.blurStrength.value = strength; // 确保达到峰值

        // 阶段2：保持峰值 (0.1秒)
        float holdTimer = 0;
        while (holdTimer < holdTime)
        {
            holdTimer += Time.deltaTime * localTimeScale;
            radialBlurEffect.blurStrength.value = strength; // 维持最大值
            yield return null;
        }

        // 阶段3：1 → 0 (0.05秒)
        for (float t = 0; t < fallTime; t += Time.deltaTime * localTimeScale)
        {
            radialBlurEffect.blurStrength.value = Mathf.Lerp(strength, 0, t / fallTime);
            yield return null;
        }
        radialBlurEffect.blurStrength.value = 0; // 确保回到0
        isPulsing = false;
    }
    public IEnumerator PulsedRadialBlur()
    {
        isPulsing = true;
        // 第一阶段：快速增强 (0 → 1)
        float riseTime = 0.01f;  // 上升时间
        float holdTime = 0.01f;   // 保持时间
        float fallTime = 0.1f;  // 下降时间

        // 阶段1：0 → 1 (0.01秒)
        for (float t = 0; t < riseTime; t += Time.deltaTime * localTimeScale)
        {
            radialBlurEffect.blurStrength.value = Mathf.Lerp(0, 0.25f, t / riseTime);
            yield return null;
        }
        radialBlurEffect.blurStrength.value = 0.25f; // 确保达到峰值

        // 阶段2：保持峰值 (0.01秒)
        float holdTimer = 0;
        while (holdTimer < holdTime)
        {
            holdTimer += Time.deltaTime * localTimeScale;
            radialBlurEffect.blurStrength.value = 0.25f; // 维持最大值
            yield return null;
        }

        // 阶段3：1 → 0 (0.1秒)
        for (float t = 0; t < fallTime; t += Time.deltaTime * localTimeScale)
        {
            radialBlurEffect.blurStrength.value = Mathf.Lerp(0.25f, 0, t / fallTime);
            yield return null;
        }
        radialBlurEffect.blurStrength.value = 0; // 确保回到0
        isPulsing = false;
    }
    #endregion
}

public class RadialBlurConfig
{
    public bool Enable = false;
    public float RiseTime = 0.01f;
    public float HoldTime = 0.01f;
    public float FallTime = 0.1f;
}
