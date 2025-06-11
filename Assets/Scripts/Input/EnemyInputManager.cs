using JKFrame;
using System;
using UnityEngine;

public class EnemyInputManager : MonoBehaviour
{
    public Key[] skillKeys;
    public MouseKey standAttackKey;
    public MouseKey heavyAttackKey;
    public Key walkKey;
    public Key dodgeKey;
    public Vector2 moveInput;

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
        return standAttackKey.valid;
    }

    public bool GetHeavyKeyState()
    {
        return heavyAttackKey.valid;
    }

    public bool GetWalkKeyState()
    {
        return walkKey.GetKeyState();
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public bool GetDodgeKeyState()
    {
        return dodgeKey.GetKeyDownState();
    }

    public void InputMoveInput(Vector2 vector)
    {
        moveInput = vector;
    }

    public void InputStandKey(bool value)
    {
        standAttackKey.valid = value;
    }

    public void CleanAllCommandsState()
    {
        for (int i = 0; i < skillKeys.Length; i++)
        {
            skillKeys[i].valid = false;
        }
        standAttackKey.valid = false;
        walkKey.valid = false;
        dodgeKey.valid = false;
        moveInput.x = 0;
        moveInput.y = 0;
    }
}
