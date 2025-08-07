using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("本角色是否进入“受伤”状态(GameCharacterState.Damaged)")]
public class Behaviour_IsDamaged : GameCharacterConditional
{
    public override void OnStart()
    {
    }
    public override TaskStatus OnUpdate()
    {
        if (controller.GameCharacterState == GameCharacterState.Damaged)
        {
            controller.CommandController.CleanAllCommandsState();
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
    
    public override void OnReset()
    {
    }
}
