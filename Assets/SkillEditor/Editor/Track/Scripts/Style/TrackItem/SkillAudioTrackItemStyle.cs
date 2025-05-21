using UnityEditor;
using UnityEngine.UIElements;

public class SkillAudioTrackItemStyle : SkillTrackItemStyleBase
{
    private Label titleLabel;       // 音效名称
    private const string trackItemAssetPath = "Assets/SkillEditor/Editor/Track/Assets/TrackItem/AudioTrackItem.uxml";
    public VisualElement mainDragArea { get; private set; }            // 主要拖拽区域
    public bool isInit { get; private set; }

    public void Init(float frameUnitWidth, SkillAudioEvent skillAudioEvent, SkillMultiLineTrackStyle.ChildTrack childTrack)
    {
        if (!isInit && skillAudioEvent.AudioClip != null)
        {
            titleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackItemAssetPath).Instantiate().Query<Label>();
            root = titleLabel;
            childTrack.InitContent(root);
            mainDragArea = root.Q<VisualElement>("Main");
            isInit = true;
        }
    }

    public void ResetView(float frameUnitWidth, SkillAudioEvent skillAudioEvent)
    {
        if (!isInit) return;
        if(skillAudioEvent.AudioClip != null)
        {
            SetTitle(skillAudioEvent.AudioClip.name);
            SetWidth(frameUnitWidth * skillAudioEvent.AudioClip.length * SkillEditorWindow.Instance.SkillConfig.FrameRate);
            SetPosition(frameUnitWidth * skillAudioEvent.FrameIndex);
        }
        else
        {
            SetTitle("");
            SetWidth(0);
            SetPosition(0);
        }
    }


    public void SetTitle(string title)
    {
        titleLabel.text = title;
    }

}
