using System;
using System.Collections.Generic;

class GameSetingProp : IPropertyObject
{
    public float m_doubleClickValve;
    public float m_soundVolume;
    public enum ENPropertyChanged
    {
        enCloseOtherPanel = 1,
        enChangeName    = 2,
        enHide,
    }

    public GameSetingProp() 
    {
        SetPropertyObjectID((int)MVCPropertyID.enGameSeting);
        BattleArena.Singleton.m_roomIsRolling = true;
        m_soundVolume = 1.0f;
        m_doubleClickValve = 1.0f;
    }

    public void OnCloseOtherChild() 
    {
        NotifyChanged((int)ENPropertyChanged.enCloseOtherPanel, null);
    }

    public void OnChangeName() 
    {
        NotifyChanged((int)ENPropertyChanged.enChangeName, null);        
    }

    #region Singleton
    static GameSetingProp m_singleton;
    static public GameSetingProp Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new GameSetingProp();
            }
            return m_singleton;
        }
    }
    #endregion
}
