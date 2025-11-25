using UnityEngine;

public struct AttackData
{
    public SkillAttackDetectionEvent detectionEvent;
    public ICharacter source;
    public Vector3 hitPoint;
    public float attackValue;
    public float stunAttackValue;
    public SkillType attackType;
}

public enum ActionDirect
{
    None,
    Forward,
    Back,
    Left,
    Right,
}

public enum SkillType
{
    StandAttack,
    Skill,
    Dodge,
    PerfectGuard,
}

public enum HitTargetStatus
{
    None,
    Invincibility,
    Armor
}
