using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("游戏角色进行普通攻击")]
public class Behaviour_StandAttack : GameCharacterAction
{
    [SerializeField] SharedBool skillState;
    [SerializeField] SharedBool skillInput;// 技能内部输入，用于重复进行某段clip时调用
    public override void OnStart()
    {
        inputManager.InputStandKey(true);
        skillState = Owner.GetVariable("SkillState") as SharedBool;
        skillState.SetValue(true);
        skillInput = Owner.GetVariable("SkillInput") as SharedBool;
        skillInput.SetValue(false);
    }
    public override TaskStatus OnUpdate()
    {
        if (controller.GameCharacterState == GameCharacterState.Skill && !skillInput.Value)
        {
            inputManager.InputStandKey(false);
        }
        if (!skillState.Value && controller.GameCharacterState != GameCharacterState.Charge)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Running;
        }
    }

    public override void OnEnd()
    {
        inputManager.CleanAllCommandsState();
        controller.Enemy_Controller.inRPC = false;
    }

    // 可选：在Inspector中重置参数
    public override void OnReset()
    {
    }
}
