using Sirenix.OdinInspector;
using UnityEngine;

public class WhiteManSkillBrain : GameCharacter_SkillBrainBase
{
    public const string X_Key = "X";
    public const string XX_Key = "XX";
    public const string XXX_Key = "XXX";
    public const string XXXX_Key = "XXXX";
    public const string XXXXHold_Key = "XXXXHold";
    public const string XXXXHoldSP_Key = "XXXXHoldSP";
    public const string XXY_Key = "XXY";
    public const string XXYY_Key = "XXYY";
    public const string XXYYY_Key = "XXYYY";
    public const string Y_Key = "Y";
    public const string YY_Key = "YY";
    public const string YYY_Key = "YYY";

    public const string SPSkillKey = "SPSkillKey";


    // 角色技能相关变量 连击数
    [ShowInInspector] public int wb_combo = 0;
    public int WB_Combo { get { return wb_combo; } }

    public override void Init(GameCharacter_Controller gameCharacter)
    {
        base.Init(gameCharacter);
        wb_combo = 0;
        AddorUpdateShareData(SPSkillKey, false);
    }

    /// <summary>
    /// 找目前出招表中可以发生的招数，hold技
    /// </summary>
    /// <param name="keyName">Name in SkillClip</param>
    /// <param name="isHeavy">新指令是否为Y</param>
    /// <returns></returns>
    public bool GetNextSkillClipKey(out string keyName, bool isHeavy)
    {
        bool flag = false;
        keyName = null;
        if (isHeavy)
        {
            TryGetSkillShareData(Y_Key, out flag); if (flag) { keyName = Y_Key; return flag; }
            TryGetSkillShareData(YY_Key, out flag); if (flag) { keyName = YY_Key; return flag; }
            TryGetSkillShareData(YYY_Key, out flag); if (flag) { keyName = YYY_Key; return flag; }
            TryGetSkillShareData(XXY_Key, out flag); if (flag) { keyName = XXY_Key; return flag; }
            TryGetSkillShareData(XXYY_Key, out flag); if (flag) { keyName = XXYY_Key; return flag; }
            TryGetSkillShareData(XXYYY_Key, out flag); if (flag) { keyName = XXYYY_Key; return flag; }
        }
        else
        {
            TryGetSkillShareData(X_Key, out flag); if (flag) { keyName = X_Key; return flag; }
            TryGetSkillShareData(XX_Key, out flag); if (flag) { keyName = XX_Key; return flag; }
            TryGetSkillShareData(XXX_Key, out flag); if (flag) { keyName = XXX_Key; return flag; }
            TryGetSkillShareData(XXXX_Key, out flag); if (flag) { keyName = XXXX_Key; return flag; }
        }
        return false;
    }

    public bool GetNextSkillClipKeyHold(out string keyName, bool isHeavy)
    {
        bool flag = false;
        keyName = null;
        if (isHeavy)
        {
        }
        else
        {
            TryGetSkillShareData(XXXXHoldSP_Key, out flag); if (flag) { keyName = XXXXHoldSP_Key; return flag; }
            TryGetSkillShareData(XXXXHold_Key, out flag); if (flag) { keyName = XXXXHold_Key; return flag; }
        }
        return false;
    }

    public void SetNextSkillClipKey(SkillClip skillClip)
    {
        ClearNextSkillClipKey();
        if (skillClip == null) return;
        if (skillClip.FollowUp.Length > 0)
        {
            for(int i = 0; i < skillClip.FollowUp.Length; i++)
            {
                AddorUpdateShareData(skillClip.FollowUp[i], true);
            }
        }
    }

    public void ClearNextSkillClipKey()
    {
        AddorUpdateShareData(X_Key, false);
        AddorUpdateShareData(XX_Key, false);
        AddorUpdateShareData(XXX_Key, false);
        AddorUpdateShareData(XXXX_Key, false);
        AddorUpdateShareData(XXXXHold_Key, false);
        AddorUpdateShareData(XXXXHoldSP_Key, false);
        AddorUpdateShareData(Y_Key, false);
        AddorUpdateShareData(YY_Key, false);
        AddorUpdateShareData(YYY_Key, false);
        AddorUpdateShareData(XXY_Key, false);
        AddorUpdateShareData(XXYY_Key, false);
        AddorUpdateShareData(XXYYY_Key, false);
    }

    public void Add_WBCombo(int c)
    {
        wb_combo = Mathf.Clamp(wb_combo + c, 0, 10);
        JKFrame.EventSystem.EventTrigger<int>("OnWhiteManComboChanged", wb_combo);
    }
}
