using System;
using UnityEngine;

//不能移动
public class ControlMoveAction : ActorAction
{
    public enum ENAnimType
    {
        enNone,
        enVertigo,//眩晕
        enControlMove,//定身
        enFreeze,//冻结
        enParalysis,//麻痹
        enSleep,//睡眠
    }
    public override ENType GetActionType() { return ENType.enControlMoveAction; }
    public static ENType SGetActionType() { return ENType.enControlMoveAction; }

    public string AnimName { get; private set; }

    public void Init(ENAnimType type)
    {
        switch (type)
        {
            case ENAnimType.enVertigo:
                {
                    AnimName = "dizziness";
                }
                break;
            case ENAnimType.enControlMove:
                {
                    AnimName = "standby";
                }
                break;
            case ENAnimType.enFreeze:
                {
                    AnimName = "standby";
                }
                break;
            case ENAnimType.enParalysis:
                {
                    AnimName = "standby";
                }
                break;
            case ENAnimType.enSleep:
                {
                    AnimName = "standby";
                }
                break;
            default:
                {
                    AnimName = "standby";
                }
                break;
        }
        RefreshActionRef();
    }
    public override void OnEnter()
	{
	}

	public override bool OnUpdate()
	{
        return false;
	}
}