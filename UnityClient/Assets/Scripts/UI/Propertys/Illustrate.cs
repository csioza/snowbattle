using System;
using System.Collections.Generic;
using UnityEngine;


public class IllustratedInfo
{
    public int m_id;//卡牌表索引ID
    public int m_cardId;//卡牌编号ID
    public int m_type;//种族
    public int m_occupation;//职业
    public int m_rarity;//稀有度
    public int m_gainLv;//获取等级
}

public enum ENGainLevel
{
    enNone = 0,//没有
    enSee = 1,//看过但是没有
    enHave = 2,//拥有
}

// 图鉴
public class Illustrate : IPropertyObject  
{
    #region Singleton
    static Illustrate m_singleton;
    static public Illustrate Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new Illustrate();
            }
            return m_singleton;
        }
    }
    #endregion

    public List<IllustratedInfo> m_cardList  = new List<IllustratedInfo>();

    public Illustrate()
    {
        SetPropertyObjectID((int)MVCPropertyID.enIllustrate);
    }

    // 获取指定ID卡牌的获取等级
    public ENGainLevel GetIllustratedGainLv(int cardId)
    {

        for ( int i = 0; i < m_cardList.Count;i++ )
        {
            if (m_cardList[i].m_id == cardId)
            {
                return (ENGainLevel)m_cardList[i].m_gainLv;
            }
        }

        return ENGainLevel.enNone;
    }
  

}
