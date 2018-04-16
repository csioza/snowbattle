using System;
using System.Collections.Generic;

// 卡牌进化
public class CardUpdateProp : IPropertyObject  
{
    public enum ENPropertyChanged
    {
        enShowCardUpdate = 1,
        enFlushCardUpdate,
        enSuccess,
    }

    #region Singleton
    static CardUpdateProp m_singleton;
    static public CardUpdateProp Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new CardUpdateProp();
            }
            return m_singleton;
        }
    }
    #endregion

    // 卡牌升级的当前 要升级的卡牌格子ID
    public CSItemGuid m_curLevelGuid        = CSItemGuid.Zero;

    // 强化成功后的 卡牌
    public CSItemGuid m_levelUpAfterGuid    = CSItemGuid.Zero;

    // 升级前的等级
    int m_oldLevel                          = 0;

    // 升级前的 等级字符串
    public string m_oldLevelTips            = "";

    // 升级前的 物理攻击力
    public float m_oldPhyAtk                = 0f;

    // 升级前的 魔法攻击力
    public float m_oldMagAtk                = 0f;

    // 升级前的 生命值
    public float m_oldHP                    = 0f;

    // 升级前的 突破次数
    public int m_oldBreak                   = 0;

    // 刚解锁的技能列表
    public List<int> m_unlockSkillList      = new List<int>();

    Dictionary<int, int> m_skillDataList = new Dictionary<int, int>(); // 记录的技能列表 skillID ,Level

    public CardUpdateProp()
    {
        SetPropertyObjectID((int)MVCPropertyID.enCardUpdate);
    }

    // 清理升级卡牌相关数据
    public void ClearUpdateCardData()
    {
        OperateCardList.Singleton.ClearUpdateDataList();
        m_curLevelGuid.Reset();
    }

    // 刷新 升级卡牌相关数据
    public void FlushUpdateCardData()
    {
        NotifyChanged((int)ENPropertyChanged.enFlushCardUpdate, null);
    }

    // 显示 升级界面
    public void OnShowCardUpdate( CSItemGuid guid )
    {
        if ( guid == CSItemGuid.Zero )
        {
            return;
        }

        // 如果选择的基本卡牌换了 则清空材料卡牌列表
        if (!guid.Equals(CSItemGuid.Zero) && !guid.Equals(m_curLevelGuid))
        {
            OperateCardList.Singleton.ClearUpdateDataList();
        }

        if (!guid.Equals(CSItemGuid.Zero))
        {
            m_curLevelGuid = guid;
        }
       

        NotifyChanged((int)ENPropertyChanged.enShowCardUpdate, null);

    }

    // 获得某个 静态卡牌 在某一等级 可以提供的经验
    public int GetCardExpByLevel(int idInTable,int level)
    {
        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(idInTable);
        if ( null == info )
        {
            return 0;
        }
        return info.ExpSupply * level;
    }

    // 获得 对某个卡牌 赋予 经验值后 的等级和经验
    public void GetLevelUpLVExp(CSItem card,int supplyExp,int breakCounts,out int level,out int exp)
    {
        level   = 0;
        exp     = 0;

       if (null == card)
       {
           return;
       }

       if ( 0 ==supplyExp )
       {
           return;
       }

        int curLevel        = card.Level;
        int curExp          = card.Exp;
        LevelUpInfo info    = GameTable.LevelUpTableAsset.LookUp(curLevel);
        HeroInfo heroInfo   = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);

        // 此卡 还可以突破的次数
        int couldBreakCouts = heroInfo.BreakThroughCount - card.BreakCounts;
        if (couldBreakCouts >0)
        {
            if ( breakCounts > couldBreakCouts)
            {
                breakCounts = couldBreakCouts;
            }
        }
        int totalExp        = supplyExp + curExp;

        int maxLevel        = card.GetMaxLevel() +breakCounts*heroInfo.BreakThroughLevel;

        
        // 代表可以升级
       if (totalExp > info.Monster_Exp)
       {
           // 用一个循环来算 100 是一个 够用范围
           for (int i = 0; i < 100; i++)
           {
               // 不超过最大等级上限
               if (curLevel >= maxLevel)
               {
                   break;
               }
               info     = GameTable.LevelUpTableAsset.LookUp(curLevel);
               curExp   = totalExp - info.Monster_Exp;
               if (curExp < 0)
               {
                   break;
               }
               totalExp = curExp;
               curLevel++;
           }
       }

       level    = curLevel;
       exp      = totalExp;

    }

    // 设置升级前的 老数据
    public void SetOldData()
    {
        CSItem card           = CardBag.Singleton.GetCardByGuid(m_curLevelGuid);
        if ( null ==  card )
        {
            return;
        }

        m_oldLevel      = card.Level;
        m_oldLevelTips  = card.Level + "/" + card.GetMaxLevel();
        m_oldHP         = card.GetHp();
        m_oldBreak      = card.BreakCounts;
        m_oldMagAtk     = card.GetMagAttack();
        m_oldPhyAtk     = card.GetPhyAttack();
        // 记录 技能相关信息
        SetOldSkillData();
    }

    // 记录 技能相关信息
    void SetOldSkillData()
    {
        CSItem card = CardBag.Singleton.GetCardByGuid(m_curLevelGuid);
        if (null == card)
        {
            return;
        }

        m_skillDataList.Clear();

        foreach (var item in card.SkillItemInfoList)
        {
            SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(item.m_skillID);
            if (skillInfo == null) continue;

            // 如果是解锁技能
            if (card.HaveSkill(item.m_skillID))
            {
                m_skillDataList.Add(item.m_skillID, item.m_skillLevel);
            }
        }
        
    }
    // 升级卡牌的 服务器回调
    public void OnS2CLevelUpSuccess(CSItemGuid guid)
    {
        // 先播放一段动画

        m_levelUpAfterGuid = guid;

        OperateCardList.Singleton.LevelUpList.Clear();

        NotifyChanged((int)ENPropertyChanged.enSuccess, null);


        // 因为 升级与突破 拆开 所以不存在 换卡ID的情况 

        // 显示技能升级界面
        SkillLevelUp();

    }

    // 显示技能升级界面
    void SkillLevelUp()
    {
        CSItem card = CardBag.Singleton.GetCardByGuid(m_curLevelGuid);
        if (null == card)
        {
            return;
        }
        LevelUp.Singleton.m_curState = LevelUp.LevelState.enCardUpdate;

        LevelUp.Singleton.ClearSkillList();

        foreach (var item in card.SkillItemInfoList)
        {
            SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(item.m_skillID);
            if (skillInfo == null) continue;

            // 如果是解锁技能
            if (card.HaveSkill(item.m_skillID))
            {
                // 如果记录中的 等级和 最新等级不一样则 为升级
                if (m_skillDataList.ContainsKey(item.m_skillID))
                {
                    if (item.m_skillLevel > m_skillDataList[item.m_skillID])
                    {
                        SkillLevelUpInfo info = new SkillLevelUpInfo();
                        info.m_skillId = item.m_skillID;
                        info.m_skillLevel = item.m_skillLevel;

                        LevelUp.Singleton.m_skillList.Add(info);
                        UnityEngine.Debug.Log("chief SkillID :" + item.m_skillID + "Level up " + item.m_skillLevel);
                    }
                }


            }
        }

        LevelUp.Singleton.ShowSkill();
    }
    // 解锁的技能列表
    public void UnlockSkillList()
    {
        m_unlockSkillList.Clear();

        CSItem card = CardBag.Singleton.GetCardByGuid(CardUpdateProp.Singleton.m_levelUpAfterGuid);
        if (null == card)
        {
            return;
        }

        int level   = card.Level;

        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
        if ( null == heroInfo )
        {
            return;
        }
       
        for ( int i =0; i <heroInfo.UnlockSkillLevelList.Count;i++ )
        {
            int temp = heroInfo.UnlockSkillLevelList[i];
            if (  temp > m_oldLevel && temp <= level )
            {
                m_unlockSkillList.Add(heroInfo.AllSkillIDList[i]);
            }
        }
           
    }
}
