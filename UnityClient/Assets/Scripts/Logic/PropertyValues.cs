using UnityEngine;

//装备可视化属性
public class PropertyValueEquipViewFactory : PropertyValueFactory
{
    static PropertyValueEquipViewFactory m_singleton;
    static public PropertyValueEquipViewFactory Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new PropertyValueEquipViewFactory();
            }
            return m_singleton;
        }
    }
    public override PropertyCustomValue CreateValue()
    {
        return new PropertyValueEquipView();
    }
};

//装备可视化属性
public class PropertyValueEquipView : PropertyCustomValue
{
    int[] m_itemID=new int[4];
	public override void Read(BinaryHelper stream, int fixSize)
    {
        for (int i = 0; i < m_itemID.Length; i++)
        {
            m_itemID[i] = stream.ReadInt();
        }
    }
	public override byte[] Write()
	{
		BinaryHelper helper = new BinaryHelper();
		foreach (int item in m_itemID)
		{
			helper.Write(item);
		}
		return helper.GetBytes();
	}

	public override void ReadFromString(string value)
	{
		StringStreamReader reader = new StringStreamReader(value);
		int length = reader.ReadInt();
		for (int index = 0; index < length; ++index)
		{
			int itemID = reader.ReadInt();
			if (m_itemID.Length < index)
			{
				m_itemID[index] = itemID;
			}
		}
	}

	public override string WriteToString()
	{
		StringStreamWriter writer = new StringStreamWriter();
		writer.Write(m_itemID.Length);
		foreach (int itemID in m_itemID)
		{
			writer.Write(itemID);
		}
		return writer.ToString();
	}

};

//玩家图鉴数据
public class PropertyValueIllustratedArrayFactory : PropertyValueFactory
{
    static PropertyValueIllustratedArrayFactory m_singleton;
    static public PropertyValueIllustratedArrayFactory Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new PropertyValueIllustratedArrayFactory();
            }
            return m_singleton;
        }
    }
    public override PropertyCustomValue CreateValue()
    {
        return new PropertyValueIllustratedArray();
    }
};

public class PropertyValueIllustratedArray : PropertyCustomValue
{
    public byte[] array = new byte[1];

    public override void Read(BinaryHelper stream, int fixSize)
    {
        array = new byte[fixSize];
        stream.InnerStream.Read(array, 0, fixSize);
    }
    public override byte[] Write()
    {
        BinaryHelper helper = new BinaryHelper();
        helper.InnerStream.Write(array, 0, array.Length);
        return helper.GetBytes();
    }
    public override void ReadFromString(string value)
    {
        
    }

    public override string WriteToString()
    {
        return "";
    }

    public bool GetBit(int idx){
		int pos = idx/8;
		if (pos<array.Length)
		{
			return ((array[pos]&(0x1<<(idx%8))) != 0);
		}
		return false;
	}
	public void SetBit(int idx,bool v){
		int pos = idx/8;
		if (pos<array.Length)
		{
			int bitPos = idx%8;
			byte vChar = array[pos];
			if (v)
			{
                vChar |= (byte)(0x1 << bitPos);
			}
			else
			{
                vChar &= (byte)(~(0x1 << bitPos));
			}
            array[pos] = vChar;
		}
	}
};

//装备数据 added by wangxin
public class PropertyValueEquipArrayFactory : PropertyValueFactory
{
    static PropertyValueEquipArrayFactory m_singleton;
    static public PropertyValueEquipArrayFactory Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new PropertyValueEquipArrayFactory();
            }
            return m_singleton;
        }
    }
    public override PropertyCustomValue CreateValue()
    {
        return new PropertyValueEquipArray();
    }
};

//装备数据
public class PropertyValueEquipArray : PropertyCustomValue
{
	public CSItemGuid[] dress = new CSItemGuid[6];

