using JKFrame;
using UnityEngine;

public class TestColorSelector : MonoBehaviour
{
    public GameObject Cube;
    void Start()
    {
        UI_ColorSelectorWindow colorSelectorWindow = UISystem.Show<UI_ColorSelectorWindow>();
        Cube.GetComponent<MeshRenderer>().material.color = colorSelectorWindow.GetColor();// ������ȡ
        colorSelectorWindow.Init(OnColorSelected,Color.white);    // �¼���ȡ

    }

    private void OnColorSelected(Color obj)
    {
        Cube.GetComponent<MeshRenderer>().material.color = obj;
    }

}
