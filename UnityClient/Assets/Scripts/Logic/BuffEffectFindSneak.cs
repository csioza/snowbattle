using UnityEngine;

//反隐形（破除潜行）
class BuffEffectFindSneak : IBuffEffect
{
    public BuffEffectFindSneak()
        : base(ENBuff.FindSneak)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectFindSneak();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (info == null)
        {
            Debug.LogWarning("buff is not exist! buff id=" + BuffID.ToString());
            return;
        }
        FindSneak(true);
    }
    public override void OnRemoved(IResultControl control)
    {
        base.OnRemoved(control);
        FindSneak(false);
    }
    void FindSneak(bool isFind)
    {
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            Debug.LogWarning("actor is not exist! actor id=" + TargetID.ToString());
            return;
        }
        if (actor.Type != ActorType.enNPC)
        {
            Debug.LogWarning("can not find sneak, type is " + actor.Type);
            return;
        }
        (actor.SelfAI as AINpc).IsFindSneak = isFind;
    }
}