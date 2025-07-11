using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;

[TaskCategory("GameCharacter")]
[TaskDescription("游戏角色进行重攻击")]
public class Behaviour_HeavyAttack : GameCharacterAction
{
    [SerializeField] SharedFloat Duration;
    private float duration;
    public override void OnStart()
    {
        inputManager.InputHeavyKey(true);
        duration = Duration.Value;
    }
    public override TaskStatus OnUpdate()
    {
        duration = Mathf.Clamp(duration - Time.deltaTime, 0, duration);
        if (duration == 0)
        {
            inputManager.InputHeavyKey(false);
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Running;
        }
    }

    public override void OnEnd()
    {
        inputManager.CleanAllCommandsState();
        controller.Enemy_Controller.inRPC = false;
    }

    // 可选：在Inspector中重置参数
    public override void OnReset()
    {
    }
}
