using UnityEngine;

public class WarriorSkill2Behaviour : Player_SkillBehaviourBase
{
    public override SkillBehaviourBase DeepCopy()
    {
        return new WarriorSkill2Behaviour() { };
    }

    public override void Release()
    {
        base.Release();
        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[0]);
        skillBrain.AddorUpdateShareData(WarriorSkillBrain.SClipStandAttackModelDataKey, true);
    }

    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y -= 9.8f * Time.deltaTime;
        owner.OnSkillMove(deltaPosition);
        owner.OnSkillRotate(deltaRotation);
    }

    public override void OnSkillClipEnd()
    {
        skillBrain.AddorUpdateShareData(WarriorSkillBrain.SClipStandAttackModelDataKey, false);
        owner.ChangeToIdleState();
    }
}
