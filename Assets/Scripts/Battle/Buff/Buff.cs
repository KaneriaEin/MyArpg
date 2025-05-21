using JKFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class Buff
{
    public BuffConfig config { get; private set; }
    public float destroyTimer { get; private set; }
    public float periodicTimer {  get; private set; }
    public int layer {  get; private set; }
    private Action<Buff> onStart;
    private Action<Buff> onPeriodic;
    private Action<Buff> onEnd;

    public void Init(BuffConfig config, Action<Buff> onStart, Action<Buff> onPeriodic, Action<Buff> onEnd)
    {
        this.config = config;
        this.onStart = onStart;
        this.onPeriodic = onPeriodic;
        this.onEnd = onEnd;
    }

    public void Start()
    {
        destroyTimer = config.duration;
        periodicTimer = config.periodicTime;
        layer = 1;
        onStart?.Invoke(this);
        Debug.Log("Start��Чһ��");
    }

    public void OnUpdate()
    {
        // ��������Ч
        if(onPeriodic != null)
        {
            periodicTimer -= Time.deltaTime;
            if(periodicTimer <= 0)
            {
                // ��Чһ��
                onPeriodic?.Invoke(this);
                Debug.Log("OnUpdate��Чһ��");
                periodicTimer += config.periodicTime;
            }
        }
        // ���ٵ���ʱ
        destroyTimer -= Time.deltaTime;
        if (destroyTimer <= 0)
        {
            // Buff����
            onEnd?.Invoke(this);
            Debug.Log("OnEnd��Чһ��");
        }
    }

    public void Stop()
    {
        config = null;
        onStart = null;
        onPeriodic = null;
        onEnd = null;
        this.ObjectPushPool();
    }

    public void AddLayer(int layer)
    {
        if (config.canStack)
        {
            this.layer = Mathf.Clamp(this.layer + layer, 0, config.maxLayer);
        }
        // ˢ�´���ʱ��
        destroyTimer = config.duration;
    }
}
