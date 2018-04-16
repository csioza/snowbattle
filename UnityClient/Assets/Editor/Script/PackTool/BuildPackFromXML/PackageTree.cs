using System;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PackageTree
{
    //所有包，xml用
    public List<Package> AllPackageList = new List<Package>();
    //根包
    public Package RootPackage = null;
    
    //构建包
    public void BuildPackage(string filename)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(Application.dataPath + "/" + filename);
        XmlNode root = xml.SelectSingleNode("Package");

        RootPackage = new Package("root", Package.ENType.enNone);
        if (!LoadPackageFromXML(root, RootPackage))
        {
            Debug.LogWarning("LoadPackageFromXML failed.");
            return;
        }

        float startTime = Time.realtimeSinceStartup;
        Build();
        float length = Time.realtimeSinceStartup - startTime;
        Debug.Log("use time:" + length);
        
        WriteXML_PackMD5();
        WriteXML_PackDepend();
        WriteXML_FilePack();
    }
    //从xml中加载package信息
    bool LoadPackageFromXML(XmlNode xn, Package dependP)
    {
        XmlElement xe = (XmlElement)xn;

        string name = xe.GetAttribute("Name");
        string path = xe.GetAttribute("Folder");
        string type = xe.GetAttribute("PackType");
        string tag = xe.GetAttribute("Tag");

        Package parentP = null;
        if (!string.IsNullOrEmpty(path))
        {
            if (type == "Single")
            {//单个打包
                parentP = new Package(path, dependP, Package.ENType.enSingle, tag);
            }
            else if (type == "OneFileOnePackage")
            {//每个文件单独打成一个包
                parentP = new Package(path, dependP, Package.ENType.enOneFileOnePackage);
            }
            else if (type == "OneDependOnePackage")
            {//每个依赖单独打成一个包
                parentP = new Package(path, dependP, Package.ENType.enOneDependOnePackage);
            }
            else
            {
                if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(type))
                {
                    Debug.LogWarning("type is error, type:" + type + ",name:" + name);
                    return false;
                }
            }
        }
        if (parentP == null)
        {
            parentP = dependP;
        }
        //child
        for (int i = 0; i < xe.ChildNodes.Count; ++i)
        {
            if (LoadPackageFromXML(xe.ChildNodes[i], parentP))
            {
                continue;
            }
            return false;
        }
        return true;
    }
    //打包
    void Build()
    {
        SetNeedBuildPackage();
        //build
        XmlDocument xml = new XmlDocument();

        RootPackage.BuildPackage(xml);

        string fileName = BuildPackFromXML.Singleton.XMLDir + "result.xml";
        ResUtil.DeleteFile(fileName);
        xml.Save(fileName);
    }
    public void AddPackage(Package p)
    {
        AllPackageList.Add(p);
    }
    //获取包的依赖，从PackDepend.xml文件中
    Dictionary<string, List<string>> GetDependListFromXML()
    {
        Dictionary<string, List<string>> dependMap = new Dictionary<string, List<string>>();

        string filename = BuildPackFromXML.Singleton.XMLDir + "PackDepend.xml";
        if (System.IO.File.Exists(filename))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(filename);
            XmlNode root = xml.SelectSingleNode("PackDepend");

            foreach (var item in root.ChildNodes)
            {
                XmlElement xe = (XmlElement)item;

                string packname = xe.GetAttribute("PackName");

                List<string> dependList = new List<string>();
                foreach (var depend in xe.ChildNodes)
                {
                    XmlElement childE = (XmlElement)depend;
                    dependList.Add(childE.GetAttribute("PackName"));
                }
                if (dependMap.ContainsKey(packname))
                {//same key
                    List<string> oldList = new List<string>();
                    dependMap.TryGetValue(packname, out oldList);
                    foreach (var depend in oldList)
                    {
                        if (!dependList.Contains(depend))
                        {
                            dependList.Add(depend);
                        }
                    }
                    dependMap[packname] = dependList;
                }
                else
                {
                    dependMap.Add(packname, dependList);
                }
            }
        }

        return dependMap;
    }
    //设置需要打包的文件
    void SetNeedBuildPackage()
    {
        Dictionary<string, List<string>> dependMap = GetDependListFromXML();
        string filename = BuildPackFromXML.Singleton.XMLDir + "PackMD5.xml";
        if (!System.IO.File.Exists(filename))
        {
            return;
        }
        XmlDocument xml = new XmlDocument();
        xml.Load(filename);
        XmlNode root = xml.SelectSingleNode("PackMD5");

        int index = 0;
        foreach (var item in root.ChildNodes)
        {
            XmlElement xe = (XmlElement)item;

            string packname = xe.GetAttribute("PackName");
            string md5 = xe.GetAttribute("MD5");

            Package p = AllPackageList.Find(pack => pack.PackName == packname);
            if (p != null)
            {
                //比对依赖
                bool isNeedBuild = false;
                if (dependMap.ContainsKey(packname))
                {
                    List<string> dependList = new List<string>();
                    dependMap.TryGetValue(packname, out dependList);
                    if (dependList.Count != p.MyDependMap.Count)
                    {//依赖数量不一样，重新打包
                        Debug.LogWarning("依赖数量不一样，重新打包,packname:" + packname);
                        isNeedBuild = true;
                    }
                    foreach (var depend in dependList)
                    {
                        bool isContains = false;
                        foreach (var child in p.MyDependMap.Values)
                        {
                            if (0 == string.Compare(child.PackName, depend))
                            {
                                isContains = true;
                            }
                        }
                        if (!isContains)
                        {//老的依赖中不包含此依赖，则重新打包
                            Debug.LogWarning("老的依赖中不包含此依赖，则重新打包,packname:" + packname);
                            isNeedBuild = true;
                            break;
                        }
                    }
                }
                if (isNeedBuild) continue;
                //比对md5
                if (string.Compare(p.MD5, md5) == 0)
                {//相等
                    p.IsNeedBuild = false;
                    ++index;
                }
            }
        }
        Debug.Log("package build count:" + index);
    }
    //保存PackMD5.xml文件
    void WriteXML_PackMD5()
    {
        XmlDocument xml = new XmlDocument();
        XmlElement root = xml.CreateElement("PackMD5");

        foreach (var item in AllPackageList)
        {
            if (item.IsNeedBuild)
            {
                Debug.LogError("pack not build,packname:" + item.PackName);
                continue;
            }
            XmlElement e = xml.CreateElement("Pack");
            e.SetAttribute("PackName", item.PackName);
            e.SetAttribute("MD5", item.MD5);
            e.SetAttribute("FileSize", item.FileSize.ToString());
            e.SetAttribute("Tag", item.m_loadType);
            root.AppendChild(e);
        }
        xml.AppendChild(root);

        string fileName = BuildPackFromXML.Singleton.XMLDir + "PackMD5.xml";
        ResUtil.DeleteFile(fileName);
        xml.Save(fileName);
    }
    //保存PackDepend.xml文件
    void WriteXML_PackDepend()
    {
        XmlDocument xml = new XmlDocument();
        XmlElement root = xml.CreateElement("PackDepend");

        foreach (var item in AllPackageList)
        {
            if (item.IsNeedBuild) { continue; }
            XmlElement e = xml.CreateElement("Pack");
            e.SetAttribute("PackName", item.PackName);
            e.SetAttribute("IsExist", item.m_type == Package.ENType.enSingle ? "1" : "0");

            foreach (var depend in item.MyDependMap.Values)
            {
                if (item.PackName == depend.PackName) continue;
                XmlElement child = xml.CreateElement("DependPack");
                child.SetAttribute("PackName", depend.PackName);
                child.SetAttribute("IsExist", depend.m_type == Package.ENType.enSingle ? "1" : "0");
                e.AppendChild(child);
            }

            root.AppendChild(e);
        }
        xml.AppendChild(root);

        string fileName = BuildPackFromXML.Singleton.XMLDir + "PackDepend.xml";
        ResUtil.DeleteFile(fileName);
        xml.Save(fileName);
    }
    //保存FilePack.xml文件
    void WriteXML_FilePack()
    {
        XmlDocument xml = new XmlDocument();
        XmlElement root = xml.CreateElement("FilePack");

        foreach (var item in AllPackageList)
        {
            if (item.IsNeedBuild) { continue; }
            foreach (var path in item.FileList)
            {
                XmlElement child = xml.CreateElement("File");
                child.SetAttribute("FilePath", path);
                child.SetAttribute("PackName", item.PackName);

                root.AppendChild(child);
            }
        }
        xml.AppendChild(root);

        string fileName = BuildPackFromXML.Singleton.XMLDir + "FilePack.xml";
        ResUtil.DeleteFile(fileName);
        xml.Save(fileName);
    }
}