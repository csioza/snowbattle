using UnityEngine;
using NGE.Network;

//立即死亡
public class ResultInstantDeath : IResult
{
    enum ENType
    {
        enNone,
        enHpPercent, //杀死除NPC中某个类型、血量少于一定百分比的敌人
        enProbability, //一定几率杀死敌人
    }
    public bool IsInstantDeath { get; private set; }
    public ResultInstantDeath()
		:base((int)ENResult.InstantDeath)
	{
        IsInstantDeath = false;
	}
    public static IResult CreateNew()
    {
        return new ResultInstantDeath();
    }

    public override void Deserialize(PacketReader stream)
    {
        base.Deserialize(stream);
        IsInstantDeath = (stream.ReadInt32()==1) ? true:false;
        Debug.Log(" ResultInstantDeath Deserialize:" + ",IsInstantDeath:" + IsInstantDeath + ",TargetID:" + TargetID);
    }


    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);
        if (param == null || param.Length != 5)
        {
            return;
        }
        switch ((ENType)param[0])
        {
            case ENType.enHpPercent:
                {
                    Actor target = ActorManager.Singleton.Lookup(TargetID);
                    if (target == null || target.IsDead)
                    {
                        return;
                    }
                    float percent = target.HP / target.MaxHP;
                    if (percent >= param[1])
                    {
                        return;
                    }
                    if (target.Type == ActorType.enNPC)
                    {
                        NPC npc = target as NPC;
                        if (npc.CurrentTableInfo.Type == (int)param[2])
                        {
                            return;
                        }
                    }
                    IsInstantDeath = true;
                }
                break;
            case ENType.enProbability:
                {
                    float r = Random.Range(0.0f, 1.0f);
                    if (r > param[1])
                    {
                        return;
                    }
                    IsInstantDeath = true;
                }
                break;
            default:
                break;
        }
    }
    public override void Exec(IResultControl control)
    {
        base.Exec(control);
        if (IsInstantDeath)
        {
            Actor target = ActorManager.Singleton.Lookup(TargetID);
            if (target == null || target.IsDead)
            {
                return;
            }
            target.SetCurHP(0);
            //死亡result
            IResult r = BattleFactory.Singleton.CreateResult(ENResult.Dead, SourceID, TargetID);
            if (r != null)
            {
                r.ResultExpr(null);
                BattleFactory.Singleton.DispatchResult(r);
            }
        }
    }

    public override void ResultServerExec(IResultControl control)
    {
        base.ResultServerExec(control);

        if (IsInstantDeath)
        {
            Actor target = ActorManager.Singleton.Lookup(TargetID);
            if (target == null || target.IsDead)
            {
                return;
            }
            target.SetCurHP(0);
        }

    }
}