//////////////////////////////////////////////////////////////////////////
//
//	file path:	E:\Codes\XProject\Assets\Scripts\Actor\ActorAction
//	created:	2013-4-16
//	author:		Mingzhen Zhang
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

//站立
public class TrapStandAction : TrapAction
{
    public override ENType GetActionType() { return ENType.enStandAction; }
    public static ENType SGetActionType() { return ENType.enStandAction; }

    //是否完成
    public bool IsFinished { get; set; }
    //当前动画名称
    public string AnimName = "t-lock-enter";
    public void Init(string animName)
    {
        AnimFullName = animName;
    }
    public override void OnEnter()
    {
        RefreshActionRef();
    }
    public override void OnInterupt(TrapAction.ENType newType)
    {
        CurrentActor.MainAnim.Stop();
        OnExit();
    }
    public override void OnExit()
    {

    }
    public override bool OnUpdate()
    {
        if (!IsFinished)
        {
            if (Time.time - AnimStartTime > AnimLength)
            {//当前动画播放完毕
                IsFinished = true;
            }
        }
        return IsFinished;
    }
    public override void Reset()
    {
        base.Reset();
        AnimName = "";
    }
};