using System.Collections.Generic;

public abstract class Enemy_SkillBrainBase : SkillBrainBase
{
    protected PersonBS_Controller enemy;
    public virtual void Init(PersonBS_Controller enemy)
    {
        base.Init(enemy);
        this.enemy = enemy;
        skillBehaviours = new List<SkillBehaviourBase>(skillConfigs.Count);
        for (int i = 0; i < skillConfigs.Count; i++)
        {
            AddSkill(enemy, i);
        }
    }

    public void AddSkill(PersonBS_Controller player, int skillIndex)
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
                return enemy.CharacterProperties.currentMP >= -cost;
        }
        return false;
    }

    public override void ApplyCost(SkillCostType costType, float cost)
    {
        switch (costType)
        {
            case SkillCostType.MP:
                enemy.CharacterProperties.AddMP(cost);
                break;
        }
        return;
    }

}
