using UnityEngine;

public class WarriorSkill1Behaviour : GameCharacter_SkillBehaviourBase
{
    #region 配置
    public float standingTime = 5;
    #endregion
    private int attackIndex = -1; // -1代表没有进入技能，0、1、2代表技能中
    public override SkillBehaviourBase DeepCopy()
    {
        return new WarriorSkill1Behaviour();
    }

    public override void Release()
    {
        base.Release();
        attackIndex += 1;
        // 若技能是最后一段则进入完整cd
        if (attackIndex == skillConfig.Clips.Length - 1)
            cdTimer = cdTime;
        else
            cdTimer = standingTime;


        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
        // 让普攻连续
        skillBrain.AddorUpdateShareData(WarriorSkillBrain.ContinuousStandAttackModelDataKey, true);
    }

    public override bool CheckRelease()
    {
        bool checkCD;
        if (attackIndex == -1) checkCD = cdTimer <= 0; // 未释放状态
        else if (attackIndex == skillConfig.Clips.Length - 1) checkCD = cdTimer <= 0; // 释放了最后一段的状态
        else checkCD = true;

        return checkCD && base.CheckRelease();
    }

    public override void UpdateCDTime()
    {
        if (playing)
        {
            if (attackIndex == skillConfig.Clips.Length - 1)
            {
                cdTimer = Mathf.Clamp(cdTimer - Time.deltaTime, 0, float.MaxValue);
                // Debug.Log($"技能最后一段播放中，正在计算cd:{cdTimer}/{cdTime}");
            }
            else
            {
                // Debug.Log("技能没有播放最后一段，不计算cd");
            }
            return;
        }
        cdTimer = Mathf.Clamp(cdTimer - Time.deltaTime, 0, float.MaxValue);
        // 技能处于某一段
        if (attackIndex != -1)
        {
            if (cdTimer <= 0)
            {
                // 超时，进入完整cd
                cdTimer = cdTime;
                attackIndex = -1;
                // Debug.Log("技能没有在限制时间内完全释放完毕，但已经开始进入cd");
            }
            else
            {
                // Debug.Log($"技能还没完全释放完毕，计算内部cd:{cdTimer}/{standingTime}");
            }
        }
        else
        {
            // Debug.Log($"技能没有释放，计算cd:{cdTimer}/{cdTime}");
        }
    }

    public override void OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        base.OnAttackDetection(target, attackData);
        // Debug.Log(collider.gameObject.name);
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
        // 当前结束的是最后一段，说明技能全部结束
        if (attackIndex == skillConfig.Clips.Length - 1)
        {
            attackIndex = -1;
        }
        // 结束的是中间某一段技能
        else if (attackIndex != -1)
        {
            cdTimer = standingTime;
        }
    }
}
