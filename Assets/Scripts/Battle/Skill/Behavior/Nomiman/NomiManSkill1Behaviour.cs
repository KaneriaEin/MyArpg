using UnityEngine;

public class NomiManSkill1Behaviour : GameCharacter_SkillBehaviourBase
{
    [SerializeField] private AnimationCurve ClipCurve;
    [SerializeField] private float ClipMaxDistance;
    public override SkillBehaviourBase DeepCopy()
    {
        return new NomiManSkill1Behaviour()
        {
            ClipCurve = new AnimationCurve(ClipCurve.keys),
            ClipMaxDistance = 3f
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
        if (character.Target != null)
        {
            float distance = Vector3.Distance(character.ModelTransform.position, character.Target.ModelTransform.position);
            float normalizedDistance = Mathf.Clamp01(distance / ClipMaxDistance);
            speedMultiplier = ClipCurve.Evaluate(normalizedDistance);
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
