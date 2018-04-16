using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 编队
public class Guild : IPropertyObject
{
   
    // 编队类型
    public enum EDITTYPE
    {
        enMain,
        enDeputy,
        enSupport,
        enALL,
        enNone,
    }
    public enum ENPropertyChanged
    {
        enShow,
        enHide,

    }
    public Guild()
    {
        SetPropertyObjectID((int)MVCPropertyID.enGuild);
    }

    #region Singleton
    static Guild m_singleton;
    static public Guild Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new Guild();
            }
            return m_singleton;
        }
    }
    #endregion


    public void OnShow()
    {
        NotifyChanged((int)ENPropertyChanged.enShow, null);
    }
    public void OnHide()
    {
        NotifyChanged((int)ENPropertyChanged.enHide, null);
    }
//     public void ShowUpdateTeamInfo(bool isCondition = true)
//     {
//         NotifyChanged((int)ENPropertyChanged.enUpdateTeam, isCondition);
//     }

}
