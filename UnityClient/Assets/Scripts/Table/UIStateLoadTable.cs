//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEngine;


public class UILoadInfo : IDataBase
{
    public int          UIID { get; private set; }		        //UIID
    public string       UINameStr { get; private set; }         //当前UI名字
    public string       UIStateStr { get; private set; }        //该UI所属的State
    public bool         IsDynamicLoad { get; private set; }     //是否动态加载
    public bool     IsInstantDelete { get; private set; }   //是否在当前状态退出销毁
    public bool     isOnceHide { get; private set; }        //起始是否隐藏
    public bool     isAsyncLoad { get; private set; }       //是否是异步加载
    public bool     IsExitDestroy { get; private set; }     //退出后是否销毁
    public int      UIWndType { get; private set; }      //UI的节点类型
    public int UIWndLvl { get; private set; }       //UI的等级
    public bool IsHideOther { get; private set; }       //UI出现是否隐藏其他窗口
    public int OnLeaveType { get; private set; }        //窗口何时调用OnLeave函数的类型

}

public class UILoadInfoTable
{
    public Dictionary<string, List<UILoadInfo>> m_stateUIDic { get; protected set; }
    public Dictionary<int, UILoadInfo> m_UILoadDic { get; protected set; }
/*    public Dictionary<string, List<UILoadInfo>> m_UIStrDic { get; protected set; }*/
    public List<UILoadInfo> Lookup_State(string stateStr)
    {
        List<UILoadInfo> info = null;
        m_stateUIDic.TryGetValue(stateStr, out info);
        return info;
    }
    public UILoadInfo Lookup_UI(string uiStateStr, string uiStr)
    {
        List<UILoadInfo> uiInfoList = new List<UILoadInfo>();
        UILoadInfo info = null;
        if (m_stateUIDic.TryGetValue(uiStateStr, out uiInfoList))
        {
            foreach (var item in uiInfoList)
            {
                if (item.UINameStr == uiStr)
                {
                    info = item;
                }
            }
        }
        return info;
    }
    public void Load(byte[] bytes)
    {
        m_stateUIDic = new Dictionary<string, List<UILoadInfo>>();
        m_UILoadDic = new Dictionary<int, UILoadInfo>();
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        for (int index = 0; index < length; ++index)
        {
            UILoadInfo info = new UILoadInfo();
            info.Load(helper);
            m_UILoadDic.Add(info.UIID, info);

            if (m_stateUIDic.ContainsKey(info.UIStateStr))
            {
                m_stateUIDic[info.UIStateStr].Add(info);
            }
            else
            {
                List<UILoadInfo> tmpUILoadInfoList = new List<UILoadInfo>();
                tmpUILoadInfoList.Add(info);
                m_stateUIDic.Add(info.UIStateStr, tmpUILoadInfoList);
            }
            
        }
    }
};