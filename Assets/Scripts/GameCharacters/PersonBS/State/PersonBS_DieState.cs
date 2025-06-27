using JKFrame;
using System.Collections;
using UnityEngine;

public class PersonBS_DieState : GameCharacterStateBase
{
    public override void Enter()
    {
        gameCharacter.HitTargetStatus = HitTargetStatus.Invincibility;

        // ���������ܻ�����
        // TODO:�ȶ���ǰ���ܹ���AttackData���پ��������ĸ�����������д��front
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
