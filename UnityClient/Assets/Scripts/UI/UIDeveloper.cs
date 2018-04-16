using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIDeveloper : UIWindow
{
    UISlider m_slider = null;
        
    static public UIDeveloper GetInstance()
    {
        UIDeveloper self = UIManager.Singleton.GetUIWithoutLoad<UIDeveloper>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UIDeveloper>("UI/UIDeveloper", UIManager.Anchor.Center);
        }
        return self;
    }



    public override void OnInit()
    {
        base.OnInit();

        m_slider = FindChildComponent<UISlider>("Slider");
    }

    public override void AttachEvent()
    {
        base.AttachEvent();

        AddChildMouseClickEvent("Button", OnClick);
        AddChildMouseClickEvent("Button1", OnClick);
    }

    // 点击
    public void OnClick(GameObject obj)
    {
        HideWindow();
    }

    public override void OnShowWindow()
    {
        m_slider.value = 0;
        base.OnShowWindow();
    }
}