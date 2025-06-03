using Cinemachine;
using JKFrame;
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
