using JKFrame;
using UnityEngine;

public class PlayerManager : SingletonMono<PlayerManager>
{
    [SerializeField] private Player_Controller player;
    public void Init(CustomCharacterData customCharacterData)
    {
        // 根据不同的职业，获取不同的角色配置
        //CharacterConfig characterConfig = ResSystem.LoadAsset<CharacterConfig>(customCharacterData.ProfessionType.ToString() + "Config");
        CharacterConfig characterConfig = ResSystem.LoadAsset<CharacterConfig>("WhiteManConfig");
        player.Init(characterConfig, customCharacterData);
    }
}
