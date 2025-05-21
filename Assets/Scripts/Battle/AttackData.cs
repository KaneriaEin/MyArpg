using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct AttackData
{
    public SkillAttackDetectionEvent detectionEvent;
    public ICharacter source;
    public Vector3 hitPoint;
    public float attackValue;
}
