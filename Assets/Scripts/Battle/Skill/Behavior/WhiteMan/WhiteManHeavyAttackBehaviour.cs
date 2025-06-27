using JKFrame;
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

    public override bool OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        // Debug.Log(target.gameObject.name);
        bool flag = base.OnAttackDetection(target, attackData);
        if(!flag) return false;

        if(attackData.detectionEvent.TrackName == "下劈")
        {
            // 顿帧 0.1s
            skill_Player.SkillHitFreeze(0.4f);
            // 通知这个target要顿帧
            MonoSystem.Start_Coroutine(target.HitFreeze(0.4f));
        }
        return true;
    }
    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y -= 9.8f * Time.deltaTime;
        owner.OnSkillMove(deltaPosition);
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
