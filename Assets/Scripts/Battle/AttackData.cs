using UnityEngine;

public struct AttackData
{
    public SkillAttackDetectionEvent detectionEvent;
    public ICharacter source;
    public Vector3 hitPoint;
    public float attackValue;
    public float stunAttackValue;
    public float atkElementValue;
    public AttackElementType attackElementType;
    public SkillType attackType;
    public bool pgPunish;
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
