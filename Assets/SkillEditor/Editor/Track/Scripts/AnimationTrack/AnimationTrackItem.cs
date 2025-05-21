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

        // ���¼�
        trackItemStyle.mainDragArea.RegisterCallback<MouseDownEvent>(MouseDown);
        trackItemStyle.mainDragArea.RegisterCallback<MouseUpEvent>(MouseUp);
        trackItemStyle.mainDragArea.RegisterCallback<MouseOutEvent>(MouseOut);
        trackItemStyle.mainDragArea.RegisterCallback<MouseMoveEvent>(MouseMove);

        ResetView(frameUnitWidth);
    }

    /// <summary>
    /// ������ʼλ�ú�֡��ˢ��һ��λ��
    /// </summary>
    /// <param name="frameUnitWidth"></param>
    public override void ResetView(float frameUnitWidth)
    {
        this.frameUnitWidth = frameUnitWidth;
        trackItemStyle.SetTitle(animationEvent.AnimationClip.name);
        // λ�ü���
        trackItemStyle.SetWidth(animationEvent.DurationFrame * frameUnitWidth);
        trackItemStyle.SetPosition(frameIndex * frameUnitWidth);

        int animationClipFrameCount = (int)(animationEvent.AnimationClip.frameRate * animationEvent.AnimationClip.length);
        // ���㶯�������ߵ�λ��
        if (animationClipFrameCount > animationEvent.DurationFrame)
        {
            trackItemStyle.animationOverLine.style.display = DisplayStyle.None;
        }
        else
        {
            trackItemStyle.animationOverLine.style.display = DisplayStyle.Flex;
            Vector3 overLinePos = trackItemStyle.animationOverLine.transform.position;
            overLinePos.x = animationClipFrameCount * frameUnitWidth - 1; // ����������Ϊ2
            trackItemStyle.animationOverLine.transform.position = overLinePos;
        }
        track.TickView(SkillEditorWindow.Instance.CurrentSelectFrameIndex);
    }

    #region ��꽻��
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
                // ȷ���޸ĵ�����
                // ˢ����ͼ
                frameIndex = targetFrameIndex;
                // �������Ҳ�߽磬��չ�߽�
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
