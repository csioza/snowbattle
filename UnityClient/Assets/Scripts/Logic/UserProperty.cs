using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserProperty 
{

	#region Singleton
	static UserProperty m_singleton;
	static public UserProperty Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new UserProperty();
			}
			return m_singleton;
		}
	}
    public UserProperty()
	{
		InitPropertyDefine();
        InitUserDefines();
	}
	#endregion

	PropertySetDefine m_define = new PropertySetDefine();
	Dictionary<string, PropertySetClass> m_namedClass = new Dictionary<string, PropertySetClass>();
	public PropertySetDefine Define
	{
		get { return m_define; }
	}

	public static int name;             // 名字
    public static int id;               // 账号ID
	public static int portrait;          // 头像
	public static int money;            // 金币
	public static int bind_money;       // 钻石 (充值获得的绑定钱)
	public static int ring;				// 戒指 (另外一种通用货币)
	public static int friendship_point;	// 友情点
	public static int level;            // 等级
    public static int exp;              // 经验
	public static int stamina;           // 体力
	public static int stamina_savetime;  // 上格体力恢复的时间	
	public static int energy;			//军资
	public static int energy_savetime;  // 上格军资恢复的时间
	public static int nowSelect_team;	//当前选择的队伍ID
	public static int team_data;        // 编队信息
	public static int friendCount_expandedSize;		//好友上限购买次数
	public static int bagCapcity_expandedSize;      // 背包容量
	public static int representative_card;		//代表卡
    public static int illustrated_data;     //图鉴信息
    public static int haveIllustrated_data; //拥有的卡牌图鉴信息
	public static int last_loginTime;	//上次登录的时间


	void InitPropertyDefine()
	{
		UserProperty.name			= m_define.AddProperty_String	("name", "empty", 16);	// 名字 
		UserProperty.portrait		= m_define.AddProperty_Int32	("potrait", 1);			// 头像
        UserProperty.money          = m_define.AddProperty_Int32    ("money", 0);			// 金币
        UserProperty.bind_money     = m_define.AddProperty_Int32    ("bind_money", 100);		// 钻石 (充值获得的绑定钱)
		UserProperty.ring			= m_define.AddProperty_Int32	("ring", 0);			// 戒指 (另外一种通用货币)
		UserProperty.friendship_point=m_define.AddProperty_Int32	("friendship_point", 0);// 友情点
		UserProperty.level			= m_define.AddProperty_Int32	("level", 1);			// 等级
        UserProperty.exp            = m_define.AddProperty_Int32    ("exp", 0);				// 经验
		UserProperty.stamina		= m_define.AddProperty_Int32	("stamina", 0);			// 体力
		UserProperty.stamina_savetime=m_define.AddProperty_Int32	("stamina_savetime", 0);// 上格体力恢复的时间
		UserProperty.energy			= m_define.AddProperty_Int32	("energy", 0);			// 军资
		UserProperty.energy_savetime= m_define.AddProperty_Int32	("energy_savetime", 0);	// 上格军资恢复的时间
		UserProperty.nowSelect_team = m_define.AddProperty_Int32	("nowSelect_team", 0);	// 当前选择的队伍
		UserProperty.team_data		= m_define.AddProperty_Buffer	("team_data", PropertyValueAccountTeamViewFactory.Singleton,10*4*8);// 编队信息(10个队，每队4个，每个双int)
		UserProperty.friendCount_expandedSize = m_define.AddProperty_Int32("friendCount_expandedSize", 0);	// 好友购买次数
		UserProperty.bagCapcity_expandedSize = m_define.AddProperty_Int32("bagCapcity_expandedSize", 0);	// 背包容量购买次数
		UserProperty.representative_card = m_define.AddProperty_Int64("representative_card", 0,0);	// 代表卡
        UserProperty.illustrated_data = m_define.AddProperty_BitArray("illustrated_data",PropertyValueIllustratedArrayFactory.Singleton, 1000);	// 图鉴信息
        UserProperty.haveIllustrated_data = m_define.AddProperty_BitArray("haveIllustrated_data",PropertyValueIllustratedArrayFactory.Singleton, 1000);	// 拥有的卡牌图鉴信息
		UserProperty.last_loginTime = m_define.AddProperty_Int32("last_loginTime", 0);			// 上次登录的时间
    }

	void InitUserDefines()
	{
		//int saveAndViewable = (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave;
		PropertySetClass theClass = new PropertySetClass(m_define);
		m_namedClass["UserProperty"] = theClass;
        theClass.AddProperty(UserProperty.name,             (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.portrait,			(int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(UserProperty.money,            (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.bind_money,		(int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.ring,				(int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.friendship_point, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.level,			(int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(UserProperty.exp,              (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.stamina,			(int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.stamina_savetime, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.energy,			(int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.energy_savetime, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.nowSelect_team, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.team_data,		(int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.friendCount_expandedSize, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.bagCapcity_expandedSize, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.representative_card, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(UserProperty.illustrated_data, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
        theClass.AddProperty(UserProperty.haveIllustrated_data, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);
		theClass.AddProperty(UserProperty.last_loginTime, (int)ENPropertyNotifyPipe.enNotifyPipeSerializeSave);		
	}


	public PropertySetClass LookupClass(string name)
	{
		if (m_namedClass.ContainsKey(name))
		{
			return m_namedClass[name];
		}
		return null;
	}
	public PropertySet CreatePropertySet(string name)
	{
		PropertySetClass theClass = LookupClass(name);
		if (theClass != null)
		{
			return theClass.CreateInstance();
		}
		return null;
	}
}
