using UnityEngine;
using System.Collections;
using NGE.Network;

public class ResultDrag : IResult
{
    enum DragType
    {
        enNull,
        enNationwide,
    }
    float[] mParam = new float[] { };
    Actor mSelfActor = null;
    public ResultDrag()
        : base((int)ENResult.Drag)
    {

    }
    public static IResult CreateNew()
    {
        return new ResultDrag();
    }
    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);
        if (param != null)
        {
            mParam = param;
        }
    }
    public override void Exec(IResultControl control)
    {
        mSelfActor = ActorManager.Singleton.Lookup(SourceID);

        switch ((DragType)mParam[0])//被拖拽类型
        {
            case DragType.enNationwide:
                ActorManager.Singleton.ForEach_result(CheckActorDistance, mParam);
                break;
            default:
                break;
        }
    }
    void CheckActorDistance(Actor target, float[] param)
    {
        if (target.Type == ActorType.enNPC)
        {
            NPC npc = target as NPC;
            if (npc.GetNpcType() == ENNpcType.enBlockNPC ||
                npc.GetNpcType() == ENNpcType.enFunctionNPC)
            {
                return;
            }
        }
        else if (target.Type == ActorType.enMain)
        {
            if (target.IsActorExit)
            {
                return;
            }
        }
        if (target.IsDead)
        {
            return;
        }
        if (!ActorTargetManager.IsEnemy(mSelfActor, target))
        {
            return;
        }
        float distance = ActorTargetManager.GetTargetDistance(mSelfActor.RealPos, target.RealPos);
        if (distance <= param[1])//拖拽生效半径
        {
            Vector3 targetPos = mSelfActor.MainPos;
            DragMoveAction action = target.ActionControl.AddAction(DragMoveAction.SGetActionType()) as DragMoveAction;
            if (null != action)
            {
                action.Init(targetPos, param[2], param[3]);//拖拽偏移量，拖拽速度
            }
            else
            {
                Debug.LogWarning("add DragMoveAction failed");
            }
        }
    }
}








// using UnityEngine;
// 
// //冻结
// class ResultDrag : IResult
// {
//     enum DragType
//     {
//         enNull,
//         enNationwide,
//     }
//     Actor mSelfActor = null;
//     public ResultDrag()
//         : base(ENResult.Drag)
//     {
//     }
//     public static IBuffEffect CreateNew()
//     {
//         return new ResultDrag();
//     }
//     public override void ResultExpr(float[] param)
//     {
//         base.ResultExpr(param);
//     }
//     public override void OnGetBuffEffect()
//     {
//         base.OnGetBuffEffect();
//         Actor actor = ActorManager.Singleton.Lookup(TargetID);
//         if (null == actor)
//         {
//             Debug.LogWarning("OnGetBuffEffect failed! actor is not exist! actor id=" + TargetID.ToString());
//             return;
//         }
//         mSelfActor = ActorManager.Singleton.Lookup(SourceID);
//         BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
//         if (info == null)
//         {
//             Debug.LogWarning("OnGetBuffEffect failed! buff is not exist! buff id=" + BuffID.ToString());
//             return;
//         }
//         foreach (var item in info.BuffResultList)
//         {
//             if (item.ID == (int)ClassID)
//             {
//                switch ((DragType)item.ParamList[0])
//                {
//                    case DragType.enNationwide:
//                        ActorManager.Singleton.ForEach_buff(CheckActorDistance, item);
//                        break;
//                    default:
//                        break;
//                }
//             }
//         }
//     }
//     void CheckActorDistance(Actor target, BuffResultInfo info)
//     {
//         if (target.Type == ActorType.enNPC)
//         {
//             NPC npc = target as NPC;
//             if (npc.GetNpcType() == ENNpcType.enBlockNPC ||
//                 npc.GetNpcType() == ENNpcType.enFunctionNPC)
//             {
//                 return;
//             }
//         }
//         else if (target.Type == ActorType.enMain)
//         {
//             MainPlayer player = target as MainPlayer;
//             if (player.IsActorExit)
//             {
//                 return;
//             }
//         }
//         if (target.IsDead)
//         {
//             return;
//         }
// 
//         switch ((ENTargetType)info.ParamList[0])
//         {
//             case ENTargetType.enEnemy:
//                 {
//                     if (!ActorTargetManager.IsEnemy(mSelfActor, target))
//                     {
//                         return;
//                     }
//                 }
//                 break;
//             case ENTargetType.enFriendly:
//                 {
//                     if (!ActorTargetManager.IsFriendly(mSelfActor, target))
//                     {
//                         return;
//                     }
//                 }
//                 break;
//             case ENTargetType.enSelf:
//                 {
//                     if (mSelfActor != target)
//                     {
//                         return;
//                     }
//                 }
//                 break;
//             case ENTargetType.enNullTarget:
//                 break;
//             case ENTargetType.enFriendlyAndSelf:
//                 {
//                     if (!ActorTargetManager.IsFriendly(mSelfActor, target) && mSelfActor != target)
//                     {
//                         return;
//                     }
//                 }
//                 break;
//             default:
//                 break;
//         }
//         float distance = ActorTargetManager.GetTargetDistance(mSelfActor.RealPos, target.RealPos);
//         if (distance <= info.ParamList[1])
//         {
//             Vector3 targetPos = target.MainPos;
//             DragMoveAction action = target.ActionControl.AddAction(DragMoveAction.SGetActionType()) as DragMoveAction;
//             if (null != action)
//             {
//                 action.Init(targetPos, info.ParamList[2], info.ParamList[3]);
//             }
//             else
//             {
//                 Debug.LogWarning("add DragMoveAction failed");
//             }
//         }
//     }
// }