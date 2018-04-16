using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class KillTips : IPropertyObject
{
    // 主角的被杀数量
    int m_beKilledNum  
    {
        get
        {

            return BattleArena.Singleton.GetbeKilledNum();
        }
    }

    // 主角的杀敌数量
    int m_killEnemyNum
    {
        get
        {

            return BattleArena.Singleton.GetkillEnemyNum();
        }
    }

    public enum ENPropertyChanged
    {
        enUpdate = 1,
    }

    public KillTips()
    {
        SetPropertyObjectID((int)MVCPropertyID.enKillTips);
    }

    public void Clear()
    {
        BattleArena.Singleton.ClearKillInfo();
    }

    #region Singleton
    static KillTips m_singleton;
    static public KillTips Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new KillTips();
            }
            return m_singleton;
        }
    }
    #endregion

    //通知UI
    public void Update(string tips)
    {
        NotifyChanged((int)ENPropertyChanged.enUpdate, tips);
    }
   
//     public void SetbeKilledNum(int num)
//     {
//         m_beKilledNum = num;
//     }
// 
//     public void SetkillEnemyNum(int num)
//     {
//         m_killEnemyNum = num;
//     }

    public int GetbeKilledNum()
    {
        return m_beKilledNum;
    }

    public int GetkillEnemyNum()
    {
        return m_killEnemyNum;
    }

}