	public override void Read(BinaryHelper stream, int fixSize)
    {
        for (int i = 0; i < dress.Length; i++)
        {
			//dress[i] = new CSItemGuid();
            dress[i].m_lowPart = stream.ReadInt();
			dress[i].m_highPart = stream.ReadInt();
        }
    }
	public override byte[] Write()
	{
		BinaryHelper helper = new BinaryHelper();
		for (int i = 0; i < dress.Length; i++)
		{
			helper.Write(dress[i].m_lowPart);
			helper.Write(dress[i].m_highPart);
		}
		return helper.GetBytes();
	}

	public override void ReadFromString(string value)
	{
		StringStreamReader reader = new StringStreamReader(value);
		int length = reader.ReadInt();
		for (int index = 0; index < length; ++index)
		{
			int lowPart = reader.ReadInt();
			int highPart = reader.ReadInt();
			if (dress.Length < index)
			{
				//dress[index] = new CSItemGuid();
				dress[index].m_lowPart = lowPart;
				dress[index].m_highPart = highPart;
			}
		}
	}

	public override string WriteToString()
	{
		StringStreamWriter writer = new StringStreamWriter();
		writer.Write(dress.Length);
		foreach (CSItemGuid guid in dress)
		{
			writer.Write(guid.m_lowPart);
			writer.Write(guid.m_highPart);
		}
		return writer.ToString();
	}
};

//玩家任务列表 
public class PropertyValueMissionListFactory : PropertyValueFactory
{
    static PropertyValueMissionListFactory m_singleton;
    static public PropertyValueMissionListFactory Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new PropertyValueMissionListFactory();
            }
            return m_singleton;
        }
    }
    public override PropertyCustomValue CreateValue()
    {
        return new PropertyValueMissionList();
    }
};

//玩家任务列表
public class PropertyValueMissionList : PropertyCustomValue
{
    public MissionInst[] myMissions = new MissionInst[30];
	public override void Read(BinaryHelper stream, int fixSize)
    {
        for (int i = 0; i < myMissions.Length; i++)
        {
            int id = stream.ReadInt();
            if (id != 0)
            {
                MissionInst inst = new MissionInst();
                inst.m_id = id;
                inst.m_isFinished = stream.ReadBool();
                inst.m_questRequire1 = stream.ReadShort();
                inst.m_questRequire2 = stream.ReadShort();
                inst.m_questRequire3 = stream.ReadShort();
                inst.m_questRequire4 = stream.ReadShort();
                myMissions[i] = inst;
            }
        }
    }
	public override byte[] Write()
	{
		BinaryHelper helper = new BinaryHelper();
		for (int i = 0; i < myMissions.Length; i++)
		{
			MissionInst inst = myMissions[i];
			helper.Write(inst.m_id);
			if (inst.m_id != 0)
			{
				helper.Write(inst.m_isFinished);
				helper.Write(inst.m_questRequire1);
				helper.Write(inst.m_questRequire2);
				helper.Write(inst.m_questRequire3);
				helper.Write(inst.m_questRequire4);
			}
		}
		return helper.GetBytes();
	}

	public override void ReadFromString(string value)
	{
		StringStreamReader reader = new StringStreamReader(value);
		int length = reader.ReadInt();
		for (int index = 0; index < length; ++index)
		{
			int id = reader.ReadInt();
			if (0 == id)
			{
				continue;
			}
			MissionInst inst = new MissionInst();
			inst.m_id = id;
			inst.m_isFinished = reader.ReadBool();
			inst.m_questRequire1 = reader.ReadShort();
			inst.m_questRequire2 = reader.ReadShort();
			inst.m_questRequire3 = reader.ReadShort();
			inst.m_questRequire4 = reader.ReadShort();

			if (myMissions.Length < index)
			{
				myMissions[index] = inst;
			}
		}
	}

	public override string WriteToString()
	{
		StringStreamWriter writer = new StringStreamWriter();
		writer.Write(myMissions.Length);
		foreach (MissionInst item in myMissions)
		{
			if (null == item)
			{
				writer.Write(0);
			}
			else
			{
				writer.Write(item.m_id);
				writer.Write(item.m_isFinished);
				writer.Write(item.m_questRequire1);
				writer.Write(item.m_questRequire2);
				writer.Write(item.m_questRequire3);
				writer.Write(item.m_questRequire4);
			}
		}
		return writer.ToString();
	}
};

