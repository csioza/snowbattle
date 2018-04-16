using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UITipMessageBox : UIWindow
{
    Animation m_anim;
    private float m_clipTime = 0;
    float m_fStartTime = float.MaxValue;
    UIPanel m_panel = null;
    static public UITipMessageBox GetInstance()
    {
        UITipMessageBox self = UIManager.Singleton.GetUIWithoutLoad<UITipMessageBox>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UITipMessageBox>("UI/UITipMessageBox", UIManager.Anchor.Center);
        return self;
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enTipMessageBox, OnPropertyChanged);
        m_anim = WindowRoot.GetComponent<Animation>();
        m_panel = WindowRoot.GetComponent<UIPanel>();
        m_clipTime = m_anim.GetClip("TipMessageBox").length;
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)TipMessageBox.ENPropertyChanged.enShowTipMessageBox)
        {
            ShowWindow();
            m_panel.alpha = 1;
            StartCoroutine(m_anim.Play("TipMessageBox", true, null));   
            m_fStartTime = Time.realtimeSinceStartup;
            //Debug.Log("CardBag.Singleton.GetCardList().Count:" + CardBag.Singleton.GetCardList().Count);
        }
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
    }

    public override void OnUpdate()
    {
        float fDuration = Time.realtimeSinceStartup - m_fStartTime;
        if (fDuration >= m_clipTime)
        {
            m_fStartTime = float.MaxValue;
            HideWindow();
        }
        base.OnUpdate();
    }
}
