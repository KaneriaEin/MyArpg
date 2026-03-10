using JKFrame;
using System.Collections;
using UnityEngine;

public class WhiteManDodgeBehaviour : GameCharacter_SkillBehaviourBase
{
    private bool perfectDodge = false;
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

        #region 新完美闪避判定
        perfectDodge = false;
        if (BattleEventManager.Instance.CheckPerfectDodge(0.5f))
        {
            // 触发完美闪避效果
            OnPerfectDodge();
            perfectDodge = true;
        }
        #endregion
        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[0]);
    }

    public override void AfterSkillCustomEvent(SkillCustomEvent customEvent)
    {
        base.AfterSkillCustomEvent(customEvent);
        if (perfectDodge)
        {
            Warp();
        }
    }

    private void OnPerfectDodge()
    {
        // 回复MP
        PlayerManager.Instance.Player.PropertyAddMP(50);
        // 利用协程，设置 时间变慢 和 镜头特效，0.5s后，设置回来
        MonoSystem.Start_Coroutine(SetTimeScale(0.2f, 0.5f));
        // 播放完美闪避音效
        AudioSystem.PlayOneShot(character.CharacterConfig.DodgeAudioClips[0], character.transform.position);
    }

    private IEnumerator SetTimeScale(float timeScale, float realityTime)
    {
        BattleEventManager.Instance.BattleBulletTimeEvent(0.5f, 0.2f);
        PostProcessingManager.Instance.SetPerfectDodgeEffect();
        yield return CoroutineTool.WaitForSecondsRealtime(realityTime);
        PostProcessingManager.Instance.RemovePerfectDodgeEffect();
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

    public override void OnTickSkill(int frameIndex)
    {
        base.OnTickSkill(frameIndex);

        #region 闪避动作衔接人物移动的处理
        if (skillBrain.CheckCanRelesse())
        {
            // 检测玩家的输入
            Vector2 cmdInput = character.CommandController.GetMoveInput();
            float h = cmdInput.x;
            float v = cmdInput.y;

            if (h != 0 || v != 0)
            {
                OnClipEndOrReleaseNewSkill();
                // 切换状态
                character.ChangeState(GameCharacterState.Move);
            }
        }
        #endregion
    }

    public void Warp()
    {
        GameObject clone = GameObject.Instantiate(character.ModelTransform.gameObject, character.ModelTransform.position, character.ModelTransform.rotation);
        GameObject.Destroy(clone.GetComponent<Animation_Controller>());
        GameObject.Destroy(clone.GetComponent<Animator>());
        GameObject.Destroy(clone.GetComponent<GameCharacter_View>());

        WeaponController[] wp = clone.GetComponentsInChildren<WeaponController>();
        foreach (WeaponController wpc in wp)
        {
            GameObject.Destroy(wpc);
        }
        BoxCollider[] boxcol = clone.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider box in boxcol)
        {
            GameObject.Destroy(box);
        }
        Rigidbody[] rgdb = clone.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rgd in rgdb)
        {
            GameObject.Destroy(rgd);
        }

        SkinnedMeshRenderer[] skinMeshList = clone.GetComponentsInChildren<SkinnedMeshRenderer>();
        Material _mat = new Material(character.CharacterConfig.glowMaterial);

        foreach (SkinnedMeshRenderer smr in skinMeshList)
        {
            smr.material = _mat;
            smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            smr.receiveShadows = false;
        }

        MeshRenderer[] meshList = clone.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in meshList)
        {
            mr.material = _mat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
        }

        MonoSystem.Start_Coroutine(WarpDisappear(clone, _mat, 0.6f));
    }

    private IEnumerator WarpDisappear(GameObject clone, Material mat, float disappentTime)
    {
        Color currentColor = mat.color;
        float alpha = currentColor.a;
        float oria = currentColor.a;
        for (float t = 0; t < disappentTime; t += Time.deltaTime)
        {
            alpha = Mathf.Lerp(oria, 0f, t/disappentTime);
            mat.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        GameObject.Destroy(clone);
    }
}
