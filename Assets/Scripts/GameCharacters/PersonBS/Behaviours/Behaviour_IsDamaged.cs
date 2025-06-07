using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("����ɫ�Ƿ���롰���ˡ�״̬(GameCharacterState.Damaged)")]
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
