using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

[TaskCategory("GameCharacter")]
[TaskDescription("��Ϸ��ɫ��Ŀ���ƶ�")]
public class Behaviour_MoveToTarget : GameCharacterAction
{
    public SharedFloat attackRange = 3f; // �����õĹ�����Χ
    public SharedTransform targetTransform; // Ŀ���Transform
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

        if(distance.Value > attackRange.Value)
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
    
    // ��ѡ����Inspector�����ò���
    public override void OnReset()
    {
        attackRange = 3f;
    }
}
