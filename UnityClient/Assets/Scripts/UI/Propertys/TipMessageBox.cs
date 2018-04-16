using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TipMessageBox : IPropertyObject
{
    public enum ENPropertyChanged
    {
        enShowTipMessageBox = 1,
    }
    public TipMessageBox()
    {
        SetPropertyObjectID((int)MVCPropertyID.enTipMessageBox);
    }
    #region Singleton
    static TipMessageBox m_singleton;
    static public TipMessageBox Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new TipMessageBox();
            }
            return m_singleton;
        }
    }
    #endregion

    // 按默认排序方式 显示公会界面
    public void OnShowTipMessageBox()
    {
        NotifyChanged((int)ENPropertyChanged.enShowTipMessageBox, null);
    }
}