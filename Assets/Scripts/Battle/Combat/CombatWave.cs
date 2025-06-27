using JKFrame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct CombatEnemySpawnConfig
{
    [LabelText("�������")] public int MaxNum;
    [LabelText("ͬ������")] public int OneTimeNum;
    [LabelText("�Ƿ�����ˢ��")] public bool IfSpawn;
    [LabelText("ˢ������ (x �� y ��)")] public Dictionary<string, int> UnlockCondition;
    [LabelText("������Ч")] public GameObject ShowUpEffect;
}

[CreateAssetMenu(menuName = "Config/Combat/CombatWave", fileName = "CombatWave")]
public class CombatWave : ConfigBase
{
    [NonSerialized, OdinSerialize] public Dictionary<string, CombatEnemySpawnConfig> enemyRules;

}
