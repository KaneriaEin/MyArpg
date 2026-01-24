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

    [SerializeField] private Renderer _renderer;
    private Material _materialInstance;
    private Coroutine _hitCoroutine;

    private GameCharacter_Controller gameCharacter;
    public void Init(GameCharacter_Controller character)
    {
        // 创建材质实例（避免影响其他对象）
        _materialInstance = new Material(rimLightMaterial);

        // 复制原有材质属性
        CopyOriginalMaterialProperties();

        // 初始化参数
        UpdateRimParameters();

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

    void UpdateRimParameters()
    {
        _materialInstance.SetColor("_RimColor", rimColor);
        _materialInstance.SetFloat("_RimPower", rimPower);
        _materialInstance.SetFloat("_RimIntensity", 0f);
        _materialInstance.SetFloat("_HitStrength", 0f);
    }

    // 触发受击效果
    public void TriggerHit(float time)
    {
        if (_hitCoroutine != null)
            StopCoroutine(_hitCoroutine);

        _hitCoroutine = StartCoroutine(HitEffectRoutine(time));
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

        _materialInstance.SetFloat("_RimIntensity", rimIntensity);
        _materialInstance.SetFloat("_HitStrength", hitStrength);
        _materialInstance.SetColor("_RimColor", rimColor);

        yield return new WaitForSeconds(time);

        // 恢复
        UpdateRimParameters();
    }


    // 调试用
    [ContextMenu("测试受击效果")]
    void TestHit()
    {
        //TriggerHit(5f);
    }
}
