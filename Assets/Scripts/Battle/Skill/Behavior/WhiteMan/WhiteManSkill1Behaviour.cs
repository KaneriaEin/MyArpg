using JKFrame;
using UnityEngine;

public class WhiteManSkill1Behaviour : GameCharacter_SkillBehaviourBase
{
    private int attackIndex = 0;

    public override SkillBehaviourBase DeepCopy()
    {
        return new WhiteManSkill1Behaviour()
        {
        };
    }

    public override void Release()
    {
        base.Release();
        // 若是防反状态下，使出SP版本
        skillBrain.TryGetSkillShareData(WhiteManSkillBrain.SPSkillKey, out bool spSkillKey);
        if (spSkillKey) { attackIndex = 1; } else { attackIndex = 0; }
        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
        ((WhiteManSkillBrain)skillBrain).SetNextSkillClipKey(skillConfig.Clips[attackIndex]);
    }

    public override bool OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        // Debug.Log(target.gameObject.name);
        bool flag = base.OnAttackDetection(target, attackData);
        if(!flag) return false;

        if(attackData.detectionEvent.AttackHitConfig.Freeze)
        {
            // 顿帧 FreezeTime
            skill_Player.SkillHitFreeze(attackData.detectionEvent.AttackHitConfig.FreezeTime);
            // 通知这个target要顿帧
            MonoSystem.Start_Coroutine(target.HitFreeze(attackData.detectionEvent.AttackHitConfig.FreezeTime));
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
}
