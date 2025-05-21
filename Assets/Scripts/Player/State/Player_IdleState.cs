using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 玩家待机状态
/// </summary>
public class Player_IdleState : PlayerStateBase
{
    public override void Enter()
    {
        player.PlayAnimation("Idle");
    }

    public override void Update()
    {
        if (CheckAndEnterSkillState()) return;
        player.CharacterController.Move(new Vector3(0, -9.8f * Time.deltaTime, 0));
        // 检测玩家的输入
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if(h!=0 || v != 0)
        {
            // 切换状态
            player.ChangeState(PlayerState.Move);
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    player.ChangeState(PlayerState.Skill);
        //}
    }
}
