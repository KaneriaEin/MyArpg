using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteMan_IdleState : GameCharacterStateBase
{
    public override void Enter()
    {
        gameCharacter.PlayAnimation("Idle");
    }

    public override void Update()
    {
        if (CheckAndEnterSkillState()) return;
        gameCharacter.CharacterController.Move(new Vector3(0, -9.8f * Time.deltaTime, 0));
        // ¼ì²âÍæ¼ÒµÄÊäÈë
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            // ÇÐ»»×´Ì¬
            gameCharacter.ChangeState(GameCharacterState.Move);
        }
    }
}
