using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//!带属性改变通知的基类接口
public abstract class IPropertyObject
{
    protected int m_objectID = 0;
    public int ObjectPropertyID { get { return m_objectID; } }
    public void NotifyChanged(int eventType,object eventObj)
    {
        PropertyNotifyManager.Singleton.NotifyChange(m_objectID, eventType, this, eventObj);
    }
	public void ReleaseNotifys()
    {
        PropertyNotifyManager.Singleton.RemoveAllReceiver(m_objectID);
    }
    protected void SetPropertyObjectID(int objID)
    {
        m_objectID = objID;
    }

}

//!属性通知管理器
public class PropertyNotifyManager
{
    public delegate void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj);
    Dictionary<int, List<OnPropertyChanged> > m_receivers=new Dictionary<int, List<OnPropertyChanged> >();
    #region Singleton
    static PropertyNotifyManager m_singleton;
    static public PropertyNotifyManager Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new PropertyNotifyManager();
            }
            return m_singleton;
        }
    }
    #endregion
    public void NotifyChange(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (!m_receivers.ContainsKey(objectID))
        {
            return;            
        }
        List<OnPropertyChanged> notifyList=m_receivers[objectID];
        for (int i = 0; i < notifyList.Count;)
        {
            int cacheCount = notifyList.Count;
            OnPropertyChanged notify = notifyList[i];
            notify(objectID, eventType, obj, eventObj);
            if (cacheCount >= notifyList.Count)
            {
                i++;
            }
        }
    }
    //!注册一个属性改变回调
    public void RegisterReceiver(int objectID,OnPropertyChanged callback)
    {
        List<OnPropertyChanged> notifyList = null;
        if (!m_receivers.ContainsKey(objectID))
        {
            notifyList = new List<OnPropertyChanged>();
            m_receivers[objectID] = notifyList;
        }
        else
        {
            notifyList = m_receivers[objectID];
        }
        notifyList.Add(callback);
    }
    public void RemoveReceiver(int objectID, OnPropertyChanged callback)
    {
        if (!m_receivers.ContainsKey(objectID))
        {
            return;
        }
        List<OnPropertyChanged> notifyList = m_receivers[objectID];
        foreach (OnPropertyChanged notify in notifyList)
        {
            if (notify == callback)
            {
                notifyList.Remove(callback);
                break;
            }
        }
    }
    public void RemoveAllReceiver(int objectID)
    {
        if (m_receivers.ContainsKey(objectID))
        {
            m_receivers.Remove(objectID);
        }
    }
}