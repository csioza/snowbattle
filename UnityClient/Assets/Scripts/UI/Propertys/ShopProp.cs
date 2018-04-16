using System;
using System.Collections.Generic;

public struct MagicStoneInfo
{
    public int m_magicStoneId;
    public int m_magicStoneNum;
    public int m_magicStonePrice;
};

public struct RingOfHonorInfo 
{
    public int m_infoId;
    public int m_cardId;
    public int m_price;
};

public class ShopProp : IPropertyObject
{


    public List<MagicStoneInfo> m_magicStoneList;
    public List<RingOfHonorInfo> m_ringOfHonorList;
    public float m_cardPre = 0.0f;
    public enum ENPropertyChanged
    {
        enUpdate = 1,
        enUpdateMagicStoneShop = 2,
        enUpdateRecruitmentPanel = 3,
        enUpdateRingOfHonorShop = 4,
        enCloseOtherChildPanel = 5,
        enHide,
    }


    public ShopProp() 
    {
        SetPropertyObjectID((int)MVCPropertyID.enShopProps);
        m_magicStoneList = new List<MagicStoneInfo>();
        m_ringOfHonorList = new List<RingOfHonorInfo>();
    }

    #region Singleton
    static ShopProp m_singleton;
    static public ShopProp Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new ShopProp();
            }
            return m_singleton;
        }
    }
    #endregion
    public void OnUpdateRecruitmentPanel() 
    {
        NotifyChanged((int)ENPropertyChanged.enUpdateRecruitmentPanel, null);
    }
    public void OnCloseOtherChild() 
    {
        NotifyChanged((int)ENPropertyChanged.enCloseOtherChildPanel, null);
    }
    public void OnShowExpandFriendsSucc(int friendCount) 
    {
        //关闭转菊花loading界面
        Loading.Singleton.Hide();
        string titelText = Localization.Get("ExpandFriendSuccess");
        string text = string.Format(Localization.Get("ExpandFriendLimit"), friendCount);
        int friendNum = User.Singleton.GetFriendCount();
        string useText = string.Format(Localization.Get("FriendSpaceStatus"), friendNum);
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enExtendFrindTipsIconId);
        int iconId = param.IntTypeValue;
        RUSure.Singleton.ShowPanelText(text, titelText, useText, iconId);
    }

    public void OnShowRecoverStaminaSucc(int staminaNum) 
    {
        //关闭转菊花loading界面
        Loading.Singleton.Hide();
        string titelText = Localization.Get("StaminaTitle");
        string text = "";
        string useText = Localization.Get("AdventureTips");
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enRecoverStaminaIconId);
        int iconId = param.IntTypeValue;

        WorldParamInfo info = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enRecoverStaminaNum);
        if (0 > info.IntTypeValue)
        {
            text = Localization.Get("AllRecoverStamina");
        }
        else
        {
            text = string.Format(Localization.Get("RecoverStamina"), staminaNum);
        }

        RUSure.Singleton.ShowPanelText(text, titelText, useText, iconId);
    }

    public void OnShowRecoverEnergySucc(int energyNum)
    {
        //关闭转菊花loading界面
        Loading.Singleton.Hide();
        string titelText = Localization.Get("EnergyTitle");
        string text = "";
        string useText = Localization.Get("AdventureTips");
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enRecoverEnergyIconId);
        int iconId = param.IntTypeValue;

        WorldParamInfo info = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enRecoverEnergyNum);
        if (0 > info.IntTypeValue)
        {
            text = Localization.Get("AllRecoverEnergy");
        }
        else
        {
            text = string.Format(Localization.Get("RecoverEnergy"), energyNum);
        }

        RUSure.Singleton.ShowPanelText(text, titelText, useText, iconId);
    }

    public void OnUpdateMagicStoneShop() 
    {
        NotifyChanged((int)ENPropertyChanged.enUpdateMagicStoneShop, null);
    }

    public void OnUpdateRingOfHonorShop() 
    {
        NotifyChanged((int)ENPropertyChanged.enUpdateRingOfHonorShop, null);
    }

    public void OnHideShop()
    {
        NotifyChanged((int)ENPropertyChanged.enHide, null);
    }

    //获得友情点数 可购买卡牌次数
    public int GetFriendShipBuyNum() 
    {
        int friendPoint = User.Singleton.UserProps.GetProperty_Int32(UserProperty.friendship_point);
        WorldParamInfo param = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enFrinendShipRecruitmentNum);
        return (friendPoint / param.IntTypeValue);
    }
    //发送请求魔法石商店道具
    public void SendMagicStoneShopInfo()
    {
        //向服务器发送请求魔法石道具数量
        MiniServer.Singleton.SendShopItemInfo_C2S((int)UIShop.ShopType.enMagicStone);
    }
    //发送购买荣誉戒指消息
    public void SendBuyRingOfHonor(int ringOfHonorId, int cardId)
    {
        MiniServer.Singleton.SendBuyRingOfHonorCard_C2S(ringOfHonorId, cardId);
    }
    //购买魔法石消息
    public void SendBuyMagicStone(int stoneId)
    {
        MiniServer.Singleton.SendBuyMagicStone_C2S(stoneId);
    }
    //一次友情点数招募
    public void SendFriendRecruitmentOnce() 
    {
        //向服务器发送一次友情招募
        MiniServer.Singleton.SendRecruitment_C2S((int)UIShop.RecruitmentType.enFriendShipOnce);
    }

    //多次友情点数招募
    public void SendFriendRecruitmentMore() 
    {
        //向服务器发送多次友情招募
        MiniServer.Singleton.SendRecruitment_C2S((int)UIShop.RecruitmentType.enFriendShipMore);
    }
    //魔法石招募
    public void SendMagicStoneRecruitment() 
    {
        MiniServer.Singleton.SendRecruitment_C2S((int)UIShop.RecruitmentType.enMagicStone);
    }
    //发送回复体力消息
    public void SendRecoverStamina() 
    {
        MiniServer.Singleton.SendShopItemInfo_C2S((int)UIShop.ShopType.enRecoverStamina);
    }
    //发送回复能量消息
    public void SendRecoverEnergy() 
    {
        MiniServer.Singleton.SendShopItemInfo_C2S((int)UIShop.ShopType.enRecoverEnergy);
    }
    //发送请求荣誉戒指商店信息
    public void SendRingOfHonorShopInfo() 
    {
        MiniServer.Singleton.SendShopItemInfo_C2S((int)UIShop.ShopType.enRingOfHonor);
    }
    //发送购买好友格子
    public void SendExpandFriends() 
    {
        MiniServer.Singleton.SendShopItemInfo_C2S((int)UIShop.ShopType.enExpandFriends);
    }
    //发送到服务器获得卡牌消息
    public void SendGetCardBuyId(int cardId) 
    {
        MiniServer.Singleton.SendGetCardById_C2S((int)UIShop.RecruitmentType.enGetCardById, cardId);
    }
}
