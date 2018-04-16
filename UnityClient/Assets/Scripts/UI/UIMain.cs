using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIMain : UIWindow
{


    //属性相关
    UILabel m_expLabel;				//经验显示数字
    UIProgressBar m_expProcess;		//经验进度
    UILabel m_levelLabel;			//等级显示
    UILabel m_moneyLabel;			//金币
    GameObject m_portrait;			//头像


    static public UIMain GetInstance()
    {
        UIMain self = UIManager.Singleton.GetUIWithoutLoad<UIMain>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UIMain>("UI/UIMain", UIManager.Anchor.Center);
        }
        return self;
    }

    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enMainMenu, OnPropertyChanged);
        AddPropChangedNotify((int)MVCPropertyID.enUserProps, OnUserPropsChanged);
        AddPropChangedNotify((int)MVCPropertyID.enSceneManager, OnPropertySceneChanged);


        //-----属相相关----
        UICenterOnChild onCenterChild = FindChild("WrapContent").GetComponent<UICenterOnChild>();
        onCenterChild.onCenter = OnCenterCallback;
        GameObject headPanel    = FindChild("HeadPanel");
        //m_expLabel              = FindChildComponent<UILabel>("ExpLabel", headPanel);
        m_expProcess            = FindChildComponent<UIProgressBar>("ExpProcess", headPanel);
        m_levelLabel            = FindChildComponent<UILabel>("LabelLevel", headPanel);
        m_portrait              = FindChild("portrait", headPanel);
        GameObject goldPanel = FindChild("GoldPanel");
        m_moneyLabel = FindChildComponent<UILabel>("Label", goldPanel);

        RefreshShowProps();

        //添加的队列1 的测试数据
        MainMenu.Singleton.Test();
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)MainMenu.ENPropertyChanged.enShow)
        {
           MainButtonList.Singleton.Show();
           ShowWindow();    
        }
        else if (eventType == (int)MainMenu.ENPropertyChanged.enHideMain)
        {
            MainButtonList.Singleton.HideMainButtonList();
            HideWindow();
        }
    }

    void OnUserPropsChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)User.ENPropertyChanged.enOnServerProps)
        {
            RefreshShowProps();
        }
        else if (eventType == (int)User.ENPropertyChanged.enRepresentativeCardChanged)
        {
            SetPortraitGUID(User.Singleton.RepresentativeCard);
        }
        else if (eventType == (int)User.ENPropertyChanged.enStaminaChanged)
        {
            //SetStamina(User.Singleton.GetStamina());
        }
        else if (eventType == (int)User.ENPropertyChanged.enUserPropertyChanged)
        {
            RefreshShowProps();
        }

    }
    void OnPropertySceneChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        // 进入副本
        if (eventType == (int)SM.RandomRoomLevel.ENPropertyChanged.enEnterDungeon)
        {
            //隐藏LOADING
            Loading.Singleton.Hide();

            // 进入副本
            OnEnterBattle();
        }
    }
    // 服务器回调 进入游戏
    void OnEnterBattle()
    {
        MainGame.Singleton.CurrentState.SwitchNexted = true;
        StageMenu.Singleton.m_isReqHelpData = true;
    }
    public override void OnShowWindow()
    {
        base.OnShowWindow();
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        //AddChildMouseClickEvent("DemoButton", OnClickDemoButton);
    }

    //---------属相相关显示---------------
    #region playerAttr 
    void SetExp(int exp, int maxEpx, bool showPercent)
    {
        if (maxEpx == 0)
        {
            maxEpx = exp;
            showPercent = true;
        }
        float fVal = (float)exp / (float)maxEpx;
        if (showPercent)
        {
            int percentVal = (int)(fVal * 100);
            //m_expLabel.text = percentVal + "%";
        }
        else
        {
            //m_expLabel.text = exp + "/" + maxEpx;
        }
        m_expProcess.GetComponent<UISlider>().value = fVal;
    }

    bool showPercent = true;

    //更改经验显示 （百分比或者 exp/maxExp ）
    public void ChangeExpShowType(object obj, EventArgs e)
    {
        int exp     = User.Singleton.GetExp();
//        int level   = User.Singleton.GetLevel();
        int maxExp  = User.Singleton.GetMaxExp();
        SetExp(exp, maxExp, !showPercent);
        showPercent = !showPercent;
    }
    public GameObject mLastCenterObj = null;
    public void OnCenterCallback (GameObject centeredObject)
    {
        if (null != mLastCenterObj)
        {
            RemoveMouseClickEvent(mLastCenterObj);
            UIButtonScale btnScale = mLastCenterObj.GetComponent<UIButtonScale>();
            GameObject.DestroyImmediate(btnScale);
            mLastCenterObj.transform.localScale = Vector3.one;
            UIPanel uiPanel = mLastCenterObj.GetComponent<UIPanel>();
            uiPanel.depth = 10;
        }
        centeredObject.transform.localScale = centeredObject.transform.localScale * 1.2f;
        UIButtonScale btnScale1 = centeredObject.AddComponent<UIButtonScale>();
        btnScale1.hover = new Vector3(1.3f, 1.3f, 1.3f);
        UIPanel uiPanel1 = centeredObject.GetComponent<UIPanel>();
        uiPanel1.depth = 100001;
        mLastCenterObj = centeredObject;
        Transform zombieModel = centeredObject.transform.Find("Zombie");
        Transform demoModel = centeredObject.transform.Find("Demo");
        if (null != demoModel)
        {
            AddMouseClickEvent(centeredObject, OnDemoClicked);
        }
        Transform teamModel = centeredObject.transform.Find("Team");
    }
    public void OnDemoClicked(object obj, EventArgs e)
    {
        OnClickDemoButton(obj as GameObject);
    }
    public void OnClickDemoButton(GameObject obj)
    {
        int floorId = 20101;
        SM.RandomRoomLevel.Singleton.m_curFloorId = floorId;
        CSItem item = null;

        Helper helper = User.Singleton.HelperList.LookupHelper(StageMenu.Singleton.m_curHelperGuid);
        if (null != helper)
        {
            item = new CSItem();
            item.m_guid = helper.m_cardGuid;
            item.IDInTable = (short)helper.m_cardId;
            item.Level = helper.m_cardLevel;
            item.BreakCounts = helper.m_cardBreakCounts;

            item.Init();

            // 测试用
            HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(helper.m_cardId);
            if (null != heroInfo)
            {
                List<int> skillIDList = heroInfo.GetAllSkillIDList();
                foreach (int skillId in skillIDList)
                {
                    HeroCardSkill info = new HeroCardSkill();
                    info.m_skillID = skillId;

                    item.AddSkill(info);
                }
                foreach (var item1 in heroInfo.PassiveSkillIDList)
                {//被动技能
                    HeroCardSkill info = new HeroCardSkill();
                    info.m_skillID = item1;

                    item.AddSkill(info);
                }
            }
        }

        Team.Singleton.Comrade = item;
        BattleArena.Singleton.PrepareEnterDungeons();
//         MainGame.Singleton.CurrentState.SwitchNexted = true;
//         StageMenu.Singleton.m_isReqHelpData = true;
    }
    //点击设置按钮
    public void OnClickSettingButton(GameObject obj)
    {
        GameSetingProp.Singleton.OnCloseOtherChild();//关闭设置界面所有子界面
        if (MainButtonList.Singleton.m_curShowType == MainButtonList.SHOWWNDTYPE.ENSetting)
        {
            return;
        }
        //TipMessageBox.Singleton.OnShowTipMessageBox();
        //UISetingPanel.GetInstance().ShowWindow();
        MainButtonList.Singleton.m_curShowType = MainButtonList.SHOWWNDTYPE.ENSetting;
        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enSetingPanel);
    }

    void RefreshShowProps()
    {
        string name = User.Singleton.UserProps.GetProperty_String(UserProperty.name);
        int level = User.Singleton.UserProps.GetProperty_Int32(UserProperty.level);
        int exp = User.Singleton.UserProps.GetProperty_Int32(UserProperty.exp);
        int maxExp = GameTable.playerAttrTableAsset.LookUp(level).m_needExp;
        int money = User.Singleton.UserProps.GetProperty_Int32(UserProperty.money);
        CSItemGuid guid = User.Singleton.RepresentativeCard;
        SetPortraitGUID(guid);
        SetName(name);
        SetExp(exp, maxExp, showPercent);
        SetLevel(level);
        SetMoney(money);
    }


    //设置显示的props
    void SetPortraitGUID(CSItemGuid guid)
    {
        CSItem item = CardBag.Singleton.GetCardByGuid(guid);
        if (item == null)
        {
            return;
        }
        HeroInfo heroInfo = GameTable.HeroInfoTableAsset.Lookup(item.IDInTable);

        if (null == heroInfo)
        {
            Debug.LogWarning("heroInfo == NULL heroInfo cardID:" + item.IDInTable);
            return;
        }

        IconInfomation iconInfo = GameTable.IconInfoTableAsset.Lookup(heroInfo.headImageId);
        m_portrait.GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(iconInfo.dirName);
    }

    //设置名字
    void SetName(string name)
    {
        //m_nameLabel.text = name;
    }
    //设置名字显示
    void SetLevel(int level)
    {
        m_levelLabel.text = level.ToString();
    }
    //设置金币显示
    void SetMoney(int money)
    {
        m_moneyLabel.text = money.ToString();
    }
    //设置钻石显示
    void SetBindMoney(int bindMoney)
    {
        //m_bindMoneyLabel.text = bindMoney.ToString();
    }
    //设置体力显示
    //void SetStamina(int stamina)
    //{
    //    int level = User.Singleton.UserProps.GetProperty_Int32(UserProperty.level);
    //    int maxStamina = GameTable.playerAttrTableAsset.LookUp(level).m_stamina;
    //    if (stamina > maxStamina)
    //    {
    //        stamina = maxStamina;
    //    }
    //    m_staminaLabel.text = stamina + "/" + maxStamina;
    //}
    #endregion
}