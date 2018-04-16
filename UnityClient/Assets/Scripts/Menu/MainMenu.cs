using System;
using System.Collections.Generic;


public class MainMenu : IPropertyObject
{
    #region Singleton
    static MainMenu m_singleton;
    static public MainMenu Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new MainMenu();
            }
            return m_singleton;
        }
    }
    #endregion

    public enum ENPropertyChanged
    {
        enShow = 1,
        enShowMain,
        enHideMain,
        enRemoveTeam,
        enUpdateTeam,
        enShowTeam,
        enShowZone,
        enHideZone,

    }

    public enum SHOWWNDTYPE
    {
        ENMain = 1, // zone界面
        ENTeam ,    // 队伍界面
        ENBag,      // 背包界面
        ENStore,    // 商店界面
        ENSocial,   // 社区界面
        ENGroup,    // 公会界面
        ENSetting,  // 设定界面
    }

    // 当前显示界面的类型
    public SHOWWNDTYPE m_curShowType = SHOWWNDTYPE.ENMain;

    public MainMenu()
    {
        SetPropertyObjectID((int)MVCPropertyID.enMainMenu);
    }


    public void ShowMainMenu()
    {
        NotifyChanged((int)ENPropertyChanged.enShow, null);

    }
    public void ShowMain()
    {
        NotifyChanged((int)ENPropertyChanged.enShowMain, null);
    }
    public void HideMain()
    {
        NotifyChanged((int)ENPropertyChanged.enHideMain, null);
    }
    // 更新信息
    public void UpdateTeamInfo()
    {
        NotifyChanged((int)ENPropertyChanged.enUpdateTeam, null);
    }

    public void RemoveTeamItem(int index)
    {
        NotifyChanged((int)ENPropertyChanged.enRemoveTeam, index);
    }

    // 显示更新队伍信息 isCondition 是否要判断 离开界面的条件
    public void ShowUpdateTeamInfo(bool isCondition = true)
    {
        NotifyChanged((int)ENPropertyChanged.enShowTeam, isCondition);
    }
    public void ShowZoneWND()
    {
        NotifyChanged((int)ENPropertyChanged.enShowZone, null);
    }
    public void OnHideZoneWND()
    {
        NotifyChanged((int)ENPropertyChanged.enHideZone, null);
    }
    public void Test()
    {
        //  如果单机
        if (GameSettings.Singleton.m_isSingle )
        {
            Team.Singleton.m_curTeamIndex = 0;
              CSItemGuid temp = new CSItemGuid();
            temp.m_lowPart = 1;

            CSItemGuid temp1 = new CSItemGuid();
            temp1.m_lowPart = 2;

            CSItemGuid temp2 = new CSItemGuid();
            temp2.m_lowPart = 3;

            if (Team.Singleton.GetCard(0, Team.EDITTYPE.enMain) == null)
            {
                CardBag.Singleton.AddCardOffLine(4);
                Team.Singleton.AddTeamMember(0, Team.EDITTYPE.enMain, temp);
            }

            if (Team.Singleton.GetCard(0, Team.EDITTYPE.enDeputy) == null)
            {
                CardBag.Singleton.AddCardOffLine(5);
                Team.Singleton.AddTeamMember(0, Team.EDITTYPE.enDeputy, temp1);
               
            }

//             if (Team.Singleton.GetCard(0, Team.EDITTYPE.enSupport) == null)
//             {
//                 CardBag.Singleton.AddCardOffLine(4);
//                 Team.Singleton.AddTeamMember(0, Team.EDITTYPE.enSupport, temp2);
//                
//             }
        }
    }
}
