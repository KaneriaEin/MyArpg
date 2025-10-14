using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Sirenix.OdinInspector;

[TaskCategory("GameCharacter")]
[TaskDescription("本角色是否接收到rpc请求")]
public class Behaviour_IsGetEnemyRpcCall : GameCharacterConditional
{
    [ShowInInspector] SharedBool isGetRPCCall;
    public override void OnStart()
    {
        isGetRPCCall = Owner.GetVariable("GetRPC_PersonBS_Skill1") as SharedBool;
        if (isGetRPCCall == null) return;
    }
    public override TaskStatus OnUpdate()
    {
        if (isGetRPCCall != null && isGetRPCCall.Value)
        {
            //Owner.SetVariableValue("GetRPC_PersonBS_Skill1", false);
            controller.CommandController.CleanAllCommandsState();
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
    
    public override void OnReset()
    {
    }
}
