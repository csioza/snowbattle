using System;
using System.Collections.Generic;

class LoadingResourcesProp : IPropertyObject
{
    public enum ENPropertyChanged
    {
        enShowUILoadingResources = 1,
        enUpDateLoadingValue = 2,
        enHide,
    }
    public LoadingResourcesProp() 
    {
        SetPropertyObjectID((int)MVCPropertyID.enLoadingResourcesProp);
       
    }
    #region Singleton
    static LoadingResourcesProp m_singleton;
    static public LoadingResourcesProp Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new LoadingResourcesProp();
            }
            return m_singleton;
        }
    }
    #endregion

    public void ShowUILoadingResources() 
    {
        NotifyChanged((int)ENPropertyChanged.enShowUILoadingResources, null);
    }
    public void OnHideUILoadingResources()
    {
        NotifyChanged((int)ENPropertyChanged.enHide, null);
    }

    public void Tick() 
    {
        NotifyChanged((int)ENPropertyChanged.enUpDateLoadingValue, null);
    }


}
