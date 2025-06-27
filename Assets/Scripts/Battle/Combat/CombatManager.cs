using JKFrame;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class CombatEnemySpawn
{
    [LabelText("怪物名称")] public string EnemyName;
    [LabelText("目前刷出数量")] public int SumNum;
    [LabelText("目前刷出数量")] public int DeadNum;
    public CombatEnemySpawnConfig SpawnConfig;
    public Dictionary<string, int> UnlockEnemies;

    public CombatEnemySpawn(string name, CombatEnemySpawnConfig rule)
    {
        EnemyName = name;
        SumNum = rule.OneTimeNum;
        SpawnConfig = rule;
        DeadNum = 0;
        if (rule.UnlockCondition != null)
        {
            UnlockEnemies = new Dictionary<string, int>(rule.UnlockCondition);
        }
        else
        {
            UnlockEnemies = null;
        }
    }
}

public class CombatManager : SingletonMono<CombatManager>
{
    private bool isWorking = false;
    [ShowInInspector] private CombatPoint currentCombatPoint;
    [ShowInInspector] private int currentCombatWaveIdx;
    [ShowInInspector] private Dictionary<string, CombatEnemySpawn> currentEnemySpawnDic;
    
    [SerializeField, LabelText("测试战点")] private CombatPoint testPoint;


    public void Init()
    {
        currentEnemySpawnDic = new Dictionary<string, CombatEnemySpawn>();
        StartNewCombat(testPoint);
    }

    public void StartNewCombat(CombatPoint combatPoint)
    {
        if (combatPoint == null || combatPoint.Waves.Length == 0)
        {
            isWorking = false;
            currentCombatPoint = null;
            currentCombatWaveIdx = -1;
            Debug.LogWarning($"战点信息为空");
            return;
        }
        currentCombatPoint = combatPoint;
        currentCombatWaveIdx = 0;
        StartNewWave();
        isWorking = true;
    }

    private void StartNewWave()
    {
        // 把wave配置读一遍读进CombatManager
        foreach (KeyValuePair<string, CombatEnemySpawnConfig> rule in currentCombatPoint.Waves[currentCombatWaveIdx].enemyRules)
        {
            currentEnemySpawnDic.Add(rule.Key, new CombatEnemySpawn(rule.Key, rule.Value));
            if (!rule.Value.IfSpawn) return;
            // 直接在开始战斗时生成对应数量敌人OneTimeNum
            for (int i = 0; i < rule.Value.OneTimeNum; i++)
            {
                //EnemyManager.Instance.CreateEnemy(rule.Key, OnEntityDie, new Vector3(4, 2.5f, 8));
                EnemyManager.Instance.CreateEnemy(rule.Key, rule.Value, OnEntityDie);
            }
        }
    }

    private void Update()
    {
        if (isWorking)
        {
            if(currentEnemySpawnDic.Count == 0)
            {
                currentCombatWaveIdx++;
                if(currentCombatWaveIdx >= currentCombatPoint.Waves.Length)
                {
                    // 这个combatPoint的怪刷完了，战点结束
                    Debug.Log($"战点 {currentCombatPoint.CombatPointName} 结束！");
                    currentCombatPoint = null;
                    currentCombatWaveIdx = -1;
                    isWorking = false;
                    return;
                }
                // 换下一波敌人，nextWave
                StartNewWave();
            }
        }
    }

    public void OnEntityDie(string enemyName)
    {
        if(currentEnemySpawnDic.TryGetValue(enemyName, out CombatEnemySpawn spawn))
        {
            // 先寻找是否有以这个死了的怪为刷新条件的怪，例如刷新一个死神的条件是死一个骑士，那么就需要把这个死神的条件给找到并解锁
            foreach (CombatEnemySpawn enemySpawn in currentEnemySpawnDic.Values)
            {
                if(enemySpawn.UnlockEnemies != null && enemySpawn.UnlockEnemies.Count != 0) // 是否有刷新条件
                {
                    if (enemySpawn.UnlockEnemies.TryGetValue(enemyName, out int num)) // 刷新条件是否包含这个死了的怪
                    {
                        // enemyName 死亡总数 = 总刷新数量 - 同场最大数量 + 1
                        if (num == spawn.SumNum - spawn.SpawnConfig.OneTimeNum + 1)
                        {
                            enemySpawn.UnlockEnemies.Remove(enemyName);
                            if(enemySpawn.UnlockEnemies.Count == 0)
                            {
                                // 刷新条件全部解锁，应该开始刷新了
                                for (int i = 0; i < enemySpawn.SpawnConfig.OneTimeNum; i++)
                                {
                                    //EnemyManager.Instance.CreateEnemy(enemySpawn.EnemyName, OnEntityDie, new Vector3(4, 2.5f, 8));
                                    EnemyManager.Instance.CreateEnemy(enemySpawn.EnemyName, enemySpawn.SpawnConfig, OnEntityDie);
                                }
                            }
                        }
                    }
                }
            }

            // 若还没刷到上限，则继续刷新敌人
            if(spawn.SumNum < spawn.SpawnConfig.MaxNum)
            {
                //EnemyManager.Instance.CreateEnemy(enemyName, OnEntityDie, new Vector3(4, 2.5f, 8));
                EnemyManager.Instance.CreateEnemy(enemyName, spawn.SpawnConfig, OnEntityDie);
                spawn.SumNum++;
            }
            spawn.DeadNum++;
            if (spawn.DeadNum == spawn.SpawnConfig.MaxNum)
            {
                currentEnemySpawnDic.Remove(enemyName);
            }
        }
    }

}
