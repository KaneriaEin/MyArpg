using JKFrame;
using System;
using UnityEngine;

public class InputManager : SingletonMono<InputManager>
{
    public Key[] skillKeys;
    public MouseKey standAttackKey;
    public Key walkKey;

    private void Update()
    {
        standAttackKey.Update();
        walkKey.Update();
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
        return skillKeys[skillIndex].GetKeyDownState();
    }

    public bool GetStandKeyState()
    {
        return standAttackKey.GetState();
    }

    public bool GetWalkKeyState()
    {
        return walkKey.GetKeyState();
    }

    public Vector2 GetMoveInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        return new Vector2(h, v);
    }
}
