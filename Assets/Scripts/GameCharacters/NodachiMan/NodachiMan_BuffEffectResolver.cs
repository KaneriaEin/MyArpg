using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class NodachiMan_BuffEffectResolver : BuffEffectResolverBase
{

    [SerializeField] private NodachiMan_Controller owner;
    public override void Resolve(Buff buff, BuffEffectDataBase effectData)
    {
        if(effectData is SimpleBuffEffectData)
        {
            SimpleBuffEffectData simpleBuffEffectData = (SimpleBuffEffectData)effectData;
            switch (simpleBuffEffectData.type)
            {
                case BuffEffectType.Hp:
                    Debug.Log("Buff"+ buff.config.buffName + "增加hp:" + simpleBuffEffectData.value * buff.layer);
                    owner.CharacterProperties.currentHP += simpleBuffEffectData.value;
                    break;
                case BuffEffectType.AtkValueMultipiler:
                    Debug.Log("Buff"+ buff.config.buffName + "增加Atk:" + simpleBuffEffectData.value * buff.layer);
                    owner.CharacterProperties.atk.MultiplierBonus += simpleBuffEffectData.value;
                    break;
                case BuffEffectType.UIShow:
                    // Debug.Log("Buff"+ buff.config.buffName + "减少" + -5 * Time.deltaTime);
                    owner.PropertyAddThunderDebuff(-5 * Time.deltaTime, buff.config);
                    break;
                default:
                    break;
            }
        }
        if(effectData is DamageBuffEffectData)
        {
            DamageBuffEffectData buffEffectData = (DamageBuffEffectData)effectData;
            switch (buffEffectData.type)
            {
                case BuffEffectType.Damage:
                    Debug.Log("Buff" + buff.config.buffName + "造成Damage" + buffEffectData.AttackValue);
                    break;
                default:
                    break;
            }
        }
    }
}
