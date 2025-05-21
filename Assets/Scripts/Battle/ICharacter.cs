using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ICharacter : IHitTarget
{
    public Animation_Controller Animation_Controller { get; }
    public Transform ModelTransform { get; }

    public float GetAttackValue(SkillAttackDetectionEvent detectionEvent);
    public void OnSkillRotate();
    public void AddBuff(BuffConfig buffConfig, int layer);
    public void ChangeToIdleState();
    public void OnSkillMove(Vector3 deltaPosition);
    public void OnSkillRotate(Quaternion deltaRotation);
}
