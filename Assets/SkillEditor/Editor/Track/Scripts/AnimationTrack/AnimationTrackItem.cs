using UnityEngine;
using UnityEngine.UIElements;

public class AnimationTrackItem : TrackItemBase<AnimationTrack>
{
    private SkillAnimationEvent animationEvent;

    public SkillAnimationEvent AnimationEvent { get { return animationEvent; } }

    private SkillAnimationTrackItemStyle trackItemStyle;

    public void Init(AnimationTrack animationTrack, SkillTrackStyleBase parentTrackStyle, int startframeIndex, float frameUnitWidth, SkillAnimationEvent animationEvent)
    {
        track = animationTrack;
        this.frameIndex = startframeIndex;
        this.animationEvent = animationEvent;

        trackItemStyle = new SkillAnimationTrackItemStyle();
        itemStyle = trackItemStyle;
        trackItemStyle.Init(parentTrackStyle, startframeIndex, frameUnitWidth);

        normalColor = new Color(0.388f, 0.850f, 0.905f, 0.5f);
        selectColor = new Color(0.388f, 0.850f, 0.905f, 1f);

        OnUnSelect();

        // 绑定事件
        trackItemStyle.mainDragArea.RegisterCallback<MouseDownEvent>(MouseDown);
        trackItemStyle.mainDragArea.RegisterCallback<MouseUpEvent>(MouseUp);
        trackItemStyle.mainDragArea.RegisterCallback<MouseOutEvent>(MouseOut);
        trackItemStyle.mainDragArea.RegisterCallback<MouseMoveEvent>(MouseMove);

        ResetView(frameUnitWidth);
    }

    /// <summary>
    /// 根据起始位置和帧宽，刷新一下位置
    /// </summary>
    /// <param name="frameUnitWidth"></param>
    public override void ResetView(float frameUnitWidth)
    {
        this.frameUnitWidth = frameUnitWidth;
        trackItemStyle.SetTitle(animationEvent.AnimationClip.name);
        // 位置计算
        trackItemStyle.SetWidth(animationEvent.DurationFrame * frameUnitWidth);
        trackItemStyle.SetPosition(frameIndex * frameUnitWidth);

        int animationClipFrameCount = (int)(animationEvent.AnimationClip.frameRate * animationEvent.AnimationClip.length);
        // 计算动画结束线的位置
        if (animationClipFrameCount > animationEvent.DurationFrame)
        {
            trackItemStyle.animationOverLine.style.display = DisplayStyle.None;
        }
        else
        {
            trackItemStyle.animationOverLine.style.display = DisplayStyle.Flex;
            Vector3 overLinePos = trackItemStyle.animationOverLine.transform.position;
            overLinePos.x = animationClipFrameCount * frameUnitWidth - 1; // 线条自身宽度为2
            trackItemStyle.animationOverLine.transform.position = overLinePos;
        }
        track.TickView(SkillEditorWindow.Instance.CurrentSelectFrameIndex);
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
            int offsetFrame = Mathf.RoundToInt(offsetPos/frameUnitWidth);
            int targetFrameIndex = startDragFrameIndex + offsetFrame;
            if (targetFrameIndex < 0) return;
            bool checkDrag = false;
            if (offsetFrame < 0)
                checkDrag = track.CheckFrameIndexOnDrag(targetFrameIndex, startDragFrameIndex, true);
            else if (offsetFrame >= 0)
                checkDrag = track.CheckFrameIndexOnDrag(targetFrameIndex + animationEvent.DurationFrame, startDragFrameIndex, false);
            else
                return;

            if (checkDrag)
            {
                // 确定修改的数据
                // 刷新视图
                frameIndex = targetFrameIndex;
                // 若超过右侧边界，拓展边界
                CheckFrameCount();
                ResetView(frameUnitWidth);
            }
        }
    }

    public void CheckFrameCount()
    {
        if (frameIndex + animationEvent.DurationFrame > SkillEditorWindow.Instance.CurrentFrameCount)
        {
            SkillEditorWindow.Instance.CurrentFrameCount = frameIndex + animationEvent.DurationFrame;
        }
    }

    private void ApplyDrag()
    {
        if (startDragFrameIndex != frameIndex)
        {
            track.SetFrameIndex(startDragFrameIndex, frameIndex);
            SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        }
    }
    #endregion

    public override void OnConfigChanged()
    {
        animationEvent = track.AnimationData.FrameData[frameIndex];
    }

}
