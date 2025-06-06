using JKFrame;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// 技能播放器
/// </summary>
public class Skill_Player : SerializedMonoBehaviour
{
    private Animation_Controller animation_Controller;

    private bool isPlaying = false;           // 当前是否处于播放状态
    public bool IsPlaying { get { return isPlaying; } }

    private SkillClip skillClip;          // 当前播放的技能配置
    private int currentFrameIndex;            // 当前是第几帧
    private float playTotalTime;            // 当前播放的总时间
    private float frameRate;                  // 当前技能的帧率

    private Transform modelTransform;
    public Transform ModelTransform { get { return modelTransform; } }
    public LayerMask attackDetectionLayer;
    private ICharacter owner;

    public void Init(ICharacter owner, Animation_Controller animation_Controller, Transform modelTransform)
    {
        this.owner = owner;
        this.animation_Controller = animation_Controller;
        this.modelTransform = modelTransform;
        foreach (WeaponController item in WeaponDic.Values)
        {
            item.Init(attackDetectionLayer, OnWeaponDetection);
        }
    }

    #region 武器
    [SerializeField] private ParentConstraint mainWeaponParentContraint;
    [SerializeField] private Dictionary<string, WeaponController> weaponDic = new Dictionary<string, WeaponController>();
    public Dictionary<string, WeaponController> WeaponDic { get { return weaponDic; } }
    public ParentConstraint MainWeaponParentContraint { get { return mainWeaponParentContraint; } }
    public void SetMainWeaponHand(bool isLeft)
    {
        if (mainWeaponParentContraint == null) return;
        ConstraintSource left = mainWeaponParentContraint.GetSource(0);
        ConstraintSource right = mainWeaponParentContraint.GetSource(1);
        left.weight = isLeft ? 1 : 0;
        right.weight = isLeft ? 0 : 1;
        mainWeaponParentContraint.SetSource(0, left);
        mainWeaponParentContraint.SetSource(1, right);
    }

    private void OnWeaponDetection(IHitTarget target, AttackData attackData)
    {
        skillBehaviour.OnAttackDetection(target, attackData);
    }
    #endregion

    private SkillBehaviourBase skillBehaviour;

    public void StartPlayerSkillConfig(SkillBehaviourBase skillBehaviour)
    {
        this.skillBehaviour = skillBehaviour;
    }

    /// <summary>
    /// 播放技能片段
    /// </summary>
    /// <param name="skillClip">技能配置</param>
    public void PlaySkillClip(SkillClip skillClip)
    {
        this.skillClip = skillClip;
        currentFrameIndex = -1;
        frameRate = skillClip.FrameRate;
        playTotalTime = 0;
        isPlaying = true;
        TickSkill();
    }

    /// <summary>
    /// 中止技能片段
    /// </summary>
    /// <param name="skillClip">技能配置</param>
    public void StopSkillClip()
    {
        isPlaying = false;
        Clean();
    }

    private void Clean()
    {
        skillClip = null;
    }

    private void Update()
    {
        //Debug.Log("playerTotalTime:" + playTotalTime);
        if (isPlaying)
        {
            playTotalTime += Time.deltaTime;
            // 根据总时间判断当前是第几帧
            int targetFrameIndex = (int)(playTotalTime * frameRate);
            // 防止一帧延迟过大，追帧
            while (currentFrameIndex < targetFrameIndex)
            {
                // 驱动一次技能
                TickSkill();
            }
            // 如果到达最后一帧，技能结束
            if (targetFrameIndex >= skillClip.FrameCount)
            {
                isPlaying = false;
                skillBehaviour.OnSkillClipEnd();
                Clean();
            }
        }
    }

    private void TickSkill()
    {
        currentFrameIndex += 1;
        skillBehaviour.OnTickSkill(currentFrameIndex);
        TickSkillCustomEvent();
        TickSkillAnimationEvent();
        TickSkillAudioEvent();
        TickSkillEffectEvent();
        TickSkillAttackDetectionEvent();
    }

