using UnityEngine;

public class NodachiManStandAttackBehaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = -1;
    [SerializeField] private int standAttackCount = 1;
    private float attackStartTime = 0;
    public override SkillBehaviourBase DeepCopy()
    {
        return new NodachiManStandAttackBehaviour()
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
        attackStartTime = Time.time;
        BattleEventManager.Instance.AddAttackInfo(skillConfig.Clips[attackIndex], character);
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
        BattleEventManager.Instance.RemoveAttackInfo(attackStartTime, character);
    }
}
