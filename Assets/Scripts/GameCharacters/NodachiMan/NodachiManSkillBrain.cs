using UnityEngine;

public class NodachiManSkillBrain : GameCharacter_SkillBrainBase
{
    public const string Skill2 = "Skill2";
    public const string Skill2ChargeTime = "Skill2ChargeTime";
    public const string SkillChargeTime = "SkillChargeTime";
    public const string CurrentChargeSkill = "CurrentChargeSkill";
    public const string SkillChargeFinish = "SkillChargeFinish";
    public const string SkillChargeEffect = "SkillChargeEffect";

    public override void Init(GameCharacter_Controller gameCharacter)
    {
        base.Init(gameCharacter);
        AddorUpdateShareData(SkillChargeTime, -1f);
        AddorUpdateShareData(Skill2ChargeTime, 5f);
        AddorUpdateShareData(SkillChargeFinish, false);
        AddorUpdateShareData<GameObject>(SkillChargeEffect, null);
    }
}
