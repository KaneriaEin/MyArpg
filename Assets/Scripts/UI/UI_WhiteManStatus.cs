using JKFrame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[UIWindowData(nameof(UI_WhiteManStatus),false, nameof(UI_WhiteManStatus), 2)]
public class UI_WhiteManStatus : UI_WindowBase
{
    // combo显示
    [SerializeField] Text ui_combo;

    public override void Init()
    {
        JKFrame.EventSystem.AddEventListener<int>("OnWhiteManComboChanged", UI_ComboChange);

        ui_combo.text = ((WhiteManSkillBrain)PlayerManager.Instance.Player.SkillBrain).WB_Combo.ToString();
    }

    private void UI_ComboChange(int combo)
    {
        ui_combo.text = combo.ToString();
    }


    public override void OnClose()
    {
        base.OnClose();
        JKFrame.EventSystem.RemoveEventListener<int>("OnWhiteManComboChanged", UI_ComboChange);
        // 释放自身资源
        ResSystem.UnloadInstance(gameObject);
    }
}
