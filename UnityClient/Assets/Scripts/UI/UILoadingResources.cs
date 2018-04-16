using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

class UILoadingResources : UIWindow
{
    UISlider m_valueSlider = null;
    UILabel m_valueText = null;
    UISprite m_LoadingResourcesBG = null;
    float m_timeTemp;
    static public UILoadingResources GetInstance()
    {
        UILoadingResources self = UIManager.Singleton.GetUIWithoutLoad<UILoadingResources>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UILoadingResources>("UI/UILoadingResources", UIManager.Anchor.Center);
        return self;
    }
    public override void OnInit() 
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enLoadingResourcesProp, OnPropertyChanged);
        m_valueSlider = FindChildComponent<UISlider>("BarValue");
        m_valueText = FindChildComponent<UILabel>("valueText");
//         m_LoadingResourcesBG = FindChildComponent<UISprite>("LoadingResourcesBG");
//         UIRoot root = GameObject.FindObjectOfType<UIRoot>();
//         if (null != root)
//         {
//             float s = (float)root.activeHeight / Screen.height;
//             int height = Mathf.CeilToInt(Screen.height * s);
//             int width = Mathf.CeilToInt(Screen.width * s);
//             UIWidget widget = m_LoadingResourcesBG.GetComponent<UIWidget>();
//             UISprite sprite = widget as UISprite;
//             sprite.width = width;
//             sprite.height = height;
//         }
    }
    public override void OnUpdate() 
    {
        base.OnUpdate();
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
    }
    public override void OnShowWindow()
    {
        base.OnShowWindow();
        ResteLoadingResourcesPanel();
    }
    public void ResteLoadingResourcesPanel()
    {
        m_valueSlider.value = 0;
        m_valueText.text = "0%";
        m_timeTemp = 4.0f;
    }

    public void UpDateLoadingValue() 
    {
        m_valueSlider.value = PreResourceLoad.Singleton.m_process;

        m_valueText.text = (m_valueSlider.value * 100) + "%";
        if (1.0f <= m_valueSlider.value)
        {
            MainGame.Singleton.TranslateTo(new StateBattle());
            MainGame.Singleton.StartCoroutine(CoroutineAnimationEnd(m_timeTemp));
        }
    }

    private IEnumerator CoroutineAnimationEnd(float timeLength)
    {
        yield return new WaitForSeconds(timeLength);
        HideWindow();
    }


    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj) 
    {
        if (eventType == (int)LoadingResourcesProp.ENPropertyChanged.enShowUILoadingResources)
        {
            ResteLoadingResourcesPanel();
            ShowWindow();
        }
        else if (eventType == (int)LoadingResourcesProp.ENPropertyChanged.enUpDateLoadingValue)
        {
            UpDateLoadingValue();
        }
        else if (eventType == (int)LoadingResourcesProp.ENPropertyChanged.enHide)
        {
            HideWindow();
        }
    }
}