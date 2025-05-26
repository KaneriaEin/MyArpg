using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonBS_IdleState : GameCharacterStateBase
{
    public override void Enter()
    {
        gameCharacter.PlayAnimation("Idle");
    }

    public override void Update()
    {
        // TODO: 这里读输入的地方需要再做一层，分辨用户和ai的输入
        //if (CheckAndEnterSkillState()) return;
        gameCharacter.CharacterController.Move(new Vector3(0, -9.8f * Time.deltaTime, 0));
        // 检测玩家的输入
        // float h = Input.GetAxis("Horizontal");
        // float v = Input.GetAxis("Vertical");
        float h = 0;
        float v = 0;

        if (h != 0 || v != 0)
        {
            // 切换状态
            gameCharacter.ChangeState(GameCharacterState.Move);
        }
    }
}
