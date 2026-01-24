using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;

[TaskCategory("GameCharacter")]
[TaskDescription("游戏角色进行重攻击")]
public class Behaviour_HeavyAttack : GameCharacterAction
{
    [SerializeField] SharedBool skillState;
    public override void OnStart()
    {
        inputManager.InputHeavyKey(true);
        skillState = Owner.GetVariable("SkillState") as SharedBool;
        skillState.SetValue(true);
    }
    public override TaskStatus OnUpdate()
    {
        if (controller.GameCharacterState == GameCharacterState.Skill)
        {
            inputManager.InputHeavyKey(false);
        }
        if (!skillState.Value && controller.GameCharacterState != GameCharacterState.Charge)
        {
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
