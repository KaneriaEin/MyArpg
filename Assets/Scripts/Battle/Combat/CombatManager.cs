using JKFrame;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class CombatEnemySpawn
{
    [LabelText("��������")] public string EnemyName;
    [LabelText("Ŀǰˢ������")] public int SumNum;
    [LabelText("Ŀǰˢ������")] public int DeadNum;
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
    
    [SerializeField, LabelText("����ս��")] private CombatPoint testPoint;


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
            Debug.LogWarning($"ս����ϢΪ��");
            return;
        }
        currentCombatPoint = combatPoint;
        currentCombatWaveIdx = 0;
        StartNewWave();
        isWorking = true;
    }

    private void StartNewWave()
    {
        // ��wave���ö�һ�����CombatManager
        foreach (KeyValuePair<string, CombatEnemySpawnConfig> rule in currentCombatPoint.Waves[currentCombatWaveIdx].enemyRules)
        {
            currentEnemySpawnDic.Add(rule.Key, new CombatEnemySpawn(rule.Key, rule.Value));
            if (!rule.Value.IfSpawn) return;
            // ֱ���ڿ�ʼս��ʱ���ɶ�Ӧ��������OneTimeNum
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
                    // ���combatPoint�Ĺ�ˢ���ˣ�ս�����
                    Debug.Log($"ս�� {currentCombatPoint.CombatPointName} ������");
                    currentCombatPoint = null;
                    currentCombatWaveIdx = -1;
                    isWorking = false;
                    return;
                }
                // ����һ�����ˣ�nextWave
                StartNewWave();
            }
        }
    }

    public void OnEntityDie(string enemyName)
    {
        if(currentEnemySpawnDic.TryGetValue(enemyName, out CombatEnemySpawn spawn))
        {
            // ��Ѱ���Ƿ�����������˵Ĺ�Ϊˢ�������Ĺ֣�����ˢ��һ���������������һ����ʿ����ô����Ҫ�����������������ҵ�������
            foreach (CombatEnemySpawn enemySpawn in currentEnemySpawnDic.Values)
            {
                if(enemySpawn.UnlockEnemies != null && enemySpawn.UnlockEnemies.Count != 0) // �Ƿ���ˢ������
                {
                    if (enemySpawn.UnlockEnemies.TryGetValue(enemyName, out int num)) // ˢ�������Ƿ����������˵Ĺ�
                    {
                        // enemyName �������� = ��ˢ������ - ͬ��������� + 1
                        if (num == spawn.SumNum - spawn.SpawnConfig.OneTimeNum + 1)
                        {
                            enemySpawn.UnlockEnemies.Remove(enemyName);
                            if(enemySpawn.UnlockEnemies.Count == 0)
                            {
                                // ˢ������ȫ��������Ӧ�ÿ�ʼˢ����
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

            // ����ûˢ�����ޣ������ˢ�µ���
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
