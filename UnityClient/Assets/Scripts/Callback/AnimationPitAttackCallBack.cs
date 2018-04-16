using UnityEngine;
using System.Collections;
using System;

public class AnimationPitAttackCallBack : MonoBehaviour
{

    public Action<GameObject, AnimationEvent> Callback { get; set; }
    private bool isOver = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if (isOver)
        {
            return;
        }
        Transform targetObj = other.transform;
        while (null != targetObj && targetObj.name != "body")
        {
            targetObj = targetObj.parent;
        }
        if (null == targetObj)
        {
            return;
        }
        ActorProp prop = targetObj.parent.GetComponent<ActorProp>();
        if (null == prop)
        {
            return;
        }
        if (prop.ActorLogicObj.IsDead)
        {
            return;
        }
        if (prop.Type == ActorType.enMain)
        {
            WorldParamInfo WorldParamList;
            WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enAttackActionName);
            gameObject.GetComponent<Animation>().Play(WorldParamList.StringTypeValue);
            WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enAttackedActionName);
            targetObj.gameObject.GetComponent<Animation>().Play(WorldParamList.StringTypeValue);
            isOver = true;
        }
    }
}
