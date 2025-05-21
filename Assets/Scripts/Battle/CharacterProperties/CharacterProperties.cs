using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class CharacterProperties : SerializedMonoBehaviour
{
    [ShowInInspector] public float currentHP;
    [ShowInInspector] public float currentMP;
    public FloatProperties maxHp = new FloatProperties();
    public FloatProperties maxMp = new FloatProperties();
    public FloatProperties atk = new FloatProperties();

    public void Init(CharacterConfig characterConfig, float currentHp = 100, float currentMp = 100)
    {
        maxHp.Init(characterConfig.hpBaseValue, null, null, null,OnMaxHPChanged);
        maxMp.Init(characterConfig.mpBaseValue, null, null, null,OnMaxMPChanged);
        atk.Init(characterConfig.atkBaseValue, null, null, null, null);
        this.currentHP = currentHp;
        this.currentMP = currentMp;
    }

    public void AddHP(float add)
    {
        SetHP(add + this.currentHP);
    }

    public void SetHP(float value)
    {
        currentHP = Mathf.Clamp(value, 0, maxHp.Total);
    }

    public void AddMP(float add)
    {
        SetMP(add + this.currentMP);
    }

    public void SetMP(float value)
    {
        currentMP = Mathf.Clamp(value, 0, maxMp.Total);
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

    [Button]
    public void TestAddMaxHP(float value)
    {
        maxHp.FixedBonus += value;
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
