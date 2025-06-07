using UnityEngine;

public class PersonBSStandAttackBehaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = -1;
    [SerializeField] private int standAttackCount = 3;
    public override SkillBehaviourBase DeepCopy()
    {
        return new PersonBSStandAttackBehaviour()
        {
            standAttackCount = standAttackCount,
        };
    }

    public override void Release()
    {
        base.Release();

        attackIndex += 1;
        if (attackIndex >= standAttackCount)
        {
            attackIndex = 0;
        }
        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
    }

    public override void Stop()
    {
        base.Stop();
        OnClipEndOrReleaseNewSkill();
        skill_Player.StopSkillClip();
    }

    public override void OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        base.OnAttackDetection(target, attackData);
        //Debug.Log(target.gameObject.name);
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
        attackIndex = -1;
    }
}
