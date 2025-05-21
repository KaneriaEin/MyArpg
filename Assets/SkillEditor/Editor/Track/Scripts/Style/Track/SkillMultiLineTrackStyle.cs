using log4net.Repository;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillMultiLineTrackStyle : SkillTrackStyleBase
{
    #region MyRegion
    private const string menuAssetPath = "Assets/SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MultiLineTrackMenu.uxml";
    private const string trackAssetPath = "Assets/SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MultiLineTrackContent.uxml";
    private const float headHeight = 35; // 5是间距
    private const float itemHeight = 32; // 2是底部外边距
    #endregion
    /// <summary>
    /// 添加数据
    /// </summary>
    private Action addChildTrackAction;
    /// <summary>
    /// 删除数据
    /// </summary>
    private Func<int, bool> delChildTrackFunc;
    private Action<int, int> swapChildTrackAction;
    private Action<ChildTrack, string> updateTrackNameAction;

    /// <summary>
    /// ve TrackMenuList
    /// </summary>
    public VisualElement menuItemParent;
    public VisualElement trackRoot;

    private List<ChildTrack> childTrackList = new List<ChildTrack>();

    public void Init(VisualElement menuParent, VisualElement contentParent, string title, Action addChildTrackAction, Func<int, bool> delChildTrackFunc, Action<int, int> swapChildTrackAction, Action<ChildTrack, string> updateTrackNameAction)
    {
        this.addChildTrackAction = addChildTrackAction;
        this.delChildTrackFunc = delChildTrackFunc;
        this.swapChildTrackAction = swapChildTrackAction;
        this.updateTrackNameAction = updateTrackNameAction;

        this.menuParent = menuParent;
        this.contentParent = contentParent;

        menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(menuAssetPath).Instantiate().Query().ToList()[1];
        menuParent.Add(menuRoot);
        titleLabel = menuRoot.Q<Label>("Title");
        titleLabel.text = title;
        menuItemParent = menuRoot.Q("TrackMenuList");

        menuItemParent.RegisterCallback<MouseDownEvent>(ItemParentMouseDown);
        menuItemParent.RegisterCallback<MouseMoveEvent>(ItemParentMouseMove);
        menuItemParent.RegisterCallback<MouseUpEvent>(ItemParentMouseUp);
        menuItemParent.RegisterCallback<MouseOutEvent>(ItemParentMouseOut);
     
        contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackAssetPath).Instantiate().Query().ToList()[1];
        contentParent.Add(contentRoot);

        // 添加子轨道的按钮
        Button addButton = menuRoot.Q<Button>("AddButton");
        addButton.clicked += AddButtonClick;
        UpdateSize();
    }

    #region 子轨道鼠标交互
    private bool isDragging = false;
    private int selectTrackIndex = -1;
    private void ItemParentMouseDown(MouseDownEvent evt)
    {
        // 关闭旧的
        if (selectTrackIndex != -1)
        {
            childTrackList[selectTrackIndex].UnSelect();
        }
        // 通过高度推导出当前交互的是第几个
        selectTrackIndex = GetChildIndexByMousePosition(evt.localMousePosition.y);
        childTrackList[selectTrackIndex].Select();

        isDragging = true;
    }

    private void ItemParentMouseMove(MouseMoveEvent evt)
    {
        if (!isDragging || selectTrackIndex == -1) return;
        int mouseTrackIndex = GetChildIndexByMousePosition(evt.localMousePosition.y);
        if(mouseTrackIndex != selectTrackIndex)
        {
            SwapChildTrack(selectTrackIndex, mouseTrackIndex);
            selectTrackIndex = mouseTrackIndex; // 因为拖拽后改变了位置和索引，需要更新当前选中轨道索引
        }
    }

    private void ItemParentMouseUp(MouseUpEvent evt)
    {
        isDragging = false;
        if (selectTrackIndex != -1)
        {
            childTrackList[selectTrackIndex].UnSelect();
            selectTrackIndex = -1;
        }
    }

    private void ItemParentMouseOut(MouseOutEvent evt)
    {
        // 因为鼠标移动到了child上，unity也会调用此函数，因此再多一层校验，只要没出界才判false
        if (!menuItemParent.contentRect.Contains(evt.localMousePosition))
        {
            isDragging = false;
            if (selectTrackIndex != -1)
            {
                childTrackList[selectTrackIndex].UnSelect();
                selectTrackIndex = -1;
            }
        }
    }

    private int GetChildIndexByMousePosition(float mouseY)
    {
        int trackIndex = (int)(mouseY / itemHeight);
        trackIndex = Mathf.Clamp(trackIndex,0, childTrackList.Count - 1);
        return trackIndex;
    }
    #endregion

    private void SwapChildTrack(int index1, int index2)
    {
        if (index1 != index2)
        {
            // 不验证有效性,上层需要保证数据正确
            ChildTrack childTrack1 = childTrackList[index1];
            ChildTrack childTrack2 = childTrackList[index2];
            childTrackList[index1] = childTrack2;
            childTrackList[index2] = childTrack1;
            UpdateChilds();

            // 上层保存数据更变
            swapChildTrackAction(index1, index2);
        }
    }

    private void UpdateSize()
    {
        float height = headHeight + (childTrackList.Count * itemHeight);
        contentRoot.style.height = height;
        menuRoot.style.height = height;
        menuItemParent.style.height = childTrackList.Count * itemHeight;
    }

    /// <summary>
    /// 添加子轨道
    /// </summary>
    private void AddButtonClick()
    {
        addChildTrackAction?.Invoke();
    }

    public ChildTrack AddChildTrack()
    {
        ChildTrack childTrack = new ChildTrack();
        childTrack.Init(menuItemParent, childTrackList.Count, contentRoot, DeleteChildTrackAndData, DeleteChildTrack, updateTrackNameAction);
        childTrackList.Add(childTrack);
        UpdateSize();
        return childTrack;
    }

    /// <summary>
    /// 更新childTrack操作，因为本类不涉及数据层面，因此仍然要上层业务层来做事情
    /// </summary>
    private void UpdateChildTrackMenuName(ChildTrack childTrack, string menuName)
    {
        updateTrackNameAction?.Invoke(childTrack, menuName);
    }

    /// <summary>
    /// 删除子轨道的显示，刷新用
    /// </summary>
    private void DeleteChildTrack(ChildTrack childTrack)
    {
        int index = childTrack.GetIndex();
        childTrack.DelDestroy();
        childTrackList.RemoveAt(index);
        // 所有子轨道都需要更新一下索引
        UpdateChilds(index);
        UpdateSize();
    }

    /// <summary>
    /// 删除子轨道的显示与数据，style负责删显示，audioTrack负责删数据
    /// </summary>
    private void DeleteChildTrackAndData(ChildTrack childTrack)
    {
        if (delChildTrackFunc == null) return;
        // 由上级具体轨道类来判断能不能删除
        int index = childTrack.GetIndex();
        if (delChildTrackFunc(index))
        {
            childTrack.DelDestroy();
            childTrackList.RemoveAt(index);
            // 所有子轨道都需要更新一下索引
            UpdateChilds(index);
            UpdateSize();
        }
    }

    /// <summary>
    /// 更新所有child的索引index
    /// </summary>
    private void UpdateChilds(int startIndex = 0)
    {
        for (int i = startIndex; i < childTrackList.Count; i++)
        {
            childTrackList[i].SetIndex(i);
        }
    }

    /// <summary>
    /// 多行轨道中的子轨道
    /// </summary>
    public class ChildTrack
    {
        private const string childTrackMenuAssetPath = "Assets/SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MultiLineTrackMenuItem.uxml";
        private const string childTrackContentAssetPath = "Assets/SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MultiLineTrackContentItem.uxml";

        public VisualElement menuRoot;
        public VisualElement trackRoot;

        public VisualElement menuParent;
        public VisualElement trackParent;

        private TextField trackNameField;

        /// <summary>
        /// 删除数据
        /// </summary>
        private Action<ChildTrack> deleteAction;
        /// <summary>
        /// 删除显示
        /// </summary>
        private Action<ChildTrack> destroyAction;
        private Action<ChildTrack, string> updateTrackNameAction;

        private static Color normalColor = new Color(0, 0, 0, 0);
        private static Color selectColor = Color.green;

        private int index;
        private VisualElement content;

        public void Init(VisualElement menuParent, int index, VisualElement contentParent, Action<ChildTrack> deleteAction, Action<ChildTrack> destroyAction, Action<ChildTrack, string> updateTrackNameAction)
        {
            this.menuParent = menuParent;
            this.trackParent = contentParent;
            this.deleteAction = deleteAction;
            this.destroyAction = destroyAction;
            this.updateTrackNameAction = updateTrackNameAction;

            menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(childTrackMenuAssetPath).Instantiate().Query().ToList()[1];
            menuParent.Add(menuRoot);

            trackNameField = menuRoot.Q<TextField>("NameField");
            trackNameField.RegisterCallback<FocusInEvent>(TrackNameFieldFocusIn);
            trackNameField.RegisterCallback<FocusOutEvent>(TrackNameFieldFocusOut);

            Button deleteButton = menuRoot.Q<Button>("DeleteButton");
            deleteButton.clicked += ()=> deleteAction(this);

            trackRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(childTrackContentAssetPath).Instantiate().Query().ToList()[1];
            trackParent.Add(trackRoot);

            SetIndex(index);
            UnSelect();
        }

        private string oldTrackNameFieldValue;
        private void TrackNameFieldFocusIn(FocusInEvent evt)
        {
            oldTrackNameFieldValue = trackNameField.value;
        }

        private void TrackNameFieldFocusOut(FocusOutEvent evt)
        {
            if(oldTrackNameFieldValue != trackNameField.value)
            {
                updateTrackNameAction?.Invoke(this, trackNameField.value);
            }
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        public void InitContent(VisualElement content)
        {
            this.content = content;
            trackRoot.Add(content);
        }

        public void SetTrackName(string trackName)
        {
            trackNameField.value = trackName;
        }

        public int GetIndex()
        {
            return this.index;
        }

        public void SetIndex(int index)
        {
            this.index = index;
            float height = 0;
            Vector3 menuPos = menuRoot.transform.position;
            height = index * itemHeight;
            menuPos.y = height;
            menuRoot.transform.position = menuPos;

            Vector3 trackPos = trackRoot.transform.position;
            height = index * itemHeight + headHeight;
            trackPos.y = height;
            trackRoot.transform.position = trackPos;
        }

        /// <summary>
        /// 删除显示
        /// </summary>
        public void Destroy()
        {
            destroyAction(this);
        }

        /// <summary>
        /// 删除数据用，删除显示时也会调用
        /// </summary>
        public void DelDestroy()
        {
            menuParent.Remove(menuRoot);
            trackParent.Remove(trackRoot);
        }

        public void Select()
        {
            menuRoot.style.backgroundColor = selectColor;
        }

        public void UnSelect()
        {
            menuRoot.style.backgroundColor = normalColor;
        }
    }
}
