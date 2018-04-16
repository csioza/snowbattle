using UnityEngine;

//技能沉默
public class BuffEffectSkillSilence : IBuffEffect
{
    public BuffEffectSkillSilence()
        : base(ENBuff.SkillSilence)
    {
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectSkillSilence();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        NotifySkillSilence(true);
    }
    public override void OnRemoved(IResultControl control)
    {
        base.OnRemoved(control);
        NotifySkillSilence(false);
    }
    void NotifySkillSilence(bool isSilence)
    {
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
        foreach (var item in info.BuffResultList)
        {
            if (item.ID == (int)ClassID)
            {
                actor.NotifySkillSilence((int)item.ParamList[0], (int)item.ParamList[1], isSilence);
            }
        }
    }
}