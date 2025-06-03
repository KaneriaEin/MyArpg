using UnityEngine;

public class Enemy_Controller : MonoBehaviour
{
    [SerializeField] private GameCharacter_Controller gameCharacter;
    [SerializeField] public string characterConfigName;

    public void Init(CharacterConfig characterConfig)
    {
        gameCharacter.Init(characterConfig);
    }
}
