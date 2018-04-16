using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 编队
public class Zone : IPropertyObject
{

    // 编队类型
    public enum EDITTYPE
    {

    }
    public enum ENPropertyChanged
    {
        enShow,
        enHide,
    }

    public Zone()
    {
        SetPropertyObjectID((int)MVCPropertyID.enZone);
    }

    #region Singleton
    static Zone m_singleton;
    static public Zone Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new Zone();
            }
            return m_singleton;
        }
    }
    #endregion

    //通知UI
    public void ShowZone()
    {
        MainButtonList.Singleton.m_curShowType = MainButtonList.SHOWWNDTYPE.ENMain;
        NotifyChanged((int)ENPropertyChanged.enShow, null);
    }
    public void HideZone()
    {
        NotifyChanged((int)ENPropertyChanged.enHide, null);
    }

}
