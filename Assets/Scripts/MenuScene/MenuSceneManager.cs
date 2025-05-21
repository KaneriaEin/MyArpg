using JKFrame;
public class MenuSceneManager : SingletonMono<MenuSceneManager>
{
    void Start()
    {
        UISystem.Show<UI_MenuSceneMenuWindow>();
    }
}
