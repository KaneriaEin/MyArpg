using UnityEngine;
/// <summary>
/// ¶¯»­Ö¡ÊÂ¼þ
/// </summary>
public class SkillAnimationEvent : SkillFrameEventBase
{
    public AnimationClip AnimationClip;
    public bool ApplyRootMotion;
    public bool MainWeaponOnLeftHand = true;
    public float TransitionTime = 0.25f;
#if UNITY_EDITOR
    public int DurationFrame;
#endif
}
