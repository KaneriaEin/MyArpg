using UnityEngine;

public class NodachiManSkill8Behaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = -1;

    public override SkillBehaviourBase DeepCopy()
    {
        return new NodachiManSkill8Behaviour()
        {
        };
    }

    public override void Release()
    {
        base.Release();

        attackIndex = 0;

        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
    }

    public override bool OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        base.OnAttackDetection(target, attackData);

        return true;
    }
    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y -= 9.8f * Time.deltaTime;
        deltaPosition.x *= 0.6f;
        deltaPosition.z *= 0.6f;
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
