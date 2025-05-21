using JKFrame;

/// <summary>
/// 玩家状态的基类
/// </summary>
public abstract  class PlayerStateBase:StateBase
{
    protected Animation_Controller animation;
    protected Player_Controller player;
    protected static int currentReleaseSkillIndex;
    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        player = (Player_Controller)owner;
        animation = player.Animation_Controller;
    }

    // TODO:临时测试逻辑
    protected bool CheckAndEnterSkillState()
    {
        // 默认0是普攻
        for(int i = 0; i< player.SkillBrain.SkillConfigCount; i++)
        {
            bool valid;
            if (i == 0)
                valid = InputManager.Instance.GetStandKeyState() && player.SkillBrain.CheckReleaseSkill(0);
            else
                valid = InputManager.Instance.GetSkillKeyState(i - 1) && player.SkillBrain.CheckReleaseSkill(i);

            if (valid)
            {
                currentReleaseSkillIndex = i;
                player.ChangeState(PlayerState.Skill);
                return true;
            }
        }

        return false;
    }

    protected void OnFootStep()
    {
        int index = UnityEngine.Random.Range(0, player.CharacterConfig.FootStepAudioClips.Length);
        AudioSystem.PlayOneShot(player.CharacterConfig.FootStepAudioClips[index], player.transform.position);
    }
}
