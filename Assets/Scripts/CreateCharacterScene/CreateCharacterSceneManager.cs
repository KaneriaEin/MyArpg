using JKFrame;
public class CreateCharacterSceneManager : SingletonMono<CreateCharacterSceneManager>
{
    private void Start()
    {
        // ��ʼ����ɫ������
        CharacterCreator.Instance.Init();
        // ��ʾ������ɫ��������
        UISystem.Show<UI_CreateCharacterWindow>();
    }
}