    private void TickSkillCustomEvent()
    {
        if(skillClip.SkillCustomEventData.FrameData.TryGetValue(currentFrameIndex, out SkillCustomEvent skillCustomEvent))
        {
            skillCustomEvent = skillBehaviour.BeforeSkillCustomEvent(skillCustomEvent);
            if (skillCustomEvent != null)
            {
                skillBehaviour.AfterSkillCustomEvent(skillCustomEvent);
            }
        }
    }

    private void TickSkillAnimationEvent()
    {
        // 驱动动画
        if (animation_Controller != null && skillClip.SkillAnimationData.FrameData.TryGetValue(currentFrameIndex, out SkillAnimationEvent skillAnimationEvent))
        {
            skillAnimationEvent = skillBehaviour.BeforeSkillAnimationEvent(skillAnimationEvent);
            if (skillAnimationEvent != null)
            {
                SetMainWeaponHand(skillAnimationEvent.MainWeaponOnLeftHand);
                animation_Controller.PlaySingleAnimation(skillAnimationEvent.AnimationClip, 1, true, 0.01f);

                if (skillAnimationEvent.ApplyRootMotion)
                {
                    animation_Controller.SetRootMotionAction(skillBehaviour.OnRootMotion);
                }
                else
                {
                    animation_Controller.ClearRootMotionAction();
                }
                skillBehaviour.AfterSkillAnimationEvent(skillAnimationEvent);
            }
        }
    }

    private void TickSkillAudioEvent()
    {
        // 驱动音效
        for (int i = 0; i < skillClip.SkillAudioData.FrameData.Count; i++)
        {
            SkillAudioEvent audioEvent = skillClip.SkillAudioData.FrameData[i];
            audioEvent = skillBehaviour.BeforeSkillAudioEvent(audioEvent);
            if (audioEvent != null)
            {
                if (audioEvent.AudioClip != null && audioEvent.FrameIndex == currentFrameIndex)
                {
                    // 播放音效，从头播放
                    AudioSystem.PlayOneShot(audioEvent.AudioClip, transform.position, false, audioEvent.Volume);
                }
                skillBehaviour.AfterSkillAudioEvent(audioEvent);
            }
        }
    }

    private void TickSkillEffectEvent()
    {
        // 驱动特效
        for (int i = 0; i < skillClip.SkillEffectData.FrameData.Count; i++)
        {
            SkillEffectEvent effectEvent = skillClip.SkillEffectData.FrameData[i];
            effectEvent = skillBehaviour.BeforeSkillEffectEvent(effectEvent);
            if(effectEvent != null)
            {
                if (effectEvent.Prefab != null && effectEvent.FrameIndex == currentFrameIndex)
                {
                    // 实例化特效
                    GameObject effectObj = PoolSystem.GetGameObject(effectEvent.Prefab.name);
                    if (effectObj == null)
                    {
                        effectObj = GameObject.Instantiate(effectEvent.Prefab);
                        effectObj.name = effectEvent.Prefab.name;
                    }
                    effectObj.transform.position = modelTransform.TransformPoint(effectEvent.Position);
                    effectObj.transform.rotation = Quaternion.Euler(modelTransform.eulerAngles + effectEvent.Rotation);
                    effectObj.transform.localScale = effectEvent.Scale;
                    if (effectEvent.AutoDestruct)
                    {
                        StartCoroutine(AutoDestructEffectGameObject((float)effectEvent.Duration / skillClip.FrameRate, effectObj));
                    }
                }
                skillBehaviour.AfterSkillEffectEvent(effectEvent);
            }
        }
    }

