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
                    // 发生一个雷暴
                    AttackData data = new AttackData();
                    data.attackType = SkillType.Skill;
                    data.source = PlayerManager.Instance.Player;
                    data.hitPoint = owner.ModelTransform.transform.position;
                    data.attackValue = buffEffectData.AttackValue;
                    data.detectionEvent = new SkillAttackDetectionEvent();
                    data.detectionEvent.TrackName = "finishThunderDown";
                    data.detectionEvent.AttackHitConfig = new AttackHitConfig();
                    data.detectionEvent.AttackHitConfig.BreakArmorLevel = 3;
                    data.detectionEvent.AttackHitConfig.HitAudioClip = buffEffectData.HitAudioClip;
                    // 特效
                    if (buffEffectData.HitEffectPrefab != null)
                    {
                        GameObject effect = ProjectUtility.GetOrInstantiateGameObject(buffEffectData.HitEffectPrefab, null);
                        effect.transform.position = data.hitPoint;
                        effect.GetComponent<EffectController>().Init(0, true);
                    }

                    CameraShakeConfig shakeConfig = new CameraShakeConfig();
                    shakeConfig.shakeShape = CameraShakeShape.Light;
                    shakeConfig.baseAmplitude = 1f;
                    shakeConfig.screenDirectionBias = Vector2.down;
                    CameraShakeManager.Instance.TriggerShake(shakeConfig, owner.ModelTransform.transform.position);
                    owner.DamageController.TakeDamage(data);
                    ((WhiteManSkillBrain)PlayerManager.Instance.Player.SkillBrain).AddThunderAtkBuff();
                    break;
                default:
                    break;
            }
        }
    }
}
