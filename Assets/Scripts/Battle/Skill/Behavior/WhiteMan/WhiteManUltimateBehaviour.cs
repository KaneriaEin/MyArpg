using JKFrame;
using UnityEngine;

public class WhiteManUltimateBehaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = 0;

    public override SkillBehaviourBase DeepCopy()
    {
        return new WhiteManUltimateBehaviour()
        {
        };
    }

    public override void Release()
    {
        base.Release();
        #region Ultimate Events
        // 主角无敌
        owner.HitTargetStatus = HitTargetStatus.Invincibility;
        #endregion

        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
    }

    public override bool OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        // Debug.Log(target.gameObject.name);
        bool flag = base.OnAttackDetection(target, attackData);
        if(!flag) return false;

        #region 顿帧处理
        if (attackData.detectionEvent.AttackHitConfig.Freeze)
        {
            // 顿帧 FreezeTime
            skill_Player.SkillHitFreeze(attackData.detectionEvent.AttackHitConfig.FreezeTime);
            // 通知这个target要顿帧
            target.TargetHitFreeze(attackData.detectionEvent.AttackHitConfig.FreezeTime);
        }
        #endregion
        return true;
    }
    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        //deltaPosition.y -= 9.8f * Time.deltaTime;
        if (character.Target != null && Vector3.Distance(character.ModelTransform.position, character.Target.ModelTransform.position) < 2f)
        {
            deltaPosition.z = 0;
            deltaPosition.x = 0;
        }
        owner.OnSkillMove(deltaPosition);
        owner.OnSkillRotate(deltaRotation);
    }

    public override void Stop()
    {
        base.Stop();
        #region Ultimate Events
        // 取消主角无敌
        owner.SetDefaultHitTargetStatus();
        #endregion
        ((WhiteManSkillBrain)skillBrain).ClearNextSkillClipKey();
    }

    public override void OnSkillClipEnd()
    {
        base.OnSkillClipEnd();
        owner.ChangeToIdleState();
    }
}
