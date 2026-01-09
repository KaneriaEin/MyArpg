public class NodachiMan_SkillState : GameCharacterStateBase
{
    public override void Enter()
    {
        gameCharacter.AddHitFreezeAction(TargetHitFreezeStart, TargetHitFreezeFinish);
        gameCharacter.BehaviorTree.SetVariableValue("SkillState", true);
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
        gameCharacter.RemoveHitFreezeAction(TargetHitFreezeStart, TargetHitFreezeFinish);
        gameCharacter.SkillBrain.StopSkill();
        animation.RemoveAnimationEvent("FootStep", OnFootStep);
        gameCharacter.BehaviorTree.SetVariableValue("SkillState", false);
    }

    public void TargetHitFreezeStart()
    {
        gameCharacter.SkillBrain.Skill_Player.SkillHitFreezeStart();
        gameCharacter.SetAnimationLayerWeight(1, 2f);
        gameCharacter.PlayAnimation_Layer1("Damage_LittleHit", null, 1 * gameCharacter.LocalTimeScale, true, 0.1f);

    }

    public void TargetHitFreezeFinish()
    {
        gameCharacter.SkillBrain.Skill_Player.SkillHitFreezeFinish();
        gameCharacter.SetAnimationLayerWeight(1, 0f);
    }
}
