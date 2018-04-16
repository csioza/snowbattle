using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 编队
public class Team : IPropertyObject  
{
    // 主角色
    public CSItem Chief { get { return GetCard(m_curTeamIndex, Team.EDITTYPE.enMain); } }

    // 副角色
    public CSItem Deputy { get { return GetCard(m_curTeamIndex, Team.EDITTYPE.enDeputy); } }

    // 支援角色
    public CSItem Support { get { return GetCard(m_curTeamIndex, Team.EDITTYPE.enSupport); } }

    // 战友
    public CSItem Comrade { get;  set; }

    // 当前编辑 队伍角色类型
    public EDITTYPE m_curEditType = EDITTYPE.enMain;

    // 队伍包裹里的 主角色
    public CSItemGuid m_bagMainSlotId       = new CSItemGuid()  ;

    // 队伍包裹里的 副角色格子ID
    public CSItemGuid m_bagDeputySlotId     = new CSItemGuid();

    // 队伍包裹里的 支持角色格子ID
    public CSItemGuid m_bagSupportSlotId    = new CSItemGuid();

    // 当前编辑队伍索引
    public int m_curTeamIndex               = 0;

    // 当前编队总数
    public int TeamNum        { get { return m_teamList.Count; } }

    // 编队类型
    public enum EDITTYPE
    {
        enMain,
        enDeputy,
        enSupport,
        enALL,
        enNone,
    }
    public enum ENPropertyChanged
    {
        enShowTeam,
        enHideTeam,
        enUpdateTeam,
        enShowEditTeam,
        enRemoveTeam,
        enTeamEmpty,
        enTeamMainEmpty,
    }
    // 编队列表
    public List<TeamItem> m_teamList = new List<TeamItem>();

    public Team()
    {
        SetPropertyObjectID((int)MVCPropertyID.enTeam);

    }

