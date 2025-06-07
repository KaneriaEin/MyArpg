using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("游戏角色朝目标移动")]
//TODO:copy from Behaviour_MoveToTarget， 未修改
public class Behaviour_MoveToPosition : GameCharacterAction
{
    public SharedFloat battleRange = 6f; // 可配置的攻击范围
    public Vector3 positionTransform; // 目标的Transform
    public SharedFloat distance;

    public override void OnStart()
    {
        positionTransform = controller.Target.ModelTransform.position;
        inputManager.InputMoveInput(new Vector2(0, 1));
    }
    public override TaskStatus OnUpdate()
    {
        if (positionTransform == null)
        {
            return TaskStatus.Failure;
        }
        
        controller.ModelTransform.LookAt(controller.Target.ModelTransform);
        distance = Vector3.Distance(transform.position, positionTransform);

        if(distance.Value > battleRange.Value)
        {
            inputManager.InputMoveInput(new Vector2(0, 1));
            return TaskStatus.Running;
        }
        else
        {
            inputManager.InputMoveInput(new Vector2(0, 0));
            return TaskStatus.Success;
        }
    }
    
    // 可选：在Inspector中重置参数
    public override void OnReset()
    {
        battleRange = 3f;
    }
}
