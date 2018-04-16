using System;
using System.Xml;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class PackageInfo
{
    //文件名称（包含路径）
    public string FilePath { get; set; }
    //包的名称
    string m_packName = "";
    public string PackName
    {
        get
        {
            if (string.IsNullOrEmpty(m_packName))
            {
                if (!string.IsNullOrEmpty(FilePath))
                {
                    m_packName = FilePath.ToLower().Replace("/", "_") + ".pack";
                }
            }
            return m_packName;
        }
    }
    //文件列表
    List<string> m_fileList = null;
    public List<string> FileList
    {
        get
        {
            if (m_fileList == null)
            {
                if (System.IO.Directory.Exists(FilePath))
                {
                    m_fileList = ArchiveUtil.NtfGetFiles(FilePath, true);
                }
                else if (System.IO.File.Exists(FilePath))
                {
                    m_fileList = new List<string>();
                    m_fileList.Add(FilePath);
                }
                else
                {
                    m_fileList = new List<string>();
                }
                m_fileList.RemoveAll(item => IsPass(item));
            }
            return m_fileList;
        }
    }

    public PackageInfo()
    {
        FilePath = "";
    }
    public PackageInfo(string filePath)
    {
        FilePath = filePath;
    }
    //是否过滤掉
    bool IsPass(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return true;
        }
        if (path.Contains("."))
        {
            string temp = path.Substring(path.LastIndexOf(".")).ToLower();
            if (temp == ".meta")
            {//脚本文件、meta文件不打包
                return true;
            }
        }
        return false;
    }
}
public class Package
{
    public enum ENType
    {
        enNone,
        enSingle,
        enOneFileOnePackage,
        enOneDependOnePackage,
    }
    //打包的类型
    public ENType m_type = ENType.enNone;
    //加载类型
    public string m_loadType = "";
    //我依赖的包的列表
    public Dictionary<string, Package> MyDependMap = new Dictionary<string, Package>();
    //依赖我的包的列表
    public Dictionary<string, Package> DependMeMap = new Dictionary<string, Package>();

    //包信息
    public PackageInfo MyInfo = null;
    public string Key { get { return MyInfo.FilePath; } }
    public string PackName { get { return MyInfo.PackName; } }
    public List<string> FileList
    {
        get
        {
            if (m_type == ENType.enSingle)
            {
                return MyInfo.FileList;
            }
            else
            {
                List<string> list = new List<string>();
                list.Add(PackName);
                return list;
            }
        }
    }

    //是否需要打包
    public bool IsNeedBuild = true;

    //包文件的大小
    long m_filesize = 0;
    public long FileSize
    {
        get
        {
            if (m_type == ENType.enSingle)
            {
                if (m_filesize == 0)
                {
                    string filename = BuildPackFromXML.Singleton.PackageDir + PackName;
                    try
                    {
                        System.IO.FileInfo f = new System.IO.FileInfo(filename);
                        m_filesize = f.Length;
                    }
                    catch (SystemException e)
                    {
                        Debug.LogWarning("read file size exception, filename:" + filename + ",msg:" + e.Message);
                    }
                }
            }
            return m_filesize;
        }
    }

    //包的md5
    string m_md5 = "";
    public string MD5
    {
        get
        {
            if (string.IsNullOrEmpty(m_md5))
            {
                List<string> fileList = FileList;
                if (fileList.Count == 1)
                {
                    if (System.IO.File.Exists(fileList[0]))
                    {
                        m_md5 = GenMD5Util.NtfGenFileMD5(fileList[0]);
                    }
                    else
                    {
                        m_md5 = GenMD5Util.NtfGenStringMD5(this.PackName);
                    }
                }
                else
                {
                    string fileMD5Combine = "";
                    foreach (var f in fileList)
                    {
                        fileMD5Combine += GenMD5Util.NtfGenFileMD5(f);
                    }
                    m_md5 = GenMD5Util.NtfGenStringMD5(fileMD5Combine);
                }
            }
            return m_md5;
        }
    }

