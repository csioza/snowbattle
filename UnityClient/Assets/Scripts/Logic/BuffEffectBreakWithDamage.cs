using UnityEngine;

//受击打断
public class BuffEffectBreakWithDamage : IBuffEffect
{
    public BuffEffectBreakWithDamage()
        : base(ENBuff.BreakWithDamage)
    {
    }
    static public IBuffEffect CreateNew()
    {
        return new BuffEffectBreakWithDamage();
    }
    public override void OnGetResult(IResult result, IResultControl control)
    {
        base.OnGetResult(result, control);
        if (result.ClassID == (int)ENResult.Damage)
        {
            //删除自身
            RemoveSelf();
        }
    }
}