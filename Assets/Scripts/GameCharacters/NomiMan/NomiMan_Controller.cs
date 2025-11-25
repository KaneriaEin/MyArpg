using BehaviorDesigner.Runtime;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class NomiMan_Controller : GameCharacter_Controller
{
    public override void Init(CharacterConfig characterConfig, Enemy_Controller enemy_Controller)
    {
        base.Init(characterConfig, enemy_Controller);

        CharacterProperties.AddStunRecoverAction(NomiManStunRecoverAction);

    }
    public override void ChangeState(GameCharacterState newState, bool reCurrstate = false)
    {
        base.ChangeState(newState, reCurrstate);
        switch (this.gameCharacterState)
        {
            case GameCharacterState.Idle:
                stateMachine.ChangeState<NomiMan_IdleState>(reCurrstate);
                break;
            case GameCharacterState.Move:
                stateMachine.ChangeState<NomiMan_MoveState>(reCurrstate);
                break;
            case GameCharacterState.Skill:
                stateMachine.ChangeState<NomiMan_SkillState>(reCurrstate);
                break;
            case GameCharacterState.Damaged:
                stateMachine.ChangeState<NomiMan_DamagedState>(reCurrstate);
                break;
            case GameCharacterState.Die:
                stateMachine.ChangeState<NomiMan_DieState>(reCurrstate);
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
                if(gameCharacterState == GameCharacterState.Skill && SkillBrain.CurrentSkillClip.PGuardPunish)
                    this.DamageController.TakeDamageFromAttackData(arg.attackData);
                break;
            default:
                break;
        }
    }

    public void NomiManStunRecoverAction()
    {
        if(gameCharacterState == GameCharacterState.Idle)
        {
            canChangeState = false;
            PlayAnimation("StunIdle_End");
        }
    }

    #region enemyManager rpcœ‡πÿ
    #endregion
}
