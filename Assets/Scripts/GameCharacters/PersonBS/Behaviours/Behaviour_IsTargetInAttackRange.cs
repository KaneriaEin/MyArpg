using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter/Conditional")]
[TaskDescription("判断目标是否在自身攻击范围内")]
public class Behaviour_IsTargetInAttackRange : GameCharacterConditional
{
    public SharedFloat attackRange = 3f; // 可配置的攻击范围
    public SharedTransform targetTransform; // 目标的Transform

    public override void OnStart()
    {
        targetTransform = controller.transform;
    }
    public override TaskStatus OnUpdate()
    {
        if (targetTransform.Value == null)
        {
            return TaskStatus.Failure;
        }
        
        float distance = Vector3.Distance(transform.position, targetTransform.Value.position);
        
        // 如果距离小于等于攻击范围，返回成功
        return distance <= attackRange.Value ? TaskStatus.Success : TaskStatus.Failure;
    }
    
    // 可选：在Inspector中重置参数
    public override void OnReset()
    {
        attackRange = 3f;
        targetTransform = null;
    }
}
