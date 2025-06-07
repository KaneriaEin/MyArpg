using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("�ж�Ŀ���Ƿ񳬳���������Χ")]
public class Behaviour_IsTargetOutOfRange : GameCharacterConditional
{
    public SharedFloat battleRange = 8f; // �����õĹ�����Χ
    public SharedTransform targetTransform; // Ŀ���Transform

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
        
        // �������С�ڵ��ڹ�����Χ�����سɹ�
        return distance > battleRange.Value ? TaskStatus.Success : TaskStatus.Failure;
    }
    
    // ��ѡ����Inspector�����ò���
    public override void OnReset()
    {
        battleRange = 3f;
        targetTransform = null;
    }
}
