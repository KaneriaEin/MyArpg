using JKFrame;
using Sirenix.OdinInspector;

public enum GameCharacterType
{
    None,
    WhiteMan,
    PersonBS,
}

public enum GameCharacter_RPCService
{
    None = 0,
    RPC_PersonBS_Skill_2,
    RPC_AllService
}

public class EnemyRuntimeSharedData : ConfigBase
{
    [LabelText("Skill1 –≈∫≈¡ø")] public int Skill1_Signal;
}
