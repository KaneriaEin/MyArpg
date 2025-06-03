using System;
using UnityEngine;

[Serializable]
public class Key
{
    public KeyCode keyCode;
    public bool isCache;
    public float cacheTime;

    private float lastInputTime;
    public bool valid;
    public bool GetKeyDownState()
    {
        if (!isCache) return Input.GetKeyDown(keyCode);
        return Input.GetKeyDown(keyCode) || (Time.time - lastInputTime) < cacheTime;
    }

    public bool GetKeyState()
    {
        return Input.GetKey(keyCode);
    }

    public void Update()
    {
        if (!isCache) return;
        if (Input.GetKeyDown(keyCode))
        {
            lastInputTime = Time.time;
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
}
