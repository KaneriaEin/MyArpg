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

        #region ÅÐ¶Ï³öÕÐ
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
}
