using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackDetectionTrackItem : TrackItemBase<AttackDetectionTrack>
{
    private SkillMultiLineTrackStyle.ChildTrack childTrackStyle; // 将片段放入的子轨道
    private SkillAttackDetectionTrackItemStyle trackItemStyle;   // 片段
    private SkillAttackDetectionEvent skillAttackDetectionEvent;
    public SkillAttackDetectionEvent SkillAttackDetectionEvent { get { return skillAttackDetectionEvent; } }
    public void Init(AttackDetectionTrack attackDetectionTrack, float frameUnitWidth, SkillAttackDetectionEvent skillAttackDetectionEvent, SkillMultiLineTrackStyle.ChildTrack childTrack)
    {
        this.track = attackDetectionTrack;
        this.frameIndex = skillAttackDetectionEvent.FrameIndex;
        this.childTrackStyle = childTrack;
        this.skillAttackDetectionEvent = skillAttackDetectionEvent;
        if(skillAttackDetectionEvent.AttackHitConfig == null) skillAttackDetectionEvent.AttackHitConfig = new AttackHitConfig();
        trackItemStyle = new SkillAttackDetectionTrackItemStyle();
        itemStyle = trackItemStyle;

        normalColor = new Color(0.388f, 0.850f, 0.905f, 0.5f);
        selectColor = new Color(0.388f, 0.850f, 0.905f, 1f);

        ResetView(frameUnitWidth);
    }

    /// <summary>
    /// 根据起始位置和帧宽，刷新一下位置
    /// </summary>
    public override void ResetView(float frameUnitWidth)
    {
        base.ResetView(frameUnitWidth);
        if (!trackItemStyle.isInit)
        {
            trackItemStyle.Init(frameUnitWidth, skillAttackDetectionEvent, childTrackStyle);
            // 绑定事件
            trackItemStyle.mainDragArea.RegisterCallback<MouseDownEvent>(MouseDown);
            trackItemStyle.mainDragArea.RegisterCallback<MouseUpEvent>(MouseUp);
            trackItemStyle.mainDragArea.RegisterCallback<MouseOutEvent>(MouseOut);
            trackItemStyle.mainDragArea.RegisterCallback<MouseMoveEvent>(MouseMove);
        }
        trackItemStyle.ResetView(frameUnitWidth, skillAttackDetectionEvent);
    }

    /// <summary>
    /// 删除显示
    /// </summary>
    public void Destroy()
    {
        childTrackStyle.Destroy();
    }

    public void SetTrackName(string trackName)
    {
        childTrackStyle.SetTrackName(trackName);
    }

    #region 鼠标交互
    private bool mouseDrag = false;
    private float startDragPosX;
    private int startDragFrameIndex;
    private void MouseDown(MouseDownEvent evt)
    {
        startDragPosX = evt.mousePosition.x;
        startDragFrameIndex = frameIndex;
        mouseDrag = true;
        Select();
    }

    private void MouseUp(MouseUpEvent evt)
    {
        if (mouseDrag)
            ApplyDrag();
        mouseDrag = false;
    }

    private void MouseOut(MouseOutEvent evt)
    {
        if (mouseDrag)
            ApplyDrag();
        mouseDrag = false;
    }

    private void MouseMove(MouseMoveEvent evt)
    {
        if (mouseDrag)
        {
            float offsetPos = evt.mousePosition.x - startDragPosX;
            int offsetFrame = Mathf.RoundToInt(offsetPos / frameUnitWidth);
            int targetFrameIndex = startDragFrameIndex + offsetFrame;

            if (targetFrameIndex < 0 || offsetFrame == 0) return;

            // 确定修改的数据
            // 刷新视图
            frameIndex = targetFrameIndex;
            skillAttackDetectionEvent.FrameIndex = frameIndex;
            // 若超过右侧边界，拓展边界
            //CheckFrameCount();
            ResetView(frameUnitWidth);
        }
    }

    public void CheckFrameCount()
    {
        int frameCount = (int)(skillAttackDetectionEvent.DurationFrame * SkillEditorWindow.Instance.SkillConfig.FrameRate);
        if (frameIndex + frameCount > SkillEditorWindow.Instance.CurrentFrameCount)
        {
            SkillEditorWindow.Instance.CurrentFrameCount = frameIndex + frameCount;
        }
    }

    private void ApplyDrag()
    {
        if (startDragFrameIndex != frameIndex)
        {
            //skillAudioEvent.FrameIndex = frameIndex;
            // SkillEditorInspector.Instance.SetTrackItemFrameIndex(frameIndex);
        }
    }
    #endregion

    public void DrawGizmos()
    {
        SkillGizmosTool.DrawDetection(skillAttackDetectionEvent, SkillEditorWindow.Instance.PreviewCharacterObj.GetComponent<Skill_Player>());
    }

    public void OnSceneGUI()
    {
        Transform previewObj = SkillEditorWindow.Instance.PreviewCharacterObj.transform;
        switch (skillAttackDetectionEvent.AttackDetectionType)
        {
            case AttackDetectionType.Box:
                AttackBoxDetectionData boxDetectionData = (AttackBoxDetectionData)skillAttackDetectionEvent.AttackDetectionData;
                Vector3 position = previewObj.TransformPoint(boxDetectionData.Position);
                Quaternion quaternion = previewObj.rotation * Quaternion.Euler(boxDetectionData.Rotation);
                EditorGUI.BeginChangeCheck();
                Handles.TransformHandle(ref position, ref quaternion, ref boxDetectionData.Scale);
                if (EditorGUI.EndChangeCheck()) // 用户停止拽动
                {
                    boxDetectionData.Position = previewObj.InverseTransformPoint(position);
                    boxDetectionData.Rotation = (Quaternion.Inverse(previewObj.rotation) * quaternion).eulerAngles;
                    SkillEditorInspector.SetTrackItem(this, track);
                }
                break;
            case AttackDetectionType.Sphere:
                AttackSphereDetectionData sphereDetectionData = (AttackSphereDetectionData)skillAttackDetectionEvent.AttackDetectionData;
                Vector3 oldPosition = previewObj.TransformPoint(sphereDetectionData.Position);
                Vector3 newPosition = Handles.PositionHandle(oldPosition, Quaternion.identity);
                float newRadius = Handles.ScaleSlider(sphereDetectionData.Radius, newPosition, Vector3.up, Quaternion.identity, sphereDetectionData.Radius + 1, 0.1f);

                if(oldPosition != newPosition || newRadius != sphereDetectionData.Radius)
                {
                    sphereDetectionData.Position = previewObj.InverseTransformPoint(newPosition);
                    sphereDetectionData.Radius = newRadius;
                    SkillEditorInspector.SetTrackItem(this, track);
                }
                break;
            case AttackDetectionType.Fan:
                AttackFanDetectionData fanDetectionData = (AttackFanDetectionData)skillAttackDetectionEvent.AttackDetectionData;
                Quaternion fanquaternion = previewObj.rotation * Quaternion.Euler(fanDetectionData.Rotation);
                Vector3 fanposition = previewObj.TransformPoint(fanDetectionData.Position);
                // x:角度  y:高度 z:外圈半径
                Vector3 fanScale = new Vector3(fanDetectionData.Angle, fanDetectionData.Height, fanDetectionData.Radius);
                EditorGUI.BeginChangeCheck();
                Handles.TransformHandle(ref fanposition, ref fanquaternion, ref fanScale);
                float insideRadiuHandle = Handles.ScaleSlider(fanDetectionData.InsideRadius, fanposition, -previewObj.forward, Quaternion.identity, 1.5f, 0.1f);
                if (EditorGUI.EndChangeCheck()) // 用户停止拽动
                {
                    fanDetectionData.Position = previewObj.InverseTransformPoint(fanposition);
                    fanDetectionData.Rotation = (Quaternion.Inverse(previewObj.rotation) * fanquaternion).eulerAngles;
                    fanDetectionData.Angle = fanScale.x;
                    fanDetectionData.Height = fanScale.y;
                    fanDetectionData.Radius = fanScale.z;
                    fanDetectionData.InsideRadius = insideRadiuHandle;

                    SkillEditorInspector.SetTrackItem(this, track);
                }
                break;
        }
    }
}
