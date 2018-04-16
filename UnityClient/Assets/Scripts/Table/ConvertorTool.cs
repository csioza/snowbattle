using System;
using System.Collections.Generic;

//string和list、dictionary等的相互转换
public class ConvertorTool
{
    #region List<int>和string的相互转换
    static public List<int> StringToList_Int(string temp, char separator = ',')
    {
        if (!string.IsNullOrEmpty(temp))
        {
            string[] param = temp.Split(new char[1] { separator });
            List<int> list = new List<int>(param.Length);
            foreach (var item in param)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    list.Add(int.Parse(item));
                }
            }
            return list;
        }
        return new List<int>();
    }
    static public string ListToString(List<int> list, string separator = ",")
    {
        string temp = "";
        if (list != null)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                temp += list[i].ToString();
                if (i != list.Count - 1)
                {
                    temp += separator;
                }
            }
        }
        return temp;
    }
    #endregion
    #region List<float>和string的相互转换
    static public List<float> StringToList_Float(string temp, char separator = ',')
    {
        if (!string.IsNullOrEmpty(temp))
        {
            string[] param = temp.Split(new char[1] { separator });
            List<float> list = new List<float>(param.Length);
            foreach (var item in param)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    list.Add(float.Parse(item));
                }
            }
            return list;
        }
        return new List<float>();
    }
    static public string ListToString(List<float> list, string separator = ",")
    {
        string temp = "";
        if (list != null)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                temp += list[i].ToString();
                if (i != list.Count - 1)
                {
                    temp += separator;
                }
            }
        }
        return temp;
    }
    #endregion
    #region List<string>和string的相互转换
    static public List<string> StringToList_String(string temp, char separator = ',')
    {
        if (!string.IsNullOrEmpty(temp))
        {
            string[] param = temp.Split(new char[1] { separator });
            List<string> list = new List<string>(param.Length);
            foreach (var item in param)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    list.Add(item);
                }
            }
            return list;
        }
        return new List<string>();
    }
    static public string ListToString(List<string> list, string separator = ",")
    {
        string temp = "";
        if (list != null)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                temp += list[i];
                if (i != list.Count - 1)
                {
                    temp += separator;
                }
            }
        }
        return temp;
    }
    #endregion
    #region int[]和string的相互转换
    static public string ArrayToString(int[] list, string separator = ",")
    {
        string temp = "";
        if (list != null)
        {
            for (int i = 0; i < list.Length; ++i)
            {
                temp += list[i].ToString();
                if (i != list.Length - 1)
                {
                    temp += separator;
                }
            }
        }
        return temp;
    }
    static public int[] StringToArray_Int(string temp, char separator = ',')
    {
        if (!string.IsNullOrEmpty(temp))
        {
            string[] param = temp.Split(new char[1] { separator });
            int[] list = new int[param.Length];
            for (int i = 0; i < param.Length; ++i)
            {
                list[i] = int.Parse(param[i]);
            }
            return list;
        }
        return new int[0];
    }
    #endregion
}