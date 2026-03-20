using JKFrame;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

[UIWindowData(nameof(UI_PlayerStatus),false, nameof(UI_PlayerStatus), 2)]
public class UI_PlayerStatus : UI_WindowBase
{
    [Header("HP")]
    // HP条
    [SerializeField] Slider hpSlider;

    [Header("MP")]
    // MP条
    [SerializeField] Slider mpSlider;
    [SerializeField] Image mpFill;
    [SerializeField] Color mpColorNotEnough;
    [SerializeField] Color mpColorEnough;

    [Header("ULT")]
    // 大招条
    [SerializeField] Slider ultSlider;
    [SerializeField] Image ultFill;
    [SerializeField] Color ultColorNotEnough;
    [SerializeField] Color ultColorEnough;

    [Header("ThunderToggle")]
    // 雷气条
    [SerializeField] Image[] thunderBuffs;
    [SerializeField] Slider thunderBuffGauge;

    public override void Init()
    {
        JKFrame.EventSystem.AddEventListener<float>("OnPlayerHPChanged", HPSliderChange);
        JKFrame.EventSystem.AddEventListener<float>("OnPlayerMPChanged", MPSliderChange);
        JKFrame.EventSystem.AddEventListener<float>("OnPlayerULTChanged", ULTSliderChange);
        JKFrame.EventSystem.AddEventListener<int>("OnPlayerThunderBuffSet", ThunderToggleSet);
        JKFrame.EventSystem.AddEventListener<float>("OnPlayerThunderBuffGaugeSet", ThunderSliderSet);

        //hpSlider.value = PlayerManager.Instance.Player.CharacterProperties.currentHP;
        //mpSlider.value = PlayerManager.Instance.Player.CharacterProperties.currentMP;
        //spSlider.value = PlayerManager.Instance.Player.CharacterProperties.currentSP;
        //ultSlider.value = PlayerManager.Instance.Player.CharacterProperties.currentULT;
        HPSliderChange(PlayerManager.Instance.Player.CharacterProperties.currentHP);
        MPSliderChange(PlayerManager.Instance.Player.CharacterProperties.currentMP);
        ULTSliderChange(PlayerManager.Instance.Player.CharacterProperties.currentULT);
        ThunderToggleSet(0);
        ThunderSliderSet(0);
    }

    private void HPSliderChange(float newhp)
    {
        hpSlider.value = newhp;
    }

    private void MPSliderChange(float newmp)
    {
        mpSlider.value = newmp;
        if (mpSlider.value < 20)
            mpFill.color = mpColorNotEnough;
        else
            mpFill.color = mpColorEnough;
    }

    private void ULTSliderChange(float newult)
    {
        if(newult != 100f) { ultFill.color = ultColorNotEnough; }
        else
        {
            ultFill.color = ultColorEnough;
            if(ultSlider.value != 100f)
                AudioSystem.PlayOneShot(PlayerManager.Instance.Player.CharacterConfig.UltFullAudioClip, null, false, 0.5f, false);
        }
        ultSlider.value = newult;
    }

    private void ThunderToggleSet(int layer)
    {
        for (int i = 0; i<thunderBuffs.Length; i++)
        {
            if (layer > 0)
                thunderBuffs[i].enabled = true;
            else
                thunderBuffs[i].enabled = false;
            --layer;
        }
    }

    private void ThunderSliderSet(float gauge)
    {
        thunderBuffGauge.value = gauge;
    }

    public override void OnClose()
    {
        base.OnClose();
        JKFrame.EventSystem.RemoveEventListener<float>("OnPlayerHPChanged", HPSliderChange);
        JKFrame.EventSystem.RemoveEventListener<float>("OnPlayerMPChanged", MPSliderChange);
        JKFrame.EventSystem.RemoveEventListener<float>("OnPlayerULTChanged", ULTSliderChange);
        JKFrame.EventSystem.RemoveEventListener<int>("OnPlayerThunderBuffSet", ThunderToggleSet);
        JKFrame.EventSystem.RemoveEventListener<float>("OnPlayerThunderBuffGaugeSet", ThunderSliderSet);
        // 释放自身资源
        ResSystem.UnloadInstance(gameObject);
    }

    private void Update()
    {
        #region ULT
        if (ultSlider.value == 100f)
        {
            var c = ultFill.color;
            c.g = Mathf.PingPong(Time.frameCount * 0.1f, 1f);
            ultFill.color = c;
        }
        #endregion
        #region ThunderBuff
        var th = thunderBuffs[0].color;
        th.a = 0.58f + Mathf.PingPong(Time.time * 1f, 0.42f);
        for (int i = 0; i < thunderBuffs.Length; i++)
        {
            thunderBuffs[i].color = th;
        }
        #endregion
    }
}
