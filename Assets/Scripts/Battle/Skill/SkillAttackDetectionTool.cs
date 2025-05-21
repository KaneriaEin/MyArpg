using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class SkillAttackDetectionTool
{
    private static Collider[] detectionResults = new Collider[20];
    public static Collider[] ShapeDetection(Transform modelTransform, AttackDetectionDataBase data, AttackDetectionType attackDetectionType, LayerMask layerMask)
    {
        switch (attackDetectionType)
        {
            case AttackDetectionType.Box:
                return BoxDetection(modelTransform, (AttackBoxDetectionData)data, layerMask);
            case AttackDetectionType.Sphere:
                return SphereDetection(modelTransform, (AttackSphereDetectionData)data, layerMask);
            case AttackDetectionType.Fan:
                return FanDetection(modelTransform, (AttackFanDetectionData)data, layerMask);
        }
        return null;
    }

    public static Collider[] BoxDetection(Transform modelTransform, AttackBoxDetectionData data, LayerMask layerMask)
    {
        CleanDetectionResults();
        Physics.OverlapBoxNonAlloc(modelTransform.TransformPoint(data.Position), data.Scale / 2, detectionResults, modelTransform.rotation * Quaternion.Euler(data.Rotation), layerMask);
        return detectionResults;
    }

    public static Collider[] SphereDetection(Transform modelTransform, AttackSphereDetectionData data, LayerMask layerMask)
    {
        CleanDetectionResults();
        Physics.OverlapSphereNonAlloc(modelTransform.TransformPoint(data.Position), data.Radius, detectionResults, layerMask);
        return detectionResults;
    }

    public static Collider[] FanDetection(Transform modelTransform, AttackFanDetectionData data, LayerMask layerMask)
    {
        CleanDetectionResults();
        Vector3 size = new Vector3();
        size.x = data.Radius * 2;
        size.z = size.x;
        size.y = data.Height;
        Vector3 fanPosition = modelTransform.TransformPoint(data.Position);
        Physics.OverlapBoxNonAlloc(fanPosition, size / 2, detectionResults, modelTransform.rotation * Quaternion.Euler(data.Rotation), layerMask);

        // 过滤无效检测
        Vector3 fanForward = modelTransform.rotation * Quaternion.Euler(data.Rotation) * Vector3.forward;
        for(int i = 0; i< detectionResults.Length; i++)
        {
            if (detectionResults[i] == null) break;
            // 过滤内半径内的、外半径外的
            Vector3 point = detectionResults[i].ClosestPoint(modelTransform.position);
            float distance = Vector3.Distance(point, modelTransform.position);
            bool remove = distance < data.InsideRadius || distance > data.Radius;
            if (!remove)
            {
                // 过滤角度范围外的
                Vector3 dir = point - fanPosition;
                float angle = Vector3.Angle(fanForward, dir);
                remove = angle > data.Angle / 2;
            }
            if (remove)
            {
                Debug.Log("remove");
                if (i < detectionResults.Length - 1)
                {
                    detectionResults[i] = null;
                }
            }
        }
        return detectionResults;
    }

    private static void CleanDetectionResults()
    {
        for (int i = 0; i < detectionResults.Length; i++)
        {
            detectionResults[i] = null;
        }
    }
}
