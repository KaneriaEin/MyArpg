using JKFrame;
using System;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy_Controller : MonoBehaviour
{
    [SerializeField] private GameCharacter_Controller gameCharacter;
    [SerializeField] public string characterConfigName;
    private Action<string> onDieAction;

    public void Init(CharacterConfig characterConfig, Action<string> dieAction = null)
    {
        gameCharacter.Init(characterConfig);
        gameCharacter.OnDieAction += dieAction;
        gameCharacter.OnDieAction += DestroyEnemy;
    }

    public void DestroyEnemy(string name)
    {
        gameCharacter.UnLockOnTarget();
        if((GameCharacter_Controller)PlayerManager.Instance.Player.Target == gameCharacter)
        {
            CameraManager.Instance.LockOn();
        }
        gameCharacter.OnDieAction = null;
        EnemyManager.Instance.RemoveEnemy(this);
    }

}
