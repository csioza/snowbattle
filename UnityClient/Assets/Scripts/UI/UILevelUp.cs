using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UILevelUp : UIWindow
{
    UILabel m_level = null;
    UILabel m_info  = null;

    static public UILevelUp GetInstance()
    {
        UILevelUp self = UIManager.Singleton.GetUIWithoutLoad<UILevelUp>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UILevelUp>("UI/UILevelUp", UIManager.Anchor.Center);
        }
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enLeveUp, OnPropertyChanged);

        m_level = FindChildComponent<UILabel>("Level");
        m_info  = FindChildComponent<UILabel>("Info");
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)LevelUp.ENPropertyChanged.enShow)
        {
            OnLevelUp();
        }
    }
    public override void OnShowWindow()
    {
        base.OnShowWindow();
    }
    public override void AttachEvent()
    {
        base.AttachEvent();

        AddChildMouseClickEvent("Button", OnClick);
    }

    void OnLevelUp()
    {
        // 等级没变 
        if (LevelUp.Singleton.m_oldLevel == User.Singleton.GetLevel())
        {
            return;
        }

        ShowWindow();

        UpdateInfo();
    }

    void UpdateInfo()
    {
        m_level.text    = string.Format(Localization.Get("LevelChange"), User.Singleton.GetLevel());

        PlayerAttrInfo playerAttrInfo = GameTable.playerAttrTableAsset.LookUp(LevelUp.Singleton.m_oldLevel);
        if (null == playerAttrInfo)
        {
            return ;
        }

        string str      = "";

        // 体力上限变化
        int oldMax      = playerAttrInfo.m_stamina; 
        int curMax      = User.Singleton.GetMaxStamina();

        if (oldMax != curMax)
        {
            str         = str + string.Format(Localization.Get("StaminaChange"), curMax)+"\n";
        }

        // 军资上限变化
        oldMax          = playerAttrInfo.m_energy;
        curMax          = User.Singleton.GetMaxEnergy();

        if (oldMax != curMax)
        {
            str         = str + string.Format(Localization.Get("EnergyChange"), curMax) + "\n";
        }

        // 队伍栏变化
        if (MainButtonList.Singleton.IsTeamMaxChange())
        {
            str         = str + string.Format(Localization.Get("TeamNumChange"), User.Singleton.GetTeamMax()) + "\n"; ;
        }

        // 统帅力上限变化
        oldMax          = playerAttrInfo.m_leaderShip;
        curMax          = User.Singleton.GetLeadership();

        if (oldMax != curMax)
        {
            str = str + string.Format(Localization.Get("LeaderShipChange"), curMax) + "\n";
        }

        m_info.text     = str;
    }

        // 点击
    public void OnClick(GameObject obj)
    {
        LevelUp.Singleton.m_oldLevel = User.Singleton.GetLevel();
        HideWindow();

        // 如果有技能升级 则显示 技能升级界面
        if (LevelUp.Singleton.m_skillList.Count>0)
        {
            LevelUp.Singleton.ShowSkill();
        }
        else
        {
            // 如果没有技能升级 则播放 结算第三阶段的动画
            UIBattleSummary.GetInstance().PlayStageAnimation(UIBattleSummary.ENSTAGE.enStage3);
        }
      
    }
}