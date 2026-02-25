using JKFrame;
using UnityEngine;
using UnityEngine.UI;

[UIWindowData(nameof(UI_PlayerStatus),false, nameof(UI_PlayerStatus), 2)]
public class UI_PlayerStatus : UI_WindowBase
{
    // HP条
    [SerializeField] Slider hpSlider;
    // MP条
    [SerializeField] Slider mpSlider;

    public override void Init()
    {
        JKFrame.EventSystem.AddEventListener<float>("OnPlayerHPChanged", HPSliderChange);
        JKFrame.EventSystem.AddEventListener<float>("OnPlayerMPChanged", MPSliderChange);

        hpSlider.value = PlayerManager.Instance.Player.CharacterProperties.currentHP;
        mpSlider.value = PlayerManager.Instance.Player.CharacterProperties.currentMP;
    }

    private void HPSliderChange(float newhp)
    {
        hpSlider.value = newhp;
    }

    private void MPSliderChange(float newmp)
    {
        mpSlider.value = newmp;
    }

    public override void OnClose()
    {
        base.OnClose();
        JKFrame.EventSystem.RemoveEventListener<float>("OnPlayerHPChanged", HPSliderChange);
        JKFrame.EventSystem.RemoveEventListener<float>("OnPlayerMPChanged", MPSliderChange);
        // 释放自身资源
        ResSystem.UnloadInstance(gameObject);
    }
}
