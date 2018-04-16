using System;
using System.Collections.Generic;

// 二次确认
public class PopWnd : IPropertyObject  
{
    public enum ENPropertyChanged
    {
       enShow = 1,
    }


    public PopWnd()
    {
        SetPropertyObjectID((int)MVCPropertyID.enPopWnd);

    }

    #region Singleton
    static PopWnd m_singleton;
    static public PopWnd Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new PopWnd();
            }
            return m_singleton;
        }
    }
    #endregion

    public void Show()
    {
        NotifyChanged((int)ENPropertyChanged.enShow, null);
    }
}

