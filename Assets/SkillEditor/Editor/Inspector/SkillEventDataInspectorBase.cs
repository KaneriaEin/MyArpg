using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public abstract class SkillEventDataInspectorBase
{
    protected VisualElement root;
    protected int itemFrameIndex;
    public void SetFrameIndex(int itemFrameIndex)
    {
        this.itemFrameIndex = itemFrameIndex;
    }
    public virtual void Draw(VisualElement root, TrackItemBase trackItem, SkillTrackBase track)
    {
        this.root = root;
        SetFrameIndex(trackItem.FrameIndex);
    }
}
public abstract class SkillEventDataInspectorBase<TrackItem, Track> : SkillEventDataInspectorBase where TrackItem : TrackItemBase where Track : SkillTrackBase
{
    protected TrackItem trackItem;
    protected Track track;

    public override void Draw(VisualElement root, TrackItemBase trackItem, SkillTrackBase track)
    {
        base.Draw(root, trackItem, track);
        this.track = (Track)track;
        this.trackItem = (TrackItem)trackItem;
        OnDraw();
    }

    public abstract void OnDraw();
}
