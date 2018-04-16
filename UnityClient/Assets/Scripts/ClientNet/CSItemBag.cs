using NGE.Network;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

//!道具guid,适配lua
public struct CSItemGuid
{
	private static CSItemGuid m_zero = new CSItemGuid();
	public static CSItemGuid Zero { get { return m_zero; } }
	public int m_lowPart;
	public int m_highPart;

	public static bool operator ==(CSItemGuid a, CSItemGuid b)
	{
		return (a.m_lowPart == b.m_lowPart) && (a.m_highPart == b.m_highPart);
	}
	public static bool operator !=(CSItemGuid a, CSItemGuid b)
	{
		return (a.m_lowPart != b.m_lowPart) || (a.m_highPart != b.m_highPart);
	}
	public override bool Equals(object obj)
	{
		if (null == obj)
		{
			return false;
		}
		if (obj.GetType() != this.GetType())
		{
			return false;
		}
		CSItemGuid other = (CSItemGuid)obj;
		return this == other;
	}
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

    public void Reset()
    {
        m_lowPart   = 0;
        m_highPart  = 0;
    }
 
};

//todo  以前的道具 .先改个名字占坑.待删除
public class CSOldItem
{
	public CSItemGuid m_guid;
	public short m_id;
	public short m_flag;
	//放其他的数据段
	public int exp;
	public int Level;                           //装备等级
	public int m_aptitude;                   //装备资质ID
	public float m_aptitudePer;                 //资质随即百分比
	public bool m_isBind;
	public int m_addPropData1;
	public int m_addPropData2;
	public int m_addPropData3;
	public int m_addPropData4;
};


//道具属性
public struct HeroCardSkill
{
	public int m_skillID;
	public int m_skillLevel;
	public int m_skillExp;
	public void Read(PacketReader reader)
	{
		m_skillID		= reader.ReadInt32();
		m_skillLevel	= reader.ReadInt32();
		m_skillExp      = reader.ReadInt32();
	}
	public void Write(PacketWriter writer)
	{
		writer.Write(m_skillID);
		writer.Write(m_skillLevel);
		writer.Write(m_skillExp);
	}
};
public class SegmentHeroCard
{

	public int m_exp;					//经验
	public short m_level;				//等级		
	public bool m_favorite;			//最爱标记(保护)
	public byte m_breakthrouthTimes;	//突破次数
	public uint m_createTime;			//获取时间
	public short[] m_equip = new short[3];			//装备
	public HeroCardSkill[] m_skill = new HeroCardSkill[8];	//技能

	public void Read(PacketReader reader)
	{
		m_exp			= reader.ReadInt32();
		m_level			= reader.ReadInt16();
		m_favorite		= (reader.ReadByte() == 1);
		m_breakthrouthTimes = reader.ReadByte();
		m_createTime	= reader.ReadUInt32();
		for (int index = 0; index < m_equip.Length; index++)
		{
			m_equip[index] = reader.ReadInt16();
		}
		short blockShort = reader.ReadInt16(); //字节对齐补读。
		for (int index = 0; index < m_skill.Length; index++)
		{
			m_skill[index].Read(reader);
		}			
	}
	public void Write(PacketWriter writer)
	{
		writer.Write(m_exp);
		writer.Write(m_level);
		writer.Write(m_favorite);
		writer.Write(m_breakthrouthTimes); 
		writer.Write(m_createTime);
		for (int index = 0; index < m_equip.Length; index++)
		{
			writer.Write(m_equip[index]);
		}
		writer.Write((short)0); //字节对齐补写
		for (int index = 0; index < m_skill.Length; index++)
		{
			m_skill[index].Write(writer);
		}		
	}
	//重载，BinaryHelper中读
	public void Read(BinaryHelper helper)
	{
		m_exp = helper.ReadInt();
		m_level = helper.ReadShort();
		m_favorite = (helper.ReadByte() == 1);
		m_breakthrouthTimes = helper.ReadByte();
		m_createTime = helper.ReadUInt();
		for (int index = 0; index < m_equip.Length; index++)
		{
			m_equip[index] = helper.ReadShort();
		}
		short blockShort = helper.ReadShort();//字节对齐补读
		for (int index = 0; index < m_skill.Length; index++)
		{
			m_skill[index].m_skillID = helper.ReadInt();
			m_skill[index].m_skillLevel = helper.ReadInt();
			m_skill[index].m_skillExp= helper.ReadInt();
		}
	}
};

