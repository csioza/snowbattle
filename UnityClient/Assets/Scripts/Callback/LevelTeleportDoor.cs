using System;
using System.Collections;
using UnityEngine;

public class LevelTeleportDoor : MonoBehaviour
{
    static string sboss     = "boss";
    static string slevel    = "level";
    public int      LevelId = 0;
    public string   Target = "";
    void OnTriggerEnter(Collider other)
	{
        if (other.isTrigger)
        {
            return;
        }
        Transform targetObj = other.transform;
        while (null != targetObj && targetObj.name != "body")
        {
            targetObj = targetObj.parent;
        }
        if (targetObj == null)
        {
            return;
        }
        ActorProp prop = targetObj.parent.GetComponent<ActorProp>();
        Actor targetActor = prop.ActorLogicObj;
        if (targetActor.Type != ActorType.enMain)
        {
            return;
        }
        MainPlayer player = targetActor as MainPlayer;
        SM.SceneRoomInfoTree infoTree = null;
        if (Target == sboss)
        {
            infoTree = SM.RandomRoomLevel.Singleton.LookupBossRoomInfo(LevelId);
            if (null != infoTree)
            {
                player.Teleport(infoTree.CharPosTransform);
            }
        }
        else if (Target == slevel)
        {
            SM.RandomRoomLevel.Singleton.OnLeaveLevel(LevelId);
            SM.RandomRoomLevel.Singleton.OnEnterlevel(LevelId + 1);
        }
        
	}
}
