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

        #region 根据玩家输入调整方向
        // 检测玩家的输入
        Vector2 cmdInput = character.CommandController.GetMoveInput();
        if(cmdInput != Vector2.zero)
        {
            float h = cmdInput.x;
            float v = cmdInput.y;
            Vector3 input = new Vector3(h, 0, v);
            character.Rotate(input, 1000f);
        }
        #endregion

        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[0]);
    }

    public override void AfterSkillCustomEvent(SkillCustomEvent customEvent)
    {
        base.AfterSkillCustomEvent(customEvent);
        #region 完美闪避判定
        if (customEvent.EventType == SkillEventType.InvincibleOn)
        {
            Collider[] colliders = new Collider[10];
            int hitCount = Physics.OverlapSphereNonAlloc(character.transform.position + new Vector3(0,1f,0), 2f, colliders, LayerMask.GetMask("Weapon"));

            if (hitCount == 0)
                return;
            else
            {
                // 触发完美闪避效果
                OnPerfectDodge(customEvent);

                for (int c = 0; c < hitCount; c++)
                {
                    Debug.Log($"触发完美闪避，闪避了：{colliders[c].name}。");
                }
            }
        }
        #endregion
    }

    private void OnPerfectDodge(SkillCustomEvent customEvent)
    {
        // 回复MP
        PlayerManager.Instance.Player.PropertyAddMP(50);
        // 利用协程，设置 时间变慢 和 镜头特效，0.5s后，设置回来
        MonoSystem.Start_Coroutine(SetTimeScale(0.2f, 0.5f));
        // 播放完美闪避音效
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
        deltaPosition = deltaPosition * 0.7f;
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
