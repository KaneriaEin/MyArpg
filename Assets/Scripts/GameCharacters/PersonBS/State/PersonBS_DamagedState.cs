public class PersonBS_DamagedState : GameCharacterStateBase
{
    public override void Enter()
    {
        // �ȶ���ǰ���ܹ���AttackData���پ��������ĸ�����
        //if(curAttackData.hitPoint)
        animation.AddAnimationEvent("OnDamageFinish", OnDamageFinish);
        gameCharacter.PlayAnimation("DamageFront", null, 1, true, 0.01f);
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
