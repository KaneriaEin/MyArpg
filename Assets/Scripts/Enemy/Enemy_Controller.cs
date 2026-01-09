using JKFrame;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy_Controller : MonoBehaviour
{
    [SerializeField] private GameCharacter_Controller gameCharacter;
    [SerializeField] public string characterConfigName;
    [SerializeField] public bool inRPC;
    private GameCharacterType enemyType;
    public GameCharacterState EnemyCharacterState { get => gameCharacter.GameCharacterState; }

    public GameCharacterType EnemyType { get { return enemyType; } }

    public GameCharacter_Controller GameCharacter { get { return gameCharacter; } }

    public void Init(CharacterConfig characterConfig, Action<string> dieAction = null)
    {
        gameCharacter.Init(characterConfig,this);
        
        this.enemyType = characterConfig.characterType;
        gameCharacter.OnDieAction += DestroyEnemy;
        gameCharacter.OnDieAction += dieAction;
    }

    public void DestroyEnemy(string name)
    {
        gameCharacter.OnDieAction = null;
        EnemyManager.Instance.RemoveEnemy(this);
    }

    #region rpc相关
    private Dictionary<GameCharacter_RPCService, Action<Enemy_Controller, RPC_DataInfo, int>> rpcServiceDic = new Dictionary<GameCharacter_RPCService, Action<Enemy_Controller, RPC_DataInfo, int>>();

    /// <summary>
    /// 服务端rpc接收处理
    /// </summary>
    /// <param name="service">rpc服务名称</param>
    /// <param name="source">发起rpc的来源对象</param>
    /// <param name="rpcInfo">rpc传参</param>
    /// <param name="index">多目标rpc时自己的编号</param>
    public void EnemyController_RPC_Service(GameCharacter_RPCService service, Enemy_Controller source, RPC_DataInfo rpcInfo, int index)
    {
        if (rpcServiceDic.TryGetValue(service, out Action<Enemy_Controller, RPC_DataInfo, int> _action))
        {
            _action?.Invoke(source,rpcInfo,index);
        }
    }

    public void AddRPCService(GameCharacter_RPCService serviceName, Action<Enemy_Controller, RPC_DataInfo, int> action)
    {
        if (rpcServiceDic.TryGetValue(serviceName, out Action<Enemy_Controller, RPC_DataInfo, int> _action))
        {
            _action += action;
        }
        else
        {
            rpcServiceDic.Add(serviceName, action);
        }
    }

    public void RemoveRPCService(GameCharacter_RPCService serviceName)
    {
        rpcServiceDic.Remove(serviceName);
    }

    public void RemoveRPCService(GameCharacter_RPCService serviceName, Action<Enemy_Controller, RPC_DataInfo, int> action)
    {
        if (rpcServiceDic.TryGetValue(serviceName, out Action<Enemy_Controller, RPC_DataInfo, int> _action))
        {
            _action -= action;
        }
    }

    public void CleanAllRPCService()
    {
        rpcServiceDic.Clear();
    }
    #endregion
}
