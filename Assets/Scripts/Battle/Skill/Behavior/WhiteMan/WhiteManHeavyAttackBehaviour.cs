using JKFrame;
using Sirenix.OdinInspector;
using UnityEngine;

public class WhiteManHeavyAttackBehaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = -1;

    [SerializeField] private AnimationCurve Clip2Curve;
    [SerializeField] private float Clip2MaxDistance;

    [ShowInInspector] string nextClipName = null;
    [ShowInInspector] bool followUp = false;
    public override SkillBehaviourBase DeepCopy()
    {
        return new WhiteManHeavyAttackBehaviour()
        {
            Clip2Curve = new AnimationCurve(Clip2Curve.keys),
            Clip2MaxDistance = 4f
        };
    }

    public override void Release()
    {
        base.Release();

        #region 判断出招
        nextClipName = null;
        followUp = ((WhiteManSkillBrain)skillBrain).GetNextSkillClipKey(out nextClipName, true);
        if (followUp)
        {
            attackIndex = GetSkillClipIndexBySkillClipName(nextClipName);
            if (attackIndex < 0) attackIndex = 0;
        }
        else
        {
            attackIndex = 0;
        }
        #endregion

        #region 调整不同招式中角色状态
        if(attackIndex == 2) // 上挑下砸技 / YYY
        {
            skillBrain.SetUnInterruptibleFlag(true);
            ((WhiteManSkillBrain)skillBrain).Add_WBCombo(-3);
        }
        #endregion

        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
        ((WhiteManSkillBrain)skillBrain).SetNextSkillClipKey(skillConfig.Clips[attackIndex]);
    }

    public override bool OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        // Debug.Log(target.gameObject.name);
        bool flag = base.OnAttackDetection(target, attackData);
        if(!flag) return false;

        if(attackData.detectionEvent.AttackHitConfig.Freeze)
        {
            // 顿帧 FreezeTime
            skill_Player.SkillHitFreeze(attackData.detectionEvent.AttackHitConfig.FreezeTime);
            // 通知这个target要顿帧
            target.TargetHitFreeze(attackData.detectionEvent.AttackHitConfig.FreezeTime);
        }
        return true;
    }
    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y -= 9.8f * Time.deltaTime;
        float speedMultiplier = 1;
        if (attackIndex == 0)
        {
            if (character.Target != null)
            {
                float distance = Vector3.Distance(character.ModelTransform.position, character.Target.ModelTransform.position);
                float normalizedDistance = Mathf.Clamp01(distance / Clip2MaxDistance);
                speedMultiplier = Clip2Curve.Evaluate(normalizedDistance);
            }
        }
        owner.OnSkillMove(deltaPosition * speedMultiplier);
        owner.OnSkillRotate(deltaRotation);
    }

    public override void Stop()
    {
        base.Stop();
        ((WhiteManSkillBrain)skillBrain).ClearNextSkillClipKey();
    }

    public override void OnSkillClipEnd()
    {
        base.OnSkillClipEnd();
        owner.ChangeToIdleState();
    }

    public override void AfterSkillCustomEvent(SkillCustomEvent customEvent)
    {
        base.AfterSkillCustomEvent(customEvent);
        if (customEvent.EventType == SkillEventType.CameraZoomIn)
        {
            CameraManager.Instance.CameraFOVZoomInForSeconds(customEvent.IntArg, 100f, customEvent.FloatArg);
        }
    }
}
