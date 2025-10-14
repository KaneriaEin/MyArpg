using System.Text;
using UnityEngine;

public class NodachiMan_DamagedState : GameCharacterStateBase
{
    public GameCharacter_Posture curPosture = GameCharacter_Posture.Stand;
    private float layTime = 0;
    private int repelStrength;
    private Vector3 repelPos;
    private float repelSpeed;
    public override void Enter()
    {
        animation.AddAnimationEvent("OnDamageFinish", OnDamageFinish);
        animation.AddAnimationEvent("IntoLayDown", IntoLayDown);
        animation.AddAnimationEvent("IntoLayDownBack", IntoLayDownBack);
        animation.AddAnimationEvent("UpdateLayTime", UpdateLayTime);
        gameCharacter.DamageController.AddHitAction(DamageBeHitAction);
        gameCharacter.DamageController.AddHitActionFromAttackData(DamageBeHitFromAttackDataAction);
        gameCharacter.Enemy_Controller.inRPC = false;
        repelStrength = 0;
        repelPos = Vector3.zero;
        repelSpeed = 0;
    }

    public override void Exit()
    {
        base.Exit();
        curPosture = GameCharacter_Posture.Stand;
        animation.RemoveAnimationEvent("OnDamageFinish", OnDamageFinish);
        animation.RemoveAnimationEvent("IntoLayDown", IntoLayDown);
        animation.RemoveAnimationEvent("IntoLayDownBack", IntoLayDownBack);
        animation.RemoveAnimationEvent("UpdateLayTime", UpdateLayTime);
        gameCharacter.DamageController.RemoveHitAction(DamageBeHitAction);
        gameCharacter.DamageController.RemoveHitActionFromAttackData(DamageBeHitFromAttackDataAction);
        repelStrength = 0;
        repelPos = Vector3.zero;
        repelSpeed = 0;
    }

    public override void Update()
    {
        base.Update();
        if(curPosture == GameCharacter_Posture.LayDown || curPosture == GameCharacter_Posture.LayDownBack)
        {
            if(layTime == 0)
            {
                if (curPosture == GameCharacter_Posture.LayDown)
                {
                    gameCharacter.PlayAnimation("Damage_Rolling_StandUp", null, 1, true, 0f);
                }
                else
                {
                    gameCharacter.PlayAnimation("Damage_Rolling_StandUp_Back", null, 1, true, 0f);
                }
                curPosture = GameCharacter_Posture.Stand;
            }
            layTime = Mathf.Clamp(layTime - Time.deltaTime, 0, layTime - Time.deltaTime);
        }
    }

    private void OnDamageFinish()
    {
        gameCharacter.ChangeToIdleState();
    }

    private void IntoLayDown()
    {
        gameCharacter.PlayAnimation("Damage_LayDown", null, 1, true, 0.1f);
        UpdateLayTime();
        curPosture = GameCharacter_Posture.LayDown;
    }

    private void IntoLayDownBack()
    {
        gameCharacter.PlayAnimation("Damage_LayDown_Back", null, 1, true, 0.1f);
        UpdateLayTime();
        curPosture = GameCharacter_Posture.LayDownBack;
    }

    private void UpdateLayTime()
    {
        layTime = Random.Range(1.5f, 2f);
    }

