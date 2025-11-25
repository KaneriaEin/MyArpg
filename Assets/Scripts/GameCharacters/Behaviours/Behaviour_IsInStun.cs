using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("本角色是否进入“击晕”状态(Stun)")]
public class Behaviour_IsInStun : GameCharacterConditional
{
    public override void OnStart()
    {
    }
    public override TaskStatus OnUpdate()
    {
        if (controller.CharacterProperties.InStun())
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
