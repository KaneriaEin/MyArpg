using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class CharacterProperties : SerializedMonoBehaviour
{
    [ShowInInspector] public float currentHP;
    [ShowInInspector] public float currentMP;
    [ShowInInspector] public float currentSP;
    [ShowInInspector] public float currentULT;
    [ShowInInspector] public float currentStun;
    [ShowInInspector] public float currentThunderDebuff;
    [ShowInInspector] public float currentThunderExplo;
    [ShowInInspector] public bool enterStun;
    public FloatProperties maxHp = new FloatProperties();
    public FloatProperties maxMp = new FloatProperties();
    public FloatProperties maxSp = new FloatProperties();
    public FloatProperties maxUlt = new FloatProperties();
    public FloatProperties atk = new FloatProperties();
    public TimeCategory characterTimeCategory;
    public FloatProperties stunGauge = new FloatProperties();
    public float stunTime;
    public float stunDuration;
    public bool inStun;
    public Action StunRecoverAction;

    #region UI同步action
    public Action<float> OnCurrentHPChanged;
    public Action<float> OnCurrentMPChanged;
    public Action<float> OnCurrentSPChanged;
    public Action<float> OnCurrentULTChanged;
    public Action<float> OnCurrentStunChanged;
    public Action<bool> OnCurrentStunInStun;
    public Action<float> OnCurrentThunderDebuffGaugeChanged;
    public Action<float> OnCurrentThunderExploChanged;
    #endregion

    public void Init(CharacterConfig characterConfig, float currentHp = 100, float currentMp = 100)
    {
        maxHp.Init(characterConfig.hpBaseValue, null, null, null,OnMaxHPChanged);
        maxMp.Init(characterConfig.mpBaseValue, null, null, null,OnMaxMPChanged);
        maxSp.Init(characterConfig.spBaseValue, null, null, null,OnMaxSPChanged);
        maxUlt.Init(characterConfig.ultBaseValue, null, null, null,OnMaxULTChanged);
        stunGauge.Init(characterConfig.stunGauge, null, null, null, OnStunGaugeChanged);
        atk.Init(characterConfig.atkBaseValue, null, null, null, null);
        this.currentHP = maxHp.Total;
        this.currentMP = maxMp.Total;
        this.currentSP = maxSp.Total;
        this.currentULT = maxUlt.Total;
        this.currentStun = stunGauge.Total;
        currentThunderDebuff = 0;
        currentThunderExplo = 0;
        characterTimeCategory = characterConfig.TimeCategory;
        stunTime = 0;
        stunDuration = characterConfig.stunDuration;
        inStun = false;
    }

    #region 主角初始状态数值，测试代码
    public void InitPlayer()
    {
        this.currentMP = 100f;
        this.currentSP = 0f;
        this.currentULT = 10f;
    }
    #endregion

    public void AddHP(float add)
    {
        SetHP(add + this.currentHP);
    }

    public void SetHP(float value)
    {
        currentHP = Mathf.Clamp(value, 0, maxHp.Total);
        OnCurrentHPChanged?.Invoke(currentHP);
    }

    public void AddMP(float add)
    {
        SetMP(add + this.currentMP);
    }

    public void SetMP(float value)
    {
        currentMP = Mathf.Clamp(value, 0, maxMp.Total);
        OnCurrentMPChanged?.Invoke(currentMP);
    }

    public void AddSP(float add)
    {
        SetSP(add + this.currentSP);
    }

    public void SetSP(float value)
    {
        currentSP = Mathf.Clamp(value, 0, maxSp.Total);
        OnCurrentSPChanged?.Invoke(currentSP);
    }

    public void AddULT(float add)
    {
        SetULT(add + this.currentULT);
    }

    public void SetULT(float value)
    {
        currentULT = Mathf.Clamp(value, 0, maxUlt.Total);
        OnCurrentULTChanged?.Invoke(currentULT);
    }

    public void AddStun(float add)
    {
        SetStun(add + this.currentStun);
    }

    public void SetStun(float value)
    {
        float oldStun = currentStun;
        currentStun = Mathf.Clamp(value, 0, stunGauge.Total);
        OnCurrentStunChanged?.Invoke(currentStun);
        if (oldStun != 0 && currentStun == 0)
        {
            // 需要变量enterStun记录刚清空晕槽的时机，以便其他模块进行对应操作，比如展现破槽动画
            enterStun = true;
            stunTime = stunDuration;
            inStun = true;
            OnCurrentStunInStun?.Invoke(inStun);
        }
    }

    public void AddThunderDebuffGauge(float add)
    {
        SetThunderDebuffGauge(add + this.currentThunderDebuff);
    }

    public void SetThunderDebuffGauge(float value)
    {
        float oldValue = currentThunderDebuff;
        currentThunderDebuff = Mathf.Clamp(value, 0, 100);
        OnCurrentThunderDebuffGaugeChanged?.Invoke(currentThunderDebuff);
    }

    public float GetThunderDebuffGauge()
    {
        return currentThunderDebuff;
    }

    public void AddThunderExploGauge(float add)
    {
        SetThunderExploGauge(add + this.currentThunderExplo);
    }

    public void SetThunderExploGauge(float value)
    {
        currentThunderExplo = Mathf.Clamp(value, 0, 100);
        OnCurrentThunderExploChanged?.Invoke(currentThunderExplo);
    }

    public float GetThunderExploGauge()
    {
        return currentThunderExplo;
    }

    public bool InStun()
    {
        return inStun;
    }

    public bool IsEnterStun()
    {
        return enterStun;
    }

    public void SetEnterStun(bool value)
    {
        enterStun = value;
    }

    public void RecoverStun()
    {
        AddStun(stunGauge.Total);
        inStun = false;
        OnCurrentStunInStun?.Invoke(inStun);
        StunRecoverAction?.Invoke();
        stunTime = 0;
    }

    private void OnMaxHPChanged(float oldMaxHP, float newMaxHP)
    {
        if (this.currentHP > newMaxHP)
        {
            this.currentHP = newMaxHP;
        }
        // TODO:同步给UI
    }

    private void OnMaxMPChanged(float oldMaxMP, float newMaxMP)
    {
        if (this.currentMP > newMaxMP)
        {
            this.currentMP = newMaxMP;
        }
        // TODO:同步给UI
    }

    private void OnMaxSPChanged(float oldMaxSP, float newMaxSP)
    {
        if (this.currentSP > newMaxSP)
        {
            this.currentSP = newMaxSP;
        }
        // TODO:同步给UI
    }

    private void OnMaxULTChanged(float oldMaxult, float newMaxult)
    {
        if (this.currentULT > newMaxult)
        {
            this.currentULT = newMaxult;
        }
        // TODO:同步给UI
    }

    private void OnStunGaugeChanged(float oldstunGauge, float newstunGauge)
    {
        if (this.currentStun > newstunGauge)
        {
            this.currentStun = newstunGauge;
        }
        // TODO:同步给UI
    }

    public void AddStunRecoverAction(Action newAction)
    {
        StunRecoverAction += newAction;
    }

    public void RemoveStunRecoverAction(Action newAction)
    {
        StunRecoverAction -= newAction;
    }

    [Button]
    public void TestAddMaxHP(float value)
    {
        maxHp.FixedBonus += value;
    }

    private void Update()
    {
        #region Stun晕值处理
        if (InStun())
        {
            stunTime -= Time.deltaTime;
            SetStun((1f - stunTime / stunDuration) * stunGauge.Total);
            if(stunTime <= 0)
            {
                RecoverStun();
            }
        }
        #endregion
    }

}

