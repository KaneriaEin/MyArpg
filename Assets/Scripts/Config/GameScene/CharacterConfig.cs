using JKFrame;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterConfig", menuName = "Config/CharacterConfig")]
public class CharacterConfig : ConfigBase
{
    [LabelText("走路速度")] public float WalkSpeed;
    [LabelText("跑步速度")] public float RunSpeed;
    [LabelText("旋转速度")] public float RotateSpeed;
    [LabelText("走路到奔跑过渡速度")] public float Walk2RunTransitionSpeed;
    [LabelText("脚步声资源")] public AudioClip[] FootStepAudioClips;
    [LabelText("应用RootMotion")] public bool ApplyRootMotionForMove;

    [LabelText("标准动画表")] public Dictionary<string, AnimationClip> StandAnimationDic;
    [LabelText("全部技能")] public List<SkillConfig> skillConfigList;
    [LabelText("基础攻击力")] public float atkBaseValue;
    [LabelText("基础生命值")] public float hpBaseValue;
    [LabelText("基础魔法值")] public float mpBaseValue;


    public AnimationClip GetAnimationByName(string name)
    {
        return StandAnimationDic[name];
    }
}
