using Cinemachine;
using JKFrame;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : SingletonMono<TimeManager>
{
    // 存储所有注册的对象，根据类别分类
    private Dictionary<TimeCategory, List<ITimeScalable>> categorizedObjects = new Dictionary<TimeCategory, List<ITimeScalable>>();

    // 每个类别的时间缩放配置
    private Dictionary<TimeCategory, float> timeScales = new Dictionary<TimeCategory, float>();

    public void Init()
    {
        // 初始化所有类别的时间缩放为1
        foreach (TimeCategory category in System.Enum.GetValues(typeof(TimeCategory)))
        {
            timeScales[category] = 1f;
            categorizedObjects[category] = new List<ITimeScalable>();
        }
    }

    /// <summary>
    /// 注册对象
    /// </summary>
    public void RegisterObject(ITimeScalable obj)
    {
        var category = obj.TimeCategory;
        if (!categorizedObjects[category].Contains(obj))
        {
            categorizedObjects[category].Add(obj);
            // 立即应用当前的时间缩放
            obj.SetTimeScale(timeScales[category]);
        }
    }

    /// <summary>
    /// 取消注册
    /// </summary>
    public void UnregisterObject(ITimeScalable obj)
    {
        var category = obj.TimeCategory;
        categorizedObjects[category].Remove(obj);
    }
    /// <summary>
    /// 为特定类别设置时间缩放
    /// </summary>
    public void SetTimeScaleForCategory(TimeCategory category, float timeScale)
    {
        timeScales[category] = timeScale;

        // 立即应用给所有该类别的对象
        foreach (ITimeScalable obj in categorizedObjects[category])
        {
            obj.SetTimeScale(timeScale);
        }
    }

    /// <summary>
    /// 为所有敌人设置时间缩放
    /// </summary>
    public void SetTimeScaleForAllEnemies(float timeScale)
    {
        SetTimeScaleForCategory(TimeCategory.SmallEnemy, timeScale);
        SetTimeScaleForCategory(TimeCategory.EliteEnemy, timeScale);
        SetTimeScaleForCategory(TimeCategory.BossEnemy, timeScale);
    }

    /// <summary>
    /// 为所有单位设置时间缩放
    /// </summary>
    public void SetTimeScaleForAll(float timeScale)
    {
        SetTimeScaleForCategory(TimeCategory.SmallEnemy, timeScale);
        SetTimeScaleForCategory(TimeCategory.EliteEnemy, timeScale);
        SetTimeScaleForCategory(TimeCategory.BossEnemy, timeScale);
        SetTimeScaleForCategory(TimeCategory.Player, timeScale);
    }

    /// <summary>
    /// 所有单位恢复正常时间
    /// </summary>
    public void ResetAllTimeScales()
    {
        foreach (TimeCategory category in System.Enum.GetValues(typeof(TimeCategory)))
        {
            SetTimeScaleForCategory(category, 1f);
        }
    }

    /// <summary>
    /// 获取某个类别的时间缩放
    /// </summary>
    public float GetTimeScaleForCategory(TimeCategory category)
    {
        return timeScales[category];
    }
}
