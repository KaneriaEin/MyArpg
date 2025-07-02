using JKFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteMan_GuardState : GameCharacterStateBase
{
    public override void Enter()
    {
        gameCharacter.PlayAnimation("Guard", null, 1, false, 0);
        gameCharacter.DamageController.AddHitAction(GuardBeHitAction);
    }

    public override void Update()
    {
        // 检测玩家的输入
        bool cmdInput = gameCharacter.CommandController.GetGuardKeyState();
        if(!cmdInput)
            gameCharacter.ChangeState(GameCharacterState.Idle);
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

    private void PlayGuardAccept(AttackData atkdata)
    {
        // 音效
        if (gameCharacter.CharacterConfig.GuardAcceptDmgAudioClips.Length == 0) return;
        int index = Random.Range(0, gameCharacter.CharacterConfig.GuardAcceptDmgAudioClips.Length);
        AudioSystem.PlayOneShot(gameCharacter.CharacterConfig.GuardAcceptDmgAudioClips[index], gameCharacter.transform.position);

        // 特效
        if (gameCharacter.CharacterConfig.GuardAcceptDmgEffect == null) return;
        GameObject effect = ProjectUtility.GetOrInstantiateGameObject(gameCharacter.CharacterConfig.GuardAcceptDmgEffect, null);
        effect.transform.position = atkdata.hitPoint;
        effect.transform.LookAt(atkdata.source.ModelTransform);
        effect.transform.transform.localEulerAngles = new Vector3(0, effect.transform.transform.localEulerAngles.y, effect.transform.transform.localEulerAngles.z);
        effect.GetComponent<EffectController>().Init();
    }
}
