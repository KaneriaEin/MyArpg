using Sirenix.Serialization;
using System;
using System.Collections.Generic;

/// <summary>
/// 技能自定义事件
/// </summary>
public class SkillCustomEventData
{
    /// <summary>
    /// 自定义事件
    /// </summary>
    [NonSerialized, OdinSerialize]
    public Dictionary<int, SkillCustomEvent> FrameData = new Dictionary<int, SkillCustomEvent>();
}
