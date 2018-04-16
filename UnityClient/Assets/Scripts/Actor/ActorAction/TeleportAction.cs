//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor\ActorAction
//	created:	2013-4-16
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//移动
public class TeleportAction : ActorAction
{
    public override ENType GetActionType() { return ENType.enTeleportAction; }
    public static ENType SGetActionType() { return ENType.enTeleportAction; }
    Vector3 mTeleportToPos = Vector3.zero;
    public void Init(Vector3 pos)
    {
        mTeleportToPos = pos;
        IsInited = false;
    }
    public override void OnEnter()
    {
        CurrentActor.ForceMoveToPosition(mTeleportToPos);
    }

    public override void Reset()
    {
        base.Reset();
        mTeleportToPos = Vector3.zero;
        IsInited = true;
    }
};