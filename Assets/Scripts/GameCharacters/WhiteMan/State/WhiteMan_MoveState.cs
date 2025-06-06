using JKFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteMan_MoveState : GameCharacterStateBase
{
    private CharacterController characterController;
    private float runTransition;
    private bool applyRootMotionForMove;

    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        characterController = gameCharacter.CharacterController;
        applyRootMotionForMove = gameCharacter.CharacterConfig.ApplyRootMotionForMove;
    }
    public override void Enter()
    {
        runTransition = 0;

        Action<Vector3, Quaternion> rootMotionAction = null;
        if (applyRootMotionForMove) rootMotionAction = OnRootMotion;

        gameCharacter.PlayBlendAnimation("Walk", "Run", rootMotionAction);
        animation.SetBlendWeight(1);
        animation.AddAnimationEvent("FootStep", OnFootStep);
    }

    public override void Update()
    {
        if (CheckAndEnterSkillState()) return;
        // �����ҵ�����
        Vector2 cmdInput = gameCharacter.CommandController.GetMoveInput();
        float h = cmdInput.x;
        float v = cmdInput.y;

        if (h == 0 && v == 0)
        {
            // �л�״̬
            gameCharacter.ChangeState(GameCharacterState.Idle);
        }
        else
        {
            // �����ƶ�
            Vector3 input = new Vector3(h, 0, v);
            if(gameCharacter.CommandController.GetWalkKeyState())
            {
                runTransition = Mathf.Clamp(runTransition - Time.deltaTime * gameCharacter.CharacterConfig.Walk2RunTransitionSpeed, 0, 1);
            }
            else
            {
                runTransition = Mathf.Clamp(runTransition + Time.deltaTime * gameCharacter.CharacterConfig.Walk2RunTransitionSpeed, 0, 1);
            }
            animation.SetBlendWeight(1 - runTransition);

            // ��ȡ�����y��תֵ
            float y = Camera.main.transform.rotation.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0, y, 0) * input;            // ��inputҲ��תy�Ƕ�    --��Ԫ����������ˣ���ʾ����������������Ԫ��������ת��õ����µ�����
            // ������ת
            gameCharacter.Rotate(input);
            if (!applyRootMotionForMove)
            {
                float speed = Mathf.Lerp(gameCharacter.WalkSpeed, gameCharacter.RunSpeed, runTransition);
                Vector3 motion = Time.deltaTime * speed * moveDir;
                motion.y = -9.8f * Time.deltaTime;
                characterController.Move(motion);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        if (applyRootMotionForMove)
        {
            animation.ClearRootMotionAction();
        }
        animation.RemoveAnimationEvent("FootStep", OnFootStep);
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        // ��ʱ���ٶ���Ӱ�춯�������ٶ����ﵽʵ���ƶ��ٶȵı仯
        float speed = Mathf.Lerp(gameCharacter.WalkSpeed, gameCharacter.RunSpeed, runTransition);
        animation.Speed = speed;
        deltaPosition.y = -9.8f * Time.deltaTime;
        characterController.Move(deltaPosition);
    }
}
