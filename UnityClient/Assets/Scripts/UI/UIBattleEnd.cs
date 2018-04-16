using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBattleEnd : UIWindow
{
    static public UIBattleEnd GetInstance()
    {
        UIBattleEnd self = UIManager.Singleton.GetUIWithoutLoad<UIBattleEnd>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIBattleEnd>("UI/UIBattleEnd", UIManager.Anchor.Center);
        return self;
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enSceneManager, OnPropertyChanged);
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)SM.RandomRoomLevel.ENPropertyChanged.enEndBattle)
        {
            MainGame.Singleton.OnAppLogicPause(true);
            ShowWindow();
        }
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("ButtonAgain", OnButtonAgain);
        AddChildMouseClickEvent("ButtonOk", OnButtonOk);
    }
    public void OnButtonOk(object sender, EventArgs e)
    {
        MainGame.Singleton.OnAppLogicPause(false);
        BattleArena.Singleton.StageStop(true, false);
        HideWindow();
    }
    public void OnButtonAgain(object sender, EventArgs e)
    {
        if (GameSettings.Singleton.m_isSingle)
        {
            MainGame.Singleton.OnAppLogicPause(false);
            //临时复活逻辑
            Relive();
            HideWindow();
            SM.RandomRoomLevel.Singleton.OnLeaveLevel();
            SM.RandomRoomLevel.Singleton.OnEnterlevel(1);
            
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
    }
    //服务器消息回调
    void OnServerMsgCallBack(MessageRespond respond)
    {
        if (respond.IsSuccess)
        {
            MainGame.Singleton.OnAppLogicPause(false);
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
        BattleArena.Singleton.StageStop(true, false);
        HideWindow();
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

}
