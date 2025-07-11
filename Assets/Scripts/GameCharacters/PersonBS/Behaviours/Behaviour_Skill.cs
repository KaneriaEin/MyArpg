using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("游戏角色发动技能")]
public class Behaviour_Skill : GameCharacterAction
{
    [SerializeField] SharedInt skillIndex;
    [SerializeField] SharedFloat Duration;
    private float duration;
    public override void OnStart()
    {
        inputManager.InputSkillKey(skillIndex.Value, true);
        duration = Duration.Value;
    }
    public override TaskStatus OnUpdate()
    {
        duration = Mathf.Clamp(duration - Time.deltaTime, 0, duration);
        if (duration == 0)
        {
            inputManager.InputSkillKey(skillIndex.Value, false);
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
        Owner.SetVariableValue("GetRPC_PersonBS_Skill1", false);
        controller.Enemy_Controller.inRPC = false;
    }

    // 可选：在Inspector中重置参数
    public override void OnReset()
    {
    }
}
