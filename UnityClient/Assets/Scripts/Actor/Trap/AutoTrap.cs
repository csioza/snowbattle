using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTrap : Trap
{
    //攻击目标
    public Actor mTargetActor = null;
    //攻击目标ID
    public int mTargetActorID = -1;

    public AutoTrap(int id, int staticID, CSItemGuid guid)
        : base(id, staticID, guid)
    {
        mTrapType = TrapType.enTouchType;
        mMinAttackTime = CurrentTableInfo.MinTouchTime;
        mMaxAttackCount = CurrentTableInfo.MaxTouchCount;

        SelfAI = new AINpcTrap(CurrentTableInfo.AIXmlName, CurrentTableInfo.AIXmlSubName);
        SelfAI.Owner = this;

        mAttackActionAnimList = new List<string>();
        for (int i = 0; i < CurrentTableInfo.AttackAnimList.Count; i++ )
        {
            mAttackActionAnimList.Add(CurrentTableInfo.AttackAnimList[i]);
        }

        mStandByAnimList = new List<string>();
        for (int i = 0; i < CurrentTableInfo.StandbyAnimList.Count; i++)
        {
            mStandByAnimList.Add(CurrentTableInfo.StandbyAnimList[i]);
        }
    }
    public override bool IsCanSelected()
    {
        return false;
    }
    //打开/关闭机关，机关变为可（不可）触发状态
}

