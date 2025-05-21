using System;
using UnityEngine;
using UnityEngine.UIElements;

public class EventTrackItem : TrackItemBase<EventTrack>
{
    private SkillCustomEvent customEvent;

    public SkillCustomEvent CustomEvent { get { return customEvent; } }

    private SkillCustomEventItemStyle trackItemStyle;
    public static EventTrackItem currentSelectItem;

    public void Init(EventTrack eventTrack, SkillTrackStyleBase parentTrackStyle, int startframeIndex, float frameUnitWidth, SkillCustomEvent customEvent)
    {
        track = eventTrack;
        this.frameIndex = startframeIndex;
        this.customEvent = customEvent;
        this.frameUnitWidth = frameUnitWidth;

        trackItemStyle = new SkillCustomEventItemStyle();
        itemStyle = trackItemStyle;
        trackItemStyle.Init(parentTrackStyle);

        normalColor = new Color(0.388f, 0.850f, 0.905f, 0.5f);
        selectColor = new Color(0.388f, 0.850f, 0.905f, 1f);

        OnUnSelect();
        trackItemStyle.root.RegisterCallback<MouseDownEvent>(MouseDown);
        ResetView(frameUnitWidth);
    }

    private void MouseDown(MouseDownEvent evt)
    {
        if (currentSelectItem == this) OnUnSelect();
        else Select();
    }

    public override void OnSelect()
    {
        base.OnSelect();
        currentSelectItem = this;
    }

    public override void OnUnSelect()
    {
        base.OnUnSelect();
        currentSelectItem = null;
    }

    public override void ResetView(float frameUnitWidth)
    {
        this.frameUnitWidth = frameUnitWidth;
        // Œª÷√º∆À„
        trackItemStyle.SetPosition(frameIndex * frameUnitWidth - frameUnitWidth / 2);
        trackItemStyle.SetWidth(frameUnitWidth);
    }

    public void ChangeFrameIndex(int newIndex)
    {
        track.SetFrameIndex(frameIndex ,newIndex);
        frameIndex = newIndex;
        SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        ResetView();
    }
}
