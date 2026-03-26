using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("游戏角色朝目标走动")]
public class Behaviour_WalkToTarget : GameCharacterAction
{
    public SharedFloat battleRange = 3f; // 可配置的攻击范围
    public SharedTransform targetTransform; // 目标的Transform
    public SharedFloat distance;

    public override void OnStart()
    {
        targetTransform = controller.Target.ModelTransform;
        inputManager.InputMoveInput(new Vector2(0, 1));
    }
    public override TaskStatus OnUpdate()
    {
        if (targetTransform.Value == null)
        {
            return TaskStatus.Failure;
        }
        
        controller.ModelTransform.LookAt(controller.Target.ModelTransform);
        distance = Vector3.Distance(transform.position, targetTransform.Value.position);

        if(distance.Value > battleRange.Value)
        {
            inputManager.InputWalkKey(true);
            inputManager.InputMoveInput(new Vector2(0, 1));
            return TaskStatus.Running;
        }
        else
        {
            inputManager.InputMoveInput(new Vector2(0, 0));
            inputManager.InputWalkKey(false);
            return TaskStatus.Success;
        }
    }
    
    // 可选：在Inspector中重置参数
    public override void OnReset()
    {
        battleRange = 3f;
    }
}
