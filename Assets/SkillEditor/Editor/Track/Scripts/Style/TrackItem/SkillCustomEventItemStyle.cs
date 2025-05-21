using UnityEditor;
using UnityEngine.UIElements;

public class SkillCustomEventItemStyle : SkillTrackItemStyleBase
{
    private const string trackItemAssetPath = "Assets/SkillEditor/Editor/Track/Assets/TrackItem/EventTrackItem.uxml";

    public void Init(SkillTrackStyleBase trackStyle)
    {
        root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackItemAssetPath).Instantiate().Query<Label>();
        trackStyle.AddItem(root);
    }
}
