using System.Collections;
using UnityEngine;

public class RimLightController : MonoBehaviour
{
    [Header("材质设置")]
    public Material rimLightMaterial;  // 使用上面的shader

    [Header("边缘光参数")]
    public Color rimColor = new Color(0, 0.5f, 1f, 1f);
    [Range(0.1f, 10f)] public float rimPower = 3f;
    [Range(0f, 10f)] public float rimIntensity = 1f;

    [Header("受击效果")]
    public float hitDuration = 0.3f;
    public AnimationCurve hitCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("精防效果")]
    public Color pgRimColor = new Color(1, 0.67f, 0f, 1f);
    [Range(0.1f, 10f)] public float pgRimPower = 8f;
    [Range(0f, 10f)] public float pgRimIntensity = 0.62f;

    [SerializeField] private Renderer _renderer;
    private Material _materialInstance;
    private Coroutine _hitCoroutine;
    private Coroutine _pgCoroutine;

    private GameCharacter_Controller gameCharacter;
    public void Init(GameCharacter_Controller character)
    {
        // 创建材质实例（避免影响其他对象）
        _materialInstance = new Material(rimLightMaterial);

        // 复制原有材质属性
        CopyOriginalMaterialProperties();

        // 初始化参数
        ResetRimParameters();

        _renderer.material = _materialInstance;
        this.gameCharacter = character;
        this.gameCharacter.targetHitFreezeEvents += TriggerHit;

    }

    void CopyOriginalMaterialProperties()
    {
        Material original = _renderer.material;

        // 复制标准shader属性
        if (original.HasProperty("_Color"))
        {
            _materialInstance.SetColor("_Color", original.GetColor("_Color"));
            //Debug.Log($"original_Color = {original.GetColor("_Color")}");
        }

        if (original.HasProperty("_MainTex"))
        {
            _materialInstance.SetTexture("_MainTex", original.GetTexture("_MainTex"));
            //Debug.Log($"original_MainTex = {original.GetTexture("_MainTex")}");
        }

        if (original.HasProperty("_Metallic"))
        {
            _materialInstance.SetFloat("_Metallic", original.GetFloat("_Metallic"));
            //Debug.Log($"original_Metallic = {original.GetFloat("_Metallic")}");
        }

        if (original.HasProperty("_Glossiness"))
        {
            _materialInstance.SetFloat("_Smoothness", original.GetFloat("_Glossiness"));
            //Debug.Log($"original_Smoothness = {original.GetFloat("_Glossiness")}");
        }
    }

    void ResetRimParameters()
    {
        _materialInstance.SetColor("_RimColor", rimColor);
        _materialInstance.SetFloat("_RimPower", 0);
        _materialInstance.SetFloat("_RimIntensity", 0f);
        _materialInstance.SetFloat("_HitStrength", 0f);
    }

    /// <summary>
    /// 触发受击效果
    /// </summary>
    /// <param name="time"></param>
    public void TriggerHit(float time)
    {
        if (_hitCoroutine != null)
            StopCoroutine(_hitCoroutine);

        _hitCoroutine = StartCoroutine(HitEffectRoutine(time));
    }
    
    /// <summary>
    /// 触发精防时的边缘光效果
    /// </summary>
    /// <param name="time"></param>
    public void TriggerPerfectGuard(float time, float disappear‌Time)
    {
        if (_pgCoroutine != null)
            StopCoroutine(_pgCoroutine);

        _pgCoroutine = StartCoroutine(PGEffectRoutine(time, disappear‌Time));
    }

    /// <summary>
    /// 开关边缘光
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator HitEffectRoutine(float time)
    {
        /// 变量解释
        /// 实际控制开关亮度的就是rimIntensity，线性控制亮度
        /// _RimPower 控制亮度的宽度，如皮肤边缘光的厚薄，幂次计算
        /// hitStrength只是一个代表增强效果的量，取值>1，和rimIntensity相同，线性控制

        float curveValue = hitCurve.Evaluate(0);

        float hitStrength = curveValue;

        _materialInstance.SetFloat("_RimPower", rimPower);
        _materialInstance.SetFloat("_RimIntensity", rimIntensity);
        _materialInstance.SetFloat("_HitStrength", hitStrength);
        _materialInstance.SetColor("_RimColor", rimColor);

        yield return new WaitForSeconds(time);

        // 恢复
        ResetRimParameters();
    }

    /// <summary>
    /// 开关精防边缘光
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator PGEffectRoutine(float duration, float disappear‌Time)
    {
        /// 变量解释
        /// 实际控制开关亮度的就是rimIntensity，线性控制亮度
        /// _RimPower 控制亮度的宽度，如皮肤边缘光的厚薄，幂次计算
        /// hitStrength只是一个代表增强效果的量，取值>1，和rimIntensity相同，线性控制

        _materialInstance.SetFloat("_RimPower", pgRimPower);
        _materialInstance.SetFloat("_RimIntensity", pgRimIntensity);
        _materialInstance.SetFloat("_HitStrength", 1);
        _materialInstance.SetColor("_RimColor", pgRimColor);

        yield return new WaitForSeconds(duration);

        // 逐帧恢复
        float intensity = pgRimIntensity;
        for (float t = 0; t < disappear‌Time; t += Time.deltaTime)
        {
            intensity = Mathf.Lerp(pgRimIntensity, 0, t / disappear‌Time);
            _materialInstance.SetFloat("_RimIntensity", intensity);
            yield return null;
        }
        ResetRimParameters();
    }

    // 调试用
    [ContextMenu("测试受击效果")]
    void TestTriggerHit()
    {
        TriggerHit(1f);
    }

    // 调试用
    [ContextMenu("测试精防效果")]
    void TestTriggerPerfectGuard()
    {
        TriggerPerfectGuard(5f, 0.5f);
    }
}
