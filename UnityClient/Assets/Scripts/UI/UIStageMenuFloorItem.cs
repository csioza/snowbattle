using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIStageMenuFloorItem : UIWindow
{
    public UILabel m_require                = null;
    public UILabel m_stageName              = null;
    public UILabel m_tag                    = null;
    UILabel m_cost                          = null;
    UITexture m_pic                         = null;

    static public UIStageMenuFloorItem Create()
    {
        UIStageMenuFloorItem self = UIManager.Singleton.LoadUI<UIStageMenuFloorItem>("UI/UIStageMenuFloorItem", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();

        m_require       = FindChildComponent<UILabel>("Require");
        m_stageName     = FindChildComponent<UILabel>("StageName");
        m_tag           = FindChildComponent<UILabel>("tag");
        m_cost          = FindChildComponent<UILabel>("Cost");
        m_pic           = FindChildComponent<UITexture>("Pic");

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

    public void Update(int floorId )
    {
         FloorInfo floorInfo = GameTable.FloorInfoTableAsset.LookUp(floorId);

        if (null == floorInfo)
        {
            return;
        }

        m_require.text      = Localization.Get("RequireLevel") + floorInfo.m_requireLevel;
        m_stageName.text    = "" + floorInfo.m_name;
        m_tag.text          = floorInfo.m_difficulities;
        m_cost.text         = string.Format(Localization.Get("StaminaCost"), floorInfo.m_cost);


        IconInfomation iconInfo = GameTable.IconInfoTableAsset.Lookup(floorInfo.m_iconId);
        if (null != iconInfo)
        {
            m_pic.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
        }
    }
}
