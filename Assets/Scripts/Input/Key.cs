using System;
using UnityEngine;
using static Cinemachine.CinemachineOrbitalTransposer;

[Serializable]
public class Key
{
    public KeyCode keyCode;
    public bool isCache;
    public float cacheTime;

    private float lastInputTime = -99;
    private bool holding = false;
    private float currentHoldtime = 0f;
    public bool valid;

    // 长短按之分用这个接口(攻击键)
    public bool GetKeyDownState()
    {
        //if (!isCache) return Input.GetKeyDown(keyCode);
        //return Input.GetKeyDown(keyCode) || (Time.time - lastInputTime) < cacheTime;
        return (Time.time - lastInputTime) < cacheTime;
    }

    // 用于读普通的按下按键，无长短按之分(闪避，防御)
    public bool GetKeyState()
    {
        return Input.GetKey(keyCode);
    }

    public void Update()
    {
        if (!isCache) return;
        if (Input.GetKeyDown(keyCode))
        {
            holding = true;
        }
        if (holding && Input.GetKey(keyCode))
        {
            currentHoldtime += Time.deltaTime;
            if (currentHoldtime >= 0.15f)
            {
                lastInputTime = Time.time;
                currentHoldtime = 0f;
                holding = false;
            }
        }
        if (holding && Input.GetKeyUp(keyCode))
        {
            holding = false;
            if (currentHoldtime < 0.15f)
            {
                lastInputTime = Time.time;
                currentHoldtime = 0f;
            }
        }
        valid = GetKeyDownState();
    }

    public void InputKeyDown()
    {
        lastInputTime = Time.time;
        valid = true;
    }

    public void InputKey()
    {
        valid = true;
    }

    public void CleanInputCache()
    {
        lastInputTime = 0;
        valid = false;
    }
}
