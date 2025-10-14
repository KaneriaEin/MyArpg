using UnityEngine;

public enum CharacterBattleEventType
{
    None = 0,
    BePerfectGuarded,
}

public class CharacterBattleEventArg
{
    public AttackData attackData;
}
