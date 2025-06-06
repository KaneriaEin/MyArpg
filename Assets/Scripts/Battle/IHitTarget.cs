using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitTarget
{
    public HitTargetStatus HitTargetStatus { get; set; }
    public void BeHit(AttackData attackData);
}
