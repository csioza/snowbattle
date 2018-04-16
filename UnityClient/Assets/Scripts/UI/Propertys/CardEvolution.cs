using System;
using System.Collections.Generic;

// 卡牌晋级
public class CardEvolution : IPropertyObject  
{
    public enum ENPropertyChanged
    {
        enShowCardEvolution = 1,
        enShowEvolutionDataCardList,
        enUpdateCardEvolution,//卡牌晋级
    }
    #region Singleton
    static CardEvolution m_singleton;
    static public CardEvolution Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new CardEvolution();
            }
            return m_singleton;
        }
    }
    #endregion

    // 当前要进化的卡牌GUID
    public CSItemGuid m_curEvolutionGuid = CSItemGuid.Zero;

    public CardEvolution()
    {
        SetPropertyObjectID((int)MVCPropertyID.enCardEvolution);
     
    }

    // 显示 卡牌晋级界面
    public void OnShowCardEvolution(CSItemGuid guid)
    {
       if ( !guid.Equals(CSItemGuid.Zero) )
       {
           m_curEvolutionGuid = guid;
       }

       NotifyChanged((int)ENPropertyChanged.enShowCardEvolution, null);
    }
    //显示 卡牌段位升级界面


    // // 显示 进化附属的 选择材料界面
    public void OnShowEvolutionDataCardList(int cardId)
    {
        NotifyChanged((int)ENPropertyChanged.enShowEvolutionDataCardList, cardId);
    }

    // 卡牌进化后的服务器的回调
    public void OnHeroCardEvolve(CSItemGuid guid)
    {
        CSItem card = CardBag.Singleton.GetCardByGuid(guid);
        if (null == card)
        {
            return;
        }

        // 显示新卡牌
        CardBag.Singleton.ShowGetNewCard(guid);

        // 清空
        CardEvolution.Singleton.m_curEvolutionGuid      = CSItemGuid.Zero; 

        // 刷新 进化界面
        NotifyChanged((int)CardEvolution.ENPropertyChanged.enUpdateCardEvolution, null);
    }
}
