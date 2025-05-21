using JKFrame;
using System;
using UnityEngine;

public class InputManager : SingletonMono<InputManager>
{
    [Serializable]
    public class Key
    {
        public KeyCode keyCode;
        public bool isCache;
        public float cacheTime;

        private float lastInputTime;
        public bool valid;
        public bool GetState()
        {
            if(!isCache) return Input.GetKeyDown(keyCode);
            return Input.GetKeyDown(keyCode) || (Time.time - lastInputTime) < cacheTime;
        }

        public void Update()
        {
            if (!isCache) return;
            if(Input.GetKeyDown(keyCode))
            {
                lastInputTime = Time.time;
            }
            valid = GetState();
        }
    }
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
            if(!isCache) return Input.GetMouseButtonDown(mouseButtonID);
            return Input.GetMouseButtonDown(mouseButtonID) || (Time.time - lastInputTime) < cacheTime;
        }

        public void Update()
        {
            if (!isCache) return;
            if(Input.GetMouseButtonDown(mouseButtonID))
            {
                lastInputTime = Time.time;
            }
            valid = GetState();
        }
    }

    public Key[] skillKeys;
    public MouseKey standAttackKey;

    private void Update()
    {
        standAttackKey.Update();
        for (int i = 0; i < skillKeys.Length; i++)
        {
            skillKeys[i].Update();
        }
    }

    public Key GetSkillKey(int skillIndex)
    {
        return skillKeys[skillIndex];
    }

    public bool GetSkillKeyState(int skillIndex)
    {
        return skillKeys[skillIndex].GetState();
    }

    public bool GetStandKeyState()
    {
        return standAttackKey.GetState();
    }

    public Vector2 GetMoveInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        return new Vector2(h, v);
    }
}
