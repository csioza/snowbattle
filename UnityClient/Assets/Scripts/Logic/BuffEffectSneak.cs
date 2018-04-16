using UnityEngine;

//潜行
class BuffEffectSneak : IBuffEffect
{
    public BuffEffectSneak()
        : base(ENBuff.Sneak)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectSneak();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            Debug.LogWarning("actor is not exist! actor id=" + TargetID.ToString());
            return;
        }
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (info == null)
        {
            Debug.LogWarning("buff is not exist! buff id=" + BuffID.ToString());
            return;
        }
        IsNeedResultPass = true;
        if (actor.Type == ActorType.enNPC)
        {//npc不能潜行
            Debug.LogWarning("npc can not sneak");
            return;
        }
        //开始潜行
        actor.StartSneak(BuffID);
    }
    public override void OnProduceResult(IResult result, IResultControl control)
    {
        base.OnProduceResult(result, control);
        if (result.ClassID == (int)ENResult.Skill)
        {
            //释放技能时， 取消潜行
            StopSneak();
        }
    }
    public override void OnRemoved(IResultControl control)
    {
        base.OnRemoved(control);
        //取消潜行
        StopSneak();
    }
    //取消潜行
    void StopSneak()
    {
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            Debug.LogWarning("actor is not exist! actor id=" + TargetID.ToString());
            return;
        }
        actor.StopSneak();
    }
}