using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 编队
public class Friend : IPropertyObject
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
    public Friend()
    {
        SetPropertyObjectID((int)MVCPropertyID.enFriend);
    }

    #region Singleton
    static Friend m_singleton;
    static public Friend Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new Friend();
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
