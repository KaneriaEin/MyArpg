using JKFrame;

public class PersonBS_DamagedState : GameCharacterStateBase
{
    public override void Enter()
    {
        animation.AddAnimationEvent("OnDamageFinish", OnDamageFinish);
        gameCharacter.DamageController.AddHitAction(DamageBeHitAction);
        gameCharacter.Enemy_Controller.inRPC = false;
    }

    public override void Exit()
    {
        base.Exit();
        animation.RemoveAnimationEvent("OnDamageFinish", OnDamageFinish);
        gameCharacter.DamageController.RemoveHitAction(DamageBeHitAction);
    }

    private void OnDamageFinish()
    {
        gameCharacter.ChangeToIdleState();
    }

    public void DamageBeHitAction(AttackData atkData)
    {
        // 播放受击动画
        // TODO:先读当前所受攻击AttackData，再决定播放哪个动画，现在写死front
        // TODO:顿不顿帧由atkEvent里的参数决定，现在写死“下劈”
        if (atkData.detectionEvent.TrackName == "下劈")
        {
            gameCharacter.PlayAnimation("DamageFrontImme", null, 1, true, 0);
        }
        else
        {
            gameCharacter.PlayAnimation("DamageFront", null, 1, true, 0);
        }
    }
}
