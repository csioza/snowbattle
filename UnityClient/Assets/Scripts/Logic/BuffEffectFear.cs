using UnityEngine;

//恐惧
class BuffEffectFear : IBuffEffect
{
    AIBase m_selfAI = null;
    public BuffEffectFear()
        : base(ENBuff.Fear)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectFear();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            Debug.LogWarning("OnGetBuffEffect failed! actor is not exist! actor id=" + TargetID.ToString());
            return;
        }
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (info == null)
        {
            Debug.LogWarning("OnGetBuffEffect failed! buff is not exist! buff id=" + BuffID.ToString());
            return;
        }
        if (actor.Type == ActorType.enNPC)
        {//此buff效果只对npc生效
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((ENFearType)item.ParamList[0])
                    {
                        case ENFearType.enEscape:
                            {//更改ai
                                m_selfAI = actor.SelfAI;
                                actor.SelfAI = new AINpc_Fear((ENFearType)item.ParamList[0], item.ParamList[1], SourceID);
                                actor.SelfAI.Owner = m_selfAI.Owner;
                            }
                            break;
                    }
                }
            }
        }
    }
    public override void OnRemoved(IResultControl control)
    {
        base.OnRemoved(control);
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            Debug.LogWarning("OnGetBuffEffect failed! actor is not exist! actor id=" + TargetID.ToString());
            return;
        }
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (info == null)
        {
            Debug.LogWarning("OnGetBuffEffect failed! buff is not exist! buff id=" + BuffID.ToString());
            return;
        }
        if (actor.Type == ActorType.enNPC)
        {//此buff效果只对npc生效
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    switch ((ENFearType)item.ParamList[0])
                    {
                        case ENFearType.enEscape:
                            {//更改ai
                                actor.SelfAI = m_selfAI;
                            }
                            break;
                    }
                }
            }
        }
    }
}