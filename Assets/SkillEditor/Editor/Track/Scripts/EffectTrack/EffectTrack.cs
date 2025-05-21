using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectTrack : SkillTrackBase
{
    private SkillMultiLineTrackStyle trackStyle;
    private List<EffectTrackItem> trackItemList = new List<EffectTrackItem>();
    public SkillEffectData EffectData { get => SkillEditorWindow.Instance.SkillConfig.SkillEffectData; }

    public static Transform EffectParent { get; private set; }

    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        base.Init(menuParent, trackParent, frameWidth);
        trackStyle = new SkillMultiLineTrackStyle();
        trackStyle. Init(menuParent, trackParent, "特效配置", AddChildTrack, CheckDeleteChildTrack, SwapChildTrack, UpdateChildTrackName);

        if (SkillEditorWindow.Instance.OnEditorScene)
        {
            EffectParent = GameObject.Find("Effects").transform;
            EffectParent.position = Vector3.zero;
            EffectParent.rotation = Quaternion.identity;
            EffectParent.localScale = Vector3.one;
            for (int i = EffectParent.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(EffectParent.GetChild(i).gameObject);
            }
        }
        ResetView();
    }

    private void CreateItem(SkillEffectEvent effectEvent)
    {
        EffectTrackItem item = new EffectTrackItem();
        item.Init(this,frameWidth, effectEvent, trackStyle.AddChildTrack());
        item.SetTrackName(effectEvent.TrackName);
        trackItemList.Add(item);
    }

    /// <summary>
    /// 更新trackMenuName，由childTrack回调
    /// </summary>
    private void UpdateChildTrackName(SkillMultiLineTrackStyle.ChildTrack track, string arg2)
    {
        // 同步给配置
        EffectData.FrameData[track.GetIndex()].TrackName = arg2;
        SkillEditorWindow.Instance.SaveConfig();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);
        // 销毁当前已有
        foreach (EffectTrackItem item in trackItemList)
        {
            item.Destroy();
        }
        trackItemList.Clear();

        // 基于特效数据 绘制轨道
        for (int i = 0; i < EffectData.FrameData.Count; i++)
        {
            CreateItem(EffectData.FrameData[i]);
        }
    }

    /// <summary>
    /// 添加数据，提供给trackStyle
    /// </summary>
    private void AddChildTrack()
    {
        SkillEffectEvent skillEffectEvent = new SkillEffectEvent();
        EffectData.FrameData.Add(skillEffectEvent);
        CreateItem(skillEffectEvent);
        SkillEditorWindow.Instance.SaveConfig();
        return;
    }

    /// <summary>
    /// 删除数据，提供给trackStyle
    /// </summary>
    private bool CheckDeleteChildTrack(int index)
    {
        if(index < 0 || index >= EffectData.FrameData.Count)
            return false;

        SkillEffectEvent skillEffectEvent = EffectData.FrameData[index];
        if (skillEffectEvent != null)
        {
            EffectData.FrameData.RemoveAt(index);
            SkillEditorWindow.Instance.SaveConfig();
            trackItemList[index].CleanEffectPreviewObj();
            trackItemList.RemoveAt(index);
        }
        return skillEffectEvent != null;
    }

    private void SwapChildTrack(int index1, int index2)
    {
        SkillEffectEvent data1 = EffectData.FrameData [index1];
        SkillEffectEvent data2 = EffectData.FrameData [index2];

        EffectData.FrameData[index1] = data2;
        EffectData.FrameData[index2] = data1;

        // 保存交给窗口的退出机制
    }

    public override void Destroy()
    {
        for (int i = 0; i < trackItemList.Count; i++)
        {
            trackItemList[i].CleanEffectPreviewObj();
        }
        trackStyle.Destroy();
    }

    public override void TickView(int frameIndex)
    {
        for (int i = 0; i < trackItemList.Count; i++)
        {
            trackItemList[i].TickView(frameIndex);
        }
    }
}
