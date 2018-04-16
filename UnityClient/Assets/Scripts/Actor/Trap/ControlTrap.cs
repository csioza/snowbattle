using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlTrap : Trap
{
    //攻击目标
    public Actor mTargetActor = null;

    public ControlTrap(int id, int staticID, CSItemGuid guid)
        : base(id, staticID, guid)
    {
        mTrapType = TrapType.enControl;
        mCloseTrapAnim = CurrentTableInfo.mCloseTrapAnim;
        mDisableAnim = CurrentTableInfo.mDisableTrapAnim;
        mOpenTrapAnim = CurrentTableInfo.mOpenTrapAnim;
        mLocalTrapAnim = CurrentTableInfo.mLockTrapAnim;
    }
    public override bool IsCanSelected()
    {
        return true;
    }

    //打开/关闭机关，机关变为可（不可）触发状态
//     public override void SetTrapAble(bool val)
//     {
//         string strAnim = val ? mOpenTrapAnim : mLocalTrapAnim;
//         PlayAnimation(strAnim);
//         m_trapAble = val; 
//     }
//     //激活/（不激活）机关
//     public override void SetTrapActive(bool val)
//     {
//         if (m_trapAble)
//         {
//             string strAnim = val ? mAbleTrapAnim : mUnableTrapAnim;
//             PlayAnimation(strAnim);
//             m_trapActivate = val;
//         }
//     }
    public override void SetTrapState(TrapState state)
    {
        if (state == TrapState.enOpen)
        {
            PlayAnimation(mOpenTrapAnim);
        }
        else if (state == TrapState.enClose)
        {
            PlayAnimation(mCloseTrapAnim);
        }
        else if (state == TrapState.enLock)
        {
            PlayAnimation(mLocalTrapAnim);
        }
        else if (state == TrapState.enDisable)
        {
            PlayAnimation(mDisableAnim);
        }
        mTrapState = state;
    }
    public override void FixedUpdate()
    {

    }
    //操作机关
    public void OperateOpenAnim()
    {

        if (mTrapState == TrapState.enOpen)
        {
            if (m_IsNeedKey)
            {
                return;
            }
            SetTrapState(TrapState.enClose);
        }
        else if (mTrapState == TrapState.enClose)
        {
            if (m_IsNeedKey)
            {
                if (BattleArena.Singleton.KeyCount > 0)
                {
                    BattleArena.Singleton.KeyCount -= 1;
                }
                else
                {
                    return;
                }
//                 if (BattleArena.Singleton.GetKeyCountByID(m_needKeyId) > 0)
//                 {
//                     BattleArena.Singleton.SetKeyCount(m_needKeyId, -1);
//                 }
//                 else
//                 {
//                     return;
//                 }
            }
            SetTrapState(TrapState.enOpen);
        }
       
    }
}