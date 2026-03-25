using UnityEngine;

public class NodachiManStandAttackBehaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = -1;
    [SerializeField] private int standAttackCount = 2;
    private float attackStartTime = 0;
    [SerializeField] private AnimationCurve Clip2Curve;
    [SerializeField] private float Clip2MaxDistance;
    public override SkillBehaviourBase DeepCopy()
    {
        return new NodachiManStandAttackBehaviour()
        {
            standAttackCount = standAttackCount,
            Clip2Curve = new AnimationCurve(Clip2Curve.keys),
            Clip2MaxDistance = 4f
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
        if (attackIndex == 1)
        {
            character.BehaviorTree.SetVariableValue("SkillInput", false);
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
        float speedMultiplier = 1;
        if (attackIndex == 1)
        {
            if (character.Target != null)
            {
                float distance = Vector3.Distance(character.ModelTransform.position, character.Target.ModelTransform.position);
                float normalizedDistance = Mathf.Clamp01(distance / Clip2MaxDistance);
                speedMultiplier = Clip2Curve.Evaluate(normalizedDistance);
            }
        }
        owner.OnSkillMove(deltaPosition * speedMultiplier * character.LocalTimeScale);
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

    public override void AfterSkillCustomEvent(SkillCustomEvent customEvent)
    {
        base.AfterSkillCustomEvent(customEvent);
        if (customEvent.EventType == SkillEventType.CanSkillRelease && attackIndex == 0)
        {
            character.BehaviorTree.SetVariableValue("SkillInput", true);
            character.GetComponent<EnemyInputManager>().InputStandKey(true);
        }
    }
}
