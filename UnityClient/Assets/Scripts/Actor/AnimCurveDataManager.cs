using System;
using System.Collections.Generic;
using UnityEngine;

//每一帧的数据
public class KeyframeData
{
    public class PropertyInfo
    {
        //material propertyName
        public string m_name = "";
        //material color
        public Color m_color = Color.white;
        //material float
        public float m_value = float.MinValue;
        public PropertyInfo(string name)
        {
            m_name = name;
            m_color = Color.white;
            m_value = float.MinValue;
        }
    }
    //list转为string后为：(string,r/g/b/a>string,r/g/b/a>...)
    List<PropertyInfo> m_propertyInfoList = new List<PropertyInfo>();
    public List<PropertyInfo> PropertyInfoList { get { return m_propertyInfoList; } }

    public KeyframeData()
    {
        ;
    }
    public KeyframeData(string temp)
    {
        StringToList(temp);
    }
    public string GetString()
    {
        string temp = "";
        foreach (var item in m_propertyInfoList)
        {
            if (string.IsNullOrEmpty(item.m_name) ||
                (item.m_color == Color.white && item.m_value == float.MinValue))
            {
                continue;
            }
            temp += item.m_name;
            temp += ",";
            if (item.m_color != Color.white)
            {
                temp += item.m_color.r.ToString() + "/" + item.m_color.g.ToString() + "/" + item.m_color.b.ToString() + "/" + item.m_color.a.ToString();
            }
            else
            {
                temp += item.m_value.ToString();
            }
            temp += ">";
        }
        return temp;
    }
    void StringToList(string temp)
    {
        if (string.IsNullOrEmpty(temp))
        {
            return;
        }
        string[] array = temp.Split(new char[1] { '>' });
        foreach (var item in array)
        {
            if (string.IsNullOrEmpty(item))
            {
                continue;
            }
            string[] param = item.Split(new char[1] { ',' });

            PropertyInfo info = new PropertyInfo(param[0]);

            if (param[1].Contains("/"))
            {//color
                string[] rgba = param[1].Split(new char[1] { '/' });
                info.m_color.r = float.Parse(rgba[0]);
                info.m_color.g = float.Parse(rgba[1]);
                info.m_color.b = float.Parse(rgba[2]);
                info.m_color.a = float.Parse(rgba[3]);

                m_propertyInfoList.Add(info);
            }
            else
            {//value
                info.m_value = float.Parse(param[1]);
                m_propertyInfoList.Add(info);
            }
        }
    }
}

//animation的曲线数据
public class AnimCurveData
{
    //每一帧的间隔
    public float m_interval = 0;
    //list转为string后为：(时间:AnimCurveInfo;时间:AnimCurveInfo;...)
    List<KeyframeData> m_animCurveInfoList = new List<KeyframeData>();
    public List<KeyframeData> AnimCurveInfoList { get { return m_animCurveInfoList; } }

    string ListToString(List<KeyframeData> list)
    {
        string temp = "";
        if (list != null)
        {
            foreach (var item in list)
            {
                temp += item.GetString();
                temp += ";";
            }
        }
        return temp;
    }
    public void Save(BinaryHelper helper)
    {
        helper.Write(m_interval);
        helper.Write(ListToString(AnimCurveInfoList));
    }
    void StringToList(string temp)
    {
        if (string.IsNullOrEmpty(temp))
        {
            return;
        }
        string[] array = temp.Split(new char[1] { ';' });
        foreach (var item in array)
        {
            if (string.IsNullOrEmpty(item))
            {
                continue;
            }
            KeyframeData info = new KeyframeData(item);
            AnimCurveInfoList.Add(info);
        }
    }
    public void Read(BinaryHelper helper)
    {
        m_interval = helper.ReadFloat();
        StringToList(helper.ReadString());
    }
}
//管理加载的animation curve data
class AnimCurveDataManager
{
    #region Singleton
    static AnimCurveDataManager m_singleton;
    static public AnimCurveDataManager Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new AnimCurveDataManager();
            }
            return m_singleton;
        }
    }
    #endregion
    Dictionary<string, AnimCurveData> m_list = new Dictionary<string, AnimCurveData>();

    public AnimCurveData GetAnimCurveData(string key)
    {
        AnimCurveData data = null;
        if (!m_list.TryGetValue(key, out data))
        {
            TextAsset asset = PoolManager.Singleton.LoadWithoutInstantiate<TextAsset>(key);
            BinaryHelper helper = new BinaryHelper(asset.bytes);
            data = new AnimCurveData();
            data.Read(helper);

            m_list.Add(key, data);
        }
        return data;
    }
}