using JKFrame;
using System;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy_Controller : MonoBehaviour
{
    [SerializeField] private GameCharacter_Controller gameCharacter;
    [SerializeField] public string characterConfigName;
    private Action<string> onDieAction;

    public GameCharacter_Controller GameCharacter { get { return gameCharacter; } }

    public void Init(CharacterConfig characterConfig, Action<string> dieAction = null)
    {
        gameCharacter.Init(characterConfig);
        gameCharacter.OnDieAction += DestroyEnemy;
        gameCharacter.OnDieAction += dieAction;
    }

    public void DestroyEnemy(string name)
    {
        gameCharacter.OnDieAction = null;
        EnemyManager.Instance.RemoveEnemy(this);
    }

}
