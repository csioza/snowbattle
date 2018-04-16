using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public enum ENReliveOption
{
	enNoRelive = 0,   //不复活
	enFloorRelive = 1,//楼层复活
	enBuyRelive = 2, //买活
}
public enum ENReliveReslut
{
	enReliveFailed = 0, 
	enReliveSucess = 1,
}

public class UIRelive : UIWindow
{
    GameObject      m_selectObj;
    GameObject      m_choiceLayerObj;
    UIPopupList     m_popupListSelect;
    List<string>    m_strLayers = new List<string>();
    string          m_curLayerStr;
//     public int      m_origialCount = 1;
//     public int      m_retLayerCount = 1;
//     public int      m_resurrectionUnLimitedCount = 5;
//     public bool     m_resurrectionUnLimited = true;
//     public bool     m_canOrigialResurrection = true;
//     public bool     m_canRetLayerResurrection = true;
    static public UIRelive GetInstance()
	{
        UIRelive self = UIManager.Singleton.GetUIWithoutLoad<UIRelive>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIRelive>("UI/UIReborn", UIManager.Anchor.Center);
		return self;
	}
	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enMainPlayer, OnPropertyChanged);
        m_selectObj = FindChild("SelectResurrection");
        m_choiceLayerObj = FindChild("Chooselayer");
        m_popupListSelect = FindChild("popup_select").GetComponent<UIPopupList>();
        m_strLayers.Add(Localization.Get("FirstLayer"));
        m_strLayers.Add(Localization.Get("SecondLayer"));
        m_strLayers.Add(Localization.Get("ThirdLayer"));
        m_curLayerStr = m_strLayers[0];
        EventDelegate.Add(m_popupListSelect.onChange, OnPopupListChanged);
	}
    public void OnPopupListChanged()
    {
        Debug.Log("UIPopupList.current.value:" + UIPopupList.current.value);
        m_curLayerStr = UIPopupList.current.value;
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)Actor.ENPropertyChanged.enRelive)
        {
            
//             UICommonMsgBoxCfg boxCfg = WindowRoot.GetComponent<UICommonMsgBoxCfg>();
//             UICommonMsgBox.GetInstance().ShowMsgBox(OnButtonYes, OnButtonNo, boxCfg);
            OnShowRelive();
        }
		else if (eventType == (int)Actor.ENPropertyChanged.enDirectRelive)
		{
			Relive();
		}
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("Resurrection", OnButtonOrigial);
        AddChildMouseClickEvent("EntranceResurrection", OnButtonRetLayer);
        AddChildMouseClickEvent("cancel", OnButtonCacel);

        AddChildMouseClickEvent("layer", OnButtonRetLayerYes);
        AddChildMouseClickEvent("return", OnButtonRetLayerNo);
    }
    public void OnShowRelive()
    {
        if (DetermineResurrection())
        {
            m_selectObj.SetActive(true);
            ShowWindow();
        }
    }
    public bool DetermineResurrection()
    {
        bool tmpBool = false;
        GameObject resurrection = FindChild("Resurrection", m_selectObj).gameObject;
        GameObject entranceResurrection = FindChild("EntranceResurrection", m_selectObj).gameObject;

        if (BattleArena.Singleton.m_canOrigialResurrection)
        {
            if (BattleArena.Singleton.m_origialCount > 0)
            {
                //显示原地复活次数
                /*string tmp = string.Format(Localization.Get("ResurrectionTime"), BattleArena.Singleton.m_origialCount);*/
                resurrection.transform.Find("Label").gameObject.GetComponent<UILabel>().text = string.Format(Localization.Get("ResurrectionTime"), BattleArena.Singleton.m_origialCount);//"原地复活(" + string.Format(Localization.Get("CurrentFriendshipPoints"), BattleArena.Singleton.m_origialCount) + ")";
//                 resurrection.transform.FindChild("Sprite").gameObject.SetActive(false);
//                 resurrection.GetComponent<BoxCollider>().enabled = true;
                tmpBool = true;
            }
            else if (BattleArena.Singleton.m_origialCount == 0)
            {
                resurrection.transform.Find("Label").gameObject.GetComponent<UILabel>().text = Localization.Get("Resurrection");
                resurrection.transform.Find("Sprite").gameObject.SetActive(true);
                resurrection.GetComponent<BoxCollider>().enabled = false;
            }
            else if (BattleArena.Singleton.m_origialCount <= -1)
            {
                resurrection.transform.Find("Label").gameObject.GetComponent<UILabel>().text = Localization.Get("Resurrection");
                tmpBool = true;
            }
        }
        else
        {
//             resurrection.transform.FindChild("Label").gameObject.GetComponent<UILabel>().text = Localization.Get("Resurrection");
//             tmpBool = true;
//             resurrection.transform.FindChild("Sprite").gameObject.SetActive(false);
//             resurrection.GetComponent<BoxCollider>().enabled = true;
            resurrection.transform.Find("Label").gameObject.GetComponent<UILabel>().text = Localization.Get("Resurrection");
            resurrection.transform.Find("Sprite").gameObject.SetActive(true);
            resurrection.GetComponent<BoxCollider>().enabled = false;
        }

        if (CanRetLayerResurrection())
        {
            entranceResurrection.SetActive(true);
            tmpBool = true;
        }
        else
        {
            entranceResurrection.SetActive(false);
        }
        if (tmpBool)
        {
            return tmpBool;
        }
        BattleArena.Singleton.StageStop(true, false);
        //BattleSummary.Singleton.OnFinished(false);
        return false;
    }
    public bool CanOrgialResurrection()
    {
        return (BattleArena.Singleton.m_origialCount > 0) && BattleArena.Singleton.m_canOrigialResurrection;
    }
    public bool CanRetLayerResurrection()
    {
        return BattleArena.Singleton.m_canRetLayerResurrection;
    }
    //拥有原地复活次数