public class ItemAllSegment
{
	public SegmentHeroCard m_heroCard = new SegmentHeroCard();
	public void Read(BinaryHelper helper)
	{
		m_heroCard.Read(helper);
	}
};


public class SortableItem
{
//     public virtual int GetID();
//     public virtual int beforLoadTime();
// 
//     public virtual CSItem GetItem();
//     public virtual long GetTime();
}


public enum FriendItemType
{

	enFriend = 1,			//好友
	enUntreatedRequest = 1<<1, //未处理的好友
    enAll = enFriend | enUntreatedRequest,
}

public class FriendItem : SortableItem
{
    public int m_id;
    public int m_level;
    public int m_choiceCount;
    public long m_beforLoadTime;
    public string m_actorName;
	public int m_relation;

    public CSItem m_itemData;
    public int GetID()
    {
        return m_id;
    }
    public long beforLoadTime()
    {
        return m_beforLoadTime;
    }
    public CSItem GetItem()
    {
        return m_itemData;
    }
    public long GotTime { get { return m_beforLoadTime; } set { m_beforLoadTime = value; } }

	public void Read(PacketReader p)
	{
		m_id = p.ReadInt32();
		m_actorName = p.ReadUTF8(16);
		m_level = p.ReadInt32();
		m_beforLoadTime = p.ReadInt32();
		m_choiceCount = p.ReadInt32();
		m_itemData = new CSItem();
		m_relation = p.ReadInt32();
		m_itemData.Read(p);
	}

	//用战友的数据构建好友结构。todo
	public static FriendItem CreateNewWithBattleHelper(Helper helper)
	{
		FriendItem item = new FriendItem();
		item.m_id = helper.m_userGuid;
		item.m_level = helper.m_userLevel;
		item.m_choiceCount = helper.m_chosenNum;
		item.m_relation = 0;
		item.m_beforLoadTime = helper.m_lastLoginTime;
		item.m_actorName = helper.m_userName;
		item.m_itemData = new CSItem();
		item.m_itemData.m_id = (short)helper.m_cardId;
		item.m_itemData.m_guid = helper.m_cardGuid;
		item.m_itemData.Level = helper.m_cardLevel;
		item.m_itemData.BreakCounts = helper.m_cardBreakCounts;
		return item;
	}
}

public class CSItem : SortableItem
{
	public CSItemGuid m_guid;
	public short m_id;
	public short m_flag = 0;
	public ItemAllSegment m_segment = new ItemAllSegment();

    //  当前等级
    public int Level { get { return m_segment.m_heroCard.m_level; } set { m_segment.m_heroCard.m_level = System.Convert.ToInt16(value); } }

    // 当前经验值
    public int Exp { get { return m_segment.m_heroCard.m_exp; } set { m_segment.m_heroCard.m_exp = value; } }

    // 是否加入收藏
    public bool Love { get { return m_segment.m_heroCard.m_favorite; } set { m_segment.m_heroCard.m_favorite = value; } }

    // 入手时间
	public uint GotTime { get { return m_segment.m_heroCard.m_createTime; } set { m_segment.m_heroCard.m_createTime = value; } }

    // 已突破次数
    public int BreakCounts { get { return m_segment.m_heroCard.m_breakthrouthTimes; } set { m_segment.m_heroCard.m_breakthrouthTimes =  System.Convert.ToByte(value); } }

    // 表中ID 
    public short IDInTable { get { return m_id; }  set { m_id = value; } }

    // 唯一id
    public CSItemGuid Guid { get { return m_guid; }  set { m_guid = value; } }

    // 技能列表
    public HeroCardSkill[] SkillItemInfoList { get { return m_segment.m_heroCard.m_skill; } }

    // 切入技
    int m_switchSkillID;
    public int SwitchSkillID { get { return m_switchSkillID; } private set { m_switchSkillID = value; } }
    //切入技等级
    int m_switchSkillLevel = 0;
    public int SwitchSkillLevel
    {
        get
        {
            if (m_switchSkillLevel == 0)
            {
                if (SkillItemInfoList != null)
                {
                    foreach (var item in SkillItemInfoList)
                    {
                        if (SwitchSkillID == item.m_skillID)
                        {
                            m_switchSkillLevel = item.m_skillLevel;
                            break;
                        }
                    }
                }
            }
            return m_switchSkillLevel;
        }
    }

    //是否判断技能
    public bool isCheckSkill = true;

    public CSItem()
    {

    }

