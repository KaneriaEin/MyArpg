using JKFrame;
using UnityEngine;
using UnityEngine.UI;

[UIWindowData(nameof(UI_MenuSceneMenuWindow), false, nameof(UI_MenuSceneMenuWindow), 1)]
public class UI_MenuSceneMenuWindow : UI_WindowBase
{
    [SerializeField] Button continueButton;
    [SerializeField] Button startButton;
    [SerializeField] Button quitButton;
    public override void Init()
    {
        continueButton.onClick.AddListener(ContinueButtonClick);
        startButton.onClick.AddListener(StartButtonClick);
        quitButton.onClick.AddListener(QuitButtonClick);
        // 如果当前没有存档，应该隐藏继续游戏按钮
        if (!DataManager.HaveArchive)
        {
            continueButton.gameObject.SetActive(false);
        }
    }

    private void ContinueButtonClick()
    {
        UISystem.Close<UI_MenuSceneMenuWindow>();
        // 使用当前存档进行游戏
        GameManager.Instance.UseCurrentArchiveAndEnterGame();
    }
    private void StartButtonClick()
    {
        UISystem.Close<UI_MenuSceneMenuWindow>();
        // 创建存档进行游戏 -》 进入自定义角色场景
        GameManager.Instance.CreateNewArchiveAndEnterGame();
    }

    public override void OnClose()
    {
        base.OnClose();
        // 释放自身资源
        ResSystem.UnloadInstance(gameObject);
    }

    private void QuitButtonClick()
    {
        Application.Quit();
    }

}
