using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIMainButtonList : UIWindow
{
    enum ShowType
    {
        enCardType =1,
        enStoreType ,
        enTeamType,
        enCommuityType,
        enStaminaType,
        enSettingType,
        enZoneType,
    }

    GameObject m_bagNew         = null;
    GameObject m_communityNew   = null;
    GameObject m_storeNew       = null;
    GameObject m_teamNew        = null;

    UISprite m_stama            = null; // 体力
    UISprite m_ration           = null; // 军资

    UILabel m_rationNum         = null;
    UILabel m_rationTime        = null;
    UILabel m_staminaNum        = null;
    UILabel m_staminaTime       = null;
    UISprite m_staminaEffect    = null;
    UISprite m_rationEffect     = null;
    UISprite m_staminaAnimation = null;
    UISprite m_rationAnimation  = null;
    UILabel m_curRationNum      = null;
    UILabel m_curStaminaNum     = null;

    //ShowType m_curType          = ShowType.enZoneType;

    public UIMainButtonList()
    {

    }
    static public UIMainButtonList GetInstance()
    {
        UIMainButtonList self = UIManager.Singleton.GetUIWithoutLoad<UIMainButtonList>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIMainButtonList>("UI/UIMainButtonList", UIManager.Anchor.Center);
        return self;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    public override void OnInit()
    {
        base.OnInit();
       
        AddPropChangedNotify((int)MVCPropertyID.enMainButtonList, OnPropertyChanged);

        m_bagNew        = FindChild("CardBagNew");
        m_communityNew  = FindChild("CommunityNew");
        m_storeNew      = FindChild("StoreNew");
        m_teamNew       = FindChild("TeamNew");

        m_stama         = FindChildComponent<UISprite>("stama");
        m_ration        = FindChildComponent<UISprite>("ration");

        m_rationNum     = FindChildComponent<UILabel>("RationNumber");
        m_rationTime    = FindChildComponent<UILabel>("RationTime");
        m_staminaNum    = FindChildComponent<UILabel>("Staminanumber");
        m_staminaTime   = FindChildComponent<UILabel>("StaminaTime");
        m_rationEffect  = FindChildComponent<UISprite>("rationeffect");
        m_staminaEffect = FindChildComponent<UISprite>("staminaeffect");
        m_staminaAnimation = FindChildComponent<UISprite>("staminaeffectPos");
        m_rationAnimation = FindChildComponent<UISprite>("rationeffectPos");
        m_curRationNum = FindChildComponent<UILabel>("RatioNumber1");
        m_curStaminaNum = FindChildComponent<UILabel>("Staminanumber1");
    }
   
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {

        if (eventType == (int)MainButtonList.ENPropertyChanged.enShow)
        {
            ShowWindow();
            UpdateInfo();
        }

        else if (eventType == (int)MainButtonList.ENPropertyChanged.enHide) 
        {
            HideWindow();
        }
       
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("CardBag", OnClickCardBag);
        AddChildMouseClickEvent("Store", OnClickStore);
        AddChildMouseClickEvent("TeamBtn", OnClickTeam);
        AddChildMouseClickEvent("CommunityBtn", OnClickCommuity);
        AddChildMouseClickEvent("GuildBtn", OnClickGuildBtn);
        AddChildMouseClickEvent("SettingBtn", OnClickSettingBtn);
    }

    // 点击背包按钮
    public void OnClickCardBag(GameObject obj)
    {
//         if (MainButtonList.Singleton.m_curShowType == MainButtonList.SHOWWNDTYPE.ENBag)
//         {
//             return;
//         }

        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardBag);
        //CardBag.Singleton.OnShowCardBag();
        m_bagNew.SetActive(false);
        MainButtonList.Singleton.ResetNewForBag();
    }

    // 点击社区按钮
    public void OnClickCommuity(GameObject obj)
    {
        //         if (MainButtonList.Singleton.m_curShowType == MainButtonList.SHOWWNDTYPE.ENSocial)
        //         {
        //             return;
        //         }

        /*        TipMessageBox.Singleton.OnShowTipMessageBox();*/
        //Guild.Singleton.OnShow();
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enGuild);
        m_communityNew.SetActive(false);
    }


    //点击 体力剂
    public void OnClickGuildBtn(GameObject obj)
    {
        // 返回主界面
        if (MainButtonList.Singleton.m_curShowType == MainButtonList.SHOWWNDTYPE.ENMain)
        {
            return;
        }
//         //Zone.Singleton.ShowZone();
//         MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enZone);
        MainUIManager.Singleton.HideCurWindow();
        MainButtonList.Singleton.m_curShowType = MainButtonList.SHOWWNDTYPE.ENMain;

    }

    // 点击设置按钮
    public void OnClickSettingBtn(GameObject obj)
    {
        GameSetingProp.Singleton.OnCloseOtherChild();//关闭设置界面所有子界面
        if (MainButtonList.Singleton.m_curShowType == MainButtonList.SHOWWNDTYPE.ENSetting)
        {
            return;
        }
        //TipMessageBox.Singleton.OnShowTipMessageBox();
        //UISetingPanel.GetInstance().ShowWindow();
        MainButtonList.Singleton.m_curShowType = MainButtonList.SHOWWNDTYPE.ENSetting;
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enSetingPanel);
    }

    // 点击商店按钮
    public void OnClickStore(GameObject obj)
    {
        ShopProp.Singleton.OnCloseOtherChild();//关闭商城所有子界面
        if (MainButtonList.Singleton.m_curShowType == MainButtonList.SHOWWNDTYPE.ENStore)
        {
            return;
        }
        /*UIShop.GetInstance().ShowWindow();*/
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enShop);
        m_storeNew.SetActive(false);
    }

    // 点击队伍按钮
    public void OnClickTeam(GameObject obj)
    {
//         if (MainButtonList.Singleton.m_curShowType == MainButtonList.SHOWWNDTYPE.ENTeam)
//         {
//             return;
//         }

        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enTeam);
        /*Team.Singleton.OnShowTeam();*/
        m_teamNew.SetActive(false);
        MainButtonList.Singleton.ResetNewForTeam();
    }

    void UpdateInfo()
    {

    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        int energy          = User.Singleton.GetEnergy();
        int maxEnergy       = User.Singleton.GetMaxEnergy();

        int stamina         = User.Singleton.GetStamina();
        int staminaMax      = User.Singleton.GetMaxStamina();

        m_rationNum.text        = maxEnergy.ToString();
        m_curRationNum.text     = energy.ToString();
        m_staminaNum.text       =  staminaMax.ToString();
        m_curStaminaNum.text    = stamina.ToString();


        m_stama.fillAmount = ((float)stamina / (float)staminaMax);
        m_ration.fillAmount = ((float)energy / (float)maxEnergy);

        int recoverTime     = User.Singleton.GetEnergyRecoverTime();
        int nMinite         = recoverTime / 60;
        int nSec            = recoverTime - nMinite * 60;
        if (recoverTime>0)
        {
            m_rationTime.text = nMinite + ":" +  nSec.ToString("00");
        }
        else
        {
            m_rationTime.text = "";
        }
       

        recoverTime         = User.Singleton.GetStaminaRecoverTime();
        nMinite             = recoverTime / 60;
        nSec                = recoverTime - nMinite * 60;

        if (recoverTime > 0)
        {
            m_staminaTime.text = nMinite + ":" + nSec.ToString("00");
        }
        else
        {
            m_staminaTime.text = "";
        }

        int newNum = MainButtonList.Singleton.GetShowNewNumForBag();
        if (newNum > 0)
        {
            m_bagNew.SetActive(true);
            string str = newNum.ToString();
            if (newNum > 99)
            {
                str = "99+";
            }
            m_bagNew.transform.Find("Label").GetComponent<UILabel>().text = str;

        }
        else if (newNum == 0)
        {
            m_bagNew.SetActive(false);
            m_bagNew.transform.Find("Label").GetComponent<UILabel>().text      = "";
        }

        m_teamNew.SetActive(MainButtonList.Singleton.IsTeamMaxChange());

        newNum = MainButtonList.Singleton.GetCanBuyNum();

        if (newNum > 0)
        {
            m_storeNew.SetActive(true);
            string str = newNum.ToString();
            if (newNum > 99)
            {
                str = "99+";
            }
            m_storeNew.transform.Find("Label").GetComponent<UILabel>().text = str;

        }
        else if (newNum == 0)
        {
            m_storeNew.SetActive(false);
            m_storeNew.transform.Find("Label").GetComponent<UILabel>().text = "";
        }
        if (0 == energy || energy == maxEnergy)
        {
            m_rationEffect.transform.gameObject.SetActive(false);
        }
        else 
        {
            m_rationEffect.transform.gameObject.SetActive(true);
            m_rationAnimation.GetComponent<Animation>().Play();
            foreach (AnimationState state in m_rationAnimation.GetComponent<Animation>())
            {
                state.normalizedTime = m_ration.fillAmount;
            }
        }
        if (0 == stamina || stamina == staminaMax)
        {
            m_staminaEffect.transform.gameObject.SetActive(false);
        }
        else 
        {
            m_staminaEffect.transform.gameObject.SetActive(true);
            m_staminaAnimation.GetComponent<Animation>().Play();
            foreach (AnimationState state in m_staminaAnimation.GetComponent<Animation>())
            {
                state.normalizedTime = m_stama.fillAmount;
            }
        }
    }
}
