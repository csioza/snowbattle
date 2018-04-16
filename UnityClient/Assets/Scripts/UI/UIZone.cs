using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIZone : UIWindow
{
    UILabel m_zoneName = null;
    int m_currentCorroutineDragOverIndex = 0;

    enum ShowWinType
    {
        enTeam,
        enMain,
    }

    static public UIZone GetInstance()
    {
        UIZone self = UIManager.Singleton.GetUIWithoutLoad<UIZone>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UIZone>("UI/UIZone", UIManager.Anchor.Center);
        }
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enZone, OnPropertyChanged);

        m_zoneName  = FindChildComponent<UILabel>("ZoneName");

    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Zone.ENPropertyChanged.enShow)
        {
//             if (MainUIManager.Singleton.ChangeUI(this))
//             {
//                 MainButtonList.Singleton.m_curShowType = MainButtonList.SHOWWNDTYPE.ENMain;
//                 ShowWindow();
//             }
        }
        else if (eventType == (int)Zone.ENPropertyChanged.enHide)
        {
            HideWindow();
        }
    }

    public override void OnShowWindow()
    {
        base.OnShowWindow();
        MainButtonList.Singleton.m_curShowType = MainButtonList.SHOWWNDTYPE.ENMain;
    }
    public override void AttachEvent()
    {
        base.AttachEvent();


        ZoneGameInfo[] zoneInfoList = WindowRoot.GetComponentsInChildren<ZoneGameInfo>();
        for (int i = 0; i < zoneInfoList.Length; ++i)
        {
            ZoneGameInfo zoneInfo = zoneInfoList[i];

            EventDelegate.Add(FindChild(zoneInfo.transform.name).GetComponent<UIEventTrigger>().onDragOut, OnHideName);

            AddChildMouseClickEvent(zoneInfo.transform.name, OnClick);
            //EventDelegate.Add(FindChild(zoneInfo.transform.name).GetComponent<UIEventTrigger>().onClick, OnClick);

            EventDelegate.Add(FindChild(zoneInfo.transform.name).GetComponent<UIEventTrigger>().onDragOver, OnChangeName);

            EventDelegate.Add(FindChild(zoneInfo.transform.name).GetComponent<UIEventTrigger>().onPress, OnPress);

            Debug.Log("zoneInfo.transform.name:" + zoneInfo.transform.name);
        }

    }

    // 点击zone按钮 进入stage
    public void OnClick(GameObject obj)
    {
        ZoneGameInfo parma = obj.GetComponent<ZoneGameInfo>();

        if ( null == parma )
        {
            return;
        }

        StageMenu.Singleton.ShowMenu(parma.m_id);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enStageMenu);

        MainMenu.Singleton.HideMain();

        OnHideName();
    }

    void OnPress()
    {
        GameObject obj = UIEventTrigger.current.gameObject;

        ZoneGameInfo parma = obj.GetComponent<ZoneGameInfo>();

        if (null == parma)
        {
            return;
        }

        ZoneTableInfo zoneInfo = GameTable.ZoneInfoTableAsset.LookUp(parma.m_id);
        if (null != zoneInfo)
        {
            m_zoneName.text = zoneInfo.m_name;
        }

        MainGame.Singleton.StartCoroutine(CoroutineDragOverAnimationEnd(++m_currentCorroutineDragOverIndex));
    }

    // 隐藏名称
    void OnHideName()
    {
        m_currentCorroutineDragOverIndex++;
        m_zoneName.GetComponent<Animation>().CrossFade("ui-showzonename-01");

    }

    void OnChangeName()
    {
        GameObject obj = UIEventTrigger.current.gameObject;

        ZoneGameInfo parma = obj.GetComponent<ZoneGameInfo>();

        if (null == parma)
        {
            return;
        }
        ZoneTableInfo zoneInfo = GameTable.ZoneInfoTableAsset.LookUp(parma.m_id);
        if (null == zoneInfo)
        {
            return;
        }
        m_zoneName.text = zoneInfo.m_name;

        MainGame.Singleton.StartCoroutine(CoroutineDragOverAnimationEnd(++m_currentCorroutineDragOverIndex));
    }


    IEnumerator CoroutineDragOverAnimationEnd(int animCoroutineIndex)
    {
        yield return new WaitForSeconds(GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enZoneAnimationDelayTime).FloatTypeValue);

        if (animCoroutineIndex == m_currentCorroutineDragOverIndex)
        {
            m_zoneName.GetComponent<Animation>().CrossFade("ui-showzonename-00");
        }
    }

}