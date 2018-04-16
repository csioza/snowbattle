using System;
using System.Collections.Generic;

// 操作条件卡牌列表
public class OperateCardList : IPropertyObject  
{
    public enum ENPropertyChanged
    {
        enShow  =1,
        enShowSelectRepresentiveCard,
        enUpdate,
        enUpdateItemForTeam,
    }

    public enum TYPE
    {
        enSellType =1,              //  出售
        enCardLevelUpBaseCardType ,  //  强化 的基本卡牌列表
        enCardLevelUpDataCardType,  //   强化 的材料卡牌列表
        enCardEvolutionBaseCardType,  //  进化 的基本卡牌列表
        enTeamCardListType,       //  单个队伍编辑 的基本卡牌列表
		enSelectRepresentativeCard,	//选择代表卡
        enCardDivisionUpdate,//卡牌段位升级 材料卡列表
    }
    
    // 当前类型
    public TYPE m_curType                   = TYPE.enSellType;

    public List<CSItemGuid> m_sellList      = null; // 出售列表 

    List<CSItemGuid> m_levelUpList          = null; // 升级 的选择的材料卡牌列表 
    public List<CSItemGuid> LevelUpList { get{ return m_levelUpList;}  }



    // 单个队伍编辑时的 已选队员的总领导力
    public int m_leadShipCost           = 0;

    public CSItemGuid m_curChooseItemGuid      = CSItemGuid.Zero; //  当前点选道具

    public int m_cardDivisionOcc;//卡牌段位升级 素材 职业
    public int m_cardDivisionLevel;//卡牌段位升级 素材 等级
    public int m_cardDivisionStar;//卡牌段位升级 素材 星级
    public int m_cardDivisionMateriaId;//素材ID

    public int m_curChosenIndex = -1;// 当前选中的索引
    public int m_chosenCardNum = 0;

    public CSItemGuid m_oldTeamMember = CSItemGuid.Zero;

    public List<CSItemGuid> m_hadItemList = new List<CSItemGuid>(); // 此列表作为 去除条件列表

    #region Singleton
    static OperateCardList m_singleton;
    static public OperateCardList Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new OperateCardList();
            }
            return m_singleton;
        }
    }
    #endregion

    public OperateCardList()
    {
        SetPropertyObjectID((int)MVCPropertyID.enOperateCardList);
        m_sellList      = new List<CSItemGuid>();
        m_levelUpList    = new List<CSItemGuid>();
    }


    // 显示 出售界面
    public void OnSellCard()
    {
        m_curType = TYPE.enSellType;

        NotifyChanged((int)ENPropertyChanged.enShow, null);
    }

    // 显示 卡牌升级基本卡牌
    public void OnShowUpdateBaseCardList()
    {
        m_curType = TYPE.enCardLevelUpBaseCardType;

        NotifyChanged((int)ENPropertyChanged.enShow, null);
    }

    // 显示 卡牌升级的 材料卡牌
    public void OnShowLevelUpDataCardList()
    {
        m_curType = TYPE.enCardLevelUpDataCardType;

        NotifyChanged((int)ENPropertyChanged.enShow, null);
    }

    // 显示 卡牌进化的基本卡牌
    public void OnShowEvolutionBaseCardList()
    {
        m_curType = TYPE.enCardEvolutionBaseCardType;

        NotifyChanged((int)ENPropertyChanged.enShow, null);
    }
    //显示 卡牌段位升级 材料卡牌
    public void OnShowDivisionCard(int occ, int level, int starNum, int materiaId) 
    {
        m_curType = TYPE.enCardDivisionUpdate;
        m_cardDivisionOcc = occ;
        m_cardDivisionLevel = level;
        m_cardDivisionStar = starNum;
        m_cardDivisionMateriaId = materiaId;
    }


    // 显示 单个队伍编辑的卡牌列表
    public void SetOperateTeamType()
    {
        CSItem oldOne = Team.Singleton.GetCard(Team.Singleton.m_curTeamIndex, Team.Singleton.m_curEditType);
        if ( oldOne == null )
        {
            m_oldTeamMember = CSItemGuid.Zero;
        }
        else
        {
            m_oldTeamMember = oldOne.Guid;
        }

        m_curType = TYPE.enTeamCardListType;
    }

	// 显示 单个队伍编辑的卡牌列表
	public void ShowSelectRepresentiveCard(bool castRootAble = false)
	{
		m_curType = TYPE.enSelectRepresentativeCard;

        NotifyChanged((int)ENPropertyChanged.enShowSelectRepresentiveCard, castRootAble);
	}

    //  为队伍编辑相关更新格子
    public void UpdateItemForTeam()
    {
        NotifyChanged((int)ENPropertyChanged.enUpdateItemForTeam, null);
    }

    // 更新卡牌列表
    public void UpdateCardList()
    {
        NotifyChanged((int)ENPropertyChanged.enUpdate, null);
    }

    // 卡牌升级 所选的材料卡牌列表 清理
    public void ClearUpdateDataList()
    {
        m_levelUpList.Clear();
    }

    // 添加 升级 材料卡牌 选中的 道具
    public void AddLevelUpDataItem( CSItemGuid guid )
    {
        // 如果大于10 则不添加
        if (m_levelUpList.Count >= 10)
        {
            return;
        }

        // 如果不存在则添加 
        if (-1 == m_levelUpList.IndexOf(guid))
        {
            m_levelUpList.Add(guid);
        }
    }

    // 移除 升级 材料卡牌 选中的 道具
    public void RemoveLevelUpDataItem(CSItemGuid guid)
    {
        CSItem item = CardBag.Singleton.GetCardByGuid(guid);

        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(item.IDInTable);
        UnityEngine.Debug.Log("RemoveLevelUpDataItem" + info.StrName);

        m_levelUpList.Remove(guid);
    }

    // 对 升级 卡牌材料列表的 操作 如果已选择则 删除， 没有则添加
    public void LevelUpItemOperation(CSItemGuid guid)
    {
       
        // 如果不存在则添加 
        if (-1 == m_levelUpList.IndexOf(guid))
        {
            // 如果大于10 则不添加
            if (m_levelUpList.Count >= 10)
            {
                return;
            }

            m_levelUpList.Add(guid);
        }
        // 存在则删除
        else
        {
            m_levelUpList.Remove(guid);
        }
    }

    // 对 出售 卡牌列表的 操作 如果已选择则 删除， 没有则添加
    public void SellItemOperation(CSItemGuid guid)
    {

        // 如果不存在则添加 
        if (-1 == m_sellList.IndexOf(guid))
        {
            // 如果大于10 则不添加
            if (m_sellList.Count >= 10)
            {
                return;
            }

            m_sellList.Add(guid);
        }
        // 存在则删除
        else
        {
            m_sellList.Remove(guid);
        }
    }

}
