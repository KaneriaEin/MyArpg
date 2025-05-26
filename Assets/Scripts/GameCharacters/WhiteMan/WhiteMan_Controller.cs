public class WhiteMan_Controller : GameCharacter_Controller
{
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
        }
    }
}
