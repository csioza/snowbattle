using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class JoystickAutoMapUI
{
    public List<GameObject> m_uiClickObjs;
    public int m_currentClickerIndex = 0;
    public UIWindow m_ui;

    public void Init(UIWindow ui)
    {
        m_ui = ui;

        if (UIManager.Singleton.JoystickAutoMap != null)
        {
            FindEventListener();
        }
    }

    public void OnDestroy()
    {
        if (UIManager.Singleton.JoystickAutoMap != null)
        {
            UIManager.Singleton.JoystickAutoMap.RemoveAutoMapUI(this);
        }
    }
    public void OnDisable()
    {
        if (UIManager.Singleton.JoystickAutoMap != null)
        {
            UIManager.Singleton.JoystickAutoMap.RemoveAutoMapUI(this);
        }
    }
    public void OnEnable()
    {
        if (UIManager.Singleton.JoystickAutoMap != null)
        {
            UIManager.Singleton.JoystickAutoMap.AddAutoMapUI(this);
        }
    }

    public void ResetIndexByOffset(int offset)
    {
        if (offset < 0)
        {
            m_currentClickerIndex = m_uiClickObjs.Count;
        }
        if (offset > 0)
        {
            m_currentClickerIndex = -1;
        }
        //Debug.LogWarning(m_ui + " Reset By Offset:" + offset + ",To:" + m_currentClickerIndex);
    }

    public void FindEventListener()
    {
        if (m_uiClickObjs == null)
        {
            m_uiClickObjs = new List<GameObject>();
        }
        m_uiClickObjs.Clear();
        UIEventListener[] objs = m_ui.WindowRoot.GetComponentsInChildren<UIEventListener>(true);
        for (int i = 0; i < objs.Length; i++)
        {
            m_uiClickObjs.Add(objs[i].gameObject);
        }
        UIMouseClick[] objsClick = m_ui.WindowRoot.GetComponentsInChildren<UIMouseClick>(true);
        for (int i = 0; i < objsClick.Length; i++)
        {
            m_uiClickObjs.Add(objsClick[i].gameObject);
        }
        UIInput[] objsInput = m_ui.WindowRoot.GetComponentsInChildren<UIInput>(true);
        for (int i = 0; i < objsInput.Length; i++)
        {
            m_uiClickObjs.Add(objsInput[i].gameObject);
        }
    }
    public bool MoveFocusButton(int offset)
    {
        if (m_uiClickObjs == null || m_uiClickObjs.Count == 0 || !m_ui.IsVisiable())
        {
            return false;
        }
        int retryCount = 100;
        GameObject obj = null;
        do
        {
            m_currentClickerIndex += offset;
            if (m_currentClickerIndex < 0 || m_currentClickerIndex >= m_uiClickObjs.Count)
            {
                break;
            }

            m_currentClickerIndex = m_currentClickerIndex % m_uiClickObjs.Count;

            obj = m_uiClickObjs[m_currentClickerIndex % m_uiClickObjs.Count];
            if (obj.activeInHierarchy)
            {
                retryCount=0;
            }
			retryCount--;
        } while (retryCount > 0);
        if (m_currentClickerIndex >= 0 && m_currentClickerIndex<m_uiClickObjs.Count)
        {
            return true;
        }
        return false;
    }
}

