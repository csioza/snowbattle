using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGE.Network;

public class StateMainUI : State
{
    public void InitGUI()
    {

        MainUIManager.Singleton.OnLoadStateUI("StateMainUI");
    }

  
	public override void OnEnter()
	{
        InitGUI();
        //UIMainButtonList.GetInstance().HideWindow();
        //UIMain.GetInstance().HideWindow();
        //UIZone.GetInstance().HideWindow();
        //StageMenu.Singleton.ShowMenu(2);
        //MainUIManager.Singleton.OnSwitchSingelUI(MainUIManager.EDUITYPE.enStageMenu);
//         UIMain main = UIMain.GetInstance();
//         main.ShowWindow();
//         UIMainButtonList mainButtonList = UIMainButtonList.GetInstance();
//         mainButtonList.ShowWindow();
//         UIZone zone = UIZone.GetInstance();
//         Zone.Singleton.ShowZone();
	}
	public override void OnExit()
	{
        MainUIManager.Singleton.OnExitDestroy("StateMainUI");


//         UICardUpdate.GetInstance().Destroy();
// 
// 
//         UICardEvolution.GetInstance().Destroy();
// 
//         UIGetCard.GetInstance().Destroy();
// 
//        // UIManager.Singleton.RemoveUI<UIMain>();
// 
//         UIStageMenu.GetInstance().Destroy();
// 
//         UICardBag.GetInstance().Destroy();
// 
//         UICard.GetInstance().Destroy();
// 
//         UICardDetail.GetInstance().Destroy();
// 
//         UIOperaterCardList.GetInstance().Destroy();
// 
//         UIExpandBag.GetInstance().Destroy();
// 
//         UIRUSure.GetInstance().Destroy();
// 
//         UIPopWnd.GetInstance().Destroy();
// 
//         //UILoading.GetInstance().Destroy();
// 
//         UITipMessageBox.GetInstance().Destroy();
// 
//         UIZone.GetInstance().Destroy();
// 
//         UIMainButtonList.GetInstance().Destroy();
// 
//         UITeam.GetInstance().Destroy();
//         UIGuild.GetInstance().Destroy();
//         UIFriend.GetInstance().Destroy();
//         UIFoundFriendByID.GetInstance().Destroy();
//         UIFriendList.GetInstance().Destroy();
//         UIApplyList.GetInstance().Destroy();
//         UILevelUp.GetInstance().Destroy();
//         UIShop.GetInstance().Destroy();
//         UISetingPanel.GetInstance().Destroy();
//         UIDeveloper.GetInstance().Destroy();
		base.OnExit();

        
	}
	public override void OnUpdate()
	{
        ClientNet.Singleton.Update();
		User.Singleton.Update();
	}
	public override void OnLateUpdate()
	{
		
	}
	public override void OnFixedUpdate()
	{
       
       

     

//         if (m_bShowBattleSummary)
//         {
//             BattleSummary.Singleton.OnShowBattleSummary();
//             m_bShowBattleSummary = false;
//         }

        if (SwitchNexted)
        {
            MainGame.Singleton.TranslateTo(new StateLoadingResources());
            SwitchNexted = false;
        }

        
	}
	public override void OnPause()
	{

	}
	public override void OnResume()
	{
	
	}

    public override void OnSwipe(Vector2 total, FingerGestures.SwipeDirection direction)
    {
        if (IsGamePause())
        {
            return;
        }
        if (direction == FingerGestures.SwipeDirection.Left)
        {
            Debug.Log("向左");
            StageMenu.Singleton.ChangeTeam(false);
        }
        else if (direction == FingerGestures.SwipeDirection.Right)
        {
            Debug.Log("向右");
             StageMenu.Singleton.ChangeTeam(true);
        }
    
    }


}