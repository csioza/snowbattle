using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//手柄自动映射UI按钮操作
public class JoystickAutoMapUIRoot
{

    string m_nextJoystickName = "Next";
    string m_prevJoystickName = "Prev";
    string m_confirmJoystickName = "Confirm";
    public GameObject m_markObject;
    public int m_currentUIIndex = -1;
    public static JoystickAutoMapUIRoot m_singleton;
    List<JoystickAutoMapUI> m_allJoystickUI = new List<JoystickAutoMapUI>();

    void Start()
    {
        m_singleton = this;
    }
    bool m_isDisable=false;
    // Update is called once per frame
    public void Update()
    {
        string[] names = Input.GetJoystickNames();
        if (names == null || names.Length == 0 || Application.isEditor)//
        {
            if (!m_isDisable)
            {
                m_markObject.SetActive(false);
                m_isDisable = true;
            }
            return;
        }
        if (m_isDisable)
        {
            m_isDisable = false;
            m_markObject.SetActive(true);
            MoveFocusButton(0);
        }
        if (Input.GetButtonDown(m_nextJoystickName))
        {
            MoveFocusButton(1);
        }
        else if (Input.GetButtonDown(m_prevJoystickName))
        {
            MoveFocusButton(-1);
        }
        else if (Input.GetButtonDown(m_confirmJoystickName))
        {
            ClickCurrent();
        }
    }

    public void AddAutoMapUI(JoystickAutoMapUI ui)
    {
        m_allJoystickUI.Remove(ui);
        m_allJoystickUI.Add(ui);
        m_currentUIIndex = m_allJoystickUI.Count - 1;
        ui.m_currentClickerIndex = 0;
        //Debug.LogWarning("AddUI:" + ui.m_ui.ToString());
        //if (GetCurrentClicker() == null)
        {
            MoveFocusButton(0);
        }
    }

    public void RemoveAutoMapUI(JoystickAutoMapUI ui)
    {
        bool isRemoved = m_allJoystickUI.Remove(ui);
        if (m_allJoystickUI.Count > 0 && isRemoved)
        {
            m_currentUIIndex = m_allJoystickUI.Count - 1;

//            JoystickAutoMapUI curUI = m_allJoystickUI[m_currentUIIndex];
            //Debug.LogWarning("RemoveUI:" + ui.m_ui.ToString() + ",Current:" + curUI.m_ui.ToString());
        
            MoveFocusButton(0);
        }
    }

    void MoveFocusButton(int offset)
    {
        int count = m_allJoystickUI.Count;
        if (count == 0)
        {
            return;
        }
        int dir = 1;
        if (offset<0)
        {
            dir = -1;
        }
        
        //多循环一次
        for (int i = 0; i < count+1; i++)
        {
            int index = (i * dir + m_currentUIIndex+count+count) % count;
            JoystickAutoMapUI ui = m_allJoystickUI[index];
            if (ui.MoveFocusButton(offset))
            {
                m_currentUIIndex = index;
//                GameObject obj = ui.m_uiClickObjs[ui.m_currentClickerIndex % ui.m_uiClickObjs.Count];
                //Debug.LogWarning("JoystickPos:" + ui.m_ui.ToString() + ",Button:" + obj);
                break;
            }
            index = (i + m_currentUIIndex+1) % count;
            JoystickAutoMapUI nextUI = m_allJoystickUI[index];
            nextUI.ResetIndexByOffset(offset);
        }

        CorrectMark();
    }
    GameObject GetCurrentClicker()
    {
        int count = m_allJoystickUI.Count;
        if (count == 0 || m_currentUIIndex<0)
        {
            return null;
        }
        JoystickAutoMapUI ui = m_allJoystickUI[m_currentUIIndex % count];
        if (ui.m_uiClickObjs == null || ui.m_uiClickObjs.Count == 0)
        {
            return null;
        }
        if (ui.m_currentClickerIndex < 0 || ui.m_currentClickerIndex >= ui.m_uiClickObjs.Count)
        {
            return null;
        }
        GameObject obj = ui.m_uiClickObjs[ui.m_currentClickerIndex % ui.m_uiClickObjs.Count];
        return obj;
    }
    void CorrectMark()
    {
        GameObject obj = GetCurrentClicker();
        if (obj == null)
        {
            return;
        }
        Vector3 offset = Vector3.zero;
        BoxCollider collider = obj.GetComponent<BoxCollider>();
        if (collider != null)
        {
            offset = collider.center;
        }
        if (m_markObject != null && obj != null)
        {
            Transform t = m_markObject.transform.parent;
            m_markObject.transform.parent = obj.transform;
            m_markObject.transform.localPosition = offset;
            Vector3 pos = m_markObject.transform.position;
            //Vector3 scale = m_markObject.transform.
            m_markObject.transform.parent = t;
            m_markObject.transform.position = pos;
        }
    }
    void ClickCurrent()
    {
        GameObject obj = GetCurrentClicker();
        if (obj == null)
        {
            return;
        }
        UIEventListener e = obj.GetComponent<UIEventListener>();
        if (e != null && e.onClick != null)
        {
            e.onClick(e.gameObject);
        }
        else
        {
            UIMouseClick e1 = obj.GetComponent<UIMouseClick>();
            if (e1 != null)
            {
                e1.OnClick();
            }
            else
            {
                UIInput i = obj.GetComponent<UIInput>();
                if (i != null)
                {
                    i.isSelected = true;
                }
            }
        }
    }
}