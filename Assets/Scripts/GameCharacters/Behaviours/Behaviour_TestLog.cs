using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("游戏调试Log Action")]
public class Behaviour_TestLog : GameCharacterAction
{
    [SerializeField] string LogText;
    [SerializeField] string EnemyName;
    public override void OnStart()
    {
        //Debug.Log($"敌人{controller.name}行为树开始。重生世界位置是{controller.transform.position},local位置是{controller.transform.localPosition}");
        controller.CommandController.CleanAllCommandsState();
        if(LogText != null) Debug.Log(LogText);
    }
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }

    // 可选：在Inspector中重置参数
    public override void OnReset()
    {
    }
}
