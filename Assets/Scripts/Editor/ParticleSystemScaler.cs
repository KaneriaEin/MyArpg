using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ParticleSystemScaler : EditorWindow
{
    private GameObject targetPrefab;
    private float scaleFactor = 0.1f;
    private Vector2 scrollPosition;

    [MenuItem("Tools/Particle System Scaler")]
    public static void ShowWindow()
    {
        GetWindow<ParticleSystemScaler>("Particle Scaler");
    }

    private void OnGUI()
    {
        GUILayout.Label("Particle System Batch Scaler", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        targetPrefab = (GameObject)EditorGUILayout.ObjectField("Target Prefab", targetPrefab, typeof(GameObject), false);
        scaleFactor = EditorGUILayout.FloatField("Scale Factor", scaleFactor);

        EditorGUILayout.Space();

        if (targetPrefab == null)
        {
            EditorGUILayout.HelpBox("Please assign a Prefab", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Apply Scale", GUILayout.Height(40)))
        {
            ApplyScaleToPrefab();
        }

        // 预览信息
        if (targetPrefab != null)
        {
            EditorGUILayout.Space();
            GUILayout.Label("Preview:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));

            // 这里只能用targetPrefab预览，因为还没实例化
            var systems = targetPrefab.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in systems)
            {
                EditorGUILayout.LabelField($"{ps.name} - Size will be ×{scaleFactor}");
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private void ApplyScaleToPrefab()
    {
        if (targetPrefab == null) return;

        // 获取Prefab的资产路径
        string prefabPath = AssetDatabase.GetAssetPath(targetPrefab);
        if (string.IsNullOrEmpty(prefabPath))
        {
            Debug.LogError("Could not find path for prefab: " + targetPrefab.name);
            return;
        }

        // ★ 关键修复：使用 LoadPrefabContents 加载Prefab的可编辑内容 ★
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);

        try
        {
            // 注册Undo，以便在编辑器内可以撤销这次批量操作
            Undo.RegisterFullObjectHierarchyUndo(prefabRoot, "Scale Particle Systems");

            var systems = prefabRoot.GetComponentsInChildren<ParticleSystem>(true);
            int count = 0;

            foreach (var ps in systems)
            {
                ScaleParticleSystem(ps, scaleFactor);
                count++;
            }

            // ★ 保存修改回原Prefab ★
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);

            // 刷新资源数据库，确保Unity识别到变更
            AssetDatabase.Refresh();

            Debug.Log($"Successfully scaled {count} particle systems by factor {scaleFactor}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error scaling particles: {e.Message}\n{e.StackTrace}");
        }
        finally
        {
            // 重要：释放LoadPrefabContents加载的内容
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }
    }

    private void ScaleParticleSystem(ParticleSystem ps, float factor)
    {
        var main = ps.main;
        var startSize = main.startSize;

        // 处理不同类型的 MinMaxCurve
        switch (startSize.mode)
        {
            case ParticleSystemCurveMode.Constant:
                startSize.constant *= factor;
                break;

            case ParticleSystemCurveMode.TwoConstants:
                startSize.constantMin *= factor;
                startSize.constantMax *= factor;
                break;

            case ParticleSystemCurveMode.Curve:
                ScaleCurveKeys(startSize.curve, factor);
                break;

            case ParticleSystemCurveMode.TwoCurves:
                ScaleCurveKeys(startSize.curveMin, factor);
                ScaleCurveKeys(startSize.curveMax, factor);
                break;
        }

        main.startSize = startSize;
    }

    private void ScaleCurveKeys(AnimationCurve curve, float factor)
    {
        if (curve == null) return;

        Keyframe[] keys = curve.keys;
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].value *= factor;
        }
        curve.keys = keys;
    }
}