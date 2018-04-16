using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;




// 编队
public class MainUIManager : IPropertyObject
{
    public enum EDUITYPE
    {
        enMain = 0,
        enCardBag,
        enTipMessageBox,
        enCard,
        enCardDetail,
        enOperaterCardList,
        enExpandBag,
        enRUSure,
        enPopWnd,
        enShop,
        enGuild,
        enFriend,
        enFoundFriendByID,
        enFriendList,
        enApplyList,
        enSetingPanel,
        enDeveloper,
        enGMPanel,
        enCardDivisionUpdate,
    }
    public enum ENPropertyChanged
    {
        enShowWnd,
        enHideWnd,
    }
    public enum ENOnLeaveType
    {
        enNone,    
        enRoot,
        enAll,
    }
    //当前状态UI字符串名字的列表
    public List<string> mStateUIStrList = new List<string>();
    public string mLastUIStr = null;
    //当前跟节点UI窗口名字的列表
    public List<string> mWndList = new List<string>();
    //当前UI窗口名字字符串
    public string mCurUIName = null;
    //当前跟节点窗口的字符串
    public string mCurRootUIName = null;
    //当前状态字符串
    public string mCurStateStr = null;

    public List<string> mTemporarilyHideWndList = new List<string>();
    public MainUIManager()
    {
        //SetPropertyObjectID((int)MVCPropertyID.enTeam);


    }

