using UnityEngine;

public abstract class CommandControllerBase : MonoBehaviour
{
    public abstract Key GetSkillKey(int skillIndex);
    public abstract bool GetSkillKeyState(int skillIndex);
    public abstract bool GetStandKeyState();
    public abstract bool GetRunKeyState();
    public abstract Vector2 GetMoveInput();
}