    public void Init()
    {
        // 测试用， 先把技能全部 放进去
        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(IDInTable);
        if (null != heroInfo)
        {
            if (GameSettings.Singleton.m_isSingle)
            {
                foreach (int skillId in heroInfo.AllSkillIDList)
                {
                    HeroCardSkill info = new HeroCardSkill();
                    info.m_skillID = skillId;

                    AddSkill(info);
                }
                foreach (var item in heroInfo.PassiveSkillIDList)
                {//被动技能
                    HeroCardSkill info = new HeroCardSkill();
                    info.m_skillID = item;

                    AddSkill(info);
                }
            }
           
            SwitchSkillID = heroInfo.SwitchSkillID;
        }
    }
    public SegmentHeroCard GetHeroCardPart()
    {
        return m_segment.m_heroCard;
    }
	public HeroInfo GetHeroInfo()
	{
		return GameTable.HeroInfoTableAsset.Lookup(IDInTable);
	}
    public int GetHp()
    {
        return BattleFormula.GetHp(m_id, Level);
    }

    // 获得当前物理攻击力
    public int GetPhyAttack()
    {

        return BattleFormula.GetPhyAttack(m_id, Level);
    }

    // 获得当前魔法攻击力
    public int GetMagAttack()
    {
        return BattleFormula.GetMagAttack(m_id, Level);
    }

    // 获得当前魔法防御力
    public int GetMagDefend()
    {
        return BattleFormula.GetMagDefend(m_id, Level);
    }

    // 获得当前物理防御力
    public int GetPhyDefend()
    {
        return BattleFormula.GetPhyDefend(m_id, Level);
    }


    // 添加技能
    public void AddSkill(HeroCardSkill info)
    {
       // 更新
       for (int i = 0 ;i < SkillItemInfoList.Length;i++ )
       {
           if ( info.m_skillID == SkillItemInfoList[i].m_skillID)
           {
               SkillItemInfoList[i].m_skillExp = info.m_skillExp;
               return;
           }
       }

        // 如果没有则添加
       for (int i = 0; i < SkillItemInfoList.Length; i++)
       {
           if (SkillItemInfoList[i].m_skillID == 0 )
           {
               SkillItemInfoList[i] = info;
               return;
           }
       }
    }

    // 删除技能
    public void RemoveSkill(int skillId)
    {
        for (int i = 0; i < SkillItemInfoList.Length; i++)
        {
            if (skillId == SkillItemInfoList[i].m_skillID)
            {
                SkillItemInfoList[i].m_skillID      = 0;
                SkillItemInfoList[i].m_skillExp     = 0;
                return;
            }
        }
    }

    // 获得技能
    public HeroCardSkill GetSkillInfo(int skillId)
    {
        HeroCardSkill temp = new HeroCardSkill();
        for (int i = 0; i < SkillItemInfoList.Length; i++)
        {
            if (skillId == SkillItemInfoList[i].m_skillID)
            {
                return SkillItemInfoList[i];
            }
        }
        return temp;
    }

    // 是否可强化 用于显示 强化按钮
    public bool IsStengthen()
    {
        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(IDInTable);
        if (heroInfo.MaxLevel == 1)
        {
            return false;
        }

        return true;
    }

    // 是否可以进化 用于显示 进化按钮
    public bool IsEvlotion()
    {
        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(IDInTable);
		// 如果进化表为空 则为不可进化
        if (heroInfo.EvolveCardList.Count == 0)
        {
            return false;
        }
        return true;
    }

	public bool ReachEvlotionLevel()
	{
		HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(IDInTable);
		if (heroInfo == null)
		{
			return false;
		}
		return Level >= heroInfo.EvolveNeedLevel;
	}

    // 等级上限
    public int GetMaxLevel()
    {
        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(IDInTable);

        int levelMax = heroInfo.MaxLevel;

        if (levelMax == 0)
        {
            RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(heroInfo.Rarity);

            levelMax = rarityInfo.m_levelMax + BreakCounts * heroInfo.BreakThroughLevel;

        }
        else
        {
            levelMax = levelMax + BreakCounts * heroInfo.BreakThroughLevel;
        }

        return levelMax;
    }
    
