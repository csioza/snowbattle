using System;
using System.Collections;
using UnityEngine;

class BuffEffectNoDeadAddBuff : IBuffEffect
{
    float m_reliveStartTime = 0;
    float m_reliveDuration = 0;
    public BuffEffectNoDeadAddBuff()
        : base(ENBuff.NoDeadAddBuff)
    {
        m_reliveStartTime = 0;
        m_reliveDuration = 0;
    }
    public static IBuffEffect CreateNew()
    {
        return new BuffEffectNoDeadAddBuff();
    }
    public override void OnGetBuffEffect()
    {
        base.OnGetBuffEffect();
        BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
        if (info == null)
        {
            Debug.LogWarning("buff is not exist, buff id is " + BuffID);
            return;
        }
        IsNeedResultPass = true;
    }
    public override void OnGetResult(IResult result, IResultControl control)
    {
        base.OnGetResult(result, control);
        if (result.ClassID == (int)ENResult.Dead)
        {//死亡目标是自己，所以在OnGetResult
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {
                    //设置死亡result的复活
                    ResultDead rd = result as ResultDead;
                    rd.m_isRelive = true;

                    IsNeedTick = true;
                    m_reliveDuration = item.ParamList[0];//第一个参数为复活的时间
                    m_reliveStartTime = Time.time;
                }
            }
        }
    }
    public override void Tick(IResultControl control, float dt)
    {
        base.Tick(control, dt);
        if (Time.time - m_reliveStartTime > m_reliveDuration)
        {
            //复活result
            IResult relive = BattleFactory.Singleton.CreateResult(ENResult.Relive, TargetID, TargetID);
            if (relive != null)
            {
                relive.ResultExpr(null);
                BattleFactory.Singleton.DispatchResult(relive);
            }
            //复活之后添加的buff
            BuffInfo info = GameTable.BuffTableAsset.Lookup(BuffID);
            foreach (var item in info.BuffResultList)
            {
                if (item.ID == (int)ClassID)
                {//第二个参数为添加buff的id
                    IResult addBuff = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, TargetID, TargetID,0,0,new float[1] { item.ParamList[1] });
                    if (addBuff != null)
                    {
                        addBuff.ResultExpr(new float[1] { item.ParamList[1] });
                        BattleFactory.Singleton.DispatchResult(addBuff);
                    }
                }
            }
            //移除自身
            IResult removeBuff = BattleFactory.Singleton.CreateResult(ENResult.RemoveBuff, TargetID, TargetID, 0, 0, new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, BuffID });
            if (removeBuff != null)
            {
                removeBuff.ResultExpr(new float[2] { (float)ResultRemoveBuff.ENRemoveBuffType.enBuffID, BuffID });
                BattleFactory.Singleton.DispatchResult(removeBuff);
            }
            //只复活一次
            IsNeedTick = false;
        }
    }
}