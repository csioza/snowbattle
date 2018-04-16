using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 服务端返回的消息,都解析成 ServerMsgStruct
/// </summary>
public abstract class ServerMsgStruct
{
	public abstract void analyzeData(object jsonObj);

	protected string GetString(Dictionary<string, object> objDic, string key)
	{
		object obj;
		if (objDic.TryGetValue(key,out obj) && obj != null)
		{
			return obj.ToString();
		}
		return "";
	}

	protected int GetInt(Dictionary<string, object> objDic, string key)
	{
		string intStr = GetString(objDic, key);

		int outInt = -1;
		int.TryParse(intStr, out outInt);
		return outInt;
	}
}


public struct ZoneInfo
{
	public string zoneName;
	public string zoneUrl;
}
public class ServerMsgStructZone : ServerMsgStruct
{
	public List<ZoneInfo> zoneInfoList;
	public override void analyzeData(object jsonObj)
	{
		List<object> zoneListObj = jsonObj as List<object>;
		zoneInfoList = new List<ZoneInfo>();
		foreach (object zoneObj in zoneListObj)
		{
			Dictionary<string, object> oneZoneData = (Dictionary<string, object>)zoneObj;
			ZoneInfo info = new ZoneInfo();
			info.zoneName = GetString(oneZoneData, "name");
			info.zoneUrl = GetString(oneZoneData, "zone_url");
			zoneInfoList.Add(info);
		}
	}
}

//每一个服务端消息对应的结构
public class ServerMsgStructSession : ServerMsgStruct
{
	public string session;
	public int create_time;
	public int refresh_time;

	public override void analyzeData(object jsonObj)
	{
		session = jsonObj.ToString();
	}
}

public class ServerMsgStructAccountInfo : ServerMsgStruct
{
	public string accname;
	public string accpwd;
	public int caeate_time;
	public string token;

	public override void analyzeData(object jsonObj)
	{
		Dictionary<string, object> accountData = (Dictionary<string, object>)jsonObj;
		accname = GetString(accountData, "accname");
		accpwd = GetString(accountData, "accpwd").ToString();
		caeate_time = GetInt(accountData, "caeate_time");
		token = GetString(accountData, "token");
	}
}

public class ServerMsgStructUserInfo : ServerMsgStruct
{
    public int uid;
    public string name;
    public int portrait;
    public int level;
    public int exp;
    public int stamina;
    public int stamina_save_time;

    //动态计算属性
    public int leaderShip;
    public int stamina_max;

	public override void analyzeData(object jsonObj)
	{
		Dictionary<string, object> userInfoData = (Dictionary<string, object>)jsonObj;
		uid		=GetInt(userInfoData,"uid");
		name	=GetString(userInfoData,"name");
		portrait=GetInt(userInfoData,"portrait");
		level	=GetInt(userInfoData,"level");
		exp		=GetInt(userInfoData,"exp");
		stamina	=GetInt(userInfoData,"stamina");
		stamina_save_time =GetInt(userInfoData,"stamina_save_time");
		leaderShip =GetInt(userInfoData,"leaderShip");
		stamina_max =GetInt(userInfoData,"stamina_max");
	}
}

public class ServerMsgHerocardInfo : ServerMsgStruct
{
	public int slot_id;			//卡牌所在的格子
    public int hero_card_id;	//卡牌的实例ID 数据库用	
    public int id_in_table;		//静态表ID
    public int breakthrough_times;//已突破次数
    public int level;			//等级
    public int exp;				//经验
    public string equipments;	//装备列表.

	public override void analyzeData(object jsonObj)
	{
		Dictionary<string, object> oneCardInfoData = (Dictionary<string, object>)jsonObj;
		slot_id = GetInt(oneCardInfoData, "slot_id");
		id_in_table = GetInt(oneCardInfoData, "id_in_table");
		breakthrough_times = GetInt(oneCardInfoData, "breakthrough_times");
		level = GetInt(oneCardInfoData, "level");
		exp = GetInt(oneCardInfoData, "exp");
		equipments = GetString(oneCardInfoData, "equipments");

		hero_card_id = GetInt(oneCardInfoData, "hero_card_id");
	}
}

