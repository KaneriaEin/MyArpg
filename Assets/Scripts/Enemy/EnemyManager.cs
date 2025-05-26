using JKFrame;
using UnityEngine;

public class EnemyManager : SingletonMono<EnemyManager>
{
    [SerializeField] private Enemy_Controller[] enemies;
    public void Init()
    {
        CharacterConfig characterConfig;
        // EnemyManager������������enemyController�ĳ�ʼ��
        for (int i = 0; i < enemies.Length; i++)
        {
            characterConfig = ResSystem.LoadAsset<CharacterConfig>(enemies[i].characterConfigName);
            enemies[i].Init(characterConfig);
        }
    }
}
