using UnityEngine;
using System.Collections;
using NGE.Network;

public class ResultRelive : IResult
{
    public ResultRelive()
		:base((int)ENResult.Relive)
	{
	}
    public static IResult CreateNew()
    {
        return new ResultRelive();
    }
    public override void Exec(IResultControl control)
    {
        base.Exec(control);
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            return;
        }
        ActorAction action = actor.ActionControl.AddAction(ActorAction.ENType.enReliveAction);
        if (action == null)
        {//添加复活action失败，则删除所有的action，再添加复活action
            actor.ActionControl.RemoveAll();
            actor.ActionControl.AddAction(ActorAction.ENType.enReliveAction);
        }
    }

    public override void ResultServerExec(IResultControl control)
    {
        base.ResultServerExec(control);
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            return;
        }
		if (actor.Type == ActorType.enMain)
		{
			MainPlayer player = actor as MainPlayer;
			//通知头像
			player.NotifyChanged((int)Actor.ENPropertyChanged.enMainHead, EnMainHeadType.enActorRelive);
		}
		if (actor.Type == ActorType.enMain || actor.Type == ActorType.enPlayer)
		{//主控角色或其他玩家
			if (!actor.IsActorExit)
			{//战斗中
				actor.ForceMoveToPosition(SM.RandomRoomLevel.Singleton.m_sceneCampReliveNode[actor.Camp]);
			}
		}
		if (actor.Type == ActorType.enMain && !actor.IsActorExit)
		{
			MainGame.Singleton.MainCamera.MoveAtOnce(actor);
		}
		//设置头顶血条
        ActorAction action = actor.ActionControl.AddAction(ActorAction.ENType.enReliveAction);
        if (action == null)
        {//添加复活action失败，则删除所有的action，再添加复活action
            actor.ActionControl.RemoveAll();
            actor.ActionControl.AddAction(ActorAction.ENType.enReliveAction);
        }
    }
}
