using UnityEngine.TextCore.Text;
using UnityEngine;
using JKFrame;

public class WhiteMan_Controller : GameCharacter_Controller
{
    public override void Init(CharacterConfig characterConfig)
    {
        base.Init(characterConfig);
        UISystem.Show<UI_WhiteManStatus>();
    }
    public override void ChangeState(GameCharacterState newState, bool reCurrstate = false)
    {
        base.ChangeState(newState, reCurrstate);
        switch (this.gameCharacterState)
        {
            case GameCharacterState.Idle:
                stateMachine.ChangeState<WhiteMan_IdleState>(reCurrstate);
                break;
            case GameCharacterState.Move:
                stateMachine.ChangeState<WhiteMan_MoveState>(reCurrstate);
                break;
            case GameCharacterState.Skill:
                stateMachine.ChangeState<WhiteMan_SkillState>(reCurrstate);
                break;
            case GameCharacterState.Damaged:
                stateMachine.ChangeState<WhiteMan_DamagedState>(reCurrstate);
                break;
            case GameCharacterState.Guard:
                stateMachine.ChangeState<WhiteMan_GuardState>(reCurrstate);
                break;
        }
    }

    public override void PropertyAddHP(float hp)
    {
        base.PropertyAddHP(hp);
        JKFrame.EventSystem.EventTrigger<float>("OnPlayerHPChanged", CharacterProperties.currentHP);
    }

    public override void PropertyAddMP(float mp)
    {
        base.PropertyAddMP(mp);
        JKFrame.EventSystem.EventTrigger<float>("OnPlayerMPChanged", CharacterProperties.currentMP);
    }
}
