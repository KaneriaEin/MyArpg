using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteMan_SkillState : GameCharacterStateBase
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
