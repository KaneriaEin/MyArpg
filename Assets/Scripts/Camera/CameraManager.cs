using Cinemachine;
using JKFrame;
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


    public CinemachineFreeLook freeLook;
    public CinemachineTargetGroup targetGroup;
    public CinemachineImpulseSource cameraImpulseSource;

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

    public void CameraGenerateImpulse(Vector3 vel)
    {
        if (vel == Vector3.zero) return;
        cameraImpulseSource.GenerateImpulseWithVelocity(vel);

    }

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
