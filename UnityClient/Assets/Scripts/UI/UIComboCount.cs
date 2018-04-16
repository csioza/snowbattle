using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIComboCount : UIWindow
{
    int m_count = 0;
    UILabel m_comboNum = null;
	UILabel m_comboBackNum = null;
    UILabel m_modifyNum = null;
    UISprite m_percent = null;

    Vector3 m_localPos;
    float m_lastTime = 0.0f;
    string m_strNumber = "";
    int m_lastCount = 0;

    private float m_showStartTime = float.MaxValue;//开始显示的时间
    #region ComboNumberShowDuration //连击数字显示的持续时间
    private float m_comboNumberShowDuration = 0;
    private float ComboNumberShowDuration
    {
        get
        {
            if (m_comboNumberShowDuration == 0)
            {
                m_comboNumberShowDuration = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enComboCountHideTime).FloatTypeValue;
            }
            return m_comboNumberShowDuration;
        }
        set { m_comboNumberShowDuration = value; }
    }
    #endregion
    #region ComboNumberOffset //连击数字显示的偏移
    private float m_comboNumberOffset = 0;
    private float ComboNumberOffset
    {
        get
        {
            if (m_comboNumberOffset == 0)
            {
                m_comboNumberOffset = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enComboCountOffset).FloatTypeValue;
            }
            return m_comboNumberOffset;
        }
        set { m_comboNumberOffset = value; }
    }
    #endregion

    static public UIComboCount GetInstance()
	{
        UIComboCount self = UIManager.Singleton.GetUIWithoutLoad<UIComboCount>();
		if (self != null)
		{
			return self;
		}
        self = UIManager.Singleton.LoadUI<UIComboCount>("UI/UIComboCount", UIManager.Anchor.TopRight);
		return self;
	}
    public UIComboCount()
    {
        IsAutoMapJoystick = false;
    }
	public override void OnInit ()
	{
		base.OnInit ();
        AddPropChangedNotify((int)MVCPropertyID.enBattlePropsManager, OnPropertyChanged);
        m_comboNum = FindChildComponent<UILabel>("ComboNumber");
		m_comboBackNum = FindChildComponent<UILabel>("ComboBackNumber");
        m_modifyNum = FindChildComponent<UILabel>("ModifyNumber");
        m_percent = FindChildComponent<UISprite>("Percent");

        m_localPos = m_comboNum.cachedTransform.localPosition;
		m_strNumber = "ComboHitSize";

        AnimationState state = this.WindowRoot.GetComponent<Animation>()["ComboHitSize"];
        m_comboAnimationLength = state.length;
    }
    private float m_comboAnimationSpeed = 1.0f;// combo播放动画的速度
    private float m_comboAnimationLength = 0.44f;
    private int m_notChangeSpeedIndex = 0;     // 同一时间内的许多combo 所不许重置速度的 个数 可叠加
    private int m_curNotChangeSpeedIndex = 0; // 当前的个数
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        if (eventType == (int)BattleArena.ENPropertyChanged.enComboCount)
        {
            float now = Time.time;
           
            // 上次连击时间与此次 间隔时间满足条件 则调整动画速度
            if (now - m_showStartTime < m_comboAnimationLength && now >= m_showStartTime && m_count != 2)
            {
                m_comboAnimationSpeed = 15f;

                if (now == m_showStartTime)
                {
                    m_notChangeSpeedIndex++;
                }
            }
            else
            {
                m_comboAnimationSpeed = 1f;
            }

            m_showStartTime = now;

            AnimationState state = this.WindowRoot.GetComponent<Animation>()["ComboHitSize"];
            state.speed = m_comboAnimationSpeed;
            state = this.WindowRoot.GetComponent<Animation>()["ComboHitSize3"];
            state.speed = m_comboAnimationSpeed;
            state = this.WindowRoot.GetComponent<Animation>()["ComboHitSize2"];
            state.speed = m_comboAnimationSpeed;

            m_count = BattleArena.Singleton.Combo.TotalComboNumber;
            m_tick = true;
            m_isNotify = true;
        }
    }
    float m_comboScaleTime = 0;
    //每次通知
    bool m_isNotify = false;
    //伤害修正
    float m_curDamageModify = 0;
    override public void OnUpdate()
    {
        float now = Time.time;
        if (BattleArena.Singleton.Combo.TotalComboNumber == 0 && now - m_showStartTime > ComboNumberShowDuration)
        {
            if (WindowRoot.activeSelf)
            {
                HideWindow();
            }
            m_tick = false;
            m_lastCount = 0;
            m_notChangeSpeedIndex = 0;
            m_curNotChangeSpeedIndex = 0;
            m_curDamageModify = 0;
            m_modifyNum.gameObject.SetActive(false);
            m_percent.gameObject.SetActive(false);
            return;
        }
        if (m_lastCount > m_count)
        {
            if (m_isNotify)
            {
                m_lastCount = 0;
            }
            else
            {
                return;
            }
        }
        else
        {
            m_isNotify = false;
        }

        if (this.WindowRoot.GetComponent<Animation>().isPlaying)
        {
            return;
        }

        if (m_lastCount == 2)
        {
            m_comboNum.cachedTransform.localPosition = m_localPos;
            m_comboBackNum.cachedTransform.localPosition = m_localPos;
        }
        else if (m_lastCount == 10 || m_lastCount == 100 || m_lastCount == 1000 || m_lastCount == 10000)
        {
            Vector3 temp = m_comboNum.cachedTransform.localPosition;
            temp.x -= ComboNumberOffset;
            m_comboNum.cachedTransform.localPosition = temp;
            m_comboBackNum.cachedTransform.localPosition = temp;
        }

        //if (m_count == m_lastCount + 1)
        {
            if (m_comboScaleTime == 0)
            {
                WorldParamInfo info = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enComboScaleTime);
                if (info != null)
                {
                    m_comboScaleTime = info.FloatTypeValue;
                }
            }
            if (now - m_lastTime <= m_comboScaleTime)
            {
                if (m_strNumber == "ComboHitSize")
                {
                    m_strNumber = "ComboHitSize2";

                }
                else if (m_strNumber == "ComboHitSize2")
                {
                    m_strNumber = "ComboHitSize3";

                }
                else if (m_strNumber == "ComboHitSize3")
                {
                    m_strNumber = "ComboHitSize3";

                }
            }
        }

        if (m_lastCount>=2)
        {
            ShowWindow();

            m_comboBackNum.text = m_lastCount.ToString();
            m_comboNum.text     = m_lastCount.ToString();
            m_lastTime          = Time.time;
            //WindowRoot.animation.Stop();
            WindowRoot.GetComponent<Animation>().Play(m_strNumber, PlayMode.StopSameLayer);

            m_modifyNum.gameObject.SetActive(false);
            m_percent.gameObject.SetActive(false);
            if (BattleArena.Singleton.Combo.DamageModifyList.ContainsKey(m_lastCount))
            {
                float modify = BattleArena.Singleton.Combo.DamageModifyList[m_lastCount];
                if (modify != 0)
                {
                    m_modifyNum.text = modify.ToString();
                    m_modifyNum.gameObject.SetActive(true);
                    m_percent.gameObject.SetActive(true);
                    if (modify != m_curDamageModify)
                    {
                        m_curDamageModify = modify;
                        m_modifyNum.gameObject.GetComponent<Animation>().Play("comboDamageincreaseNumberPop", PlayMode.StopSameLayer);
                    }
                }
            }
        }
        // 同一时间 许多COMBO时 则不重置速度
        if (m_curNotChangeSpeedIndex < m_notChangeSpeedIndex)
        {
            m_curNotChangeSpeedIndex++;
        }
        else
        {// 重置正常速率
            m_comboAnimationSpeed = 1f;
            AnimationState state = this.WindowRoot.GetComponent<Animation>()["ComboHitSize"];
            state.speed = m_comboAnimationSpeed;
            state = this.WindowRoot.GetComponent<Animation>()["ComboHitSize3"];
            state.speed = m_comboAnimationSpeed;
            state = this.WindowRoot.GetComponent<Animation>()["ComboHitSize2"];
            state.speed = m_comboAnimationSpeed;
         }
        m_lastCount++;
     }

}
