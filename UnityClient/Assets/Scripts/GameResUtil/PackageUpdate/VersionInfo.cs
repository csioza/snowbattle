using System;
using System.Xml;
using UnityEngine;

//版本信息
public class VersionInfo
{
    public string GameVersion = "";
    public string PackageVersion = "";

    public VersionInfo()
    {
        ;
    }
    public VersionInfo(string filePath)
    {
        LoadInfoWithFile(filePath);
    }
    //从文件中加载信息
    //xml格式如下：
    /*
     *  <Root>
     *      <Ver GameVersion="0.0.0.0" PackageVersion="0.0.0.0"/>
     *  </Root>
    */
    public void LoadInfoWithFile(string filePath)
    {
        if (!ResUtil.IsExistFile(filePath))
        {
            Debug.LogWarning("file is not exist, file = " + filePath);
            return;
        }
        try
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(filePath);

            XmlNode root = xml.SelectSingleNode("Root");
            foreach (var item in root.ChildNodes)
            {
                XmlElement xe = (XmlElement)item;
                
                GameVersion = xe.GetAttribute("GameVersion");
                PackageVersion = xe.GetAttribute("PackageVersion");
            }
        }
        catch (Exception exp)
        {
            Debug.LogWarning("LoadInfoWithFile exception, filePath = " + filePath + ", msg = " + exp.Message);
        }
    }
    //比较两个版本（字符串）
    static int Compare(string strA, string strB)
    {
        string[] arrayA = strA.Split(new char[1] { '.' });
        string[] arrayB = strB.Split(new char[1] { '.' });
        if (arrayA.Length > arrayB.Length)
        {
            return 1;
        }
        else if (arrayA.Length < arrayB.Length)
        {
            return -1;
        }
        else
        {
            for (int i = 0; i < arrayA.Length; ++i)
            {
                int numA = int.Parse(arrayA[i]);
                int numB = int.Parse(arrayB[i]);
                if (numA > numB)
                {
                    return 1;
                }
                else if (numA < numB)
                {
                    return -1;
                }
            }
        }
        return 0;
    }
    //比较两个VersionInfo中的GameVersion和PackageVersion
    static public int CompareVersion(VersionInfo infoA, VersionInfo infoB)
    {
        if (string.IsNullOrEmpty(infoA.GameVersion) || string.IsNullOrEmpty(infoA.PackageVersion))
        {
            return -1;
        }
        if (0 != string.Compare(infoA.GameVersion, infoB.GameVersion))
        {
            if (0 > Compare(infoA.GameVersion, infoB.GameVersion))
            {
                return -1;
            }
        }
        if (0 != string.Compare(infoA.PackageVersion, infoB.PackageVersion))
        {
            return Compare(infoA.PackageVersion, infoB.PackageVersion);
        }
        return 0;
    }
}
