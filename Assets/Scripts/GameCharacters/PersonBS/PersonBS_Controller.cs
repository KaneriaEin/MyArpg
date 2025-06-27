using System.Collections;
using UnityEngine;

public class PersonBS_Controller : GameCharacter_Controller
{
    public override void ChangeState(GameCharacterState newState, bool reCurrstate = false)
    {
        base.ChangeState(newState, reCurrstate);
        switch (this.gameCharacterState)
        {
            case GameCharacterState.Idle:
                stateMachine.ChangeState<PersonBS_IdleState>(reCurrstate);
                break;
            case GameCharacterState.Move:
                stateMachine.ChangeState<PersonBS_MoveState>(reCurrstate);
                break;
            case GameCharacterState.Skill:
                stateMachine.ChangeState<PersonBS_SkillState>(reCurrstate);
                break;
            case GameCharacterState.Damaged:
                stateMachine.ChangeState<PersonBS_DamagedState>(reCurrstate);
                break;
            case GameCharacterState.Die:
                stateMachine.ChangeState<PersonBS_DieState>(reCurrstate);
                break;
        }
    }

    public override void BeHit(AttackData attackData)
    {
        base.BeHit(attackData);
        CurAttackData = attackData;
        CharacterProperties.AddHP(-attackData.attackValue);
        if(CharacterProperties.currentHP == 0)
            ChangeState(GameCharacterState.Die, true);
        else
            ChangeState(GameCharacterState.Damaged, true);
    }
}