    //当m_type为enOneDependOnePackage时，用到,string为path
    Dictionary<string, Package> AllPackageMap = null;
    //当m_type为enOneDependOnePackage时，用到
    List<List<Package>> m_buildPackageList = null;

    public Package()
    {
        MyInfo = new PackageInfo();
        m_type = ENType.enNone;
        BuildPackFromXML.Singleton.PackTree.AddPackage(this);
    }
    public Package(string path, ENType type)
    {
        MyInfo = new PackageInfo(path);
        m_type = type;
        BuildPackFromXML.Singleton.PackTree.AddPackage(this);
    }
    public Package(string path, Package dependP, ENType type, string tag = "")
    {
        MyInfo = new PackageInfo(path);
        if (dependP.Key != path || string.IsNullOrEmpty(path))
        {
            AddMyDependPackage(dependP);
        }
        m_type = type;
        switch (m_type)
        {
            case ENType.enSingle:
                {
                    if (m_type == ENType.enSingle)
                    {
                        m_loadType = tag;
                    }
                }
                break;
            case ENType.enOneFileOnePackage:
                {
                    List<string> fileList = new List<string>();
                    if (ResUtil.IsExistFile(path))
                    {
                        fileList.Add(path);
                    }
                    else
                    {
                        fileList = ArchiveUtil.NtfGetAllFiles(path, true);
                    }
                    foreach (var item in fileList)
                    {
                        if (IsPass(item)) continue;

                        new Package(item, this, Package.ENType.enSingle);
                    }
                }
                break;
            case ENType.enOneDependOnePackage:
                {
                    AllPackageMap = new Dictionary<string, Package>();
                    List<string> fileList = new List<string>();
                    if (ResUtil.IsExistFile(path))
                    {
                        fileList.Add(path);
                    }
                    else
                    {
                        fileList = ArchiveUtil.NtfGetAllFiles(path, true);
                    }
                    //共享包
                    Package shareP = CreateSharePackage(fileList);
                    //每个子包
                    foreach (var item in fileList)
                    {
                        if (IsPass(item)) continue;

                        //加载依赖包
                        if (null == LoadDependPackage(item, shareP, this))
                        {
                            Debug.LogWarning("load depend package failed.path:" + path + ",file:" + item);
                        }
                    }
                    //构建一个列表，分层
                    List<Package> list = new List<Package>(DependMeMap.Values);
                    m_buildPackageList = new List<List<Package>>();
                    m_buildPackageList.Add(new List<Package>(list));
                    while (true)
                    {
                        List<Package> dependList = new List<Package>();
                        foreach (var item in list)
                        {
                            List<Package> childList = new List<Package>(item.DependMeMap.Values);
                            foreach (var child in childList)
                            {
                                if (null == dependList.Find(p => p.PackName == child.PackName))
                                {
                                    dependList.Add(child);
                                }
                            }
                        }
                        if (dependList.Count == 0)
                        {
                            break;
                        }
                        list = new List<Package>(dependList);
                        m_buildPackageList.Add(new List<Package>(dependList));
                    }
                    //删除层次靠前，并且和层次后面重叠的package
                    for (int i = m_buildPackageList.Count - 1; i >= 0; --i)
                    {
                        List<Package> lastList = m_buildPackageList[i];
                        for (int j = i - 1; j >= 0; --j)
                        {
                            List<Package> curList = m_buildPackageList[j];
                            for (int k = 0; k < curList.Count; )
                            {
                                Package curP = curList[k];
                                if (null != lastList.Find(p => p.PackName == curP.PackName))
                                {
                                    curList.RemoveAt(k);
                                }
                                else
                                {
                                    ++k;
                                }
                            }
                        }
                    }
                }
                break;
        }
        BuildPackFromXML.Singleton.PackTree.AddPackage(this);
    }
    #region depend
    //创建共享包
    Package CreateSharePackage(List<string> fileList)
    {
        List<UnityEngine.Object> objList = new List<UnityEngine.Object>();
        foreach (var item in fileList)
        {
            UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(item);
            if (obj == null) continue;

            objList.Add(obj);
        }
        UnityEngine.Object[] dependObjList = EditorUtility.CollectDependencies(objList.ToArray());
        List<UnityEngine.Object> shareObjList = new List<UnityEngine.Object>();
        foreach (var item in dependObjList)
        {
            if (item.GetType() == typeof(UnityEngine.Shader))
            {
                if (!shareObjList.Contains(item))
                {
                    shareObjList.Add(item);
                }
            }
        }

        string filepath = "assets/resources/" + this.Key.ToLower().Replace("/", "_") + "_Share.prefab";
        CreatePrefab.Singleton.Create(filepath, shareObjList);
        Package p = new Package(filepath, this, ENType.enSingle);
        return p;
    }
    //加载依赖包
    Package LoadDependPackage(string path, Package dependP, Package rootPackage)
    {
        Package returnP = null;
        string[] dependArray = AssetDatabase.GetDependencies(new string[1] { path });
        if (dependArray == null || dependArray.Length == 0)
        {//获取依赖失败
            Debug.LogError("GetDependencies error, path:" + path);
        }
        else
        {
            Package parentP = GetPackage(path, rootPackage);
            if (parentP != null)
            {
                parentP.AddMyDependPackage(dependP);
                if (dependArray.Length != 1)
                {//依赖的依赖
                    foreach (var item in dependArray)
                    {
                        if (item.ToLower() == path.ToLower()) continue;

                        //加载依赖包
                        Package childP = LoadDependPackage(item, dependP, rootPackage);
                        parentP.AddMyDependPackage(childP);
                    }
                }
                returnP = parentP;
            }
            else
            {
                returnP = dependP;
            }
        }
        return returnP;
    }
    //添加包到AllPackageMap
    void AddPackage(Package p)
    {
        if (!AllPackageMap.ContainsKey(p.Key))
        {
            AllPackageMap.Add(p.Key, p);
        }
    }
    //获取包
    Package GetPackage(string path, Package rootPackage, ENType type = ENType.enSingle)
    {
        Package p = null;
        if (AllPackageMap.ContainsKey(path))
        {
            AllPackageMap.TryGetValue(path, out p);
        }
        else
        {//是否存在依赖包里
            if (rootPackage.IsFileWithMyDepend(path))
            {
                return null;
            }
        }
        if (p == null)
        {
            p = new Package(path, type);
            AddPackage(p);
        }
        return p;
    }
    #endregion
    //路径是否被过滤掉
    bool IsPass(string path)
    {
        string temp = path.Substring(path.LastIndexOf(".")).ToLower();
        if (temp == ".meta")
        {//.meta文件不打包
            return true;
        }
        return false;
    }
    //添加我依赖的包到MyDependMap
    void AddMyDependPackage(Package dependP)
    {
        if (dependP.PackName == this.PackName)
        {
            return;
        }
        if (this.IsMyDepend(dependP.Key))
        {
            return;
        }
        string key = dependP.Key;
        if (!MyDependMap.ContainsKey(key))
        {
            MyDependMap.Add(key, dependP);
        }
        key = this.Key;
        if (!dependP.DependMeMap.ContainsKey(key))
        {
            dependP.DependMeMap.Add(key, this);
        }
    }
    //是否是我的依赖
    bool IsMyDepend(string key)
    {
        foreach (var item in MyDependMap.Values)
        {
            if (item.IsMyDepend(key))
            {//是子包的依赖
                return true;
            }
        }
        return MyDependMap.ContainsKey(key);
    }
    //是否是我依赖包里的文件
    bool IsFileWithMyDepend(string path)
    {
        foreach (var pack in MyDependMap.Values)
        {
            if (pack.IsFileWithMyDepend(path))
            {
                return true;
            }
            foreach (var file in pack.FileList)
            {
                if (0 == string.Compare(file.ToLower(), path.ToLower()))
                {//存在依赖包中
                    return true;
                }
            }
        }
        return false;
    }
    void Push()
    {
        BuildPipeline.PushAssetDependencies();
        ++BuildPackFromXML.Singleton.PushPackageCount;
    }
    void PopAll()
    {
        int count = BuildPackFromXML.Singleton.PushPackageCount;
        while (count > 0)
        {
            BuildPipeline.PopAssetDependencies();
            --count;
        }
    }
    //构建包
    public void BuildPackage(XmlDocument xml, XmlNode xn = null)
    {
        BuildPipeline.PushAssetDependencies();
        XmlElement xe = null;
        if (xn == null)
        {
            xe = xml.CreateElement("Package");
            xe.SetAttribute("PackName", PackName);
            xml.AppendChild(xe);

            xn = xe;
        }
        else
        {
            xe = xml.CreateElement("Package");
            xe.SetAttribute("PackName", this.PackName);
            xn.AppendChild(xe);
        }
        switch (m_type)
        {
            case ENType.enNone:
            case ENType.enSingle:
            case ENType.enOneFileOnePackage:
                {
                    BuildSelf();
                    foreach (var item in DependMeMap.Values)
                    {
                        item.BuildPackage(xml, xe);
                    }
                }
                break;
            case ENType.enOneDependOnePackage:
                {
                    BuildSelf();
                    int index = 0;
                    foreach (var item in m_buildPackageList)
                    {
                        Push();

                        ++index;
                        XmlElement child = xml.CreateElement("package" + index.ToString());
                        xe.AppendChild(child);

                        foreach (var p in item)
                        {
                            XmlElement child2 = xml.CreateElement("child" + index.ToString());
                            child2.SetAttribute("PackName", p.PackName);
                            child.AppendChild(child2);

                            p.BuildSelf();
                        }
                    }
                    PopAll();
                }
                break;
        }
        BuildPipeline.PopAssetDependencies();
    }
    //构建自己
    public void BuildSelf()
    {
        if (!IsNeedBuild)
        {
            ++BuildPackFromXML.Singleton.SuccPackageCount;
            return;
        }
        IsNeedBuild = false;
        List<UnityEngine.Object> objList = new List<UnityEngine.Object>();
        List<string> fileList = new List<string>();
        if (m_type == ENType.enSingle)
        {
            foreach (var item in MyInfo.FileList)
            {
                UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(item);
                if (obj == null) continue;

                objList.Add(obj);
                fileList.Add(item.ToLower());
            }
        }
        else
        {
            IsNeedBuild = false;
            ++BuildPackFromXML.Singleton.SuccPackageCount;
            return;
        }
        if (fileList.Count == 0)
        {
            Debug.Log("build package failed, FileList.Count:" + MyInfo.FileList.Count + ", PackName:" + MyInfo.PackName);
            //空+1
            ++BuildPackFromXML.Singleton.NullPackageCount;
            return;
        }
        string pathname = BuildPackFromXML.Singleton.PackageDir + MyInfo.PackName;
        BuildAssetBundleOptions ops = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;
        if (!BuildPipeline.BuildAssetBundleExplicitAssetNames(objList.ToArray(), fileList.ToArray(), pathname, ops, BuildPackFromXML.Singleton.Target))
        {
            Debug.LogError("build package failed! package file path is " + pathname);
            //失败+1
            ++BuildPackFromXML.Singleton.FailPackageCount;
            return;
        }
        Debug.Log("build package succed! package file path is " + pathname);
        //成功+1
        ++BuildPackFromXML.Singleton.SuccPackageCount;
    }
}