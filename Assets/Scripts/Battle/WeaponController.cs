using System;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Collider detectionCollider;
    private LayerMask attackDetectionLayer;
    private Func<IHitTarget, AttackData, bool> onDetection;
    private AttackData attackData;
    public void Init(LayerMask attackDetectionLayer, Func<IHitTarget, AttackData, bool> onDetection)
    {
        detectionCollider.enabled = false;
        this.attackDetectionLayer = attackDetectionLayer;
        this.onDetection = onDetection;
    }

    public void StartDetection(AttackData attackData)
    {
        detectionCollider.enabled = true;
        this.attackData = attackData;
    }

    public void StopDetection()
    {
        detectionCollider.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        // 判断是否在LayerMask的范围内
        if((attackDetectionLayer & 1 << other.gameObject.layer) > 0)
        {
            IHitTarget hitTarget = other.GetComponentInChildren<IHitTarget>();
            if(hitTarget != null)
            {
                if (hitTarget.HitTargetStatus == HitTargetStatus.Invincibility) return;
                attackData.hitPoint = other.ClosestPoint(transform.position);
                onDetection?.Invoke(hitTarget, attackData);
            }
        }
    }
}
