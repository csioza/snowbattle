using System;
using System.Collections.Generic;

// 扩充背包
public class ExpandBag : IPropertyObject  
{
    public enum ENPropertyChanged
    {
       enShowExpandBag = 1,
    }


    public enum ExpandType
    {
        enExpand1 = 1,
        enExpand2 = 2,
        enExpand3 = 3,
    }

    public int m_showOptionNum  = 1; // 显示选择的种类个数

    public int m_expandType     = 1;//扩充类型 1 扩充5 2 扩充20 3 扩充50

    public ExpandBag()
    {
        SetPropertyObjectID((int)MVCPropertyID.enExpandBag);

       
    }

    #region Singleton
    static ExpandBag m_singleton;
    static public ExpandBag Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new ExpandBag();
            }
            return m_singleton;
        }
    }
    #endregion


    public void ShowExpandBag()
    {
        NotifyChanged((int)ENPropertyChanged.enShowExpandBag, null);
    }
}



