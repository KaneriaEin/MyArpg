using JKFrame;
using System;
using System.Collections;
using UnityEngine;
using static TMPro.TMP_InputField;

public class NodachiMan_Controller : GameCharacter_Controller
{
    public override void Init(CharacterConfig characterConfig, Enemy_Controller enemy_Controller)
    {
        base.Init(characterConfig, enemy_Controller);
        NodachiManAnimEventInit();

        #region 测试敌人的ui显示，临时代码
        CharacterProperties.OnCurrentHPChanged += UIAddHP;
        CharacterProperties.OnCurrentStunChanged += UIAddStun;
        CharacterProperties.OnCurrentStunInStun += UIInStun;
        CharacterProperties.OnCurrentThunderDebuffGaugeChanged += UIAddThunderDebuffGauge;
        CharacterProperties.OnCurrentThunderExploChanged += UIAddThunderExplo;
        #endregion
    }

    public override void ChangeState(GameCharacterState newState, bool reCurrstate = false)
    {
        base.ChangeState(newState, reCurrstate);
        switch (this.gameCharacterState)
        {
            case GameCharacterState.Idle:
                stateMachine.ChangeState<NodachiMan_IdleState>(reCurrstate);
                break;
            case GameCharacterState.Move:
                stateMachine.ChangeState<NodachiMan_MoveState>(reCurrstate);
                break;
            case GameCharacterState.Skill:
                stateMachine.ChangeState<NodachiMan_SkillState>(reCurrstate);
                break;
            case GameCharacterState.Damaged:
                stateMachine.ChangeState<NodachiMan_DamagedState>(reCurrstate);
                break;
            case GameCharacterState.Die:
                stateMachine.ChangeState<NodachiMan_DieState>(reCurrstate);
                break;
            case GameCharacterState.Charge:
                stateMachine.ChangeState<NodachiMan_ChargeState>(reCurrstate);
                break;
            case GameCharacterState.Stun:
                stateMachine.ChangeState<NodachiMan_StunState>(reCurrstate);
                break;
        }
    }

    public override void OnDie(string name)
    {
        base.OnDie(name);
    }

    public override void CharacterBattleEvent(CharacterBattleEventType eventType, CharacterBattleEventArg arg)
    {
        switch (eventType)
        {
            case CharacterBattleEventType.BePerfectGuarded:
                if (gameCharacterState == GameCharacterState.Skill && SkillBrain.CurrentSkillClip.PGuardPunish)
                    this.DamageController.TakeDamageFromAttackData(arg.attackData);
                break;
            default:
                break;
        }
    }

    public void NodachiManStunRecoverAction()
    {
        ChangeToIdleState();
        //if (gameCharacterState == GameCharacterState.Idle)
        //{
        //    canChangeState = false;
        //    PlayAnimation("StunIdle_End");
        //}
    }

    #region 动画注册相关
    private void NodachiManAnimEventInit()
    {
        Animation_Controller.RemoveAnimationEvent("ResetAnimationLayer1Weight");
        Animation_Controller.AddAnimationEvent("ResetAnimationLayer1Weight", ResetAnimationLayer1Weight);
    }

    private void ResetAnimationLayer1Weight()
    {
        SetAnimationLayerWeight(1, 0f);
    }
    #endregion

    #region enemyManager rpc相关
    #endregion

    #region ThunderBuff相关
    public override void PropertyAddThunderDebuff(float value, BuffConfig buff)
    {
        if (value > 0)
        {
            if (BuffController.GetBuffLayer(buff) == 0)
            {
                CharacterProperties.AddThunderDebuffGauge(value);
                if (CharacterProperties.GetThunderDebuffGauge() == 100)
                {
                    BuffController.AddBuff(buff);
                    UIAddThunderDebuff(true);
                }
            }
            else
            {
                PropertyAddThunderExplo(value, buff);
            }
        }
        else
        {
            if (BuffController.GetBuffLayer(buff) > 0)
            {
                CharacterProperties.AddThunderDebuffGauge(value);
                if (CharacterProperties.GetThunderDebuffGauge() == 0)
                {
                    BuffController.RemoveBuff(buff);
                    UIAddThunderDebuff(false);
                    float explo = CharacterProperties.GetThunderExploGauge();
                    PropertyAddThunderExplo(-explo, buff);
                }
            }
        }
    }

    public override void PropertyAddThunderExplo(float value, BuffConfig buff)
    {
        float setValue = CharacterProperties.GetThunderExploGauge();
        setValue += value;
        while (setValue >= 100)
        {
            // 发生一个雷暴
            AttackData data = new AttackData();
            data.attackType = SkillType.Skill;
            data.source = PlayerManager.Instance.Player;
            data.hitPoint = ModelTransform.transform.position;
            data.attackValue = 20f;
            data.detectionEvent = new SkillAttackDetectionEvent();
            data.detectionEvent.TrackName = "downthunder";
            data.detectionEvent.AttackHitConfig = new AttackHitConfig();
            data.detectionEvent.AttackHitConfig.BreakArmorLevel = 2;
            data.detectionEvent.AttackHitConfig.HitAudioClip = ((DamageBuffEffectData)buff.endEffect).HitAudioClip;
            // 特效
            if (((DamageBuffEffectData)buff.endEffect).HitEffectPrefab != null)
            {
                GameObject effect = ProjectUtility.GetOrInstantiateGameObject(((DamageBuffEffectData)buff.endEffect).HitEffectPrefab, null);
                effect.transform.position = data.hitPoint;
                effect.GetComponent<EffectController>().Init(0, true);
            }

            CameraShakeConfig shakeConfig = new CameraShakeConfig();
            shakeConfig.shakeShape = CameraShakeShape.Light;
            shakeConfig.baseAmplitude = 1f;
            shakeConfig.screenDirectionBias = Vector2.down;
            CameraShakeManager.Instance.TriggerShake(shakeConfig, ModelTransform.transform.position);
            DamageController.TakeDamage(data);

            setValue -= 100;
        }
        CharacterProperties.SetThunderExploGauge(setValue);
    }
    #endregion

    #region 测试敌人的ui显示，临时代码
    public void UIAddHP(float hp)
    {
        JKFrame.EventSystem.EventTrigger<float>("OnNodachiHPChanged", hp);
    }

    public void UIAddStun(float stun)
    {
        JKFrame.EventSystem.EventTrigger<float>("OnNodachiStunChanged", stun);
    }

    public void UIInStun(bool stun)
    {
        JKFrame.EventSystem.EventTrigger<bool>("OnNodachiStunInStun", stun);
    }

    public void UIAddThunderDebuff(bool value)
    {
        JKFrame.EventSystem.EventTrigger<bool>("OnNodachiGetThunderDebuffChanged", value);
    }

    public void UIAddThunderDebuffGauge(float value)
    {
        JKFrame.EventSystem.EventTrigger<float>("OnNodachiGetThunderDebuffGaugeChanged", value);
    }

    public void UIAddThunderExplo(float value)
    {
        JKFrame.EventSystem.EventTrigger<float>("OnNodachiGetThunderExploChanged", value);
    }
    #endregion
}
