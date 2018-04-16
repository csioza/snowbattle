using System;
using System.Collections.Generic;

public class BaseCardOperateProp : IPropertyObject
{

    public BaseCardOperateProp()
    {
        SetPropertyObjectID((int)MVCPropertyID.enBaseCardOperateProp);
    }
    #region Singleton
    static BaseCardOperateProp m_singleton;
    static public BaseCardOperateProp Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new BaseCardOperateProp();
            }
            return m_singleton;
        }
    }
    #endregion

}
