using UnityEngine;

public class WhiteManHeavyAttackBehaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = -1;
    [SerializeField] private int heavyAttackCount = 3;
    public override SkillBehaviourBase DeepCopy()
    {
        return new WhiteManHeavyAttackBehaviour()
        {
            heavyAttackCount = heavyAttackCount,
        };
    }

    public override void Release()
    {
        base.Release();

        attackIndex += 1;
        if (attackIndex >= heavyAttackCount)
        {
            attackIndex = 0;
        }
        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
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
