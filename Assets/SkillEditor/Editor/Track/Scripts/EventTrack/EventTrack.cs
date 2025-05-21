using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class EventTrack : SkillTrackBase
{
    private SkillSingleLineTrackStyle trackStyle;
    // <每个item起始帧，item>
    private Dictionary<int, EventTrackItem> trackItemDic = new Dictionary<int, EventTrackItem>();
    public SkillCustomEventData CustomEventData { get => SkillEditorWindow.Instance.SkillConfig.SkillCustomEventData; }

    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        base.Init(menuParent, trackParent, frameWidth);
        trackStyle = new SkillSingleLineTrackStyle();
        trackStyle.Init(menuParent, trackParent, "事件配置");
        trackStyle.contentRoot.RegisterCallback<MouseDownEvent>(TrackContentRootMouseDown);
        ResetView();
    }

    private void TrackContentRootMouseDown(MouseDownEvent evt)
    {
        int frameIndex = SkillEditorWindow.Instance.GetFrameIndexByMousePos(evt.localMousePosition.x);
        if (CustomEventData.FrameData.ContainsKey(frameIndex)) return;
        if(EventTrackItem.currentSelectItem != null)
        {
            // 变换位置
            EventTrackItem.currentSelectItem.ChangeFrameIndex(frameIndex);
        }
        else
        {
            // 添加轨道
            SkillCustomEvent customEvent = new SkillCustomEvent();

            CustomEventData.FrameData.Add(frameIndex, customEvent);
            SkillEditorWindow.Instance.SaveConfig();
            CreateItem(frameIndex, customEvent);
        }
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);


        // 销毁当前已有
        foreach (var item in trackItemDic)
        {
            trackStyle.DeleteItem(item.Value.itemStyle.root);
        }
        trackItemDic.Clear();

        if (SkillEditorWindow.Instance.SkillConfig == null) return;

        // 根据数据绘制TrackItem
        foreach (var item in CustomEventData.FrameData)
        {
            CreateItem(item.Key, item.Value);
        }
    }

    private void CreateItem(int frameIndex, SkillCustomEvent skillCustomEvent)
    {
        EventTrackItem trackItem = new EventTrackItem();
        trackItem.Init(this, trackStyle, frameIndex, frameWidth, skillCustomEvent);
        trackItemDic.Add(frameIndex, trackItem);
    }

    /// <summary>
    /// 将原本oldIndex的数据变为newINdex
    /// </summary>
    /// <param name="oldIndex"></param>
    /// <param name="newIndex"></param>
    public void SetFrameIndex(int oldIndex, int newIndex)
    {
        if (CustomEventData.FrameData.Remove(oldIndex, out SkillCustomEvent customEvent))
        {
            CustomEventData.FrameData.Add(newIndex, customEvent);
            trackItemDic.Remove(oldIndex, out EventTrackItem trackItem);
            trackItemDic.Add(newIndex, trackItem);
            SkillEditorWindow.Instance.SaveConfig();
        }
    }

    public override void DeleteTrackItem(int frameIndex)
    {
        CustomEventData.FrameData.Remove(frameIndex);
        if (trackItemDic.Remove(frameIndex, out EventTrackItem item))
        {
            trackStyle.DeleteItem(item.itemStyle.root);
        }
        SkillEditorWindow.Instance.SaveConfig();
    }

    public override void Destroy()
    {
        trackStyle.Destroy();
    }
}
