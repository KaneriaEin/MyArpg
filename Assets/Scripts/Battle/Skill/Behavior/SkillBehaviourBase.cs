using JKFrame;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBehaviourBase
{
    protected ICharacter owner;
    protected SkillConfig skillConfig;
    protected SkillBrainBase skillBrain;
    protected Skill_Player skill_Player;
    protected bool playing = false;
    protected bool canRotate = false;
    protected float cdTimer;
    protected int skillPriority;
    [SerializeField] protected float cdTime => skillConfig.cdTime;
    public abstract SkillBehaviourBase DeepCopy();
    private HashSet<IHitTarget> hitTargets;

    public int SkillPriority {  get { return skillPriority; } }

    public virtual void Init(ICharacter owner, SkillConfig skillConfig, SkillBrainBase skillBrain, Skill_Player skill_Player)
    {
        this.owner = owner;
        this.skillConfig = skillConfig;
        this.skillBrain = skillBrain;
        this.skill_Player = skill_Player;
        this.hitTargets = new HashSet<IHitTarget>();
        this.skillPriority = skillConfig.Prioriy;
    }

    public virtual void OnUpdate()
    {
        UpdateCDTime();
        RotateOnUpdate();
    }

    public virtual void Release()
    {
        canRotate = false;
        hitTargets.Clear();
        playing = true;
        skillBrain.SetCanReleaseFlag(false);
        ApplyCosts();
    }

    public virtual void Stop()
    {
        playing = false;
        hitTargets.Clear();
        skillBrain.SetCanReleaseFlag(true);
    }

    public virtual void ApplyCosts()
    {
        foreach (KeyValuePair<SkillCostType, float> item in skillConfig.releaseCostDic)
        {
            skillBrain.ApplyCost(item.Key, item.Value);
        }
    }

    public virtual void UpdateCDTime()
    {
        if (cdTimer <= 0) return;
        cdTimer = Mathf.Clamp(cdTimer - Time.deltaTime, 0, float.MaxValue);
    }

    public virtual bool CheckRelease() { return CheckReleaseCost(); }
    public virtual bool CheckReleaseCost()
    {
        foreach (KeyValuePair<SkillCostType, float> item in skillConfig.releaseCostDic)
        {
            if(!skillBrain.CheckCost(item.Key, item.Value)) return false;
        }
        return true;
    }

    protected virtual void RotateOnUpdate()
    {
        if (canRotate)
        {
            owner.OnSkillRotate();
        }
    }

    public virtual void OnReleaseNewSkill()
    {
        // 释放新技能时，需要把这个没放完的技能的一些已经打开但需要关闭的flag给关闭
        skill_Player.CleanEvents();
        OnClipEndOrReleaseNewSkill();
    }
    public virtual void OnSkillClipEnd()
    {
        OnClipEndOrReleaseNewSkill();
    }

    public virtual void OnClipEndOrReleaseNewSkill()
    {
        playing = false;
        hitTargets.Clear();
    }

    #region 技能驱动时的事件
    public virtual void OnTickSkill(int frameIndex) { }
    public virtual SkillCustomEvent BeforeSkillCustomEvent(SkillCustomEvent customEvent){ return customEvent; }
    public virtual SkillAnimationEvent BeforeSkillAnimationEvent(SkillAnimationEvent animationEvent) { return animationEvent; }
    public virtual SkillAudioEvent BeforeSkillAudioEvent(SkillAudioEvent audioEvent) { return audioEvent; }
    public virtual SkillEffectEvent BeforeSkillEffectEvent(SkillEffectEvent effectEvent) { return effectEvent; }
    public virtual SkillAttackDetectionEvent BeforeSkillAttackDetectionEvent(SkillAttackDetectionEvent attackDetectionEvent) { return attackDetectionEvent; }
    public virtual void AfterSkillCustomEvent(SkillCustomEvent customEvent)
    {
        if(customEvent.EventType == SkillEventType.CanSkillRelease)
        {
            skillBrain.SetCanReleaseFlag(true);
        }
        else if (customEvent.EventType == SkillEventType.CanRotate)
        {
            if(owner.Target == null)
            {
                canRotate = true;
            }
            else
            {
                owner.ModelTransform.LookAt(owner.Target.ModelTransform);
            }
        }
        else if (customEvent.EventType == SkillEventType.CanNotRotate)
        {
            canRotate = false;
        }
        else if (customEvent.EventType == SkillEventType.AddBuff)
        {
            owner.AddBuff((BuffConfig)customEvent.ObjectArg, customEvent.IntArg);
        }
        else if (customEvent.EventType == SkillEventType.InvincibleOn)
        {
            owner.HitTargetStatus = HitTargetStatus.Invincibility;
        }
        else if (customEvent.EventType == SkillEventType.InvincibleOff)
        {
            owner.HitTargetStatus = HitTargetStatus.None;
        }
    }
    public virtual void AfterSkillAnimationEvent(SkillAnimationEvent animationEvent) { }
    public virtual void AfterSkillAudioEvent(SkillAudioEvent audioEvent) { }
    public virtual void AfterSkillEffectEvent(SkillEffectEvent effectEvent) { }
    public virtual void AfterSkillAttackDetectionEvent(SkillAttackDetectionEvent attackDetectionEvent) { }
    public virtual void OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        // 避免重复传递伤害行为和数据
        if (!hitTargets.Contains(target))
        {
            hitTargets.Add(target);
            OnHitTarget(target, attackData);
        }
    }

    public virtual void OnHitTarget(IHitTarget hitTarget, AttackData attackData)
    {
        if(attackData.detectionEvent.AttackHitConfig != null)
        {
            DoHitEffect(attackData);
        }
        hitTarget.BeHit(attackData);
    }

    protected void DoHitEffect(AttackData attackData)
    {
        AttackHitConfig attackHitConfig = attackData.detectionEvent.AttackHitConfig;
        if(attackHitConfig != null)
        {
            // 音效
            if (attackHitConfig.HitAudioClip != null) AudioSystem.PlayOneShot(attackHitConfig.HitAudioClip, attackData.hitPoint);

            // 特效
            if(attackHitConfig.HitEffectPrefab != null)
            {
                GameObject effect = ProjectUtility.GetOrInstantiateGameObject(attackHitConfig.HitEffectPrefab, null);
                effect.transform.position = attackData.hitPoint;
                effect.transform.LookAt(Camera.main.transform.position);
                effect.GetComponent<EffectController>().Init();
            }
        }
    }

    public virtual void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation) { }
    #endregion
}
