using UnityEngine;
using System.Collections;

class StateLoadingResources : State
{

    public override void OnEnter() 
    {
        MainUIManager.Singleton.OnLoadStateUI("StateLoadingResources");
        //UILoadingResources.GetInstance().ShowWindow();
        SM.RandomRoomLevel.Singleton.PrebuildSceneData();
        
    }
    public override void OnUpdate()
    {
        LoadingResourcesProp.Singleton.Tick();
    }

    public override void OnExit()
    {
        MainUIManager.Singleton.OnExitDestroy("StateLoadingResources");
        base.OnExit();
    }
}