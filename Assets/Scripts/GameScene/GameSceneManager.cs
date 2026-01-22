using JKFrame;
using UnityEngine;
public class GameSceneManager : SingletonMono<GameSceneManager>
{
    #region ≤‚ ‘¬ﬂº≠
    public bool IsTest;
    public bool IsCreateArchive;
    #endregion
    private void Start()
    {
        #region ≤‚ ‘¬ﬂº≠
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
        CameraManager.Instance.Init();
        Cursor.lockState = CursorLockMode.Locked;
        TimeManager.Instance.Init();
        PostProcessingManager.Instance.Init();
        BattleEventManager.Instance.Init();
        // ≥ı ºªØΩ«…´
        PlayerManager.Instance.Init(DataManager.CustomCharacterData);
        EnemyManager.Instance.Init();
        CombatManager.Instance.Init();
        UISystem.Show<UI_PlayerStatus>();
    }

    private void Update()
    {
        // Debug.Log($"œ÷‘⁄“ª÷°{Time.deltaTime}√Î");
    }
}
