using Sirenix.OdinInspector;
using UnityEngine;

public class WhiteManStandAttackBehaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = -1;
    private bool atkIdxFind = false;
    [ShowInInspector] string nextClipName = null;
    [ShowInInspector] bool followUp = false;
    public override SkillBehaviourBase DeepCopy()
    {
        return new WhiteManStandAttackBehaviour()
        {
        };
    }

    public override void Release()
    {
        base.Release();
        attackIndex = -1;
        atkIdxFind = false;

        #region 判断出招
        nextClipName = null;
        // 先判断是否有精防
        skillBrain.TryGetSkillShareData(WhiteManSkillBrain.SPSkillKey, out bool spSkillKey);
        if (spSkillKey) { attackIndex = 7; character.HitTargetStatus = HitTargetStatus.Invincibility; atkIdxFind = true; }
        // 先确认hold技
        if(!atkIdxFind && character.CommandController.GetStandKeyHoldState())
        {
            followUp = ((WhiteManSkillBrain)skillBrain).GetNextSkillClipKeyHold(out nextClipName, false);
            if (followUp)
            {
                ((WhiteManSkillBrain)skillBrain).CheckClip(ref nextClipName);
                attackIndex = GetSkillClipIndexBySkillClipName(nextClipName);
                atkIdxFind = true;
            }
        }
        if (!atkIdxFind)
        {
            followUp = ((WhiteManSkillBrain)skillBrain).GetNextSkillClipKey(out nextClipName, false);
            if (followUp)
            {
                ((WhiteManSkillBrain)skillBrain).CheckClip(ref nextClipName);
                attackIndex = GetSkillClipIndexBySkillClipName(nextClipName);
                atkIdxFind = true;
            }
        }
        if (!atkIdxFind)
        {
            attackIndex = 0;
            atkIdxFind = true;
        }
        // Debug.Log($"attackindex = {attackIndex}");
        #endregion

        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
        ((WhiteManSkillBrain)skillBrain).SetNextSkillClipKey(skillConfig.Clips[attackIndex]);
    }

    public override bool OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        if(base.OnAttackDetection(target, attackData))
        {
            //Debug.Log(target.gameObject.name);
            ((WhiteManSkillBrain)skillBrain).Add_WBCombo(1);

            if (attackData.detectionEvent.AttackHitConfig.Freeze)
            {
                // 顿帧 FreezeTime
                skill_Player.SkillHitFreeze(attackData.detectionEvent.AttackHitConfig.FreezeTime);
                // 通知这个target要顿帧
                target.TargetHitFreeze(attackData.detectionEvent.AttackHitConfig.FreezeTime);
            }
        }
        return true;
    }
    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y -= 9.8f * Time.deltaTime;
        #region 不同的攻击修正对应的系数
        if (attackIndex == 3) deltaPosition = deltaPosition * 0.5f;
        #endregion
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
        ((WhiteManSkillBrain)skillBrain).ClearNextSkillClipKey();
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
