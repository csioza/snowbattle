using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIOperateEvolution : UIWindow
{


    static public UIOperateEvolution Create()
    {
        UIOperateEvolution self = UIManager.Singleton.GetUIWithoutLoad<UIOperateEvolution>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIOperateEvolution>("UI/UIOperateEvolution", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();



    }

    public override void AttachEvent()
    {
        base.AttachEvent();

        AddChildMouseClickEvent("EvolutionDataCancel", OnEvolutionDataCancel);

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

  
     //  点击响应
    public void OnClickItem(UICardItem item, ENSortType sortType)
    {
        CardEvolution.Singleton.OnShowCardEvolution(item.m_param.m_guid);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardEvolution);
    }

    public bool OnSortCondion(CSItem item)
    {

        // 如果不能进行进化 则不显示
        if (!item.IsEvlotion())
        {
            return true;
        }

        return false;
    }

    // 进化的基本卡牌 取消
    void OnEvolutionDataCancel(GameObject obj)
    {
        CardEvolution.Singleton.OnShowCardEvolution(CardEvolution.Singleton.m_curEvolutionGuid);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardEvolution);
    }

}
