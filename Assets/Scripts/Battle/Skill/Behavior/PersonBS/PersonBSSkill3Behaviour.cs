using UnityEngine;

public class PersonBSSkill3Behaviour : GameCharacter_SkillBehaviourBase
{
    public override SkillBehaviourBase DeepCopy()
    {
        return new PersonBSSkill3Behaviour()
        {
        };
    }

    public override void Release()
    {
        base.Release();

        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[0]);
    }

    public override bool OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        base.OnAttackDetection(target, attackData);
        //Debug.Log(target.gameObject.name);

        return true;
    }
    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y -= 9.8f * Time.deltaTime;
        float speedMultiplier = 1;
        owner.OnSkillMove(deltaPosition * speedMultiplier);
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
