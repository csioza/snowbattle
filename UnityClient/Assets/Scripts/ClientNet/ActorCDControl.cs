using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class ActorCDControl
{
    public List<CD> CDList = new List<CD>();
    public ActorCDControl()
    {
    }
    public void AddCD(int cdID, int actorID, int skillID)
    {
        IResult skillCD = BattleFactory.Singleton.CreateResult(ENResult.SkillCD, actorID, actorID, cdID, skillID, new float[5] { cdID, skillID, 0, 0, 0 });
        if (skillCD != null)
        {//设置技能CD
            skillCD.ResultExpr(new float[5] { cdID, skillID, 0, 0, 0 });
            BattleFactory.Singleton.DispatchResult(skillCD);
        }
    }
    public void Tick(Actor actor)
    {
        foreach (var item in CDList)
        {
            item.Tick(actor);
        }
        CDList.RemoveAll(item => item.IsNeedRemove);
    }
    public void RemoveCD(int cdID)
    {
        CD cd = CDList.Find(item => item.CDID == cdID);
        if (cd != null)
        {
            cd.Remove();
        }
    }
    public void RemoveAll()
    {
        foreach (var item in CDList)
        {
            item.Remove();
        }
    }
    public void CDListAdd(CD cd)
    {
        CDList.Add(cd);
    }
}
