using UnityEngine;

public class NomiManSkill1Behaviour : GameCharacter_SkillBehaviourBase
{
    [SerializeField] private AnimationCurve ClipCurve;
    [SerializeField] private float ClipMaxDistance;
    [SerializeField] private float movDistance;
    [SerializeField] private Vector3 deltaDistance;

    //[SerializeField] private float MoveTime;TODO: 暂时保留固定时间出招的接口
    public override SkillBehaviourBase DeepCopy()
    {
        return new NomiManSkill1Behaviour()
        {
            ClipCurve = new AnimationCurve(ClipCurve.keys),
            ClipMaxDistance = 3f,
            //MoveTime = 1f / 6f
        };
    }

    public override void Release()
    {
        base.Release();
        movDistance = Mathf.Min(Vector3.Distance(character.ModelTransform.position, character.Target.ModelTransform.position), ClipMaxDistance);
        //deltaDistance = movDistance / MoveTime;
        deltaDistance = (character.Target.ModelTransform.position - character.ModelTransform.position).normalized * 50;

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
        if (character.Target != null && Vector3.Distance(character.ModelTransform.position, character.Target.ModelTransform.position) > 1.5f)
        {
            
            deltaPosition += deltaDistance * ClipCurve.Evaluate(skill_Player.CurrentFrameIndex) * Time.deltaTime;
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
    }
}
