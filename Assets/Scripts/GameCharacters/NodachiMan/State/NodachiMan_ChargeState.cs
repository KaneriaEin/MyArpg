using JKFrame;
using UnityEngine;

public class NodachiMan_ChargeState : GameCharacterStateBase
{
    float chargeTime = 0f;
    GameObject chargetEffect = null;
    int currentSkill = -1;
    public override void Enter()
    {
        //Debug.Log("Enter NodachiMan_ChargeState");
        gameCharacter.AddHitFreezeAction(TargetHitFreezeStart, TargetHitFreezeFinish);
        gameCharacter.PlayAnimationSequentially("Skill2ChargeStart", null, 1, false, 0f, () => { gameCharacter.PlayAnimation("Skill2ChargeLoop"); });
        
        gameCharacter.SkillBrain.TryGetSkillShareData(NodachiManSkillBrain.SkillChargeTime, out chargeTime);
        gameCharacter.SkillBrain.TryGetSkillShareData(NodachiManSkillBrain.CurrentChargeSkill, out currentSkill);
        if (chargeTime < 1f) chargeTime = 1.5f;  // 蓄力时间起码为1.5s

        gameCharacter.SkillBrain.TryGetSkillShareData(NodachiManSkillBrain.SkillChargeEffect, out GameObject effect);
        if(effect != null)
        {
            chargetEffect = ProjectUtility.GetOrInstantiateGameObject(effect, null);
            chargetEffect.transform.position = gameCharacter.ModelTransform.position + new Vector3(0,0.5f,0);
            chargetEffect.transform.LookAt(Camera.main.transform.position);
            chargetEffect.GetComponent<EffectController>().Init(chargeTime, 0);
        }
    }

    public override void Exit()
    {
        base.Exit();
        gameCharacter.RemoveHitFreezeAction(TargetHitFreezeStart, TargetHitFreezeFinish);
        if (chargeTime > 0f) 
        {
            chargetEffect.GameObjectPushPool();
            gameCharacter.SkillBrain.AddorUpdateShareData("Skill" + currentSkill.ToString() + "ChargeTime", chargeTime);
        }
        chargetEffect = null;

        //Debug.Log("Exit NodachiMan_ChargeState");
    }

    public override void Update()
    {
        chargeTime -= Time.deltaTime;
        if (chargeTime < 0f)
        {
            Debug.Log("Update SkillChargeFinish");
            gameCharacter.SkillBrain.AddorUpdateShareData(NodachiManSkillBrain.SkillChargeFinish, true);
            gameCharacter.SkillBrain.AddorUpdateShareData("Skill" + currentSkill.ToString() + "ChargeTime", -1f);
            currentReleaseSkillIndex = currentSkill;
            gameCharacter.ChangeState(GameCharacterState.Skill);
        }
    }

    #region TargetHitFreeze相关
    public void TargetHitFreezeStart()
    {
        gameCharacter.SkillBrain.Skill_Player.SkillHitFreezeStart();
        gameCharacter.SetAnimationLayerWeight(1, 1f);
        gameCharacter.PlayAnimation_Layer1("Damage_LittleHit", null, 1 * gameCharacter.LocalTimeScale * 0.5f, true, 0.1f);

    }

    public void TargetHitFreezeFinish()
    {
        gameCharacter.SkillBrain.Skill_Player.SkillHitFreezeFinish();
        gameCharacter.SetAnimationLayerWeight(1, 0f);
    }
    #endregion

}
