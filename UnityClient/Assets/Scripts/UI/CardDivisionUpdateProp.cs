using System;
using System.Collections.Generic;

public class CardDivisionUpdateProp :BaseCardOperateProp
{
    public enum CardDivisionFormula
    {
        FormulaOne = 1,
        FormulaTwo = 2,
        FormulaThree = 3,
        FormulaFour = 4,

    }
    public enum CardDivisionUpdate 
    {
        enChooseCard,
    }
    // 当前要段位升级的卡牌GUID
    public CSItemGuid m_curDivisionCardGuid = CSItemGuid.Zero;
    //当前选择的公式
    public int m_curtFormula = (int)CardDivisionFormula.FormulaOne;
    //选择的素材卡牌
    public Dictionary<int, CSItem> m_chooseCardList = new Dictionary<int, CSItem>();
    //全部的素材卡牌
    public List<CSItem> m_allCardList = new List<CSItem>();
    public CardDivisionUpdateProp()
    {
        SetPropertyObjectID((int)MVCPropertyID.enCardDivisionUpdateProp);
    }
    #region Singleton
    static CardDivisionUpdateProp m_singleton;
    static public CardDivisionUpdateProp Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new CardDivisionUpdateProp();
            }
            return m_singleton;
        }
    }
    #endregion


    public void SetCardCurt(CSItemGuid guid) 
    {
        if (!guid.Equals(CSItemGuid.Zero))
        {
            m_curDivisionCardGuid = guid;
        }
    }

    public void SetCardDivisionFormula(int formula) 
    {
        m_curtFormula = formula;
    }

    public void ClearChooseCardList() 
    {
        m_chooseCardList.Clear();
        m_allCardList.Clear();
    }

    public void ChooseCard(CSItem card,int chooseId) 
    {
        m_chooseCardList[chooseId] = card;
        m_allCardList.Add(card);
        NotifyChanged((int)CardDivisionUpdate.enChooseCard, null);
    }
    public void AddMaterialCard(CSItem card) 
    {
        m_allCardList.Add(card);
    }

    public void SendDivisionUpdate() 
    {
        CSItem card = CardBag.Singleton.GetCardByGuid(m_curDivisionCardGuid);
        //发送卡牌升段消息
        MiniServer.Singleton.SendCardDivisionUpdate(card, m_curtFormula, m_allCardList);
        ClearChooseCardList();
    }


}
