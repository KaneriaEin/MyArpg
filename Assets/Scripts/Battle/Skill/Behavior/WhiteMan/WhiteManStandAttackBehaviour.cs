using Sirenix.OdinInspector;
using UnityEngine;

public class WhiteManStandAttackBehaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = -1;
    [ShowInInspector] string nextClipName = null;
    [ShowInInspector] bool followUp = false;

    private bool hitFlag = false;
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
        hitFlag = false;
        bool spSkillKey = false;

        #region 判断出招
        nextClipName = null;
        // 判断是否有精防
        skillBrain.TryGetSkillShareData(WhiteManSkillBrain.PGuardKey, out spSkillKey);
        if (spSkillKey)
        {
            nextClipName = WhiteManSkillBrain.PGuardX_Key;
        }
        else
        {
            // 判断是否有精闪
            skillBrain.TryGetSkillShareData(WhiteManSkillBrain.PDodgeKey, out spSkillKey);
            if (spSkillKey)
            {
                nextClipName = WhiteManSkillBrain.PDodgeX_Key;
            }
        }
        if(nextClipName == null)
        {
            if (character.CommandController.GetStandKeyHoldState())
            {
                followUp = ((WhiteManSkillBrain)skillBrain).GetNextSkillClipKeyHold(out nextClipName, false);
            }
            else
            {
                followUp = ((WhiteManSkillBrain)skillBrain).GetNextSkillClipKey(out nextClipName, false);
            }
            if (!followUp) nextClipName = WhiteManSkillBrain.X_Key;
        }
        ((WhiteManSkillBrain)skillBrain).CheckClip(ref nextClipName);
        attackIndex = GetSkillClipIndexBySkillClipName(nextClipName);
        if (attackIndex < 0)
        {
            Debug.Log("can't find attackIndex：" + (nextClipName == null ? "" : nextClipName));
            attackIndex = 0;
        }
        // Debug.Log($"attackindex = {attackIndex}");
        CheckInvincibilitySkill(attackIndex, true);
        #endregion

        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
        ((WhiteManSkillBrain)skillBrain).SetNextSkillClipKey(skillConfig.Clips[attackIndex]);
    }

    public override bool OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        #region 命中回复MPSPULT
        if (!hitFlag)
        {
            hitFlag = true;
            float addMp = attackData.detectionEvent.AttackHitConfig.GainMp;
            float addUlt = attackData.detectionEvent.AttackHitConfig.GainUlt;
            float addSp = attackData.detectionEvent.AttackHitConfig.GainSp;

            character.PropertyAddMP(addMp);
            character.PropertyAddULT(addUlt);
            ((WhiteManSkillBrain)skillBrain).AddThunderAtkGauge(addSp);
        }
        #endregion
        if (base.OnAttackDetection(target, attackData))
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
        if(skillConfig.Clips[attackIndex].SkillName == WhiteManSkillBrain.XSP_Key) deltaPosition = deltaPosition * 3f;
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
        if (skillConfig.Clips[idx].SkillName == WhiteManSkillBrain.XXXXX_Key
            || skillConfig.Clips[idx].SkillName == WhiteManSkillBrain.PDodgeX_Key
            || skillConfig.Clips[idx].SkillName == WhiteManSkillBrain.PDodgeXSP_Key
            || skillConfig.Clips[idx].SkillName == WhiteManSkillBrain.PGuardX_Key
            || skillConfig.Clips[idx].SkillName == WhiteManSkillBrain.PGuardXSP_Key)
        {
            if (turn) { character.HitTargetStatus = HitTargetStatus.Invincibility; }
            else { character.HitTargetStatus = HitTargetStatus.None; }
        }
    }
}
