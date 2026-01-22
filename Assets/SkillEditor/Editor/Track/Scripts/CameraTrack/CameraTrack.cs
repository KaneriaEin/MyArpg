using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraTrack : SkillTrackBase
{
    public SkillCameraData CameraData { get => SkillEditorWindow.Instance.SkillConfig.SkillCameraData; }

    public static Transform CameraTrackParent { get; private set; }

    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        base.Init(menuParent, trackParent, frameWidth);

        if (SkillEditorWindow.Instance.OnEditorScene)
        {
            if (SkillEditorWindow.Instance.DollyCameraTrackRoot != null)
            {
                CameraTrackParent = SkillEditorWindow.Instance.DollyCameraTrackRoot.transform;
                for (int i = CameraTrackParent.childCount - 1; i >= 0; i--)
                {
                    GameObject.DestroyImmediate(CameraTrackParent.GetChild(i).gameObject);
                }
            }
        }
        ResetView();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);
    }

    private GameObject dollyTrackObj;
    public override void TickView(int frameIndex)
    {
        if (CameraData == null || CameraData.DollyTrackPrefab == null || CameraData.DollyPosCurve == null)
        {
            Debug.Log("CameraData == null");
            return;
        }
        if (SkillEditorWindow.Instance.DollyCameraTrackRoot == null)
        {
            // Debug.Log("DollyCameraTrackRoot == null");
            return;
        }

        // 计算这一帧对应真实时间多少秒
        float timeSum = 0f;
        if(SkillEditorWindow.Instance.SkillConfig.SpeedCurve.keys.Length > 0)
        {
            for (int i = 0; i < frameIndex; i++)
            {
                timeSum += 1f / 60f * SkillEditorWindow.Instance.SkillConfig.SpeedCurve.Evaluate(i);
            }
        }
        else
        {
            timeSum = 1f / 60f * frameIndex;
        }

        #region POS相机位置
        // 判断这一帧是否在Pos Curve字典中
        if (frameIndex <= CameraData.DollyPosCurve.keys[CameraData.DollyPosCurve.keys.Length - 1].time && frameIndex >= CameraData.DollyPosCurve.keys[0].time)
        {
            GameObject.DestroyImmediate(dollyTrackObj);
            // 实例化
            dollyTrackObj = PrefabUtility.InstantiatePrefab(CameraData.DollyTrackPrefab, SkillEditorWindow.Instance.DollyCameraTrackRoot.transform) as GameObject;
            if (dollyTrackObj == null) return;
            dollyTrackObj.name = CameraData.DollyTrackPrefab.name;
            dollyTrackObj.transform.localScale = Vector3.one;
            dollyTrackObj.transform.localPosition = Vector3.zero;
            dollyTrackObj.transform.localRotation = Quaternion.identity;
            SkillEditorWindow.Instance.DollyCameraCart.m_Path = dollyTrackObj.GetComponent<CinemachineSmoothPath>();
            SkillEditorWindow.Instance.DollyCameraCart.m_Position = CameraData.DollyPosCurve.Evaluate(frameIndex);
            SkillEditorWindow.Instance.DollyCamera.LookAt = SkillEditorWindow.Instance.DollyCameraTrackRoot.transform.parent;
        }
        else
        {
            if (dollyTrackObj != null)
            {
                CleanDollyTrackObj();
            }
        }
        #endregion
        #region Fov
        // 判断这一帧是否在FOV Curve中
        if (frameIndex <= CameraData.DollyFovCurve.keys[CameraData.DollyFovCurve.keys.Length - 1].time && frameIndex >= CameraData.DollyFovCurve.keys[0].time)
        {
            SkillEditorWindow.Instance.DollyCamera.m_Lens.FieldOfView = CameraData.DollyFovCurve.Evaluate(frameIndex);
        }
        #endregion
        #region Dutch
        // 判断这一帧是否在Dutch Curve中
        if (frameIndex <= CameraData.DollyDutchCurve.keys[CameraData.DollyDutchCurve.keys.Length - 1].time && frameIndex >= CameraData.DollyDutchCurve.keys[0].time)
        {
            SkillEditorWindow.Instance.DollyCamera.m_Lens.Dutch = CameraData.DollyDutchCurve.Evaluate(frameIndex);
        }
        #endregion
        #region xOffset Curve
        // xOffset Curve中
        if(CameraData.DollyXOffsetCurve.keys.Length > 0)
        {
            if (frameIndex <= CameraData.DollyXOffsetCurve.keys[CameraData.DollyXOffsetCurve.keys.Length - 1].time && frameIndex >= CameraData.DollyXOffsetCurve.keys[0].time)
            {
                SkillEditorWindow.Instance.DollyCamera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.x = CameraData.DollyXOffsetCurve.Evaluate(frameIndex);
            }
        }
        #endregion
        #region yOffset Curve
        // yOffset Curve中
        if(CameraData.DollyYOffsetCurve.keys.Length > 0)
        {
            if (frameIndex <= CameraData.DollyYOffsetCurve.keys[CameraData.DollyYOffsetCurve.keys.Length - 1].time && frameIndex >= CameraData.DollyYOffsetCurve.keys[0].time)
            {
                SkillEditorWindow.Instance.DollyCamera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = CameraData.DollyYOffsetCurve.Evaluate(frameIndex);
            }
        }
        #endregion
        #region zOffset Curve
        // zOffset Curve中
        if (CameraData.DollyZOffsetCurve.keys.Length > 0)
        {
            if (frameIndex <= CameraData.DollyZOffsetCurve.keys[CameraData.DollyZOffsetCurve.keys.Length - 1].time && frameIndex >= CameraData.DollyZOffsetCurve.keys[0].time)
            {
                SkillEditorWindow.Instance.DollyCamera.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.z = CameraData.DollyZOffsetCurve.Evaluate(frameIndex);
            }
        }
        #endregion
        // 判断这一秒是否在字典中
        //if (timeSum <= CameraData.DollyPosCurve.keys[CameraData.DollyPosCurve.keys.Length - 1].time && timeSum >= CameraData.DollyPosCurve.keys[0].time)
        //{
        //    GameObject.DestroyImmediate(dollyTrackObj);
        //    // 实例化
        //    dollyTrackObj = PrefabUtility.InstantiatePrefab(CameraData.DollyTrackPrefab, SkillEditorWindow.Instance.DollyCameraTrackRoot.transform) as GameObject;
        //    dollyTrackObj.name = CameraData.DollyTrackPrefab.name;
        //    dollyTrackObj.transform.localScale = Vector3.one;
        //    dollyTrackObj.transform.localPosition = Vector3.zero;
        //    dollyTrackObj.transform.localRotation = Quaternion.identity;
        //    SkillEditorWindow.Instance.DollyCameraCart.m_Path = dollyTrackObj.GetComponent<CinemachineSmoothPath>();
        //    SkillEditorWindow.Instance.DollyCameraCart.m_Position = CameraData.DollyPosCurve.Evaluate(timeSum) / 100f;
        //    SkillEditorWindow.Instance.DollyCamera.LookAt = SkillEditorWindow.Instance.DollyCameraTrackRoot.transform.parent;
        //}
        //else
        //{
        //    if (dollyTrackObj != null)
        //    {
        //        CleanDollyTrackObj();
        //    }
        //}
    }

    public override void ResetView()
    {
        base.ResetView();
        // 强行重新生成预览
        CleanDollyTrackObj();
        TickView(SkillEditorWindow.Instance.CurrentSelectFrameIndex);
    }

    private void CleanDollyTrackObj()
    {
        if (dollyTrackObj != null)
        {
            SkillEditorWindow.Instance.DollyCameraCart.m_Path = null;
            SkillEditorWindow.Instance.DollyCameraCart.transform.position = Vector3.zero;
            SkillEditorWindow.Instance.DollyCameraCart.transform.localRotation = Quaternion.identity;
            GameObject.DestroyImmediate(dollyTrackObj);
            dollyTrackObj = null;
        }
    }
}
