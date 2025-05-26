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
        }
    }
}
