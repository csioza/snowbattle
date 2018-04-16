using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICard : UIWindow
{

    UILabel m_cardAttack    = null;
	UILabel m_cardCost      = null;
    UILabel m_cardHp        = null;
    UILabel m_cardName      = null;
    UISprite m_cardRarity   = null;
    UITexture m_card = null;
    
    static public UICard GetInstance()
	{
        UICard self = UIManager.Singleton.GetUIWithoutLoad<UICard>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UICard>("UI/UICard", UIManager.Anchor.Center);
		return self;
	}
	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enCardBag, OnPropertyChanged);

        m_cardAttack    = FindChildComponent<UILabel>("Attack");
        m_cardCost      = FindChildComponent<UILabel>("Cost");
        m_cardHp        = FindChildComponent<UILabel>("HP");
        m_cardName      = FindChildComponent<UILabel>("Name");
        m_cardRarity    = FindChildComponent<UISprite>("Rarity");
        m_card          = FindChildComponent<UITexture>("CardBG");
	}

    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("CardBG", OnHide);

    }

    void OnHide(object sender, EventArgs e)
    {
        HideWindow();
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        Debug.Log("UICard OnPropertyChanged:" + eventType);
        if (eventType == (int)CardBag.ENPropertyChanged.enShowCard)
        {
            CSItemGuid guid = (CSItemGuid)eventObj;
            UpdateInfo(guid);
            ShowWindow();
        }
    }

    // 设置星级图片
    public void SetRarity(int Rarity)
    {
        if ( Rarity == 0 )
        {
            return;
        }

        m_cardRarity.spriteName = "i-star-0" + Rarity;

    }

    // 更新卡牌界面相关信息 
    public void UpdateInfo(CSItemGuid guid)
    {
        CSItem cardInfo = CardBag.Singleton.itemBag.GetItemByGuid(guid);

        if (cardInfo == null )
        {
            return;
        }
        // 卡牌ID 
        int id              = cardInfo.IDInTable;

        HeroInfo info       = GameTable.HeroInfoTableAsset.Lookup(id);
        if (null == info)
        {
            Debug.Log("UICard UpdateInfo HeroInfo 表数据没有 ID 为：" + id);
            return;
        }

         // 星级
        SetRarity(info.Rarity);

        RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(info.Rarity);
        if (null == rarityInfo)
        {
            Debug.Log("UICard UpdateInfo RarityRelativeInfo 表数据没有 ID 为：" + id);
            return;
        }

        IconInfomation imageInfo = GameTable.IconInfoTableAsset.Lookup(info.ImageId);

        if (null == imageInfo)
        {
            return;
        }

        // 消耗
        m_cardCost.text     =  info.Cost.ToString();

        m_cardName.text     =  info.StrName.ToString();

        m_card.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(imageInfo.dirName);


        // 计算生命值
        float hp    = cardInfo.GetHp();

        m_cardHp.text       = hp.ToString();

        // 计算物理攻击力
        float attack = cardInfo.GetPhyAttack();

        m_cardAttack.text   = attack.ToString();

        Debug.Log("UICard UpdateInfo：" + info.StrName);
    }
}
