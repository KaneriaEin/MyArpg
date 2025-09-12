public class NodachiMan_Controller : GameCharacter_Controller
{
    public override void Init(CharacterConfig characterConfig, Enemy_Controller enemy_Controller)
    {
        base.Init(characterConfig, enemy_Controller);

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
        }
    }

    public override void OnDie(string name)
    {
        base.OnDie(name);
    }

    #region enemyManager rpc相关
    #endregion
}
