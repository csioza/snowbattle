using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIOperateRepresentativeCard : UIWindow
{


    GameObject m_selectRCCancelBtn = null;

    GameObject m_RCOption = null;	//选择代表卡之后的界面，下面是它的子控件

    GameObject m_selectRC = null;	//选择

    GameObject m_RCDetail = null;	//详细

    GameObject m_RCCancel = null;	//取消

    CSItemGuid m_nowSelectCRGuid;	//当前选择的卡牌的guid


    static public UIOperateRepresentativeCard Create()
    {
        UIOperateRepresentativeCard self = UIManager.Singleton.GetUIWithoutLoad<UIOperateRepresentativeCard>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIOperateRepresentativeCard>("UI/UIOperateRepresentativeCard", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();

        m_selectRCCancelBtn = FindChild("SelectRCCancel");

        m_RCOption = FindChild("RCOption");
        m_selectRC = FindChild("ASelect");
        m_RCDetail = FindChild("BDetail");
        m_RCCancel = FindChild("CCancel");
    }

    public override void AttachEvent()
    {
        base.AttachEvent();

        AddMouseClickEvent(m_selectRCCancelBtn, OnSelectRCCancel);
        AddMouseClickEvent(m_selectRC, OnRcSelectClicked);
        AddMouseClickEvent(m_RCDetail, OnRcDetailClicked);
        AddMouseClickEvent(m_RCCancel, OnRcCancelClicked);
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }
    }


    // 队伍编辑 更新
    public override void OnShowWindow()
    {
        base.OnShowWindow();
    }

  
     //  点击响应
    public void OnClickItem(UICardItem item, ENSortType sortType)
    {

        ShowRepresentativeCardInfo(item.m_param.m_guid);
        m_nowSelectCRGuid = item.m_param.m_guid;
    }

    public bool OnSortCondion(CSItem item)
    {
        return false;
    }

    //换代表卡的返回
    void OnSelectRCCancel(object obj, EventArgs e)
    {
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enMainButtonList);
    }


    //代表卡信息里的 选择按钮
    void OnRcSelectClicked(object obj, EventArgs e)
    {
        MiniServer.Singleton.req_setRepresentativeCard(m_nowSelectCRGuid);
        m_RCOption.SetActive(false);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enMainButtonList);

    }
    //代表卡信息里的 详细信息按钮
    void OnRcDetailClicked(object obj, EventArgs e)
    {
        // 显示卡牌详细界面 只带返回按钮
        CardBag.Singleton.OnShowCardDetail(CardBag.Singleton.GetCardByGuid(m_nowSelectCRGuid), true);

    }
    //代表卡信息里的 取消按钮
    void OnRcCancelClicked(object obj, EventArgs e)
    {
        m_RCOption.SetActive(false);
    }

    void ShowRepresentativeCardInfo(CSItemGuid guid)
    {
        m_RCOption.SetActive(true);

        CSItem card = CardBag.Singleton.GetCardByGuid(guid);

        HeroInfo hero = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);

        if (null == hero)
        {
            Debug.LogWarning("null == hero card.IDInTable:" + card.IDInTable);
            return;
        }

        IconInfomation icon = GameTable.IconInfoTableAsset.Lookup(hero.ImageId);

        RaceInfo race = GameTable.RaceInfoTableAsset.LookUp(hero.Type);

        OccupationInfo occupationInfo = GameTable.OccupationInfoAsset.LookUp(hero.Occupation);

        // 图片
        FindChildComponent<UITexture>("CardPic", m_RCOption).mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);
        GameObject info = FindChild("Info", m_RCOption);
        FindChildComponent<UILabel>("InfoName", info).text = hero.StrName;
        FindChildComponent<UILabel>("InfoLevel", info).text = Localization.Get("CardLevel") + card.Level;
        FindChildComponent<UILabel>("InfoOp", info).text = occupationInfo.m_name;
        FindChildComponent<UILabel>("InfoHp", info).text = card.GetHp().ToString();
        FindChildComponent<UILabel>("InfoMagAttack", info).text = card.GetMagAttack().ToString();
        FindChildComponent<UILabel>("InfoPhyAttack", info).text = card.GetPhyAttack().ToString();
        FindChildComponent<UILabel>("InfoRace", info).text = race.m_name;


        RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(hero.Rarity);
        icon = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
        FindChildComponent<UITexture>("InfoRank", info).mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);
    }
   
}
