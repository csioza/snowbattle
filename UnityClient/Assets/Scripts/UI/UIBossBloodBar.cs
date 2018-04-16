using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIBossBloodBar : UIWindow
{
    UIBossHPBar m_hpBar = null;
    static public UIBossBloodBar Singleton
    {
        get
        {
            return UIManager.Singleton.LoadUI<UIBossBloodBar>("UI/UIBossBloodBar", UIManager.Anchor.Top);
        }
    }
    public void Register(int id)
    {
        AddPropChangedNotify((int)MVCPropertyID.enActorStartID + id, OnPropertyChanged);
    }
	public override void OnInit()
	{
		base.OnInit ();
        m_hpBar = FindChildComponent<UIBossHPBar>("BarHP");
	}
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Actor.ENPropertyChanged.enBossBloodBar)
        {
            m_hpBar.SetHp(obj as NPC);
            if ((bool)eventObj)
            {
                ShowWindow();
            }
            else
            {
                if (IsVisiable())
                {
                    HideWindow();
                }
            }
        }
    }
}
