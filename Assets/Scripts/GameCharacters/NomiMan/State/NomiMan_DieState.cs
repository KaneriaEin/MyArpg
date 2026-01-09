using JKFrame;
using System.Collections;
using UnityEngine;

public class NomiMan_DieState : GameCharacterStateBase
{
    public override void Enter()
    {
        gameCharacter.HitTargetStatus = HitTargetStatus.Invincibility;
        gameCharacter.DamageController.AddHitAction(DieBeHitAction);
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        base.Exit();
    }

    private IEnumerator CharacterDie()
    {
        gameCharacter.DamageController.RemoveHitAction(DieBeHitAction);
        gameCharacter.OnDie(gameCharacter.name);
        yield return new WaitForSeconds(1.5f);
        gameCharacter.OnDieAction?.Invoke(gameCharacter.gameObject.name);
    }

    public void DieBeHitAction(AttackData atkData)
    {
        // 播放死亡受击动画
        // TODO:先读当前所受攻击AttackData，再决定播放哪个动画，现在写死DieNormal
        gameCharacter.PlayAnimation("DieNormal", null, 1, true, 0);
        MonoSystem.Start_Coroutine(CharacterDie());
    }
}
