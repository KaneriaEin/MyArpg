using JKFrame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Config/Skill/SkillClip", fileName ="SkillClip")]
public class SkillClip : ConfigBase
{
    [LabelText("技能名称")] public string SkillName;
    [LabelText("帧数上限")] public int FrameCount = 100;
    [LabelText("帧率")] public int FrameRate = 30;
    [LabelText("可接技能")] public string[] FollowUp;


    [NonSerialized, OdinSerialize] public SkillAnimationData SkillAnimationData = new SkillAnimationData();
    [NonSerialized, OdinSerialize] public SkillAudioData SkillAudioData = new SkillAudioData();
    [NonSerialized, OdinSerialize] public SkillEffectData SkillEffectData = new SkillEffectData();
    [NonSerialized, OdinSerialize] public SkillAttackDetectionData SkillAttackDetectionData = new SkillAttackDetectionData();
    [NonSerialized, OdinSerialize] public SkillCustomEventData SkillCustomEventData = new SkillCustomEventData();

#if UNITY_EDITOR
    private static Action SkillConfigValidate;
    public static void SetValidateAction(Action action)
    {
        SkillConfigValidate = action;
    }
    private void OnValidate()
    {
        SkillConfigValidate?.Invoke();
    }
#endif
}
