using System.Collections;

public interface IHitTarget
{
    public HitTargetStatus HitTargetStatus { get; set; }
    public void BeHit(AttackData attackData);
    public IEnumerator HitFreeze(float time)
    {
        yield break;
    }
}