    #region Singleton
    static Team m_singleton;
    static public Team Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new Team();
            }
            return m_singleton;
        }
    }
    #endregion

    public void AddTeamMember(int index,EDITTYPE type,CSItemGuid guid)
    {
        //UnityEngine.Debug.Log("AddTeamMember:" + index + "," + type);

        foreach ( TeamItem item in m_teamList )
        {
            if ( item.m_index == index)
            {
                item.m_memberList[(int)type]    = guid;
                return;
            }
        }

        TeamItem teamItem               =  new TeamItem();

        teamItem.m_memberList[(int)type]= guid;
        teamItem.m_index                = index;
        m_teamList.Add(teamItem);
        
    }

    public void RemoveTeamMember(int index, EDITTYPE type)
    {
        UnityEngine.Debug.Log("RemoveTeamMember:" + index + "," + type);

        // 删除整队
        if (type == EDITTYPE.enALL)
        {
            foreach (TeamItem item in m_teamList)
            {
                if (item.m_index == index)
                {
                    m_teamList.Remove(item);
                    return;
                }
            }
        }
        // 删除一个队友
        else
        {
            foreach (TeamItem item in m_teamList)
            {
                if (item.m_index == index)
                {
                    item.m_memberList[(int)type] = CSItemGuid.Zero;
                    return;
                }
            }
        }
    }

    // index 队伍索引 type 类型
    public CSItem GetCard(int index, EDITTYPE type)
    {
        foreach (TeamItem item in m_teamList)
        {
            if (item.m_index == index)
            {
                CSItemGuid guid = item.m_memberList[(int)type];

                return CardBag.Singleton.GetCardByGuid(guid);
            }
        }

        return null;
    }

   

    public void UpdateTeamMember()
    {
        if (m_bagMainSlotId.Equals(CSItemGuid.Zero))
        {
            RemoveTeamMember(m_curTeamIndex, EDITTYPE.enMain);
        }
        else
        {
            AddTeamMember(m_curTeamIndex, EDITTYPE.enMain, m_bagMainSlotId);
        }
        if (m_bagDeputySlotId.Equals(CSItemGuid.Zero))
        {
            RemoveTeamMember(m_curTeamIndex, EDITTYPE.enDeputy);
        }
        else
        {
            AddTeamMember(m_curTeamIndex, EDITTYPE.enDeputy, m_bagDeputySlotId); 
        }


        if (m_bagSupportSlotId.Equals(CSItemGuid.Zero))
        {
            RemoveTeamMember(m_curTeamIndex, EDITTYPE.enSupport);
        }
        else
        {
            AddTeamMember(m_curTeamIndex, EDITTYPE.enSupport, m_bagSupportSlotId);
        }

        // 发送服务器保存 当前队伍索引
        IMiniServer.Singleton.SendSaveOneTeamData(m_curTeamIndex);
        IMiniServer.Singleton.SendSelectTeamIndex(m_curTeamIndex);
    }

    // 获得指定索引的TEAMITEM
    public TeamItem GetTeamItemByIndex(int index)
    {
        foreach (TeamItem item in m_teamList)
        {
            if (item.m_index == index)
            {

                return item;
            }
        }
        return null;
    }

    public void UpdateTeamBagAllSlotId()
    {
        CSItemGuid guid = new CSItemGuid();

        CSItem card = GetCard(m_curTeamIndex, EDITTYPE.enMain);
        if (card == null)
        {
            m_bagMainSlotId = guid;
        }
        else
        {
            m_bagMainSlotId = card.Guid;
        }

        card = GetCard(m_curTeamIndex, EDITTYPE.enDeputy);
        if (card == null)
        {
            m_bagDeputySlotId = guid;
        }
        else
        {
            m_bagDeputySlotId = card.Guid;
        }

        card = GetCard(m_curTeamIndex, EDITTYPE.enSupport);
        if (card == null)
        {
            m_bagSupportSlotId = guid;
        }
        else
        {
            m_bagSupportSlotId = card.Guid;
        }
    }

    // 添加一个战队
    public void ShowEmptyTeam()
    {

        m_curTeamIndex = TeamNum ;

        NotifyChanged((int)ENPropertyChanged.enUpdateTeam, null);
        // 通知界面
        //MainMenu.Singleton.UpdateTeamInfo();

    }

    // 删除一个战队
    public void RemoveTeam()
    {

        if (TeamNum == 1)
        {
            return;
        }

        // 如果不存在
        if (!IsExistTeam(m_curTeamIndex))
        {
            return;
        }
        int index = m_curTeamIndex;

        // 删除
        foreach (TeamItem item in m_teamList)
        {
            if (item.m_index == index)
            {
                m_teamList.Remove(item);
                break;
            }
        }

        // 删除的是 编号最大的队伍
        if (index == TeamNum)
        {
            --index;
        }

        m_curTeamIndex = index;

        // 重新更新 队伍索引
        int i = 0;
        foreach (TeamItem item in m_teamList)
        {
            item.m_index = i;
            i++;
        }


        UnityEngine.Debug.Log("RemoveTeam:" + m_curTeamIndex);
        // 通知界面
        NotifyChanged((int)ENPropertyChanged.enRemoveTeam, null);
       // MainMenu.Singleton.RemoveTeamItem(index);

    }
    // 是否存在指定索引的队伍
    public bool IsExistTeam(int index)
    {
        foreach (TeamItem item in m_teamList)
        {
            if (item.m_index == index)
            {
                return true;
            }
        }

        return false;
    }

    // 更新要战斗的队伍
    public void UpdateBattleTeam()
    {
        CSItem card = Chief;

        if (null != card)
        {
            HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
            if (null != info)
            {
                UnityEngine.Debug.Log("m_chief:" + info.StrName);
            }
        }

        card = Deputy;
        if (null != card)
        {
            HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
            if (null != info)
            {
                UnityEngine.Debug.Log("m_deputy:" + info.StrName);
            }
        }

        card = Support;
        if (null != card)
        {
            HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
            if (null != info)
            {
                UnityEngine.Debug.Log("m_support:" + info.StrName);
            }
        }
        
        // 发送服务器 保存的队伍信息
        IMiniServer.Singleton.SendSaveOneTeamData(m_curTeamIndex);
    }

    // 是否在队伍中
    public bool IsCardInTeam(CSItemGuid guid)
    {
        foreach (TeamItem item in m_teamList)
        {
            if (item.m_memberList[(int)EDITTYPE.enMain].Equals(guid))
            {
                return true;
            }
            else if (item.m_memberList[(int)EDITTYPE.enDeputy].Equals(guid))
            {
                return true;
            }
            else if (item.m_memberList[(int)EDITTYPE.enSupport].Equals(guid))
            {
                return true;
            }
            
        }

        return false;
    }

    // 是否在指定队伍索引的 队伍中
    public bool IsCardInTheTeam(int teamIndex,CSItemGuid guid)
    {
        foreach (TeamItem item in m_teamList)
        {
            if (item.m_index == teamIndex)
            {
                if (item.m_memberList[(int)EDITTYPE.enMain].Equals(guid))
                {
                    return true;
                }
                else if (item.m_memberList[(int)EDITTYPE.enDeputy].Equals(guid))
                {
                    return true;
                }
                else if (item.m_memberList[(int)EDITTYPE.enSupport].Equals(guid))
                {
                    return true;
                }
            }
        }

        return false;
    }

     // 获取 编辑队伍时的 选取的 队友所有消耗力
    public int GetAllCost()
    {
        CSItem card     = null;
        int leadship    = 0;
        HeroInfo info   = null;

        card            = CardBag.Singleton.GetCardByGuid(m_bagMainSlotId);

        if (null != card)
        {
            info        = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
            leadship    = leadship + info.Cost;
        }

        card            = CardBag.Singleton.GetCardByGuid(m_bagDeputySlotId);
        if (null != card)
        {
            info        = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
            leadship    = leadship + info.Cost;
        }

        card            = CardBag.Singleton.GetCardByGuid(m_bagSupportSlotId);
        if (null != card)
        {
            info        = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
            leadship    = leadship + info.Cost;
        }
      

        return leadship;
    }
    // 获取 编辑队伍时的 除了传参进来的 ID的 其他 已点选的 队伍消耗力
    public int GetTheTeamCostExcept(CSItemGuid guid)
    {
        CSItem card         = null;
        int leadship        = 0;
        HeroInfo info       = null;

        if (!m_bagMainSlotId.Equals(guid))
        {
             card           = CardBag.Singleton.GetCardByGuid(m_bagMainSlotId);

            if (null != card)
            {
                info        = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
                leadship    = leadship + info.Cost;
            }
        }

        if (!m_bagDeputySlotId.Equals(guid))
        {
            card            = CardBag.Singleton.GetCardByGuid(m_bagDeputySlotId);

            if (null != card)
            {
                info        = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
                leadship    = leadship + info.Cost;
            }

        }

        if (!m_bagSupportSlotId.Equals(guid))
        {
            card            = CardBag.Singleton.GetCardByGuid(m_bagSupportSlotId);

            if (null != card)
            {
                info        = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);
                leadship    = leadship + info.Cost;
            }
        }


        return leadship;
    }


    //指定索引索引的队伍中 是否是空队伍
    public bool IsTeamEmpty(int teamIndex)
    {
        bool empty = true;
        foreach (TeamItem item in m_teamList)
        {
            if (item.m_index == teamIndex)
            {
                if (!item.IsEmpty())
                {
                    empty = false;
                    break;
                }
            }
        }
        return empty;
    }

    //指定索引索引的队伍中 的指定角色位 是否是空
    public bool IsDutyEmpty(int teamIndex, EDITTYPE dutyType)
    {
        bool empty = true;
        foreach (TeamItem item in m_teamList)
        {
            if (item.m_index == teamIndex)
            {
                if (!item.m_memberList[(int)dutyType].Equals(CSItemGuid.Zero))
                {
                    empty = false;
                    break;
                }
            }
        }
        return empty;
    }

     // 删除空白队伍
    public void RemoveEmpty()
    {
        //队伍 只有一个 则返回
        if (m_teamList.Count ==1 )
        {
            return;
        }

        List<TeamItem> temp = new List<TeamItem>();

        // 删除空白前的 队伍数量
        int oldTeamNum = m_teamList.Count;

        // 找出空队伍
        foreach (TeamItem item in m_teamList)
        {
            // 如果为空
            if (item.IsEmpty())
            {
                temp.Add(item);
            }
        }

        // 删除空队伍
        foreach (TeamItem item in temp)
        {
            UnityEngine.Debug.Log("删除空队伍:" + item.m_index); 
            m_teamList.Remove(item);
        }

        // 更新队伍索引
        int i = 0;
        foreach (TeamItem item in m_teamList)
        {
            item.m_index = i;
            i++;
        }

        // 如果成功删除了空白队伍
        if (oldTeamNum != m_teamList.Count)
        {
            // 通知服务器 队伍数据变化
            for (int k = 0; k < oldTeamNum; k++)
            {
                IMiniServer.Singleton.SendSaveOneTeamData(k);
            }
        }  
    }

    public void OnButtonYes(object sender, EventArgs e)
    {

    }
    public void OnButtonNo(object sender, EventArgs e)
    {

    }
    public void OnShowTeam()
    {
        NotifyChanged((int)ENPropertyChanged.enShowTeam, null);
    }
    public void ShowUpdateTeamInfo(bool isCondition = true)
    {
        NotifyChanged((int)ENPropertyChanged.enUpdateTeam, isCondition);
    }
    public void OnHideWnd()
    {
        NotifyChanged((int)ENPropertyChanged.enHideTeam, null);
    }
}
