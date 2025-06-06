using UnityEngine;

public class PlayerCommandController : CommandControllerBase
{

    public override Vector2 GetMoveInput()
    {
        return InputManager.Instance.GetMoveInput();
    }

    public override Key GetSkillKey(int skillIndex)
    {
        return InputManager.Instance.GetSkillKey(skillIndex);
    }

    public override bool GetSkillKeyState(int skillIndex)
    {
        return InputManager.Instance.GetSkillKeyState(skillIndex);
    }

    public override bool GetStandKeyState()
    {
        return InputManager.Instance.GetStandKeyState();
    }

    public override bool GetWalkKeyState()
    {
        return InputManager.Instance.GetWalkKeyState();
    }

    public override bool GetDodgeKeyState()
    {
        return InputManager.Instance.GetDodgeKeyState();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            CameraManager.Instance.LockOn();
        }
    }
}
