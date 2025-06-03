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

    public override bool GetRunKeyState()
    {
        return InputManager.Instance.GetWalkKeyState();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            CameraManager.Instance.LockOn();
        }
    }
}