//完成任务 
public class PropertyValueFinishedMissionListFactory : PropertyValueFactory
{
    static PropertyValueFinishedMissionListFactory m_singleton;
    static public PropertyValueFinishedMissionListFactory Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new PropertyValueFinishedMissionListFactory();
            }
            return m_singleton;
        }
    }
    public override PropertyCustomValue CreateValue()
    {
        return new PropertyValueFinishedMissionList();
    }
};

//完成任务
public class PropertyValueFinishedMissionList : PropertyCustomValue
{
    public short[] m_finishedList = new short[256];
	public override void Read(BinaryHelper stream, int fixSize)
    {
        for (int i = 0; i < m_finishedList.Length; i++)
        {
            m_finishedList[i] = stream.ReadShort();
        }
    }
	public override byte[] Write()
	{
		BinaryHelper helper = new BinaryHelper();
		for (int i = 0; i < m_finishedList.Length; i++)
		{
			helper.Write(m_finishedList[i]);
		}
		return helper.GetBytes();
	}

	public override void ReadFromString(string value)
	{
		StringStreamReader reader = new StringStreamReader(value);
		int length = reader.ReadInt();
		for (int index = 0; index < length; ++index)
		{
			short item = reader.ReadShort();
			if (m_finishedList.Length < index)
			{
				m_finishedList[index] = item;
			}
		}
	}

	public override string WriteToString()
	{
		StringStreamWriter writer = new StringStreamWriter();
		writer.Write(m_finishedList.Length);
		foreach (short value in m_finishedList)
		{
			writer.Write(value);
		}
		return writer.ToString();
	}
};


//完成副本 
public class PropertyValueFinishDungeonListFactory : PropertyValueFactory
{
    static PropertyValueFinishDungeonListFactory m_singleton;
    static public PropertyValueFinishDungeonListFactory Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new PropertyValueFinishDungeonListFactory();
            }
            return m_singleton;
        }
    }
    public override PropertyCustomValue CreateValue()
    {
        return new PropertyValueFinishDungeonList();
    }
};

//完成副本
public class PropertyValueFinishDungeonList : PropertyCustomValue
{
    public byte[] m_finishedList = new byte[50];
    public override void Read(BinaryHelper stream, int fixSize)
    {
        for (int i = 0; i < m_finishedList.Length; i++)
        {
            m_finishedList[i] = stream.ReadByte();
        }
    }
	public override byte[] Write()
	{
		BinaryHelper helper = new BinaryHelper();
		for (int i = 0; i < m_finishedList.Length; i++)
		{
			helper.Write(m_finishedList[i]);
		}
		return helper.GetBytes();
	}

	public override void ReadFromString(string value)
	{
		StringStreamReader reader = new StringStreamReader(value);
		int length = reader.ReadInt();
		for (int index = 0; index < length; ++index)
		{
			byte item = reader.ReadByte();
			if (m_finishedList.Length < index)
			{
				m_finishedList[index] = item;
			}
		}
	}

	public override string WriteToString()
	{
		StringStreamWriter writer = new StringStreamWriter();
		writer.Write(m_finishedList.Length);
		foreach (byte value in m_finishedList)
		{
			writer.Write(value);
		}
		return writer.ToString();
	}
};

//技能
public class PropertyValueSkillViewFactory : PropertyValueFactory
{
    static PropertyValueSkillViewFactory m_singleton;
    static public PropertyValueSkillViewFactory Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new PropertyValueSkillViewFactory();
            }
            return m_singleton;
        }
    }
    public override PropertyCustomValue CreateValue()
    {
        return new PropertyValueSkillView();
    }
};

