using JKFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventManager : SingletonMono<BattleEventManager>
{
    public void Init()
    {

    }

    #region 子弹时间相关
    private bool bulletTimeOn;
    private float bulletTime_duration;
    private float bulletTime_timeScale;
    private Action bulletTime_overEvent;
    public bool BulletTimeOn { get { return bulletTimeOn;} }

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
        bulletTime_duration = 0;
        bulletTimeOn = false;
        bulletTime_overEvent?.Invoke();
        //Debug.Log("bullet Time over!!!!!!!!!!!!");
    }
    #endregion

    private void Update()
    {
        #region 子弹时间相关
        if (bulletTimeOn)
        {
            if(bulletTime_duration <= 0)
            {
                StopBattleBulletTime();
            }
            bulletTime_duration -= Time.deltaTime;
        }
        #endregion
    }
}