    /// <summary>
    /// 正常战斗中产生的攻击伤害数据处理
    /// </summary>
    public void DamageBeHitAction(AttackData atkData)
    {
        // 播放受击动画
        // 先读当前所受攻击AttackData，再决定播放哪个动画
        // 顿不顿帧由atkEvent里的freeze参数决定
        StringBuilder animkey = new StringBuilder();
        animkey.Append("Damage");
        repelStrength = 0;
        repelPos = Vector3.zero;
        repelSpeed = 0;
        switch (curPosture)
        {
            case GameCharacter_Posture.Stand:
                animkey.Append("_Stand");
                switch (atkData.detectionEvent.AttackHitConfig.RepelStrength / 10)
                {
                    case 0:
                        animkey.Append("_InSitu");
                        if (CheckAttackDirectionBack(gameCharacter.ModelTransform.position, gameCharacter.ModelTransform.forward, atkData.hitPoint))
                        {
                            animkey.Append("_Back");
                        }
                        break;
                    case 1:
                        animkey.Append("_Stagger");
                        break;
                    case 2:
                        animkey.Append("_Kneel");
                        break;
                    case 3:
                        animkey.Append("_Repel");
                        break;
                    default:
                        break;
                }
                break;
            case GameCharacter_Posture.LayDown:
                animkey.Append("_Lay");
                switch (atkData.detectionEvent.AttackHitConfig.RepelStrength / 10)
                {
                    case 3:
                        animkey.Append("_Repel");
                        break;
                    default:
                        animkey.Append("_InSitu");
                        break;
                }
                break;
            case GameCharacter_Posture.LayDownBack:
                animkey.Append("_Lay");
                switch (atkData.detectionEvent.AttackHitConfig.RepelStrength / 10)
                {
                    case 3:
                        animkey.Append("_Repel");
                        break;
                    default:
                        animkey.Append("_InSitu_Back");
                        break;
                }
                break;
        }
        if (atkData.detectionEvent.AttackHitConfig.Freeze)
        {
            animkey.Append("_Imme");
        }
        if (!animkey.ToString().Contains("InSitu"))
        {
            // 特殊受击动作需要面朝 hitPoint
            gameCharacter.ModelTransform.LookAt(new Vector3(atkData.hitPoint.x, gameCharacter.ModelTransform.position.y, atkData.hitPoint.z));
            // 非原地受击动画需要通过根运动调整击飞距离
            repelSpeed = (atkData.detectionEvent.AttackHitConfig.RepelStrength % 10 / 10f) + 1;
        }
        else
        {
            // 原地受击动画需要调整击飞位移
            repelStrength = atkData.detectionEvent.AttackHitConfig.RepelStrength % 10;
        }
        #region 计算击飞值
        if (repelStrength != 0)
        {
            // 计算击飞方向
            Vector3 repelDir = (gameCharacter.transform.position - atkData.hitPoint).normalized;
            // 计算击飞距离，之后在rootMotion中处理击飞位移
            repelPos = gameCharacter.transform.position + repelDir * repelStrength;
        }
        Debug.Log($"此时repelSpeed = {repelSpeed},repelPos = {repelPos}");
        gameCharacter.PlayAnimation(animkey.ToString(), OnRootMotion, 1, true, 0);
        #endregion
    }

    /// <summary>
    /// 非战斗中发生，而是特殊时间导致伤害行为AttackData产生，用这个接口处理
    /// </summary>
    public void DamageBeHitFromAttackDataAction(AttackData atkData)
    {
        switch (atkData.attackType)
        {
            case SkillType.PerfectGuard:
                Debug.Log($"我被完美防御了，需要做出反应");
                break;
        }
    }

    /// <summary>
    /// 计算攻击位置是否在角色的后方
    /// </summary>
    /// <param name="characterPos"></param>
    /// <param name="characterForward"></param>
    /// <param name="hitPos"></param>
    /// <returns></returns>
    private bool CheckAttackDirectionBack(Vector3 characterPos, Vector3 characterForward, Vector3 hitPos)
    {
        // 计算从角色指向攻击点的向量
        Vector3 attackDirection = hitPos - characterPos;
        attackDirection.Normalize(); // 标准化为方向向量

        // 计算点积
        float dotProduct = Vector3.Dot(characterForward, attackDirection);

        // 判断前后关系
        if (dotProduct > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        if(repelPos != Vector3.zero)
        {
            deltaPosition = (repelPos - gameCharacter.transform.position).normalized * Time.deltaTime;
        }
        else
        {
            deltaPosition = deltaPosition * repelSpeed;
        }
        deltaPosition.y = -9.8f * Time.deltaTime;
        gameCharacter.CharacterController.Move(deltaPosition);
    }

}
