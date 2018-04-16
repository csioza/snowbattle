using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

class UIGachaPanel : UIWindow
{

    UISprite m_star1;
    UISprite m_star2;
    UISprite m_star3;
    UISprite m_star4;
    UISprite m_star5;

    UIProgressBar m_cardBar;

    GameObject m_scrollAnimation;
    bool m_isPress = false;
    bool m_isUpButton = false;

    Transform m_getCardEffect;



    static public UIGachaPanel GetInstance()
    {
        UIGachaPanel self = UIManager.Singleton.GetUIWithoutLoad<UIGachaPanel>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIGachaPanel>("UI/UIGachaPanel", UIManager.Anchor.Center);
        return self;
    }



    public override void OnInit() 
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enGachaPanelProp, OnPropertyChanged);

        m_cardBar = FindChildComponent<UIProgressBar>("CardBar");

        EventDelegate.Add(m_cardBar.onChange, OnScrollBar);

        m_star1 = FindChildComponent<UISprite>("CardStar1");
        m_star2 = FindChildComponent<UISprite>("CardStar2");
        m_star3 = FindChildComponent<UISprite>("CardStar3");
        m_star4 = FindChildComponent<UISprite>("CardStar4");
        m_star5 = FindChildComponent<UISprite>("CardStar5");

        m_scrollAnimation = FindChildComponent<UISprite>("ScrollAnimation").GetComponent<Animation>().gameObject;

        m_getCardEffect = FindChildComponent<Transform>("ef-demo-attacked02");

    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        EventDelegate.Add(FindChild("CardPanelBG").GetComponent<UIEventTrigger>().onRelease, OnRelease);

    }
    public void OnRelease()
    {
        if (m_isPress)
        {
            m_isUpButton = true;
        }
    }
    

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj) 
    {

    }

    public void ResetPanelPos() 
    {
        m_cardBar.value = 0.0f;
        m_star1.transform.GetComponent<Parma>().m_type = 0;
        m_star2.transform.GetComponent<Parma>().m_type = 0;
        m_star3.transform.GetComponent<Parma>().m_type = 0;
        m_star4.transform.GetComponent<Parma>().m_type = 0;
        m_star5.transform.GetComponent<Parma>().m_type = 0;
        m_star1.spriteName = "cardlevelstar-yellow";
        m_star2.spriteName = "cardlevelstar-yellow";
        m_star3.spriteName = "cardlevelstar-yellow";
        m_star4.spriteName = "cardlevelstar-yellow";
        m_star5.spriteName = "cardlevelstar-yellow";
        m_isPress = false;
        m_isUpButton = false;
        AnimationState st = m_scrollAnimation.GetComponent<Animation>()["UIGachaPanelScrollAnimation"];
        st.normalizedTime = 0.0f;
        m_getCardEffect.gameObject.SetActive(false);
    }


    void OnScrollBar() 
    {
        if (m_isUpButton)
        {
            return;
        }
        //根据星级个数和拖动卡牌界面的位置 显示星级图片
        int rarity = GachaPanelProp.Singleton.m_cardRarity;
        float temp = m_cardBar.value;
        m_isPress = true;
        
        int star1Type = m_star1.transform.GetComponent<Parma>().m_type;
        int star2Type = m_star2.transform.GetComponent<Parma>().m_type;
        int star3Type = m_star3.transform.GetComponent<Parma>().m_type;
        int star4Type = m_star4.transform.GetComponent<Parma>().m_type;
        int star5Type = m_star5.transform.GetComponent<Parma>().m_type;
        if (temp > 0.05 && temp < 0.16 && rarity >= 1 && 0 == star1Type)
        {
            m_star1.transform.GetComponent<Parma>().m_type = 1;
            m_star1.spriteName = "cardlevelstar-yellow";//拉出第一颗星时 改变星级图片 播放特效
            //m_star1.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star1.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
            
        }
        if (temp > 0.16 && temp < 0.28 && rarity >= 2 && 0 == star2Type)
        {
            m_star2.transform.GetComponent<Parma>().m_type = 1;
            m_star1.spriteName = "cardlevelstar-coppery";//拉出第二颗星时 改变星图片 播放特效
            m_star2.spriteName = "cardlevelstar-coppery";
            //m_star1.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star1.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
            //m_star2.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star2.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
  
        }
        if (temp > 0.28 && temp < 0.4 && rarity >= 3 && 0 == star3Type)
        {
            m_star3.transform.GetComponent<Parma>().m_type = 1;
            m_star1.spriteName = "cardlevelstar-silver";//拉出第三颗星时 改变星图片 播放特效
            m_star2.spriteName = "cardlevelstar-silver";
            m_star3.spriteName = "cardlevelstar-silver";
            //m_star1.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star1.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
            //m_star2.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star2.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
            //m_star3.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star3.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
        }
        if (temp > 0.4 && temp < 0.5 && rarity >= 4 && 0 == star4Type)
        {
            m_star4.transform.GetComponent<Parma>().m_type = 1;
            m_star1.spriteName = "cardlevelstar-golden";//拉出第四颗星时 改变星图片 播放特效
            m_star2.spriteName = "cardlevelstar-golden";
            m_star3.spriteName = "cardlevelstar-golden";
            m_star4.spriteName = "cardlevelstar-golden";
            //m_star1.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star1.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
            //m_star2.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star2.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
            //m_star3.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star3.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
            //m_star4.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star4.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
        }
        if (temp > 0.5 && temp < 0.6 && rarity >= 5 && 0 == star5Type)
        {
            m_star5.transform.GetComponent<Parma>().m_type = 1;
            m_star1.spriteName = "cardlevelstar-colourful";//拉出第五颗星时 改变星图片 播放特效
            m_star2.spriteName = "cardlevelstar-colourful";
            m_star3.spriteName = "cardlevelstar-colourful";
            m_star4.spriteName = "cardlevelstar-colourful";
            m_star5.spriteName = "cardlevelstar-colourful";
            //m_star1.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star1.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
            //m_star2.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star2.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
            //m_star3.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star3.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
            //m_star4.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star4.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
            //m_star5.transform.FindChild("ef-demo-attacked02").active = true;
            //m_star5.transform.FindChild("ef-demo-attacked02").transform.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
        }
        //显示 卷轴滚动特效
        ScrollShowEffect(temp);

        //卡牌 完全拖出 播放特效 显示卡牌详情界面
        ShowCardEffectAndInfo(temp);
    }

    void ShowCardEffectAndInfo( float cardPanelPos) 
    {
        if (cardPanelPos >= 0.7)
        {
            //m_getCardEffect.active = true;
            //m_getCardEffect.FindChild("deathPebbles").GetComponent<ParticleSystem>().Play();
            MainGame.Singleton.StartCoroutine(CoroutineAnimationEnd(1.0f));
        }
    }
    private IEnumerator CoroutineAnimationEnd(float timeLength)
    {
        yield return new WaitForSeconds(timeLength);
        foreach (int value in GachaPanelProp.Singleton.m_slotList)
        {
            CSItem item = User.Singleton.ItemBag.GetItem(value);
            CardBag.Singleton.OnShowCardDetail(item, true);
            UICardDetail.GetInstance().RegisterCallBack(EndGachaAndShowShopPanel);
            MainUIManager.Singleton.ShowAllNeedShowWnd();
        }
        HideWindow();
    }

    void EndGachaAndShowShopPanel() 
    {
        UIShop.GetInstance().ShowWindow();
        UIShop.GetInstance().ShowRecruitment();
    }

    void ScrollShowEffect(float cardPanelPos) 
    {
        //AnimationState st = m_scrollAnimation.animation["UIGachaPanelScrollAnimation"];
        ////st.enabled = true;
        //st.normalizedTime = cardPanelPos;
        //st.weight = 1.0f;
        //m_scrollAnimation.SampleAnimation(st.clip, cardPanelPos * st.length);
    }


    public override void OnUpdate()
    {
        base.OnUpdate();
        if (m_isUpButton)
        {
            float cardPanelPos = m_cardBar.value;
            if (cardPanelPos >= 0.36f)
            {
                m_cardBar.value = cardPanelPos + 0.05f;
                ScrollShowEffect(cardPanelPos);
                ShowCardEffectAndInfo(cardPanelPos);
            }
            else 
            {
                m_cardBar.value = cardPanelPos - 0.05f;
                ScrollShowEffect(cardPanelPos);
                if (m_cardBar.value <= 0.0f)
                {
                    m_cardBar.value = 0.0f;
                    ResetPanelPos();
                }
            }
        }
    }


}