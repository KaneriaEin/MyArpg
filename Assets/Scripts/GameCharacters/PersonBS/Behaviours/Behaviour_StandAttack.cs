using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("��Ϸ��ɫ������ͨ����")]
public class Behaviour_StandAttack : GameCharacterAction
{
    [SerializeField] SharedBool isFinish = false;
    public override void OnStart()
    {
        inputManager.InputStandKey(true);
    }
    public override TaskStatus OnUpdate()
    {
        if (isFinish.Value) inputManager.InputStandKey(false);
        return TaskStatus.Success;
    }

    // ��ѡ����Inspector�����ò���
    public override void OnReset()
    {
    }
}
