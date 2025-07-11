using JKFrame;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonMono<EnemyManager>
{
    [ShowInInspector] private Dictionary<GameCharacterType, List<Enemy_Controller>> enemies;
    [ShowInInspector] private Dictionary<GameCharacterType, Dictionary<string, int>> enemyRuntimeDataDic;
    [SerializeField] private Vector3[] spawnPostions;
    public void Init()
    {
        enemies = new Dictionary<GameCharacterType, List<Enemy_Controller>>();
        enemyRuntimeDataDic = new Dictionary<GameCharacterType, Dictionary<string, int>>();
        spawnPostionsIdx = 0;
    }

    public void CreateEnemy(string prefabName, CombatEnemySpawnConfig config, Action<string> onDie, Vector3 position = default)
    {
        // go实例化
        if (position == default) // 若无指定则随机地点刷新
        {
            position = GetSpawnPosition();
        }
        GameObject enemyGo = PoolSystem.GetGameObject(prefabName);
        if (enemyGo == null)
        {
            enemyGo = ResSystem.InstantiateGameObject(prefabName, this.gameObject.transform);
            enemyGo.name = prefabName;
            enemyGo.transform.position = position;
        }
        else
        {
            enemyGo.GetComponent<GameCharacter_Controller>().CharacterController.enabled = false;
            enemyGo.transform.position = position;
            enemyGo.transform.SetParent(this.gameObject.transform, true);
            enemyGo.GetComponent<GameCharacter_Controller>().CharacterController.enabled = true;
        }


        // 初始化操作
        Enemy_Controller enemy_Controller = enemyGo.GetComponent<Enemy_Controller>();
        CharacterConfig characterConfig = ResSystem.LoadAsset<CharacterConfig>(enemy_Controller.characterConfigName);
        enemy_Controller.Init(characterConfig, onDie);

        // 进场特效
        if(config.ShowUpEffect != null)
        {
            GameObject effectObj = PoolSystem.GetGameObject(config.ShowUpEffect.name);
            if (effectObj == null)
            {
                effectObj = GameObject.Instantiate(config.ShowUpEffect);
                effectObj.name = config.ShowUpEffect.name;
            }
            effectObj.transform.position = enemyGo.transform.position;
            effectObj.transform.rotation = Quaternion.Euler(enemyGo.transform.eulerAngles);
            StartCoroutine(AutoDestructEffectGameObject(3f, effectObj));
        }

        // 添加进链表记录
        if (enemies.ContainsKey(enemy_Controller.EnemyType))
        {
            enemies[enemy_Controller.EnemyType].Add(enemy_Controller);
        }
        else
        {
            enemies.Add(enemy_Controller.EnemyType, new List<Enemy_Controller>());
            enemies[enemy_Controller.EnemyType].Add(enemy_Controller);
        }
        if (characterConfig.enemyRuntimeSharedData != null)
        {
            if (!enemyRuntimeDataDic.TryGetValue(enemy_Controller.EnemyType, out Dictionary<string, int> data))
            {
                enemyRuntimeDataDic.Add(enemy_Controller.EnemyType, characterConfig.enemyRuntimeSharedData);
            }
        }


    }

    public void RemoveEnemy(Enemy_Controller enemy_Controller)
    {
        enemies[enemy_Controller.EnemyType].Remove(enemy_Controller);
        if (enemies[enemy_Controller.EnemyType].Count == 0)
        {
            enemies.Remove(enemy_Controller.EnemyType);
            enemyRuntimeDataDic.Remove(enemy_Controller.EnemyType);
        }
        PoolSystem.PushGameObject(enemy_Controller.gameObject);
    }

    [SerializeField] private int spawnPostionsIdx;
    private Vector3 GetSpawnPosition()
    {
        Vector3 pos = spawnPostions[spawnPostionsIdx];
        spawnPostionsIdx = (spawnPostionsIdx + 1) % spawnPostions.Length;
        return pos;
    }

    private IEnumerator AutoDestructEffectGameObject(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.GameObjectPushPool();
    }

    #region RPC相关
    public void EnemyController_RPC_Client(GameCharacter_RPCService service, Enemy_Controller source, RPC_DataInfo rpcInfo, GameCharacterType enemyType, int maxNum)
    {
        Debug.Log($"RPC_Client::角色{source.name}发起了rpc“{service}”请求，目标类型是{enemyType}，目标数是{maxNum}");
        int num = 0;
        for (int i = 0; i < enemies[enemyType].Count; i++)
        {
            if(enemies[enemyType][i] == source) continue;
            if(enemies[enemyType][i].inRPC) continue;
            if(enemies[enemyType][i].EnemyCharacterState == GameCharacterState.Damaged || enemies[enemyType][i].EnemyCharacterState == GameCharacterState.Die) continue;
            enemies[enemyType][i].EnemyController_RPC_Service(service,source,rpcInfo,num);
            num ++; if (num >= maxNum) break;
        }
    }

    /// <summary>
    /// 敌人单位用于获取一些共享数据pv操作，默认获取1个
    /// </summary>
    public bool EnemyController_GetSharedData(Enemy_Controller source, string dataName, int count = 1)
    {
        Debug.Log($"RPC_Client::角色{source.name}请求“{dataName}”信号量，需求个数是{count}");
        if(enemyRuntimeDataDic.TryGetValue(source.EnemyType, out Dictionary<string, int> sharedData))
        {
            if (sharedData.ContainsKey(dataName))
            {
                if(sharedData[dataName] >= count)
                {
                    sharedData[dataName] -= count;
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 敌人单位用于归还一些共享数据pv操作，默认归还1个
    /// </summary>
    public void EnemyController_ReleaseSharedData(Enemy_Controller source, string dataName, int count = 1)
    {
        Debug.Log($"RPC_Client::角色{source.name}归还“{dataName}”信号量，个数是{count}");
        if(enemyRuntimeDataDic.TryGetValue(source.EnemyType, out Dictionary<string, int> sharedData))
        {
            if (sharedData.ContainsKey(dataName))
            {
                sharedData[dataName] += count;
            }
        }
    }
    #endregion
}

public class RPC_DataInfo
{
    public Enemy_Controller source;
    public int[] serverIdx;
    public Vector3 desPos;
    public Vector3[] desPoses;
    public int skillIndex;
}

