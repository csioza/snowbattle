using UnityEngine;
using System.Collections;

public class NPCBehaviour : MonoBehaviour {
    public Actor CurrentActor {get;set;}
    public float DeadDuration { get { return 10f; } private set { } }
	void OnBecameInvisible()
    {
        //角色模型会被复用，挂载此脚本的有可能不是npc
        if (CurrentActor != null)
        {
            CurrentActor.OnBecameInvisible();
            //ActorManager.Singleton.ReleaseActor(CurrentActor.ID);
        }
        
    }
}
