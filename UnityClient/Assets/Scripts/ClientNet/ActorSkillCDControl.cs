using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class ActorSkillCDControl
{
    public ActorSkillCDControl()
    {
    }
    public void AddSkill(int skillID, Actor actor)
    {
        Actor.ActorSkillInfo info = actor.SkillBag.Find(item => item.SkillTableInfo.ID == skillID);
        if (null == info)
        {
            return;
        }
        CDInfo cdInfo = GameTable.CDTableAsset.Lookup(info.SkillTableInfo.CoolDown);
		if (null != cdInfo)
        {
			actor.CDControl.AddCD(cdInfo.ID, actor.ID, skillID);
		}
    }
    public void RemoveSkillCD(int skillID, Actor actor)
    {
        Actor.ActorSkillInfo info = actor.SkillBag.Find(item => item.SkillTableInfo.ID == skillID);
        if (null == info)
        {
            return;
        }
        CDInfo cdInfo = GameTable.CDTableAsset.Lookup(info.SkillTableInfo.CoolDown);
		if (null != cdInfo)
		{
			actor.CDControl.CDList.RemoveAll(item => item.CDID == cdInfo.ID);
		}
    }
    public bool IsSkillCDRunning(int skillID, Actor actor)
    {
        Actor.ActorSkillInfo info = actor.SkillBag.Find(item => item.SkillTableInfo.ID == skillID);
        if (null == info)
        {
            return false;
        }
        CDInfo cdInfo = GameTable.CDTableAsset.Lookup(info.SkillTableInfo.CoolDown);
		if (null != cdInfo)
		{
            if (null == actor.CDControl.CDList.Find(item => item.CDID == cdInfo.ID))
            {
                if (info.SkillTableInfo.PreCoolDown != 0)
                {
                    cdInfo = GameTable.CDTableAsset.Lookup(info.SkillTableInfo.PreCoolDown);
                    if (null == actor.CDControl.CDList.Find(item => item.CDID == cdInfo.ID))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
		}
		else
		{
			return false;
		}
    }
    public void AddSkillPreCD(int skillID, Actor actor)
    {
        Actor.ActorSkillInfo info = actor.SkillBag.Find(item => item.SkillTableInfo.ID == skillID);
        if (null == info || info.SkillTableInfo.PreCoolDown == 0)
        {
            return;
        }
        CDInfo cdInfo = GameTable.CDTableAsset.Lookup(info.SkillTableInfo.PreCoolDown);
        if (null != cdInfo)
        {
            actor.CDControl.AddCD(cdInfo.ID, actor.ID, skillID);
        }
    }
    public void RemoveSkillPreCD(int skillID, Actor actor)
    {
        Actor.ActorSkillInfo info = actor.SkillBag.Find(item => item.SkillTableInfo.ID == skillID);
        if (null == info || info.SkillTableInfo.PreCoolDown == 0)
        {
            return;
        }
        CDInfo cdInfo = GameTable.CDTableAsset.Lookup(info.SkillTableInfo.PreCoolDown);
        if (null != cdInfo)
        {
            actor.CDControl.CDList.RemoveAll(item => item.CDID == cdInfo.ID);
        }
    }
}
