using System;
using System.Collections.Generic;

// LOading
public class Loading : IPropertyObject  
{
    public enum ENPropertyChanged
    {
		enShow = 1,
		enHide,
		enServerLog,
    }

    // 提示内容ID
    public int m_tipsId = 1;

    public int m_conFailedNum = 0; // 连接失败 二次弹框的 次数

    public Loading()
    {
        SetPropertyObjectID((int)MVCPropertyID.enLoading);

    }

    #region Singleton
    static Loading m_singleton;
    static public Loading Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new Loading();
            }
            return m_singleton;
        }
    }
    #endregion

    // id 提示内容ID
    public void SetLoadingTips( int id  = 1 )
    {
        m_tipsId = id;
        //NotifyChanged((int)ENPropertyChanged.enShow, null);        
    }

    public void Hide()
    {
        NotifyChanged((int)ENPropertyChanged.enHide, null);
    }
}

