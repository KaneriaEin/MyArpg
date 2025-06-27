public class PersonBS_DamagedState : GameCharacterStateBase
{
    public override void Enter()
    {
        animation.AddAnimationEvent("OnDamageFinish", OnDamageFinish);

        // �����ܻ�����
        // TODO:�ȶ���ǰ���ܹ���AttackData���پ��������ĸ�����������д��front
        AttackData atkData = gameCharacter.CurAttackData;
        if(atkData.detectionEvent.TrackName == "����")
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
