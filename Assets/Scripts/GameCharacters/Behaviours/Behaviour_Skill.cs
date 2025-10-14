using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("游戏角色发动技能")]
public class Behaviour_Skill : GameCharacterAction
{
    [SerializeField] SharedInt skillConfigIndex;
    [SerializeField] SharedFloat Duration;
    private float duration;
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
            inputManager.InputSkillKey(skillConfigIndex.Value, true);
        }
        duration = Duration.Value;
    }
    public override TaskStatus OnUpdate()
    {
        duration = Mathf.Clamp(duration - Time.deltaTime, 0, duration);
        if (duration == 0)
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
                inputManager.InputSkillKey(skillConfigIndex.Value, false);
            }
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
