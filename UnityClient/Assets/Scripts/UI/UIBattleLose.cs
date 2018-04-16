using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIBattleLose : UIWindow
{
    GameObject m_lose = null;
    int m_currentCorroutineIndex = 0;

    static public UIBattleLose GetInstance()
    {
        UIBattleLose self = UIManager.Singleton.GetUIWithoutLoad<UIBattleLose>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UIBattleLose>("UI/UIBattleLose", UIManager.Anchor.Center);
        }
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enBattelSummary, OnPropertyChanged);

        m_lose = FindChild("lose");
     
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)BattleSummary.ENPropertyChanged.enFail)
        {
            ShowWindow();

            AnimationClip clip = m_lose.GetComponent<Animation>().GetClip("ui-battlelose-00");
            if (clip == null)
            {
                Debug.LogWarning("animation ui-battlelose-00 is null");
            }
            else
            {
                m_lose.GetComponent<Animation>().Play(clip.name);
                MainGame.Singleton.StartCoroutine(CoroutineClose(++m_currentCorroutineIndex, clip.length));
            }
        }
    }

    public override void OnShowWindow()
    {
        base.OnShowWindow();
    }

    public override void AttachEvent()
    {
        base.AttachEvent();
        //AddChildMouseClickEvent("BG", OnClose);
    }

    void OnClose(GameObject obj)
    {
        // 退出结算界面后才转到 主界面
        StateMainUI mainState = new StateMainUI();
        MainGame.Singleton.TranslateTo(mainState);

        HideWindow();
    }


    IEnumerator CoroutineClose(int animCoroutineIndex, float timeLength)
    {
        yield return new WaitForSeconds(timeLength);
        if (animCoroutineIndex == m_currentCorroutineIndex)
        {
            OnClose(null);
        }
    }
  
}