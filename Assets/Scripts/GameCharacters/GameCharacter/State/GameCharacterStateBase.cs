using JKFrame;

/// <summary>
/// 玩家状态的基类
/// </summary>
public abstract class GameCharacterStateBase:StateBase
{
    protected Animation_Controller animation;
    protected GameCharacter_Controller gameCharacter;
    protected static int currentReleaseSkillIndex;
    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        gameCharacter = (GameCharacter_Controller)owner;
        animation = gameCharacter.Animation_Controller;
    }

    protected bool CheckAndEnterSkillState()
    {
        // TODO: 这段代码太蠢，可能不需要这么写，毕竟玩家一时间也只会输入一个指令，不太会冲突
        // TODO: 或者把所有技能按优先级塞进skillBehaviours，按照优先顺序遍历；
        bool valid = false;
        valid = CheckDodgeInput();
        if (valid)
        {
            gameCharacter.ChangeState(GameCharacterState.Skill);
            return true;
        }
        valid = CheckHeavyAttackInput();
        if (valid)
        {
            gameCharacter.ChangeState(GameCharacterState.Skill);
            return true;
        }
        valid = CheckSkillInput();
        if (valid)
        {
            gameCharacter.ChangeState(GameCharacterState.Skill);
            return true;
        }
        valid = CheckStandAttackInput();
        if (valid)
        {
            gameCharacter.ChangeState(GameCharacterState.Skill);
            return true;
        }

        return false;
    }

    protected bool CheckDodgeInput()
    {
        // 默认0是普攻1是闪避2是重击
        bool valid;
        valid = gameCharacter.CommandController.GetDodgeKeyState() && gameCharacter.SkillBrain.CheckReleaseSkill(1);
        if (valid)
        {
            currentReleaseSkillIndex = 1;
            return true;
        }

        return false;
    }

    protected bool CheckSkillInput()
    {
        // 默认0是普攻1是闪避2是重击
        bool valid;
        for (int i = 3; i < gameCharacter.SkillBrain.SkillConfigCount; i++)
        {
            valid = gameCharacter.CommandController.GetSkillKeyState(i - 1) && gameCharacter.SkillBrain.CheckReleaseSkill(i);

            if (valid)
            {
                currentReleaseSkillIndex = i;
                return true;
            }
        }

        return false;
    }

    protected bool CheckStandAttackInput()
    {
        // 默认0是普攻1是闪避2是重击
        bool valid = false;
        valid = gameCharacter.CommandController.GetStandKeyState() && gameCharacter.SkillBrain.CheckReleaseSkill(0);

        if (valid)
        {
            currentReleaseSkillIndex = 0;
            return true;
        }
        return false;
    }

    protected bool CheckHeavyAttackInput()
    {
        // 默认0是普攻1是闪避2是重击
        bool valid = false;
        valid = gameCharacter.CommandController.GetHeavyKeyState() && gameCharacter.SkillBrain.CheckReleaseSkill(2);

        if (valid)
        {
            currentReleaseSkillIndex = 2;
            return true;
        }
        return false;
    }

    protected void OnFootStep()
    {
        int index = UnityEngine.Random.Range(0, gameCharacter.CharacterConfig.FootStepAudioClips.Length);
        AudioSystem.PlayOneShot(gameCharacter.CharacterConfig.FootStepAudioClips[index], gameCharacter.transform.position);
    }
}
