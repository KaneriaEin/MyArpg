using JKFrame;

/// <summary>
/// 玩家状态的基类
/// </summary>
public abstract class GameCharacterStateBase:StateBase
{
    protected Animation_Controller animation;
    protected GameCharacter_Controller gameCharacter;
    protected static int currentReleaseSkillIndex;
    protected static AttackData curAttackData;
    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        gameCharacter = (GameCharacter_Controller)owner;
        animation = gameCharacter.Animation_Controller;
    }

    protected bool CheckAndEnterSkillState()
    {
        // 默认0是普攻
        for(int i = 0; i< gameCharacter.SkillBrain.SkillConfigCount; i++)
        {
            bool valid;
            if (i == 0)
                valid = gameCharacter.CommandController.GetStandKeyState() && gameCharacter.SkillBrain.CheckReleaseSkill(0);
            else
                valid = gameCharacter.CommandController.GetSkillKeyState(i - 1) && gameCharacter.SkillBrain.CheckReleaseSkill(i);

            if (valid)
            {
                currentReleaseSkillIndex = i;
                gameCharacter.ChangeState(GameCharacterState.Skill);
                return true;
            }
        }

        return false;
    }

    protected void OnFootStep()
    {
        int index = UnityEngine.Random.Range(0, gameCharacter.CharacterConfig.FootStepAudioClips.Length);
        AudioSystem.PlayOneShot(gameCharacter.CharacterConfig.FootStepAudioClips[index], gameCharacter.transform.position);
    }
}
