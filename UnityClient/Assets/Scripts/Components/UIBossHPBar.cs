using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBossHPBar : MonoBehaviour
{
    public UISlider m_sliderHP = null;
    public UISlider m_sliderFlash = null;
    public UISprite m_spriteEnd = null;

    public UILabel m_labelIndex = null;

    public float m_hpDuration = 0;
    float m_hpChangeSpeed = 0;
    public float m_hpMaxSpeed = 0;

    public float m_flashDuration = 0;
    float m_flashChangeSpeed = 0;
    public float m_flashMaxSpeed = 0;

    //上次的hp值
    float m_lastHPValue = -1;
    //上次的段数
    int m_lastIndex = -1;
    //总的段数
    int m_totalIndex = 0;
    //血条值
    float m_hpProcess = 0;
    //m_sliderHP改变的时间
    float m_hpTime = 0;
    //m_sliderFlash改变的时间
    float m_flashTime = 0;
    public void SetHp(NPC npc)
    {
        float hp = npc.HP;
        float hpMax = npc.MaxHP;
        if (m_lastHPValue <= 0 && !npc.IsDead)
        {//初始
            Reset(npc);
        }
        if (hp == m_lastHPValue)
        {
            return;
        }
        m_lastHPValue = hp;

        float curHPMax = (float)hpMax / m_totalIndex;
        float curIndex = (int)(hp / curHPMax) + (hp % curHPMax == 0 ? 0 : 1);
        float curProcess = hp % curHPMax != 0 ? hp % curHPMax / curHPMax : (hp <= 0 ? 0 : 1);

        m_hpProcess = curProcess;

        //flash
        if (m_flashDuration == 0 || m_hpDuration == 0)
        {
            Debug.LogWarning("boss blood bar, flash duration is " + m_flashDuration.ToString() + ", hp duration is " + m_hpDuration.ToString());
            return;
        }
        m_flashChangeSpeed = ((curProcess + curIndex) - (m_sliderFlash.value + m_lastIndex)) / m_flashDuration;
        if (System.Math.Abs(m_flashChangeSpeed) > m_flashMaxSpeed)
        {
            if (curProcess + curIndex > m_sliderFlash.value + m_lastIndex)
            {
                m_flashChangeSpeed = m_flashMaxSpeed;
            }
            else
            {
                m_flashChangeSpeed = -m_flashMaxSpeed;
            }
            m_flashTime = System.Math.Abs(((curProcess + curIndex) - (m_sliderFlash.value + m_lastIndex)) / m_flashChangeSpeed);
        }
        else
        {
            if (m_flashChangeSpeed != 0)
            {
                m_flashTime = m_flashDuration;
                m_sliderFlash.gameObject.SetActive(true);
                m_sliderFlash.GetComponent<Animation>().Play(m_sliderFlash.GetComponent<Animation>().clip.name);
            }
        }
        m_hpChangeSpeed = ((curProcess + curIndex) - (m_sliderHP.value + m_lastIndex)) / m_hpDuration;
        if (System.Math.Abs(m_hpChangeSpeed) > m_hpMaxSpeed)
        {
            if (curProcess + curIndex > m_sliderHP.value + m_lastIndex)
            {
                m_hpChangeSpeed = m_hpMaxSpeed;
            }
            else
            {
                m_hpChangeSpeed = -m_hpMaxSpeed;
            }
            m_hpTime = System.Math.Abs(((curProcess + curIndex) - (m_sliderHP.value + m_lastIndex)) / m_hpChangeSpeed);
        }
        else
        {
            if (m_hpChangeSpeed != 0)
            {
                m_hpTime = m_hpDuration;
            }
        }
    }

    void Reset(NPC npc)
    {
        m_sliderHP.value = 1;
        m_sliderFlash.value = 1;

        m_lastHPValue = npc.MaxHP;
        m_lastIndex = npc.CurrentTableInfo.HPCount;
        m_totalIndex = npc.CurrentTableInfo.HPCount;
        m_hpProcess = 1;

        ChangeColor();
    }

    void ChangeColor()
    {
        if (m_lastIndex == 0)
        {
            return;
        }
        if (m_lastIndex > m_totalIndex)
        {
            m_lastIndex = m_totalIndex;
        }
        m_labelIndex.text = "X" + m_lastIndex.ToString();
        if (m_lastIndex > 1)
        {
            m_spriteEnd.gameObject.SetActive(true);
        }
        else
        {
            m_spriteEnd.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if (m_hpTime > 0)
        {
            m_hpTime -= Time.deltaTime;
            if (m_hpTime < 0)
            {
                m_sliderHP.value = m_hpProcess;
            }
            else
            {
                float value = m_sliderHP.value;
                value += m_hpChangeSpeed * Time.deltaTime;
                if (value < 0)
                {
                    --m_lastIndex;
                    m_sliderHP.value = value + 1;
                    ChangeColor();
                }
                else if (value > 1)
                {
                    ++m_lastIndex;
                    m_sliderHP.value = value - 1;
                    ChangeColor();
                }
                else
                {
                    m_sliderHP.value = value;
                }
                if (m_lastIndex == 0)
                {
                    m_sliderHP.value = 0;
                    m_hpTime = 0;
                }
                else if (m_lastIndex > m_totalIndex)
                {
                    m_lastIndex = m_totalIndex;
                    m_sliderHP.value = 1;
                    m_hpTime = 0;
                }
            }
        }
        if (m_flashTime > 0.0f)
        {
            m_flashTime -= Time.deltaTime;
            if (m_flashTime < 0.0f)
            {
                m_sliderFlash.value = m_hpProcess;
                m_sliderFlash.gameObject.SetActive(false);
            }
            else
            {
                float value = m_sliderFlash.value;
                value += m_flashChangeSpeed * Time.deltaTime;
                if (value < 0)
                {
                    if (m_lastIndex != 0)
                    {
                        m_sliderFlash.value = value + 1;
                    }
                }
                else if (value > 1)
                {
                    if (m_lastIndex != m_totalIndex)
                    {
                        m_sliderFlash.value = value - 1;
                    }
                }
                else
                {
                    m_sliderFlash.value = value;
                }
            }
        }
        if (m_hpProcess <= 0 && m_hpTime <= 0 && m_flashTime < 0)
        {
            m_sliderHP.value = 0;
            m_sliderFlash.value = 0;
            m_hpChangeSpeed = 0;
            m_flashChangeSpeed = 0;
            gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}