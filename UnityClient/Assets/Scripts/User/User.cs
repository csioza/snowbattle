using UnityEngine;
using System.Collections;

public class User :IPropertyObject
{
	public enum ENPropertyChanged
	{
		enOnServerProps,
		enStaminaChanged,
		enRepresentativeCardChanged,
		enUserPropertyChanged,
	}

#region Singleton
	static User m_singleton;
	static public User Singleton
	{
		get
		{
			if (m_singleton == null)
			{
				m_singleton = new User();
				m_singleton.ItemBag = new CSItemBag();
				m_singleton.floorInsts = new PlayerFloorInsts();
				m_singleton.HelperList = new BattleHelperList();
				Localization.language = "简体中文";
			}
			return m_singleton;
		}
	}
#endregion

    public User()
    {
		SetPropertyObjectID((int)MVCPropertyID.enUserProps);

        UserProps = UserProperty.Singleton.CreatePropertySet("UserProperty");

        UserProps.SetProperty_String(UserProperty.name, "guo");
        UserProps.SetProperty_Int32(UserProperty.id, 123456789);
        UserProps.SetProperty_Int32(UserProperty.portrait, 1);
        UserProps.SetProperty_Int32(UserProperty.money, 1);
        UserProps.SetProperty_Int32(UserProperty.bind_money, 19000);
        UserProps.SetProperty_Int32(UserProperty.ring, 10);
        UserProps.SetProperty_Int32(UserProperty.friendship_point, 100);
        UserProps.SetProperty_Int32(UserProperty.level, 1);
        UserProps.SetProperty_Int32(UserProperty.exp, 0);
		UserProps.SetProperty_Int32(UserProperty.stamina, 12);
		UserProps.SetProperty_Int32(UserProperty.stamina_savetime, (int)TimeUtil.GetCurrentTimeStamp());		
		UserProps.SetProperty_Int32(UserProperty.friendCount_expandedSize, 0);
		UserProps.SetProperty_Int32(UserProperty.bagCapcity_expandedSize, 0);

		IsTeamLeaderModel = false;

    }
	public int Guid { get; set; }
	public PropertySet UserProps { get; set; }
	public CSItemBag ItemBag { get; set; }
	public bool IsTeamLeaderModel { get; set; }
	public BattleHelperList HelperList { get; set; }
	public PlayerFloorInsts floorInsts { get;set; }

    public int m_lastLevel { get; set;} //  升级前的 等级

	public CSItemGuid RepresentativeCard 
	{
		get
		{
			MyInt64 representativeCardID = UserProps.GetProperty_Int64(UserProperty.representative_card);
			int vHigh = representativeCardID.m_vHigh;
			int vLow = representativeCardID.m_vLow;
			
			CSItemGuid guid = new CSItemGuid();
			if (ItemBag.GetItemCount() == 0)
			{
				return guid;
			}
			guid.m_lowPart = vLow;
			guid.m_highPart = vHigh;
			if (ItemBag.GetItemByGuid(guid) == null)
			{
				return ItemBag.GetItem(0).Guid;
			}
			return guid;
		}
		set
		{
			UserProps.SetProperty_Int64(UserProperty.representative_card, value.m_lowPart, value.m_highPart);
			NotifyChanged((int)ENPropertyChanged.enRepresentativeCardChanged,null);
		}
	}

    public void OnInit()
    {

    }

