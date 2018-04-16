using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UISetingPanel : UIWindow
{

   
//设置界面
    UISlider m_soundSlider = null;
    UISlider m_soundEffectSlider = null;
    UIPanel m_settingPanel = null;
    UISprite m_soundIcon = null;
    UISlider m_doubleClickSlider = null;

//图鉴界面
    UIPanel m_illustratedPanel = null;
    UIGrid m_girdPanel = null;
   
    UIDragScrollView m_item = null;
    // 格子列表 ID，格子UI
    Dictionary<int, UIDragScrollView> m_gridList = null;
    UIPanel m_sortCardPanel = null;


//筛选
    GameObject m_occToggles;            //职业按钮
    GameObject m_raceToggles;           //种族按钮
    GameObject m_selectedContion;

    GameObject m_OKButton;				//确认
    GameObject m_selectedPanel;			//上方的已选择
    GameObject m_cancelSelect;			//取消选择
    GameObject m_backgroundGrid;			//已选条件的背景格子
    GameObject m_bg;					//已选条件后面的长条背景图

    List<int> m_selectOccList = new List<int>();	//职业 
    List<int> m_selectRaceList = new List<int>();	//种族
    bool m_status;
//改名字界面
    UIPanel m_changeNamePanel = null;
    UIInput m_userName = null;
    UIButton m_okChangeButton = null;

    Texture2D m_unopen = null;


    static public UISetingPanel GetInstance()
    {
        UISetingPanel self = UIManager.Singleton.GetUIWithoutLoad<UISetingPanel>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UISetingPanel>("UI/UIGameSetingPanel", UIManager.Anchor.Center);
        return self;
    }
    public override void OnInit() 
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enGameSeting, OnPropertyChanged);
        
        m_gridList = new Dictionary<int, UIDragScrollView>();
        m_soundSlider = FindChildComponent<UISlider>("SoundSlider");
        m_soundEffectSlider = FindChildComponent<UISlider>("SoundEffectSlider");
        m_doubleClickSlider = FindChildComponent<UISlider>("DoubleClickSlider");
        m_settingPanel = FindChildComponent<UIPanel>("SettingPanel");
        m_illustratedPanel = FindChildComponent<UIPanel>("IllustratedPanel");
        m_soundIcon = FindChildComponent<UISprite>("SoundIcon");
        m_girdPanel = FindChildComponent<UIGrid>("ImageGridList");
        m_item = FindChildComponent<UIDragScrollView>("ImageGird");
        m_sortCardPanel = FindChildComponent<UIPanel>("SortCardPanel");
        m_occToggles = FindChildComponent<UITable>("Occupation").gameObject;
        m_raceToggles = FindChildComponent<UITable>("Race").gameObject;

        m_OKButton = FindChildComponent<UIButton>("OK").gameObject;

        m_selectedPanel = FindChild("SelectedPanel");
        m_selectedContion = FindChild("SelectedCondition", m_selectedPanel);
        m_cancelSelect = FindChild("cancel", m_selectedPanel);
        m_backgroundGrid = FindChild("Grid", m_selectedPanel);
        m_bg = FindChild("bg", m_selectedPanel);


        m_changeNamePanel = FindChildComponent<UIPanel>("ChangeNamePanel");
        m_userName = FindChildComponent<UIInput>("ChangeNameInput");
        m_okChangeButton = FindChildComponent<UIButton>("OKChangeNameButton");


        WorldParamInfo worldInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enUnlockHeadImgIconId);
        IconInfomation iconInfo = GameTable.IconInfoTableAsset.Lookup(worldInfo.IntTypeValue);

        m_unopen = PoolManager.Singleton.LoadIcon<Texture2D>(iconInfo.dirName);
        InitIllustratedList();
        UpdateIllustratedPanel();
        RefreshOccAndRace();
        RefreshSelectedCondition();
        InitPanelSetting();
    }

    public override void AttachEvent() 
    {
        base.AttachEvent();
        EventDelegate.Add(m_soundSlider.onChange, ChangeSound);
        EventDelegate.Add(m_soundEffectSlider.onChange, ChangeSoundEffect);
        EventDelegate.Add(m_doubleClickSlider.onChange, OnDoubleClickSlider);
        
        AddChildMouseClickEvent("CloseButton", OnCloseButton);
        AddChildMouseClickEvent("DoubleClickToggle", OnDoubleClickToggle);
        AddChildMouseClickEvent("CloseSettingButton", OnCloseSettingButton);
        AddChildMouseClickEvent("SettingButton", OnSettingButton);
        AddChildMouseClickEvent("DeveloperButton", OnDeveloperButton);
        AddChildMouseClickEvent("CloseIllustratedButton", OnCloseIllustratedButton);
        AddChildMouseClickEvent("ImageInfoButton", OnImageInfoButton);
        AddChildMouseClickEvent("IllustratedSortButton", OnIllustratedSortButton);
        AddChildMouseClickEvent("CardNumButton", OnCardNumButton);
        AddChildMouseClickEvent("CardOccupationButton", OnCardOccupationButton);
        AddChildMouseClickEvent("CardRarityButton", OnCardRarityButton);
        AddChildMouseClickEvent("CardTypeButton", OnCardTypeButton);
        for (int index = 0; index < m_occToggles.transform.childCount; index++)
        {
            AddMouseClickEvent(m_occToggles.transform.GetChild(index).gameObject, OnSelectOcc);
        }
        for (int index = 0; index < m_raceToggles.transform.childCount; index++)
        {
            AddMouseClickEvent(m_raceToggles.transform.GetChild(index).gameObject, OnSelectRace);
        }

        AddMouseClickEvent(m_OKButton, OnOKClicked);
        AddMouseClickEvent(m_cancelSelect, OnCancelClicked);
        AddChildMouseClickEvent("CloseChangeNameButton", OnCloseChangeNameButton);
        AddChildMouseClickEvent("OKChangeNameButton", OnOKChangeNameButton);
        AddChildMouseClickEvent("ChangeNameButton", OnChangeNameButton);
        AddChildMouseClickEvent("GMButton", OnGMButtonButton);
        EventDelegate.Add(m_userName.onChange, OnInputChange);

        AddChildMouseClickEvent("BindIDButton", OnLogOutGame);

      
    }

    void InitPanelSetting() 
    {
        //根据属性记录的是否双击 来更新面板显示
        m_doubleClickSlider.value = GameSetingProp.Singleton.m_doubleClickValve;
        //根据属性记录的声音大小来改变声音
        AudioListener.volume = GameSetingProp.Singleton.m_soundVolume;
        if (AudioListener.volume <= 0.0f)
        {
            m_soundSlider.value = 0.0f;
            m_soundIcon.spriteName = "volume_off";
        }
        else
        {
            m_soundSlider.value = 1.0f;
            m_soundIcon.spriteName = "volume";
        }
    }

    public override void OnShowWindow()
    {
        base.OnShowWindow();
        InitPanelSetting();
        if (MainUIManager.Singleton.ChangeUI(this))
        {
            MainButtonList.Singleton.m_curShowType = MainButtonList.SHOWWNDTYPE.ENSetting;
        }
    }

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj) 
    {
        if (eventType == (int)GameSetingProp.ENPropertyChanged.enCloseOtherPanel)
        {
            CloseUISettingChildPanel();
        }
        else if (eventType == (int)GameSetingProp.ENPropertyChanged.enChangeName)
        {
            OnChangeNameSucc();
        }
        else if (eventType == (int)GameSetingProp.ENPropertyChanged.enHide)
        {
            HideWindow();
        }
        
    }
    void CloseUISettingChildPanel() 
    {
        m_settingPanel.gameObject.SetActive(false);
        m_illustratedPanel.gameObject.SetActive(false);
        m_sortCardPanel.gameObject.SetActive(false);
        m_changeNamePanel.gameObject.SetActive(false);
        m_userName.value = "";
    }

    void ChangeSound() 
    {
        AudioListener.volume = m_soundSlider.value;
        GameSetingProp.Singleton.m_soundVolume = AudioListener.volume;
        if (m_soundSlider.value <= 0)
        {
            m_soundIcon.spriteName = "volume_off";
        }
        else 
        {
            m_soundIcon.spriteName = "volume";
        }
    }
    void ChangeSoundEffect() 
    {

    }
    public override void OnHideWindow() 
    {
        base.OnHideWindow();
        //MainButtonList.Singleton.Show();
        //MainMenu.Singleton.ShowMainMenu();
        //Zone.Singleton.ShowZone();
    }



    void OnCloseButton(GameObject obj) 
    {
//        MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enZone);
        //HideWindow();
    }

    void OnCloseSettingButton(GameObject obj) 
    {
        m_settingPanel.gameObject.SetActive(false);
    }

    void OnSettingButton(GameObject obj) 
    {
        m_settingPanel.gameObject.SetActive(true);
    }

    void OnImageInfoButton(GameObject obj) 
    {
        m_illustratedPanel.gameObject.SetActive(true);
    }

    void OnIllustratedSortButton(GameObject obj) 
    {
        if (m_sortCardPanel.gameObject.activeSelf)
        {
            m_sortCardPanel.gameObject.SetActive(false);
            SetVisable(false);
        }
        else 
        {
            m_sortCardPanel.gameObject.SetActive(true);
            SetVisable(true);
        }
    }

    void OnDoubleClickToggle(GameObject obj) 
    {
        BattleArena.Singleton.m_roomIsRolling = !BattleArena.Singleton.m_roomIsRolling;
    }

    void OnDeveloperButton(GameObject obj)
    {
        UIDeveloper.GetInstance().ShowWindow();
    }

    void OnCloseIllustratedButton(GameObject obj) 
    {
        m_illustratedPanel.gameObject.SetActive(false);
        m_sortCardPanel.gameObject.SetActive(false);
        //从新对List排序编号排序
        Illustrate.Singleton.m_cardList.Sort(new CardIdCompare());
    }

    //更新图鉴面板
    void UpdateIllustratedPanel() 
    {
        return;
        int index = 1;
        foreach (IllustratedInfo item in Illustrate.Singleton.m_cardList)
        {
            if (m_gridList.ContainsKey(index))
            {
                UIDragScrollView scrollView = m_gridList[index];
                scrollView.GetComponent<Parma>().m_id = item.m_id;
                scrollView.GetComponent<Parma>().m_type = item.m_gainLv;
                scrollView.transform.Find("ImageNum").GetComponent<UILabel>().text = ""+item.m_cardId;
                int iconId = GameTable.HeroInfoTableAsset.Lookup(item.m_id).headImageId;
                IconInfomation icon = GameTable.IconInfoTableAsset.Lookup(iconId);
                scrollView.transform.Find("ImageSprite").GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);
                scrollView.transform.parent = FindChildComponent<UIGrid>("ImageGridList").transform;
                if (0 == m_selectOccList.Count && 0 == m_selectRaceList.Count)
                {
                    scrollView.gameObject.SetActive(true);
                }
                else 
                {
                    if (m_selectOccList.IndexOf(item.m_occupation) == -1 && m_selectRaceList.IndexOf(item.m_type) == -1)
                    {
                        scrollView.gameObject.SetActive(false);
                    }
                    else 
                    {
                        scrollView.gameObject.SetActive(true);
                    }
                }
                
                switch (item.m_gainLv)
                {
                    case ((int)ENGainLevel.enNone):
                        scrollView.transform.Find("ImageSprite").Find("LockSprite").gameObject.SetActive(false);
                        scrollView.transform.Find("ImageSprite").GetComponent<UITexture>().mainTexture = m_unopen;
                        break;
                    case ((int)ENGainLevel.enSee):
                        scrollView.transform.Find("ImageSprite").Find("LockSprite").gameObject.SetActive(true);
                        break;
                    case ((int)ENGainLevel.enHave):
                        scrollView.transform.Find("ImageSprite").Find("LockSprite").gameObject.SetActive(false);
                        break;
                }
            }
            else
            {
                int iconId = GameTable.HeroInfoTableAsset.Lookup(item.m_id).headImageId;
                IconInfomation icon = GameTable.IconInfoTableAsset.Lookup(iconId);
                if (null != icon)
                {
                    UIDragScrollView copy = GameObject.Instantiate(m_item) as UIDragScrollView;
                    copy.GetComponent<Parma>().m_id = item.m_id;
                    copy.GetComponent<Parma>().m_type = item.m_gainLv;
                    copy.transform.Find("ImageNum").GetComponent<UILabel>().text = "" + item.m_cardId;
                    copy.transform.Find("ImageSprite").GetComponent<UITexture>().mainTexture = PoolManager.Singleton.LoadIcon<Texture>(icon.dirName);
                    copy.transform.parent = FindChildComponent<UIGrid>("ImageGridList").transform;
                    // 设置大小
                    copy.transform.localScale = m_item.transform.localScale;
                    if (0 == m_selectOccList.Count && 0 == m_selectRaceList.Count)
                    {
                        copy.gameObject.SetActive(true);
                    }
                    else
                    {
                        if (m_selectOccList.IndexOf(item.m_occupation) == -1 && m_selectRaceList.IndexOf(item.m_type) == -1)
                        {
                            copy.gameObject.SetActive(false);
                        }
                        else
                        {
                            copy.gameObject.SetActive(true);
                        }
                    }
                    m_gridList.Add(index, copy);
                    AddMouseClickEvent(copy.gameObject, OnClickCardImage);
                    switch (item.m_gainLv)
                    {
                        case ((int)ENGainLevel.enNone):
                            copy.transform.Find("ImageSprite").Find("LockSprite").gameObject.SetActive(false);
                            copy.transform.Find("ImageSprite").GetComponent<UITexture>().mainTexture = m_unopen;
                            break;
                        case ((int)ENGainLevel.enSee):
                            copy.transform.Find("ImageSprite").Find("LockSprite").gameObject.SetActive(false);
                            break;
                        case ((int)ENGainLevel.enHave):
                            copy.transform.Find("ImageSprite").Find("LockSprite").gameObject.SetActive(false);
                            break;
                    }
                }
            }
            index++;
        }
        m_girdPanel.Reposition();
    }
    void OnClickCardImage(object sender, EventArgs e) 
    {
        GameObject obj = (GameObject)sender;
        Parma param = obj.GetComponent<Parma>();
        if (param.m_type == (int)ENGainLevel.enNone)
        {
            return;
        }
        if (null == param)
        {
            return;
        }
        CSItem card = new CSItem();
        card.m_id = (short)param.m_id;
        card.Level = 1;
        card.isCheckSkill = false;
        CardBag.Singleton.OnShowCardDetail(card,true);
    }

    //初始化 图鉴列表
    void InitIllustratedList() 
    {
        PropertyValueIllustratedArray illustrated = User.Singleton.UserProps.GetProperty_Custom(UserProperty.illustrated_data) as PropertyValueIllustratedArray;
        PropertyValueIllustratedArray haveIllustrated = User.Singleton.UserProps.GetProperty_Custom(UserProperty.haveIllustrated_data) as PropertyValueIllustratedArray;

        foreach (KeyValuePair<int, HeroInfo> item in GameTable.HeroInfoTableAsset.m_list)
        {
            IllustratedInfo info = new IllustratedInfo();
            info.m_id = item.Key;
            info.m_cardId = item.Value.CardId;
            info.m_type = item.Value.Type;
            info.m_occupation = item.Value.Occupation;
            info.m_rarity = item.Value.Rarity;
            info.m_gainLv = illustrated.GetBit(item.Key) ? (haveIllustrated.GetBit(item.Key) ? (int)ENGainLevel.enHave : (int)ENGainLevel.enSee) : (int)ENGainLevel.enNone;
            Illustrate.Singleton.m_cardList.Add(info);
        }
    }


    //编号排序 从小到大
    public class CardIdCompare : IComparer<IllustratedInfo>
    {
        //按ID排序
        public int Compare(IllustratedInfo x, IllustratedInfo y)
        {
            return x.m_cardId.CompareTo(y.m_cardId);
        }
    }
    //稀有度排序
    public class CardRarityCompare : IComparer<IllustratedInfo>
    {
        //按稀有度排序
        public int Compare(IllustratedInfo x, IllustratedInfo y)
        {
            return (x.m_rarity == y.m_rarity) ? ((x.m_occupation == y.m_occupation) ?
                (x.m_cardId.CompareTo(y.m_cardId)) : (x.m_occupation.CompareTo(y.m_occupation)))
                : (x.m_rarity.CompareTo(y.m_rarity));
        }
    }
    //职业排序
    public class CardOccupationCompare : IComparer<IllustratedInfo>
    {
        //按职业排序
        public int Compare(IllustratedInfo x, IllustratedInfo y)
        {
            return (x.m_occupation == y.m_occupation) ? 
                (x.m_cardId.CompareTo(y.m_cardId)) : (x.m_occupation.CompareTo(y.m_occupation));
        }
    }

    //种族排序
    public class CardTypeCompare : IComparer<IllustratedInfo>
    {
        //按种族排序
        public int Compare(IllustratedInfo x, IllustratedInfo y)
        {
            return (x.m_type == y.m_type) ? ((x.m_occupation == y.m_occupation) ? 
                (x.m_cardId.CompareTo(y.m_cardId)) : (x.m_occupation.CompareTo(y.m_occupation))) 
                : (x.m_type.CompareTo(y.m_type));
        }
    }

    //编号排序按钮响应
    void OnCardNumButton(GameObject obj) 
    {
        //从新对List排序编号排序
        Illustrate.Singleton.m_cardList.Sort(new CardIdCompare());
        UpdateIllustratedPanel();
        m_sortCardPanel.gameObject.SetActive(false);
    }

    //稀有度排序按钮响应
    void OnCardRarityButton(GameObject obj)
    {
        Illustrate.Singleton.m_cardList.Sort(new CardRarityCompare());
        UpdateIllustratedPanel();
        m_sortCardPanel.gameObject.SetActive(false);
    }
    //职业排序按钮响应
    void OnCardOccupationButton(GameObject obj)
    {
        Illustrate.Singleton.m_cardList.Sort(new CardOccupationCompare());
        UpdateIllustratedPanel();
        m_sortCardPanel.gameObject.SetActive(false);
    }
    //种族排序按钮响应
    void OnCardTypeButton(GameObject obj)
    {
        Illustrate.Singleton.m_cardList.Sort(new CardTypeCompare());
        UpdateIllustratedPanel();
        m_sortCardPanel.gameObject.SetActive(false);
    }

    void RefreshOccAndRace()
    {
        //如果没选，全亮
        if (m_selectRaceList.Count + m_selectOccList.Count == 0)
        {
            for (int index = 0; index < m_occToggles.transform.childCount; index++)
            {
                GameObject obj = m_occToggles.transform.GetChild(index).gameObject;
                UIToggle toggle = obj.GetComponent<UIToggle>();
                toggle.value = true;
            }
            for (int index = 0; index < m_raceToggles.transform.childCount; index++)
            {
                GameObject obj = m_raceToggles.transform.GetChild(index).gameObject;
                UIToggle toggle = obj.GetComponent<UIToggle>();
                toggle.value = true;
            }

            return;
        }

        //先全部暗掉，
        for (int index = 0; index < m_occToggles.transform.childCount; index++)
        {
            GameObject obj = m_occToggles.transform.GetChild(index).gameObject;
            UIToggle toggle = obj.GetComponent<UIToggle>();
            toggle.value = false;
        }
        for (int index = 0; index < m_raceToggles.transform.childCount; index++)
        {
            GameObject obj = m_raceToggles.transform.GetChild(index).gameObject;
            UIToggle toggle = obj.GetComponent<UIToggle>();
            toggle.value = false;
        }

        //选择的亮起来
        for (int index = 0; index < m_selectOccList.Count; index++)
        {
            int occID = m_selectOccList[index];
            Transform trans = m_occToggles.transform.Find(occID.ToString());
            if (trans)
            {
                trans.gameObject.GetComponent<UIToggle>().value = true;
            }
        }

        for (int index = 0; index < m_selectRaceList.Count; index++)
        {
            int raceID = m_selectRaceList[index];
            Transform trans = m_raceToggles.transform.Find(raceID.ToString());
            if (trans)
            {
                trans.gameObject.GetComponent<UIToggle>().value = true;
            }
        }

    }

    void RefreshSelectedCondition()
    {
        for (int index = 0; index < m_selectedContion.transform.childCount; index++)
        {
            m_selectedContion.transform.GetChild(index).gameObject.SetActive(false);
        }

        if (m_selectRaceList.Count + m_selectOccList.Count == 0)
        {
            //m_cancelSelect.SetActive(false);
            return;
        }
        foreach (int occID in m_selectOccList)
        {
            GameObject obj = m_selectedContion.transform.Find("occ" + occID).gameObject;
            if (obj)
            {
                obj.SetActive(true);
            }
        }
        foreach (int raceID in m_selectRaceList)
        {
            GameObject obj = m_selectedContion.transform.Find("race" + raceID).gameObject;
            if (obj)
            {
                obj.SetActive(true);
            }
        }
        m_cancelSelect.SetActive(true);
        m_selectedContion.GetComponent<UIGrid>().Reposition();
    }
    public void OnSelectOcc(object obj, EventArgs e)
    {
        GameObject toggleObj = obj as GameObject;
//        UIToggle toggle = toggleObj.GetComponent<UIToggle>();
        int occID = 0;
        try
        {
            occID = Convert.ToInt32(toggleObj.name);
        }
        catch
        {

        }

        if (m_selectOccList.Contains(occID))
        {
            RemoveOccSelected(occID);
        }
        else if (m_selectRaceList.Count + m_selectOccList.Count < 5)
        {
            AddOccSelected(occID);
        }
        RefreshOccAndRace();
        RefreshSelectedCondition();
        UpdateIllustratedPanel();
    }
    
    public void OnSelectRace(object obj, EventArgs e)
    {
        GameObject toggleObj = obj as GameObject;
//        UIToggle toggle = toggleObj.GetComponent<UIToggle>();
        int raceID = 0;
        try
        {
            raceID = Convert.ToInt32(toggleObj.name);
        }
        catch
        {

        }
        if (m_selectRaceList.Contains(raceID))
        {
            RemoveRaceSelected(raceID);
        }
        else if (m_selectRaceList.Count + m_selectOccList.Count < 5)
        {
            AddRaceSelected(raceID);
        }
        RefreshOccAndRace();
        RefreshSelectedCondition();
        UpdateIllustratedPanel();
    }
    void OnOKClicked(object sender, EventArgs e)
    {
        SetVisable(false);
        m_sortCardPanel.gameObject.SetActive(false);
    }

    void OnCancelClicked(object sender, EventArgs e)
    {
        ReSet();
        for (int index = 0; index < m_raceToggles.transform.childCount; index++)
        {
            m_raceToggles.transform.GetChild(index).GetComponent<UIToggle>().value = false;
        }
        for (int index = 0; index < m_occToggles.transform.childCount; index++)
        {
            m_occToggles.transform.GetChild(index).GetComponent<UIToggle>().value = false;
        }
    }
    public void ReSet()
    {
        m_selectOccList.Clear();
        m_selectRaceList.Clear();
        RefreshOccAndRace();
        RefreshSelectedCondition();
        SetSelectedCondition(m_status);
    }

    // 添加职业筛选ID
    public void AddOccSelected(int occId)
    {
        if (m_selectOccList.Contains(occId))
        {
            return;
        }
        m_selectOccList.Add(occId);
    }

    // 移除职业筛选ID
    public void RemoveOccSelected(int occId)
    {
        if (m_selectOccList.Contains(occId))
        {
            m_selectOccList.Remove(occId);
        }
    }
    // 移除种族筛选ID
    public void RemoveRaceSelected(int raceId)
    {
        if (m_selectRaceList.Contains(raceId))
        {
            m_selectRaceList.Remove(raceId);
        }
    }
    // 添加种族筛选ID
    public void AddRaceSelected(int raceId)
    {
        if (m_selectRaceList.Contains(raceId))
        {
            return;
        }
        m_selectRaceList.Add(raceId);
    }
    public void SetSelectedCondition(bool visable)
    {
        m_cancelSelect.SetActive(visable);
        m_selectedContion.SetActive(true);
        if (visable)
        {
            ShowGridCount(5);
            m_bg.SetActive(true);
        }
        else
        {
            int allCount = m_selectOccList.Count + m_selectRaceList.Count;
            ShowGridCount(allCount);
            if (allCount == 0)
            {
                m_bg.SetActive(false);
            }
        }
    }
    void ShowGridCount(int count)
    {
        for (int index = 0; index < m_backgroundGrid.transform.childCount; index++)
        {
            if (index < count)
            {
                m_backgroundGrid.transform.GetChild(index).gameObject.SetActive(true);
            }
            else
            {
                m_backgroundGrid.transform.GetChild(index).gameObject.SetActive(false);
            }
        }
    }
    public void SetVisable(bool visable)
    {
        m_status = visable;
        SetSelectedCondition(visable);
    }


    //改名
    void OnCloseChangeNameButton(GameObject obj) 
    {
        m_changeNamePanel.gameObject.SetActive(false);
        m_userName.value = "";
    }

    void OnOKChangeNameButton(GameObject obj) 
    {
        if (0 >= m_userName.value.Length)
        {
            return;
        }
        MiniServer.Singleton.SendChangeName_C2S(m_userName.value);
    }
    void OnChangeNameButton(GameObject obj) 
    {
        m_changeNamePanel.gameObject.SetActive(true);
    }
    void OnDoubleClickSlider() 
    {
        GameSetingProp.Singleton.m_doubleClickValve = m_doubleClickSlider.value;
        if (m_doubleClickSlider.value <= 0.5f)
        {
            BattleArena.Singleton.m_roomIsRolling = false;
            m_doubleClickSlider.value = 0.0f;
        }
        else 
        {
            BattleArena.Singleton.m_roomIsRolling = true;
            m_doubleClickSlider.value =1.0f;
        }
    }

    void OnInputChange() 
    {
        if (0 >= m_userName.value.Length)
        {
            m_okChangeButton.isEnabled = false;
        }
        else 
        {
            m_okChangeButton.isEnabled = true;
        }
    }

    void OnChangeNameSucc() 
    {
        UICommonMsgBoxCfg boxCfg = m_okChangeButton.transform.GetComponent<UICommonMsgBoxCfg>();

        boxCfg.mainTextPrefab = boxCfg.textPrefabList[1];
        UICommonMsgBox.GetInstance().ShowMsgBox(OnOKChangeName, null, boxCfg);
        
        string buyText = Localization.Get("NameChanged");
        UICommonMsgBox.GetInstance().GetMainText().SetText(buyText);
    }
    void OnOKChangeName(object sender, EventArgs e) 
    {
        m_changeNamePanel.gameObject.SetActive(false);
        m_userName.value = "";
    }

    void OnGMButtonButton(GameObject obj)
    {
        UIGMPanel.GetInstance().ShowWindow();
    }


    void OnLogOutGame(GameObject obj)
    {
       PlayerPrefs.DeleteKey("XUserName");
       PlayerPrefs.DeleteKey("XPassword");
       ClientNet.Singleton.ShortConnect.CloseConnection();
       //MiniServer.Singleton.user_LogOut();
       Loading.Singleton.Hide();
       MainGame.Singleton.TranslateTo(new StateStartup());
    }
}




