using System;
using UnityEngine;

[Serializable]
public class KeyHold
{
    public KeyCode keyCode;
    public float cacheTime;

    public float holdTime = 0.15f;
    public float currentHoldTime = 0f;
    public bool holding = false;
    private float lastInputTime = -99;
    public bool valid;
    public bool GetKeyDownState()
    {
        return (Time.time - lastInputTime) < cacheTime;
    }

    public void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            holding = true;
        }
        if (holding && Input.GetKey(keyCode))
        {
            currentHoldTime += Time.deltaTime;
        }
        if (currentHoldTime >= holdTime)
        {
            lastInputTime = Time.time;
            // Debug.Log("³¤°´");
            holding = false;
            currentHoldTime = 0f;
        }
        if (holding && Input.GetKeyUp(keyCode))
        {
            holding = false;
            currentHoldTime = 0f;
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
