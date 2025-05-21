using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillAnimationTrackItemStyle : SkillTrackItemStyleBase
{
    private const string trackItemAssetPath = "Assets/SkillEditor/Editor/Track/Assets/TrackItem/AnimationTrackItem.uxml";

    private Label titleLabel;       // 动画名称
    public VisualElement mainDragArea { get; private set; }            // 主要拖拽区域
    public VisualElement animationOverLine { get; private set; }
    public void Init(SkillTrackStyleBase trackStyle, int startFrameIndex,float frameUnitWidth)
    {
        titleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackItemAssetPath).Instantiate().Query<Label>();
        root = titleLabel;
        mainDragArea = root.Q<VisualElement>("Main");
        animationOverLine = root.Q<VisualElement>("OverLine");
        trackStyle.AddItem(root);
    }

    public void SetTitle(string title)
    {
        titleLabel.text = title;
    }
}
