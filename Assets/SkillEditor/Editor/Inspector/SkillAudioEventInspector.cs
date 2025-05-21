using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillAudioEventInspector : SkillEventDataInspectorBase<AudioTrackItem, AudioTrack>
{
    private FloatField volumeField;
    public override void OnDraw()
    {
        // 获取轨道索引
        itemFrameIndex = trackItem.FrameIndex;

        // 动画资源
        ObjectField audioClipAssetField = new ObjectField("音效资源");
        audioClipAssetField.objectType = typeof(AudioClip);
        audioClipAssetField.value = trackItem.SkillAudioEvent.AudioClip;
        audioClipAssetField.RegisterValueChangedCallback(AudioClipAssetFieldValueChanged);
        root.Add(audioClipAssetField);

        // 动画资源
        volumeField = new FloatField("播放音量");
        volumeField.value = trackItem.SkillAudioEvent.Volume;
        volumeField.RegisterCallback<FocusInEvent>(VolumeFieldFocusIn);
        volumeField.RegisterCallback<FocusOutEvent>(VolumeFieldFocusOut);
        root.Add(volumeField);
    }
    private void AudioClipAssetFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        AudioClip clip = evt.newValue as AudioClip;
        // 保存到配置
        trackItem.SkillAudioEvent.AudioClip = clip;
        SkillEditorWindow.Instance.SaveConfig();
        trackItem.ResetView();
    }

    float oldVolumeValue;
    private void VolumeFieldFocusIn(FocusInEvent evt)
    {
        oldVolumeValue = volumeField.value;
    }

    private void VolumeFieldFocusOut(FocusOutEvent evt)
    {
        if (oldVolumeValue == volumeField.value) return;
        trackItem.SkillAudioEvent.Volume = volumeField.value;
        SkillEditorWindow.Instance.SaveConfig();
    }
}
