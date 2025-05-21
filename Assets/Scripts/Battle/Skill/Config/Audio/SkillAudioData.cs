using Sirenix.Serialization;
using System;
using System.Collections.Generic;


/// <summary>
/// 技能音效，记录所有的 动画clip与其起始帧
/// </summary>
[Serializable]
public  class SkillAudioData
{
    /// <summary>
    /// 音效帧事件
    /// </summary>
    [NonSerialized, OdinSerialize]
    public List<SkillAudioEvent> FrameData = new List<SkillAudioEvent>();
}
