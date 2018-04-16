using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 编队
public class FriendMessageBox : IPropertyObject
{

    // 编队类型
    public enum EDITTYPE
    {
        enErrorID,
        enErrorSelf,
        enSucceedApply,
        enALL,
        enNone,
    }
    public enum ENPropertyChanged
    {
        enShow,
        enHide,

    }
    public FriendMessageBox()
    {
        //SetPropertyObjectID((int)MVCPropertyID.enFriendMessageBox);
    }

    #region Singleton
    static FriendMessageBox m_singleton;
    static public FriendMessageBox Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new FriendMessageBox();
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
}
