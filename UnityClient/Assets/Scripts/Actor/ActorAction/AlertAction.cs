using System;
using System.Collections.Generic;
using UnityEngine;

public class AlertAction : ActorAction
{
    public override ENType GetActionType() { return ENType.enAlertAction; }
    public static ENType SGetActionType() { return ENType.enAlertAction; }
    
    private bool m_isShowDoubleWarning = false;
    private float m_duration = 0.0f;
    private float m_startTime = 0.0f;
    private NPC Self { get { return CurrentActor as NPC; } set { CurrentActor = value; } }
    public override void OnEnter()
    {
        m_startTime = Time.time;
        m_duration = Self.ShowHeadWarnTip();
		if (Self.CurRoom != null)
		{
			Self.CurRoom.EnterBattleState();
		}
        RefreshActionRef();
    }
    public override void OnInterupt()
    {
        OnExit();
    }
    public override void OnExit()
    {
        Self.HideWarnTip();
    }
    public override bool OnUpdate()
    {
        if (Time.time - m_startTime > m_duration)
        {
            if (!m_isShowDoubleWarning)
            {//改变头顶叹号颜色
                Self.ChangeHeadWarnTip();
            }
            return m_isShowDoubleWarning;
        }
        return false;
    }
    public override void Reset()
    {
        base.Reset();
        m_isShowDoubleWarning = false;
    }
    public void Refresh()
    {
        if (!m_isShowDoubleWarning)
        {
            m_isShowDoubleWarning = true;
            m_startTime = Time.time;
            m_duration = Self.ShowDoubleWarnTip();
        }
    }
}

