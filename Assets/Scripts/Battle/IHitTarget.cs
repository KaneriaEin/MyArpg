using UnityEngine;

public interface IHitTarget
{
    public Vector3 ModelCenterPosition { get; }
    public HitTargetStatus HitTargetStatus { get; set; }
    public int ArmorLevel{ get; set; }
    public void SetDefaultHitTargetStatus();
    public void BeHit(AttackData attackData);
    public void TargetHitFreeze(float time);
}
