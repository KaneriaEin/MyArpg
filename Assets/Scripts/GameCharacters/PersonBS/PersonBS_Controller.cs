using BehaviorDesigner.Runtime;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PersonBS_Controller : GameCharacter_Controller
{
    public override void Init(CharacterConfig characterConfig, Enemy_Controller enemy_Controller)
    {
        base.Init(characterConfig, enemy_Controller);

        // 注册PersonBS的一些enemyManager_rpc事件
        enemy_Controller.AddRPCService(GameCharacter_RPCService.RPC_PersonBS_Skill_2, PersonBS_Skill1_RPC_Server);
    }
    public override void ChangeState(GameCharacterState newState, bool reCurrstate = false)
    {
        base.ChangeState(newState, reCurrstate);
        switch (this.gameCharacterState)
        {
            case GameCharacterState.Idle:
                stateMachine.ChangeState<PersonBS_IdleState>(reCurrstate);
                break;
            case GameCharacterState.Move:
                stateMachine.ChangeState<PersonBS_MoveState>(reCurrstate);
                break;
            case GameCharacterState.Skill:
                stateMachine.ChangeState<PersonBS_SkillState>(reCurrstate);
                break;
            case GameCharacterState.Damaged:
                stateMachine.ChangeState<PersonBS_DamagedState>(reCurrstate);
                break;
            case GameCharacterState.Die:
                stateMachine.ChangeState<PersonBS_DieState>(reCurrstate);
                break;
        }
    }

    public override void OnDie(string name)
    {
        enemy_Controller.RemoveRPCService(GameCharacter_RPCService.RPC_PersonBS_Skill_2, PersonBS_Skill1_RPC_Server);
        base.OnDie(name);
    }

    #region enemyManager rpc相关
    /// <summary>
    /// PersonBS发动skill1发号施令时，其他的人处理
    /// 移动到对应位置
    /// </summary>
    public void PersonBS_Skill1_RPC_Server(Enemy_Controller source, RPC_DataInfo info,int index)
    {
        Debug.Log($"RPCSerVer::角色 {name} 接收到了来源是 {source.name} 的 RPC — Skill1_RPC 请求,自己编号是{index}");
        enemy_Controller.inRPC = true;
        behaviorTree.SetVariableValue("MovePosition", info.desPoses[index]);
        behaviorTree.SetVariableValue("GetRPC_PersonBS_Skill1", true);

    }
    #endregion
}
