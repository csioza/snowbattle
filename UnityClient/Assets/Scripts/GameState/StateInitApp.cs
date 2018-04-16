using UnityEngine;
using System.Collections;

public class StateInitApp : State
{
	int waiting_time = 0;
	public override void OnEnter()
	{
		MiniServer.Singleton.gateserver_globalconfig();
	}

	public override void OnFixedUpdate()
	{
		//依据服务端状态,进入登录,或者进入战斗模式
		if (GlobalServerSetting.Singleton.GotSetting == 1)
		{
			MainGame.Singleton.TranslateTo(new StateLogin());
		}
		else if (GlobalServerSetting.Singleton.GotSetting == 2)
		{
			MainGame.Singleton.TranslateTo(new StateBattle());
		}
		//如果太久,也进入battle
		waiting_time++;
		if (waiting_time >= 100)
		{
			MainGame.Singleton.TranslateTo(new StateBattle());
		}
	}
    public override void OnUpdate()
    {
        ClientNet.Singleton.Update();
    }
}
