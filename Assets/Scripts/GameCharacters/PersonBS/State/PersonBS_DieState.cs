using JKFrame;
using System.Collections;
using UnityEngine;

public class PersonBS_DieState : GameCharacterStateBase
{
    public override void Enter()
    {
        gameCharacter.HitTargetStatus = HitTargetStatus.Invincibility;

        // 播放死亡受击动画
        // TODO:先读当前所受攻击AttackData，再决定播放哪个动画，现在写死front
        AttackData atkData = gameCharacter.CurAttackData;
        gameCharacter.PlayAnimation("DieNormal", null, 1, true, 0);
        MonoSystem.Start_Coroutine(CharacterDie());
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
        yield return new WaitForSeconds(2);
        gameCharacter.OnDieAction?.Invoke(gameCharacter.gameObject.name);
    }
}
