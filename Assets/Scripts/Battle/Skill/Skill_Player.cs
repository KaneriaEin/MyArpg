using Cinemachine;
using JKFrame;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.ParticleSystem;

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
    private float localTimeScale;
    [ShowInInspector] private float percent = 0;
    public float LocalTimeScale 
    { get { return localTimeScale; } 
      set 
        {
            // Debug.Log($"{gameObject.name}的Skill_Player的localtimeScale设为{value}");
            #region 调整特效速度
            for (int i = 0; i < effectObjs.Count; i++)
            {
                EffectController effectController = effectObjs[i].GetComponent<EffectController>();
                effectController.ResetSimulationSpeed(value);
            }
            #endregion

            localTimeScale = value; 
        } 
    }
    public SkillClip SkillClip { get { return skillClip; } }
    public int CurrentFrameIndex { get { return currentFrameIndex; } }

    private List<GameObject> effectObjs;
    private Coroutine skillFreezeCoroutine;
    private bool isFreezing;
    private float currentAnimPlaySpeed = 1;
    private float currentSkillSpeed = 1;

    public void Init(ICharacter owner, Animation_Controller animation_Controller, Transform modelTransform)
    {
        this.owner = owner;
        this.animation_Controller = animation_Controller;
        this.modelTransform = modelTransform;
        this.localTimeScale = 1f;
        foreach (WeaponController item in WeaponDic.Values)
        {
            item.Init(attackDetectionLayer, OnWeaponDetection);
        }

        effectObjs = new List<GameObject>();
        skillFreezeCoroutine = null;
        isFreezing = false;
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

    private bool OnWeaponDetection(IHitTarget target, AttackData attackData)
    {
        return skillBehaviour.OnAttackDetection(target, attackData);
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
        TickSkillCameraEvent(0,true);
    }

    /// <summary>
    /// 中止技能片段
    /// </summary>
    /// <param name="skillClip">技能配置</param>
    public void StopSkillClip()
    {
        isPlaying = false;
        SkillHitFreezeStop();
        StopSkillEffects();
        Clean();
    }

    private void Clean()
    {
        CleanEvents();
        // CleanSkillEffects();
    }

    private void Update()
    {
        //Debug.Log("playerTotalTime:" + playTotalTime);
        if (isPlaying)
        {
            percent = ((float)currentFrameIndex) / ((float)skillClip.FrameCount);
            if (localTimeScale == 0) return;
            if (isFreezing) return;

            currentSkillSpeed = GetSkillSpeed(currentFrameIndex);
            playTotalTime += Time.deltaTime * currentSkillSpeed * localTimeScale;
            
            // 根据总时间判断当前是第几帧
            int targetFrameIndex = (int)(playTotalTime * frameRate);

            // 防止一帧延迟过大，追帧
            while (currentFrameIndex < targetFrameIndex)
            {
                // 驱动一次技能
                TickSkill();
            }
            // Camera运镜根据实际时间tick
            TickSkillCameraEvent(playTotalTime);
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
        TickSpeed();
        //TickSkillCameraEvent();
    }

    public void CleanEvents()
    {
        if (skillClip == null) return;
        // 迅速过一边事件，武器判定，把一些打开的碰撞和flag清掉
        #region CustomEvent事件
        // 只需要考虑让角色恢复正常状态的事件flag
        int fastFrame = currentFrameIndex;
        while(fastFrame <= skillClip.FrameCount)
        {
            if (skillClip.SkillCustomEventData.FrameData.TryGetValue(fastFrame, out SkillCustomEvent skillCustomEvent))
            {
                if (skillCustomEvent != null)
                {
                    if (skillCustomEvent.EventType == SkillEventType.CanSkillRelease
                        || skillCustomEvent.EventType == SkillEventType.CanRotate
                        || skillCustomEvent.EventType == SkillEventType.InvincibleOff)
                    {
                        skillBehaviour.AfterSkillCustomEvent(skillCustomEvent);
                    }
                }
            }
            fastFrame++;
        }
        #endregion
        #region AttackDetectionEvent武器判定
        // 判断当前帧时，是否有打开的武器判定，直接关闭即可
        for (int i = 0; i < skillClip.SkillAttackDetectionData.FrameData.Count; i++)
        {
            SkillAttackDetectionEvent detectionEvent = skillClip.SkillAttackDetectionData.FrameData[i];
            detectionEvent = skillBehaviour.BeforeSkillAttackDetectionEvent(detectionEvent);
            if (detectionEvent != null)
            {
                AttackDetectionType attackDetectionType = detectionEvent.GetAttackDetectionType();
                if (attackDetectionType == AttackDetectionType.Weapon)
                {
                    if (currentFrameIndex <= detectionEvent.FrameIndex + detectionEvent.DurationFrame && currentFrameIndex >= detectionEvent.FrameIndex)
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
            }
        }
        #endregion
        #region 相机归位
        if (skillClip.SkillCameraData.DollyTrackPrefab != null)
            CameraManager.Instance.DollyStop();
        #endregion
    }

    private void TickSpeed()
    {
        if (skillClip.SpeedCurve.keys.Length == 0) return;
        currentSkillSpeed = GetSkillSpeed(currentFrameIndex);
        // 动画速度
        if (currentFrameIndex == 0) // 技能刚开始播放，直接乘speed就好
        {
            animation_Controller.Speed *= currentSkillSpeed;
        }
        else
        {
            animation_Controller.Speed *= currentSkillSpeed / GetSkillSpeed(currentFrameIndex - 1);
        }

        // 特效速度
        ParticleSystem ps;
        for (int i = 0; i < effectObjs.Count; i++)
        {
            ps = effectObjs[i].GetComponent<ParticleSystem>();
            // 需要同时设置父对象和所有子特效
            ParticleSystem[] allParticles = effectObjs[i].GetComponentsInChildren<ParticleSystem>();
            if (ps.time == 0) // 特效刚开始播放，直接乘speed就好
            {
                for (int j = 0; j < allParticles.Length; j++)
                {
                    var main = allParticles[j].main;
                    main.simulationSpeed *= localTimeScale * currentSkillSpeed;
                }
                // Debug.Log($"特效{effectObjs[i]}刚开始播放");
            }
            else // 特效至少已经播放一帧，那么需要除oldSpeed
            {
                float oldSpeed = GetSkillSpeed(currentFrameIndex - 1);
                for (int j = 0; j < allParticles.Length; j++)
                {
                    var main = allParticles[j].main;
                    main.simulationSpeed *= currentSkillSpeed / oldSpeed;
                }
            }
        }

        // 玩家特殊慢动作或特写需要控制其他单位的localTimeScale
        if (gameObject.tag == "Player") { TimeManager.Instance.SetTimeScaleForAllEnemies(currentSkillSpeed); }
    }

    private float GetSkillSpeed(int index)
    {
        if (skillClip.SpeedCurve.keys.Length == 0) return 1f;
        if (index < skillClip.SpeedCurve.keys[0].time || index > skillClip.SpeedCurve.keys[skillClip.SpeedCurve.keys.Length - 1].time) return 1f;
        return skillClip.SpeedCurve.Evaluate(index);
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

                animation_Controller.PlaySingleAnimation(skillAnimationEvent.AnimationClip, skillAnimationEvent.PlaySpeed * localTimeScale, true, 0f);
                currentAnimPlaySpeed = skillAnimationEvent.PlaySpeed;

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
                    effectObj.GetComponent<EffectController>().Init(0,false);
                    effectObjs.Add(effectObj);
                    if (effectEvent.AutoDestruct)
                    {
                        StartCoroutine(AutoDestructEffectGameObject((float)effectEvent.Duration / skillClip.FrameRate + 5, effectObj));
                        //暂时不用协程销毁特效，手动cleanEffect。销毁特效方式采用手动于技能结束时的clean内调用cleanEffect
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
                        #region 径向模糊和震动
                        // 若此攻击判定需要开启径向模糊，则此刻处理
                        if (detectionEvent.RadialBlurConfig != null && detectionEvent.RadialBlurConfig.Enable)
                        {
                            PostProcessingManager.Instance.TriggerRadialBlur(detectionEvent.RadialBlurConfig.RiseTime, detectionEvent.RadialBlurConfig.HoldTime, detectionEvent.RadialBlurConfig.FallTime);
                        }
                        if (detectionEvent.AttackHitConfig.ShakeConfig != null)
                        {
                            CameraShakeManager.Instance.TriggerShake(detectionEvent.AttackHitConfig.ShakeConfig);
                        }
                        #endregion
                        // 驱动武器开启
                        AttackWeaponDetectionData weaponDetectionData = (AttackWeaponDetectionData)detectionEvent.AttackDetectionData;
                        if (weaponDic.TryGetValue(weaponDetectionData.weaponName, out WeaponController weapon))
                        {
                            //MonoSystem.Start_Coroutine(PostProcessingManager.Instance.PulsedRadialBlur());
                            AttackData attackData = new AttackData
                            {
                                detectionEvent = detectionEvent,
                                source = owner,
                                attackValue = owner.GetAttackValue(detectionEvent),
                                stunAttackValue = detectionEvent.AttackHitConfig.StunAttackMultiply,
                                pgPunish = skillClip.PGuardPunish,
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
                    if (currentFrameIndex >= detectionEvent.FrameIndex && currentFrameIndex < detectionEvent.FrameIndex + detectionEvent.DurationFrame)
                    {
                        #region 径向模糊和震动
                        // 若此攻击判定需要开启径向模糊，则此刻处理。只需此攻击判定的第一帧处理即可
                        if (currentFrameIndex == detectionEvent.FrameIndex)
                        {
                            if (detectionEvent.RadialBlurConfig != null && detectionEvent.RadialBlurConfig.Enable)
                            {
                                PostProcessingManager.Instance.TriggerRadialBlur(detectionEvent.RadialBlurConfig.RiseTime, detectionEvent.RadialBlurConfig.HoldTime, detectionEvent.RadialBlurConfig.FallTime);
                            }
                        }
                        if (detectionEvent.AttackHitConfig.ShakeConfig != null)
                        {
                            CameraShakeManager.Instance.TriggerShake(detectionEvent.AttackHitConfig.ShakeConfig);
                        }
                        #endregion
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
                                        stunAttackValue = detectionEvent.AttackHitConfig.StunAttackMultiply,
                                        hitPoint = colliders[c].ClosestPoint(modelTransform.TransformPoint(hitpos)),
                                        pgPunish = skillClip.PGuardPunish,
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

    /// <summary>
    /// Tick摄像机有点特殊，需要根据实际技能经过的时间Time.deltaTime来tick
    /// </summary>
    private void TickSkillCameraEvent(float totalTime, bool start = false)
    {
        if(skillClip.SkillCameraData.DollyTrackPrefab == null) return;

        if(start)
        {
            // 技能刚开始时要实例化轨道预制体
            GameObject trackObj = PoolSystem.GetGameObject(skillClip.SkillCameraData.DollyTrackPrefab.name);
            if (trackObj == null)
            {
                trackObj = GameObject.Instantiate(skillClip.SkillCameraData.DollyTrackPrefab);
                trackObj.name = skillClip.SkillCameraData.DollyTrackPrefab.name;
            }
            trackObj.transform.SetParent(modelTransform);
            trackObj.transform.localPosition = Vector3.zero;
            trackObj.transform.localRotation = Quaternion.identity;
            CameraManager.Instance.DollySetPath(trackObj.GetComponent<CinemachineSmoothPath>());
            CameraManager.Instance.DollyStart(modelTransform);
            StartCoroutine(AutoDestructGameObject(10f, trackObj)); // 等待10s回收轨道
        }

        CameraManager.Instance.DollyMoveUpdate(skillClip.SkillCameraData, totalTime * skillClip.FrameRate);
        if(currentFrameIndex >= skillClip.FrameCount)
        {
            CameraManager.Instance.DollyStop();
        }
    }
 
    private IEnumerator AutoDestructEffectGameObject(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        if (effectObjs.Contains(obj))
        {
            effectObjs.Remove(obj);
            obj.GameObjectPushPool();
        }
    }

    private IEnumerator AutoDestructGameObject(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.GameObjectPushPool();
    }

    private void StopSkillEffects()
    {
        for (int i = 0; i < effectObjs.Count; i++)
        {
            if (effectObjs[i].GetComponent<ParticleSystem>())
            {
                effectObjs[i].GetComponent<ParticleSystem>().Stop();
                // effectObjs[i].GetComponent<ParticleSystem>().Clear();
            }
        }
    }

    private void CleanSkillEffects()
    {
        GameObject obj = null;
        for (int i = effectObjs.Count - 1; i >= 0; i--)
        {
            obj = effectObjs[i];
            effectObjs.Remove(obj);
            obj.GameObjectPushPool();
        }
    }

    #region 顿帧效果
    /// <summary>
    /// 触发顿帧效果
    /// </summary>
    /// <param name="time"></param>
    public void SkillHitFreeze(float time)
    {
        // 1if (!IsPlaying) return;
        skillFreezeCoroutine = StartCoroutine(SkillHitFreezeWait(time));
    }
    /// <summary>
    /// 停止顿帧效果。如主角被打，则中断角色的进攻顿帧状态
    /// </summary>
    private void SkillHitFreezeStop()
    {
        if (skillFreezeCoroutine != null)
        {
            StopCoroutine(skillFreezeCoroutine);
        }
        // 恢复动画速度
        animation_Controller.SetAnimationSpeed(localTimeScale * currentAnimPlaySpeed * currentSkillSpeed);
        // 恢复特效速度
        ParticleSystem particleSystem = null;
        for (int i = 0; i < effectObjs.Count; i++)
        {
            particleSystem = effectObjs[i].GetComponent<ParticleSystem>();
            if (particleSystem != null && particleSystem.IsAlive(true))
            {
                particleSystem.Play();
            }
        }
        isFreezing = false;
    }
    /// <summary>
    /// 攻击方触发顿帧的协程
    /// 暂停动画、特效后等待time后恢复原状
    /// </summary>
    private IEnumerator SkillHitFreezeWait(float time)
    {
        // Test Debug.Log($"顿帧！{time}秒");
        // 1isPlaying = false;
        isFreezing = true;
        #region 动画
        float oldspeed = animation_Controller.Speed;
        animation_Controller.SetAnimationSpeed(0);
        #endregion
        #region 特效
        ParticleSystem particleSystem = null;
        for (int i = 0; i < effectObjs.Count; i++)
        {
            particleSystem = effectObjs[i].GetComponent<ParticleSystem>();
            if (particleSystem != null && particleSystem.IsAlive(true))
                particleSystem.Pause();
        }
        #endregion

        yield return new WaitForSeconds(time);

        #region 动画
        animation_Controller.SetAnimationSpeed(localTimeScale * currentAnimPlaySpeed * currentSkillSpeed);
        #endregion
        #region 特效
        for (int i = 0; i < effectObjs.Count; i++)
        {
            particleSystem = effectObjs[i].GetComponent<ParticleSystem>();
            if (particleSystem != null && particleSystem.IsAlive(true))
            {
                particleSystem.Play();
            }
        }
        particleSystem = null;
        #endregion

        isFreezing = false;
        // 1isPlaying = true;
    }

    /// <summary>
    /// 提供给外部的接口
    /// 启用顿帧效果，动画、特效暂停播放
    /// </summary>
    public void SkillHitFreezeStart()
    {
        isFreezing = true;
        #region 动画
        float oldspeed = animation_Controller.Speed;
        animation_Controller.SetAnimationSpeed(0);
        #endregion
        #region 特效
        ParticleSystem particleSystem = null;
        for (int i = 0; i < effectObjs.Count; i++)
        {
            particleSystem = effectObjs[i].GetComponent<ParticleSystem>();
            if (particleSystem != null && particleSystem.IsAlive(true))
                particleSystem.Pause();
        }
        #endregion
    }

    /// <summary>
    /// 提供给外部的接口
    /// 结束顿帧效果，动画、特效继续播放
    /// </summary>
    public void SkillHitFreezeFinish()
    {
        #region 动画
        animation_Controller.SetAnimationSpeed(localTimeScale * currentAnimPlaySpeed * currentSkillSpeed);
        #endregion
        #region 特效
        ParticleSystem particleSystem = null;
        for (int i = 0; i < effectObjs.Count; i++)
        {
            particleSystem = effectObjs[i].GetComponent<ParticleSystem>();
            if (particleSystem != null && particleSystem.IsAlive(true))
                particleSystem.Play();
        }
        #endregion

        isFreezing = false;
    }
    #endregion

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