    #region Singleton
    static MainUIManager m_singleton;
    static public MainUIManager Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new MainUIManager();
            }
            return m_singleton;
        }
    }
    #endregion
    //--------------------------StateUIList的加载----------------------------
    public void OnLoadStateUI(string stateStr)
    {
        List<UILoadInfo> tmpUIInfoList = GameTable.UILoadInfoTableAsset.Lookup_State(stateStr);
        mCurStateStr = stateStr;
        foreach (var item in tmpUIInfoList)
        {
            if (stateStr == "StateMainUI")
            {
                mStateUIStrList.Add(item.UINameStr);
            }
            if (!item.IsDynamicLoad)
            {
                UIWindow win = OnLoadUI(item.UINameStr);
                if (item.isOnceHide)
                {
                    win.HideWindow();
                }
                else
                {
                    win.ShowWindow();
                }
            }
        }
    }
    public UIWindow OnLoadUI(string uiStr)
    {
        Type uiNameObj = Type.GetType(uiStr);
        object instanceFunc = System.Activator.CreateInstance(uiNameObj);
        System.Reflection.MethodInfo instance = uiNameObj.GetMethod("GetInstance");
        UIWindow win = instance.Invoke(instanceFunc, null) as UIWindow;
        return win;
    }
    //
    public void OnExitDestroy(string stateStr)
    {
        List<UILoadInfo> tmpUIInfoList = GameTable.UILoadInfoTableAsset.Lookup_State(stateStr);
        foreach (var item in tmpUIInfoList)
        {
            if (!item.IsDynamicLoad)
            {
                Type uiNameObj = Type.GetType(item.UINameStr);
                if (uiNameObj == null)
                {
                    Debug.Log("The UIString is null:" + item.UINameStr);
                    continue;
                }
                System.Reflection.MethodInfo instance = uiNameObj.GetMethod("GetInstance");
                object instanceFunc = System.Activator.CreateInstance(uiNameObj);
                UIWindow win = instance.Invoke(instanceFunc, null) as UIWindow;
                if (item.IsExitDestroy)
                {
                    win.Destroy();
                }
                instanceFunc = null;
            }
        }
        ReSet();
    }
    public void ReSet()
    {
        mStateUIStrList.Clear();
        mWndList.Clear();
        mCurRootUIName = null;
        mCurUIName = null;
    }
    //UI的轮转
    public void OnSwitchSingelUI(EDUITYPE uitype)
    {
        UIWindow tmpCurUI = null;
        string tmpCurUIStr = mStateUIStrList[(int)uitype];
        tmpCurUI = GetCurUIWindow(tmpCurUIStr);
        if (tmpCurUI == null)
        {
            tmpCurUI = OnLoadUI(tmpCurUIStr);
            tmpCurUI.HideWindow();
        }
        if (ChangeUI(tmpCurUI))
        {
            tmpCurUI.ShowWindow();
        }
    }
    public UIWindow GetUIWindow(string uiStr)
    {
        return null;
    }

    public bool IsLoaded<T>() where T : UIWindow, new()
    {
        return UIManager.Singleton.GetUIWithoutLoad<T>() == null;
    }

    // 
    //     public void Notify(string name)
    //     {
    //         UIWindow ;
    //         MainUIManager.Singleton.ChangeUI(UIWindow)
    //             UIWindow.showwindow()
    //                 if (IsLoaded<UITeam>())
    //                 {
    //                     tmpCurUI = UITeam.GetInstance();
    //                     tmpCurUI.HideWindow();
    //                 }
    //                 else
    //                 {
    //                     tmpCurUI = UITeam.GetInstance();
    //                 }

    //     }

    //----------------------------------------UI界面的转换--------------------------------------
    public bool ChangeUI(UIWindow wnd)
    {
        //判断是否是连续按同一个界面
        if (mCurUIName == wnd.ToString())
        {
            return false;
        }
        UILoadInfo tmpWndInfo = GameTable.UILoadInfoTableAsset.Lookup_UI(mCurStateStr, wnd.ToString());
        if ((ENUIWindowLevel)tmpWndInfo.UIWndType == ENUIWindowLevel.enTrough)
        {
            return true;
        }

        if (((ENUIWindowLevel)tmpWndInfo.UIWndType == ENUIWindowLevel.enRoot && mCurRootUIName != wnd.ToString()) || wnd.m_castRootAble)
        {
            if (ChangeUIByRoot(wnd))
            {
                return true;
            }
        }
        else
        {
            if (ChangeUIByLevel(wnd))
            {
                return true;
            }
        }
        return false;
    }

    public bool HideCurWindow()
    {
        if (!HideCurWndList(true))
        {
            return false;
        }
        RemoveCurWndList();
        return true;
    }
    public bool ChangeUIByRoot(UIWindow wnd)
    {
        if (wnd.m_castRootAble)
        {

        }
        else if (mCurRootUIName == wnd.ToString() && mCurUIName == wnd.ToString())
        {
            return false;
        }
        //if ((mCurRootUIName == null || (GetCurUIWindow(mCurRootUIName) != null)))
        {
            if (!HideCurWndList(true))
            {
                return false;
            }
            RemoveCurWndList();

            mWndList.Add(wnd.ToString());
            mCurRootUIName = wnd.ToString();
            mCurUIName = wnd.ToString();

            return true;
        }

//        return false;
    }

    public bool ChangeUIByLevel(UIWindow wnd)
    {
        if (mCurUIName == wnd.ToString())
        {
            return false;
        }

        bool tmpContain = mWndList.Contains(wnd.ToString());
        UILoadInfo WndInfo = GameTable.UILoadInfoTableAsset.Lookup_UI(mCurStateStr, wnd.ToString());
        if (WndInfo.IsHideOther)
        {
            if (!HideCurWndList(false))
            {
                return false;
            }
        }
        else
        {
            for (int i = 0; i < mWndList.Count; i++)
            {
                UIWindow tmpWnd = GetCurUIWindow(mWndList[i]);
                if (tmpWnd == null)
                {
                    continue;
                }
                UILoadInfo tmpWndInfo = GameTable.UILoadInfoTableAsset.Lookup_UI(mCurStateStr, tmpWnd.ToString());
                if (tmpWndInfo.UIWndLvl >= WndInfo.UIWndLvl)
                {
                    HideUIWnd(mWndList[i], false);
                }
            }
        }
        if (tmpContain)
        {
//             int tmpCurWndIndex = mWndList.FindIndex(s => s == wnd.ToString());
//             for (int i = tmpCurWndIndex; i < mWndList.Count; i++)
//             {
//                 HideUIWnd(mWndList[i]);
//             }
//            mWndList.Add(wnd.ToString());
            mCurUIName = wnd.ToString();
            return true;
        }
        else
        {
            mWndList.Add(wnd.ToString());
            mCurUIName = wnd.ToString();
            return true;
        }
//        return false;
    }


    public bool HideUIWnd(string uiStr, bool rootSwitch = false)
    {
        UIWindow tmpWnd = GetCurUIWindow(uiStr);
        if (tmpWnd == null)
        {
            Debug.Log("The UIWindow is NULL" + uiStr);
            return false;
        }
        if (tmpWnd.IsVisiable())
        {
            bool hideSuccess = true;
            UILoadInfo tmpWndInfo = GameTable.UILoadInfoTableAsset.Lookup_UI(mCurStateStr, uiStr);
            switch ((ENOnLeaveType)tmpWndInfo.OnLeaveType)
            {
                case ENOnLeaveType.enNone:
                    tmpWnd.OnLeave();
                    break;
                case ENOnLeaveType.enAll:
                    if (!tmpWnd.OnLeave())
                    {
                        hideSuccess = false;
                    }
                    break;
                case ENOnLeaveType.enRoot:
                    if (rootSwitch)
                    {
                        if (!tmpWnd.OnLeave())
                        {
                            hideSuccess = false;
                        }
                    }
                    break;
            }
            if (hideSuccess)
            {
                tmpWnd.HideWindow();
            }
            else
            {
                tmpWnd.ShowWindow();
                mCurUIName = tmpWnd.ToString();
                return false;
            }
        }
        return true;
    }
    public bool HideCurWndList(bool rootSwitch)
    {
        for (int i = mWndList.Count - 1; i >= 0; i--)
        {
            UIWindow tmpWnd = GetCurUIWindow(mWndList[i]);
            if (tmpWnd == null)
            {
                Debug.Log("The HideCurWndList is fail:" + mWndList[i]);
                return false;
            }
            if (!HideUIWnd(mWndList[i], rootSwitch)) 
            {
                return false;
            }
        }
        return true;
    }

    public void RemoveCurWndList()
    {
        mWndList.Clear();
        mCurUIName = "";
        mCurRootUIName = "";
    }

    public UIWindow GetCurUIWindow(string uiStr)
    {
        return UIManager.Singleton.LookupNamedUI<UIWindow>(uiStr);

    }

    public void HideAllNeedHideWnd()
    {
       foreach (string item in mStateUIStrList)
       {
           UIWindow tmpWnd = GetCurUIWindow(item);
           if (tmpWnd.IsVisiable())
           {
               mTemporarilyHideWndList.Add(item);
               tmpWnd.HideWindow();
           }
       }
    }
    public void ShowAllNeedShowWnd()
    {
        foreach (string item in mTemporarilyHideWndList)
        {
            UIWindow tmpWnd = GetCurUIWindow(item);
            tmpWnd.ShowWindow();
        }
    }
}
//     public void OnLoadSingelUI(EDUITYPE uitype)
//     {
//         switch (uitype)
//         {
//             case EDUITYPE.enZone:
//                 Zone.Singleton.ShowZone();
//                 break;
//             case EDUITYPE.enCardBag:
//                 UICardBag tmpUIWnd = UICardBag.GetInstance();
//                 break;
//             case EDUITYPE.enTeam:
//                 break;
//             case EDUITYPE.enShop:
//                 break;
//             //             case EDUITYPE.enGuild:
//             //                 break;
//             //             case EDUITYPE.enSetting:
//             //                 break;
//             //             case EDUITYPE.enCommunity:
//             //                 break;
//             default:
//                 break;
//         }
//     }
//     public void OnShowSingelUI(EDUITYPE uitype)
//     {
//         switch (uitype)
//         {
//             case EDUITYPE.enZone:
//                 Zone.Singleton.ShowZone();
//                 break;
//             case EDUITYPE.enCardBag:
//                 break;
//             case EDUITYPE.enTeam:
//                 break;
//             case EDUITYPE.enShop:
//                 break;
//             //             case EDUITYPE.enGuild:
//             //                 break;
//             //             case EDUITYPE.enSetting:
//             //                 break;
//             //             case EDUITYPE.enCommunity:
//             //                 break;
//             default:
//                 break;
//         }
//     }
// 
// 
//     public void SetLastOnLoadSingelUI(string lastUIStr)
//     {
//         mLastUIStr = lastUIStr;
//     }




