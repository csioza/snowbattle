using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIStageMenuTeamItem : UIWindow
{

    public GameObject m_mainModelObj        = null;
    public GameObject m_deputyModelObj      = null;
    public GameObject m_supportModelObj     = null;
    public GameObject m_comradeModelObj     = null;

    public UILabel m_mainTips               = null;
    public UILabel m_deputyTips             = null;
    public UILabel m_supportTips            = null;
    public UILabel m_comradeTips            = null;

    public GameObject m_noMainTipsLab       = null;

    static public UIStageMenuTeamItem Create()
    {
        UIStageMenuTeamItem self = UIManager.Singleton.LoadUI<UIStageMenuTeamItem>("UI/UIStageMenuTeamItem", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();

        m_mainModelObj      = FindChild("MainModel");
        m_deputyModelObj    = FindChild("DeputyModel");
        m_supportModelObj   = FindChild("SupportModel");
        m_comradeModelObj   = FindChild("ComradeModel");

        m_mainTips          = FindChildComponent<UILabel>("MainTips");
        m_deputyTips        = FindChildComponent<UILabel>("DeputyTips"); 
        m_supportTips       = FindChildComponent<UILabel>("SupportTips");
        m_comradeTips       = FindChildComponent<UILabel>("ComradeTips");

        m_noMainTipsLab     = FindChild("NoMainTips"); 

    }

    public override void AttachEvent()
    {
        base.AttachEvent();
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }
    }


    // 队伍编辑 更新
    public override void OnShowWindow()
    {
        base.OnShowWindow();
    }
}
