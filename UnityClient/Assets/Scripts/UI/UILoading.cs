using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UILoading : UIWindow
{
    UILabel m_tips = null;
    int m_currentCorroutineIndex = 0;

    public delegate void OnCallBack();

    OnCallBack m_callBack = null;

    static public UILoading GetInstance()
	{
        UILoading self = UIManager.Singleton.GetUIWithoutLoad<UILoading>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UILoading>("UI/UILoading", UIManager.Anchor.Center);
		return self;

	}

	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enLoading, OnPropertyChanged);
        AddPropChangedNotify((int)MVCPropertyID.enUserProps, OnClinetNetPropertyChanged);
        m_tips = FindChildComponent<UILabel>("LoadingTips");


		AddPropChangedNotify((int)MVCPropertyID.enMessageRespond, OnServerMessageRespond);

		SetCallBack(ResendMessage);
	}
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Loading.ENPropertyChanged.enShow)
        {
            ShowWindow();
            UpdateInfo();
        }
        else if (eventType == (int)Loading.ENPropertyChanged.enHide)
        {
            HideWindow();
        }
		else if (eventType == (int)Loading.ENPropertyChanged.enServerLog)
		{
			OnServerLog((string)eventObj);
		}
    }

    void OnClinetNetPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        // 人物属性接受完毕
        if (eventType == (int)User.ENPropertyChanged.enOnServerProps)
        {
            // 当前状态 登陆状态
            HideWindow();
        }
    }

    public override void AttachEvent()
    {
        base.AttachEvent();
    }

	public override void OnShowWindow()
	{
		base.OnShowWindow();
		UpdateInfo();
        // 从显示界面开始计时 一段时间 还在显示 则 提示
        MainGame.Singleton.StartCoroutine(CoroutineCallBack(++m_currentCorroutineIndex));

	}

    public override void OnHideWindow()
	{
        // 关掉计时 超时提示
        m_currentCorroutineIndex++;
        base.OnHideWindow();
	}

    void UpdateInfo()
    {     
        m_tips.text = Localization.Get(GameTable.loadingTipsAsset.GetTips(Loading.Singleton.m_tipsId));
    }

	int WaitMessageID { get; set; }
	public void SetWaitMessageID(int id)
	{
		WaitMessageID = id;
	}
	void OnServerMessageRespond(int objectID, int eventType, IPropertyObject obj, object eventObj)
	{
		MessageRespond respond = (MessageRespond)eventObj;
		int messageID = respond.MessageID;
		if (messageID == WaitMessageID)
		{
			if (UICommonMsgBox.GetInstance().IsVisiable() )
			{
				UICommonMsgBox.GetInstance().HideMsgBox();
			}
			if(IsVisiable())
			{
				HideWindow();
			}			
		}
	}

    IEnumerator CoroutineCallBack(int coroutineIndex)
    {
        int timeout = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enConnectFailed).IntTypeValue;
        yield return new WaitForSeconds(timeout);
        if (coroutineIndex == m_currentCorroutineIndex)
        {
            // 
            ShowConFailed();
        }
        
    }

    // 显示连接失败的 二次弹框
    public void ShowConFailed()
    {
        Loading.Singleton.m_conFailedNum++;
        UICommonMsgBoxCfg boxCfg = WindowRoot.transform.GetComponent<UICommonMsgBoxCfg>();

        // 如果是战斗状态下 
        if (MainGame.Singleton.CurrentState is StateBattle)
        {
            boxCfg.buttonNum            = 1;

            // 如果提示次数到上限 则  出现两个按钮
            WorldParamInfo wordParmInfo = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enConTimeOutCounts);

            if ( wordParmInfo != null && Loading.Singleton.m_conFailedNum > wordParmInfo.IntTypeValue )
            {
                boxCfg.buttonNum = 2;
            }
        }

        UICommonMsgBox.GetInstance().ShowMsgBox(OnYesClicked, OnNoClicked, boxCfg);

        string buyText = Localization.Get("ConFailed");
        UICommonMsgBox.GetInstance().GetMainText().SetText(buyText);

        HideWindow();
    }

    // 显示连接失败的 离开战斗的 二次弹框
    public void ShowConFailedLeave()
    {
        UICommonMsgBoxCfg boxCfg = WindowRoot.transform.Find("LeaveConfirm").GetComponent<UICommonMsgBoxCfg>();

        UICommonMsgBox.GetInstance().ShowMsgBox(OnLeaveYesClicked, OnLeaveNoClicked, boxCfg);

        HideWindow();
    }

    void OnYesClicked(object sender, EventArgs e)
    {
        if ( null != m_callBack )
        {
            m_callBack();
        }
        //m_callBack = null;
    }

	void OnNoClicked(object sender, EventArgs e)
	{
        // 如果是战斗状态下 
        if (MainGame.Singleton.CurrentState is StateBattle)
        {
            // 离开战斗将会失去所有战利品 是否确定离开
            ShowConFailedLeave();
        }
        else
        {
            // 返回初始状态
            MainGame.Singleton.TranslateTo(new StateLogin());
        }
       
	}


    void OnLeaveYesClicked(object sender, EventArgs e)
    {
        // 返回初始状态
        MainGame.Singleton.TranslateTo(new StateLogin());
    }

    void OnLeaveNoClicked(object sender, EventArgs e)
    {
        ShowConFailed();
    }

    // 设置回调
    public void SetCallBack(OnCallBack callBack)
    {
        m_callBack = callBack;
    }

	public void ResendMessage()
	{
		MessageTransfer.Singleton.SendCacheCachedMessage();
	}

	public void OnServerLog(string logStr)
	{
		UICommonMsgBoxCfg boxCfg = new UICommonMsgBoxCfg();
		boxCfg.isUsePrefab = false;
		boxCfg.mainTextPrefab = logStr;
		boxCfg.buttonNum = 1;
		UICommonMsgBox.GetInstance().ShowMsgBox(null, null, boxCfg);
		//Debug.LogError("协议版本号与服务端不同，请更新客户端");

		HideWindow();
	}
}
