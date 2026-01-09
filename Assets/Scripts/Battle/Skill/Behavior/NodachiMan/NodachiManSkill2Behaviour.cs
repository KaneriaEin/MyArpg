using UnityEngine;

public class NodachiManSkill2Behaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = -1;
    [SerializeField] private int attackCount = 0;
    [SerializeField] private int attackTotalCount = 3;
    [SerializeField] private GameObject chargeEffect;

    [SerializeField] private AnimationCurve Clip2Curve;
    [SerializeField] private float Clip2MaxDistance;
    public override SkillBehaviourBase DeepCopy()
    {
        return new NodachiManSkill2Behaviour()
        {
            attackIndex = attackIndex,
            attackCount = attackCount,
            attackTotalCount = attackTotalCount,
            chargeEffect = chargeEffect,
            Clip2Curve = new AnimationCurve(Clip2Curve.keys),
            Clip2MaxDistance = 4f
        };
    }

    public override void Release()
    {
        base.Release();
        skill_Player.StartPlayerSkillConfig(this);

        skillBrain.TryGetSkillShareData(NodachiManSkillBrain.SkillChargeFinish, out bool chargeFin);
        if (chargeFin)
        {
            // 蓄力结束释放攻击clip
            attackIndex = 1;
            attackCount++;
            // 判断攻击是否打完，打完结束，没打完接着打
            if (attackCount == attackTotalCount)
            {
                skillBrain.AddorUpdateShareData(NodachiManSkillBrain.SkillChargeFinish, false);
                character.BehaviorTree.SetVariableValue("SkillState", false);
            }
        }
        else
        {
            // 进行蓄力clip
            attackIndex = 0;
            attackCount = 0;
        }
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
        if (attackIndex == 0)
        {
            // 蓄力进入蓄力状态
            skillBrain.AddorUpdateShareData(NodachiManSkillBrain.SkillChargeEffect, chargeEffect);
            character.ChangeState(GameCharacterState.Charge);
        }
        else
        {
            owner.ChangeToIdleState();
        }
    }

    public override void OnClipEndOrReleaseNewSkill()
    {
        base.OnClipEndOrReleaseNewSkill();
    }

    public override void Stop()
    {
        base.Stop();
        skillBrain.AddorUpdateShareData(NodachiManSkillBrain.SkillChargeFinish, false);
    }
}
