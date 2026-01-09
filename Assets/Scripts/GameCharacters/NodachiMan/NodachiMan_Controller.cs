using System.Collections;
using UnityEngine;

public class NodachiMan_Controller : GameCharacter_Controller
{
    public override void Init(CharacterConfig characterConfig, Enemy_Controller enemy_Controller)
    {
        base.Init(characterConfig, enemy_Controller);

        CharacterProperties.AddStunRecoverAction(NodachiManStunRecoverAction);
        // 注册PersonBS的一些enemyManager_rpc事件
    }
    public override void ChangeState(GameCharacterState newState, bool reCurrstate = false)
    {
        base.ChangeState(newState, reCurrstate);
        switch (this.gameCharacterState)
        {
            case GameCharacterState.Idle:
                stateMachine.ChangeState<NodachiMan_IdleState>(reCurrstate);
                break;
            case GameCharacterState.Move:
                stateMachine.ChangeState<NodachiMan_MoveState>(reCurrstate);
                break;
            case GameCharacterState.Skill:
                stateMachine.ChangeState<NodachiMan_SkillState>(reCurrstate);
                break;
            case GameCharacterState.Damaged:
                stateMachine.ChangeState<NodachiMan_DamagedState>(reCurrstate);
                break;
            case GameCharacterState.Die:
                stateMachine.ChangeState<NodachiMan_DieState>(reCurrstate);
                break;
            case GameCharacterState.Charge:
                stateMachine.ChangeState<NodachiMan_ChargeState>(reCurrstate);
                break;
        }
    }

    public override void OnDie(string name)
    {
        base.OnDie(name);
    }

    public override void CharacterBattleEvent(CharacterBattleEventType eventType, CharacterBattleEventArg arg)
    {
        switch (eventType)
        {
            case CharacterBattleEventType.BePerfectGuarded:
                if (gameCharacterState == GameCharacterState.Skill && SkillBrain.CurrentSkillClip.PGuardPunish)
                    this.DamageController.TakeDamageFromAttackData(arg.attackData);
                break;
            default:
                break;
        }
    }

    public void NodachiManStunRecoverAction()
    {
        if(gameCharacterState == GameCharacterState.Idle)
        {
            canChangeState = false;
            PlayAnimation("StunIdle_End");
        }
    }

    public override IEnumerator TargetHitFreezeWait(float time)
    {
        // 受伤状态则执行普通动画暂停效果
        if(gameCharacterState == GameCharacterState.Damaged)
        {
            float oldspeed = Animation_Controller.Speed;
            Animation_Controller.SetAnimationSpeed(0);

            yield return new WaitForSeconds(time);

            Animation_Controller.SetAnimationSpeed(oldspeed * LocalTimeScale);
        }
        else 
        {
            float oldspeed = Animation_Controller.Speed;
            Animation_Controller.SetAnimationSpeed(0);
            targetHitFreezeStart?.Invoke();

            yield return new WaitForSeconds(time);
            targetHitFreezeFinish?.Invoke();

            Animation_Controller.SetAnimationSpeed(oldspeed * LocalTimeScale);
        }

    }

    #region enemyManager rpc相关
    #endregion
}
