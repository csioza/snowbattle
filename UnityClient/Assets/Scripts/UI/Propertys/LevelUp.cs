using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class SkillLevelUpInfo
{
    public int m_skillId    = 0;
    public int m_skillLevel = 0;
}
// 升级
public class LevelUp : IPropertyObject
{
    public enum LevelState
    {
        enNone      = 0,
        enSummary   =1,
        enCardUpdate,
    }
    public int m_oldLevel   = 0;

    public LevelState m_curState = LevelState.enNone;

    // 升级的 技能ID列表
    public List<SkillLevelUpInfo> m_skillList = new List<SkillLevelUpInfo>(); 

    public enum ENPropertyChanged
    {
        enShow,
        enHide,
        enShowSkill,
    }

    public LevelUp()
    {
        SetPropertyObjectID((int)MVCPropertyID.enLeveUp);

        m_oldLevel = User.Singleton.GetLevel();
    }

    #region Singleton
    static LevelUp m_singleton;
    static public LevelUp Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new LevelUp();
            }
            return m_singleton;
        }
    }
    #endregion

   public void Show()
    {
        NotifyChanged((int)ENPropertyChanged.enShow, null);
    }

    // 初始化 升级数据
    public void InitData()
   {
       m_oldLevel = User.Singleton.GetLevel();
   }

    public void ShowSkill()
    {
        // 如果没有技能升级
        if ( m_skillList.Count == 0 )
        {
            return;
        }

        NotifyChanged((int)ENPropertyChanged.enShowSkill, null);
    }


    public void ClearSkillList()
    {
        m_skillList.Clear();
    }
}