//         switch (uiStr)
//         {
//             case "UICardEvolution":
//                 
//                 return UIManager.Singleton.GetUIWithoutLoad<UICardEvolution>();
//             case "UIGetCard":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIGetCard>();
//             case "UIMain":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIMain>();
//             case "UIZone":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIZone>();
//             case "UIMainButtonList":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIMainButtonList>();
//             case "UITeam":
//                 return UIManager.Singleton.GetUIWithoutLoad<UITeam>();
//             case "UIStageMenu":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIStageMenu>();
//             case "UICardBag":
//                 return UIManager.Singleton.GetUIWithoutLoad<UICardBag>();
//             case "UITipMessageBox":
//                 return UIManager.Singleton.GetUIWithoutLoad<UITipMessageBox>();
//             case "UICard":
//                 return UIManager.Singleton.GetUIWithoutLoad<UICard>();
//             case "UICardDetail":
//                 return UIManager.Singleton.GetUIWithoutLoad<UICardDetail>();
//             case "UIOperaterCardList":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIOperaterCardList>();
//             case "UIExpandBag":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIExpandBag>();
//             case "UIRUSure":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIRUSure>();
//             case "UIPopWnd":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIPopWnd>();
//             case "UIShop":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIShop>();
//             case "UIGuild":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIGuild>();
//             case "UIFriend":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIFriend>();
//             case "UIFoundFriendByID":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIFoundFriendByID>();
//             case "UIFriendList":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIFriendList>();
//             case "UIApplyList":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIApplyList>();
//             case "UISetingPanel":
//                 return UIManager.Singleton.GetUIWithoutLoad<UISetingPanel>();
//             case "UILevelUp":
//                 return UIManager.Singleton.GetUIWithoutLoad<UILevelUp>();
//             case "UIDeveloper":
//                 return UIManager.Singleton.GetUIWithoutLoad<UIDeveloper>();
//             case "UILoadingResources":
//                 return UIManager.Singleton.GetUIWithoutLoad<UILoadingResources>();
//             case "UICardUpdate":
//                 return UIManager.Singleton.GetUIWithoutLoad<UICardUpdate>();
//             default:
//                 break;
//         }
//         return null;


    /*
    /// <summary>
    /// /////////////////////////////////////////
    /// </summary>
    public EDUITYPE m_curUIType = 0;
    public String m_curUIRootName = "";
    public UIWindow m_curUIRootWnd = null;
    public String m_curUIName = "";
    //public UIWindow[] m_wndList = new UIWindow[6];
    public List<UIWindow> m_wndList = new List<UIWindow>();
    public int m_wndIndex = 0;
    Dictionary<String, int> m_getWndIndexByWndStr = new Dictionary<String, int>();
    //Dictionary<MainUIManager.EDUITYPE, UIWindow> m_showWndList = new Dictionary<MainUIManager.EDUITYPE, UIWindow>();
    Dictionary<String, UIWindow> m_showWndList = new Dictionary<String, UIWindow>();
    Dictionary<int, UIWindow> m_WndListByLevel = new Dictionary<int, UIWindow>();

    public void InitData(UIWindow wnd)
    {
        m_getWndIndexByWndStr[wnd.ToString()] = m_wndList.Count;
        m_wndList.Add(wnd);
        m_curUIRootWnd = wnd;
        m_curUIRootName = wnd.ToString();
    }
    public void PushUIList(UIWindow pObject)
    {
        m_getWndIndexByWndStr.Add(pObject.ToString(), m_wndIndex);
        m_wndList[m_wndIndex++] = pObject;       
    }
    public void PopUIList(UIWindow pObject)
    {

    }

    public Boolean ChangeUI(UIWindow wnd)
    {
        if (wnd.m_uiWindowCFG.m_uiWindowType == ENUIWindowLevel.enTrough)
        {
            return true;
        }
        else if (((wnd.m_uiWindowCFG.m_uiWindowType == ENUIWindowLevel.enRoot && (m_curUIRootName != wnd.ToString()))) || wnd.m_castRootAble)
        {
            if (ChangeUIByRoot(wnd))
            {
                return true;
            }
        }
        else
        {
            if (ChangeUIByUILevel(wnd))
            {
                return true;
            }
        }
        return false;
    }
    //转换根界面
    public Boolean ChangeUIByUIType(UIWindow wnd)
    {

        if (m_curUIRootName == wnd.ToString() && m_curUIName == wnd.ToString())
        {
            return false;
        }
        int tmpWndLevel = 0;
        if (wnd.m_castRootAble)
        {
            tmpWndLevel = 0;
        }
        else
        {
            tmpWndLevel = wnd.m_uiWindowCFG.m_uiWndLevel;
        }
        if ((m_curUIRootWnd == null) || m_curUIRootWnd.OnLeave())
        {
            HideCurUITypeList();
            RemoveCurUITypeList();

            m_WndListByLevel.Add(tmpWndLevel,wnd);
            m_curUIRootWnd = wnd;
            m_curUIRootName = wnd.ToString();
            m_curUIName = wnd.ToString();
            return true;
        }

        return false;
    }
    //不是根界面第一次进入和界面的转换
    public Boolean ChangeUIByUILevel(UIWindow wnd)
    {

        if (m_curUIName == wnd.ToString())
        {
            return false;
        }
        int tmpWndLevel = 0;
        if (wnd.m_castRootAble)
        {
            tmpWndLevel = 0;
        }
        else
        {
            tmpWndLevel = wnd.m_uiWindowCFG.m_uiWndLevel;
        }
        if (m_WndListByLevel.ContainsKey(tmpWndLevel)) 
        {
            int wndListCount = m_WndListByLevel.Count;
            if (tmpWndLevel > wndListCount)
            {
                return false;
            }
            List<int> tmpUIKeyList = new List<int>();
            foreach (var uiItem in m_WndListByLevel)
            {
                if (wnd.m_uiWindowCFG.m_uiWndLevel <= uiItem.Value.m_uiWindowCFG.m_uiWndLevel)
                {
                    uiItem.Value.HideWindow();
                    tmpUIKeyList.Add(uiItem.Key);
                }
            }
//             if (tmpUIKeyList.Count == 0)
//             {
//                 return false;
//             }
//             m_WndListByLevel[tmpWndLevel].HideWindow();
//             tmpUIKeyList.Add(tmpWndLevel);
            for (int i=0;i<tmpUIKeyList.Count;i++)
            {
                m_WndListByLevel.Remove(tmpUIKeyList[i]);
            }
           
            m_WndListByLevel.Add(tmpWndLevel, wnd);
            m_curUIName = wnd.ToString();
            return true;
        }
        else
        {
            if (wnd.m_uiWindowCFG.m_isHideOther)
            {
                HideCurUITypeList();
            }
            m_WndListByLevel.Add(tmpWndLevel, wnd);
            m_curUIName = wnd.ToString();
            return true;
        }
    }

    public void HideCurUITypeList()
    {
        foreach (UIWindow uiItem in m_WndListByLevel.Values)
        {
            if (uiItem.IsVisiable())
            {
                uiItem.HideWindow();
            }
        }
    }
    public void RemoveCurUITypeList()
    {
        m_WndListByLevel.Clear();
        m_curUIRootWnd = null;
        m_curUIRootName = "";
        m_curUIName = "";
    }
}*/