//技能
public class PropertyValueSkillView : PropertyCustomValue
{
    public SkillInst[] m_item = new SkillInst[30];
    public override void Read(BinaryHelper stream, int fixSize)
    {
        for (int i = 0; i < m_item.Length; i++)
        {
            int id = stream.ReadInt();
            SkillInfo info = GameTable.SkillTableAsset.Lookup(id);
            if (null != info)
            {
                SkillInst inst = new SkillInst();
                inst.SkillInfo = info;
                inst.SkillLevel = stream.ReadInt();
                inst.SkillAddLevel = stream.ReadInt();
                m_item[i] = inst;
                MySkill.Singleton.m_skillInst[i] = inst;
            }
            else
            {
                Debug.Log("skill info is null! ID = " + id);
				return;
            }
        }
    }
	public override byte[] Write()
	{
		BinaryHelper helper = new BinaryHelper();
		foreach (var item in m_item)
		{
			if (null != item.SkillInfo)
			{
				helper.Write(item.SkillInfo.ID);
				helper.Write(item.SkillLevel);
				helper.Write(item.SkillAddLevel);
			}
			else
			{
				helper.Write(0);
			}
		}
		return helper.GetBytes();
	}

	public override void ReadFromString(string value)
	{
		StringStreamReader reader = new StringStreamReader(value);
		int length = reader.ReadInt();
		for (int index = 0; index < length; ++index)
		{
			int id = reader.ReadInt();
            SkillInfo info = GameTable.SkillTableAsset.Lookup(id);
			if (null == info)
			{
				continue;
			}
			SkillInst inst = new SkillInst();
			inst.SkillInfo = info;
			inst.SkillLevel = reader.ReadInt();
			inst.SkillAddLevel = reader.ReadInt();
			m_item[index] = inst;
			MySkill.Singleton.m_skillInst[index] = inst;
		}
	}

	public override string WriteToString()
	{
		StringStreamWriter writer = new StringStreamWriter();
		writer.Write(m_item.Length);
		foreach (var item in m_item)
		{
			if (null != item)
			{
				writer.Write(item.SkillInfo.ID);
				writer.Write(item.SkillLevel);
				writer.Write(item.SkillAddLevel);
			}
			else
			{
				writer.Write(0);
			}
		}
		return writer.ToString();
	}
};


// ACCOUT人物属性
public class PropertyValueAccountTeamViewFactory : PropertyValueFactory
{
    static PropertyValueAccountTeamViewFactory m_singleton;
    static public PropertyValueAccountTeamViewFactory Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new PropertyValueAccountTeamViewFactory();
            }
            return m_singleton;
        }
    }
    public override PropertyCustomValue CreateValue()
    {
        return new PropertyValueAccoutTeamView();
    }
};

// ACCOUT人物的编队属性
public class PropertyValueAccoutTeamView : PropertyCustomValue
{
   public TeamItem[] m_teamList = new TeamItem[10];
    public override void Read(BinaryHelper stream, int fixSize)
    {
        for (int i = 0; i < m_teamList.Length; i++)
        {
            if (null == m_teamList[i])
            {
                m_teamList[i]               = new TeamItem() ;
				m_teamList[i].m_memberList  = new CSItemGuid[4];                
            }

			m_teamList[i].m_index = i;
			for (int index = 0; index < 4; index++)
			{
				m_teamList[i].m_memberList[index].m_lowPart = stream.ReadInt();
				m_teamList[i].m_memberList[index].m_highPart = stream.ReadInt();
			}
        }
    }

    public override byte[] Write()
    {
        BinaryHelper helper = new BinaryHelper();
//        foreach (TeamItem item in m_teamList)
//        {
//             helper.Write(item.m_memberList[(int)Team.EDITTYPE.enMain]);
//             helper.Write(item.m_memberList[(int)Team.EDITTYPE.enDeputy]);
//             helper.Write(item.m_memberList[(int)Team.EDITTYPE.enSupport]);
//        }
        return helper.GetBytes();
    }

