using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("游戏角色朝目标Pos移动")]
public class Behaviour_MoveToPosition : GameCharacterAction
{
    public Vector3 desPos; // 目标Position
    public SharedFloat distance;

    public override void OnStart()
    {
        desPos = (Owner.GetVariable("MovePosition") as SharedVector3).Value;
        inputManager.InputMoveInput(new Vector2(0, 1));
    }
    public override TaskStatus OnUpdate()
    {
        if (desPos == null)
        {
            return TaskStatus.Failure;
        }
        
        controller.ModelTransform.LookAt(desPos);
        distance = Vector3.Distance(transform.position, desPos);

        if(distance.Value > 1f)
        {
            inputManager.InputMoveInput(new Vector2(0, 1));
            return TaskStatus.Running;
        }
        else
        {
            inputManager.InputMoveInput(new Vector2(0, 0));
            controller.ModelTransform.LookAt(controller.Target.ModelTransform);
            return TaskStatus.Success;
        }
    }
    
    // 可选：在Inspector中重置参数
    public override void OnReset()
    {
    }
}
