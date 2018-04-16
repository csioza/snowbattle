using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICardDetailCardPanel : UIWindow
{
    UILabel m_name      = null;
    UITexture m_rarity  = null;
    GameObject m_occ    = null;

    // 卡牌的背景图
    UITexture m_card    = null;
    UILabel m_cardNumber = null;

    static public UICardDetailCardPanel Create()
    {
        UICardDetailCardPanel self = UIManager.Singleton.LoadUI<UICardDetailCardPanel>("UI/UICardDetailCardPanel", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();

        m_name      = FindChildComponent<UILabel>("Card_Name");
        m_rarity    = FindChildComponent<UITexture>("Star_Level");
        m_occ       = FindChild("Occupation");
        m_card      = FindChildComponent<UITexture>("Card");

        m_cardNumber = FindChildComponent<UILabel>("Card_Number"); 
    }

    public override void AttachEvent()
    {
        base.AttachEvent();


        AddChildMouseClickEvent("Box1", OnShowBigCard);

        AddChildMouseClickEvent("Box2", OnShowSmallCard);
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }
    }


    public override void OnShowWindow()
    {
        base.OnShowWindow();
    }
    // 播放展开大图的动画
    void OnShowBigCard(GameObject obj)
    {
        if (WindowRoot.GetComponent<Animation>().IsPlaying("UI-cardScaleMax-00"))
        {
            return;
        }
        WindowRoot.GetComponent<Animation>().Play("UI-cardScaleMax-00");
    }

    // 播放 变成小图的动画
    void OnShowSmallCard(GameObject obj)
    {
        if (WindowRoot.GetComponent<Animation>().IsPlaying("UI-cardScaleMax-01"))
        {
            return;
        }
        WindowRoot.GetComponent<Animation>().Play("UI-cardScaleMax-01");
    }

    public void Update(int cardId)
    {

        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(cardId);

        if (null == heroInfo)
        {
            return;
        }

        m_name.text                     = heroInfo.StrName;

        m_cardNumber.text               = heroInfo.CardId+"";

        IconInfomation iconInfo         = GameTable.IconInfoTableAsset.Lookup(heroInfo.ImageId);
        m_card.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
        OccupationInfo occupationInfo   = GameTable.OccupationInfoAsset.LookUp(heroInfo.Occupation);
        iconInfo                        = GameTable.IconInfoTableAsset.Lookup(occupationInfo.m_iconId);
        m_occ.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);


        RarityRelativeInfo rarityInfo   = GameTable.RarityRelativeAsset.LookUp(heroInfo.Rarity);
        iconInfo                        = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
        m_rarity.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
    }
  

}
