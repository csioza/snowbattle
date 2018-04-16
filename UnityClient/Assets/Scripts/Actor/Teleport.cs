using System;
using UnityEngine;

//瞬移
public class Teleport
{
    public enum ENTeleportType
    {
        enNone,
        enTimes,//次数
    }
    ENTeleportType m_teleportType { get; set; }
    int m_times { get; set; }
    public Teleport()
    {
        ;
    }
    public Teleport(int times)
    {
        m_teleportType = ENTeleportType.enTimes;
        m_times = times;
    }
    //移除
    public void Remove(ENTeleportType type)
    {
        if (m_teleportType == type)
        {
            m_teleportType = ENTeleportType.enNone;
        }
    }
    //瞬移一次
    public void TeleportOnce(Actor actor, Vector3 targetPos)
    {
        actor.PlayEffectInPosition(BattleArena.Singleton.TeleportHideEffect, actor.RealPos);
        actor.MainPos = GetValidPosition(actor.RealPos, targetPos);
        actor.PlayEffectInPosition(BattleArena.Singleton.TeleportShowEffect, actor.MainPos);
        switch (m_teleportType)
        {
            case ENTeleportType.enTimes:
                {
                    --m_times;
                }
                break;
        }
    }
    Vector3 GetValidPosition(Vector3 sourcePos, Vector3 targetPos)
    {
        Vector3 validPos = targetPos;
        //方向
        Vector3 d = sourcePos - targetPos;
        d.y = 0;
        d.Normalize();

        while (true)
        {
            //if (validPos)//validPos在寻路中是有效的位置
            {
                break;
            }
//            validPos = validPos + d * 0.1f;
        }
        return validPos;
    }
    //是否可以瞬移
    public bool IsCanTeleport()
    {
        switch (m_teleportType)
        {
            case ENTeleportType.enTimes:
                {
                    return m_times > 0;
                }
//                break;
        }
        return false;
    }
}