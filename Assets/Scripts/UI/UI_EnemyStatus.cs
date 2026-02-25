using JKFrame;
using UnityEngine;
using UnityEngine.UI;

[UIWindowData(nameof(UI_EnemyStatus),false, nameof(UI_EnemyStatus), 2)]
public class UI_EnemyStatus : UI_WindowBase
{
    // HP条
    [SerializeField] Slider hpSlider;
    // MP条
    [SerializeField] Slider stunSlider;

    [SerializeField] Image stunFillImage;
    [SerializeField] Color stunNormal;
    [SerializeField] Color stunRecovering;

    public override void Init()
    {
        JKFrame.EventSystem.AddEventListener<float>("OnNodachiHPChanged", HPSliderChange);
        JKFrame.EventSystem.AddEventListener<float>("OnNodachiStunChanged", StunSliderChange);
        JKFrame.EventSystem.AddEventListener<bool>("OnNodachiStunInStun", StunSliderInStun);

        hpSlider.maxValue = EnemyManager.Instance.firstEnemy.CharacterProperties.maxHp.Total;
        hpSlider.value = EnemyManager.Instance.firstEnemy.CharacterProperties.currentHP;
        stunSlider.maxValue = EnemyManager.Instance.firstEnemy.CharacterProperties.stunGauge.Total;
        stunSlider.value = EnemyManager.Instance.firstEnemy.CharacterProperties.currentStun;
    }

    private void HPSliderChange(float newhp)
    {
        hpSlider.value = newhp;
    }

    private void StunSliderChange(float newStun)
    {
        stunSlider.value = newStun;
    }

    private void StunSliderInStun(bool instun)
    {
        if (instun) { stunFillImage.color = stunRecovering; } else { stunFillImage.color = stunNormal; }
    }

    public override void OnClose()
    {
        base.OnClose();
        JKFrame.EventSystem.RemoveEventListener<float>("OnNodachiHPChanged", HPSliderChange);
        JKFrame.EventSystem.RemoveEventListener<float>("OnNodachiStunChanged", StunSliderChange);
        JKFrame.EventSystem.RemoveEventListener<bool>("OnNodachiStunInStun", StunSliderInStun);
        // 释放自身资源
        ResSystem.UnloadInstance(gameObject);
    }
}
