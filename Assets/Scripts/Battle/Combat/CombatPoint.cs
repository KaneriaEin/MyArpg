using JKFrame;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Combat/CombatPoint", fileName = "CombatPoint")]
public class CombatPoint : ConfigBase
{
    [LabelText("战点名称")] public string CombatPointName;
    public CombatWave[] Waves; // 全部的敌人波数
}
