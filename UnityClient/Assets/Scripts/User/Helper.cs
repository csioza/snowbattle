using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HelpType
{
    enAdventurers   = 1, //冒险者
    enFriend       ,       //好友
    enGroupMember  ,       //公会
}

// 战友的数据结构
public class Helper
{
	// GUID
	public int m_userGuid           = 0;
	// 战友名称
	public string m_userName        = "";	
	// 战友等级
	public int m_userLevel          = 0;
	//战友卡GUID
	public CSItemGuid m_cardGuid;	
	// 代表卡牌表中ID
	public int m_cardId             = 0;	
	// 代表卡牌等级
	public int m_cardLevel          = 0;	
	// 代表卡牌突破次数
	public int m_cardBreakCounts    = 0;
    // 战友类型
	public int m_type               = 0;
    // 已选择次数
    public int m_chosenNum          = 0;
	//上次登录的时间
	public int m_lastLoginTime		= 0;
	//卡牌技能
	public HeroCardSkill[] m_cardSkill = new HeroCardSkill[8];	//技能
}

public class BattleHelperList
{
	List<Helper> m_helpers = new List<Helper>();
	public List<Helper> BattleHelpers { get { return m_helpers; } private set { m_helpers = value; } }

	public void AddHelperCard(Helper newHelper)
	{
		Helper existHelper = m_helpers.Find(helper => helper.m_userGuid == newHelper.m_userGuid);
		if (existHelper == null)
		{
			m_helpers.Add(newHelper);
		}
	}

	public void RemoveHelperCard(int userGUID)
	{
		for (int index = 0; index < m_helpers.Count; index ++ )
		{
			if (m_helpers[index].m_userGuid == userGUID )
			{
				m_helpers.RemoveAt(index);
				return;
			}
		}
	}
	public Helper LookupHelper(int userGUID)
	{
		return m_helpers.Find(helper => helper.m_userGuid == userGUID);
	}

    public void Clear()
    {
        m_helpers.Clear();
        BattleHelpers.Clear();

    }
    // 脱机测试数据
    public void Test()
    {
        CSItemGuid guid     = new CSItemGuid();
        guid.m_highPart     = 10001;

        Helper help         = new Helper();
        help.m_userName     = "肥子";
        help.m_userLevel    = 10;
        help.m_userGuid     = m_helpers.Count + 1;
        help.m_cardGuid     = guid;
        help.m_type         = 1;
        help.m_cardId       = 5;
        help.m_cardLevel    = 2;
        help.m_cardBreakCounts = 1;

        m_helpers.Add(help);

        guid                = new CSItemGuid();
        guid.m_highPart     = 10002;

        help                = new Helper();
        help.m_userName     = "肥子2";
        help.m_userLevel    = 10;
        help.m_userGuid     = m_helpers.Count + 1;
        help.m_cardGuid     = guid;
        help.m_type         = 2;
        help.m_cardId       = 25;
        help.m_cardLevel    = 2;
        help.m_cardBreakCounts = 1;
        m_helpers.Add(help);

        guid                = new CSItemGuid();
        guid.m_highPart     = 10003;

        help                = new Helper();
        help.m_userName     = "喜洋洋";
        help.m_userLevel    = 10;
        help.m_userGuid     = m_helpers.Count + 1;
        help.m_cardGuid     = guid;
        help.m_type         = 3;
        help.m_cardId       = 25;
        help.m_cardLevel    = 2;
        help.m_cardBreakCounts = 1;
        m_helpers.Add(help);


        guid                = new CSItemGuid();
        guid.m_highPart     = 10004;

        help                = new Helper();
        help.m_userName     = "TG";
        help.m_userLevel    = 10;
        help.m_userGuid     = m_helpers.Count + 1;
        help.m_cardGuid     = guid;
        help.m_type         = 1;
        help.m_cardId       = 25;
        help.m_cardLevel    = 2;
        help.m_cardBreakCounts = 1;
        m_helpers.Add(help);


        guid                = new CSItemGuid();
        guid.m_highPart     = 10005;
        help                = new Helper();
        help.m_userName     = "笑锅";
        help.m_userLevel    = 10;
        help.m_userGuid     = m_helpers.Count + 1;
        help.m_cardGuid     = guid;
        help.m_type         = 1;
        help.m_cardId       = 25;
        help.m_cardLevel    = 2;
        help.m_cardBreakCounts = 1;

        m_helpers.Add(help);


        guid                = new CSItemGuid();
        guid.m_highPart     = 10006;
        help                = new Helper();
        help.m_userName     = "小云云";
        help.m_userLevel    = 10;
        help.m_userGuid     = m_helpers.Count + 1;
        help.m_cardGuid     = guid;
        help.m_type         = 2;
        help.m_cardId       = 25;
        help.m_cardLevel    = 2;
        help.m_cardBreakCounts = 1;
        m_helpers.Add(help);


        guid                = new CSItemGuid();
        guid.m_highPart     = 10007;
        help                = new Helper();
        help.m_userName     = "宋宋";
        help.m_userLevel    = 10;
        help.m_userGuid     = m_helpers.Count + 1;
        help.m_cardGuid     = guid;
        help.m_type         = 2;
        help.m_cardId       = 25;
        help.m_cardLevel    = 2;
        help.m_cardBreakCounts = 1;
        m_helpers.Add(help);


        guid                = new CSItemGuid();
        guid.m_highPart     = 10008;
        help                = new Helper();
        help.m_userName     = "大熊";
        help.m_userLevel    = 10;
        help.m_userGuid     = m_helpers.Count + 1;
        help.m_cardGuid     = guid;
        help.m_type         = 2;
        help.m_cardId       = 25;
        help.m_cardLevel    = 2;
        help.m_cardBreakCounts = 1;
        m_helpers.Add(help);
    }
}