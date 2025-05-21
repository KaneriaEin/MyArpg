using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillEffectTrackItemStyle : SkillTrackItemStyleBase
{
    private Label titleLabel;       // 音效名称
    private const string trackItemAssetPath = "Assets/SkillEditor/Editor/Track/Assets/TrackItem/AudioTrackItem.uxml";
    public VisualElement mainDragArea { get; private set; }            // 主要拖拽区域
    public bool isInit { get; private set; }

    public void Init(float frameUnitWidth, SkillEffectEvent skillEffectEvent, SkillMultiLineTrackStyle.ChildTrack childTrack)
    {
        if (!isInit && skillEffectEvent.Prefab != null)
        {
            titleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackItemAssetPath).Instantiate().Query<Label>();
            root = titleLabel;
            childTrack.InitContent(root);
            mainDragArea = root.Q<VisualElement>("Main");
            isInit = true;
        }
    }

    public void ResetView(float frameUnitWidth, SkillEffectEvent skillEffectEvent)
    {
        if (!isInit) return;
        if(skillEffectEvent.Prefab != null)
        {
            SetTitle(skillEffectEvent.Prefab.name);
            SetWidth(frameUnitWidth * skillEffectEvent.Duration);
            SetPosition(frameUnitWidth * skillEffectEvent.FrameIndex);
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
