using System;
using UnityEngine;

[Serializable]
public class MouseKey
{
    public int mouseButtonID;
    public bool isCache;
    public float cacheTime;

    private float lastInputTime;
    public bool valid;
    public bool GetState()
    {
        if (!isCache) return Input.GetMouseButtonDown(mouseButtonID);
        return Input.GetMouseButtonDown(mouseButtonID) || (Time.time - lastInputTime) < cacheTime;
    }

    public void Update()
    {
        if (!isCache) return;
        if (Input.GetMouseButtonDown(mouseButtonID))
        {
            lastInputTime = Time.time;
        }
        valid = GetState();
    }
}
