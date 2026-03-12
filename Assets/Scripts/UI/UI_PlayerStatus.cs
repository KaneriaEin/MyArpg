using JKFrame;
using System;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("SP")]
    // sp资源条
    [SerializeField] Slider spSlider;
    [SerializeField] GameObject[] spLines;

    [Header("ULT")]
    // 大招条
    [SerializeField] Slider ultSlider;
    [SerializeField] Image ultFill;
    [SerializeField] Color ultColorNotEnough;
    [SerializeField] Color ultColorEnough;

    public override void Init()
    {
        JKFrame.EventSystem.AddEventListener<float>("OnPlayerHPChanged", HPSliderChange);
        JKFrame.EventSystem.AddEventListener<float>("OnPlayerMPChanged", MPSliderChange);
        JKFrame.EventSystem.AddEventListener<float>("OnPlayerSPChanged", SPSliderChange);
        JKFrame.EventSystem.AddEventListener<float>("OnPlayerULTChanged", ULTSliderChange);

        //hpSlider.value = PlayerManager.Instance.Player.CharacterProperties.currentHP;
        //mpSlider.value = PlayerManager.Instance.Player.CharacterProperties.currentMP;
        //spSlider.value = PlayerManager.Instance.Player.CharacterProperties.currentSP;
        //ultSlider.value = PlayerManager.Instance.Player.CharacterProperties.currentULT;
        HPSliderChange(PlayerManager.Instance.Player.CharacterProperties.currentHP);
        MPSliderChange(PlayerManager.Instance.Player.CharacterProperties.currentMP);
        SPSliderChange(PlayerManager.Instance.Player.CharacterProperties.currentSP);
        ULTSliderChange(PlayerManager.Instance.Player.CharacterProperties.currentULT);
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

    private void SPSliderChange(float newsp)
    {
        spSlider.value = (int)newsp / 10 * 10;
        for(int i = 1; i < 10; i++)
        {
            if (i <= newsp / 10 - 1) { spLines[i].SetActive(true); }
            else { spLines[i].SetActive(false); }
        }
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

    public override void OnClose()
    {
        base.OnClose();
        JKFrame.EventSystem.RemoveEventListener<float>("OnPlayerHPChanged", HPSliderChange);
        JKFrame.EventSystem.RemoveEventListener<float>("OnPlayerMPChanged", MPSliderChange);
        JKFrame.EventSystem.RemoveEventListener<float>("OnPlayerSPChanged", SPSliderChange);
        JKFrame.EventSystem.RemoveEventListener<float>("OnPlayerULTChanged", ULTSliderChange);
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
    }
}
