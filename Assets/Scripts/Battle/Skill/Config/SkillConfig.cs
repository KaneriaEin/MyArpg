using JKFrame;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Skill/SkillConfig", fileName = "SkillConfig")]
public class SkillConfig : ConfigBase
{
    public Dictionary<SkillCostType, float> releaseCostDic = new Dictionary<SkillCostType, float>();
    public SkillClip[] Clips; // 全部的技能片段
    public SkillBehaviourBase Behaviour; // 技能的运行逻辑
    public float cdTime;
}
