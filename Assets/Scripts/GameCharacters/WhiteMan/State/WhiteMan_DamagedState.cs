using System.Collections.Generic;
using UnityEngine;

public class WhiteMan_DamagedState : GameCharacterStateBase
{
    public override void Enter()
    {
        animation.AddAnimationEvent("OnDamageFinish", OnDamageFinish);
        gameCharacter.DamageController.AddHitAction(DamageBeHitAction);
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        base.Exit();
        animation.AddAnimationEvent("OnDamageFinish", OnDamageFinish);
        gameCharacter.DamageController.RemoveHitAction(DamageBeHitAction);
    }

    private void OnDamageFinish()
    {
        gameCharacter.ChangeToIdleState();
    }

    public void DamageBeHitAction(AttackData atkData)
    {
        gameCharacter.PlayAnimation("DamageFront", null, 1 * gameCharacter.LocalTimeScale, true, 0f);
    }
}
