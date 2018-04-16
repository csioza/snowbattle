using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NGE.Network;

public class ResultRemoveBuff : IResult
{
    public enum ENRemoveBuffType
    {
        enNone,
        enBuffID,
        enBuffType,
    }
    List<float> m_buffIDList = new List<float>();
    public ResultRemoveBuff()
 		:base((int)ENResult.RemoveBuff)
    {
        
    }
    public static IResult CreateNew()
    {
        return new ResultRemoveBuff();
    }

    public override void Deserialize(PacketReader stream)
    {
        base.Deserialize(stream);
        m_buffIDList.Clear();
        int size        = stream.ReadInt32();
//        float[] param   = new float[size];

        for (int i = 0; i < size; i++)
        {
           int buffId = stream.ReadInt32();
           m_buffIDList.Add(buffId);
           
        }
    }

    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);
        if (param != null)
        {
            Actor actor = ActorManager.Singleton.Lookup(TargetID);
            if (null == actor)
            {
                return;
            }
            //第一个参数为删除buff的类型ENRemoveBuffType
            switch ((ENRemoveBuffType)param[0])
            {
                case ENRemoveBuffType.enBuffID:
                    {
                        for (int i = 1; i < param.Length; ++i)
                        {
                            m_buffIDList.Add(param[i]);
                        }
                    }
                    break;
                case ENRemoveBuffType.enBuffType:
                    {
                        foreach (var item in actor.MyBuffControl.BuffList)
                        {
                            BuffInfo info = GameTable.BuffTableAsset.Lookup(item.BuffID);
                            if (info == null)
                            {
                                continue;
                            }
                            if (info.BuffType != (int)param[1])
                            {
                                continue;
                            }
                            m_buffIDList.Add(info.ID);
                        }
                    }
                    break;
            }
        }
    }
    public override void Exec(IResultControl control)
    {
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            return;
        }
        foreach (var item in m_buffIDList)
        {
            if (item == 0)
            {
                continue;
            }
            actor.MyBuffControl.RemoveBuff((int)item, control);
        }
    }

    public override void ResultServerExec(IResultControl control)
    {
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            return;
        }
        foreach (var item in m_buffIDList)
        {
            if (item == 0)
            {
                continue;
            }
            actor.MyBuffControl.RemoveBuff((int)item, control);
        }
    }
}
