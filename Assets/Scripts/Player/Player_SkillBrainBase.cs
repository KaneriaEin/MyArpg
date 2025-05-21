using System.Collections.Generic;

public abstract class Player_SkillBrainBase : SkillBrainBase
{
    protected Player_Controller player;
    public virtual void Init(Player_Controller player)
    {
        base.Init(player);
        this.player = player;
        skillBehaviours = new List<SkillBehaviourBase>(skillConfigs.Count);
        for (int i = 0; i < skillConfigs.Count; i++)
        {
            AddSkill(player, i);
        }
    }

    public void AddSkill(Player_Controller player, int skillIndex)
    {
        SkillConfig skillConfig = skillConfigs[skillIndex];
        SkillBehaviourBase skillBehaviour = skillConfig.Behaviour.DeepCopy();
        skillBehaviour.Init(player, skillConfig, this, skill_Player);
        skillBehaviours.Add(skillBehaviour);
    }

    public override bool CheckCost(SkillCostType costType, float cost)
    {
        switch (costType)
        {
            case SkillCostType.MP:
                return player.CharacterProperties.currentMP >= -cost;
        }
        return false;
    }

    public override void ApplyCost(SkillCostType costType, float cost)
    {
        switch (costType)
        {
            case SkillCostType.MP:
                player.CharacterProperties.AddMP(cost);
                break;
        }
        return;
    }

}
