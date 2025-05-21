using JKFrame;
using UnityEngine;
public class GameSceneManager : SingletonMono<GameSceneManager>
{
    #region �����߼�
    public bool IsTest;
    public bool IsCreateArchive;
    #endregion
    private void Start()
    {
        #region �����߼�
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
        // ��ʼ����ɫ
        PlayerManager.Instance.Init(DataManager.CustomCharacterData);
    }
}
