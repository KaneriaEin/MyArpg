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

        #region 四种闪避方向判定
        // 这里关于方向的判定，if顺序不能改。左右优先级最高 -> 只按后是back -> 默认是向前forward
        // TODO: 现在做不到四种闪避动作，因为角色无法固定面朝一个方向前后左右移动，现在先只做一种闪避动作
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
        #region 完美闪避判定
        if (customEvent.EventType == SkillEventType.InvincibleOn)
        {
            Collider[] colliders = new Collider[10];
            int hitCount = Physics.OverlapSphereNonAlloc(character.transform.position + new Vector3(0,1f,0), 2f, colliders, LayerMask.GetMask("Weapon"));

            if (hitCount == 0)
                return;
            else
            {
                // 触发完美闪避特写
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
