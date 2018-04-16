using System;
using UnityEngine;

class UISkillCombo : UIWindow
{
    UILabel m_labelNumber = null;
    Animation m_anim = null;
    int m_count = 0;
    //
    float m_hideStartTime = float.MaxValue;
    float m_hideDuration = 0;
    float HideDuration
    {
        get
        {
            if (m_hideDuration == 0)
            {
                m_hideDuration = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSkillComboHideTime).FloatTypeValue;
            }
            return m_hideDuration;
        }
    }
    static public UISkillCombo Singleton
    {
        get
        {
            UISkillCombo self = UIManager.Singleton.GetUIWithoutLoad<UISkillCombo>();
            if (self == null)
            {
                self = UIManager.Singleton.LoadUI<UISkillCombo>("UI/UISkillCombo", UIManager.Anchor.Left);
            }
            return self;
        }
    }

    static public UISkillCombo GetInstance()
    {
        UISkillCombo self = UIManager.Singleton.GetUIWithoutLoad<UISkillCombo>();
        if (self == null)
        {
            self = UIManager.Singleton.LoadUI<UISkillCombo>("UI/UISkillCombo", UIManager.Anchor.Left);
        }
        return self;
    }
    public override void OnInit()
    {
        base.OnInit();
        AddPropChangedNotify((int)MVCPropertyID.enBattlePropsManager, OnPropertyChanged);

        m_labelNumber = FindChildComponent<UILabel>("ComboNumber");
        m_anim = WindowRoot.GetComponent<Animation>();
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)BattleArena.ENPropertyChanged.enSkillCombo)
        {
            if ((bool)eventObj)
            {
                if (m_count == BattleArena.Singleton.SkillCombo.SkillComboNumber)
                {
                    return;
                }
                m_count = BattleArena.Singleton.SkillCombo.SkillComboNumber;

                ShowWindow();

                m_labelNumber.text = m_count.ToString();
                m_anim.Play("ComboHitSize", PlayMode.StopAll);
                m_hideStartTime = float.MaxValue;
            }
            else
            {
                m_count = 0;
                m_hideStartTime = Time.time;
            }
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Time.time - m_hideStartTime > HideDuration)
        {
            if (IsVisiable())
            {
                HideWindow();
                m_hideStartTime = float.MaxValue;
            }
        }
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
