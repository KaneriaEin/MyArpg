using UnityEngine;

public class NodachiManSkill3Behaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = -1;
    private float attackStartTime = 0;

    [SerializeField] private AnimationCurve Clip1Curve;
    [SerializeField] private float Clip1MaxDistance;
    public override SkillBehaviourBase DeepCopy()
    {
        return new NodachiManSkill3Behaviour()
        {
            Clip1Curve = new AnimationCurve(Clip1Curve.keys),
            Clip1MaxDistance = 5f
        };
    }

    public override void Release()
    {
        base.Release();
        attackStartTime = 0;

        attackIndex = 0;

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
        if (character.Target != null && Vector3.Distance(character.ModelTransform.position, character.Target.ModelTransform.position) > 1f)
        {
            float distance = Vector3.Distance(character.ModelTransform.position, character.Target.ModelTransform.position);
            float normalizedDistance = Mathf.Clamp01(distance / Clip1MaxDistance);
            speedMultiplier = Clip1Curve.Evaluate(normalizedDistance);
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
        attackIndex = -1;
        BattleEventManager.Instance.RemoveAttackInfo(attackStartTime, character);
    }
}
