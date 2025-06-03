using JKFrame;
using System;
using UnityEngine;

public class EnemyInputManager : MonoBehaviour
{
    public Key[] skillKeys;
    public MouseKey standAttackKey;
    public Key walkKey;
    public Vector2 moveInput;

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
        return moveInput;
    }

    public void InputMoveInput(Vector2 vector)
    {
        moveInput = vector;
    }
}
