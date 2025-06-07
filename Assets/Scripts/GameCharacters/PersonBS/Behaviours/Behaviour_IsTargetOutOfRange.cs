using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("判断目标是否超出自身攻击范围")]
public class Behaviour_IsTargetOutOfRange : GameCharacterConditional
{
    public SharedFloat battleRange = 8f; // 可配置的攻击范围
    public SharedTransform targetTransform; // 目标的Transform

    public override void OnStart()
    {
        targetTransform = controller.Target.ModelTransform;
    }
    public override TaskStatus OnUpdate()
    {
        if (targetTransform.Value == null)
        {
            return TaskStatus.Failure;
        }
        
        float distance = Vector3.Distance(transform.position, targetTransform.Value.position);
        
        // 如果距离小于等于攻击范围，返回成功
        return distance > battleRange.Value ? TaskStatus.Success : TaskStatus.Failure;
    }
    
    // 可选：在Inspector中重置参数
    public override void OnReset()
    {
        battleRange = 3f;
        targetTransform = null;
    }
}