    private void TickSkillAttackDetectionEvent()
    {
#if UNITY_EDITOR
        if (drawAttackDetectionGizmos) currentAttackDetectionList.Clear();
#endif

        // 驱动伤害检测
        for (int i = 0; i < skillClip.SkillAttackDetectionData.FrameData.Count; i++)
        {
            SkillAttackDetectionEvent detectionEvent = skillClip.SkillAttackDetectionData.FrameData[i];
            detectionEvent = skillBehaviour.BeforeSkillAttackDetectionEvent(detectionEvent);
            if (detectionEvent != null)
            {
                AttackDetectionType attackDetectionType = detectionEvent.GetAttackDetectionType();
                if (attackDetectionType == AttackDetectionType.Weapon)
                {
                    // 武器需要关注第一帧和结束帧
                    if (detectionEvent.FrameIndex == currentFrameIndex)
                    {
                        // 驱动武器开启
                        AttackWeaponDetectionData weaponDetectionData = (AttackWeaponDetectionData)detectionEvent.AttackDetectionData;
                        if (weaponDic.TryGetValue(weaponDetectionData.weaponName, out WeaponController weapon))
                        {
                            AttackData attackData = new AttackData
                            {
                                detectionEvent = detectionEvent,
                                source = owner,
                                attackValue = owner.GetAttackValue(detectionEvent)
                            };
                            weapon.StartDetection(attackData);
                        }
                        else Debug.LogError("没有指定的武器");
                    }
                    if (currentFrameIndex == detectionEvent.FrameIndex + detectionEvent.DurationFrame)
                    {
                        // 驱动武器关闭
                        AttackWeaponDetectionData weaponDetectionData = (AttackWeaponDetectionData)detectionEvent.AttackDetectionData;
                        if (weaponDic.TryGetValue(weaponDetectionData.weaponName, out WeaponController weapon))
                        {
                            weapon.StopDetection();
                        }
                        else Debug.LogError("没有指定的武器");
                    }
                }
                // 其他形状内每一帧都做检测
                else
                {
                    // 当前帧在范围内
                    if (currentFrameIndex >= detectionEvent.FrameIndex && currentFrameIndex <= detectionEvent.FrameIndex + detectionEvent.DurationFrame)
                    {
                        Collider[] colliders = SkillAttackDetectionTool.ShapeDetection(modelTransform, detectionEvent.AttackDetectionData, attackDetectionType, attackDetectionLayer);
                        if (colliders == null) continue;
                        for (int c = 0; c < colliders.Length; c++)
                        {
                            // TEST:Debug.Log(colliders[c].name + "//skillType = " + detectionEvent.AttackDetectionType + "//frame = " + detectionEvent.FrameIndex);
                            if (colliders[c] != null)
                            {
                                IHitTarget hitTarget = colliders[c].GetComponentInChildren<IHitTarget>();
                                if (hitTarget != null)
                                {
                                    if (hitTarget.HitTargetStatus == HitTargetStatus.Invincibility) continue;
                                    Vector3 hitpos = ((AttackShapeDetectionDataBase)detectionEvent.AttackDetectionData).Position;
                                    AttackData attackData = new AttackData
                                    {
                                        detectionEvent = detectionEvent,
                                        source = owner,
                                        attackValue = owner.GetAttackValue(detectionEvent),
                                        hitPoint = colliders[c].ClosestPoint(modelTransform.TransformPoint(hitpos)),
                                    };
                                    skillBehaviour.OnAttackDetection(hitTarget, attackData);
                                }
                            }
                        }
                    }
                }
                skillBehaviour.AfterSkillAttackDetectionEvent(detectionEvent);
#if UNITY_EDITOR
                if (drawAttackDetectionGizmos)
                {
                    if (currentFrameIndex >= detectionEvent.FrameIndex && currentFrameIndex <= detectionEvent.FrameIndex + detectionEvent.DurationFrame)
                    {
                        currentAttackDetectionList.Add(detectionEvent);
                    }
                }
#endif
            }
        }
    }
    private IEnumerator AutoDestructEffectGameObject(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.GameObjectPushPool();
    }

    #region Editor
#if UNITY_EDITOR
    [SerializeField] private bool drawAttackDetectionGizmos;
    private List<SkillAttackDetectionEvent> currentAttackDetectionList = new List<SkillAttackDetectionEvent>();
    private void OnDrawGizmos()
    {
        if (drawAttackDetectionGizmos && currentAttackDetectionList.Count != 0)
        {
            for (int i = 0; i < currentAttackDetectionList.Count; i++)
            {
                SkillGizmosTool.DrawDetection(currentAttackDetectionList[i], this);
            }
        }
    }
#endif
    #endregion
}
