using JKFrame;
using UnityEngine;
public class GameSceneManager : SingletonMono<GameSceneManager>
{
    #region ²âÊÔÂß¼­
    public bool IsTest;
    public bool IsCreateArchive;
    #endregion
    private void Start()
    {
        #region ²âÊÔÂß¼­
        if (IsTest)
        {
            if (IsCreateArchive)
            {
                DataManager.CreateArchive();
            }
            else
            {
                DataManager.LoadCurrentArchive();
            }
        }
        #endregion
        Cursor.lockState = CursorLockMode.Locked;
        // ³õÊ¼»¯½ÇÉ«
        PlayerManager.Instance.Init(DataManager.CustomCharacterData);
    }
}
