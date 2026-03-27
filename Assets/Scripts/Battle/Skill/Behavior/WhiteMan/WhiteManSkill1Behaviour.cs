using JKFrame;
using Sirenix.OdinInspector;
using UnityEngine;

public class WhiteManSkill1Behaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = 0;
    [ShowInInspector] string nextClipName = null;
    [ShowInInspector] bool followUp = false;
    private bool attackEffect = false;

    public override SkillBehaviourBase DeepCopy()
    {
        return new WhiteManSkill1Behaviour()
        {
            attackIndex = attackIndex,
            nextClipName = nextClipName,
            followUp = followUp,
            attackEffect = attackEffect
        };
    }

    public override void Release()
    {
        base.Release();
        attackIndex = -1;
        attackEffect = false;

        #region 判断出招，先确认hold技，再确认普通技
        nextClipName = null;
        if (character.CommandController.GetSkillKeyHoldState(0))
        {
            followUp = ((WhiteManSkillBrain)skillBrain).GetNextSkillClipKeyHold(out nextClipName, false, true);
            if (!followUp) nextClipName = WhiteManSkillBrain.Skill1Hold_Key;
        }
        else
        {
            followUp = ((WhiteManSkillBrain)skillBrain).GetNextSkillClipKey(out nextClipName, false, true);
            if (!followUp) nextClipName = WhiteManSkillBrain.Skill1_Key;
        }
        ((WhiteManSkillBrain)skillBrain).CheckClip(ref nextClipName);
        attackIndex = GetSkillClipIndexBySkillClipName(nextClipName);
        if(attackIndex < 0)
        {
            Debug.Log("can't find attackIndex："+(nextClipName == null? "":nextClipName));
            attackIndex = 0;
        }
        // Debug.Log($"attackindex = {attackIndex}");
        CheckInvincibilitySkill(attackIndex, true);
        #endregion

        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
        if (skillConfig.Clips[attackIndex].SkillName != WhiteManSkillBrain.Skill1_Key)
        {
            // Skill1_Key不中断出招派生
            ((WhiteManSkillBrain)skillBrain).SetNextSkillClipKey(skillConfig.Clips[attackIndex]);
        }
    }

    public override bool OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        // Debug.Log(target.gameObject.name);
        #region 命中信息修正
        if (skillConfig.Clips[attackIndex].SkillName == WhiteManSkillBrain.Skill1Hold_Key || skillConfig.Clips[attackIndex].SkillName == WhiteManSkillBrain.Skill1HoldSP_Key)
        {
            int layers = ((WhiteManSkillBrain)skillBrain).GetThunderAtkBuff();
            if (layers == 1) { attackData.attackValue = 10; }
            else if (layers == 2) { attackData.attackValue = 50; }
            else if (layers == 3) { attackData.attackValue = 90; }
            else if (layers == 4) { attackData.attackValue = 130; }
            else if (layers == 5) { attackData.attackValue = 200; }
            else if (layers == 6) { attackData.attackValue = 250; }
        }
        if (skillConfig.Clips[attackIndex].SkillName == WhiteManSkillBrain.Skill1HoldSP_Key)
        {
            if (attackData.detectionEvent.TrackName == "thunder1" ||
                attackData.detectionEvent.TrackName == "thunder2" ||
                attackData.detectionEvent.TrackName == "thunder3" ||
                attackData.detectionEvent.TrackName == "thunder4")
            {
                attackData.hitPoint = target.ModelCenterPosition;
            }
        }
        #endregion

        bool flag = base.OnAttackDetection(target, attackData);
        if(!flag) return false;

        #region 命中效果处理
        if (!attackEffect)
        {
            if (skillConfig.Clips[attackIndex].SkillName == WhiteManSkillBrain.Skill1_Key)
            {
                ((WhiteManSkillBrain)skillBrain).AddThunderAtkBuff();
            }
            if (skillConfig.Clips[attackIndex].SkillName == WhiteManSkillBrain.Skill1Hold_Key
                || skillConfig.Clips[attackIndex].SkillName == WhiteManSkillBrain.Skill1HoldSP_Key)
            {
                int layer = ((WhiteManSkillBrain)skillBrain).GetThunderAtkBuff();
                ((WhiteManSkillBrain)skillBrain).RemoveThunderAtkBuff(layer);
                if(layer >= 4) { ((WhiteManSkillBrain)skillBrain).AddThunderAtkBuff(); } // 若消耗4格sp点，则返1点；
            }
            attackEffect = true;
        }
        #endregion

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
        character.HitTargetStatus = HitTargetStatus.None;
        ((WhiteManSkillBrain)skillBrain).ClearNextSkillClipKey();
        CheckInvincibilitySkill(attackIndex, false);
    }

    public override void OnSkillClipEnd()
    {
        base.OnSkillClipEnd();
        owner.ChangeToIdleState();
    }

    public override void OnReleaseNewSkillClip()
    {
        base.OnReleaseNewSkillClip();
        CheckInvincibilitySkill(attackIndex, false);
    }

    private void CheckInvincibilitySkill(int idx, bool turn)
    {
        if (skillConfig.Clips[idx].SkillName == WhiteManSkillBrain.Skill1_Key)
        {
            if (turn) { character.HitTargetStatus = HitTargetStatus.Invincibility; }
            else { character.HitTargetStatus = HitTargetStatus.None; }
        }
    }

}
