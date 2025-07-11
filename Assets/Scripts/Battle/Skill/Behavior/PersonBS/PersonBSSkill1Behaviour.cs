using UnityEngine;

public class PersonBSSkill1Behaviour : GameCharacter_SkillBehaviourBase
{
    public override SkillBehaviourBase DeepCopy()
    {
        return new PersonBSSkill1Behaviour()
        {
        };
    }

    public override void Release()
    {
        base.Release();

        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[0]);
    }
    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y -= 9.8f * Time.deltaTime;
        owner.OnSkillMove(deltaPosition);
        owner.OnSkillRotate(deltaRotation);

    }

    public override void OnSkillClipEnd()
    {
        base.OnSkillClipEnd();
        owner.ChangeToIdleState();
    }

    public override void OnClipEndOrReleaseNewSkill()
    {
        base.OnClipEndOrReleaseNewSkill();
    }
}
