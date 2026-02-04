using JKFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.TMP_InputField;

public class WhiteMan_GuardState : GameCharacterStateBase
{
    private int PerfectGuardFrame = 3; // 根据30帧计算
    private float guardTotalTime = 0;  // 进入防御状态后的时间
    private int GameFrameRate = 30;  // 游戏帧率
    private Vector3 RepulsePos = Vector3.zero;   // 击退距离
    private float RepulseSpeed = 10;  // 击退速度
    private ICharacter atkSource;
    private bool duringGuard = false; // 防御姿态
    private bool duringPFGuardAttack = false; // 弹反后的出招时间

    private List<GameObject> perfectGuardEffects = new List<GameObject>();

    public override void Enter()
    {
        PerfectGuardFrame = 10;
        RepulsePos = Vector3.zero;
        guardTotalTime = 0;
        gameCharacter.PlayAnimation("Guard", null, 1, false, 0);
        gameCharacter.DamageController.AddHitAction(GuardBeHitAction);
        animation.AddAnimationEvent("PerfectGuardBulletTimeStart", PerfectGuardBulletTimeStart);
        animation.AddAnimationEvent("PerfectGuardAttack", PerfectGuardAttack);
        animation.AddAnimationEvent("PerfectGuardOver", PerfectGuardOver);
    }

