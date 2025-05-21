using JKFrame;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    [ShowInInspector]
    private Dictionary<BuffConfig, Buff> buffDic = new Dictionary<BuffConfig, Buff>();
    [SerializeField] private BuffEffectResolverBase buffEffectResolver;
    private List<Buff> destroyBuffs = new List<Buff>();
    private void Update()
    {
        foreach(Buff item in buffDic.Values)
        {
            item.OnUpdate();
            if(item.destroyTimer <= 0)
            {
                destroyBuffs.Add(item);
            }
        }
        foreach (var item in destroyBuffs)
        {
            buffDic.Remove(item.config);
            item.Stop();
        }
        destroyBuffs.Clear();
    }

    [Button]
    public Buff AddBuff(BuffConfig buffConfig, int layer = 1)
    {
        if(buffDic.TryGetValue(buffConfig, out Buff buff))
        {
            buff.AddLayer(layer);
        }
        else
        {
            buff = ResSystem.GetOrNew<Buff>();
            buff.Init(buffConfig, OnBuffStart, OnBuffPeriodic, OnBuffEnd);
            buff.Start();
            buffDic.Add(buffConfig, buff);
        }
        return buff;
    }

    public void CleanBuffs()
    {
        foreach (Buff item in buffDic.Values)
        {
            item.Stop();
        }
        buffDic.Clear();
    }

    // BuffÊÂ¼þ½âÎöÆ÷
    private void OnBuffStart(Buff buff)
    {
        buffEffectResolver.Resolve(buff, buff.config.startEffect);
    }

    private void OnBuffPeriodic(Buff buff)
    {
        buffEffectResolver.Resolve(buff, buff.config.periodicEffect);
    }

    private void OnBuffEnd(Buff buff)
    {
        buffEffectResolver.Resolve(buff, buff.config.endEffect);
    }
}
