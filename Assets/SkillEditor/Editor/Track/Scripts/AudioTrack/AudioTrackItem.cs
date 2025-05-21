using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioTrackItem : TrackItemBase<AudioTrack>
{
    private SkillMultiLineTrackStyle.ChildTrack childTrackStyle; // 将片段放入的子轨道
    private SkillAudioTrackItemStyle trackItemStyle;   // 片段
    private SkillAudioEvent skillAudioEvent;
    public SkillAudioEvent SkillAudioEvent { get { return skillAudioEvent; } }

    public void Init(AudioTrack audioTrack,float frameUnitWidth, SkillAudioEvent skillAudioEvent, SkillMultiLineTrackStyle.ChildTrack childTrack)
    {
        this.track = audioTrack;
        this.frameIndex = skillAudioEvent.FrameIndex;
        this.childTrackStyle = childTrack;
        this.skillAudioEvent = skillAudioEvent;
        trackItemStyle = new SkillAudioTrackItemStyle();
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
        if (skillAudioEvent.AudioClip != null)
        {
            if (!trackItemStyle.isInit)
            {
                trackItemStyle.Init(frameUnitWidth, skillAudioEvent, childTrackStyle);
                // 绑定事件
                trackItemStyle.mainDragArea.RegisterCallback<MouseDownEvent>(MouseDown);
                trackItemStyle.mainDragArea.RegisterCallback<MouseUpEvent>(MouseUp);
                trackItemStyle.mainDragArea.RegisterCallback<MouseOutEvent>(MouseOut);
                trackItemStyle.mainDragArea.RegisterCallback<MouseMoveEvent>(MouseMove);
            }
        }
        trackItemStyle.ResetView(frameUnitWidth, skillAudioEvent);
    }

    /// <summary>
    /// 删除显示
    /// </summary>
    public void Destroy()
    {
        childTrackStyle.Destroy();
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

            if (targetFrameIndex < 0 || offsetFrame == 0) return;

            // 确定修改的数据
            // 刷新视图
            frameIndex = targetFrameIndex;
            skillAudioEvent.FrameIndex = frameIndex;
            // 若超过右侧边界，拓展边界
            //CheckFrameCount();
            ResetView(frameUnitWidth);
        }
    }

    public void CheckFrameCount()
    {
        int frameCount = (int)(skillAudioEvent.AudioClip.length * SkillEditorWindow.Instance.SkillConfig.FrameRate);
        if (frameIndex + frameCount > SkillEditorWindow.Instance.CurrentFrameCount)
        {
            SkillEditorWindow.Instance.CurrentFrameCount = frameIndex + frameCount;
        }
    }

    private void ApplyDrag()
    {
        if (startDragFrameIndex != frameIndex)
        {
            //skillAudioEvent.FrameIndex = frameIndex;
            // SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        }
    }
    #endregion

    #region 拖拽资源
    private void OnDragUpdate(DragUpdatedEvent evt)
    {
        // 监听用户拖拽的是否是动画
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        AudioClip clip = objs[0] as AudioClip;
        if (clip != null)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }
    }

    private void OnDragExited(DragExitedEvent evt)
    {
        // 监听用户拖拽的是否是动画
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        AudioClip clip = objs[0] as AudioClip;
        if (clip != null)
        {
            // 放置动画资源
            int selectFrameIndex = SkillEditorWindow.Instance.GetFrameIndexByPos(evt.localMousePosition.x);
            if(selectFrameIndex >= 0)
            {
                skillAudioEvent.AudioClip = clip;
                skillAudioEvent.FrameIndex = selectFrameIndex;
                skillAudioEvent.Volume = 1;
                this.frameIndex = selectFrameIndex;
                ResetView();
                SkillEditorWindow.Instance.SaveConfig();
            }
        }
    }
    #endregion
}
