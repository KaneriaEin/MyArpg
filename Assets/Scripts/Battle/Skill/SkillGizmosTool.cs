#if UNITY_EDITOR
using UnityEngine;

public static class SkillGizmosTool
{
    public static void DrawDetection(SkillAttackDetectionEvent skillAttackDetectionEvent, Skill_Player skill_Player)
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Transform modelTransform = skill_Player.ModelTransform == null ? skill_Player.transform : skill_Player.ModelTransform;
        Matrix4x4 rotateAndPositionMat = new Matrix4x4();
        switch (skillAttackDetectionEvent.AttackDetectionType)
        {
            case AttackDetectionType.Weapon:
                AttackWeaponDetectionData weaponDetectionData = (AttackWeaponDetectionData)skillAttackDetectionEvent.AttackDetectionData;
                if (!string.IsNullOrEmpty(weaponDetectionData.weaponName) && skill_Player.WeaponDic.TryGetValue(weaponDetectionData.weaponName, out WeaponController weapon))
                {
                    Collider collider = weapon.GetComponent<Collider>();
                    rotateAndPositionMat.SetTRS(collider.transform.position, collider.transform.rotation, collider.transform.localScale);
                    Gizmos.matrix = rotateAndPositionMat;
                    if (collider is BoxCollider)
                    {
                        BoxCollider boxCollider = (BoxCollider)collider;
                        Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
                    }
                    else if (collider is SphereCollider)
                    {
                        SphereCollider sphereCollider = (SphereCollider)collider;
                        Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
                    }
                }
                break;
            case AttackDetectionType.Box:
                AttackBoxDetectionData boxDetectionData = (AttackBoxDetectionData)skillAttackDetectionEvent.AttackDetectionData;
                Vector3 position = modelTransform.TransformPoint(boxDetectionData.Position);
                Quaternion rotation = modelTransform.rotation * Quaternion.Euler(boxDetectionData.Rotation);
                rotateAndPositionMat.SetTRS(position, rotation, Vector3.one);
                Gizmos.matrix = rotateAndPositionMat;
                Gizmos.DrawCube(Vector3.zero, boxDetectionData.Scale);
                break;
            case AttackDetectionType.Sphere:
                AttackSphereDetectionData sphereDetectionData = (AttackSphereDetectionData)skillAttackDetectionEvent.AttackDetectionData;
                Vector3 position2 = modelTransform.TransformPoint(sphereDetectionData.Position);
                Gizmos.DrawSphere(position2, sphereDetectionData.Radius);
                break;
            case AttackDetectionType.Fan:
                AttackFanDetectionData fanDetectionData = (AttackFanDetectionData)skillAttackDetectionEvent.AttackDetectionData;
                Vector3 fanPos = modelTransform.TransformPoint(fanDetectionData.Position);
                Quaternion fanRot = modelTransform.rotation * Quaternion.Euler(fanDetectionData.Rotation);
                Mesh mesh = MeshGenerator.GenerateFanMesh(fanDetectionData.InsideRadius, fanDetectionData.Radius, fanDetectionData.Height, fanDetectionData.Angle);
                Gizmos.DrawMesh(mesh, fanPos, fanRot);
                break;
        }
        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.identity;
    }

}
#endif
