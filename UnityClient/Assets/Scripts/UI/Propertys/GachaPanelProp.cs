using System;
using System.Collections.Generic;

class GachaPanelProp : IPropertyObject
{

    public List<int> m_slotList = new List<int>();
    public int m_cardRarity;
    public GachaPanelProp() 
    {
        SetPropertyObjectID((int)MVCPropertyID.enGachaPanelProp);
    }

    #region Singleton
    static GachaPanelProp m_singleton;
    static public GachaPanelProp Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new GachaPanelProp();
            }
            return m_singleton;
        }
    }
    #endregion

    public void AddCardSlot(int slot) 
    {
        CSItem item = User.Singleton.ItemBag.GetItem(slot);
        if (null != item)
        {
            HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(item.m_id);
            if (null != heroInfo && 0 == m_cardRarity)
            {
                m_cardRarity = heroInfo.Rarity;
            }
        }
        m_slotList.Add(slot);
    }
    public void ResetSlotList() 
    {
        m_slotList.Clear();
        m_cardRarity = 0;
    }
}