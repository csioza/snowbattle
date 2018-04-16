using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIComboAppraise : UIWindow
{
    UISprite m_comboNice = null;
    UISprite m_comboCool = null;
    UISprite m_comboGreat = null;
    UISprite m_comboBravo = null;
    UISprite m_comboPerfect = null;

    private float m_showStartTime = float.MaxValue;//开始显示的时间
    #region ComboAppraiseShowDuration //连击评价显示的持续时间
    private float m_comboAppraiseShowDuration = 0;
    private float ComboAppraiseShowDuration
    {
        get
        {
            if (m_comboAppraiseShowDuration == 0)
            {
                m_comboAppraiseShowDuration = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enComboEvaluationHideTime).FloatTypeValue;
            }
            return m_comboAppraiseShowDuration;
        }
        set { m_comboAppraiseShowDuration = value; }
    }
    #endregion

    static public UIComboAppraise GetInstance()
	{
        UIComboAppraise self = UIManager.Singleton.GetUIWithoutLoad<UIComboAppraise>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIComboAppraise>("UI/UIComboAppraise", UIManager.Anchor.TopRight);
		return self;
	}
	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enBattlePropsManager, OnPropertyChanged);
        m_comboNice = FindChildComponent<UISprite>("ComboNice");
        m_comboCool = FindChildComponent<UISprite>("ComboCool");
        m_comboGreat = FindChildComponent<UISprite>("ComboGreat");
        m_comboBravo = FindChildComponent<UISprite>("ComboBravo");
        m_comboPerfect = FindChildComponent<UISprite>("ComboPerfect");
	}
    //显示评价
    private void ShowEvaluation(int index)
    {
        m_comboNice.gameObject.SetActive(index == (int)ENWorldParamIndex.enComboNice ? true : false);
        m_comboCool.gameObject.SetActive(index == (int)ENWorldParamIndex.enComboCool ? true : false);
        m_comboGreat.gameObject.SetActive(index == (int)ENWorldParamIndex.enComboGreat ? true : false);
        m_comboBravo.gameObject.SetActive(index == (int)ENWorldParamIndex.enComboBravo ? true : false);
        m_comboPerfect.gameObject.SetActive(index == (int)ENWorldParamIndex.enComboPerfect ? true : false);
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)BattleArena.ENPropertyChanged.enComboAppraise)
        {
            ShowEvaluation((int)eventObj);
            ShowWindow();
            m_showStartTime = Time.time;
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Time.time - m_showStartTime > ComboAppraiseShowDuration)
        {
            m_showStartTime = float.MaxValue;
            HideWindow();
        }
    }
}
