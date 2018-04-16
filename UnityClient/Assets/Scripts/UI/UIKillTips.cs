using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIKillTips : UIWindow
{

    UILabel m_tips      = null;
    UILabel m_killEnemy = null;
    UILabel m_beKilled  = null;

    float m_starTime;
    float m_curCountDownTime = 10;

    static public UIKillTips GetInstance()
    {
        UIKillTips self = UIManager.Singleton.GetUIWithoutLoad<UIKillTips>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIKillTips>("UI/UIKillTips", UIManager.Anchor.Top);
        return self;
    }
    
    public UIKillTips()
    {
        IsAutoMapJoystick = false;
    }

    public override void OnInit()
    {
        base.OnInit();

        AddPropChangedNotify((int)MVCPropertyID.enKillTips, OnPropertyChanged);

        m_tips      = FindChildComponent<UILabel>("Tips");

        m_killEnemy = FindChildComponent<UILabel>("KillEnmy");

        m_beKilled  = FindChildComponent<UILabel>("Bekilled");
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

        if (eventType == (int)KillTips.ENPropertyChanged.enUpdate)
        {
            string tips = (string)eventObj;
            Update(tips);
        }
    }


    public override void OnShowWindow()
    {
        base.OnShowWindow();
    }

    void Update(string tips)
    {
        m_tips.gameObject.SetActive(true);

        m_tips.text         = tips;

        m_killEnemy.text    = KillTips.Singleton.GetkillEnemyNum() + "";

        m_beKilled.text     = KillTips.Singleton.GetbeKilledNum() + "";

        m_starTime          = Time.time;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!m_tips.gameObject.activeInHierarchy)
        {
            return;
        }

        if (m_starTime == 0 )
        {
            return;
        }

        if (Math.Ceiling(m_curCountDownTime - (Time.time - m_starTime)) < 0)
        {
            m_tips.gameObject.SetActive(false);
            m_starTime = 0;
        }
    }
}
