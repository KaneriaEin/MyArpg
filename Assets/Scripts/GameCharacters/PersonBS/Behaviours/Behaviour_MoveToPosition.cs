using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("��Ϸ��ɫ��Ŀ���ƶ�")]
//TODO:copy from Behaviour_MoveToTarget�� δ�޸�
public class Behaviour_MoveToPosition : GameCharacterAction
{
    public SharedFloat battleRange = 6f; // �����õĹ�����Χ
    public Vector3 positionTransform; // Ŀ���Transform
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
    
    // ��ѡ����Inspector�����ò���
    public override void OnReset()
    {
        battleRange = 3f;
    }
}