	int StaminaRecoverTimeInterval
	{
		get
		{
			return GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enStaminaRecoverTime).IntTypeValue;
		}
	}

    int energyRecoverTimeInterval
	{
		get
		{
            return GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enEnergyRecoverTime).IntTypeValue;
		}
	}

    
	public void Update()
	{
		UpdateStamina();
	}

	void UpdateStamina()
	{
		//update
		//还有多少回复体力
		int stamina = GetStamina();
		int staminaMax = GetMaxStamina();
		if (stamina >= staminaMax)
		{
			return;
		}
		int stamina_savetime = User.Singleton.UserProps.GetProperty_Int32(UserProperty.stamina_savetime);
		int serverTime = (int)TimeUtil.GetServerTimeStampNow();
		int timeInterval = serverTime - stamina_savetime;
		if (timeInterval > StaminaRecoverTimeInterval)
		{
			User.Singleton.UserProps.SetProperty_Int32(UserProperty.stamina, stamina + 1);
			User.Singleton.UserProps.SetProperty_Int32(UserProperty.stamina_savetime, stamina_savetime + StaminaRecoverTimeInterval);
			NotifyChanged((int)ENPropertyChanged.enStaminaChanged, null);
		}

        //还有多少回复军资
        int energy      = GetEnergy();
        int energyMax   = GetMaxEnergy();
        if (energy >= energyMax)
        {
            return;
        }
        int energy_savetime     = User.Singleton.UserProps.GetProperty_Int32(UserProperty.energy_savetime);
        int intervalTime        = serverTime - energy_savetime;
        if (intervalTime > energyRecoverTimeInterval)
        {
            User.Singleton.UserProps.SetProperty_Int32(UserProperty.energy, energy + 1);
            User.Singleton.UserProps.SetProperty_Int32(UserProperty.energy_savetime, energy_savetime + energyRecoverTimeInterval);
            NotifyChanged((int)ENPropertyChanged.enStaminaChanged, null);
        }
	}

    public int GetUserID()
    {
        return User.Singleton.UserProps.GetProperty_Int32(UserProperty.id);
    }

	public int GetStaminaRecoverTime()
	{
		int stamina = GetStamina();
		int staminaMax = GetMaxStamina();
		if (stamina >= staminaMax)
		{
			return 0;
		}
		int serverTime = (int)TimeUtil.GetServerTimeStampNow();
		int stamina_savetime = User.Singleton.UserProps.GetProperty_Int32(UserProperty.stamina_savetime);
		return StaminaRecoverTimeInterval - (serverTime - stamina_savetime);
	}

    public int GetEnergyRecoverTime()
    {
        int energy     = GetEnergy();
		int energyMax = GetMaxEnergy();
		if (energy >= energyMax)
        {
            return 0;
        }
        int serverTime          = (int)TimeUtil.GetServerTimeStampNow();
		int energy_savetime = User.Singleton.UserProps.GetProperty_Int32(UserProperty.energy_savetime);
		return energyRecoverTimeInterval - (serverTime - energy_savetime);
    }

    public void AddDiamond( int diamond )
    {
        int bind_money = UserProps.GetProperty_Int32(UserProperty.bind_money);

        bind_money = bind_money + diamond;

        if (bind_money <= 0 )
        {
            bind_money = 0;
        }

        UserProps.SetProperty_Int32(UserProperty.bind_money, bind_money);

        Debug.Log("添加或减少的钻石为：" + diamond + ",调整后的身上钻石为：" + bind_money);
    }

    // 获得钻石
    public int GetDiamond()
    {
        return UserProps.GetProperty_Int32(UserProperty.bind_money);
    }

    // 获得金币
    public int GetMoney()
    {
        return UserProps.GetProperty_Int32(UserProperty.money);
    }

	//获取绑定钱的数量
	public int GetBindMoney()
	{
		return GetDiamond();
	}
	

	//获取友情点的数量
	public int GetFriendShipPoint()
	{
		return UserProps.GetProperty_Int32(UserProperty.friendship_point);
	}

    // 设置金币
    public void AddMoney(int money)
    {
        int num = UserProps.GetProperty_Int32(UserProperty.money);

        num     = num + money;

        if (num <= 0)
        {
            num = 0;
        }

        UserProps.SetProperty_Int32(UserProperty.money, num);

        Debug.Log("设置金币：" + money + ",调整后的身上金币为：" + num);
		NotifyChanged((int)ENPropertyChanged.enUserPropertyChanged, null);
    }


    // 获得当前经验
    public int GetExp()
    {
        return UserProps.GetProperty_Int32(UserProperty.exp);
    }

    // 获得当前需要升级的经验
    public int GetMaxExp()
    {
          PlayerAttrInfo playerAttrInfo = GameTable.playerAttrTableAsset.LookUp(GetLevel());
          if (null == playerAttrInfo)
          {
              return 0;
          }

          return playerAttrInfo.m_needExp; 
    }

    // 获得当前体力
    public int GetStamina()
    {
        return UserProps.GetProperty_Int32(UserProperty.stamina);
    }

    // 获得最大体力
    public int GetMaxStamina()
    {
        PlayerAttrInfo playerAttrInfo = GameTable.playerAttrTableAsset.LookUp(GetLevel());
        if (null == playerAttrInfo)
        {
            return 0;
        }

        return playerAttrInfo.m_stamina; 
    }

	//获取军资
	public int GetEnergy()
	{
		return UserProps.GetProperty_Int32(UserProperty.energy);
	}
	//获取最大军资
	public int GetMaxEnergy()
	{
		PlayerAttrInfo playerAttrInfo = GameTable.playerAttrTableAsset.LookUp(GetLevel());
		if (null == playerAttrInfo)
		{
			return 0;
		}

		return playerAttrInfo.m_energy; 
	}
    //获得卡牌添加图鉴
    public void GetCardChangeIllustrated( int cardId)
    {
        PropertyValueIllustratedArray illustrated = UserProps.GetProperty_Custom(UserProperty.illustrated_data) as PropertyValueIllustratedArray;
        PropertyValueIllustratedArray haveIllustrated = UserProps.GetProperty_Custom(UserProperty.haveIllustrated_data) as PropertyValueIllustratedArray;
        illustrated.SetBit(cardId, true);
        haveIllustrated.SetBit(cardId, true);
    }
    //看过卡牌添加图鉴
    public void SeeCardChangeIllustrated(int cardId) 
    {
        PropertyValueIllustratedArray haveIllustrated = UserProps.GetProperty_Custom(UserProperty.haveIllustrated_data) as PropertyValueIllustratedArray;
        haveIllustrated.SetBit(cardId,true);
    }

    // 设置经验
    public void AddExp(int exp)
    {
		int old_exp = GetExp();
		int allExp = exp + old_exp;
		int nowLevel = GetLevel();
		int LevelUpNeedExp = GameTable.playerAttrTableAsset.LookUp(nowLevel).m_needExp;
		if (0 == LevelUpNeedExp)
		{
			return;
		}
		while (allExp > LevelUpNeedExp)
		{
			allExp = allExp - LevelUpNeedExp;
			nowLevel = nowLevel + 1;
			LevelUpNeedExp = GameTable.playerAttrTableAsset.LookUp(nowLevel).m_needExp;
			if (0 == LevelUpNeedExp)
			{
				break;
			}
		}

		UserProps.SetProperty_Int32(UserProperty.level, nowLevel);
		UserProps.SetProperty_Int32(UserProperty.exp, allExp);

		NotifyChanged((int)ENPropertyChanged.enUserPropertyChanged, null);

		Debug.Log("获取经验：" + exp + ",新等级：" + nowLevel + ",新经验：" + allExp);
    }

	public void SetExp(int exp)
	{
		UserProps.SetProperty_Int32(UserProperty.exp, exp);
	}


    // 获得当前等级
    public int GetLevel()
    {
        return UserProps.GetProperty_Int32(UserProperty.level);
    }

    // 设置等级
    public void SetLevel(int level)
    {
        UserProps.SetProperty_Int32(UserProperty.level, level);

        Debug.Log("设置等级：" + level);
    }

    // 获得领导力
    public int GetLeadership()
    {
        PlayerAttrInfo info = GameTable.playerAttrTableAsset.LookUp(GetLevel());
        if ( null == info )
        {
            return 0;
        }
        return info.m_leaderShip;
    }

    // 获得队伍最大数量
    public int GetTeamMax()
    {
        PlayerAttrInfo info = GameTable.playerAttrTableAsset.LookUp(GetLevel());
        if (null == info)
        {
            return 0;
        }
        return info.m_teamNum;
    }

	public void SetLevelExp(int level, int exp)
	{
        m_lastLevel     = GetLevel();
		UserProps.SetProperty_Int32(UserProperty.level, level);
		UserProps.SetProperty_Int32(UserProperty.exp, exp);

        // 如果升级了
        if (level > m_lastLevel)
        {
            // 回复体力和军资到最大
            UserProps.SetProperty_Int32(UserProperty.stamina, GetMaxStamina());
            UserProps.SetProperty_Int32(UserProperty.energy, GetMaxEnergy());

        }
		NotifyChanged((int)ENPropertyChanged.enUserPropertyChanged, null);
	}

	public void SetMoney(int money)
	{
		UserProps.SetProperty_Int32(UserProperty.money, money);
		NotifyChanged((int)ENPropertyChanged.enUserPropertyChanged, null);
	}
	public void SetBindMoney(int bindMoney)
	{
		UserProps.SetProperty_Int32(UserProperty.bind_money, bindMoney);
		NotifyChanged((int)ENPropertyChanged.enUserPropertyChanged, null);
	}
	public void SetRing(int ring)
	{
		UserProps.SetProperty_Int32(UserProperty.ring, ring);
		NotifyChanged((int)ENPropertyChanged.enUserPropertyChanged, null);
	}
	public void SetFriendshpiPoint(int friendShipPoint)
	{
		UserProps.SetProperty_Int32(UserProperty.friendship_point, friendShipPoint);
		NotifyChanged((int)ENPropertyChanged.enUserPropertyChanged, null);
	}

	public void SetStamina(int stamina, int staminaSaveTime)
	{
		UserProps.SetProperty_Int32(UserProperty.stamina, stamina);
		UserProps.SetProperty_Int32(UserProperty.stamina_savetime, staminaSaveTime);
		NotifyChanged((int)ENPropertyChanged.enUserPropertyChanged, null);
	}

	public void SetEnergy(int energy, int energySaveTime)
	{
		UserProps.SetProperty_Int32(UserProperty.energy, energy);
		UserProps.SetProperty_Int32(UserProperty.energy_savetime, energySaveTime);
		NotifyChanged((int)ENPropertyChanged.enUserPropertyChanged, null);
	}

	public int GetNowSelectTeam()
	{
		return UserProps.GetProperty_Int32(UserProperty.nowSelect_team);
	}
	public void SetNowSelectTeam(int teamIndex)
	{
		UserProps.SetProperty_Int32(UserProperty.nowSelect_team,teamIndex);
		NotifyChanged((int)ENPropertyChanged.enUserPropertyChanged, null);
	}

	public int GetBagCapcity()
	{
        BagInfo info    = GameTable.BagTableAsset.LookUp((int)BAGTYPEENUM.enCardBagType);
        int capcity     = 50;
        if ( info != null )
        {
            capcity     = info.m_initalSize;
        }
		return capcity + UserProps.GetProperty_Int32(UserProperty.bagCapcity_expandedSize);
	}
	
	public void AddBagCapcity()
	{
		int nowBoughtTimes = UserProps.GetProperty_Int32(UserProperty.bagCapcity_expandedSize);
		UserProps.SetProperty_Int32(UserProperty.bagCapcity_expandedSize, nowBoughtTimes + 1);
		NotifyChanged((int)ENPropertyChanged.enUserPropertyChanged, null);
	}

	//好友的数量
	public int GetFriendCount()
	{
		int initSize = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enInitFriendCount).IntTypeValue;
		return initSize + UserProps.GetProperty_Int32(UserProperty.friendCount_expandedSize);
	}

    public void  UserLogin()
    {
        string username = SystemInfo.deviceUniqueIdentifier;
        if (Application.isEditor)
        {
            if (username.Length > 64)
            {
                username = username.Substring(0, 64);
            }
        }
        else
        {
            if (username.Length > 50)
            {
                username = username.Substring(0, 50);
            }
            username += Application.platform.ToString();
        }
        username += "c";
        string userpwd = "123456";
        MiniServer.Singleton.user_login(username, userpwd);
    }

	public void DeserializeTeamData()
	{
		//todo 队伍交给sy用
		PropertyValueAccoutTeamView teamValue = UserProps.GetProperty_Custom(UserProperty.team_data) as PropertyValueAccoutTeamView;
		//Debug.Log(teamValue.)

       // 初始化队伍数据
       for (int i = 0 ; i < teamValue.m_teamList.Length;i++)
       {
           bool isValid = false;
           for (int j = 0 ; j <= (int)Team.EDITTYPE.enSupport;j++)
           {
               if (!teamValue.m_teamList[i].m_memberList[j].Equals(CSItemGuid.Zero))
               {
                   isValid = true;
                   break;
               }
           }

           // 如果有有效的队伍数据 则添加进去
           if (isValid)
           {
               for (int j = 0; j <= (int)Team.EDITTYPE.enSupport; j++)
               {
                   Team.Singleton.AddTeamMember(teamValue.m_teamList[i].m_index, (Team.EDITTYPE)j, teamValue.m_teamList[i].m_memberList[j]);

                   //Debug.Log("m_index " + teamValue.m_teamList[i].m_index + ",j:" + j + ",guid:" + teamValue.m_teamList[i].m_memberList[j].m_highPart + "," + teamValue.m_teamList[i].m_memberList[j].m_lowPart);
               }
           } 
       }

     
       // 更新 当前队伍索引
       Team.Singleton.m_curTeamIndex = GetNowSelectTeam();
       Debug.Log("DeserializeTeamData 初始化队伍数据当前索引为：" + Team.Singleton.m_curTeamIndex);

     
      
	}

	public void OnServerProps()
	{
		DeserializeTeamData();
		NotifyChanged((int)ENPropertyChanged.enOnServerProps, null);

        m_lastLevel = GetLevel();
	}

    public void SetUserName(string name) 
    {
        UserProps.SetProperty_String(UserProperty.name, name);
        NotifyChanged((int)ENPropertyChanged.enUserPropertyChanged, null);
    }
	 public void RecordData()
    {
        Debug.Log("RecordData Guid:" + Guid);
        // 记录人物GUID
        PlayerPrefs.SetInt("User.Singleton.Guid", Guid);
        PlayerPrefs.Save();
    }


	public void OnPropUpdate(int propID)
	{
		
	}
}