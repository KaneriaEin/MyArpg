using JKFrame;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Skill/SkillConfig", fileName = "SkillConfig")]
public class SkillConfig : ConfigBase
{
    public Dictionary<SkillCostType, float> releaseCostDic = new Dictionary<SkillCostType, float>();
    public SkillClip[] Clips; // ȫ���ļ���Ƭ��
    public SkillBehaviourBase Behaviour; // ���ܵ������߼�
    public float cdTime;
}
