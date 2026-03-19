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
    public GameObject DollyTrackPrefab;
    public AnimationCurve DollyPosCurve;
    public AnimationCurve DollyFovCurve;
    public AnimationCurve DollyDutchCurve;
    public AnimationCurve DollyXOffsetCurve;
    public AnimationCurve DollyYOffsetCurve;
    public AnimationCurve DollyZOffsetCurve;
}
