//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Callback
//	created:	2013-5-20
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;


public class BreakPointCallback : MonoBehaviour
{
    void BreakPoint(AnimationEvent animEvent)
    {
        throw new Exception("no support this call");
        //ActorProp selfProp = transform.parent.GetComponent<ActorProp>();
        //if (null == selfProp.ActorLogicObj.TargetManager || selfProp.ActorLogicObj.TargetManager.TargetList.Count > 0)
        //{
        //    return;
        //}
        //selfProp.ActorLogicObj.MoveTo(selfProp.ActorLogicObj.RealPos + selfProp.ActorLogicObj.MainObj.transform.forward);
    }
};