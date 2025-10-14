using UnityEngine;

public interface ICharacter : IHitTarget
{
    public Animation_Controller Animation_Controller { get; }
    public Transform ModelTransform { get; }
    public ICharacter Target { get; }
    public CommandControllerBase CommandController { get; }
    public float GetAttackValue(SkillAttackDetectionEvent detectionEvent);
    public void OnSkillRotate();
    public void AddBuff(BuffConfig buffConfig, int layer);
    public void ChangeToIdleState();
    public void OnSkillMove(Vector3 deltaPosition);
    public void OnSkillRotate(Quaternion deltaRotation);
    public void CharacterBattleEvent(CharacterBattleEventType eventType, CharacterBattleEventArg arg);
}
