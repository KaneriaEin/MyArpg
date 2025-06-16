public class PersonBS_DamagedState : GameCharacterStateBase
{
    public override void Enter()
    {
        // TODO:先读当前所受攻击AttackData，再决定播放哪个动画，现在写死front
        //if(curAttackData.hitPoint)
        animation.AddAnimationEvent("OnDamageFinish", OnDamageFinish);
        gameCharacter.PlayAnimation("DamageFront", null, 1, true, 0);
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
