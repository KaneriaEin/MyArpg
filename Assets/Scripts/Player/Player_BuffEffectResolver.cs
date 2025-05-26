using System;
using UnityEngine;

public class Player_BuffEffectResolver : BuffEffectResolverBase
{

    [SerializeField] private GameCharacter_Controller player;
    public override void Resolve(Buff buff, BuffEffectDataBase effectData)
    {
        if(effectData is SimpleBuffEffectData)
        {
            SimpleBuffEffectData simpleBuffEffectData = (SimpleBuffEffectData)effectData;
            switch (simpleBuffEffectData.type)
            {
                case BuffEffectType.Hp:
                    Debug.Log("Buff"+ buff.config.buffName + "增加hp:" + simpleBuffEffectData.value * buff.layer);
                    player.CharacterProperties.currentHP += simpleBuffEffectData.value;
                    break;
                case BuffEffectType.AtkValueMultipiler:
                    Debug.Log("Buff"+ buff.config.buffName + "增加Atk:" + simpleBuffEffectData.value * buff.layer);
                    player.CharacterProperties.atk.MultiplierBonus += simpleBuffEffectData.value;
                    break;
                default:
                    break;
            }
        }
    }
}
