using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICardDetail : UIWindow
{

    UILabel m_cost          = null;
    UILabel m_phyAttack     = null;
    UILabel m_magAttack     = null;
    UILabel m_hp            = null;
    UILabel m_occupation    = null;
    UILabel m_type          = null;
    UILabel m_curLevel      = null;
    GameObject m_skillParent        = null;
    GameObject m_passiveSkillParent = null;
    UILabel m_skillNoneLabel        = null;
    UICardDetailSkillItem[] m_skillList         = null;
    UICardDetailSkillItem[] m_passvieSkillList  = null;

    UIGrid m_grid               = null;
    UIButton m_levelUpBtn       = null;
    UIButton m_evolutionBtn     = null;
    UIButton m_breachBtn        = null;

    GameObject m_raceTexture    = null;
    GameObject m_occTexture     = null;

    GameObject m_model          = null;

    GameObject m_maxLevel       = null;
    GameResPackage.AsyncLoadObjectData m_modelData = new GameResPackage.AsyncLoadObjectData();
    GameObject m_tips             = null;

    UILabel m_tipsSkillName         = null;
    UILabel m_skillGetTips      = null;
    UILabel m_tipsSkillDes          = null;
    UILabel m_tipsSkillCD           = null;

    UILabel m_tipsSkillLevel        = null;

    //bool m_curFriendShowCardState = false;

    int m_currentCorroutineIndex = 0;

    public delegate void OnOkButtonCallbacked();

    OnOkButtonCallbacked m_okButtonCallBack;


    //int m_tipsDesHeightBase         = 32;
    //int m_tipsSwitchSkillBasePos    = 120;

    GameObject m_switchSkillTips    = null;
    GameObject m_comboItem         = null;

    List<GameObject> m_comboList = new List<GameObject>();

    GameObject m_tipsSkillType = null;

    UICardDetailCardPanel m_cardPanel = null;

    public delegate void OnClick();
    OnClick m_clickPre  = null;
    OnClick m_clickNext = null;

//    GameObject m_pre    = null;
//    GameObject m_next   = null;

    static public UICardDetail GetInstance()
    {
        UICardDetail self = UIManager.Singleton.GetUIWithoutLoad<UICardDetail>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UICardDetail>("UI/UICardDetails", UIManager.Anchor.Center);
        return self;
    }

	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enCardBag, OnPropertyChanged);
       
        m_cost      = FindChildComponent<UILabel>("Command_Number");
        
        m_phyAttack = FindChildComponent<UILabel>("PhyAttack_Number");
        m_magAttack = FindChildComponent<UILabel>("MagAttack_Number");
        m_hp        = FindChildComponent<UILabel>("HP_Number");
        m_occupation= FindChildComponent<UILabel>("Occupation_Lable");
        m_type      = FindChildComponent<UILabel>("Race_Lable");
        
        m_curLevel  = FindChildComponent<UILabel>("Level_Live");
        m_maxLevel  = FindChild("Level_Top");
        m_skillParent           = FindChild("Skill");
        m_passiveSkillParent    = FindChild("PassiveSkill");

        m_skillNoneLabel    = FindChildComponent<UILabel>("PassiveSkillNone");

        m_skillList         = new UICardDetailSkillItem[6];
        m_passvieSkillList  = new UICardDetailSkillItem[4];

        m_grid              = FindChildComponent<UIGrid>("OptionGrid");
        m_levelUpBtn        = FindChildComponent<UIButton>("ALeveUp");
        m_evolutionBtn      = FindChildComponent<UIButton>("BEvolution");
        m_breachBtn         = FindChildComponent<UIButton>("Breach");

        m_model             = FindChild("Model");

        m_tips              = FindChild("Tips");

        m_tipsSkillName         = FindChildComponent<UILabel>("SkillName");

        m_skillGetTips          = FindChildComponent<UILabel>("SkillGetTips");

        m_tipsSkillDes          = FindChildComponent<UILabel>("SkillDes");
        m_tipsSkillCD           = FindChildComponent<UILabel>("SkillCD");
        m_tipsSkillLevel        = FindChildComponent<UILabel>("SkillLevel");

        m_raceTexture           = FindChild("RaceTexture");
        m_occTexture            = FindChild("OccTexture");

     

       
        m_switchSkillTips       = FindChild("SwitchSkillTips");
        m_comboItem             = FindChild("Combo");

        m_tipsSkillType = FindChild("SkillType");

        m_cardPanel = UICardDetailCardPanel.Create();
        m_cardPanel.SetParent(WindowRoot);