    // 是否有指定技能
    public bool HaveSkill(int skillId)
    {
        if (!isCheckSkill)
        {
            return true;
        }
        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(IDInTable);
        if (null == heroInfo)
        {
            return false;
        }

        int needLevel = heroInfo.GetSkillNeedLevel(skillId);

        // 等级不够
        if (Level < needLevel)
        {
            return false;
        }
        
        // 身上无此技能
        for (int i = 0; i < SkillItemInfoList.Length; i++)
        {
            if (SkillItemInfoList[i].m_skillID == skillId)
            {
                return true;
            }
        }

        return false;
    }


      // 是否 是切入技
    public bool IsSwitchSkill(int skillId)
    {
        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(IDInTable);
        if (null == heroInfo)
        {
            return false;
        }

        if (skillId == heroInfo.SwitchSkillID)
        {
            return true;
        }

        return false;
    }
	public void Read(PacketReader reader)
	{
		m_guid.m_lowPart = reader.ReadInt32();
		m_guid.m_highPart = reader.ReadInt32();
		m_id = reader.ReadInt16();
		m_flag = reader.ReadInt16();

		//item的segment 固定读128位
		int length = 128;
		byte[] temp = reader.ReadBuffer(length);
		BinaryHelper helper = new BinaryHelper(temp);
		m_segment.Read(helper);
	}
	public void ReplaceSegmentData(CSItem item)
	{
		m_segment = item.m_segment;
	}
};

public enum ENItemSegmentType
{
	enHeroCard = 0,
	enMax,
};

public enum ENItemTypeFlag
{
	HeroCardFlag = 1,
}

public class CSItemSegmentDefine
{
	#region Singleton
	static CSItemSegmentDefine m_singleton;
	static public CSItemSegmentDefine Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new CSItemSegmentDefine();
			}
			return m_singleton;
		}
	}
	public CSItemSegmentDefine()
	{
		MaxSegmentType = 0;
		InitItemSegments();
	}
	#endregion
	public int MaxSegmentType { get; private set; }
	void InitItemSegments()
	{
		MaxSegmentType = (int)ENItemSegmentType.enMax;
	}
	public int ReadSegment(CSItem item, int segType, PacketReader stream, int bytes)
	{
		int current = stream.Seek(0, SeekOrigin.Current);

		switch ((ENItemSegmentType)segType)
		{
			case ENItemSegmentType.enHeroCard:
				{
					item.m_segment.m_heroCard.Read(stream);
				}
				break;
			default:
				break;
		}

		return stream.Seek(0, SeekOrigin.Current) - current;
	}
	public void WriteSegment(CSItem item, int segType, PacketWriter writer)
	{
		switch ((ENItemSegmentType)segType)
		{
			case ENItemSegmentType.enHeroCard:
				{
					item.m_segment.m_heroCard.Write(writer);
				}
				break;
			default:
				break;
		}
	}

	public void ReadSegmentFromText(CSItem item, int segType, string text)
	{
		//		StringStreamReader reader = new StringStreamReader(text);
		// 		switch ((ENItemSegmentType)segType)
		// 		{
		// 			case ENItemSegmentType.Level:
		// 				{
		// 					item.exp = reader.ReadInt();
		// 					item.Level = reader.ReadInt();
		// 					item.m_aptitude = reader.ReadInt();
		// 					item.m_aptitudePer = reader.ReadFloat();
		// 					item.m_isBind = reader.ReadBool();
		// 					item.m_addPropData1 = reader.ReadInt();
		// 					item.m_addPropData2 = reader.ReadInt();
		// 					item.m_addPropData3 = reader.ReadInt();
		// 					item.m_addPropData4 = reader.ReadInt();
		// 				}
		// 				break;
		// 			case ENItemSegmentType.LevelFlag:
		// 				{
		// 					item.exp = reader.ReadInt();
		// 					item.Level = reader.ReadInt();
		// 					item.m_aptitude = reader.ReadInt();
		// 					item.m_aptitudePer = reader.ReadFloat();
		// 					item.m_isBind = reader.ReadBool();
		// 					item.m_addPropData1 = reader.ReadInt();
		// 					item.m_addPropData2 = reader.ReadInt();
		// 					item.m_addPropData3 = reader.ReadInt();
		// 					item.m_addPropData4 = reader.ReadInt();
		// 				}
		// 				break;
		// 			default:
		// 				break;
		// 		}
	}
	public string WriteSegmentToText(CSItem item, int segType)
	{
		StringStreamWriter writer = new StringStreamWriter();
		// 
		// 		switch ((ENItemSegmentType)segType)
		// 		{
		// 			case ENItemSegmentType.Level:
		// 				{
		// 					writer.Write(item.exp);
		// 					writer.Write(item.Level);
		// 					writer.Write(item.m_aptitude);
		// 					writer.Write(item.m_aptitudePer);
		// 					writer.Write(item.m_isBind);
		// 					writer.Write(item.m_addPropData1);
		// 					writer.Write(item.m_addPropData2);
		// 					writer.Write(item.m_addPropData3);
		// 					writer.Write(item.m_addPropData4);
		// 				}
		// 				break;
		// 			case ENItemSegmentType.LevelFlag:
		// 				{
		// 					writer.Write(item.exp);
		// 					writer.Write(item.Level);
		// 					writer.Write(item.m_aptitude);
		// 					writer.Write(item.m_aptitudePer);
		// 					writer.Write(item.m_isBind);
		// 					writer.Write(item.m_addPropData1);
		// 					writer.Write(item.m_addPropData2);
		// 					writer.Write(item.m_addPropData3);
		// 					writer.Write(item.m_addPropData4);
		// 				}
		// 				break;
		// 			default:
		// 				break;
		// 		}
		return writer.ToString();
	}
};

