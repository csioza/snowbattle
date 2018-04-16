using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion
//按下状态
public class HoldDownAction : ActorAction
{
    //private float m_time;
    //public float m_startTime;
    //public float m_endTime = 0.0f;
    public override ENType GetActionType() { return ENType.enHoldDownAction; }
    public static ENType SGetActionType() { return ENType.enHoldDownAction; }
    //public float m_skillbarNumMin;
    //WorldParamInfo WorldParamList;
    //private bool IsFireSkill;
    //EndTimeDisplay timewidget;
    //private bool IsDisplayCD;
    public override void OnEnter()
    {
        //IsFireSkill = false;
        //IsDisplayCD = false;
        //WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillbarSkillFirstPartTime);
        //m_skillbarNumMin = WorldParamList.FloatTypeValue;
        //CurrentActor.PlayAnimation("w1-xuli-00");
        //RefreshActionRef();
    }
    public override void OnInterupt()
    {
//        CurrentActor.PlayAnimation("standby");
        //m_endTime = 0.0f;
        //m_startTime = 0.0f;
        //IsFireSkill = false;
        //if (IsDisplayCD && timewidget!= null)
        //{
        //    timewidget.Release();
        //}
        //IsDisplayCD = false;
    }
    public override void OnExit()
    {
        //m_endTime = 0.0f;
        //m_startTime = 0.0f;
        //IsFireSkill = false;
        //if (IsDisplayCD && timewidget != null)
        //{
        //    timewidget.Release();
        //}
        //IsDisplayCD = false;
    }
    public override bool OnUpdate()
    {
        //if (m_endTime > 0.0f&&m_endTime < m_skillbarNumMin)
        //{
        //    return true;
        //}
        //else if (m_endTime >= m_skillbarNumMin)
        //{
        //    m_startTime = m_time;
        //    WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillbarSkillCountDown);
        //    if (Time.time - m_startTime > WorldParamList.FloatTypeValue)
        //    {
        //        IsEnable = false;
        //        return true;
        //    }
        //}
        //return IsFireSkill;
        return true;
    }
    public void DisplayCountDown(int Num)
    {
//         m_SkillbarNum = Num;
//         m_time = Time.time;
//         IsDisplayCD = true;
//         Actor actor = ActorManager.Singleton.MainActor;
//         timewidget = MainGame.Singleton.CurrentWidgets.AddWidget<EndTimeDisplay>();
//         WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillbarSkillCountDown);
//         timewidget.EndTime = WorldParamList.IntTypeValue;
//         timewidget.SetValue(timewidget.EndTime);
//         timewidget.AttachObj(actor.CenterPart);
    }
    //Actor actor = ActorManager.Singleton.MainActor;
    //public int m_SkillbarNum = 0;
    //public int m_SkillbarNumMin = 0;
    public void FireSkillbarSkill(Vector3 finalDirection)
    {
//         WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillbarPartNum);
//         int m_SkillbarNumMax = WorldParamList.IntTypeValue;
//          WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillbarSkillType);
//          int m_SkillbarSkillType = WorldParamList.IntTypeValue;
//         int actorvocationid = actor.Props.GetProperty_Int32(ENProperty.vocationid);
//         List<SkillInfo> skilllist;
        //skilllist = GameTable.SkillTableAsset.LookupByFrofesgion(actorvocationid);
        //int m_SkillID = 0;
        //foreach (SkillInfo infopair in skilllist)
        //{
        //    if (infopair.Type == m_SkillbarSkillType)
        //    {
        //        m_SkillID = infopair.ID;
        //        break;
        //    }
        //}
        //actor.FireSkill(m_SkillID, finalDirection.normalized, m_SkillbarNum);
        //actor.ActionControl.AddAction(ActorAction.ENType.enUndownAction) as UndownAction;
        //UIUniqueSkillbar m_UniqueSkillbar = UIManager.Singleton.GetUI<UIUniqueSkillbar>();
        //m_UniqueSkillbar.SetSkillID(m_SkillID);
        //IsFireSkill = true;
    }
}