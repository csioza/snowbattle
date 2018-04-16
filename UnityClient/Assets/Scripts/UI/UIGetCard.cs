using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIGetCard : UIWindow
{

    UITexture m_card         = null;
    UILabel m_name          = null;

    UILabel m_cardAttack    = null;
    UILabel m_cardCost      = null;
    UILabel m_cardHp        = null;
    UILabel m_cardName      = null;
    UITexture m_cardRarity   = null;
    UILabel m_attirbutename = null;
    UILabel m_magAttack     = null;

    static public UIGetCard GetInstance()
	{
        UIGetCard self = UIManager.Singleton.GetUIWithoutLoad<UIGetCard>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIGetCard>("UI/UIGetCard", UIManager.Anchor.Center);
		return self;
	}
	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enCardBag, OnPropertyChanged);

        m_card          = FindChildComponent<UITexture>("CardBG");
        m_name          = FindChildComponent<UILabel>("Name");

        m_cardAttack    = FindChildComponent<UILabel>("Attack");
        m_cardCost      = FindChildComponent<UILabel>("Cost");
        m_cardHp        = FindChildComponent<UILabel>("HP");
        m_cardName      = FindChildComponent<UILabel>("Name");
        m_cardRarity    = FindChildComponent<UITexture>("Rarity");
        m_magAttack     = FindChildComponent<UILabel>("MagAttack");
        m_attirbutename = FindChildComponent<UILabel>("AttirbuteName");
	}

    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("CardBG", OnHide);
        AddChildMouseClickEvent("BG", OnHide);

    }

    void OnHide(object sender, EventArgs e)
    {
        HideWindow();
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)CardBag.ENPropertyChanged.enShowGetNewCard)
        {
            UpdateInfo((CSItemGuid)eventObj);
            ShowWindow();
        }
    }
    //设置名字
    public void SetName(string name) 
    {
        m_cardName.text = name;
    }

    // 更新卡牌界面相关信息 
    public void UpdateInfo(CSItemGuid guid)
    {
        CSItem card = CardBag.Singleton.GetCardByGuid(guid);
        if (null == card)
        {
            return;
        }
        
        m_name.text         = "";

        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
        if (null == info)
        {
            Debug.Log("UICard UpdateInfo HeroInfo 表数据没有 ID 为：" + card.IDInTable);
            return;
        }

        IconInfomation imageInfo  = GameTable.IconInfoTableAsset.Lookup(info.ImageId);

        if (null == imageInfo)
        {
            return;
        }

        m_card.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(imageInfo.dirName);

        m_name.text             = info.StrName;
        m_attirbutename.name    = info.StrName;

        // 星级

        RarityRelativeInfo rarityInfo   = GameTable.RarityRelativeAsset.LookUp(info.Rarity);
        if (null == rarityInfo)
        {
            return;
        }
        IconInfomation icon             = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
        m_cardRarity.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);

        // 消耗
        m_cardCost.text     = info.Cost.ToString();

        // 生命值
        m_cardHp.text       = card.GetHp().ToString();

        m_magAttack.text    = card.GetMagAttack().ToString();

        m_cardAttack.text   = card.GetPhyAttack().ToString();
    }
}