    public override void ReadFromString(string value)
    {
        StringStreamReader reader = new StringStreamReader(value);
        int length = reader.ReadInt();
    

        for (int index = 0; index < length; ++index)
        {
            int id = reader.ReadInt();
            if (0 == id)
            {
                continue;
            }

            TeamItem inst           = new TeamItem();
//             inst.m_memberList[(int)Team.EDITTYPE.enMain]    = reader.ReadInt();
//             inst.m_memberList[(int)Team.EDITTYPE.enDeputy]  = reader.ReadInt();
//             inst.m_memberList[(int)Team.EDITTYPE.enSupport] = reader.ReadInt();

            if (m_teamList.Length < index)
            {
                m_teamList[index] = inst;
            }
        }
    }

    public override string WriteToString()
    {
        StringStreamWriter writer = new StringStreamWriter();
        writer.Write(m_teamList.Length);
        foreach (TeamItem item in m_teamList)
        {
            if (null == item)
            {
                writer.Write(0);
            }
            else
            {
//                 writer.Write(item.m_memberList[(int)Team.EDITTYPE.enMain]);
//                 writer.Write(item.m_memberList[(int)Team.EDITTYPE.enDeputy]);
//                 writer.Write(item.m_memberList[(int)Team.EDITTYPE.enSupport]);
            }
        }

        return writer.ToString();
    }

};

// int类型的List
public class PropertyValueIntListViewFactory : PropertyValueFactory
{
    static PropertyValueIntListViewFactory m_singleton;
    static public PropertyValueIntListViewFactory Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new PropertyValueIntListViewFactory();
            }
            return m_singleton;
        }
    }
    public override PropertyCustomValue CreateValue()
    {
        return new PropertyValueIntListView();
    }
};

// int类型的List
public class PropertyValueIntListView : PropertyCustomValue
{
    public int[] m_list = new int[10];
    public override void Read(BinaryHelper stream, int fixSize)
    {
        for (int i = 0; i < m_list.Length; i++)
        {
            m_list[i] = stream.ReadInt();
        }
    }

    public override byte[] Write()
    {
        BinaryHelper helper = new BinaryHelper();
        foreach (var item in m_list)
        {
            helper.Write(item);
        }
        return helper.GetBytes();
    }

    public override void ReadFromString(string value)
    {
        StringStreamReader reader = new StringStreamReader(value);
        int length = reader.ReadInt();

        for (int index = 0; index < length; ++index)
        {
            int id = reader.ReadInt();
            if (m_list.Length < index)
            {
                m_list[index] = id;
            }
        }
    }

    public override string WriteToString()
    {
        StringStreamWriter writer = new StringStreamWriter();
        writer.Write(m_list.Length);
        foreach (var item in m_list)
        {
            writer.Write(item);
        }

        return writer.ToString();
    }
};



public class PropertyValueInt64Factory : PropertyValueFactory
{
	static PropertyValueInt64Factory m_singleton;
	static public PropertyValueInt64Factory Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new PropertyValueInt64Factory();
			}
			return m_singleton;
		}
	}
	public override PropertyCustomValue CreateValue()
	{
		return new PropertyValueInt64();
	}
};


public struct MyInt64
{
	public int m_vLow;
	public int m_vHigh;
}
//int64
public class PropertyValueInt64 : PropertyCustomValue
{
	public MyInt64 m_value;
	public override void Read(BinaryHelper stream, int fixSize)
	{
		m_value.m_vLow = stream.ReadInt();
		m_value.m_vHigh = stream.ReadInt();
	}
	public override byte[] Write()
	{
		BinaryHelper helper = new BinaryHelper();
		helper.Write(m_value.m_vLow);
		helper.Write(m_value.m_vHigh);
		return helper.GetBytes();
	}

	public void SetInt64(int vLow, int vHigh)
	{
		m_value.m_vLow = vLow;
		m_value.m_vHigh = vHigh;
	}

	public override void ReadFromString(string value)
	{

	}

	public override string WriteToString()
	{
		return "";
	}

};

