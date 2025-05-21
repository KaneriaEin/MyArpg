using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class AttackDetectionTrack : SkillTrackBase
{
    private SkillMultiLineTrackStyle trackStyle;
    public SkillAttackDetectionData AttackDetectionData { get => SkillEditorWindow.Instance.SkillConfig.SkillAttackDetectionData; }
    private List<AttackDetectionTrackItem> trackItemList = new List<AttackDetectionTrackItem>();
    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        base.Init(menuParent, trackParent, frameWidth);
        trackStyle = new SkillMultiLineTrackStyle();
        trackStyle.Init(menuParent, trackParent, "攻击伤害检测", AddChildTrack, CheckDeleteChildTrack, SwapChildTrack, UpdateChildTrackName);

        ResetView();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);
        // 销毁当前已有
        foreach (AttackDetectionTrackItem item in trackItemList)
        {
            item.Destroy();
        }
        trackItemList.Clear();

        if (SkillEditorWindow.Instance.SkillConfig == null) return;

        // 基于音效数据 绘制轨道
        for (int i = 0; i < AttackDetectionData.FrameData.Count; i++)
        {
            CreateItem(AttackDetectionData.FrameData[i]);
        }
    }

    private void CreateItem(SkillAttackDetectionEvent attackDetectionEvent)
    {
        AttackDetectionTrackItem item = new AttackDetectionTrackItem();
        item.Init(this, frameWidth, attackDetectionEvent, trackStyle.AddChildTrack());
        item.SetTrackName(attackDetectionEvent.TrackName);
        trackItemList.Add(item);
    }

    /// <summary>
    /// 更新trackMenuName，由childTrack回调
    /// </summary>
    private void UpdateChildTrackName(SkillMultiLineTrackStyle.ChildTrack track, string arg2)
    {
        // 同步给配置
        AttackDetectionData.FrameData[track.GetIndex()].TrackName = arg2;
        SkillEditorWindow.Instance.SaveConfig();
    }

    /// <summary>
    /// 添加数据，提供给trackStyle
    /// </summary>
    private void AddChildTrack()
    {
        SkillAttackDetectionEvent skillattackEvent = new SkillAttackDetectionEvent();
        AttackDetectionData.FrameData.Add(skillattackEvent);
        CreateItem(skillattackEvent);
        SkillEditorWindow.Instance.SaveConfig();
        return;
    }

    /// <summary>
    /// 删除数据，提供给trackStyle
    /// </summary>
    private bool CheckDeleteChildTrack(int index)
    {
        if (index < 0 || index >= AttackDetectionData.FrameData.Count)
            return false;

        SkillAttackDetectionEvent skillAttackEvent = AttackDetectionData.FrameData[index];
        if (skillAttackEvent != null)
        {
            AttackDetectionData.FrameData.RemoveAt(index);
            SkillEditorWindow.Instance.SaveConfig();
            trackItemList.RemoveAt(index);
        }
        return skillAttackEvent != null;
    }

    private void SwapChildTrack(int index1, int index2)
    {
        SkillAttackDetectionEvent data1 = AttackDetectionData.FrameData[index1];
        SkillAttackDetectionEvent data2 = AttackDetectionData.FrameData[index2];

        AttackDetectionData.FrameData[index1] = data2;
        AttackDetectionData.FrameData[index2] = data1;

        // 保存交给窗口的退出机制
    }

    public override void Destroy()
    {
        trackStyle.Destroy();
    }

    public override void DrawGizmos()
    {
        int curFrameIndex = SkillEditorWindow.Instance.CurrentSelectFrameIndex;
        for (int i = 0; i < trackItemList.Count; i++)
        {
            SkillAttackDetectionEvent atkEvent = trackItemList[i].SkillAttackDetectionEvent;
            if (curFrameIndex < atkEvent.FrameIndex || curFrameIndex > atkEvent.FrameIndex + atkEvent.DurationFrame) continue;

            trackItemList[i].DrawGizmos();
        }
    }

    public override void OnSceneGUI()
    {
        int curFrameIndex = SkillEditorWindow.Instance.CurrentSelectFrameIndex;
        for (int i = 0; i < trackItemList.Count; i++)
        {
            if(SkillEditorInspector.currentTrackItem != trackItemList[i]) continue;
            SkillAttackDetectionEvent atkEvent = trackItemList[i].SkillAttackDetectionEvent;
            if (curFrameIndex < atkEvent.FrameIndex || curFrameIndex > atkEvent.FrameIndex + atkEvent.DurationFrame) continue;
            trackItemList[i].OnSceneGUI();
        }
    }
}