//     public void OnOrigialFunc()
//     {
//         if (CanRetLayerResurrection())
//         {
//             if (m_resurrectionUnLimited)
//             {
//                 //有复活次数限制，显示复活剩余次数
//                 //如果复活剩余次数为0，原地复活按钮为灰
//             }
//             m_selectObj.SetActive(true);
//         }
//         else
//         {
//             UICommonMsgBoxCfg boxCfg = WindowRoot.GetComponent<UICommonMsgBoxCfg>();
//             UICommonMsgBox.GetInstance().ShowMsgBox(OnButtonOrigial, OnButtonNo, boxCfg);
//         }
//        
//     }
//     public void ExecuteOrigial()
//     {
//         IResult r = BattleFactory.Singleton.CreateResult(ENResult.Relive, (int)EnMyPlayers.enChief, (int)EnMyPlayers.enChief);
//         if (r != null)
//         {
//             r.ResultExpr(null);
//             BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
//         }
//         if (Team.Singleton.Deputy != null)
//         {
//             r = BattleFactory.Singleton.CreateResult(ENResult.Relive, (int)EnMyPlayers.enDeputy, (int)EnMyPlayers.enDeputy);
//             if (r != null)
//             {
//                 r.ResultExpr(null);
//                 BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
//             }
//         }
//         HideWindow();
//         ++BattleArena.Singleton.ReliveCount;
//     }
//-------------------------------------------------------------
    public void OnButtonCacel(object sender, EventArgs e)
    {
        //BattleSummary.Singleton.OnFinished(false);
        //HideWindow();
        m_selectObj.SetActive(false);
        m_choiceLayerObj.SetActive(false);
        UICommonMsgBoxCfg boxCfg = WindowRoot.GetComponent<UICommonMsgBoxCfg>();
        UICommonMsgBox.GetInstance().ShowMsgBox(OnButtonYes, OnButtonNo, boxCfg);
    }
    public void OnButtonYes(object sender, EventArgs e)
    {
        BattleArena.Singleton.StageStop(true, false);
        HideWindow();
    }
    public void OnButtonNo(object sender, EventArgs e)
    {
        //BattleSummary.Singleton.OnFinished(false);
        m_selectObj.SetActive(true);
    }
    public void OnButtonOrigial(object sender, EventArgs e)
    {
        m_selectObj.SetActive(false);
        OnShowOrigialBox();
    }
    public void OnShowOrigialBox()
    {
        UICommonMsgBoxCfg boxCfg = FindChildComponent<UICommonMsgBoxCfg>("Resurrection");
        UICommonMsgBox.GetInstance().ShowMsgBox(OnButtonOrigialYes, OnButtonOrigialNo, boxCfg);
    }
    public void OnButtonOrigialYes(object sender, EventArgs e)
    {
        m_choiceLayerObj.SetActive(false);
        if (GameSettings.Singleton.m_isSingle)
        {
            //临时复活逻辑
            Relive();
            HideWindow();
        }
        else
        {
            //本地判断蓝珀灵石够不够
            if (User.Singleton.GetDiamond() <= 1)
            {
                //宝石不够显示是否充值界面
                OnShowPayBox();
                return;
            }
            //宝石够发送服务器进行二次判断
            int msgID = MiniServer.Singleton.SendFloorReliveOption(SM.RandomRoomLevel.Singleton.m_curFloorId, (int)ENReliveOption.enBuyRelive);
            RegisterRespondFuncByMessageID(msgID, OnServerMsgCallBack);
        }
        

        //宝石够发送服务器进行二次判断
//         int msgID = MiniServer.Singleton.SendFloorReliveOption(SceneManger.Singleton.m_curFloorId, (int)ENReliveOption.enBuyRelive);
//         RegisterRespondFuncByMessageID(msgID, OnServerMsgCallBack);
    }
    public void OnBuyReliveMsgCallBack(int floorId, ENReliveReslut result)
    {
        if (result == ENReliveReslut.enReliveSucess)
        {
            Relive();
            if (BattleArena.Singleton.m_origialCount > 0)
            {
                BattleArena.Singleton.m_origialCount = BattleArena.Singleton.m_origialCount - 1;
            }
            HideWindow();
        }
    }

    public void OnButtonOrigialNo(object sender, EventArgs e)
    {
        m_choiceLayerObj.SetActive(false);
        m_selectObj.SetActive(true);
    }

    public void OnShowPayBox()
    {
        UICommonMsgBoxCfg boxCfg = FindChildComponent<UICommonMsgBoxCfg>("RechargeAsk");
        UICommonMsgBox.GetInstance().ShowMsgBox(OnButtonPayYes, OnButtonPayNo, boxCfg);
    }
    public void OnButtonPayYes(object sender, EventArgs e)
    {//充值
        //
		//ShopProp.Singleton.OpenMagicStonePanel((int)ShopProp.ENBuyMagicStoneType.enReliveBuy, OnBuyDiamondCallBack);
    }

    public void OnButtonPayNo(object sender, EventArgs e)
    {//不充值
        OnShowOrigialBox();
    }

    public override void OnHideWindow()
    {
        base.OnHideWindow();
        m_selectObj.SetActive(false);
        m_choiceLayerObj.SetActive(false);
    }
    public void ShowWindow()
    {
        base.ShowWindow();
    }
    public void OnButtonRetLayer(object sender, EventArgs e)
    {
        int nNum = 1;
        nNum = SM.RandomRoomLevel.Singleton.mCurLevelId;
        if (nNum <= 1)
        {
            OnButtonRetLayerYes(null, null);
            return;
        }
        m_selectObj.SetActive(false);
        m_popupListSelect.items.Clear();
        m_choiceLayerObj.SetActive(true);
        for (int i = 0; i < nNum; i++ )
        {
            m_popupListSelect.items.Add(m_strLayers[i]);
        }
        m_popupListSelect.value = m_popupListSelect.items[0];
//         UICommonMsgBoxCfg boxCfg = FindChildComponent<UICommonMsgBoxCfg>("EntranceResurrection");
//         UICommonMsgBox.GetInstance().ShowMsgBox(OnButtonRetLayerYes, OnButtonRetLayerNo, boxCfg.mainTextPrefab, boxCfg.yesIcon, boxCfg.noIcon);
    }
    public void OnButtonRetLayerYes(object sender, EventArgs e)
    {
        int i = 0;
        for (; i < m_strLayers.Count; i++)
        {
            if (m_curLayerStr == m_strLayers[i])
            {
                break;
            }
        }
        if (i == m_strLayers.Count)
        {
            return;
        }
        Relive();
        int nLayerNum = i + 1;
        //关卡复活，清除主控角色的目标
        ActorManager.Singleton.MainActor.CurrentTarget = null;
        HideWindow();
        SM.RandomRoomLevel.Singleton.OnLeaveLevel();
        SM.RandomRoomLevel.Singleton.OnEnterlevel(nLayerNum);
        BattleArena.Singleton.m_canRetLayerResurrection = false;
    }
    public void OnButtonRetLayerNo(object sender, EventArgs e)
    {
        m_choiceLayerObj.SetActive(false);
        m_selectObj.SetActive(true);
//         BattleSummary.Singleton.OnFinished(false);
//         HideWindow();
    }
    void Relive()
    {
        IResult r = BattleFactory.Singleton.CreateResult(ENResult.Relive, ActorManager.Singleton.Chief.ID, ActorManager.Singleton.Chief.ID);
        if (r != null)
        {
            r.ResultExpr(null);
            BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
            //清除技能cd
            ActorManager.Singleton.Chief.CDControl.RemoveAll();
        }
        if (ActorManager.Singleton.Deputy != null)
        {
            r = BattleFactory.Singleton.CreateResult(ENResult.Relive, ActorManager.Singleton.Deputy.ID, ActorManager.Singleton.Deputy.ID);
            if (r != null)
            {
                r.ResultExpr(null);
                BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
            }
            //清除技能cd
            ActorManager.Singleton.Deputy.CDControl.RemoveAll();
        }
        ++BattleArena.Singleton.ReliveCount;
        //通知头像
        ActorManager.Singleton.MainActor.NotifyChanged((int)Actor.ENPropertyChanged.enMainHead, EnMainHeadType.enActorRelive);
    }

	//服务器消息回调
    void OnServerMsgCallBack(MessageRespond respond)
    {
        if (respond.IsSuccess)
        {
            Relive();
            if (BattleArena.Singleton.m_origialCount > 0)
            {
                BattleArena.Singleton.m_origialCount = BattleArena.Singleton.m_origialCount - 1;
            }
            HideWindow();
            return;
        }
		else
		{
			//服务器验证失败，宝石不够跳转到是否充值
			OnShowPayBox();
		}
//         if ((ENMessageRespond)respond.RespondCode == ENMessageRespond.enNotEnoughBMoney)
//         {
//             OnShowPayBox();
//         }
    }
	//充值界面回调
	void OnBuyDiamondCallBack(object sender, EventArgs e)
	{
		OnShowRelive();
	}
}
