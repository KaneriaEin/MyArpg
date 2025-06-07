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
        return enemyInputManager.GetSkillKey(skillIndex);
    }

    public override bool GetSkillKeyState(int skillIndex)
    {
        return enemyInputManager.GetSkillKeyState(skillIndex);
    }

    public override bool GetStandKeyState()
    {
        return enemyInputManager.GetStandKeyState();
    }

    public override bool GetWalkKeyState()
    {
        return enemyInputManager.GetWalkKeyState();
    }

    public override bool GetDodgeKeyState()
    {
        return enemyInputManager.GetDodgeKeyState();
    }

    public override void CleanAllCommandsState()
    {
        enemyInputManager.CleanAllCommandsState();
    }
}
