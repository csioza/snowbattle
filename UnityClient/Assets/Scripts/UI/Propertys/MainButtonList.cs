using System;
using System.Collections.Generic;


public class MainButtonList : IPropertyObject
{
    // 获得新卡牌的数量
    int m_newCardNum = 0;

    #region Singleton
    static MainButtonList m_singleton;
    static public MainButtonList Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new MainButtonList();
            }
            return m_singleton;
        }
    }
    #endregion

    public enum ENPropertyChanged
    {
        enShow = 1,
        enHide,
        enRemoveTeam,
        enUpdateTeam,
        enShowTeam,
        enShowZone,
    }

    public enum SHOWWNDTYPE
    {
        ENMain = 1, // zone界面
        ENTeam,    // 队伍界面
        ENBag,      // 背包界面
        ENStore,    // 商店界面
        ENSocial,   // 社区界面
        ENGroup,    // 公会界面
        ENSetting,  // 设定界面
    }

    // 当前显示界面的类型
    public SHOWWNDTYPE m_curShowType = SHOWWNDTYPE.ENMain;

    public MainButtonList()
    {
        SetPropertyObjectID((int)MVCPropertyID.enMainButtonList);
    }


    public void Show()
    {
        NotifyChanged((int)ENPropertyChanged.enShow, null);

    }

    public void ShowZoneWND()
    {
        NotifyChanged((int)ENPropertyChanged.enShowZone, null);
    }
    public void HideMainButtonList()
    {
        NotifyChanged((int)ENPropertyChanged.enHide, null);
    }
    
    // 获得新卡
    public void GotNewCard()
    {
        m_newCardNum++;
    }

    // 是否显示 背包上的 NEW
    public int GetShowNewNumForBag()
    {
        return m_newCardNum;
    }

    // 重置背包上的NEW数量
    public void ResetNewForBag()
    {
        m_newCardNum = 0;
    }

    // 获得队伍上限是否有变化
    public bool IsTeamMaxChange()
    {
        if (GameTable.playerAttrTableAsset == null )
        {
            return false;
        }

        if ( GameTable.playerAttrTableAsset == null )
        {
            return false;
        }

        // 升级前的
        PlayerAttrInfo playerInfo   = GameTable.playerAttrTableAsset.LookUp(User.Singleton.m_lastLevel);
        if ( playerInfo == null )
        {
            return false;
        }

        int oldTeamNum              = playerInfo.m_teamNum;

        playerInfo                  = GameTable.playerAttrTableAsset.LookUp(User.Singleton.GetLevel());
        if (playerInfo == null)
        {
            return false;
        }
        int newTeamNum              = playerInfo.m_teamNum;

        if (newTeamNum != oldTeamNum)
        {
            return true;
        }

        return false;
    }

    // 重置
    public void ResetNewForTeam()
    {
        User.Singleton.m_lastLevel = User.Singleton.GetLevel();
    }

    // 获得友情点数可以购买的 数量
    public int GetCanBuyNum()
    {
       return ShopProp.Singleton.GetFriendShipBuyNum();
    }
}
