using JKFrame;
using System.Collections;
using UnityEngine;

public class WhiteManDodgeBehaviour : GameCharacter_SkillBehaviourBase
{
    public override SkillBehaviourBase DeepCopy()
    {
        return new WhiteManDodgeBehaviour()
        {
        };
    }

    public override void Release()
    {
        base.Release();
        ((WhiteManSkillBrain)skillBrain).ClearNextSkillClipKey();

        #region �������ܷ����ж�
        // ������ڷ�����ж���if˳���ܸġ��������ȼ���� -> ֻ������back -> Ĭ������ǰforward
        // TODO: �����������������ܶ�������Ϊ��ɫ�޷��̶��泯һ������ǰ�������ƶ���������ֻ��һ�����ܶ���
        //Vector2 move = character.CommandController.GetMoveInput();
        //int clipIdx = 2;
        int clipIdx = 1;
        //if (move.y < 0)
        //{
        //    clipIdx = 3;
        //}
        //if (move.x > 0)
        //{
        //    clipIdx = 1;
        //}
        //if (move.x < 0)
        //{
        //    clipIdx = 0;
        //}
        #endregion

        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[clipIdx]);
    }

    public override void AfterSkillCustomEvent(SkillCustomEvent customEvent)
    {
        base.AfterSkillCustomEvent(customEvent);
        #region ���������ж�
        if (customEvent.EventType == SkillEventType.InvincibleOn)
        {
            Collider[] colliders = new Collider[10];
            int hitCount = Physics.OverlapSphereNonAlloc(character.transform.position + new Vector3(0,1f,0), 2f, colliders, LayerMask.GetMask("Weapon"));

            if (hitCount == 0)
                return;
            else
            {
                // ��������������д
                OnPerfectDodge(customEvent);

                for (int c = 0; c < hitCount; c++)
                {
                    Debug.Log(colliders[c].name);
                }
            }
        }
        #endregion
    }

    private void OnPerfectDodge(SkillCustomEvent customEvent)
    {
        // ����Э�̣����� ʱ����� �� ��ͷ��Ч��0.5s�����û���
        MonoSystem.Start_Coroutine(SetTimeScale(0.2f, 0.5f));
        // ��������������Ч
        AudioSystem.PlayOneShot((AudioClip)customEvent.ObjectArg, character.transform.position);
    }

    private IEnumerator SetTimeScale(float timeScale, float realityTime)
    {
        Time.timeScale = timeScale;
        PostProcessingManager.Instance.SetPerfectDodgeEffect();
        yield return CoroutineTool.WaitForSecondsRealtime(realityTime);
        PostProcessingManager.Instance.RemovePerfectDodgeEffect();
        Time.timeScale = 1;
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
    }
}
