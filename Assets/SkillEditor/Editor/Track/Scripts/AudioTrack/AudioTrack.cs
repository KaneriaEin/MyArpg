using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UIElements;
using static Codice.CM.WorkspaceServer.DataStore.WkTree.WriteWorkspaceTree;

public class AudioTrack : SkillTrackBase
{
    private SkillMultiLineTrackStyle trackStyle;
    private List<AudioTrackItem> trackItemList = new List<AudioTrackItem>();
    public SkillAudioData AudioData { get => SkillEditorWindow.Instance.SkillConfig.SkillAudioData; }

    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        base.Init(menuParent, trackParent, frameWidth);
        trackStyle = new SkillMultiLineTrackStyle();
        trackStyle. Init(menuParent, trackParent, "音效配置", AddChildTrack, CheckDeleteChildTrack, SwapChildTrack, UpdateChildTrackName);
    
        ResetView();
    }

    private void CreateItem(SkillAudioEvent audioEvent)
    {
        AudioTrackItem item = new AudioTrackItem();
        item.Init(this,frameWidth, audioEvent, trackStyle.AddChildTrack());
        item.SetTrackName(audioEvent.TrackName);
        trackItemList.Add(item);
    }

    /// <summary>
    /// 更新trackMenuName，由childTrack回调
    /// </summary>
    private void UpdateChildTrackName(SkillMultiLineTrackStyle.ChildTrack track, string arg2)
    {
        // 同步给配置
        AudioData.FrameData[track.GetIndex()].TrackName = arg2;
        SkillEditorWindow.Instance.SaveConfig();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);
        // 销毁当前已有
        foreach (AudioTrackItem item in trackItemList)
        {
            item.Destroy();
        }
        trackItemList.Clear();

        if (SkillEditorWindow.Instance.SkillConfig == null) return;

        // 基于音效数据 绘制轨道
        for (int i = 0; i < AudioData.FrameData.Count; i++)
        {
            CreateItem(AudioData.FrameData[i]);
        }
    }

    /// <summary>
    /// 添加数据，提供给trackStyle
    /// </summary>
    private void AddChildTrack()
    {
        SkillAudioEvent skillAudioEvent = new SkillAudioEvent();
        AudioData.FrameData.Add(skillAudioEvent);
        CreateItem(skillAudioEvent);
        SkillEditorWindow.Instance.SaveConfig();
        return;
    }

    /// <summary>
    /// 删除数据，提供给trackStyle
    /// </summary>
    private bool CheckDeleteChildTrack(int index)
    {
        if(index < 0 || index >= AudioData.FrameData.Count)
            return false;

        SkillAudioEvent skillAudioEvent = AudioData.FrameData[index];
        if (skillAudioEvent != null)
        {
            AudioData.FrameData.RemoveAt(index);
            SkillEditorWindow.Instance.SaveConfig();
            trackItemList.RemoveAt(index);
        }
        return skillAudioEvent != null;
    }

    private void SwapChildTrack(int index1, int index2)
    {
        SkillAudioEvent data1 = AudioData.FrameData [index1];
        SkillAudioEvent data2 = AudioData.FrameData [index2];

        AudioData.FrameData[index1] = data2;
        AudioData.FrameData[index2] = data1;

        // 保存交给窗口的退出机制
    }

    public override void Destroy()
    {
        trackStyle.Destroy();
    }

    public override void OnPlay(int startFrameIndex)
    {
        for (int i = 0; i < AudioData.FrameData.Count; i++)
        {
            SkillAudioEvent audioEvent = AudioData.FrameData[i];
            if (audioEvent.AudioClip == null) continue;

            int audioClipLastFrameIndex = (int)(audioEvent.AudioClip.length * SkillEditorWindow.Instance.SkillConfig.FrameRate) + audioEvent.FrameIndex;

            // 意味着当前帧在一个clip的中间
            if (audioEvent.FrameIndex < startFrameIndex
                && audioClipLastFrameIndex > startFrameIndex)
            {
                int offsetFrame = startFrameIndex - audioEvent.FrameIndex;
                float rate = offsetFrame / (audioEvent.AudioClip.length * SkillEditorWindow.Instance.SkillConfig.FrameRate);
                EditorAudioUtility.PlayAudio(audioEvent.AudioClip, rate);
            }
            // 处理点击播放时的开始的第一帧
            else if (audioEvent.FrameIndex == startFrameIndex)
            {
                EditorAudioUtility.PlayAudio(audioEvent.AudioClip, 0);
            }
        }
    }

    public override void TickView(int frameIndex)
    {
        if (SkillEditorWindow.Instance.IsPlaying)
        {
            for (int i = 0; i < AudioData.FrameData.Count; i++)
            {
                SkillAudioEvent audioEvent = AudioData.FrameData[i];
                if (audioEvent.AudioClip != null && audioEvent.FrameIndex == frameIndex)
                {
                    // 播放音效，从头播放
                    EditorAudioUtility.PlayAudio(audioEvent.AudioClip, 0);
                }
            }
         }
    }
}
