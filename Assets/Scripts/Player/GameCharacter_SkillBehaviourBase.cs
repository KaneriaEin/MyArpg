using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class GameCharacter_SkillBehaviourBase : SkillBehaviourBase
{
    protected GameCharacter_Controller character;
    public override void Init(ICharacter owner, SkillConfig skillConfig, SkillBrainBase skillBrain, Skill_Player skill_Player)
    {
        base.Init(owner, skillConfig, skillBrain, skill_Player);
        character = (GameCharacter_Controller)owner;
    }

    /// <summary>
    /// Exit Skillstate时调用
    /// </summary>
    public override void Stop()
    {
        base.Stop();
        skill_Player.StopSkillClip();
        OnClipEndOrReleaseNewSkill();
    }
}
