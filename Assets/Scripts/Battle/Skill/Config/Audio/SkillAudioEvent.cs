using UnityEngine;
/// <summary>
/// 音效帧事件
/// </summary>
public class SkillAudioEvent : SkillFrameEventBase
{
#if UNITY_EDITOR
    public string TrackName = "音效轨道";
#endif
    public int FrameIndex = -1;
    public AudioClip AudioClip;
    public float Volume = 1;
}