//        m_next      = FindChild("Next");
//        m_pre       = FindChild("Pre");

    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)CardBag.ENPropertyChanged.enShowCardDetail)
        {
            ShowWindow();
            UpdateInfo();
        }

    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("DClose", OnClose);

        AddChildMouseClickEvent("ALeveUp", OnUpdateCard);

        AddChildMouseClickEvent("BEvolution", OnEvlotionCard);

        AddChildMouseClickEvent("Model", OnClickModel);

        AddChildMouseClickEvent("Breach", OnDivisionUpdateCard);

        AddChildMouseClickEvent("Pre", OnClickPre);
        AddChildMouseClickEvent("Next", OnClickNext);


    }

    public void RegisterCallBack(OnOkButtonCallbacked okButton)
    {
        m_okButtonCallBack = okButton;
    }
    public override void OnDestroy()
    {
        base.OnDestroy();

        m_cardPanel.Destroy();

        for (int i = 0; i < m_skillList.Length;  i++)
        {
            if (null != m_skillList[i])
            {
                m_skillList[i].Destroy();
            }
           
        }
 

        for (int i = 0; i < m_passvieSkillList.Length; i++)
        {
            if (null != m_passvieSkillList[i])
            {
                m_passvieSkillList[i].Destroy();
            }

        }

        for (int i = 0; i < m_comboList.Count; i ++ )
        {
            if (null != m_comboList[i])
            {
                GameObject.Destroy(m_comboList[i]);
            }
        }

        UIManager.Singleton.HideModel(m_modelData.m_obj as GameObject);
    }


    // 显示强化
    void OnUpdateCard(GameObject obj)
    {
        HideWindow();
        CardUpdateProp.Singleton.OnShowCardUpdate(CardBag.Singleton.m_curOptinGuid);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardUpdate);
    }

    // 显示进化
    void OnEvlotionCard(GameObject obj)
    {
        HideWindow();
       CardEvolution.Singleton.OnShowCardEvolution(CardBag.Singleton.m_curOptinGuid);
       //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardEvolution);
    }

    // 显示 突破
    void OnDivisionUpdateCard(GameObject obj)
    {
        HideWindow();
        CardDivisionUpdateProp.Singleton.SetCardCurt(CardBag.Singleton.m_curOptinGuid);
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enCardDivisionUpdate);
    }
   
    // 点击模型 播放动画
    void OnClickModel(GameObject obj)
    {
        obj = m_modelData.m_obj as GameObject;
        if (obj != null && obj.activeSelf)
        {//如果没有播放 胜利动画 则播放
            if (!obj.GetComponent<Animation>().IsPlaying("victory-00"))
            {
                obj.GetComponent<Animation>().Play("victory-00", PlayMode.StopSameLayer);
                AnimationClip clip = obj.GetComponent<Animation>().clip;
                if (null != clip)
                {
                    MainGame.Singleton.StartCoroutine(CoroutineAnimationEnd(++m_currentCorroutineIndex, obj, clip.length));
                }
            }
        }
    }

    // 胜利动画播放完成后 播放待机动画
    private IEnumerator CoroutineAnimationEnd(int animCoroutineIndex, GameObject obj,float timeLength)
    {
        yield return new WaitForSeconds(timeLength);
        if (null != obj && animCoroutineIndex == m_currentCorroutineIndex)
		{
            obj.GetComponent<Animation>().Play("standby-00", PlayMode.StopSameLayer);
        }
    }

    void OnClose( GameObject obj )
    {
        HideWindow();
        if (null != m_okButtonCallBack)
        {
            m_okButtonCallBack();
        }
        m_okButtonCallBack = null;
    }

    // 更新信息
    void UpdateInfo()
    {
        CSItem card = CardBag.Singleton.m_cardForDetail;

        if (null == card)
        {
            return;
        }

        // 如果卡牌ID为0则 是用预览卡牌（不是背包里的卡牌） 此时 加到最爱 进化 强化等按钮不显示
        if (CardBag.Singleton.m_curOptinGuid.Equals(CSItemGuid.Zero))
        {
            m_levelUpBtn.gameObject.SetActive(false);

            m_evolutionBtn.gameObject.SetActive(false);

            m_breachBtn.gameObject.SetActive(false);
        }
        else
        {
            m_levelUpBtn.gameObject.SetActive(card.IsStengthen());

            m_evolutionBtn.gameObject.SetActive(card.IsEvlotion());

            m_breachBtn.gameObject.SetActive(true);
        }
        m_grid.Reposition();
        AddModel(card, m_model);
        HeroInfo heroInfo               = GameTable.HeroInfoTableAsset.Lookup(card.IDInTable);

        if (null == heroInfo)
        {
            Debug.LogWarning("heroInfo == NULL heroInfo cardID:" + card.IDInTable);
            return;
        }

        m_cardPanel.Update(card.IDInTable);

        IconInfomation iconInfo         = GameTable.IconInfoTableAsset.Lookup(heroInfo.ImageId);
        OccupationInfo occupationInfo   = GameTable.OccupationInfoAsset.LookUp(heroInfo.Occupation);
        RaceInfo raceInfo               = GameTable.RaceInfoTableAsset.LookUp(heroInfo.Type);
//        LevelUpInfo levelupInfo         = GameTable.LevelUpTableAsset.LookUp(card.Level);
        m_cost.text         = "" + heroInfo.Cost;
        m_phyAttack.text    = "" + (int)card.GetPhyAttack();
        m_magAttack.text    = "" + (int)card.GetMagAttack();
        m_hp.text           = "" + (int)card.GetHp();
        m_occupation.text   = occupationInfo.m_name;
        m_type.text         = raceInfo.m_name;
        m_curLevel.text     = "" + card.Level ;
        m_maxLevel.GetComponent<UILabel>().text = card.GetMaxLevel().ToString();

        IconInfomation icon = GameTable.IconInfoTableAsset.Lookup(occupationInfo.m_iconId);
        m_occTexture.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);
        icon = GameTable.IconInfoTableAsset.Lookup(raceInfo.m_iconId);
        m_raceTexture.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);

        // 段位升级
        UpdateDanData(card.BreakCounts);

        foreach (UICardDetailSkillItem item in m_skillList)
        {
            if (null != item)
            {
                item.HideWindow();
            }
           
        }

        foreach (UICardDetailSkillItem item in m_passvieSkillList)
        {
            if (null != item)
            {
                item.HideWindow();
            }

        }
     
      int i = 0;
      List<int> skillIDList = heroInfo.GetAllSkillIDList();
      foreach (int skillId in skillIDList)
      {

        if (0 != heroInfo.NormalSkillIDList.Find(item => item == skillId))
        {
            continue;
        }
        SkillInfo skillInfo   = GameTable.SkillTableAsset.Lookup(skillId);
        if (null == skillInfo)
        {
            continue;
        }

        // 普通技能才被列入
        if (skillInfo.SkillType != 0)
        {
            continue;
        }
        iconInfo = GameTable.IconInfoTableAsset.Lookup(skillInfo.Icon);
                  
        if ( null == iconInfo )
        {
            continue;
        }
        if ( i < m_skillList.Length  )
        {
            UICardDetailSkillItem item = m_skillList[i];
            if (null == item)
            {
                item                          = UICardDetailSkillItem.Create();
                m_skillList[i]                = item;
                item.SetParent(m_skillParent.transform);
                item.SetPressCallbacked(OnShowTips);
                item.SetClickCallbacked(OnHideTips);
            }

            item.ShowWindow();
            item.Update(skillId);
            i++;
        } 
                  
      }

      m_skillParent.GetComponent<UIGrid>().Reposition();

      i = 0;
      bool bNone = true;
      foreach (int skillId in heroInfo.PassiveSkillIDList)
      {
          SkillInfo skillInfo = GameTable.SkillTableAsset.Lookup(skillId);
          if (null == skillInfo)
          {
              continue;
          }
          iconInfo      = GameTable.IconInfoTableAsset.Lookup(skillInfo.Icon);
          if (null == iconInfo)
          {
              continue;
          }
          if (i >= m_passvieSkillList.Length)
          {
              continue;
          }

          bNone         = false;

          UICardDetailSkillItem item = m_passvieSkillList[i];
          if (null == item)
          {
              item = UICardDetailSkillItem.Create();
              m_passvieSkillList[i] = item;
              item.SetParent(m_passiveSkillParent.transform);

              item.SetPressCallbacked(OnShowTips);
              item.SetClickCallbacked(OnHideTips);
          }

          item.ShowWindow();
          item.Update(skillId);
          i++;
              
      }

     m_passiveSkillParent.GetComponent<UIGrid>().Reposition();
     m_skillNoneLabel.gameObject.SetActive(bNone);

    }

    // 隐藏TIPS
    void OnHideTips(object sender, EventArgs e)
    {
        m_tips.SetActive(false);
    }

    // 显示Tips
    void OnShowTips()
    {
        Parma param =  UIEventTrigger.current.gameObject.GetComponent<Parma>();
        if (null == param)
        {
            return;
        }

        m_tips.gameObject.SetActive(true);

        // 更新
        UpdateTipsInfo(param.m_id);

       
    }

    // 更新TIPS 信息
    void UpdateTipsInfo( int skillId )
    {
        CSItem card         = null;
        if (CardBag.Singleton.m_curOptinGuid.Equals(CSItemGuid.Zero))
        {
            card = CardBag.Singleton.m_cardForDetail;
        }
        else
        {
            card = CardBag.Singleton.itemBag.GetItemByGuid(CardBag.Singleton.m_curOptinGuid);
        }

        if (null == card)
        {
            return;
        }

        
        HeroInfo heroInfo   = GameTable.HeroInfoTableAsset.Lookup(card.m_id);
        if ( null == heroInfo)
        {
            return;
        }

        SkillInfo info      = GameTable.SkillTableAsset.Lookup(skillId);
        if ( null == info )
        {
            return;
        }
        HeroCardSkill heroSkillInfo     =  card.GetSkillInfo(skillId);

        CDInfo cdInfo       = GameTable.CDTableAsset.Lookup(info.CoolDown);

        if ( cdInfo == null )
        {
            m_tipsSkillCD.text = GetTimeString(0f);
        }
        else
        {
            m_tipsSkillCD.text = GetTimeString(cdInfo.CDTime);
        }


//        Debug.Log("m_tipsSkillDes:" + m_tipsSkillDes.height);

        int needLevel       = 0;
        // tips 位置
        int offset          = 50;

        needLevel = heroInfo.GetSkillNeedLevel(skillId);
        // 主动技能类型
        if (info .SkillType== (int)ENSkillType.enSkill)
        {
            offset        = 100;
        }
        else
        {
            offset        = 200;
        }

        // 是否有此技能
        if (!card.HaveSkill(skillId))
        {
            m_skillGetTips.text = string.Format(Localization.Get("SkillUnlockLevel"), needLevel);
            m_skillGetTips.gameObject.SetActive(true);
        }
        else
        {
            m_skillGetTips.gameObject.SetActive(false);
        }

       
        
        // 设置内容
        m_tipsSkillName.text        = info.Name;
        m_tipsSkillDes.text         = info.Description;
        m_tipsSkillLevel.text       = heroSkillInfo.m_skillLevel.ToString();

       

        int height                  = 100;
        height                      = height + m_tipsSkillDes.height;

        int switchTipsHeight        = 100;

        switchTipsHeight            = switchTipsHeight + m_tipsSkillDes.height;


        m_comboItem.SetActive(false);

        for (int i = 0; i < m_comboList.Count; i++)
        {
            if (null != m_comboList[i])
            {
                GameObject.Destroy(m_comboList[i]);
            }
        }

        string strSkillType = "";

        // 终结技 
        if (info.SkillConnectionType == (int)ENSkillConnectionType.enFinal)
        {
            strSkillType = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enFinalSkill).StringTypeValue;

            m_comboItem.SetActive(true);
            int minCombo = info.MinComboRequir + (int)(info.ComboRequirementParam * heroSkillInfo.m_skillLevel);
            m_comboItem.GetComponent<UILabel>().text    = minCombo.ToString();
            WorldParamInfo worldInfo                    = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enActiveSkill);
            m_comboItem.transform.Find("ComboDes").GetComponent<UILabel>().text = Localization.Get(worldInfo.StringTypeValue);
            height = height + 60;
           

            Dictionary<int, string> list = GetComboList(skillId);
            int comboMax = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMaxCombo).IntTypeValue;

            if ( list!= null )
            {
                int i = 1;
                foreach (KeyValuePair<int,string> item in list )
                {
                    if (i>=comboMax)
                    {
                        break;
                    }
                    GameObject copy         = GameObject.Instantiate(m_comboItem) as GameObject;

                    copy.SetActive(true);
                    copy.transform.parent   = m_switchSkillTips.transform;
                    copy.name               = copy.name + i;
                    copy.transform.localScale = Vector3.one;

                    copy.transform.localPosition = Vector3.zero;

                    copy.GetComponent<UILabel>().text                                   = item.Key.ToString();
                    copy.transform.Find("ComboDes").GetComponent<UILabel>().text   = item.Value;
                    height                  = height + 60;
                    i++;

                    m_comboList.Add(copy);
                }
            }
        }
        else if (info.SkillConnectionType == (int)ENSkillConnectionType.enConnect)
        {
            strSkillType = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enConectSkill).StringTypeValue;
        }
        else if (info.SkillConnectionType == (int)ENSkillConnectionType.enSpecial)
        {
            strSkillType = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSpecialSkill).StringTypeValue;
        }


        m_tipsSkillType.GetComponent<UILabel>().text = Localization.Get(strSkillType);

        // 设置tips的 大小 主要是高度
        m_tips.GetComponent<UISprite>().height = height;

        // 设置位置
        m_tips.GetComponent<UISprite>().topAnchor.absolute = -offset;

        m_switchSkillTips.GetComponent<UILabel>().topAnchor.absolute    = -switchTipsHeight;

        m_switchSkillTips.GetComponent<UIGrid>().Reposition();

        

    }

    // 获得某以技能 所包含的 combo信息
    Dictionary<int, string> GetComboList(int skillId)
    {
        Dictionary<int, string> list = new Dictionary<int, string>();

        int startIndex  = (skillId - 1) * 10 + 1;
        int endIndex    = skillId * 10;

        for (int i = startIndex; i <= endIndex;i++ )
        {
            SkillResultInfo info = GameTable.SkillResultTableAsset.Lookup(i);

            if ( null == info )
            {
                continue;
            }

            foreach (var item in info.ExtraParamList)
            {
                if (item.ComboJudgeCount <= 0 )
                {
                    continue;
                }
                if (!list.ContainsKey(item.ComboJudgeCount))
                {
                    list.Add(item.ComboJudgeCount, item.ComboTips);
                }
            }
        }
        return list;
    }

    // 获得时间的 字符串
    public string GetTimeString(float time)
    {
        string str      = "";
        float min       = time / 60;
        float sec       = time % 60;
        if (min >= 1)
        {
            str = string.Format(Localization.Get("RevertTime"), min, sec);
        }
        else
        {
            str = string.Format(Localization.Get("RevertTime"), 0, sec);
        }
        return str;
    }
    // 添加显示模型
    void AddModel(CSItem card, GameObject parent)
    {
        if (card == null || parent == null)
        {
            return;
        }
        UIManager.Singleton.HideModel(m_modelData.m_obj as GameObject);
        m_modelData.m_isFinish = false;
        UIManager.Singleton.AddModel(card.IDInTable, parent, m_modelData);
    }

    // 更新段位界面
    void UpdateDanData(int dan)
    {
        for (int i = 1; i <= dan; i++)
        {
            UIButton danBtn     = FindChildComponent<UIButton>("Dan_" + i);
            danBtn.isEnabled    = true;
        }

        for (int i = dan+1; i <= 4; i++)
        {
            UIButton danBtn = FindChildComponent<UIButton>("Dan_" + i);
            danBtn.isEnabled = false;
        }
    }

    public void ShowPre(bool bShow)
    {

    }

    public void ShowNext(bool bShow)
    {

    }

    public void SetPreCallback(OnClick callback)
    {
        m_clickPre = callback;
    }
    
    public void SetNextCallback(OnClick callback)
    {
        m_clickNext = callback;
    }

    void OnClickPre(object sender, EventArgs e)
    {
        if (m_clickPre!= null)
        {
            m_clickPre();
        }
    }

    void OnClickNext(object sender, EventArgs e)
    {
        if (m_clickNext != null)
        {
            m_clickNext();
        }
    }
}
