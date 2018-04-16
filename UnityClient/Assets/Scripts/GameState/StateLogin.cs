using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGE.Network;

public class StateLogin : State
{

    public void InitGUI()
    {
        MainUIManager.Singleton.OnLoadStateUI("StateLogin");
//         UIGameInit btn = UIGameInit.GetInstance();
// 
//         btn.ShowWindow();
// 
//         UILogin loginBtn    = UILogin.GetInstance();
// 
//         loginBtn.HideWindow();
// 
//         UIRegist registBtn  = UIRegist.GetInstance();
// 
//         registBtn.HideWindow();
// 
//         UIServerList server = UIServerList.GetInstance();
// 
//         server.HideWindow();
// 
//         UICard card         = UICard.GetInstance();
// 
//         card.HideWindow();
// 
//         UIChooseCard wnd    = UIChooseCard.GetInstance();
//         wnd.HideWindow();
// 
//         UICreateActor createWnd = UICreateActor.GetInstance();
//         createWnd.HideWindow();
// 
//         UIDebugStartUp.GetInstance().HideWindow();
//         Login.Singleton.OnInit();
// 
//         //UILoading.GetInstance().HideWindow();
// 
//         UIRUSure.GetInstance().HideWindow();

    }

	public override void OnEnter()
	{
		ActorManager.Singleton.ClearActor();
		GameObject areaObj = GameObject.Find("SceneAreas");
		if (null != areaObj)
		{
			GameObject.Destroy(areaObj);
		}

        InitGUI();

       // MiniServer.Singleton.account_serverconfig();

		//UIManager.Singleton.RemoveAllUI();

        //UIManager.Singleton.LoadUI<UIServerList>();
        //UIMapDisplay map = UIManager.Singleton.GetUI<UIMapDisplay>();
        //map.HideWindow();
	}
	public override void OnExit()
	{
        // 释放资源
        MainUIManager.Singleton.OnExitDestroy("StateLogin");

//         UIGameInit.GetInstance().Destroy();
// 
//         UILogin.GetInstance().Destroy();
// 
//         UIRegist.GetInstance().Destroy();
// 
//         UIServerList.GetInstance().Destroy();
// 
//         UICard.GetInstance().Destroy();
// 
//         UIChooseCard.GetInstance().Destroy();
// 
//         UICreateActor.GetInstance().Destroy();
// 
//         UIDebugStartUp.GetInstance().Destroy();
// 
//         UIRUSure.GetInstance().Destroy();

		base.OnExit();
	}
	public override void OnUpdate()
	{
        ClientNet.Singleton.Update();
	}
	public override void OnLateUpdate()
	{
		
	}
	public override void OnFixedUpdate()
	{
		
	    //MainGame.Singleton.TranslateTo(new StateMainUI());
		
	}
	public override void OnPause()
	{

	}
	public override void OnResume()
	{
		//base.OnResume();
	}

	//public override void OnTap(Vector2 fingerPos) { }
	//public override void OnDoubleTap(Vector2 fingerPos) { }
	//public override void OnDragMove(Vector2 fingerPos, Vector2 delta) { }
	//public override void OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta) { }
	//public override void OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta) { }
	//public override void OnTwoFingerDragMoveEnd(Vector2 fingerPos) { }
	//public override void OnDrawBegin(Vector2 fingerPos, Vector2 startPos) { }
	//public override void OnDrawEnd(Vector2 fingerPos) { }
	//public override void OnPressUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown) { }
	//public override void OnPressDown(int fingerIndex, Vector2 fingerPos) { }
	//public override void OnRotate(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta) { }
	//public override void OnRotateEnd(Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle) { }
	//public override void OnBackKey() { }
}