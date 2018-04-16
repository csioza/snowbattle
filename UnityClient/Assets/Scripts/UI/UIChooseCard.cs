using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIChooseCard : UIWindow
{

    UIGrid m_grid           = null;
    GameObject m_cardItem   = null;

    GameObject m_choose     = null;
    GameObject m_enterName  = null;
    GameObject m_cardDetail = null;
    UIButton m_enterBtn     = null;

    UIInput m_input         = null;
    UILabel m_infoName      = null;
    UITexture m_star         = null;
    UITexture m_cardPic      = null;
    UILabel m_infoOp        = null;
    UILabel m_ocInfo        = null;
    UILabel m_raceInfo      = null;
    UILabel m_ocDetail      = null;
    GameObject m_skillItem  = null;
    GameObject m_skillParent = null;
    UISprite m_tips         = null;
    UILabel m_skillName     = null;
    UILabel m_skillDes      = null;
    UILabel m_skillCD       = null;



    Dictionary<int,GameObject> m_itemList = null;

    Dictionary<int,GameObject> m_skillList = new Dictionary<int,GameObject>();

    static public UIChooseCard GetInstance()
	{
        UIChooseCard self = UIManager.Singleton.GetUIWithoutLoad<UIChooseCard>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIChooseCard>("UI/UIChooseCard", UIManager.Anchor.Center);
		return self;
	}

	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enLogin, OnPropertyChanged);

        m_grid      = FindChildComponent<UIGrid>("Grid");
        m_cardItem  = FindChild("Card");

        m_input     = FindChildComponent<UIInput>("Input");
        m_choose    = FindChild("Choose");
        m_enterName = FindChild("EnterName");
        m_cardDetail= FindChild("CardDetail");

        m_enterBtn = FindChildComponent<UIButton>("EnterNameBtn");
        m_infoName = FindChildComponent<UILabel>("InfoName");
        m_star     = FindChildComponent<UITexture>("InfoStar");
        m_cardPic = FindChildComponent<UITexture>("DetialCardBG");
        m_infoOp   = FindChildComponent<UILabel>("InfoOp");
        m_ocInfo   = FindChildComponent<UILabel>("OcInfo");
        m_raceInfo = FindChildComponent<UILabel>("InfoRace");
        m_ocDetail = FindChildComponent<UILabel>("OcDetail");
        m_skillItem= FindChild("SkillItem");
        m_skillParent = FindChild("Skill");

        m_skillName     = FindChildComponent<UILabel>("SkillName");


        m_skillDes      = FindChildComponent<UILabel>("SkillDes");
        m_skillCD       = FindChildComponent<UILabel>("SkillCD");

        m_tips          = FindChildComponent<UISprite>("Tips");

        m_itemList    = new Dictionary<int, GameObject>();


        EventDelegate.Add(m_input.onChange, OnInputChange);

        m_input.value = SystemInfo.deviceName;
       
	}
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Login.ENPropertyChanged.enInitChooseCard)
        {

            InitCardList();
        }

        if (eventType == (int)Login.ENPropertyChanged.enChooseCardHide)
        {
            HideWindow();
        }

        if (eventType == (int)Login.ENPropertyChanged.enShowEnterName)
        {
            m_enterName.transform.Find("Input").GetComponent<UIInput>().value = "";

            m_enterBtn.isEnabled= false;

            m_enterName.SetActive(true);
            
            m_choose.SetActive(false);
            ShowWindow();
        }
        else if (eventType == (int)Login.ENPropertyChanged.enShowChooseCard)
        {
            m_enterBtn.isEnabled = false;
            // 隐藏输入名称
            m_enterName.SetActive(false);
            // 显示选择卡牌
            m_choose.SetActive(true);
            InitCardList();
            ShowWindow();
        }
    }

    public override void AttachEvent()
    {

        base.AttachEvent();
        AddChildMouseClickEvent("Button", OnChoose);

        AddChildMouseClickEvent("EnterNameBtn", OnEnterName);

        AddChildMouseClickEvent("Return", OnReturn);

        AddChildMouseClickEvent("Create", OnCreate);

        AddChildMouseClickEvent("ButtonLeft", OnLeft);
        AddChildMouseClickEvent("ButtonRight", OnRight);
    }

    private List<int> StringToList(string temp)
    {
        List<int> list = new List<int>();
        if (!string.IsNullOrEmpty(temp))
        {
            string[] param = temp.Split(new char[1] { ',' });
            foreach (string i in param)
            {
                if (!string.IsNullOrEmpty(i))
                    list.Add(int.Parse(i));
            }
        }
        return list;
    }

    public void OnInputChange()
    {
        m_enterBtn.isEnabled = (m_input.value.Length > 0);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

       foreach (KeyValuePair<int,GameObject> item in m_itemList)
       {
           GameObject.Destroy(item.Value);
       }

       foreach (KeyValuePair<int,GameObject>  item in m_skillList)
       {
           GameObject.Destroy(item.Value);
       }
    }

     // 选择
    public void OnChoose(GameObject obj)
    {
        Parma param = obj.GetComponent<Parma>();


        Debug.Log("OnChoose  选择卡牌角色 :" + param.m_id);

        m_choose.SetActive(false);

        UpdateCardInfo(param.m_id);
        
    }

    public void OnEnterName( GameObject obj )
    {
        if (m_input.value=="")
        {
            return;
        }
        UICommonMsgBoxCfg confg = obj.GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnEnterYes, null, confg);
        string text             = string.Format(Localization.Get("EnsureUseThisName"), m_input.value);
        Debug.Log("text:" + text + ",m_input.value:" + m_input.value);
        UICommonMsgBox.GetInstance().GetMainText().SetText(text);
    }

    // 返回
    public void OnReturn(GameObject obj)
    {
        m_choose.SetActive(true);
        m_cardDetail.SetActive(false);
    }

    // 创建角色
    public void OnCreate(GameObject obj)
    {
        UICommonMsgBoxCfg boxCfg = obj.transform.GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnCreateButtonYes, OnCreateButtonNo, boxCfg);
        string cardName = GameTable.HeroInfoTableAsset.Lookup(Login.Singleton.m_curCardId).StrName;
        string buyText = string.Format(Localization.Get("EnsureChooseThisCard"), cardName);
        UICommonMsgBox.GetInstance().GetMainText().SetText(buyText);
    }

    void OnCreateButtonYes(object sender, EventArgs e)
    {
        Loading.Singleton.SetLoadingTips();

        // 创建初始卡牌
        MiniServer.Singleton.InitCard_C2S();

        HideWindow();
    }
    void OnCreateButtonNo(object sender, EventArgs e)
    {

    }



    public void OnLeft(GameObject obj)
    {
        WorldParamInfo worldInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enCreatCardList);
        if (null == worldInfo)
        {
            return;
        }

        List<int> cardList = StringToList(worldInfo.StringTypeValue);
        if (null == cardList)
        {
            return;
        }

        int index   = cardList.IndexOf(Login.Singleton.m_curCardId);

        index       = index - 1;

        if (index<0)
        {
            index = cardList.Count - 1;
        }

        UpdateCardInfo(cardList[index]);
    }

    public void OnRight(GameObject obj)
    {
        WorldParamInfo worldInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enCreatCardList);
        if (null == worldInfo)
        {
            return;
        }

        List<int> cardList = StringToList(worldInfo.StringTypeValue);
        if (null == cardList)
        {
            return;
        }

        int index   = cardList.IndexOf(Login.Singleton.m_curCardId);

        index       = index + 1;

        if (index >= cardList.Count)
        {
            index = 0;
        }

        UpdateCardInfo(cardList[index]);
    }


    // 输入名称后的 二次确认
    public void OnEnterYes(object sender, EventArgs e)
    {
        // 隐藏输入名称
        m_enterName.SetActive(false);
        // 显示选择卡牌
        m_choose.SetActive(true);

        Login.Singleton.m_playrerName = m_input.value;

        // 创建角色
        MiniServer.Singleton.user_create_char(Login.Singleton.m_playrerName);
    }

    //  初始化 选择角色卡牌列表 
    public void InitCardList()
    {
        WorldParamInfo worldInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enCreatCardList);
        if (null == worldInfo )
        {
            return;
        }

        List<int> cardList = StringToList(worldInfo.StringTypeValue);
        if ( null == cardList )
        {
            Debug.Log("InitCardList null == cardList");
            return;
        }
        if ( cardList.Count == 0 )
        {
            Debug.Log("UIChooseCard InitCardList cardList为空 ！");
            return;
        }

        foreach (int cardId in cardList)
        {
            NewCard(cardId);
        }

        m_grid.Reposition();
        RefreshJoystickAutoMap();
    }

    public void NewCard(int cardId)
    {
        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(cardId);
        if (null == info)
        {
            Debug.Log("UIChooseCard NewCard HeroInfo 表数据没有 ID 为：" + cardId);
            return;
        }

        GameObject item = null;

        if (m_itemList.ContainsKey(cardId))
        {
            item = m_itemList[cardId];
        }
        else
        {
            item                                = GameObject.Instantiate(m_cardItem) as GameObject;

            item.gameObject.SetActive(true);

            // 设置 父物体
            item.transform.parent               = FindChildComponent<UIGrid>("Grid").transform;

            // 设置大小
            item.transform.localScale           = m_cardItem.transform.localScale;

            Parma param                         = item.transform.Find("Button").GetComponent<Parma>();

            param.m_id                          = cardId;

            // 设置按钮名称
            item.transform.Find("Button").name    = cardId.ToString();

            m_itemList.Add(cardId, item);

           
        }

        // 设置图片
        UITexture cardBg = item.transform.Find("CardBG").GetComponent<UITexture>();
        IconInfomation iconInfo   = GameTable.IconInfoTableAsset.Lookup(info.ImageId);
        if ( null != iconInfo )
        {
            cardBg.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
        }

        UILabel cardName    = item.transform.Find("CardName").GetComponent<UILabel>();
        cardName.text       = info.StrName;

        AddChildMouseClickEvent(cardId.ToString(), OnChoose); 
    }

    void UpdateCardInfo(int cardId)
    {
        Debug.Log("UpdateCardInfo:" + cardId);
        m_cardDetail.SetActive(true);

        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(cardId);
        if (null == heroInfo)
        {
            return;
        }

        Login.Singleton.m_curCardId     = cardId;

        IconInfomation iconInfo         = GameTable.IconInfoTableAsset.Lookup(heroInfo.ImageId);

        m_infoName.text                 = heroInfo.StrName;

        RarityRelativeInfo rarityInfo   = GameTable.RarityRelativeAsset.LookUp(heroInfo.Rarity);
        IconInfomation icon             = GameTable.IconInfoTableAsset.Lookup(rarityInfo.m_iconId);

        m_star.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);
        m_cardPic.mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);

        OccupationInfo occInfo  = GameTable.OccupationInfoAsset.LookUp(heroInfo.Occupation);
         
        RaceInfo raceInfo       = GameTable.RaceInfoTableAsset.LookUp(heroInfo.Type);

        m_infoOp.text           = occInfo.m_name;

        m_ocInfo.text           = occInfo.m_name;

        m_raceInfo.text         = raceInfo.m_name;

        m_ocDetail.text         = occInfo.m_describe;

        int i = 0;
        List<int> skillIDList = heroInfo.GetAllSkillIDList();
        foreach (int skillId in skillIDList)
        {
            SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(skillId);
            if (null == skillInfo || 0 != skillInfo.SkillType)
            {
                continue;
            }

            iconInfo = GameTable.IconInfoTableAsset.Lookup(skillInfo.Icon);

            if (iconInfo == null)
            {
                Debug.LogWarning("iconInfo 为空 skillId:" + skillId + ",skillInfo.Icon:" + skillInfo.Icon);
                continue;
            }
            if (m_skillList.ContainsKey(skillId))
            {
                GameObject obj                          = m_skillList[skillId];
                obj.SetActive(true);
                obj.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
                Parma parma                             = obj.GetComponent<Parma>();
                parma.m_id                              = skillId;
            }
            else
            {
                GameObject copy             = GameObject.Instantiate(m_skillItem.gameObject) as GameObject;
                copy.transform.parent       = m_skillParent.transform;
                copy.transform.localScale   = m_skillItem.transform.localScale;
                copy.gameObject.SetActive(true);
                copy.transform.LocalPositionX(0 + i * 90f);
                copy.transform.LocalPositionY(-45f);
                copy.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
                copy.name                   = copy.name + i;
                Parma parma                 = copy.GetComponent<Parma>();
                parma.m_id                  = skillId;

                EventDelegate.Add(copy.GetComponent<UIEventTrigger>().onPress, OnShowTips);

                AddChildMouseClickEvent(copy.name, OnHideTips);

                m_skillList.Add(skillId, copy);

            }
            i++;
        }            
                
    }

    // 隐藏TIPS
    void OnHideTips(GameObject obj)
    {
        m_tips.gameObject.SetActive(false);
    }
    
    // 显示Tips
    void OnShowTips()
    {
        Parma param = UIEventTrigger.current.gameObject.GetComponent<Parma>();
        if (null == param)
        {
            return;
        }

        // 更新
        UpdateTipsInfo(param.m_id);

        m_tips.gameObject.SetActive(true);
    }

    // 获得时间的 字符串
    public string GetTimeString(float time)
    {
        string str = "";
        float min = time / 60;
        float sec = time % 60;
        if (min >= 1)
        {
            str = string.Format(Localization.Get("RevertTime"), min, sec);
        }
        else
        {
            str = string.Format(Localization.Get("RevertTimeSec"), sec);
        }
        return str;
    }

    // 更新TIPS 信息
    void UpdateTipsInfo(int skillId)
    {
      
        SkillInfo info = GameTable.SkillTableAsset.Lookup(skillId);
        if (null == info)
        {
            return;
        }

        CDInfo cdInfo       = GameTable.CDTableAsset.Lookup(info.CoolDown);

        m_skillName.text    = info.Name;
        m_skillDes.text     = info.Description;
        m_skillCD.text      = GetTimeString(cdInfo.CDTime);


    }
}
