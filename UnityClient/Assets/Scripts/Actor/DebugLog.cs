using System;
using System.Collections.Generic;
using UnityEngine;

// 
public class DebugLog : IPropertyObject  
{
    #region Singleton
    static DebugLog m_singleton;
    static public DebugLog Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new DebugLog();
            }
            return m_singleton;
        }
    }
    #endregion

    public string m_log;

    public enum ENPropertyChanged
    {
        enShow = 1,
  
    }
    public DebugLog()
    {
        SetPropertyObjectID((int)MVCPropertyID.enDebugLog);

        // 调试信息显示窗口
        UIDebugLog.GetInstance().HideWindow();
    }

    public void OnShowLog( string str )
    {
        m_log = str +"\n";

        NotifyChanged((int)ENPropertyChanged.enShow, null);
    }

   
}
