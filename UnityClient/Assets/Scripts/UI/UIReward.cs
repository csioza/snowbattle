using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIReward : UIWindow
{

    GameObject m_gold           = null;
    GameObject m_key            = null;
    UILabel m_goldLabel         = null;
    UILabel m_cardLabel         = null;

    float svelocitx             = 0.0f;
    bool m_goldGrow             = false;
    bool m_cardGrow             = false;

    static public UIReward GetInstance()
    {
        UIReward self = UIManager.Singleton.GetUIWithoutLoad<UIReward>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UIReward>("UI/UIReward", UIManager.Anchor.Center);
        }
        return self;
    }
       
    public UIReward()
    {
        IsAutoMapJoystick = false;
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enReward, OnPropertyChanged);

        m_gold      = FindChild("Gold");

        m_key       = FindChild("Key");

        m_goldLabel = FindChildComponent<UILabel>("GoldNum");

        m_cardLabel = FindChildComponent<UILabel>("CardNum");
     
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Reward.ENPropertyChanged.enEnableKey)
        {
            EnableKey(true);
        }
        else if (eventType == (int)Reward.ENPropertyChanged.enNeedKey)
        {
            bool need = (bool)eventObj;
            if (need)
            {
                EnableKey(true);
            }
            else
            {
                ShowKey(need);
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
    }

    // 播放金币获得动画
    public void PlayGoldAnimation()
    {

        m_gold.GetComponent<Animation>().Play("ui-goldcoin-00");

        m_goldGrow = true;
    }

    // 播放契约书获得动画
    public void PlayCardAnimation()
    {

        //m_gold.animation.Play("ui-goldcoin-00");

        m_cardGrow = true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (m_goldGrow)
        {
            // 金币自动增长
            if (Reward.Singleton.m_goldDesNum - Reward.Singleton.m_goldNum > 0.9)
            {
                // m_goldNum = m_goldNum + 1;

                float golda = Mathf.SmoothDamp(Reward.Singleton.m_goldNum, Reward.Singleton.m_goldDesNum, ref svelocitx, 0.5f);

                Reward.Singleton.m_goldNum = golda;
                m_goldLabel.text = ((int)Reward.Singleton.m_goldNum).ToString();
            }
            else
            {
                m_goldGrow = false;
            }
        }

        if ( m_cardGrow )
        {
            // 自动增长
            if (Reward.Singleton.m_cardDesNum - Reward.Singleton.m_cardNum > 0)
            {
                Reward.Singleton.m_cardNum = Reward.Singleton.m_cardNum + 1;

                m_cardLabel.text            = Reward.Singleton.m_cardNum.ToString();
            }
            else
            {
                m_cardGrow = false;
            }
        }
    }


    public Vector3 GetGoldPostion()
    {
        return m_gold.transform.position;
    }

    // 激活钥匙
    public void EnableKey(bool enable)
    {
        m_key.SetActive(true);

        if (enable)
        {
            m_key.GetComponent<UISprite>().spriteName = "key";
        }
        else
        {
            m_key.GetComponent<UISprite>().spriteName = "key_disabled";
        }
    }

    // 显示钥匙
    public void ShowKey(bool show)
    {
        m_key.SetActive(show);
    }
}