using System;
using UnityEngine;

//召唤
public class ResultCall : IResult
{
    enum ENCallType
    {
        enNone,
        enCallFriend,
    }
    ActorType m_actorType;
    int m_actorID = 0;
    int m_actorCount = 0;
    float m_range = 0;
    float m_duration = 0;
    public ResultCall()
        : base((int)ENResult.Call)
    {
    }
    static public IResult CreateNew()
    {
        return new ResultCall();
    }
    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);
        if (param == null || param.Length == 0)
        {
            Debug.LogWarning("result call ResultExpr, param is error");
            return;
        }
        Actor self = ActorManager.Singleton.Lookup(SourceID);
        if (self == null)
        {
            Debug.LogWarning("actor is not exist, id is " + SourceID);
            return;
        }
        switch ((ENCallType)param[0])
        {
            case ENCallType.enCallFriend:
                {
                    if (param.Length < 5)
                    {
                        return;
                    }
                    if (self.Type == ActorType.enNPC)
                    {
                        m_actorType = ActorType.enNPC;
                    }
                    else
                    {
                        m_actorType = ActorType.enFollow;
                    }
                    m_actorID = (int)param[1];
                    m_actorCount = (int)param[2];
                    m_range = param[3];
                    m_duration = param[4];
                }
                break;
        }
    }
    public override void Exec(IResultControl control)
    {
        base.Exec(control);
        Actor self = ActorManager.Singleton.Lookup(SourceID);
        if (self == null)
        {
            Debug.LogWarning("actor is not exist, id is " + SourceID);
            return;
        }
        for (int i = 0; i < m_actorCount; ++i)
        {
            int actorID = ActorManager.Singleton.GetValidActorID(2000 + i);
            Actor actor = ActorManager.Singleton.CreatePureActor(m_actorType, actorID, CSItemGuid.Zero, m_actorID);
            actor.Props.SetProperty_Int32(ENProperty.islive, 1);
            if (actor.Type == ActorType.enNPC)
            {
                NPC npc = actor as NPC;
                npc.CurRoom = (self as NPC).CurRoom;
                npc.roomElement = new RoomElement();
                npc.IsActive = true;
            }
            //else
            //{
            //    Player p = actor as Player;
            //    p.InitPartnerSkill();
            //}
            actor.CreateNeedModels();
            Vector3 pos = GetValidPosition(actor.MainPos, m_range);
            actor.ForceMoveToPosition(pos);
            actor.StartDeadCD(m_duration);
        }
    }
    Vector3 GetValidPosition(Vector3 srcPos, float range)
    {
        Vector3 validPos;
        while (true)
        {
            float x = UnityEngine.Random.Range(srcPos.x - range, srcPos.x + range);
            float z = UnityEngine.Random.Range(srcPos.z - range, srcPos.z + range);
            validPos = new Vector3(x, srcPos.y, z);
            //if (validPos)//validPos在寻路中是有效的位置
            {
                break;
            }
        }
        return validPos;
    }
}