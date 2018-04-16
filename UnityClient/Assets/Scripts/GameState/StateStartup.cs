using UnityEngine;
using System.Collections;

public class StateStartup : State
{
    public override void OnEnter()
    {
        //MainUIManager.Singleton.OnLoadStateUI("StateStartup");
        UIDebugStartUp ui = UIDebugStartUp.GetInstance();//.ShowWindow();
        ui.ShowWindow();
        //GameResMng.ForcePackage = true;
        //GameResMng.GetResMng().StartWWW(true);
    }
    public override void OnExit()
    {
        //MainUIManager.Singleton.OnExitDestroy("StateStartup");
        UIDebugStartUp self = UIManager.Singleton.GetUIWithoutLoad<UIDebugStartUp>();
 
        self.Destroy();
        base.OnExit();
    }

    public override void OnFixedUpdate()
    {

    }
    public override void OnUpdate()
    {
        UIDebugStartUp self = UIManager.Singleton.GetUIWithoutLoad<UIDebugStartUp>();
        ClientNet.Singleton.Update();
        if (GameResManager.Singleton.m_isPreLoadFinish && self.CanProcessMainGame)
        {
            GameTable.LoadTable();
            if (!GameSettings.Singleton.m_isSingle)
            {
                MainGame.Singleton.TranslateTo(new StateLogin());
            }
        }
    }
    public override void OnGUI()
    {
        GameResManager.Singleton.DrawGui();
        //GameResMng.DrawGui();
    }
}
