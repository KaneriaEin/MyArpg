using System;
using UnityEngine;

[Serializable]
public class MouseKeyHold
{
    public int mouseButtonID;
    public float cacheTime;

    public float holdTime = 0.15f;
    public float currentHoldTime = 0f;
    public bool holding = false;
    private float lastInputTime = -99;
    public bool valid;
    public bool GetState()
    {
        return (Time.time - lastInputTime) < cacheTime;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(mouseButtonID))
        {
            holding = true;
        }
        if (holding && Input.GetMouseButton(mouseButtonID))
        {
            currentHoldTime += Time.deltaTime;
        }
        if(currentHoldTime >= holdTime)
        {
            lastInputTime = Time.time;
            // Debug.Log("³¤°´");
            holding = false;
            currentHoldTime = 0f;
        }
        if (holding && Input.GetMouseButtonUp(mouseButtonID))
        {
            holding = false;
            currentHoldTime = 0f;
        }
        valid = GetState();
    }

    public void CleanInputCache()
    {
        lastInputTime = 0;
        valid = false;
    }
}
