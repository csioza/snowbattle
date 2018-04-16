using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIStageMenuStageItem : UIWindow
{
    public UILabel m_require                = null;
    public UILabel m_stageName              = null;
    public UILabel m_tag                    = null;

    static public UIStageMenuStageItem Create()
    {
        UIStageMenuStageItem self = UIManager.Singleton.LoadUI<UIStageMenuStageItem>("UI/UIStageMenuStageItem", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();

        m_require   = FindChildComponent<UILabel>("Require");
        m_stageName = FindChildComponent<UILabel>("StageName");
        m_tag       = FindChildComponent<UILabel>("tag");

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

    public void Update(int stageID )
    {
        StageDetailInfo stageInfo = GameTable.StageInfoTableAsset.LookUp(stageID);

        if (null == stageInfo)
        {
            return;
        }

        m_require.text      = Localization.Get("RequireLevel") + stageInfo.m_requireLevel;
        m_stageName.text    = "" + stageInfo.m_name;
        string strTag       = Localization.Get("New");
        //if (ActorManager.Singleton.MainActor.m_curStageId > stageId)
        {
            strTag = Localization.Get("Clear");
        }
        m_tag.text          = strTag;
    }
}