    public override void Update()
    {
        guardTotalTime += Time.deltaTime;
        if (duringGuard)
        {
            if (duringPFGuardAttack)
            {
                if (CheckAndEnterSkillState())
                {
                    duringPFGuardAttack = false;
                    // BattleEventManager.Instance.StopBattleBulletTime();
                    return; 
                }
            }
            return;
        }
        if (RepulsePos != Vector3.zero && Vector3.Distance(gameCharacter.transform.position, RepulsePos) > 1f)
        {
            // 击退效果
            Vector3 moveDir = RepulsePos - gameCharacter.transform.position;
            moveDir.Normalize();
            gameCharacter.CharacterController.Move(moveDir * Time.deltaTime * RepulseSpeed);
            if (Vector3.Distance(gameCharacter.transform.position, RepulsePos) <= 1f)
            {
                RepulsePos = Vector3.zero;
            }
        }
        else
        {
            // 不处于防御硬直中
            // 检测玩家的输入
            if (BattleEventManager.Instance.BulletTimeOn)
            {
                if (CheckAndEnterSkillState()) return;
            }
            else
            {
                bool cmdInput = gameCharacter.CommandController.GetGuardKeyState();
                if (!cmdInput)
                    gameCharacter.ChangeState(GameCharacterState.Idle);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        duringGuard = false;
        atkSource = null;
        perfectGuardEffects.Clear();
        CameraManager.Instance.DefenceStop();//todo
        gameCharacter.SkillBrain.AddorUpdateShareData(WhiteManSkillBrain.SPSkillKey, false);//todo
        animation.RemoveAnimationEvent("PerfectGuardBulletTimeStart");
        animation.RemoveAnimationEvent("PerfectGuardAttack");
        animation.RemoveAnimationEvent("PerfectGuardOver");
        gameCharacter.DamageController.RemoveHitAction(GuardBeHitAction);
    }

    public void GuardBeHitAction(AttackData atkData)
    {
        // Debug.Log(gameCharacter.name + $": 我防御住了来自{atkData.source.ModelTransform.gameObject.name}的攻击。");
        // TODO:将来可根据atkEvent中的参数(如伤害，或破防值)改变对应动画类型;
        gameCharacter.ModelTransform.LookAt(atkData.source.ModelTransform);
        gameCharacter.PlayAnimation("GuardDmgAccept", null, 1, true, 0f);
        PlayGuardAccept(atkData);
    }

    /// <summary>
    /// 处理防御时的音效、特效以及其他事件
    /// </summary>
    private void PlayGuardAccept(AttackData atkdata)
    {
        int curFrame = (int)(guardTotalTime * GameFrameRate);
        if (gameCharacter.CharacterConfig.GuardAcceptDmgAudioClips.Length == 0) return;
        int index = UnityEngine.Random.Range(0, gameCharacter.CharacterConfig.GuardAcceptDmgAudioClips.Length);
        if (gameCharacter.CharacterConfig.GuardAcceptDmgEffect.Length == 0) return;
        GameObject effect;
        atkSource = atkdata.source;

        // 根据是否完美防御决定播放的 音效 和 特效
        if (curFrame <= PerfectGuardFrame)
        {
            //// 完美防御流程
            // 角色状态奖励
            PlayerManager.Instance.Player.PropertyAddMP(20f);
            index = 0;
            effect = ProjectUtility.GetOrInstantiateGameObject(gameCharacter.CharacterConfig.GuardAcceptDmgEffect[1], null);
            effect.transform.position = atkdata.hitPoint;
            effect.transform.LookAt(atkdata.source.ModelTransform);
            effect.transform.transform.localEulerAngles = new Vector3(0, effect.transform.transform.localEulerAngles.y, effect.transform.transform.localEulerAngles.z);
            effect.GetComponent<EffectController>().Init();

            // 若是精防黄光技能
            if (atkdata.pgPunish)
            {
                PlayerManager.Instance.Player.HitTargetStatus = HitTargetStatus.Invincibility;
                // 特效

                effect = ProjectUtility.GetOrInstantiateGameObject(gameCharacter.CharacterConfig.GuardAcceptDmgEffect[2], null);
                effect.GetComponent<EffectController>().Init();
                effect.GetComponent<ParticleSystem>().Simulate(0.12f, true, true, false);
                effect.GetComponent<ParticleSystem>().Pause();
                effect.transform.position = atkdata.hitPoint;
                effect.transform.LookAt(atkdata.source.ModelTransform);
                effect.transform.transform.localEulerAngles = new Vector3(0, effect.transform.transform.localEulerAngles.y, effect.transform.transform.localEulerAngles.z);
                perfectGuardEffects.Add(effect);
                // 0.2s后特效恢复播放
                MonoSystem.Start_Coroutine(PlayPerfectGuardEffects(0.2f));
                // 进入精防特写 , 触发震动和模糊
                CameraManager.Instance.DefenceStart();
                CameraShakeConfig shakeConfig = new CameraShakeConfig();
                shakeConfig.shakeShape = CameraShakeShape.PerfectGuard;
                shakeConfig.baseAmplitude = 0.2f;
                shakeConfig.screenDirectionBias = new Vector2(1, -1);
                CameraShakeManager.Instance.TriggerShake(shakeConfig);
                PostProcessingManager.Instance.TriggerRadialBlur(0.1f, 0.4f, 0.3f, new Vector2(0.37f,0.66f), 0.1f);
            }
        }
        else
        {
            // 普通防御流程
            PlayerManager.Instance.Player.PropertyAddMP(-5f);
            // 特效
            index = 1;
            effect = ProjectUtility.GetOrInstantiateGameObject(gameCharacter.CharacterConfig.GuardAcceptDmgEffect[0], null);
            // 特效
            effect.transform.position = atkdata.hitPoint;
            effect.transform.LookAt(atkdata.source.ModelTransform);
            effect.transform.transform.localEulerAngles = new Vector3(0, effect.transform.transform.localEulerAngles.y, effect.transform.transform.localEulerAngles.z);
            effect.GetComponent<EffectController>().Init();

            // 有后退距离,后退到角色人身后的xx距离，这里设计距离为2
            Vector3 moveDir = gameCharacter.transform.position - atkdata.hitPoint;
            Vector2 moveDirXZ = new Vector2 { x = moveDir.x, y = moveDir.z };
            moveDirXZ.Normalize();
            moveDir.x = moveDirXZ.x; moveDir.z = moveDirXZ.y;
            moveDir = moveDir * 2;
            moveDir.y = 0;
            RepulsePos = gameCharacter.transform.position + moveDir;
        }

        // 音效
        AudioSystem.PlayOneShot(gameCharacter.CharacterConfig.GuardAcceptDmgAudioClips[index], gameCharacter.transform.position);



        // 如果完美防御黄光招式，则产生子弹时间，定格敌方命中时的一瞬间0.3s
        if (curFrame <= PerfectGuardFrame && atkdata.pgPunish)
        {
            duringGuard = true;
            BattleEventManager.Instance.BattleBulletTimeEvent(0.35f, 0, PerfectGuardEvent);
        }
    }

    private void PerfectGuardEvent()
    {
        gameCharacter.PlayAnimation("GuardPerfect", null, 1, true, 0f);
    }

    /// <summary>
    /// 弹反动画的攻击帧事件
    /// </summary>
    private void PerfectGuardAttack()
    {
        // 相机震动
        // CameraManager.Instance.CameraGenerateImpulse(new Vector3(1, 1, 3));

        AttackData data = new AttackData();
        data.attackType = SkillType.PerfectGuard;
        data.source = gameCharacter;
        data.hitPoint = gameCharacter.transform.position;
        atkSource.CharacterBattleEvent(CharacterBattleEventType.BePerfectGuarded, new CharacterBattleEventArg { attackData = data });
        AudioSystem.PlayOneShot(gameCharacter.CharacterConfig.GuardAcceptDmgAudioClips[2], gameCharacter.transform.position);
    }

    private IEnumerator PlayPerfectGuardEffects(float time)
    {
        yield return new WaitForSeconds(time);

        for (int i = 0; i < perfectGuardEffects.Count; i++)
        {
            perfectGuardEffects[i].GetComponent<ParticleSystem>().Play();
        }
    }

    /// <summary>
    /// 弹反后的出技能子弹时间，持续4秒
    /// 此刻等待玩家输入技能指令 或 等待时间流逝自动退出此状态
    /// </summary>
    private void PerfectGuardBulletTimeStart()
    {
        duringPFGuardAttack = true;
        gameCharacter.SkillBrain.AddorUpdateShareData(WhiteManSkillBrain.SPSkillKey, true);
        // BattleEventManager.Instance.BattleBulletTimeEvent(4f, 0.01f, PerfectGuardBulletTimeOver);
    }

    /// <summary>
    /// 弹反弹反反击动作结束
    /// 恢复idle
    /// </summary>
    private void PerfectGuardOver()
    {
        duringPFGuardAttack = false;
        duringGuard = false;
        gameCharacter.SkillBrain.AddorUpdateShareData(WhiteManSkillBrain.SPSkillKey, false);
        // 退出精防特写
        CameraManager.Instance.DefenceStop();
        PlayerManager.Instance.Player.HitTargetStatus = HitTargetStatus.None;
        gameCharacter.ChangeState(GameCharacterState.Idle);
    }

    private void PerfectGuardBulletTimeOver()
    {
        gameCharacter.SkillBrain.AddorUpdateShareData(WhiteManSkillBrain.SPSkillKey, false);
        duringGuard = false;

        // 退出精防特写
        CameraManager.Instance.DefenceStop();
        PlayerManager.Instance.Player.HitTargetStatus = HitTargetStatus.None;
    }
}
