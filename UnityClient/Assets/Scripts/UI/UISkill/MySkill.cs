using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MySkill
{
#region Singleton
    static MySkill m_mySkill;
    public static MySkill Singleton
    {
        get
        {
            if (m_mySkill == null)
            {
                m_mySkill = new MySkill();
            }
            return m_mySkill;
        }
    }
#endregion

    public List<INotifier> m_notifierList;
    static Dictionary<int, SkillInst> m_mySkills = new Dictionary<int, SkillInst>();
    public SkillInst[] m_skillInst =  new SkillInst[30];
    public void AddNotifier(INotifier val)
    {
        for (int index = 0; index != m_notifierList.Count;index ++ )
        {
            if (m_notifierList[index] == val)
            {
                return;
            }
            m_notifierList.Add(val);
        }
    }
    public Dictionary<int, SkillInst> mySkills
    {
        get
        {
            return m_mySkills;
        }
    }
    public void AddRandomSkill()
    {
//        List<SkillInfo> list = GameTable.SkillTableAsset.LookupByFrofesgion(2);
//        for (int index = 0; index < list.Count;index ++ )
//        {
//            SkillInfo skillInfo = list[index];
//            if (skillInfo == null)
//            {
//                return;
//            }
//            //SkillInst inst = Lookup(skillInfo.ID);
////             if (inst != null)
////             {
//// 
////             }
////             SkillInst newSkill = new SkillInst();
////             newSkill.SkillInfo = skillInfo;
////             newSkill.SkillLevel = UnityEngine.Random.Range(0, 10);
////             newSkill.SkillAddLevel = 0;
////             m_mySkills.Add(newSkill.SkillInfo.ID, newSkill);
//        }
    }
    public void AddNewSkill(int skillID,int level/*SkillInst inst*/)
    {
        if (m_mySkills.ContainsKey(skillID))
        {
            m_mySkills[skillID].SkillLevel = level;
        }
        else
        {
            SkillInst inst = new SkillInst();
            inst.SkillInfo = GameTable.SkillTableAsset.Lookup(skillID);
            inst.SkillLevel = level;
            inst.SkillAddLevel = 0;
            m_mySkills.Add(inst.SkillInfo.ID, inst);
        }
    }

    public SkillInst Lookup(int skill_id)
    {
        SkillInst inst = null;
        m_mySkills.TryGetValue(skill_id, out inst);
        return inst;
    }

    public SkillInst FindSkillInst(int skill_id)
    {
        for (int i = 0; i < m_skillInst.Length;i++ )
        {
            SkillInst inst = m_skillInst[i];
            if (null == inst)
            {
                return null;
            }
            else if (inst.SkillInfo.ID == skill_id)
            {
                return inst;
            }
        }
        return null;
    }

    public int GetSkillLevel(int skill_id)
    {
        SkillInst inst = m_mySkills[skill_id];
        if (inst != null)
        {
            return inst.SkillLevel;
        }
        return 0;
    }
}
//技能实例.
public class SkillInst
{
    public SkillInfo SkillInfo { get; set; }    //SkillInfo指针
    public int SkillLevel;                      //技能等级
    public int SkillAddLevel;                   //技能附加等级
}
