using JKFrame;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    private GameCharacter_Controller gameCharacter;
    [ShowInInspector] private Action<AttackData> beHitAction;
    private Action<AttackData> beHitActionFromAttackData;
    private AttackData curAttackData;

    public void Init(GameCharacter_Controller gCharacter)
    {
        gameCharacter = gCharacter;
        curAttackData = default;
        beHitAction = null;
    }

    public void TakeDamage(AttackData attackData)
    {
        // 计算伤害(也涉及到 若没击破霸体，则不用进入受伤状态)
        curAttackData = attackData;
        if(gameCharacter.GameCharacterState != GameCharacterState.Guard)
        {
            gameCharacter.PropertyAddHP(-attackData.attackValue);
            if(!gameCharacter.CharacterProperties.InStun()) 
                gameCharacter.PropertyAddStun(-attackData.stunAttackValue);
            if(attackData.detectionEvent.BuffConfig != null)
            {
                if (attackData.attackElementType == AttackElementType.Thunder)
                    gameCharacter.PropertyAddThunderDebuff(attackData.atkElementValue, attackData.detectionEvent.BuffConfig);
            }
        }

        // 切换状态 死亡 或 受伤
        bool enterStun = false;
        if (gameCharacter.GameCharacterState != GameCharacterState.Guard)
        {
            if (gameCharacter.CharacterProperties.currentHP == 0)
            {
                //Debug.Log("DamageController.Die");
                gameCharacter.ChangeState(GameCharacterState.Die, true);
            }
            else if (gameCharacter.CharacterProperties.InStun())
            {
                //Debug.Log("DamageController.Stun");
                if (gameCharacter.CharacterProperties.IsEnterStun())
                {
                    gameCharacter.ChangeState(GameCharacterState.Stun, true);
                    enterStun = true;
                }
                else
                {
                    gameCharacter.ChangeState(GameCharacterState.Stun, false);
                }
            }
            else if (CheckCharacterEnterDamage(attackData))
            {
                //Debug.Log("DamageController.Damaged");
                gameCharacter.ChangeState(GameCharacterState.Damaged);
            }

            // 播放命中音效
            if (attackData.detectionEvent.AttackHitConfig != null && attackData.detectionEvent.AttackHitConfig.HitAudioClip != null)
            {
                AudioSystem.PlayOneShot(attackData.detectionEvent.AttackHitConfig.HitAudioClip, attackData.hitPoint, false, 0.5f);
            }
            // 特效
            if (attackData.detectionEvent.AttackHitConfig != null && attackData.detectionEvent.AttackHitConfig.HitEffectPrefab != null)
            {
                GameObject effect = ProjectUtility.GetOrInstantiateGameObject(attackData.detectionEvent.AttackHitConfig.HitEffectPrefab, null);
                effect.transform.position = attackData.hitPoint;
                effect.transform.LookAt(Camera.main.transform.position);
                effect.GetComponent<EffectController>().Init(attackData.detectionEvent.AttackHitConfig.HitEffectStartRotation, true);
            }
            if (enterStun)
            {
                // 生成击晕时的受击特效
                if (gameCharacter.CanChangeState == false) return; // 意味着已经在处理enterStun相关事件，不用往下走直接返回；
                GameObject stunEffect;
                stunEffect = ProjectUtility.GetOrInstantiateGameObject(gameCharacter.CharacterConfig.EnterStunEffect, null);
                stunEffect.GetComponent<EffectController>().Init();
                stunEffect.GetComponent<ParticleSystem>().Simulate(0.000001f, true, true, false);
                stunEffect.transform.position = attackData.hitPoint;
                stunEffect.transform.LookAt(attackData.source.ModelTransform);
                stunEffect.transform.transform.localEulerAngles = new Vector3(0, stunEffect.transform.transform.localEulerAngles.y, stunEffect.transform.transform.localEulerAngles.z);
            }

        }

        beHitAction?.Invoke(curAttackData);

    }

    private bool CheckCharacterEnterDamage(AttackData attackData)
    {
        if (gameCharacter.CanChangeState)
        {
            //if (gameCharacter.HitTargetStatus == HitTargetStatus.None)
            //{
            //    return true;
            //}
            if(attackData.detectionEvent.AttackHitConfig.BreakArmor && gameCharacter.HitTargetStatus == HitTargetStatus.Armor)
            {
                return true;
            }
            if(attackData.detectionEvent.AttackHitConfig.BreakArmorLevel >= gameCharacter.ArmorLevel && gameCharacter.HitTargetStatus == HitTargetStatus.None)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 非战斗中发生，而是特殊事件导致伤害行为AttackData产生，用这个接口处理
    /// </summary>
    public void TakeDamageFromAttackData(AttackData attackData)
    {
        switch (attackData.attackType)
        {
            case SkillType.PerfectGuard:
                curAttackData = attackData;
                gameCharacter.ChangeState(GameCharacterState.Damaged);
                beHitActionFromAttackData?.Invoke(curAttackData);
                break;
            default:
                break;
        }
    }

    public void AddHitAction(Action<AttackData> newAction)
    {
        beHitAction += newAction;
    }

    public void RemoveHitAction(Action<AttackData> newAction)
    {
        beHitAction -= newAction;
    }

    public void AddHitActionFromAttackData(Action<AttackData> newAction)
    {
        beHitActionFromAttackData += newAction;
    }

    public void RemoveHitActionFromAttackData(Action<AttackData> newAction)
    {
        beHitActionFromAttackData -= newAction;
    }
}
