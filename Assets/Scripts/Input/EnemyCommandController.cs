using UnityEngine;

public class EnemyCommandController : CommandControllerBase
{
    [SerializeField] private EnemyInputManager enemyInputManager;
    public override Vector2 GetMoveInput()
    {
        return enemyInputManager.GetMoveInput();
    }

    public override Key GetSkillKey(int skillIndex)
    {
        return null;
    }

    public override bool GetSkillKeyState(int skillIndex)
    {
        return false;
    }

    public override bool GetStandKeyState()
    {
        return false;
    }

    public override bool GetRunKeyState()
    {
        return false;
    }
}
