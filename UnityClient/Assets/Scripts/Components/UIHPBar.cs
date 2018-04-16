using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHPBar : MonoBehaviour
{
    public UISlider m_hpSlider = null;
    public UISlider m_flashSlider = null;

    float m_hp = 1.0f;

    //时间模式下EnChangeType.enTime
    public float m_hpDuration = 0.1f;
    public float m_hpMaxSpeed = 2;

    public float m_flashDuration = 0.1f;
    public float m_flashMaxSpeed = 2;

    //速度模式下 EnChangeType.enSpeed
    public float m_hpSpeed = 0.5f;
    public float m_flashSpeed = 0.1f;
    //是否播放消失动画
    public bool m_isPlayHideAnim = true;
    public Animation m_anim = null;

	public bool m_actorIsDead = false;
    //public int m_ownerID = 0;
    enum EnChangeType
    {
        enTime,
        enSpeed,
    }
    EnChangeType m_type = EnChangeType.enSpeed;

    public float HP
    {
        get { return m_hp; }
        set
        {
            if (m_hp == value)
            {
                return;
            }
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            if (value == 1 && m_hp <= 0.0f)
            {//由0到1，瞬间回满血
                SetHPWithoutFlash(value);
                m_type = EnChangeType.enSpeed;
            }
            else
            {
                if (value == 0)
                {
                    m_type = EnChangeType.enTime;
                }
                m_hp = value;
                Flash();
            }
        }
    }
    public void SetHPWithoutFlash(float hp)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        m_hp = hp;
        m_hpSlider.value = m_hp;
        if (m_flashSlider != null)
        {
            m_flashSlider.value = m_hp;
        }
    }

	// Use this for initialization
	void Start ()
    {
	}

    float m_flashChangeTime = 0;
    float m_hpChangeTime = 0;
    float m_flashChangeSpeed = 0;
    float m_hpChangeSpeed = 0;
    //flash judge
    float m_flashJudgeStartTime = 0;
    float m_flashJudgeTime = 0;
    //flash变速的最小值
    float m_flashChangeMinSpeed = float.MinValue;
	void Update()
    {
        if (m_flashJudgeTime == 0)
        {
            m_flashJudgeTime = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enHpBarFlashJudgeTime).FloatTypeValue;
        }

        if (m_flashSlider != null)
        {
            if (Time.time - m_flashJudgeStartTime > m_flashJudgeTime)
            {
                if (m_flashChangeTime > 0.0f)
                {
                    m_flashChangeTime -= Time.deltaTime;
                    if (m_flashChangeTime < 0.0f)
                    {
                        m_flashSlider.value = m_hp;
                        m_flashSlider.gameObject.SetActive(false);
                    }
                    else
                    {
                        m_flashSlider.value += m_flashChangeSpeed * Time.deltaTime;
                    }
                }
            }
        }
        if (m_hpChangeTime > 0.0f)
        {
            m_hpChangeTime -= Time.deltaTime;
            if (m_hpChangeTime < 0.0f)
            {
                m_hpSlider.value = m_hp;
            }
            else
            {
                m_hpSlider.value += m_hpChangeSpeed * Time.deltaTime;
            }
        }
		if (m_actorIsDead && m_hpChangeTime < 0.0f && m_flashChangeTime <= 0.0f)
        {
            if (m_isPlayHideAnim)
            {
                m_playType = ENPlayType.enHide;
                m_animStartTime = float.MaxValue;
                m_animLength = float.MaxValue;
                //Debug.LogWarning("time is over, hide" + ", id:" + m_ownerID);
            }
            m_flashChangeMinSpeed = float.MinValue;
        }
        if (m_anim != null)
        {
            string animName = "";
            switch (m_playType)
            {
                case ENPlayType.enShow:
                    animName = "ui-bloodbarShow-00";
                    break;
                case ENPlayType.enHide:
                    animName = "ui-bloodbarFade-00";
                    break;
            }
            if (Time.time - m_animStartTime >= m_animLength)
            {
                m_playType = ENPlayType.enNone;
            }
            else
            {
                if (!string.IsNullOrEmpty(animName) && !m_anim.IsPlaying(animName))
                {
                    AnimationClip clip = m_anim.GetClip(animName);
                    m_animLength = clip.length;
                    m_animStartTime = Time.time;
                    m_anim[animName].wrapMode = WrapMode.ClampForever;

                    m_anim.Stop();
                    m_anim.Play(animName, PlayMode.StopAll);
                    //Debug.LogWarning("animation name:" + animName + ", id:" + m_ownerID);
                }
            }
        }
    }
    enum ENPlayType
    {
        enNone,
        enShow,
        enHide,
    }
    ENPlayType m_playType = ENPlayType.enNone;
    public void PlayShowAnim()
    {
        m_playType = ENPlayType.enShow;
        m_animStartTime = float.MaxValue;
        m_animLength = float.MaxValue;
    }
    float m_animStartTime = float.MaxValue;
    float m_animLength = float.MaxValue;
    public void PlayHideAnim()
    {
        m_playType = ENPlayType.enHide;
        m_animStartTime = float.MaxValue;
        m_animLength = float.MaxValue;
    }
    void Flash()
    {
        if (m_type == EnChangeType.enTime)
        {
            if (m_flashDuration == 0 || m_hpDuration == 0)
            {
                Debug.LogWarning("ui hp bar, flash duration is " + m_flashDuration.ToString() + ", hp duration is " + m_hpDuration.ToString());
                return;
            }

            if (m_flashSlider != null)
            {
                m_flashChangeSpeed = (m_hp - m_flashSlider.value) / m_flashDuration;
                if (Mathf.Abs(m_flashChangeSpeed) > m_flashMaxSpeed)
                {
                    if (m_hp > m_flashSlider.value)
                    {
                        m_flashChangeSpeed = m_flashMaxSpeed;
                    }
                    else
                    {
                        m_flashChangeSpeed = -m_flashMaxSpeed;
                    }
                }
            }
            m_hpChangeSpeed = (m_hp - m_hpSlider.value) / m_hpDuration;
            if (Mathf.Abs(m_hpChangeSpeed) > m_hpMaxSpeed)
            {
                if (m_hp > m_hpSlider.value)
                {
                    m_hpChangeSpeed = m_hpMaxSpeed;
                }
                else
                {
                    m_hpChangeSpeed = -m_hpMaxSpeed;
                }
            }
            if (m_hpChangeSpeed != 0)
            {
                m_hpChangeTime = m_hpDuration;
            }
            if (m_flashSlider != null)
            {
                if (m_flashChangeSpeed != 0)
                {
                    m_flashChangeTime = m_flashDuration;
                    m_flashSlider.gameObject.SetActive(true);
                    m_flashSlider.GetComponent<Animation>().Play(m_flashSlider.GetComponent<Animation>().clip.name);
                }
            }
        }
        else
        {
            if (m_flashSpeed == 0 || m_hpSpeed == 0)
            {
                Debug.LogWarning("ui hp bar, flash speed is " + m_flashSpeed.ToString() + ", hp speed is " + m_hpSpeed.ToString());
                return;
            }

            if (m_flashSlider != null)
            {
                float d = m_flashSlider.value - m_hpSlider.value;
                if (m_hp - m_flashSlider.value > 0)
                {
                    m_flashChangeSpeed = m_flashSpeed * ((int)(d / 0.1f) * 0.5f + 1);
                }
                else
                {
                    m_flashChangeSpeed = -m_flashSpeed * ((int)(d / 0.1f) * 0.5f + 1);
                    if (m_flashChangeMinSpeed > m_flashChangeSpeed)
                    {
                        m_flashChangeSpeed = m_flashChangeMinSpeed;
                    }
                }
                m_flashChangeTime = (m_hp - m_flashSlider.value) / m_flashChangeSpeed;
                m_flashChangeTime = Mathf.Abs(m_flashChangeTime);
            }

            m_hpChangeTime = (m_hp - m_hpSlider.value) / m_hpSpeed;
            m_hpChangeTime = Mathf.Abs(m_hpChangeTime);
            m_hpChangeSpeed = (m_hp - m_hpSlider.value) / m_hpChangeTime;

            if (m_hpChangeTime != 0)
            {
                if (m_flashSlider != null)
                {
                    m_flashSlider.gameObject.SetActive(true);
                    m_flashSlider.GetComponent<Animation>().Play(m_flashSlider.GetComponent<Animation>().clip.name);
                }
            }
        }
        if (m_flashChangeSpeed < 0)
        {//减血才判定，加血不判定
            m_flashJudgeStartTime = Time.time;
        }
    }
}
