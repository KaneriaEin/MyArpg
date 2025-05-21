using UnityEngine;

/// <summary>
/// 音效帧事件
/// </summary>
public class SkillEffectEvent : SkillFrameEventBase
{
#if UNITY_EDITOR
    public string TrackName = "特效轨道";
#endif
    public int FrameIndex = -1;
    public GameObject Prefab;
    public int Duration;
    public bool AutoDestruct;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
}
