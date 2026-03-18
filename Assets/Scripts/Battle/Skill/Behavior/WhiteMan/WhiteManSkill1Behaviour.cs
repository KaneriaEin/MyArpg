using JKFrame;
using Sirenix.OdinInspector;
using UnityEngine;

public class WhiteManSkill1Behaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = 0;
    [ShowInInspector] string nextClipName = null;
    [ShowInInspector] bool followUp = false;
    [SerializeField] private BuffConfig thunderAtkBuff;
    private bool addBuff = false;

    public override SkillBehaviourBase DeepCopy()
    {
        return new WhiteManSkill1Behaviour()
        {
            attackIndex = attackIndex,
            nextClipName = nextClipName,
            followUp = followUp,
            thunderAtkBuff = thunderAtkBuff,
            addBuff = addBuff
        };
    }

    public override void Release()
    {
        base.Release();
        attackIndex = -1;
        addBuff = false;

        #region 判断出招，先确认hold技，再确认普通技
        nextClipName = null;
        if (character.CommandController.GetSkillKeyHoldState(0))
        {
            followUp = ((WhiteManSkillBrain)skillBrain).GetNextSkillClipKeyHold(out nextClipName, false);
            if (followUp)
            {
                attackIndex = GetSkillClipIndexBySkillClipName(nextClipName);
            }
            if (attackIndex < 0)
            {
                followUp = ((WhiteManSkillBrain)skillBrain).GetNextSkillClipKey(out nextClipName, false);
                if (followUp)
                {
                    attackIndex = GetSkillClipIndexBySkillClipName(nextClipName);
                }
            }
            if (attackIndex < 0) attackIndex = 1;
            character.HitTargetStatus = HitTargetStatus.None;
        }
        else
        {
            followUp = ((WhiteManSkillBrain)skillBrain).GetNextSkillClipKey(out nextClipName, false);
            if (followUp)
            {
                attackIndex = GetSkillClipIndexBySkillClipName(nextClipName);
                if (attackIndex < 0) attackIndex = 0;
            }
            else
            {
                attackIndex = 0;
            }
        }
        // Debug.Log($"attackindex = {attackIndex}");
        #endregion

        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
        ((WhiteManSkillBrain)skillBrain).SetNextSkillClipKey(skillConfig.Clips[attackIndex]);
    }

    public override bool OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        if(!addBuff) { ((WhiteManSkillBrain)skillBrain).AddThunderAtkBuff(); addBuff = true; }

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
    }

    public override void OnSkillClipEnd()
    {
        base.OnSkillClipEnd();
        owner.ChangeToIdleState();
    }
}
