using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class WarriorStandAttackBehaviour : Player_SkillBehaviourBase
{
    private int attackIndex = -1;
    [SerializeField] private int standAttackCount = 3;
    [SerializeField] private int sClipIndex = 3;
    public override SkillBehaviourBase DeepCopy()
    {
        return new WarriorStandAttackBehaviour()
        {
            standAttackCount = standAttackCount,
            sClipIndex = sClipIndex
        };
    }

    public override void Release()
    {
        base.Release();

        // ·­¹öµÄÌØÊâ¹¥»÷
        if(skillBrain.TryGetSkillShareData(WarriorSkillBrain.SClipStandAttackModelDataKey, out bool sclip) && sclip)
        {
            skillBrain.AddorUpdateShareData(WarriorSkillBrain.SClipStandAttackModelDataKey, false);
            attackIndex = sClipIndex;
        }
        else
        {
            attackIndex += 1;
            if (attackIndex >= standAttackCount)
            {
                attackIndex = 0;
            }
        }
        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
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
        owner.ChangeToIdleState();
    }

    public override void OnClipEndOrReleaseNewSkill()
    {
        base.OnClipEndOrReleaseNewSkill();
        skillBrain.TryGetSkillShareData(WarriorSkillBrain.ContinuousStandAttackModelDataKey, out bool continuous);
        if (!continuous) attackIndex = -1;
        skillBrain.AddorUpdateShareData(WarriorSkillBrain.ContinuousStandAttackModelDataKey, false);
    }
}