public class CSItemBag
{
	List<CSItem> m_items = new List<CSItem>();
    public List<CSItem> itemList { get { return m_items; } private set { m_items = value; } }

	public void Deserialize(PacketReader stream)
	{
		int mask = stream.ReadInt32();
		if ((mask & (int)ENSerializeMask.enSerializeDirty) > 0)
		{
			Debug.Log("Sync item bag error, mask & enSerializeDirty > 0 ");
			return;
		}
		byte version = stream.ReadByte();
		int count = stream.ReadInt32();
		m_items.Clear();
		//MyEquips.Singleton.myEquips.Clear();
		CSItemSegmentDefine segmentDef = CSItemSegmentDefine.Singleton;
		for (uint i = 0; i < count; i++)
		{
			CSItem item = new CSItem();
			item.m_guid.m_lowPart = stream.ReadInt32();
			item.m_guid.m_highPart = stream.ReadInt32();
			item.m_id = stream.ReadInt16();
			item.m_flag = stream.ReadInt16();
			int readDataLength = stream.ReadInt32();
            item.Init();
			for (int seg = 0; seg <= segmentDef.MaxSegmentType; seg++)
			{
				if ((item.m_flag & (0x1 << seg)) > 0)
				{
					readDataLength -= segmentDef.ReadSegment(item, seg, stream, readDataLength);
				}
			}
			stream.Seek(readDataLength, SeekOrigin.Current);
			//MyEquips.Singleton.myEquips.Add(item.m_guid, item);
			AddItem(item);
            item.Init();

           
		}
	}
	public void Serialize(PacketWriter stream)
	{
		stream.Write(default(byte));
		stream.Write(m_items.Count);
		CSItemSegmentDefine segmentDef = CSItemSegmentDefine.Singleton;
		foreach (var item in m_items)
		{
			stream.Write(item.m_guid.m_lowPart);
			stream.Write(item.m_guid.m_highPart);
			stream.Write(item.m_id);
			stream.Write(item.m_flag);
			stream.Write(0);
			for (int seg = 0; seg <= segmentDef.MaxSegmentType; seg++)
			{
				if ((item.m_flag & (0x1 << seg)) > 0)
				{
					segmentDef.WriteSegment(item, seg, stream);
				}
			}
		}
	}
	public void DeserializeFromText(string text)
	{
		m_items.Clear();
		MyEquips.Singleton.myEquips.Clear();

		StringStreamReader reader = new StringStreamReader(text);
		//byte version = reader.ReadByte();
		int count = reader.ReadInt();

		CSItemSegmentDefine segmentDef = CSItemSegmentDefine.Singleton;
		for (int index = 0; index < count; ++index)
		{
			CSItem item = new CSItem();
			item.m_guid.m_lowPart = reader.ReadInt();
			item.m_guid.m_highPart = reader.ReadInt();
			item.m_id = reader.ReadShort();
			item.m_flag = reader.ReadShort();

			for (int seg = 0; seg <= segmentDef.MaxSegmentType; seg++)
			{
				if ((item.m_flag & (0x1 << seg)) > 0)
				{
					segmentDef.ReadSegmentFromText(item, seg, reader.ReadString());
				}
			}
		}
	}
	public string SerializeToText()
	{
		StringStreamWriter writer = new StringStreamWriter();
		writer.Write(m_items.Count);
		CSItemSegmentDefine segmentDef = CSItemSegmentDefine.Singleton;
		foreach (var item in m_items)
		{
			writer.Write(item.m_guid.m_lowPart);
			writer.Write(item.m_guid.m_highPart);
			writer.Write(item.m_id);
			writer.Write(item.m_flag);
			for (int seg = 0; seg <= segmentDef.MaxSegmentType; seg++)
			{
				if ((item.m_flag & (0x1 << seg)) > 0)
				{
					writer.Write(segmentDef.WriteSegmentToText(item, seg));
				}
			}
		}
		return writer.ToString();
	}
	public void RemoveItem(int idx)
	{
		if (idx >= 0 && idx < m_items.Count)
		{
			m_items.RemoveAt(idx);
		}
	}
	void MarkRemove(int idx)
	{
		if (idx >= 0 && idx < m_items.Count)
		{
			m_items[idx].m_id = 0;
		}
	}
	void RemoveAllMarked()
	{
		int count = m_items.Count;
		for (int i = 0; i < count; )
		{
			CSItem item = m_items[i];
			if (item.m_id == 0)
			{
				m_items.RemoveAt(i);
				continue;
			}
			++i;
		}
	}
	public int AddItem(CSItem item)
	{
		int slot = m_items.Count;
		m_items.Add(item);
		return slot;
	}

