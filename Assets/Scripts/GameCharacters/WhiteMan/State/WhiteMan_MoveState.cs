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
        // 检测玩家的输入
        Vector2 cmdInput = gameCharacter.CommandController.GetMoveInput();
        float h = cmdInput.x;
        float v = cmdInput.y;

        if (h == 0 && v == 0)
        {
            // 切换状态
            gameCharacter.ChangeState(GameCharacterState.Idle);
        }
        else
        {
            // 处理移动
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

            // 获取相机的y旋转值
            float y = Camera.main.transform.rotation.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0, y, 0) * input;            // 让input也旋转y角度    --四元数与向量相乘：表示这个向量按照这个四元数进行旋转后得到的新的向量
            // 处理旋转
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
        // 此时的速度是影响动画播放速度来达到实际移动速度的变化
        float speed = Mathf.Lerp(gameCharacter.WalkSpeed, gameCharacter.RunSpeed, runTransition);
        animation.Speed = speed;
        deltaPosition.y = -9.8f * Time.deltaTime;
        characterController.Move(deltaPosition);
    }
}
