using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UISkillLevelUp : UIWindow
{
    GameObject m_info = null;
    GameObject m_icon = null;

    

    static public UISkillLevelUp GetInstance()
    {
        UISkillLevelUp self = UIManager.Singleton.GetUIWithoutLoad<UISkillLevelUp>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UISkillLevelUp>("UI/UISkillLevelUp", UIManager.Anchor.Center);
        }
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enLeveUp, OnPropertyChanged);

        m_info = FindChild("Info");
        m_icon = FindChild("SkillIcon");
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)LevelUp.ENPropertyChanged.enShowSkill)
        {
            ShowWindow();
            OnSkillLevelUp();
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

        // 点击
    public void OnClick(GameObject obj)
    {
        HideWindow();

        if (LevelUp.Singleton.m_skillList.Count != 0)
        {
            LevelUp.Singleton.m_skillList.Remove(LevelUp.Singleton.m_skillList[0]);

            if (LevelUp.Singleton.m_skillList.Count == 0)
            {
                // 升级时处于什么状态 
                switch ( LevelUp.Singleton.m_curState )
                {
                    case LevelUp.LevelState.enNone:
                    case LevelUp.LevelState.enCardUpdate:
                        {
                            break;
                        }
                    case LevelUp.LevelState.enSummary:
                        {
                            UIBattleSummary.GetInstance().PlayStageAnimation(UIBattleSummary.ENSTAGE.enStage3);
                            break;
                        }
                }
                
            }
            else
            {
                ShowWindow();
                OnSkillLevelUp();
            }
        }
    }

    void OnSkillLevelUp()
    {
        if ( LevelUp.Singleton.m_skillList.Count == 0 )
        {
            return;
        }

        SkillLevelUpInfo info   = LevelUp.Singleton.m_skillList[0];
        int skillId             = info.m_skillId;

        SkillInfo skiillInfo    = GameTable.SkillTableAsset.Lookup(skillId);

        if (null == skiillInfo)
        {
            Debug.LogWarning("OnSkillLevelUp skiillInfo == null skillId:"+skillId);
            return;
        }

       
        IconInfomation iconInfo = GameTable.IconInfoTableAsset.Lookup(skiillInfo.Icon);


        m_icon.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);

        string str = string.Format(Localization.Get("SkillLevelUp"), skiillInfo.Name, info.m_skillLevel);

        m_info.GetComponent<UILabel>().text = str;

    }
}