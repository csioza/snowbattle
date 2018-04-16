using UnityEngine;
using System.Collections.Generic;

public class ChangeMaterial : MonoBehaviour
{
    public List<GameObject> m_objList = null;
    public string m_pathName = "";
    int m_lastKey = -1;
    float m_percent = 0;
    public float Percent
    {
        protected get { return m_percent; }
        set
        {
            if (value > 1)
                value = 1;
            m_percent = value;
        }
    }

    void Update()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        AnimCurveData data = AnimCurveDataManager.Singleton.GetAnimCurveData("Anim/" + m_pathName + ".bytes");
        int key = (int)((data.AnimCurveInfoList.Count-1) * Percent);
        if (key <= m_lastKey)
        {
            return;
        }
        if (key >= data.AnimCurveInfoList.Count)
        {
            Debug.LogWarning("曲线数据出错，当前key是：" + key.ToString() + ", name is " + m_pathName);
            return;
        }
        m_lastKey = key;
        foreach (var item in m_objList)
        {
            Renderer[] renders = item.GetComponents<Renderer>();
            foreach (Renderer r in renders)
            {
                KeyframeData info = data.AnimCurveInfoList[key];
                foreach (var propItem in info.PropertyInfoList)
                {
                    if (r.material.HasProperty(propItem.m_name))
                    {
                        if (propItem.m_color != Color.white)
                        {
                            r.material.SetColor(propItem.m_name, propItem.m_color);
                        }
                        if (propItem.m_value != float.MinValue)
                        {
                            r.material.SetFloat(propItem.m_name, propItem.m_value);
                        }
                    }
                }
            }
        }
    }
}
