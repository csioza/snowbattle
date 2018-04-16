using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 编队
public class FoundFriendByID : IPropertyObject
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
    //二次确认框提示类型
    public enum ENMsgInfoType
    {
        enErrorID,
        enErrorSelf,
        enSuccess,
    }
    public enum ENPropertyChanged
    {
        enShow,
        enHide,
        enLookupPlayer,

    }
    public FriendItem m_findFriendItem = new FriendItem();
    public FoundFriendByID()
    {
        SetPropertyObjectID((int)MVCPropertyID.enFoundFriendByID);
    }

    #region Singleton
    static FoundFriendByID m_singleton;
    static public FoundFriendByID Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new FoundFriendByID();
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
    public void OnShowSuccessMsgBox(FriendItem item)
    {
        m_findFriendItem = item;
        NotifyChanged((int)ENPropertyChanged.enLookupPlayer, null);
    }
    //     public void ShowUpdateTeamInfo(bool isCondition = true)
    //     {
    //         NotifyChanged((int)ENPropertyChanged.enUpdateTeam, isCondition);
    //     }

}
