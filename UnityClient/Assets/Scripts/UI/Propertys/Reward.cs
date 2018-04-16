using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 战利品
public class Reward : IPropertyObject
{
    public float m_goldNum      = 0f; // 原来的金币的数目
    public float m_goldDesNum   = 0f; // 增长后的金币数目

    public int m_cardNum        = 0; // 原来契约书的数量
    public int m_cardDesNum     = 0; // 增长后契约书的数量

    public enum ENPropertyChanged
    {
        enEnableKey,
        enNeedKey,
    }

    public Reward()
    {
        SetPropertyObjectID((int)MVCPropertyID.enReward);  
    }

    #region Singleton
    static Reward m_singleton;
    static public Reward Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new Reward();
            }
            return m_singleton;
        }
    }
    #endregion

    public void Reset()
    {
        m_goldNum       = 0.0f;
        m_goldDesNum    = 0.0f;

        m_cardNum       = 0; 
        m_cardDesNum    = 0;
    }

    // 激活钥匙
    public void EnableKey()
    {
        NotifyChanged((int)ENPropertyChanged.enEnableKey, null);
    }

    // 需要钥匙
    public void NeedKey(bool need)
    {
        NotifyChanged((int)ENPropertyChanged.enNeedKey, need);
    }

}
