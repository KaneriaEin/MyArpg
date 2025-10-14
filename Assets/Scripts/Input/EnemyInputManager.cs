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
    public Key guardKey;
    public Vector2 moveInput;

    public Key GetSkillKey(int skillIndex)
    {
        return skillKeys[skillIndex];
    }

    public bool GetSkillKeyState(int skillIndex)
    {
        return skillKeys[skillIndex].valid;
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
        return walkKey.valid;
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public bool GetDodgeKeyState()
    {
        return dodgeKey.valid;
    }

    public bool GetGuardKeyState()
    {
        return guardKey.valid;
    }

    public void InputMoveInput(Vector2 vector)
    {
        moveInput = vector;
    }

    public void InputStandKey(bool value)
    {
        standAttackKey.valid = value;
    }

    public void InputHeavyKey(bool value)
    {
        heavyAttackKey.valid = value;
    }

    public void InputDodgeKey(bool value)
    {
        dodgeKey.valid = value;
    }

    public void InputSkillKey(int skillIdx, bool value)
    {
        skillKeys[skillIdx].valid = value;
    }

    public void CleanAllCommandsState()
    {
        for (int i = 0; i < skillKeys.Length; i++)
        {
            skillKeys[i].valid = false;
        }
        standAttackKey.valid = false;
        heavyAttackKey.valid = false;
        walkKey.valid = false;
        dodgeKey.valid = false;
        moveInput.x = 0;
        moveInput.y = 0;
    }
}
