using UnityEngine.UIElements;

public abstract class SkillTrackStyleBase
{
    public Label titleLabel;
    #region 自身父物体
    public VisualElement menuParent;
    public VisualElement contentParent;
    #endregion

    #region 自身根节点
    public VisualElement menuRoot;
    public VisualElement contentRoot;
    #endregion

    public virtual void AddItem(VisualElement ve)
    {
        contentRoot.Add(ve);
    }

    public virtual void DeleteItem(VisualElement ve)
    {
        contentRoot.Remove(ve);
    }

    public virtual void Destroy()
    {
        if (menuRoot != null)
            menuParent.Remove(menuRoot);
        if (contentRoot != null)
            contentParent.Remove(contentRoot);
    }
}