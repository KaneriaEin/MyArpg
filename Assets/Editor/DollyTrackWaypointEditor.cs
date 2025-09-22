using UnityEngine;
using UnityEditor;
using Cinemachine;

public class DollyTrackWaypointEditor : EditorWindow
{
    private CinemachineSmoothPath targetTrack;
    private Vector3 positionOffset = Vector3.zero;

    [MenuItem("Tools/Cinemachine/Add Scene View Camera as Waypoint")]
    public static void AddSceneViewCameraAsWaypoint()
    {
        // 获取当前场景视图的相机位置和旋转
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null)
        {
            Debug.LogWarning("No active Scene View found.");
            return;
        }

        Camera sceneCam = sceneView.camera;
        Vector3 cameraPosition = sceneCam.transform.position;
        Quaternion cameraRotation = sceneCam.transform.rotation;

        // 尝试获取选中的Smooth Path
        CinemachineSmoothPath selectedTrack = Selection.activeGameObject?.GetComponent<CinemachineSmoothPath>();
        if (selectedTrack == null)
        {
            // 如果没有选中的Track，创建一个新的
            GameObject trackGO = new GameObject("Dolly Track (Smooth Path)");
            selectedTrack = trackGO.AddComponent<CinemachineSmoothPath>();
            Undo.RegisterCreatedObjectUndo(trackGO, "Create Dolly Track");
            Debug.Log("Created a new CinemachineSmoothPath as none was selected.");
        }

        // 向选中的Track添加路点
        Undo.RecordObject(selectedTrack, "Add Waypoint");
        CinemachineSmoothPath.Waypoint newWaypoint = new CinemachineSmoothPath.Waypoint();
        newWaypoint.position = selectedTrack.transform.InverseTransformPoint(cameraPosition); // 转换为本地坐标
        newWaypoint.roll = 0; // 根据需要调整

        // 扩展Waypoints数组并添加新路点
        var waypointsList = new System.Collections.Generic.List<CinemachineSmoothPath.Waypoint>(selectedTrack.m_Waypoints);
        waypointsList.Add(newWaypoint);
        selectedTrack.m_Waypoints = waypointsList.ToArray();

        // 选中新创建的路点所在的Track，方便继续操作
        Selection.activeGameObject = selectedTrack.gameObject;

        Debug.Log($"Added new waypoint from Scene View camera to '{selectedTrack.gameObject.name}'.");
    }

    // 添加一个带偏移量的版本到菜单
    [MenuItem("Tools/Cinemachine/Add Waypoint with Offset...")]
    public static void ShowWindow()
    {
        GetWindow<DollyTrackWaypointEditor>("Add Waypoint with Offset");
    }

    private void OnGUI()
    {
        GUILayout.Label("Add a waypoint from scene camera with an offset", EditorStyles.boldLabel);

        positionOffset = EditorGUILayout.Vector3Field("Position Offset", positionOffset);

        if (GUILayout.Button("Add Waypoint"))
        {
            AddWaypointWithOffset();
        }
    }

    private void AddWaypointWithOffset()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null) return;

        Camera sceneCam = sceneView.camera;
        Vector3 cameraPosition = sceneCam.transform.position + positionOffset; // 应用偏移

        CinemachineSmoothPath selectedTrack = Selection.activeGameObject?.GetComponent<CinemachineSmoothPath>();
        if (selectedTrack == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a CinemachineSmoothPath first!", "OK");
            return;
        }

        Undo.RecordObject(selectedTrack, "Add Waypoint with Offset");
        CinemachineSmoothPath.Waypoint newWaypoint = new CinemachineSmoothPath.Waypoint();
        newWaypoint.position = selectedTrack.transform.InverseTransformPoint(cameraPosition);

        var waypointsList = new System.Collections.Generic.List<CinemachineSmoothPath.Waypoint>(selectedTrack.m_Waypoints);
        waypointsList.Add(newWaypoint);
        selectedTrack.m_Waypoints = waypointsList.ToArray();

        Debug.Log($"Added offset waypoint to '{selectedTrack.gameObject.name}'.");
    }
}