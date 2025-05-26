using JKFrame;

/// <summary>
/// 玩家技能状态
/// </summary>
public class Player_SkillState : GameCharacterStateBase
{
    public override void Enter()
    {
        animation.AddAnimationEvent("FootStep", OnFootStep);
        PlaySkill();
    }

    public override void Update()
    {
        if (CheckAndEnterSkillState())
        {
            PlaySkill();
        }
    }

    private void PlaySkill()
    {
        gameCharacter.SkillBrain.ReleaseSkill(currentReleaseSkillIndex);
    }

    public override void Exit()
    {
        base.Exit();
        animation.RemoveAnimationEvent("FootStep", OnFootStep);
    }
}
