using UnityEngine;

public class WarriorSkill1Behaviour : GameCharacter_SkillBehaviourBase
{
    #region ����
    public float standingTime = 5;
    #endregion
    private int attackIndex = -1; // -1����û�н��뼼�ܣ�0��1��2��������
    public override SkillBehaviourBase DeepCopy()
    {
        return new WarriorSkill1Behaviour();
    }

    public override void Release()
    {
        base.Release();
        attackIndex += 1;
        // �����������һ�����������cd
        if (attackIndex == skillConfig.Clips.Length - 1)
            cdTimer = cdTime;
        else
            cdTimer = standingTime;


        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
        // ���չ�����
        skillBrain.AddorUpdateShareData(WarriorSkillBrain.ContinuousStandAttackModelDataKey, true);
    }

    public override bool CheckRelease()
    {
        bool checkCD;
        if (attackIndex == -1) checkCD = cdTimer <= 0; // δ�ͷ�״̬
        else if (attackIndex == skillConfig.Clips.Length - 1) checkCD = cdTimer <= 0; // �ͷ������һ�ε�״̬
        else checkCD = true;

        return checkCD && base.CheckRelease();
    }

    public override void UpdateCDTime()
    {
        if (playing)
        {
            if (attackIndex == skillConfig.Clips.Length - 1)
            {
                cdTimer = Mathf.Clamp(cdTimer - Time.deltaTime, 0, float.MaxValue);
                // Debug.Log($"�������һ�β����У����ڼ���cd:{cdTimer}/{cdTime}");
            }
            else
            {
                // Debug.Log("����û�в������һ�Σ�������cd");
            }
            return;
        }
        cdTimer = Mathf.Clamp(cdTimer - Time.deltaTime, 0, float.MaxValue);
        // ���ܴ���ĳһ��
        if (attackIndex != -1)
        {
            if (cdTimer <= 0)
            {
                // ��ʱ����������cd
                cdTimer = cdTime;
                attackIndex = -1;
                // Debug.Log("����û��������ʱ������ȫ�ͷ���ϣ����Ѿ���ʼ����cd");
            }
            else
            {
                // Debug.Log($"���ܻ�û��ȫ�ͷ���ϣ������ڲ�cd:{cdTimer}/{standingTime}");
            }
        }
        else
        {
            // Debug.Log($"����û���ͷţ�����cd:{cdTimer}/{cdTime}");
        }
    }

    public override void OnAttackDetection(IHitTarget target, AttackData attackData)
    {
        base.OnAttackDetection(target, attackData);
        // Debug.Log(collider.gameObject.name);
    }
    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y -= 9.8f * Time.deltaTime;
        owner.OnSkillMove(deltaPosition);
        owner.OnSkillRotate(deltaRotation);

    }

    public override void OnSkillClipEnd()
    {
        base.OnSkillClipEnd();
        owner.ChangeToIdleState();
    }

    public override void OnClipEndOrReleaseNewSkill()
    {
        base.OnClipEndOrReleaseNewSkill();
        // ��ǰ�����������һ�Σ�˵������ȫ������
        if (attackIndex == skillConfig.Clips.Length - 1)
        {
            attackIndex = -1;
        }
        // ���������м�ĳһ�μ���
        else if (attackIndex != -1)
        {
            cdTimer = standingTime;
        }
    }
}