public class ServerMsgHerocardBag : ServerMsgStruct
{
	public int capacity;
	public Dictionary<int, ServerMsgHerocardInfo> herocardList = new Dictionary<int, ServerMsgHerocardInfo>();

	public override void analyzeData(object jsonObj)
	{
		Dictionary<string, object> msgData = (Dictionary<string, object>)jsonObj;
		capacity = GetInt(msgData, "capacity");
		Dictionary<string, object> cardsData = msgData["herocardList"] as Dictionary<string, object>;
		if (cardsData == null)
		{
			return ;
		}

		foreach (KeyValuePair<string, object> CardInfoData in cardsData)
		{
			ServerMsgHerocardInfo cardInfo = new ServerMsgHerocardInfo();
			cardInfo.analyzeData(CardInfoData.Value);
			herocardList.Add(cardInfo.slot_id, cardInfo);
		}
	}
}

//网关配置
public class ServermsgStructGlobalConfig : ServerMsgStruct
{
	public List<ZoneInfo> zoneInfoList;

	public override void analyzeData(object jsonObj)
	{
		Dictionary<string, object> configsData = (Dictionary<string, object>)jsonObj;
		object data;
		if (configsData.TryGetValue("zones", out data))
		{
			List<object> zonesData = (List<object>)data;
			zoneInfoList = new List<ZoneInfo>();
			foreach (object zoneObj in zonesData)
			{
				Dictionary<string, object> oneZoneData = (Dictionary<string, object>)zoneObj;
				ZoneInfo info = new ZoneInfo();
				info.zoneName = GetString(oneZoneData, "name");
				info.zoneUrl = GetString(oneZoneData, "zone_url");
				zoneInfoList.Add(info);
			}
		}
	}

}
//单服配置
public class ServermsgStructServerConfig : ServerMsgStruct
{
	public List<int> initHerocards;	//初始可选卡牌,静态表ID的列表  
	public override void analyzeData(object jsonObj)
	{
		Dictionary<string, object> configsData = (Dictionary<string, object>)jsonObj;
		object data;
		if (configsData.TryGetValue("init_herocards", out data))
		{
			List<object> cardsData = (List<object>)data;
			initHerocards = new List<int>();
			foreach (object cardObj in cardsData)
			{
				initHerocards.Add(int.Parse(cardObj.ToString()));
			}
		}		
	}
}

public class ServerMsgStructHeroCardLose : ServerMsgStruct
{
	public List<int> loseList;//失去卡牌的格子id
	public override void analyzeData(object jsonObj)
	{
		List<object> loseListObj = jsonObj as List<object>;
		loseList = new List<int>();
		foreach (object cardIDObj in loseListObj)
		{
			int cardID = 0;
			if (int.TryParse(cardIDObj.ToString(),out cardID))
			{
				loseList.Add(cardID);
			}
		}
	}
}
public class ServerMsgStructHeroCardGain : ServerMsgStruct
{
	public Dictionary<int,ServerMsgHerocardInfo> gainList;	
	public override void analyzeData(object jsonObj)
	{
		List<object> cardsDataList = (List<object>)jsonObj;
		gainList = new Dictionary<int, ServerMsgHerocardInfo>();
		foreach (object obj in cardsDataList)
		{
			ServerMsgHerocardInfo cardInfo = new ServerMsgHerocardInfo();
			cardInfo.analyzeData(obj);
			if (cardInfo.slot_id != 0)
			{
				gainList.Add(cardInfo.slot_id, cardInfo);
			}
		}
	}
}
public class ServerMsgStructHeroCardUpdate : ServerMsgStruct
{
	public Dictionary<int, ServerMsgHerocardInfo> updateList;
	public override void analyzeData(object jsonObj)
	{
		List<object> cardsDataList = (List<object>)jsonObj;
		updateList = new Dictionary<int, ServerMsgHerocardInfo>();
		foreach (object obj in cardsDataList)
		{
			ServerMsgHerocardInfo cardInfo = new ServerMsgHerocardInfo();
			cardInfo.analyzeData(obj);
			if (cardInfo.slot_id != 0)
			{
				updateList.Add(cardInfo.slot_id, cardInfo);
			}
		}

	}
}
