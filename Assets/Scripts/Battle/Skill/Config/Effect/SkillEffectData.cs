using Sirenix.Serialization;
using System;
using System.Collections.Generic;


/// <summary>
/// 技能特效
/// </summary>
[Serializable]
public  class SkillEffectData
{
    /// <summary>
    /// 特效帧事件
    /// </summary>
    [NonSerialized, OdinSerialize]
    public List<SkillEffectEvent> FrameData = new List<SkillEffectEvent>();
}
