using JKFrame;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventManager : SingletonMono<BattleEventManager>
{
    public void Init()
    {
        #region 场景内攻击信息相关
        attackInfo = new List<AttackInfo>();
        #endregion
    }

    private void Update()
    {
        #region 子弹时间相关
        if (bulletTimeOn)
        {
            if (bulletTime_duration <= 0)
            {
                StopBattleBulletTime();
            }
            bulletTime_duration -= Time.deltaTime;
        }
        #endregion

        #region 场景内攻击信息相关
        for (int i = attackInfo.Count - 1; i >= 0; i--)
        {
            var temp = attackInfo[i];
            temp.attackCurTime += Time.deltaTime * bulletTime_timeScale;
            attackInfo[i] = temp;
            for (int j = attackInfo[i].attackDetects.Count - 1; j >= 0; j--)
            {
                if (attackInfo[i].attackExpTime[j] < attackInfo[i].attackCurTime)
                {
                    attackInfo[i].attackDetects.RemoveAt(j);
                    attackInfo[i].attackExpTime.RemoveAt(j);
                }
            }
            if (attackInfo[i].attackDetects.Count == 0)
            {
                attackInfo.RemoveAt(i);
            }
        }
        #endregion
    }

    #region 子弹时间相关
    private bool bulletTimeOn;
    private float bulletTime_duration;
    private float bulletTime_timeScale;
    private Action bulletTime_overEvent;
    public bool BulletTimeOn { get { return bulletTimeOn;} }
    public float BulletTime_TimeScale {  get { return bulletTime_timeScale;} }

    /// <summary>
    /// 触发子弹时间接口
    /// </summary>
    /// <param name="duration">持续时间</param>
    /// <param name="timeScale">时间流速</param>
    /// <param name="action">退出此状态时调用的Action</param>
    public void BattleBulletTimeEvent(float duration, float timeScale = 0.1f, Action action = null)
    {
        if (bulletTimeOn) return;
        bulletTime_duration = duration;
        bulletTime_timeScale = timeScale;
        bulletTime_overEvent = action;
        bulletTimeOn = true;
        StartBattleBulletTime();

    }

    private void StartBattleBulletTime()
    {
        // Camera
        //CameraManager.Instance.DefenceStart();

        // TimeScale
        TimeManager.Instance.SetTimeScaleForAll(bulletTime_timeScale);
    }

    public void StopBattleBulletTime()
    {
        // Camera
        //CameraManager.Instance.DefenceStop();

        // TimeScale
        TimeManager.Instance.ResetAllTimeScales();
        bulletTime_timeScale = 1;
        bulletTime_duration = 0;
        bulletTimeOn = false;
        bulletTime_overEvent?.Invoke();
        bulletTime_overEvent = null;
        //Debug.Log("bullet Time over!!!!!!!!!!!!");
    }
    #endregion

    #region 场景内攻击信息相关
    public struct AttackInfo
    {
        public float attackStartTime;
        public float attackCurTime;
        public List<float> attackExpTime;
        public List<SkillAttackDetectionEvent> attackDetects;
        public GameCharacter_Controller attacker;
        // 保留接口，用于区分 近战/龙车/远程道具/定时发生 的攻击类型
        // public AttackType type;
    }

    [ShowInInspector] private List<AttackInfo> attackInfo;
    [SerializeField] private LayerMask playerMask;

    public void AddAttackInfo(SkillClip clip, GameCharacter_Controller character)
    {
        if(clip.SkillAttackDetectionData.FrameData.Count == 0) return;
        AttackInfo info = new AttackInfo { attackStartTime = Time.time, attackCurTime = Time.time, attacker = character , attackExpTime = new List<float>()};
        info.attackDetects = new List<SkillAttackDetectionEvent>();
        for(int i = 0; i < clip.SkillAttackDetectionData.FrameData.Count;i++)
        {
            SkillAttackDetectionEvent atkevent = new SkillAttackDetectionEvent();
            atkevent.TrackName = clip.SkillAttackDetectionData.FrameData[i].TrackName;
            atkevent.FrameIndex = clip.SkillAttackDetectionData.FrameData[i].FrameIndex;
            atkevent.DurationFrame = clip.SkillAttackDetectionData.FrameData[i].DurationFrame;
            atkevent.AttackDetectionData = clip.SkillAttackDetectionData.FrameData[i].AttackDetectionData;
            info.attackDetects.Add(atkevent);
            float exp = Time.time + (atkevent.FrameIndex + atkevent.DurationFrame) * 1f / 60f;
            info.attackExpTime.Add(exp);
        }
        attackInfo.Add(info);
    }

    public void RemoveAttackInfo(float startTime, GameCharacter_Controller character)
    {
        for(int i = attackInfo.Count - 1; i >= 0; i--)
        {
            if (attackInfo[i].attackStartTime == startTime && attackInfo[i].attacker == character)
            {
                attackInfo.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 判断是否触发完美闪避
    /// </summary>
    /// <param name="perfectWindow">完美闪避时间窗口</param>
    /// <returns></returns>
    public bool CheckPerfectDodge(float perfectWindow)
    {
        for(int i = 0; i < attackInfo.Count; i++)
        {
            for (int j = 0; j < attackInfo[i].attackDetects.Count; j++)
            {
                // 时间窗口内会发生攻击
                if (attackInfo[i].attackStartTime + (attackInfo[i].attackDetects[j].FrameIndex * 1f/60f) - Time.time <= perfectWindow)
                {
                    // 人物在会被攻击到的位置
                    if(CheckAttackInfoDetection(attackInfo[i].attackDetects[j], attackInfo[i].attacker))
                    {
                        // Debug.Log($"主角闪避了{attackInfo[i].attacker.name}的{attackInfo[i].attackDetects[j].TrackName}");
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool CheckAttackInfoDetection(SkillAttackDetectionEvent atkevent, GameCharacter_Controller attacker)
    {
        Collider[] colliders = SkillAttackDetectionTool.ShapeDetection(attacker.ModelTransform, atkevent.AttackDetectionData, atkevent.GetAttackDetectionType(), playerMask);
        if (colliders == null) { return false; }
        else { return true; }
    }
    #endregion
}