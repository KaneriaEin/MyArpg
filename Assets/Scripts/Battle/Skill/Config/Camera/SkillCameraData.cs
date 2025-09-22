using Cinemachine;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能自定义事件
/// </summary>
public class SkillCameraData
{
    /// <summary>
    /// 相机事件
    /// </summary>
    [NonSerialized, OdinSerialize]
    public Dictionary<int, SkillCameraEvent> CartPostionData = new Dictionary<int, SkillCameraEvent>();// <Frame, CartPosition>
    public GameObject DollyTrackPrefab;
}
