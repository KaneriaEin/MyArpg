using Sirenix.Serialization;
using System;
using System.Collections.Generic;
/// <summary>
/// ���ܶ�������¼���е� ����clip������ʼ֡
/// </summary>
[Serializable]
public class SkillAnimationData
{
    /// <summary>
    /// ����֡�¼�
    /// KEy:֡��
    /// Value���¼�����
    /// </summary>
    [NonSerialized, OdinSerialize]
    public Dictionary<int, SkillAnimationEvent> FrameData = new Dictionary<int, SkillAnimationEvent>();
}
