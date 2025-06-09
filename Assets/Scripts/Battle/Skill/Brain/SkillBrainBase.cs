using JKFrame;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SkillBrainBase : MonoBehaviour
{
    [SerializeField] protected Skill_Player skill_Player;
    [SerializeField] protected List<SkillConfig> skillConfigs = new List<SkillConfig>(); // 战斗技能
    [SerializeField] protected SkillConfig dodgeConfig; // 闪避操作
    [ShowInInspector] protected List<SkillBehaviourBase> skillBehaviours;
    [SerializeField] protected int currentSkillPriority;
    public virtual int lastBehaviourIndex { get; protected set; } = -1;

    public virtual bool canRelease { get; protected set; }
    public int SkillConfigCount => skillConfigs.Count;
    public int DodgeConfigIdx => 1;
    public int StandAttackConfigIdx => 0;
    public int AllConfigCount => 1 + SkillConfigCount;

    public virtual void Init(ICharacter owner)
    {
        canRelease = true;
        skill_Player.Init(owner, owner.Animation_Controller, owner.ModelTransform);
    }

    public virtual void SetCanReleaseFlag(bool newValue)
    {
        canRelease = newValue;
    }

    public virtual bool CheckReleaseSkill(int index)
    {
        if (index >= skillBehaviours.Count) return false;
        if (skillBehaviours[index].SkillPriority > currentSkillPriority)
        {
            return skillBehaviours[index].CheckRelease();
        }
        else
        {
            return canRelease && skillBehaviours[index].CheckRelease();
        }
    }

    public virtual void ReleaseSkill(int index)
    {
        if(lastBehaviourIndex != index && lastBehaviourIndex != -1)
        {
            skillBehaviours[lastBehaviourIndex].OnReleaseNewSkill();
        }
        skillBehaviours[index].Release();
        lastBehaviourIndex = index;
        currentSkillPriority = skillBehaviours[index].SkillPriority;

    }

    public virtual void StopSkill()
    {
        skillBehaviours[lastBehaviourIndex].Stop();
    }

    protected virtual void Update()
    {
        for(int i = 0;i < skillBehaviours.Count; i++)
        {
            skillBehaviours[i].OnUpdate();
        }
    }

    public virtual bool CheckCost(SkillCostType costType, float cost)
    {
        return true;
    }

    // 技能的消耗代价做实际扣除
    public virtual void ApplyCost(SkillCostType costType, float cost)
    {
        Debug.Log($"释放技能的代价{costType}:{cost}");
    }

    #region 共享数据
    protected interface ISkillShareData { }
    protected class SkillShareData<T> : ISkillShareData
    {
        public T value;
    }

    private Dictionary<string, ISkillShareData> shareDataDic = new Dictionary<string, ISkillShareData>();

    protected SkillShareData<T> GetSkillShareData<T>()
    {
        return ResSystem.GetOrNew<SkillShareData<T>>();
    }

    protected void DestroySkillShareData(ISkillShareData obj)
    {
        obj.ObjectPushPool();
    }

    public void AddShareData<T>(string key, T value)
    {
        SkillShareData<T> skillShareData = GetSkillShareData<T>();
        skillShareData.value = value;
        shareDataDic.Add(key, skillShareData);
        if(sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            ((SharedDataEventData<T>)sharedDataEventData).TriggerCreate(value);
            ((SharedDataEventData<T>)sharedDataEventData).TriggerChange(value);
        }
    }

    public void AddorUpdateShareData<T>(string key, T value)
    {
        if(shareDataDic.TryGetValue(key, out ISkillShareData skillShareData))
        {
            ((SkillShareData<T>)skillShareData).value = value;
            if (sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
            {
                ((SharedDataEventData<T>)sharedDataEventData).TriggerChange(value);
            }
        }
        else
            AddShareData<T>(key, value);
    }

    public bool ContainsShareData(string key)
    {
        return shareDataDic.ContainsKey(key);
    }

    public bool TryGetSkillShareData<T>(string key, out T value)
    {
        bool res = shareDataDic.TryGetValue(key, out ISkillShareData data);
        if (res)
            value = ((SkillShareData<T>)data).value;
        else
            value = default;
        return res;
    }

    public void RemoveShareData<T>(string key)
    {
        if (shareDataDic.Remove(key, out ISkillShareData data))
        {
            if (sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
            {
                sharedDataEventData.TriggerRemove();
            }
            DestroySkillShareData(data);
        }
    }

    public void CleanShareData()
    {
        foreach (KeyValuePair<string, ISkillShareData> item in shareDataDic)
        {
            DestroySkillShareData(item.Value);
            if (sharedDataEventDic.TryGetValue(item.Key, out ISharedDataEventData sharedDataEventData))
            {
                sharedDataEventData.TriggerRemove();
            }
        }
        shareDataDic.Clear();
    }
    #endregion

    #region 共享数据相关事件
    private interface ISharedDataEventData 
    {
        public void TriggerRemove();
    }
    private class SharedDataEventData<T> : ISharedDataEventData
    {
        public Action<T> onCreate;
        public Action<T> onChanged;
        public Action onRemove;
        public void TriggerCreate(T value)
        {
            onCreate?.Invoke(value);
        }
        public void TriggerChange(T value)
        {
            onChanged?.Invoke(value);
        }
        public void TriggerRemove()
        {
            onRemove?.Invoke();
        }
    }
    private Dictionary<string,ISharedDataEventData> sharedDataEventDic = new Dictionary<string, ISharedDataEventData> ();

    public void AddSharedDataCreateEventListener<T>(string key, Action<T> action)
    {
        if(!sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            SharedDataEventData<T> eventData = new SharedDataEventData<T>();
            eventData.onCreate += action;
            sharedDataEventDic.Add(key, eventData);
        }
        else
        {
            SharedDataEventData<T> eventData = (SharedDataEventData<T>)sharedDataEventData;
            eventData.onCreate += action;
        }
    }

    public void RemoveSharedDataCreateEventListener<T>(string key, Action<T> action)
    {
        if(sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            SharedDataEventData<T> eventData = (SharedDataEventData<T>)sharedDataEventData;
            eventData.onCreate -= action;
        }
    }

    public void AddSharedDataChangedEventListener<T>(string key, Action<T> action)
    {
        if(!sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            SharedDataEventData<T> eventData = new SharedDataEventData<T>();
            eventData.onChanged += action;
            sharedDataEventDic.Add(key, eventData);
        }
        else
        {
            SharedDataEventData<T> eventData = (SharedDataEventData<T>)sharedDataEventData;
            eventData.onChanged += action;
        }
    }

    public void RemoveSharedDataChangeEventListener<T>(string key, Action<T> action)
    {
        if(sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            SharedDataEventData<T> eventData = (SharedDataEventData<T>)sharedDataEventData;
            eventData.onChanged -= action;
        }
    }

    public void AddSharedDataRemoveEventListener<T>(string key, Action action)
    {
        if(!sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            SharedDataEventData<T> eventData = new SharedDataEventData<T>();
            eventData.onRemove += action;
            sharedDataEventDic.Add(key, eventData);
        }
        else
        {
            SharedDataEventData<T> eventData = (SharedDataEventData<T>)sharedDataEventData;
            eventData.onRemove += action;
        }
    }

    public void RemoveSharedDataRemoveEventListener<T>(string key, Action action)
    {
        if(sharedDataEventDic.TryGetValue(key, out ISharedDataEventData sharedDataEventData))
        {
            SharedDataEventData<T> eventData = (SharedDataEventData<T>)sharedDataEventData;
            eventData.onRemove -= action;
        }
    }
    #endregion
}
