using Sirenix.Serialization;
using System;
using System.Collections.Generic;

/// <summary>
/// �����Զ����¼�
/// </summary>
public class SkillCustomEventData
{
    /// <summary>
    /// �Զ����¼�
    /// </summary>
    [NonSerialized, OdinSerialize]
    public Dictionary<int, SkillCustomEvent> FrameData = new Dictionary<int, SkillCustomEvent>();
}
