using UnityEngine;

//变形
public class BuffEffectChangeMode : IBuffEffect
{
    public enum ENChangeModelType
    {
        enNone,
        enChangeModel,
    }
    public BuffEffectChangeMode()
        : base(ENBuff.ChangeModel)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectChangeMode();
    }
}