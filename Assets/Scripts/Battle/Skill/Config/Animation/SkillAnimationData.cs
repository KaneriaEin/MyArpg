using Sirenix.Serialization;
using System;
using System.Collections.Generic;
/// <summary>
/// 技能动画，记录所有的 动画clip与其起始帧
/// </summary>
[Serializable]
public class SkillAnimationData
{
    /// <summary>
    /// 动画帧事件
    /// KEy:帧数
    /// Value：事件数据
    /// </summary>
    [NonSerialized, OdinSerialize]
    public Dictionary<int, SkillAnimationEvent> FrameData = new Dictionary<int, SkillAnimationEvent>();
}
