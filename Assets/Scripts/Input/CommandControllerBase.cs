using UnityEngine;

public abstract class CommandControllerBase : MonoBehaviour
{
    public abstract Key GetSkillKey(int skillIndex);
    public abstract bool GetSkillKeyState(int skillIndex);
    public abstract bool GetStandKeyState();
    public abstract bool GetWalkKeyState();
    public abstract bool GetDodgeKeyState();
    public abstract Vector2 GetMoveInput();
}
