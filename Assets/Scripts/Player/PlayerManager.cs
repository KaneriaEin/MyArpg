using JKFrame;
using UnityEngine;

public class PlayerManager : SingletonMono<PlayerManager>
{
    [SerializeField] private Player_Controller player;
    public void Init(CustomCharacterData customCharacterData)
    {
        // ���ݲ�ͬ��ְҵ����ȡ��ͬ�Ľ�ɫ����
        //CharacterConfig characterConfig = ResSystem.LoadAsset<CharacterConfig>(customCharacterData.ProfessionType.ToString() + "Config");
        CharacterConfig characterConfig = ResSystem.LoadAsset<CharacterConfig>("WhiteManConfig");
        player.Init(characterConfig, customCharacterData);
    }
}