    public void RemoveItem(CSItemGuid guid)
    {
        int count = m_items.Count;
        for (int i = 0; i < count; i++)
        {
            CSItem item = m_items[i];
            if (item.m_guid == guid)
            {
                m_items.Remove(item);
                return;
            }
        }

    }

    void UpdateItem(CSItem item)
    {
        if (null == item)
        {
            return;
        }

        int count   = m_items.Count;
        for (int i = 0; i < count; i++)
        {
            if (m_items[i].m_guid == item.Guid)
            {
                m_items[i].m_flag       = item.m_flag;
                m_items[i].m_id         = item.m_id;
                m_items[i].m_segment    = item.m_segment;
                return;
            }
        }
    }

    public void AddItemInBag(CSItem item)
    {
        CSItem temp = GetItemByGuid(item.Guid);
        if (null == temp)
        {
            AddItem(item);
        }
        else
        {
            UpdateItem(item);
        }
    }
	public int GetItemCount()
	{
		return m_items.Count;
	}
	public CSItem GetItem(int idx)
	{
		if (idx >= 0 && idx < m_items.Count)
		{
			return m_items[idx];
		}
		return null;
	}
	public CSItem GetItemByGuid(CSItemGuid guid)
	{
		int count = m_items.Count;
		for (int i = 0; i < count; i++)
		{
			CSItem item = m_items[i];
			if (item.m_guid == guid)
			{
				return item;
			}
		}
		return null;
	}
	public int GetItemSlotByGuid(CSItemGuid guid)
	{
		int count = m_items.Count;
		for (int i = 0; i < count; i++)
		{
			CSItem item = m_items[i];
			if (item.m_guid == guid)
			{
				return i;
			}
		}
		return -1;
	}

    // 获得 同一卡牌ID 的卡牌个数
    public int GetCardNumById(int id)
    {
        int cardNum = 0;
        int count = m_items.Count;
        for (int i = 0; i < count; i++)
        {
            CSItem item = m_items[i];
            if (item.IDInTable == id)
            {
                cardNum++;
            }
        }
        return cardNum;
    }
    //获得同ID的卡牌 用于段位升级公式一
    public CSItem GetCardBuyIdCardDivisionFormula(CSItem card) 
    {
        CSItem temp = null;
        foreach (CSItem item in m_items)
        {
            bool isInTeam = Team.Singleton.IsCardInTeam(item.m_guid);
            //不是最爱 不是代表 不是 在队伍中 并且卡牌ID相等 并且不是本身升段卡牌
            if (!card.Love && !isInTeam && User.Singleton.RepresentativeCard != item.m_guid && card.m_id == item.m_id
                && card.m_guid != item.m_guid)
            {
                if (null == temp)
                {
                    temp = item;
                }
                else 
                {
                    //找到等级最低并且升段次数最低的卡牌
                    if (item.BreakCounts < temp.BreakCounts)
                    {
                        temp = item;
                    }
                    else if (item.BreakCounts == temp.BreakCounts)
                    {
                        if (item.Level < temp.Level)
                        {
                            temp = item;
                        }
                    }
                }
            }
        }
        return temp;
    }
}
