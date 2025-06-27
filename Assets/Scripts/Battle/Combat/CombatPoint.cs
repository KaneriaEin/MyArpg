using JKFrame;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Combat/CombatPoint", fileName = "CombatPoint")]
public class CombatPoint : ConfigBase
{
    [LabelText("ս������")] public string CombatPointName;
    public CombatWave[] Waves; // ȫ���ĵ��˲���
}
