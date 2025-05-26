using UnityEngine;

public class WhiteMan_BuffEffectResolver : BuffEffectResolverBase
{
    [SerializeField] private WhiteMan_Controller owner;
    public override void Resolve(Buff buff, BuffEffectDataBase effectData)
    {
        if (effectData is SimpleBuffEffectData)
        {
            SimpleBuffEffectData simpleBuffEffectData = (SimpleBuffEffectData)effectData;
            switch (simpleBuffEffectData.type)
            {
                case BuffEffectType.Hp:
                    Debug.Log("Buff" + buff.config.buffName + "增加hp:" + simpleBuffEffectData.value * buff.layer);
                    owner.CharacterProperties.currentHP += simpleBuffEffectData.value;
                    break;
                case BuffEffectType.AtkValueMultipiler:
                    Debug.Log("Buff" + buff.config.buffName + "增加Atk:" + simpleBuffEffectData.value * buff.layer);
                    owner.CharacterProperties.atk.MultiplierBonus += simpleBuffEffectData.value;
                    break;
                default:
                    break;
            }
        }
    }
}
