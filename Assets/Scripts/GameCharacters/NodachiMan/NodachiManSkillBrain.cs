using UnityEngine;

public class NodachiManSkillBrain : GameCharacter_SkillBrainBase
{
    public const string SkillChargeTime = "SkillChargeTime";
    public const string SkillChargeFinish = "SkillChargeFinish";
    public const string SkillChargeEffect = "SkillChargeEffect";
    public const string SkillChargeInterrupt = "SkillChargeInterrupt";

    public override void Init(GameCharacter_Controller gameCharacter)
    {
        base.Init(gameCharacter);
        AddorUpdateShareData(SkillChargeTime, 5f);
        AddorUpdateShareData(SkillChargeFinish, false);
        AddorUpdateShareData(SkillChargeInterrupt, false);
        AddorUpdateShareData<GameObject>(SkillChargeEffect, null);
    }
}
