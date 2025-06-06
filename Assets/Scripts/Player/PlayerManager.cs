using JKFrame;
using UnityEngine;

public class PlayerManager : SingletonMono<PlayerManager>
{
    [SerializeField] private GameCharacter_Controller player;
    public GameCharacter_Controller Player { get { return player; } }
    public void Init(CustomCharacterData customCharacterData)
    {
        // 根据不同的职业，获取不同的角色配置
        //CharacterConfig characterConfig = ResSystem.LoadAsset<CharacterConfig>(customCharacterData.ProfessionType.ToString() + "Config");
        CharacterConfig characterConfig = ResSystem.LoadAsset<CharacterConfig>("WhiteManConfig");
        player.Init(characterConfig);
    }
}
