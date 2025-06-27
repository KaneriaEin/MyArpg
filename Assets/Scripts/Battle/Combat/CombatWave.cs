using JKFrame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct CombatEnemySpawnConfig
{
    [LabelText("最大数量")] public int MaxNum;
    [LabelText("同屏数量")] public int OneTimeNum;
    [LabelText("是否立刻刷出")] public bool IfSpawn;
    [LabelText("刷出条件 (x 个 y 怪)")] public Dictionary<string, int> UnlockCondition;
    [LabelText("出场特效")] public GameObject ShowUpEffect;
}

[CreateAssetMenu(menuName = "Config/Combat/CombatWave", fileName = "CombatWave")]
public class CombatWave : ConfigBase
{
    [NonSerialized, OdinSerialize] public Dictionary<string, CombatEnemySpawnConfig> enemyRules;

}
