using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("GameCharacter")]
[TaskDescription("本角色HP低于某值")]
public class Behaviour_IsHPLower : GameCharacterConditional
{
    public SharedFloat hpPercent = 0f; // 可配置的攻击范围
    public float currentHpPercent;
    public override void OnStart()
    {
        currentHpPercent = controller.CharacterProperties.currentHP / controller.CharacterProperties.maxHp.BaseValue;
    }
    public override TaskStatus OnUpdate()
    {
        if (currentHpPercent < hpPercent.Value)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}
