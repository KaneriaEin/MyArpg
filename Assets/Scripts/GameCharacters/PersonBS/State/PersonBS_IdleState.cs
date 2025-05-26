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
        // TODO: ���������ĵط���Ҫ����һ�㣬�ֱ��û���ai������
        //if (CheckAndEnterSkillState()) return;
        gameCharacter.CharacterController.Move(new Vector3(0, -9.8f * Time.deltaTime, 0));
        // �����ҵ�����
        // float h = Input.GetAxis("Horizontal");
        // float v = Input.GetAxis("Vertical");
        float h = 0;
        float v = 0;

        if (h != 0 || v != 0)
        {
            // �л�״̬
            gameCharacter.ChangeState(GameCharacterState.Move);
        }
    }
}
