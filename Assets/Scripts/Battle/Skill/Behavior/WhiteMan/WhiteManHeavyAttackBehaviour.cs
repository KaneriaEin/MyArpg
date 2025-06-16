using Sirenix.OdinInspector;
using UnityEngine;

public class WhiteManHeavyAttackBehaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = -1;

    [ShowInInspector] string nextClipName = null;
    [ShowInInspector] bool followUp = false;
    public override SkillBehaviourBase DeepCopy()
    {
        return new WhiteManHeavyAttackBehaviour()
        {
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
        }
        #endregion

        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
        ((WhiteManSkillBrain)skillBrain).SetNextSkillClipKey(skillConfig.Clips[attackIndex]);
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
        ((WhiteManSkillBrain)skillBrain).ClearNextSkillClipKey();
        owner.ChangeToIdleState();
    }

    public override void OnClipEndOrReleaseNewSkill()
    {
        base.OnClipEndOrReleaseNewSkill();
    }

    public override void AfterSkillCustomEvent(SkillCustomEvent customEvent)
    {
        base.AfterSkillCustomEvent(customEvent);
        if (customEvent.EventType == SkillEventType.CameraZoomIn)
        {
            CameraManager.Instance.CameraFOVZoomIn(customEvent.FloatArg, 100f);
        }
        else if (customEvent.EventType == SkillEventType.CameraZoomOut)
        {
            CameraManager.Instance.CameraFOVZoomOut(customEvent.FloatArg, 100f);
        }
    }
}
