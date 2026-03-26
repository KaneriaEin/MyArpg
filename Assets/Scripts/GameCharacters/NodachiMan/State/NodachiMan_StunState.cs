using JKFrame;
using System.Text;
using UnityEngine;

public class NodachiMan_StunState : GameCharacterStateBase
{
    private Vector3 repelDir; // 原地击退目的地
    private int repelStrength; // 击退力度，用于计算
    private float repelTime; // 无根运动位移的后退时间
    private bool lockFlag = false;

    public override void Enter()
    {
        // Debug.Log("Stun_Enter");
        gameCharacter.CharacterProperties.SetEnterStun(false);
        gameCharacter.ArmorLevel = 0;
        animation.AddAnimationEvent("OnDamageFinish", OnDamageFinish);
        animation.AddAnimationEvent("OnStunIdleEnd_Finish", OnStunIdleEnd_Finish);
        animation.AddAnimationEvent("IntoStunIdle", IntoStunIdle);
        gameCharacter.CharacterProperties.AddStunRecoverAction(StunRecoverAction);
        gameCharacter.DamageController.AddHitAction(DamageBeHitAction);
        lockFlag = true;
        gameCharacter.PlayAnimationSequentially("Stun_Break", OnRootMotion, 0.5f * gameCharacter.LocalTimeScale, true, 0f, () => {
            gameCharacter.PlayAnimation("StunIdle_Start", null, 1 * gameCharacter.LocalTimeScale, false, 0.3f);
            lockFlag = false;
        });
    }

    public override void Exit()
    {
        base.Exit();
        gameCharacter.SetDefaultHitTargetStatus();
        animation.RemoveAnimationEvent("OnDamageFinish");
        animation.RemoveAnimationEvent("OnStunIdleEnd_Finish");
        animation.RemoveAnimationEvent("IntoStunIdle");
        gameCharacter.CharacterProperties.RemoveStunRecoverAction(StunRecoverAction);
        gameCharacter.DamageController.RemoveHitAction(DamageBeHitAction);
        // Debug.Log("Stun_Exit");
    }

    public override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// 正常战斗中产生的攻击伤害数据处理
    /// </summary>
    public void DamageBeHitAction(AttackData atkData)
    {
        if (lockFlag) return;
        // 先读当前所受攻击AttackData，再决定播放哪个动画
        // 顿不顿帧由atkEvent里的freeze参数决定
        StringBuilder animkey = new StringBuilder();
        animkey.Append("Damage_InStun");
        this.repelDir = Vector3.zero;
        repelTime = 0.1f;
        if (atkData.detectionEvent.AttackHitConfig.Freeze)
        {
            animkey.Append("_Imme");
        }

        #region 计算击飞值
        // 计算击飞方向
        repelStrength = atkData.detectionEvent.AttackHitConfig.RepelStrength % 10;
        repelDir = (gameCharacter.transform.position - atkData.hitPoint).normalized;
        gameCharacter.PlayAnimation(animkey.ToString(), OnRootMotion, 1 * gameCharacter.LocalTimeScale, true, 0.01f);
        #endregion
    }

    public void StunRecoverAction()
    {
        // 复原韧性级别
        gameCharacter.SetDefaultArmorLevel();
        gameCharacter.DamageController.RemoveHitAction(DamageBeHitAction);
        gameCharacter.PlayAnimation("StunIdle_End", null, 1, false, 0.1f);
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        if (repelDir != Vector3.zero)
        {
            if (repelTime > 0)
            {
                if (gameCharacter.Animation_Controller.Speed == 0) return;
                deltaPosition = repelDir * Time.deltaTime * repelStrength * 10;
                repelTime -= Time.deltaTime;
            }
        }
        deltaPosition.y = -9.8f * Time.deltaTime;
        gameCharacter.CharacterController.Move(deltaPosition);
    }

    #region 动画帧事件
    private void IntoStunIdle()
    {
        gameCharacter.PlayAnimation("StunIdle", null, 0.1f, true, 0.1f);
    }

    private void OnDamageFinish()
    {
        // Debug.Log("OnDamageFinish_Stun");
        gameCharacter.PlayAnimation("StunIdle", null, 1, true, 0.1f);
    }

    private void OnStunIdleEnd_Finish()
    {
        gameCharacter.ChangeToIdleState();
    }

    #endregion

}
