using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("本角色处于“击晕”状态(Stun)")]
public class Behaviour_InStun : GameCharacterAction
{
    public override void OnStart()
    {
    }
    public override TaskStatus OnUpdate()
    {
        if (controller.GameCharacterState == GameCharacterState.Stun)
        {
            return TaskStatus.Running;
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
