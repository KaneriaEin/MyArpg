using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("游戏角色发动技能")]
public class Behaviour_Skill : GameCharacterAction
{
    [SerializeField] SharedInt skillConfigIndex;
    [SerializeField] SharedBool skillState;
    [SerializeField] SharedBool skillInput;// 技能内部输入，用于重复进行某段clip时调用
    public override void OnStart()
    {
        if(skillConfigIndex.Value == 0)
        {
            inputManager.InputStandKey(true);
        }
        else if(skillConfigIndex.Value == 1)
        {
            inputManager.InputDodgeKey(true);
        }
        else if(skillConfigIndex.Value == 2)
        {
            inputManager.InputHeavyKey(true);
        }
        else
        {
            inputManager.InputSkillKey(skillConfigIndex.Value - 3, true);
        }
        skillState = Owner.GetVariable("SkillState") as SharedBool;
        skillState.SetValue(true);
        skillInput = Owner.GetVariable("SkillInput") as SharedBool;
        skillInput.SetValue(false);
    }
    public override TaskStatus OnUpdate()
    {
        // 当释放了技能,角色正在发动skill，那么键位可以松开
        if (controller.GameCharacterState == GameCharacterState.Skill && !skillInput.Value)
        {
            if (skillConfigIndex.Value == 0)
            {
                inputManager.InputStandKey(false);
            }
            else if (skillConfigIndex.Value == 1)
            {
                inputManager.InputDodgeKey(false);
            }
            else if (skillConfigIndex.Value == 2)
            {
                inputManager.InputHeavyKey(false);
            }
            else
            {
                inputManager.InputSkillKey(skillConfigIndex.Value - 3, false);
            }
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
        //Owner.SetVariableValue("GetRPC_PersonBS_Skill1", false);
        controller.Enemy_Controller.inRPC = false;
    }

    // 可选：在Inspector中重置参数
    public override void OnReset()
    {
    }
}
