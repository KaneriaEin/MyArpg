public class PersonBS_DamagedState : GameCharacterStateBase
{
    public override void Enter()
    {
        animation.AddAnimationEvent("OnDamageFinish", OnDamageFinish);

        // 播放受击动画
        // TODO:先读当前所受攻击AttackData，再决定播放哪个动画，现在写死front
        AttackData atkData = gameCharacter.CurAttackData;
        if(atkData.detectionEvent.TrackName == "下劈")
        {
            gameCharacter.PlayAnimation("DamageFrontImme", null, 1, true, 0);
        }
        else
        {
            gameCharacter.PlayAnimation("DamageFront", null, 1, true, 0);
        }
    }

    private void OnDamageFinish()
    {
        gameCharacter.ChangeToIdleState();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        base.Exit();
        animation.AddAnimationEvent("OnDamageFinish", OnDamageFinish);
    }
}
