using Cinemachine;
using JKFrame;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : SingletonMono<CameraManager>
{
    public Transform playerTransform;
    public Transform targetTransform;
    public Image lockDot;  //锁定圆点UI
    public Vector3 lockOffset;  //锁定圆点UI
    public bool isLocked;  //是否锁定Flag

    // FreeLook
    public CinemachineFreeLook freeLook;
    public CinemachineTargetGroup targetGroup;
    public CinemachineImpulseSource cameraImpulseSource;

    // 运镜相关
    public CinemachineDollyCart dollyCart;
    public CinemachineVirtualCamera dollyCam;

    protected override void Awake()
    {
        base.Awake();
        lockDot.enabled = false;
        isLocked = false;
    }

    public void LockOn()
    {
        if (freeLook.LookAt == PlayerManager.Instance.Player.transform)
        {
            Vector3 modelOrigin = playerTransform.position;
            Vector3 boxCenter = modelOrigin + playerTransform.transform.forward * 6.0f;
            Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(20.0f, 20.0f, 20f), playerTransform.transform.rotation, LayerMask.GetMask("Enemy"));
            if (cols != null)
                foreach (var col in cols)
                {
                    targetTransform = col.transform;
                    targetGroup.AddMember(targetTransform, 1f,2f);
                    targetGroup.AddMember(PlayerManager.Instance.Player.transform, 1f,2f);
                    freeLook.LookAt = targetGroup.transform;
                    lockDot.enabled = true;
                    isLocked = true;
                    PlayerManager.Instance.Player.LockOnTarget(targetTransform.GetComponent<GameCharacter_Controller>());
                    break;
                }
        }
        else
        {
            PlayerManager.Instance.Player.UnLockOnTarget();
            isLocked = false;
            lockDot.enabled = false;
            targetTransform = null;
            freeLook.LookAt = PlayerManager.Instance.Player.transform;
            targetGroup.RemoveMember(targetGroup.m_Targets[0].target);
            targetGroup.RemoveMember(targetGroup.m_Targets[0].target);
        }
    }

    /// <summary>
    /// freelook归为到角色背后
    /// </summary>
    public void ResetFreeLookToPlayerBack()
    {
        // 1. 获取角色当前的前方向量（世界坐标）
        Vector3 playerForward = PlayerManager.Instance.Player.ModelTransform.forward;

        // 2. 计算这个方向在世界XZ平面上的角度（忽略Y轴）
        // Mathf.Atan2返回的是弧度，需要转换为角度
        float targetAngle = Mathf.Atan2(playerForward.x, playerForward.z) * Mathf.Rad2Deg;

        // 3. 将FreeLook相机的角度设置为这个角度
        freeLook.m_XAxis.Value = targetAngle;
        freeLook.m_YAxis.Value = 0.5f;
    }

    /// <summary>
    /// 相机震动
    /// </summary>
    public void CameraGenerateImpulse(Vector3 vel)
    {
        if (vel == Vector3.zero) return;
        cameraImpulseSource.GenerateImpulseWithVelocity(vel);

    }

    #region 聚焦相关
    public void CameraFOVZoomIn(int deltaFov, float speed)
    {
        if (deltaFov == 0) return;
        if (speed <= 0) return;
        // 利用协程，period时间内 缩小deltaFov
        MonoSystem.Start_Coroutine(SetCameraFov(-deltaFov, speed));
    }

    public void CameraFOVZoomInForSeconds(int deltaFov, float speed, float seconds)
    {
        if (deltaFov == 0) return;
        if (speed <= 0) return;
        // 利用协程，period时间内 缩小deltaFov, 一段时间过后恢复原状
        MonoSystem.Start_Coroutine(SetCameraFovForSeconds(-deltaFov, speed, seconds));
    }

    public void CameraFOVZoomOut(int deltaFov, float speed)
    {
        if (deltaFov == 0) return;
        if (speed <= 0) return;
        // 利用协程，period时间内 放大deltaFov
        MonoSystem.Start_Coroutine(SetCameraFov(deltaFov, speed));
    }

    public void CameraFOVZoomOutForSeconds(int deltaFov, float speed, float seconds)
    {
        if (deltaFov == 0) return;
        if (speed <= 0) return;
        // 利用协程，period时间内 放大deltaFov, 一段时间过后恢复原状
        MonoSystem.Start_Coroutine(SetCameraFovForSeconds(deltaFov, speed, seconds));
    }

    private IEnumerator SetCameraFov(int deltaFov, float speed)
    {
        float oldFov = freeLook.m_Lens.FieldOfView;
        if (deltaFov > 0)
        {
            while (freeLook.m_Lens.FieldOfView < oldFov + deltaFov)
            {
                freeLook.m_Lens.FieldOfView = Mathf.Clamp(freeLook.m_Lens.FieldOfView + Time.deltaTime * speed, freeLook.m_Lens.FieldOfView + Time.deltaTime * speed, oldFov + deltaFov);
                // Test Debug.Log($">0fov={freeLook.m_Lens.FieldOfView}");
                yield return null;
            }
        }
        else if (deltaFov < 0)
        {
            while (freeLook.m_Lens.FieldOfView > oldFov + deltaFov)
            {
                freeLook.m_Lens.FieldOfView = Mathf.Clamp(freeLook.m_Lens.FieldOfView - Time.deltaTime * speed, oldFov + deltaFov, freeLook.m_Lens.FieldOfView - Time.deltaTime * speed);
                // Test Debug.Log($"<0fov={freeLook.m_Lens.FieldOfView}");
                yield return null;
            }
        }
    }

    private IEnumerator SetCameraFovForSeconds(int deltaFov, float speed, float seconds)
    {
        float oldFov = freeLook.m_Lens.FieldOfView;
        if (deltaFov > 0)
        {
            while (freeLook.m_Lens.FieldOfView < oldFov + deltaFov)
            {
                freeLook.m_Lens.FieldOfView = Mathf.Clamp(freeLook.m_Lens.FieldOfView + Time.deltaTime * speed, freeLook.m_Lens.FieldOfView + Time.deltaTime * speed, oldFov + deltaFov);
                yield return null;
            }
            yield return new WaitForSeconds(seconds);
            while (freeLook.m_Lens.FieldOfView > oldFov)
            {
                freeLook.m_Lens.FieldOfView = Mathf.Clamp(freeLook.m_Lens.FieldOfView - Time.deltaTime * speed, oldFov, freeLook.m_Lens.FieldOfView - Time.deltaTime * speed);
                yield return null;
            }
        }
        else if (deltaFov < 0)
        {
            while (freeLook.m_Lens.FieldOfView > oldFov + deltaFov)
            {
                freeLook.m_Lens.FieldOfView = Mathf.Clamp(freeLook.m_Lens.FieldOfView - Time.deltaTime * speed, oldFov + deltaFov, freeLook.m_Lens.FieldOfView - Time.deltaTime * speed);
                yield return null;
            }
            yield return new WaitForSeconds(seconds);
            while (freeLook.m_Lens.FieldOfView < oldFov)
            {
                freeLook.m_Lens.FieldOfView = Mathf.Clamp(freeLook.m_Lens.FieldOfView + Time.deltaTime * speed, freeLook.m_Lens.FieldOfView + Time.deltaTime * speed, oldFov);
                yield return null;
            }
        }
    }
    #endregion

    #region 运镜相关
    [ShowInInspector] private float CartSpeed;  // cart的移动速度
    /// <summary>
    /// 设定路径
    /// </summary>
    public void DollySetPath(CinemachineSmoothPath track)
    {
        dollyCart.m_Path = track;
    }

    public void DollySetSpeed(float speed)
    {
        CartSpeed = speed;
    }

    public void DollyStart(Transform lookAtTarget)
    {
        dollyCam.LookAt = lookAtTarget;
        dollyCam.Priority = 20;
        dollyCart.m_Position = 0;
    }

    public void DollyMoveUpdate()
    {
        dollyCart.m_Position += CartSpeed;
    }

    public void DollyStop()
    {
        ResetFreeLookToPlayerBack();
        dollyCam.Priority = 0;
        dollyCam.LookAt = null;
        dollyCart.m_Path = null;
        dollyCart.m_Position = 0;
    }
    #endregion

    private void Update()
    {
        #region 锁定标志
        if (isLocked)
        {
            lockDot.transform.position = Camera.main.WorldToScreenPoint(targetTransform.position + lockOffset);
        }
        #endregion

    }

}