public class FloatProperties
{
    [SerializeField] private float baseValue;
    [SerializeField] private float fixedBonus;
    [SerializeField] private float multiplierBonus;

    private Action<float, float> onBaseValueChangedAction;
    private Action<float, float> onFixedValueChangedAction;
    private Action<float, float> onMultiplierValueChangedAction;
    private Action<float, float> onTotalValueChangedAction;

    public void Init(float baseValue, Action<float, float> onBaseValueChangedAction, Action<float, float> onFixedValueChangedAction, Action<float, float> onMultiplierValueChangedAction, Action<float, float> onTotalValueChangedAction)
    {
        this.BaseValue = baseValue;
        this.onBaseValueChangedAction = onBaseValueChangedAction;
        this.onFixedValueChangedAction = onFixedValueChangedAction;
        this.onMultiplierValueChangedAction = onMultiplierValueChangedAction;
        this.onTotalValueChangedAction = onTotalValueChangedAction;
    }
    public float Total => baseValue + FixedBonus + (baseValue * MultiplierBonus);

    public float BaseValue
    {
        get => baseValue;
        set
        {
            onBaseValueChangedAction?.Invoke(baseValue, value);
            if (onTotalValueChangedAction != null)
            {
                float oldTotal = Total;
                baseValue = value;
                onTotalValueChangedAction?.Invoke(oldTotal, Total);
            }
            else baseValue = value;
        }
    }

    public float FixedBonus
    {
        get => fixedBonus;
        set
        {
            onFixedValueChangedAction?.Invoke(fixedBonus, value);
            if (onTotalValueChangedAction != null)
            {
                float oldTotal = Total;
                fixedBonus = value;
                onTotalValueChangedAction?.Invoke(oldTotal, Total);
            }
            else fixedBonus = value;
        }
    }
    public float MultiplierBonus
    {
        get => multiplierBonus;
        set
        {
            onMultiplierValueChangedAction?.Invoke(multiplierBonus, value);
            if (onTotalValueChangedAction != null)
            {
                float oldTotal = Total;
                multiplierBonus = value;
                onTotalValueChangedAction?.Invoke(oldTotal, Total);
            }
            else multiplierBonus = value;
        }
    }
}
