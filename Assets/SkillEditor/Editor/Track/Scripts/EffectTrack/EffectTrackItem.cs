using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectTrackItem : TrackItemBase<EffectTrack>
{
    private SkillMultiLineTrackStyle.ChildTrack childTrackStyle; // 将片段放入的子轨道
    private SkillEffectTrackItemStyle trackItemStyle;   // 片段
    private SkillEffectEvent skillEffectEvent;
    public SkillEffectEvent SkillEffectEvent { get { return skillEffectEvent; } }

    public void Init(EffectTrack effectTrack, float frameUnitWidth, SkillEffectEvent skillEffectEvent, SkillMultiLineTrackStyle.ChildTrack childTrack)
    {
        this.track = effectTrack;
        this.frameIndex = skillEffectEvent.FrameIndex;
        this.childTrackStyle = childTrack;
        this.skillEffectEvent = skillEffectEvent;
        trackItemStyle = new SkillEffectTrackItemStyle();
        itemStyle = trackItemStyle;

        normalColor = new Color(0.388f, 0.850f, 0.905f, 0.5f);
        selectColor = new Color(0.388f, 0.850f, 0.905f, 1f);

        childTrack.trackRoot.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        childTrack.trackRoot.RegisterCallback<DragExitedEvent>(OnDragExited);
        ResetView(frameUnitWidth);
    }

    /// <summary>
    /// 根据起始位置和帧宽，刷新一下位置
    /// </summary>
    public override void ResetView(float frameUnitWidth)
    {
        base.ResetView(frameUnitWidth);
        if (skillEffectEvent.Prefab != null)
        {
            if (!trackItemStyle.isInit)
            {
                trackItemStyle.Init(frameUnitWidth, skillEffectEvent, childTrackStyle);
                // 绑定事件
                trackItemStyle.mainDragArea.RegisterCallback<MouseDownEvent>(MouseDown);
                trackItemStyle.mainDragArea.RegisterCallback<MouseUpEvent>(MouseUp);
                trackItemStyle.mainDragArea.RegisterCallback<MouseOutEvent>(MouseOut);
                trackItemStyle.mainDragArea.RegisterCallback<MouseMoveEvent>(MouseMove);
            }
        }
        trackItemStyle.ResetView(frameUnitWidth, skillEffectEvent);

        // 强行重新生成预览
        CleanEffectPreviewObj();
        TickView(SkillEditorWindow.Instance.CurrentSelectFrameIndex);
    }

    /// <summary>
    /// 删除显示
    /// </summary>
    public void Destroy()
    {
        CleanEffectPreviewObj();
        childTrackStyle.Destroy();
    }

    public void CleanEffectPreviewObj()
    {
        if (effectPreviewObj != null)
        {
            GameObject.DestroyImmediate(effectPreviewObj);
            effectPreviewObj = null;
        }
    }

    public void SetTrackName(string trackName)
    {
        childTrackStyle.SetTrackName(trackName);
    }

    #region 鼠标交互
    private bool mouseDrag = false;
    private float startDragPosX;
    private int startDragFrameIndex;
    private void MouseDown(MouseDownEvent evt)
    {
        startDragPosX = evt.mousePosition.x;
        startDragFrameIndex = frameIndex;
        mouseDrag = true;
        Select();
    }

    private void MouseUp(MouseUpEvent evt)
    {
        if (mouseDrag)
            ApplyDrag();
        mouseDrag = false;
    }

    private void MouseOut(MouseOutEvent evt)
    {
        if (mouseDrag)
            ApplyDrag();
        mouseDrag = false;
    }

    private void MouseMove(MouseMoveEvent evt)
    {
        if (mouseDrag)
        {
            float offsetPos = evt.mousePosition.x - startDragPosX;
            int offsetFrame = Mathf.RoundToInt(offsetPos / frameUnitWidth);
            int targetFrameIndex = startDragFrameIndex + offsetFrame;

            if (targetFrameIndex < 0) return;
            //if (targetFrameIndex < 0 || offsetFrame == 0) return;

            // 确定修改的数据
            // 刷新视图
            frameIndex = targetFrameIndex;
            skillEffectEvent.FrameIndex = frameIndex;
            // 若超过右侧边界，拓展边界
            //CheckFrameCount();
            ResetView(frameUnitWidth);
        }
    }

    /// <summary>
    /// TODO:暂时不用
    /// </summary>
    public void CheckFrameCount()
    {
        //int frameCount = (int)(skillEffectEvent.AudioClip.length * SkillEditorWindow.Instance.SkillClip.FrameRate);
        //if (frameIndex + frameCount > SkillEditorWindow.Instance.CurrentFrameCount)
        //{
        //    SkillEditorWindow.Instance.CurrentFrameCount = frameIndex + frameCount;
        //}
    }

    /// <summary>
    /// TODO:暂时不用
    /// </summary>
    private void ApplyDrag()
    {
        if (startDragFrameIndex != frameIndex)
        {
            skillEffectEvent.FrameIndex = frameIndex;
            SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        }
    }
    #endregion

    #region 拖拽资源
    private void OnDragUpdate(DragUpdatedEvent evt)
    {
        // 监听用户拖拽的是否是动画
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        GameObject prefab = objs[0] as GameObject;
        if (prefab != null)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }
    }

    private void OnDragExited(DragExitedEvent evt)
    {
        // 监听用户拖拽的是否是特效
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        GameObject prefab = objs[0] as GameObject;
        if (prefab != null)
        {
            // 放置特效资源
            int selectFrameIndex = SkillEditorWindow.Instance.GetFrameIndexByPos(evt.localMousePosition.x);
            if (selectFrameIndex >= 0)
            {
                skillEffectEvent.FrameIndex = selectFrameIndex;
                skillEffectEvent.Prefab = prefab;
                skillEffectEvent.Position = Vector3.zero;
                skillEffectEvent.Rotation = Vector3.zero;
                skillEffectEvent.Scale = Vector3.one;
                skillEffectEvent.AutoDestruct = true;

                ParticleSystem[] particleSystems = prefab.GetComponentsInChildren<ParticleSystem>();
                float max = -1;
                float curr = -1;
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    if (particleSystems[i].main.duration > max)
                    {
                        max = particleSystems[i].main.duration;
                        curr = i;
                    }
                }

                skillEffectEvent.Duration = (int)(max * SkillEditorWindow.Instance.SkillConfig.FrameRate);

                this.frameIndex = selectFrameIndex;
                ResetView();
                SkillEditorWindow.Instance.SaveConfig();
            }
        }
    }
    #endregion

    #region 预览
    private GameObject effectPreviewObj;
    public void TickView(int frameIndex)
    {
        if (skillEffectEvent.Prefab == null || SkillEditorWindow.Instance.PreviewCharacterObj== null) return;
        // 是不是在播放范围内
        int durationFrame = skillEffectEvent.Duration;

        if (skillEffectEvent.FrameIndex <= frameIndex && skillEffectEvent.FrameIndex + durationFrame > frameIndex)
        {
            GameObject.DestroyImmediate(effectPreviewObj);
            Transform characterRoot = SkillEditorWindow.Instance.PreviewCharacterObj.transform;
            // 获取模拟坐标
            Vector3 rootPostion = SkillEditorWindow.Instance.GetPositionForRootMotion(skillEffectEvent.FrameIndex, true);
            Vector3 pos = characterRoot.TransformPoint(skillEffectEvent.Position);
            Vector3 rot = characterRoot.eulerAngles + skillEffectEvent.Rotation;

            // 实例化
            effectPreviewObj = GameObject.Instantiate(skillEffectEvent.Prefab, pos, Quaternion.Euler(rot), EffectTrack.EffectParent);
            effectPreviewObj.name = skillEffectEvent.Prefab.name;
            effectPreviewObj.transform.localScale = skillEffectEvent.Scale;

            ParticleSystem[] particleSystems = effectPreviewObj.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particleSystems.Length; i++)
            {
                int simulateFrame = frameIndex - skillEffectEvent.FrameIndex;
                particleSystems[i].Simulate((float)simulateFrame / SkillEditorWindow.Instance.SkillConfig.FrameRate);
            }
        }
        else
        {
            if (effectPreviewObj != null)
            {
                CleanEffectPreviewObj();
            }
        }
    }


    public void ApplyModelTransformData()
    {
        if(effectPreviewObj != null)
        {
            Transform characterRoot = SkillEditorWindow.Instance.PreviewCharacterObj.transform;
            // 获取模拟坐标
            Vector3 rootPostion = SkillEditorWindow.Instance.GetPositionForRootMotion(skillEffectEvent.FrameIndex, true);
            Vector3 oldPos = characterRoot.position;

            // 把角色临时设置到播放坐标
            characterRoot.position = rootPostion;
            skillEffectEvent.Position = characterRoot.InverseTransformPoint(effectPreviewObj.transform.position);
            skillEffectEvent.Rotation = effectPreviewObj.transform.rotation.eulerAngles - characterRoot.eulerAngles;
            skillEffectEvent.Scale = effectPreviewObj.transform.localScale;
            // 还原坐标
            characterRoot.position = oldPos;
        }
    }

    #endregion
}
