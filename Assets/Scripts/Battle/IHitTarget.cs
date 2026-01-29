using System.Collections;

public interface IHitTarget
{
    public HitTargetStatus HitTargetStatus { get; set; }
    public int ArmorLevel{ get; set; }
    public void SetDefaultHitTargetStatus();
    public void BeHit(AttackData attackData);
    public void TargetHitFreeze(float time);
}
