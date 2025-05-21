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
    private const float headHeight = 35; // 5�Ǽ��
    private const float itemHeight = 32; // 2�ǵײ���߾�
    #endregion
    /// <summary>
    /// �������
    /// </summary>
    private Action addChildTrackAction;
    /// <summary>
    /// ɾ������
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

        // ����ӹ���İ�ť
        Button addButton = menuRoot.Q<Button>("AddButton");
        addButton.clicked += AddButtonClick;
        UpdateSize();
    }

    #region �ӹ����꽻��
    private bool isDragging = false;
    private int selectTrackIndex = -1;
    private void ItemParentMouseDown(MouseDownEvent evt)
    {
        // �رվɵ�
        if (selectTrackIndex != -1)
        {
            childTrackList[selectTrackIndex].UnSelect();
        }
        // ͨ���߶��Ƶ�����ǰ�������ǵڼ���
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
            selectTrackIndex = mouseTrackIndex; // ��Ϊ��ק��ı���λ�ú���������Ҫ���µ�ǰѡ�й������
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
        // ��Ϊ����ƶ�����child�ϣ�unityҲ����ô˺���������ٶ�һ��У�飬ֻҪû�������false
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
            // ����֤��Ч��,�ϲ���Ҫ��֤������ȷ
            ChildTrack childTrack1 = childTrackList[index1];
            ChildTrack childTrack2 = childTrackList[index2];
            childTrackList[index1] = childTrack2;
            childTrackList[index2] = childTrack1;
            UpdateChilds();

            // �ϲ㱣�����ݸ���
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
    /// ����ӹ��
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
    /// ����childTrack��������Ϊ���಻�漰���ݲ��棬�����ȻҪ�ϲ�ҵ�����������
    /// </summary>
    private void UpdateChildTrackMenuName(ChildTrack childTrack, string menuName)
    {
        updateTrackNameAction?.Invoke(childTrack, menuName);
    }

    /// <summary>
    /// ɾ���ӹ������ʾ��ˢ����
    /// </summary>
    private void DeleteChildTrack(ChildTrack childTrack)
    {
        int index = childTrack.GetIndex();
        childTrack.DelDestroy();
        childTrackList.RemoveAt(index);
        // �����ӹ������Ҫ����һ������
        UpdateChilds(index);
        UpdateSize();
    }

    /// <summary>
    /// ɾ���ӹ������ʾ�����ݣ�style����ɾ��ʾ��audioTrack����ɾ����
    /// </summary>
    private void DeleteChildTrackAndData(ChildTrack childTrack)
    {
        if (delChildTrackFunc == null) return;
        // ���ϼ������������ж��ܲ���ɾ��
        int index = childTrack.GetIndex();
        if (delChildTrackFunc(index))
        {
            childTrack.DelDestroy();
            childTrackList.RemoveAt(index);
            // �����ӹ������Ҫ����һ������
            UpdateChilds(index);
            UpdateSize();
        }
    }

    /// <summary>
    /// ��������child������index
    /// </summary>
    private void UpdateChilds(int startIndex = 0)
    {
        for (int i = startIndex; i < childTrackList.Count; i++)
        {
            childTrackList[i].SetIndex(i);
        }
    }

    /// <summary>
    /// ���й���е��ӹ��
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
        /// ɾ������
        /// </summary>
        private Action<ChildTrack> deleteAction;
        /// <summary>
        /// ɾ����ʾ
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
        /// ��ʼ������
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
        /// ɾ����ʾ
        /// </summary>
        public void Destroy()
        {
            destroyAction(this);
        }

        /// <summary>
        /// ɾ�������ã�ɾ����ʾʱҲ�����
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
