using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIExpandBag : UIWindow
{

    UILabel m_diamond   = null;
    UILabel m_cost1     = null;
    UILabel m_cost2     = null;
    UILabel m_cost3     = null;

    UILabel m_tips1     = null;
    UILabel m_tips2     = null;
    UILabel m_tips3     = null;

    //UISprite m_bg       = null;
    UILabel m_btn       = null;

    //UIToggle m_one      = null;
    UIToggle m_two      = null;
    UIToggle m_thr      = null;

    static public UIExpandBag GetInstance()
    {
        UIExpandBag self = UIManager.Singleton.GetUIWithoutLoad<UIExpandBag>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UIExpandBag>("UI/UIExpandBag", UIManager.Anchor.Center);
        }
        return self;
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enExpandBag, OnPropertyChanged);


        m_diamond   = FindChildComponent<UILabel>("Diamond");

        m_cost1     = FindChildComponent<UILabel>("Cost1");
        m_cost2     = FindChildComponent<UILabel>("Cost2");
        m_cost3     = FindChildComponent<UILabel>("Cost3");

        m_tips1     = FindChildComponent<UILabel>("Tips1");
        m_tips2     = FindChildComponent<UILabel>("Tips2");
        m_tips3     = FindChildComponent<UILabel>("Tips3");

//        m_bg        = FindChildComponent<UISprite>("BG");
        m_btn       = FindChildComponent<UILabel>("Btn");

//        m_one       = FindChildComponent<UIToggle>("1");
        m_two       = FindChildComponent<UIToggle>("2");
        m_thr       = FindChildComponent<UIToggle>("3");
    }
	void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)//heizTest
    {
        if (eventType == (int)ExpandBag.ENPropertyChanged.enShowExpandBag)
        {
            ShowWindow();
            UpdateInfo();
        }
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("OK", OnOk);
        AddChildMouseClickEvent("Cancel", OnCancel);
        AddChildMouseClickEvent("1", OnClick);
        AddChildMouseClickEvent("2", OnClick);
        AddChildMouseClickEvent("3", OnClick);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

    }

    public void OnOk( GameObject obj )
    {
        CardBag.Singleton.ExpandCardBag(ExpandBag.Singleton.m_expandType);
        HideWindow();

    }

    public void OnCancel(GameObject obj)
    {
        HideWindow();
    }

    public void OnClick(GameObject obj)
    {
        int type                            = int.Parse(obj.gameObject.name);

        ExpandBag.Singleton.m_expandType    = type;
    }


    void UpdateInfo()
    {
        int diamond                     = User.Singleton.GetDiamond();
        m_diamond.text                  = string.Format(Localization.Get("MagicStoneNum"), diamond); 


        BagInfo bagInfo                 =  CardBag.Singleton.m_bagTableInfo;

        string text1    = string.Format(Localization.Get("ExpandBagCost"), bagInfo.m_expandCost1, bagInfo.m_expandSize1);

        string text2    = string.Format(Localization.Get("ExpandBagRemain"), (diamond - bagInfo.m_expandCost1)); 

        m_cost1.text    = text1;

        if (diamond >= bagInfo.m_expandSize1)
        {
            m_tips1.color   = Color.white;  
            m_tips1.text    = text2;
        }
        else
        {
            m_tips1.GetComponent<UILabel>().color = Color.red;
            m_tips1.text = Localization.Get("NotEnoughMagicStone");
        }


        text1           = string.Format(Localization.Get("ExpandBagCost"), bagInfo.m_expandCost2, bagInfo.m_expandSize2);

        text2           = string.Format(Localization.Get("ExpandBagRemain"), (diamond - bagInfo.m_expandCost2)); 

        m_cost2.text    = text1;

        if (diamond >= bagInfo.m_expandCost2)
        {
            m_tips2.GetComponent<UILabel>().color = Color.white;
            m_tips2.text = text2;
        }
        else
        {
            m_tips2.GetComponent<UILabel>().color = Color.red;
            m_tips2.text = Localization.Get("NotEnoughMagicStone");
        }

        text1           = string.Format(Localization.Get("ExpandBagCost"), bagInfo.m_expandCost3, bagInfo.m_expandSize3); 

        text2           = string.Format(Localization.Get("ExpandBagRemain"), (diamond - bagInfo.m_expandCost3)); 

        m_cost3.text    = text1;
        if (diamond >= bagInfo.m_expandCost3)
        {
            m_tips3.GetComponent<UILabel>().color = Color.white;
            m_tips3.text = text2;
        }
        else
        {
            m_tips2.GetComponent<UILabel>().color = Color.red;
            m_tips2.text = Localization.Get("NotEnoughMagicStone");
        }

        // 可以扩充的格子数量
        int slotNum = CardBag.Singleton.m_bagTableInfo.m_maxSize - CardBag.Singleton.GetBagCapcity();

        // 根据可以扩充的格子 来自动调整是否要隐藏 扩充选择选项
        if (ExpandBag.Singleton.m_showOptionNum == 2)
        {
            if (slotNum < bagInfo.m_expandSize2)
           {
               ExpandBag.Singleton.m_showOptionNum = 1;
           }
            
        }

        if (ExpandBag.Singleton.m_showOptionNum == 3)
        {
            if (slotNum < bagInfo.m_expandSize3)
            {
                ExpandBag.Singleton.m_showOptionNum = 2;
            }

            if (slotNum < bagInfo.m_expandSize2)
            {
                ExpandBag.Singleton.m_showOptionNum = 1;
            }

        }

        if (ExpandBag.Singleton.m_showOptionNum == 1)
        {
            //m_bg.transform.localScale       = new UnityEngine.Vector3(2.3f,1.97f,1f);
            //m_bg.transform.localPosition    = UnityEngine.Vector3.zero;

            m_btn.transform.localPosition   = UnityEngine.Vector3.zero;

            m_two.gameObject.SetActive(false);
            m_thr.gameObject.SetActive(false);
        }
        else if (ExpandBag.Singleton.m_showOptionNum == 2)
        {
            //m_bg.transform.localScale       = new UnityEngine.Vector3(2.3f, 2.7f, 1f);
            //m_bg.transform.localPosition    = new UnityEngine.Vector3(0f, -50f, 0f);

            m_btn.transform.localPosition   = new UnityEngine.Vector3(0f, -75f, 0f);

            m_two.gameObject.SetActive(true);
            m_thr.gameObject.SetActive(false);
        }
        else if (ExpandBag.Singleton.m_showOptionNum == 3)
        {

            //m_bg.transform.localScale = new UnityEngine.Vector3(2.3f, 3.6f, 1f);
            //m_bg.transform.localPosition = new UnityEngine.Vector3(0f, -88f, 0f);

            m_btn.transform.localPosition = new UnityEngine.Vector3(0f, -154f, 0f);


            m_two.gameObject.SetActive(true);
            m_thr.gameObject.SetActive(true);
        }
    }
}