using JKFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteMan_GuardState : GameCharacterStateBase
{
    private int PerfectGuardFrame = 3; // 根据30帧计算
    private float guardTotalTime = 0;  // 进入防御状态后的时间
    private int GameFrameRate = 30;  // 游戏帧率
    private Vector3 RepulsePos = Vector3.zero;   // 击退距离
    private float RepulseSpeed = 10;  // 击退速度

    public override void Enter()
    {
        PerfectGuardFrame = 10;
        RepulsePos = Vector3.zero;
        guardTotalTime = 0;
        gameCharacter.PlayAnimation("Guard", null, 1, false, 0);
        gameCharacter.DamageController.AddHitAction(GuardBeHitAction);
    }

    public override void Update()
    {
        guardTotalTime += Time.deltaTime;
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
            bool cmdInput = gameCharacter.CommandController.GetGuardKeyState();
            if (!cmdInput)
                gameCharacter.ChangeState(GameCharacterState.Idle);
        }
    }

    public override void Exit()
    {
        base.Exit();
        gameCharacter.DamageController.RemoveHitAction(GuardBeHitAction);
    }

    public void GuardBeHitAction(AttackData atkData)
    {
        Debug.Log(gameCharacter.name + $": 我防御住了来自{atkData.source.ModelTransform.gameObject.name}的攻击。");
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
        int index = Random.Range(0, gameCharacter.CharacterConfig.GuardAcceptDmgAudioClips.Length);
        if (gameCharacter.CharacterConfig.GuardAcceptDmgEffect.Length == 0) return;
        GameObject effect;

        // 根据是否完美防御决定播放的 音效 和 特效
        if (curFrame <= PerfectGuardFrame)
        {
            // 完美防御流程
            PlayerManager.Instance.Player.PropertyAddMP(20f);
            index = 0;
            effect = ProjectUtility.GetOrInstantiateGameObject(gameCharacter.CharacterConfig.GuardAcceptDmgEffect[0], null);
        }
        else
        {
            // 普通防御流程
            PlayerManager.Instance.Player.PropertyAddMP(-5f);
            index = 1;
            effect = ProjectUtility.GetOrInstantiateGameObject(gameCharacter.CharacterConfig.GuardAcceptDmgEffect[1], null);

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

        // 特效
        effect.transform.position = atkdata.hitPoint;
        effect.transform.LookAt(atkdata.source.ModelTransform);
        effect.transform.transform.localEulerAngles = new Vector3(0, effect.transform.transform.localEulerAngles.y, effect.transform.transform.localEulerAngles.z);
        effect.GetComponent<EffectController>().Init();

        // 如果完美防御，则对敌方产生效果
        if (curFrame <= PerfectGuardFrame)
            PerfectGuardEvent(atkdata);
    }

    private void PerfectGuardEvent(AttackData atkdata)
    {
        AttackData data = new AttackData();
        data.attackType = SkillType.PerfectGuard;
        data.source = gameCharacter;
        data.hitPoint = gameCharacter.transform.position;
        atkdata.source.CharacterBattleEvent(CharacterBattleEventType.BePerfectGuarded, new CharacterBattleEventArg { attackData = data });
    }
}
