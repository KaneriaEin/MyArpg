using UnityEngine;

public class NodachiManSkill5Behaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = -1;

    public override SkillBehaviourBase DeepCopy()
    {
        return new NodachiManSkill5Behaviour()
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
        float speedMultiplier = 1;
        if (character.Target != null && Vector3.Distance(character.ModelTransform.position, character.Target.ModelTransform.position) < 2)
        {
            speedMultiplier = 0.08f;
        }
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
