using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICardHead : UIWindow
{
    public delegate void OnClickCallbacked(CSItem card, int materialId);
    public delegate void OnPressCallbacked(CSItem card);

    OnClickCallbacked m_clickCallBack;
    OnPressCallbacked m_pressCallBack;

    UISprite m_cardMask;//卡牌头像不可点击遮罩
    UITexture m_cardHeadImage;//卡牌头像图片
    UISprite m_cardLove;//是否为最爱卡牌
    UITexture m_cardOcc;//卡牌职业图标
    public UILabel m_cardInfo;//卡牌信息 代表，使用中
    UITexture m_cardStar;//卡牌星级

    public CSItem m_showCard;
    public int m_materialId;

    GameObject m_new = null;
    public bool m_bNew     = false;// 是否要显示NEW
    static public UICardHead Create()
    {
        UICardHead self = UIManager.Singleton.LoadUI<UICardHead>("UI/UICardHead", UIManager.Anchor.Center);
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();
        m_cardMask = FindChildComponent<UISprite>("Card_Mask");
        m_cardHeadImage = FindChildComponent<UITexture>("CardHead_Texture");
        m_cardLove = FindChildComponent<UISprite>("Love_Card");
        m_cardOcc = FindChildComponent<UITexture>("Occupation_Texture");
        m_cardInfo = FindChildComponent<UILabel>("Show_Info");
        m_cardStar = FindChildComponent<UITexture>("Star_Texture");

        m_new = FindChild("New");

        //小型通用头像 
        if (null != m_cardHeadImage)
        {
            AddMouseClickEvent(m_cardHeadImage.gameObject,OnClickItem);
            AddMouseLongPressEvent(m_cardHeadImage.gameObject, OnClickLongPressItem);
        }
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
    }
    public override void OnShowWindow()
    {
        base.OnShowWindow();
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (null == WindowRoot)
        {
            return;
        }
    }

    public void RegisterCallBack(OnClickCallbacked click,OnPressCallbacked press)
    {
        m_clickCallBack = click;
        m_pressCallBack = press;
    }

    
    public void ShowCard()
    {
        if (null == m_showCard)
        {
            return;
        }
        //设置卡牌头像
        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(m_showCard.IDInTable);
        if (null == info)
        {
            Debug.Log("UICardEvolution HeroInfo == null card.m_id:" + m_showCard.IDInTable);
            return;
        }
         IconInfomation imageInfo = GameTable.IconInfoTableAsset.Lookup(info.ImageId);

        if (null == imageInfo)
        {
            Debug.Log("UICardEvolution IconInfo == null info.ImageId:" + info.ImageId);
            return;
        }
        m_cardHeadImage.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(imageInfo.dirName);
        //设置星级图标
        RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(info.Rarity);
        if (null == rarityInfo)
        {
            Debug.Log("RarityRelativeInfo rarityInfo == null info.RarityId:" + info.Rarity);
            return;
        }
        IconInfomation rarityIcon = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);
        if (null == rarityIcon)
        {
            Debug.Log("IconInfomation rarityIcon == null rarityInfo.m_iconId:" + rarityInfo.m_iconId);
            return;
        }
        m_cardStar.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(rarityIcon.dirName);
        //设置职业图标
        OccupationInfo occInfo = GameTable.OccupationInfoAsset.LookUp(info.Occupation);
        if (null == occInfo)
        {
            Debug.Log("OccupationInfo occInfo == null info.Occupation:" + info.Occupation);
            return;
        }
        IconInfomation occIcon = GameTable.IconInfoTableAsset.Lookup(occInfo.m_iconId);
        if (null == occIcon)
        {
            Debug.Log("IconInfomation occIcon == null rarityInfo.m_iconId:" + occInfo.m_iconId);
            return;
        }
        m_cardOcc.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(occIcon.dirName);
        //是否在编队中
        bool isInTeam = Team.Singleton.IsCardInTeam(m_showCard.m_guid);
        if (isInTeam)
        {
            m_cardInfo.gameObject.SetActive(true);
            m_cardInfo.text = Localization.Get("InTheUseOf");
        }
        //是否是代表卡
        if (User.Singleton.RepresentativeCard == m_showCard.m_guid)
        {
            m_cardInfo.gameObject.SetActive(true);
            m_cardInfo.text = Localization.Get("Onbehalfof");
        }
        //是否是最爱
        if (m_showCard.Love)
        {
            m_cardLove.gameObject.SetActive(true);
        }

        // 是否是新卡
        m_new.gameObject.SetActive(m_bNew);
       
    }
    public void SetCardMask(bool isEnabled)
    {
        m_cardMask.gameObject.SetActive(isEnabled);
    }
    public void SetCardInfoShow(bool isEnabled) 
    {
        m_cardInfo.gameObject.SetActive(isEnabled);
    }
    public void SetCardLoveShow(bool isEnabled)
    {
        m_cardLove.gameObject.SetActive(isEnabled);
    }

    public void SetCardNew(bool isEnabled)
    {
        m_bNew = isEnabled;
    }
    public void SetCardInfo(CSItem item) 
    {
        m_showCard = item;
    }

    void OnClickItem(object sender, EventArgs e) 
    {
        //如果卡牌存在遮罩则不可 出现点击
        if (m_cardMask.gameObject.activeSelf)
        {
            return;
        }
        if (null != m_clickCallBack)
        {
            m_clickCallBack(m_showCard,m_materialId);
        }
    }

    void OnClickLongPressItem(object sender, EventArgs e)
    {
        if (null != m_pressCallBack)
        {
            m_pressCallBack(m_showCard);
        }
    }
    
}
