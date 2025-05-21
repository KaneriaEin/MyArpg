using Sirenix.Serialization;
using System;
using System.Collections.Generic;

public class SkillAttackDetectionData
{
    /// <summary>
    /// 攻击检测事件
    /// </summary>
    [NonSerialized, OdinSerialize]
    public List<SkillAttackDetectionEvent> FrameData = new List<SkillAttackDetectionEvent>();
}
