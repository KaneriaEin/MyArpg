using UnityEngine;

public class NodachiMan_IdleState : GameCharacterStateBase
{
    public override void Enter()
    {
        gameCharacter.AddHitFreezeAction(TargetHitFreezeStart, TargetHitFreezeFinish);
        gameCharacter.PlayAnimation("Idle");
    }

    public override void Exit()
    {
        base.Exit();
        gameCharacter.RemoveHitFreezeAction(TargetHitFreezeStart, TargetHitFreezeFinish);
    }

    public override void Update()
    {
        if (gameCharacter.CharacterProperties.InStun()) return;
        if (CheckAndEnterSkillState()) return;
        gameCharacter.CharacterController.Move(new Vector3(0, -9.8f * Time.deltaTime, 0));
        // 检测玩家的输入
        Vector2 cmdInput = gameCharacter.CommandController.GetMoveInput();
        float h = cmdInput.x;
        float v = cmdInput.y;

        if (h != 0 || v != 0)
        {
            // 切换状态
            gameCharacter.ChangeState(GameCharacterState.Move);
        }
    }

    #region 受击相关
    public void TargetHitFreezeStart()
    {
        gameCharacter.SkillBrain.Skill_Player.SkillHitFreezeStart();
        gameCharacter.SetAnimationLayerWeight(1, 1f);
        gameCharacter.PlayAnimation_Layer1("Damage_LittleHit", null, 1f * gameCharacter.LocalTimeScale, true, 0.1f);

    }

    public void TargetHitFreezeFinish()
    {
        gameCharacter.SkillBrain.Skill_Player.SkillHitFreezeFinish();
        // gameCharacter.SetAnimationLayerWeight(1, 0f);
    }
    #endregion
}
