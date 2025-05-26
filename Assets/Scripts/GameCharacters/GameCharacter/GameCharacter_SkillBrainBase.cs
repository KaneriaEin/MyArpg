using System.Collections.Generic;

public abstract class GameCharacter_SkillBrainBase : SkillBrainBase
{
    protected GameCharacter_Controller gameCharacter;
    public virtual void Init(GameCharacter_Controller gameCharacter)
    {
        base.Init(gameCharacter);
        this.gameCharacter = gameCharacter;
        skillBehaviours = new List<SkillBehaviourBase>(skillConfigs.Count);
        for (int i = 0; i < skillConfigs.Count; i++)
        {
            AddSkill(gameCharacter, i);
        }
    }

    public void AddSkill(GameCharacter_Controller gameCharacter, int skillIndex)
    {
        SkillConfig skillConfig = skillConfigs[skillIndex];
        SkillBehaviourBase skillBehaviour = skillConfig.Behaviour.DeepCopy();
        skillBehaviour.Init(gameCharacter, skillConfig, this, skill_Player);
        skillBehaviours.Add(skillBehaviour);
    }

    public override bool CheckCost(SkillCostType costType, float cost)
    {
        switch (costType)
        {
            case SkillCostType.MP:
                return gameCharacter.CharacterProperties.currentMP >= -cost;
        }
        return false;
    }

    public override void ApplyCost(SkillCostType costType, float cost)
    {
        switch (costType)
        {
            case SkillCostType.MP:
                gameCharacter.CharacterProperties.AddMP(cost);
                break;
        }
        return;
    }

}
