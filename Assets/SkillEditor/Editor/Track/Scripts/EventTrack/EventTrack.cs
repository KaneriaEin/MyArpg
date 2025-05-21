using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class EventTrack : SkillTrackBase
{
    private SkillSingleLineTrackStyle trackStyle;
    // <ÿ��item��ʼ֡��item>
    private Dictionary<int, EventTrackItem> trackItemDic = new Dictionary<int, EventTrackItem>();
    public SkillCustomEventData CustomEventData { get => SkillEditorWindow.Instance.SkillConfig.SkillCustomEventData; }

    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        base.Init(menuParent, trackParent, frameWidth);
        trackStyle = new SkillSingleLineTrackStyle();
        trackStyle.Init(menuParent, trackParent, "�¼�����");
        trackStyle.contentRoot.RegisterCallback<MouseDownEvent>(TrackContentRootMouseDown);
        ResetView();
    }

    private void TrackContentRootMouseDown(MouseDownEvent evt)
    {
        int frameIndex = SkillEditorWindow.Instance.GetFrameIndexByMousePos(evt.localMousePosition.x);
        if (CustomEventData.FrameData.ContainsKey(frameIndex)) return;
        if(EventTrackItem.currentSelectItem != null)
        {
            // �任λ��
            EventTrackItem.currentSelectItem.ChangeFrameIndex(frameIndex);
        }
        else
        {
            // ��ӹ��
            SkillCustomEvent customEvent = new SkillCustomEvent();

            CustomEventData.FrameData.Add(frameIndex, customEvent);
            SkillEditorWindow.Instance.SaveConfig();
            CreateItem(frameIndex, customEvent);
        }
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);


        // ���ٵ�ǰ����
        foreach (var item in trackItemDic)
        {
            trackStyle.DeleteItem(item.Value.itemStyle.root);
        }
        trackItemDic.Clear();

        if (SkillEditorWindow.Instance.SkillConfig == null) return;

        // �������ݻ���TrackItem
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
    /// ��ԭ��oldIndex�����ݱ�ΪnewINdex
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
