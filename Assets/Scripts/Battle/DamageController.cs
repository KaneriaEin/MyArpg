using JKFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    private GameCharacter_Controller gameCharacter;
    private Action<AttackData> beHitAction;
    private AttackData curAttackData;

    public void Init(GameCharacter_Controller gCharacter)
    {
        gameCharacter = gCharacter;
        curAttackData = default;
        beHitAction = null;
    }

    public void TakeDamage(AttackData attackData)
    {
        // 计算伤害
        curAttackData = attackData;
        gameCharacter.CharacterProperties.AddHP(-attackData.attackValue);

        // 切换状态 死亡 或 受伤
        if (gameCharacter.GameCharacterState != GameCharacterState.Guard)
        {
            if (gameCharacter.CharacterProperties.currentHP == 0)
                gameCharacter.ChangeState(GameCharacterState.Die, true);
            else
                gameCharacter.ChangeState(GameCharacterState.Damaged);

            // 播放命中音效
            if (attackData.detectionEvent.AttackHitConfig != null && attackData.detectionEvent.AttackHitConfig.HitAudioClip != null)
            {
                AudioSystem.PlayOneShot(attackData.detectionEvent.AttackHitConfig.HitAudioClip, attackData.hitPoint);
            }
            // 特效
            if (attackData.detectionEvent.AttackHitConfig != null && attackData.detectionEvent.AttackHitConfig.HitEffectPrefab != null)
            {
                GameObject effect = ProjectUtility.GetOrInstantiateGameObject(attackData.detectionEvent.AttackHitConfig.HitEffectPrefab, null);
                effect.transform.position = attackData.hitPoint;
                effect.transform.LookAt(Camera.main.transform.position);
                effect.GetComponent<EffectController>().Init();
            }

        }

        beHitAction?.Invoke(curAttackData);

    }

    public void AddHitAction(Action<AttackData> newAction)
    {
        beHitAction += newAction;
    }

    public void RemoveHitAction(Action<AttackData> newAction)
    {
        beHitAction -= newAction;
    }
}
