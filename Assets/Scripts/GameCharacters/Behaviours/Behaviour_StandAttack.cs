using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("GameCharacter")]
[TaskDescription("游戏角色进行普通攻击")]
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

    // 可选：在Inspector中重置参数
    public override void OnReset()
    {
    }
}
