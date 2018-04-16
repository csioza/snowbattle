using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICardSort : UIWindow
{
    public Dictionary<int, string> m_srotWordByType = new Dictionary<int, string>();

   
    GameObject m_sortButtons;           //排序按钮列表
    GameObject m_filter;                //筛选按钮列表
    GameObject m_occToggles;            //职业按钮
    GameObject m_raceToggles;           //种族按钮
	GameObject m_selectedContion;

    GameObject m_OKButton;				//确认
	GameObject m_selectedPanel;			//上方的已选择
	GameObject m_cancelSelect;			//取消选择
	GameObject m_backgroundGrid;			//已选条件的背景格子
	GameObject m_bg;					//已选条件后面的长条背景图

	//获得的排序结果列表
	List<CSItemGuid> m_sortedCardList = new List<CSItemGuid>();
	List<int> m_selectOccList = new List<int>();	//职业 
	List<int> m_selectRaceList = new List<int>();	//种族

	public ENSortType m_lastSortType;	//上次排序类型。 
	public bool m_lastDirReverse = true;
    UIWindow m_parentWnd;
    public ENSortClassType m_curSortClassType;

	delegate List<CSItemGuid> SortDelegate(ENSortType sortType, bool reverse);				//排序函数
	Dictionary<int, SortDelegate> m_sortDelegates = new Dictionary<int, SortDelegate>();

    public static UICardSort Load()
    {
        UICardSort sort = UIManager.Singleton.LoadUI<UICardSort>("UI/UICardSort", UIManager.Anchor.Center);
        return sort;
    }

    public override void OnInit()
    {
        Debug.Log("UISort.Init");
        base.OnInit();
        m_sortButtons = FindChild("SortButtons");
        m_filter = FindChild("Filter");

        m_occToggles = FindChild("Occupation", m_filter);
        m_raceToggles = FindChild("Race", m_filter);
		m_OKButton = FindChild("OK");

		m_selectedPanel = FindChild("SelectedPanel");
		m_selectedContion = FindChild("SelectedCondition", m_selectedPanel);
		m_cancelSelect = FindChild("cancel", m_selectedPanel);
		m_backgroundGrid = FindChild("Grid", m_selectedPanel);
		m_bg = FindChild("bg", m_selectedPanel);

		InitSortFuncs();
		RefreshOccAndRace();
		RefreshSelectedCondition();
        SetVisable(false);

        InitSetting();
    }

    public void SetParentWnd(UIWindow parentWnd)
    {
        if(parentWnd == null )
        {
            Debug.Log("SetParentWnd err, parentWnd == null");
            return;
        }
        m_parentWnd = parentWnd;
        SetParent(m_parentWnd.WindowRoot);
//         SetLocalPosition(Vector3.zero);
//         SetLocalScale(Vector3.one);
    }

    public override void AttachEvent()
    {
        for(int index = 0; index <m_sortButtons.transform.childCount; index ++)
        {
            AddMouseClickEvent(m_sortButtons.transform.GetChild(index).gameObject, OnSortButtonClicked);
        }

        for(int index = 0; index <m_occToggles.transform.childCount; index ++)
        {
            AddMouseClickEvent(m_occToggles.transform.GetChild(index).gameObject, OnSelectOcc);
        }
        for (int index = 0; index < m_raceToggles.transform.childCount; index++)
        {
            AddMouseClickEvent(m_raceToggles.transform.GetChild(index).gameObject, OnSelectRace);
        }

        AddMouseClickEvent(m_OKButton, OnOKClicked);
		AddMouseClickEvent(m_cancelSelect, OnCancelClicked);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    void InitSetting()
    {
        if (m_parentWnd == null )
        {
            Debug.Log("m_parentWnd == null");
            return;
        }
        UICardSortSettings setting = m_parentWnd.FindChildComponent<UICardSortSettings>("CardSort");
        if (setting == null)
        {
            Debug.Log("setting == null");
            return;
        }

        //m_sortButtons.transform.FindChild(((int)ENSortType.enDefault).ToString("00")).gameObject.SetActive(setting.enDefault);
        m_sortButtons.transform.Find(((int)ENSortType.enGotTime).ToString("00")).gameObject.SetActive(setting.enGotTime);
        m_sortButtons.transform.Find(((int)ENSortType.enByRarity).ToString("00")).gameObject.SetActive(setting.enByRarity);
        m_sortButtons.transform.Find(((int)ENSortType.enByPhyAttack).ToString("00")).gameObject.SetActive(setting.enByPhyAttack);
        m_sortButtons.transform.Find(((int)ENSortType.enByMagAttack).ToString("00")).gameObject.SetActive(setting.enByMagAttack);
        m_sortButtons.transform.Find(((int)ENSortType.enByHp).ToString("00")).gameObject.SetActive(setting.enByHp);
        m_sortButtons.transform.Find(((int)ENSortType.enByOccupation).ToString("00")).gameObject.SetActive(setting.enByOccupation);
        m_sortButtons.transform.Find(((int)ENSortType.enByType).ToString("00")).gameObject.SetActive(setting.enByType);
        m_sortButtons.transform.Find(((int)ENSortType.enByLevel).ToString("00")).gameObject.SetActive(setting.enByLevel);
        m_sortButtons.transform.Find(((int)ENSortType.enByFavorite).ToString("00")).gameObject.SetActive(setting.enByFavorite);
        m_sortButtons.transform.Find(((int)ENSortType.enCanEvolve).ToString("00")).gameObject.SetActive(setting.enCanEvolve);
        m_sortButtons.transform.Find(((int)ENSortType.enGold).ToString("00")).gameObject.SetActive(setting.enGold);
        m_sortButtons.transform.Find(((int)ENSortType.enRing).ToString("00")).gameObject.SetActive(setting.enRing);
    }

	void InitSortFuncs()
	{
		RegisterSortFunc(ENSortType.enGotTime,		CardBag.Singleton.SortCardByGotTime	);
		RegisterSortFunc(ENSortType.enByRarity,		CardBag.Singleton.SortByRarity		);
		RegisterSortFunc(ENSortType.enByPhyAttack,	CardBag.Singleton.SortByFlotProperty);
		RegisterSortFunc(ENSortType.enByMagAttack,	CardBag.Singleton.SortByFlotProperty);
		RegisterSortFunc(ENSortType.enByHp,			CardBag.Singleton.SortByFlotProperty);
		RegisterSortFunc(ENSortType.enByOccupation, CardBag.Singleton.SortCardByOccOrRace);
		RegisterSortFunc(ENSortType.enByType,		CardBag.Singleton.SortCardByOccOrRace);
		RegisterSortFunc(ENSortType.enByLevel,		CardBag.Singleton.SortByIntProperty	);
		RegisterSortFunc(ENSortType.enByFavorite,	CardBag.Singleton.SortByIntProperty	);
		RegisterSortFunc(ENSortType.enCanEvolve,	CardBag.Singleton.SortCardByEvolve	);
		RegisterSortFunc(ENSortType.enGold,			CardBag.Singleton.SortCardBySellPrice);
		RegisterSortFunc(ENSortType.enRing,			CardBag.Singleton.SortCardBySellPrice);
	}

	SortDelegate GetSortFunc(ENSortType type)
	{
		SortDelegate func = null;
		if (m_sortDelegates.TryGetValue((int) type,out func))
		{
			return func;
		}
		return null;
	}

	void RegisterSortFunc(ENSortType sortType, SortDelegate degelate)
	{
		m_sortDelegates.Add((int)sortType, degelate);
	}

    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
    }

    void OnOKClicked(object sender, EventArgs e)
    {
        Debug.Log("OnOkClicked" + m_sortButtons.activeSelf);
        SetVisable(false);
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

    public void OnSortButtonClicked(object sender, EventArgs e)
    {
        GameObject sortButton = sender as GameObject;
        string objName = sortButton.name;
        Debug.Log(objName + "OnClicked");

		ENSortType sortType = ENSortType.enDefault;
        try
        {
            sortType = (ENSortType)(Convert.ToInt32(objName)); 
        }
        catch
        {
			return;
        }
//		ENSortType newSortType;
		bool reverse = false;
		if (m_lastSortType == sortType)
		{
			reverse = !m_lastDirReverse;
		}
		else
		{
//			newSortType = sortType;
		}
        if (ENSortClassType.enFriend != m_curSortClassType) 
        {
            SortCards(sortType, reverse);
        }
        else
        {
            FriendList.Singleton.SortCards(sortType, reverse);

        }
        SetVisable(false);
    }

	public void SortCards(ENSortType sortType, bool reverse)
	{
		if ( sortType == ENSortType.enDefault)
		{
			sortType = ENSortType.enGotTime;
		}
		SortDelegate sortFunc = GetSortFunc((ENSortType)sortType);
		if (sortFunc != null)
		{
			m_lastSortType = sortType;
			m_lastDirReverse = reverse;
			m_sortedCardList = sortFunc(sortType, reverse);
			CardBag.Singleton.NotifySortResult();
		}

		//SetVisable(false);
	}
    public void SortCards(ENSortType sortType, bool reverse, ENSortClassType classType)
    {
        if (sortType == ENSortType.enDefault)
        {
            sortType = ENSortType.enGotTime;
        }
        SortDelegate sortFunc = GetSortFunc((ENSortType)sortType);
        if (sortFunc != null)
        {
            m_lastSortType = sortType;
            m_lastDirReverse = reverse;
            m_sortedCardList = sortFunc(sortType, reverse);
            CardBag.Singleton.NotifySortResult();
        }

        //SetVisable(false);
    }

    public void OnFilterButtonClicked(GameObject obj)
    {

    }

    public void OnSelectOcc(object obj,EventArgs e)
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

		if (m_selectOccList.Contains(occID) )
		{
			RemoveOccSelected(occID);
		}
		else if (m_selectRaceList.Count + m_selectOccList.Count < 5)
		{
			AddOccSelected(occID);
		}

        if (m_curSortClassType == ENSortClassType.enFriend)
        {
            FriendList.Singleton.SortCardUpdateInfo();
        }
        else
        {
            CardBag.Singleton.NotifySelect();
        }
		RefreshOccAndRace();
		RefreshSelectedCondition();
    }


    public void OnSelectRace(object obj, EventArgs e)
    {
        GameObject toggleObj = obj as GameObject;
//        UIToggle toggle = toggleObj.GetComponent<UIToggle>();
        int raceID= 0;
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
        if (m_curSortClassType == ENSortClassType.enFriend)
        {
            FriendList.Singleton.SortCardUpdateInfo();
        }
        else
        {
            CardBag.Singleton.NotifySelect();
        }
		RefreshOccAndRace();
		RefreshSelectedCondition();
    }

    // 添加职业筛选ID
    public void AddOccSelected(int occId)
    {
        if (m_selectOccList.Contains(occId))
        {
            return;
        }
		m_selectOccList.Add(occId);
        UnityEngine.Debug.Log("AddOccSelected:" + occId);
    }

    // 移除职业筛选ID
    public void RemoveOccSelected(int occId)
    {
		if (m_selectOccList.Contains(occId))
        {
			m_selectOccList.Remove(occId);
            UnityEngine.Debug.Log("RemoveOccSelected:" + occId);
        }
    }

    // 添加种族筛选ID
    public void AddRaceSelected(int raceId)
    {
        if (m_selectRaceList.Contains(raceId))
        {
            return;
        } 
        UnityEngine.Debug.Log("AddRaceSelected:" + raceId);
        m_selectRaceList.Add(raceId);
    }

    // 移除种族筛选ID
    public void RemoveRaceSelected(int raceId)
    {
        if (m_selectRaceList.Contains(raceId))
        {
            m_selectRaceList.Remove(raceId);
            UnityEngine.Debug.Log("RemoveRaceSelected:" + raceId);
        }
    }

	void RefreshOccAndRace()
	{
		//如果没选，全亮
		if (m_selectRaceList.Count + m_selectOccList.Count == 0)
		{
			for (int index = 0; index < m_occToggles.transform.childCount; index ++ )
			{
				GameObject obj = m_occToggles.transform.GetChild(index).gameObject;

                if (obj == null )
                {
                    continue;
                }

				UIToggle toggle = obj.GetComponent<UIToggle>();
				toggle.value = true;
			}
			for (int index = 0; index < m_raceToggles.transform.childCount; index++)
			{
				GameObject obj = m_raceToggles.transform.GetChild(index).gameObject;

                if (obj == null)
                {
                    continue;
                }
				UIToggle toggle = obj.GetComponent<UIToggle>();
				toggle.value = true;
			}

			return;
		}

		//先全部暗掉，
		for (int index = 0; index < m_occToggles.transform.childCount; index++)
		{
			GameObject obj = m_occToggles.transform.GetChild(index).gameObject;
            if (obj == null)
            {
                continue;
            }
			UIToggle toggle = obj.GetComponent<UIToggle>();
			toggle.value = false;
		}
		for (int index = 0; index < m_raceToggles.transform.childCount; index++)
		{
			GameObject obj = m_raceToggles.transform.GetChild(index).gameObject;
            if (obj == null)
            {
                continue;
            }
			UIToggle toggle = obj.GetComponent<UIToggle>();
			toggle.value = false;
		}

		//选择的亮起来
		for (int index = 0; index < m_selectOccList.Count; index ++ )
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


    bool m_status;
    
    public void ShowOrHide()
    {
        SetVisable(!m_status);
    }

    public void SetVisable(bool visable)
    {
        m_sortButtons.SetActive(visable);
        m_filter.SetActive(visable);
        m_status = visable;
		SetSelectedCondition(visable);
        if (!visable)
        {
            m_curSortClassType = ENSortClassType.enCSItem;
        }
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
		for (int index = 0; index < m_backgroundGrid.transform.childCount; index++ )
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

	public List<CSItemGuid> GetSortReslut()
	{
		return m_sortedCardList;
	}
	public List<int> GetSelectedOcc()
	{
		return m_selectOccList;
	}
	public List<int> GetSelectedRace()
	{
		return m_selectRaceList;
	}
	public ENSortType GetLastSortType()
	{
		return m_lastSortType;
	}

	public void ReSet()
	{
		SortCards(ENSortType.enDefault, false);
		m_selectOccList.Clear();
		m_selectRaceList.Clear();
		CardBag.Singleton.NotifySelect();
		RefreshOccAndRace();
		RefreshSelectedCondition();
		SetSelectedCondition(m_status);
	}
    public override void OnShowWindow()
    {
        base.OnShowWindow();
        ReSet();
    }
}
