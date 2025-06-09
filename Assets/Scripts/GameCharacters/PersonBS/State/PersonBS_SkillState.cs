public class PersonBS_SkillState : GameCharacterStateBase
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
        gameCharacter.SkillBrain.StopSkill();
        animation.RemoveAnimationEvent("FootStep", OnFootStep);
    }
}
