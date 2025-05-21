using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Player_SkillBehaviourBase : SkillBehaviourBase
{
    protected Player_Controller player;
    public override void Init(ICharacter owner, SkillConfig skillConfig, SkillBrainBase skillBrain, Skill_Player skill_Player)
    {
        base.Init(owner, skillConfig, skillBrain, skill_Player);
        player = (Player_Controller)owner;
    }
}
