using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//包和md5的对应（基于PackMD5.xml）
public class XML_PackMD5
{
    public class Info
    {
        public string PackName;
        public string MD5;
        public string FileSize;
        public string Tag;

        public Info()
        {
            ;
        }
        public Info(Info source)
        {
            Update(source);
        }
        public void Update(Info source)
        {
            PackName = source.PackName;
            MD5 = source.MD5;
            FileSize = source.MD5;
            Tag = source.Tag;
        }
    }
    public List<Info> InfoList = new List<Info>();
    XmlDocument XML = new XmlDocument();

    public XML_PackMD5(string path)
    {
        Load(path);
    }
    //从xml文件中加载
    bool Load(string path)
    {
        string filename = path + ResPath.PackMD5FileName;
        if (!System.IO.File.Exists(filename))
        {
            Debug.LogWarning("file is not exists, filename:" + filename);
            return false;
        }
        if (XML == null)
        {
            XML = new XmlDocument();
        }
        try
        {
            XML.Load(filename);
            XmlNode root = XML.SelectSingleNode("PackMD5");

            foreach (var item in root.ChildNodes)
            {
                XmlElement xe = (XmlElement)item;

                Info info = new Info();
                info.PackName = xe.GetAttribute("PackName");
                info.MD5 = xe.GetAttribute("MD5");
                info.FileSize = xe.GetAttribute("FileSize");
                info.Tag = xe.GetAttribute("Tag");

                InfoList.Add(info);
            }
        }
        catch (XmlException e)
        {
            Debug.LogWarning("load xml exception, filename:" + filename + ",msg:" + e.Message);
            return false;
        }
        return true;
    }
    //获取大小
    public long GetFileSize(string packname)
    {
        Info info = InfoList.Find(item => item.PackName == packname);
        if (info != null)
        {
            return long.Parse(info.FileSize);
        }
        return 0;
    }
    //更新
    public void Update(Info info)
    {
        Info infoLocal = InfoList.Find(item => item.PackName == info.PackName);
        if (infoLocal == null)
        {
            infoLocal = new Info(info);
            InfoList.Add(infoLocal);
        }
        infoLocal.Update(info);
    }
    //保存
    public void Save()
    {
        XmlNode root = XML.SelectSingleNode("PackMD5");
        if (root == null)
        {
            root = XML.CreateElement("PackMD5");
        }
        root.RemoveAll();
        foreach (var item in InfoList)
        {
            XmlElement e = XML.CreateElement("Pack");
            e.SetAttribute("PackName", item.PackName);
            e.SetAttribute("MD5", item.MD5);
            e.SetAttribute("FileSize", item.FileSize);
            e.SetAttribute("Tag", item.Tag);
            root.AppendChild(e);
        }
        XML.AppendChild(root);
        string filename = ResPath.LocalPath + ResPath.CurrentPathName + ResPath.PackMD5FileName;
        ResUtil.DeleteFile(filename);
        XML.Save(filename);
    }
    //清除
    public void Clear()
    {
        InfoList.Clear();
        InfoList = null;
        XML.RemoveAll();
        XML = null;
    }
    //获取更新列表 xmlA是服务器版本，xmlB是本地版本
    static public List<Info> GetUpdateList(XML_PackMD5 xmlA, XML_PackMD5 xmlB, out long length)
    {
        List<Info> updateList = new List<Info>();
        length = 0;
        foreach (var infoA in xmlA.InfoList)
        {
            Info infoB = xmlB.InfoList.Find(item => item.PackName == infoA.PackName);
            if (infoB != null)
            {
                if (0 != string.Compare(infoA.MD5, infoB.MD5))
                {//md5码不一致
                    updateList.Add(infoA);
                    length += long.Parse(infoA.FileSize);
                }
            }
            else
            {//本地不存在
                updateList.Add(infoA);
                length += long.Parse(infoA.FileSize);
            }
        }
        return updateList;
    }
}
//文件和包的对应（基于FilePack.xml）
public class XML_FilePack
{
    public class Info
    {
        public string FileName;
        public string PackName;
    }
    public List<Info> InfoList = new List<Info>();
    XmlDocument XML = new XmlDocument();

    public XML_FilePack(string path)
    {
        Load(path);
    }
    //从xml文件中加载
    bool Load(string path)
    {
        string filename = path + ResPath.FilePackFileName;
        if (!System.IO.File.Exists(filename))
        {
            Debug.LogWarning("file is not exists, filename:" + filename);
            return false;
        }
        if (XML == null)
        {
            XML = new XmlDocument();
        }
        try
        {
            XML.Load(filename);
            XmlNode root = XML.SelectSingleNode("FilePack");

            foreach (var item in root.ChildNodes)
            {
                XmlElement xe = (XmlElement)item;

                Info info = new Info();
                info.FileName = xe.GetAttribute("FilePath");
                info.PackName = xe.GetAttribute("PackName");

                InfoList.Add(info);
            }
        }
        catch (XmlException e)
        {
            Debug.LogWarning("load xml exception, filename:" + filename + ",msg:" + e.Message);
            return false;
        }
        return true;
    }
    //更新
    public void Update(string packname, XML_FilePack remote)
    {
        InfoList.RemoveAll(item => item.PackName == packname);
        InfoList.AddRange(remote.InfoList.FindAll(item => item.PackName == packname));
    }
    //保存
    public void Save()
    {
        XmlNode root = XML.SelectSingleNode("FilePack");
        if (root == null)
        {
            root = XML.CreateElement("FilePack");
        }
        root.RemoveAll();
        foreach (var item in InfoList)
        {
            XmlElement e = XML.CreateElement("File");
            e.SetAttribute("FilePath", item.FileName);
            e.SetAttribute("PackName", item.PackName);
            root.AppendChild(e);
        }
        XML.AppendChild(root);
        string filename = ResPath.LocalPath + ResPath.CurrentPathName + ResPath.FilePackFileName;
        ResUtil.DeleteFile(filename);
        XML.Save(filename);
    }
    //清除
    public void Clear()
    {
        InfoList.Clear();
        InfoList = null;
        XML.RemoveAll();
        XML = null;
    }
}
//依赖包（基于PackDepend.xml）
public class XML_PackDepend
{
    public class Info
    {
        public string PackName;
        public Dictionary<string, bool> DependPackMap = new Dictionary<string, bool>();

        public Info()
        {
            ;
        }
        public Info(Info source)
        {
            Update(source);
        }
        public void Update(Info source)
        {
            PackName = source.PackName;
            foreach (var item in source.DependPackMap)
            {
                if (!DependPackMap.ContainsKey(item.Key))
                {
                    DependPackMap.Add(item.Key, item.Value);
                }
                else
                {
                    DependPackMap[item.Key] = item.Value;
                }
            }
        }
    }
    public List<Info> InfoList = new List<Info>();
    XmlDocument XML = new XmlDocument();

    public XML_PackDepend(string path)
    {
        Load(path);
    }
    //从xml文件中加载
    bool Load(string path)
    {
        string filename = path + ResPath.PackDependFileName;
        if (!System.IO.File.Exists(filename))
        {
            Debug.LogWarning("file is not exists, filename:" + filename);
            return false;
        }
        if (XML == null)
        {
            XML = new XmlDocument();
        }
        try
        {
            XML.Load(filename);
            XmlNode root = XML.SelectSingleNode("PackDepend");

            foreach (var item in root.ChildNodes)
            {
                XmlElement xe = (XmlElement)item;

                Info info = new Info();
                info.PackName = xe.GetAttribute("PackName");

                foreach (var depend in xe.ChildNodes)
                {
                    XmlElement childE = (XmlElement)depend;
                    info.DependPackMap.Add(childE.GetAttribute("PackName"), childE.GetAttribute("IsExist") == "1" ? true : false);
                }
                InfoList.Add(info);
            }
        }
        catch (XmlException e)
        {
            Debug.LogWarning("load xml exception, filename:" + filename + ",msg:" + e.Message);
            return false;
        }
        return true;
    }
    //更新
    public void Update(string packname, XML_PackDepend remote)
    {
        Info infoLocal = InfoList.Find(item => item.PackName == packname);
        Info infoRemote = remote.InfoList.Find(item => item.PackName == packname);
        if (infoLocal == null)
        {
            infoLocal = new Info(infoRemote);
            InfoList.Add(infoLocal);
        }
        infoLocal.Update(infoRemote);
    }
    //保存
    public void Save()
    {
        XmlNode root = XML.SelectSingleNode("PackDepend");
        if (root == null)
        {
            root = XML.CreateElement("PackDepend");
        }
        root.RemoveAll();
        foreach (var item in InfoList)
        {
            XmlElement e = XML.CreateElement("Pack");
            e.SetAttribute("PackName", item.PackName);

            foreach (var depend in item.DependPackMap)
            {
                XmlElement child = XML.CreateElement("DependPack");
                child.SetAttribute("PackName", depend.Key);
                child.SetAttribute("IsExist", depend.Value ? "1" : "0");
                e.AppendChild(child);
            }
            root.AppendChild(e);
        }
        XML.AppendChild(root);
        string filename = ResPath.LocalPath + ResPath.CurrentPathName + ResPath.PackDependFileName;
        ResUtil.DeleteFile(filename);
        XML.Save(filename);
    }
    //清除
    public void Clear()
    {
        InfoList.Clear();
        InfoList = null;
        XML.RemoveAll();
        XML = null;
    }
}
//包
public class GameResPackage
{
    //加载资源包
    public class AsyncLoadPackageData
    {
        public bool m_isFinish = false;

        public virtual void Reset()
        {
            m_isFinish = false;
        }
    }
    //加载资源数据
    public class AsyncLoadObjectData : AsyncLoadPackageData
    {
        public UnityEngine.Object m_obj = null;
        public override void Reset()
        {
            base.Reset();
            m_obj = null;
        }
    }
    public enum ENLoadType
    {
        enNone,
        enPreLoad,//预先加载
        enBattleLoad,//战斗加载
    }
    //包的名称
    public string PackName;
    //包加载的类型
    public ENLoadType m_loadType = ENLoadType.enNone;
    //包是否存在
    public bool m_isExist;
    //依赖包
    public List<GameResPackage> DependPack = new List<GameResPackage>();
    //www
    WWW m_wLoad = null;
    //
    AssetBundle m_assetbundle = null;
    //状态
    public enum ENState
    {
        enNone,
        enLoading,//加载中
        enSucced,//加载成功
        enFailed,//加载失败
    }
    ENState m_state = ENState.enNone;
    //是否可以清除的计数
    int m_canCleanCount = 0;
    public bool m_canClean { get { return m_canCleanCount == 0; } }
    //被使用的计数
    int m_beUsedCount = 0;
    //清除开始的时间
    public float m_delayCleanStartTime = 0;

    public GameResPackage(string packname, string tag, bool isExist = true)
    {
        PackName = packname;
        m_state = ENState.enNone;
        m_isExist = isExist;
        if (tag == "PreLoad")
        {
            m_loadType = ENLoadType.enPreLoad;
        }
    }
    //加载资源obj
    public UnityEngine.Object LoadObject(string path)
    {
        UnityEngine.Object obj = null;
        if (m_assetbundle != null)
        {
            if (!m_assetbundle.Contains(path))
            {
                Debug.LogWarning("asset bundle load null, path:" + path);
            }
            obj = m_assetbundle.LoadAsset(path);
            if (obj == null)
            {
                Debug.LogWarning("assetBundle is not contains filename:" + path);
            }
        }
        return obj;
    }
    //异步加载资源obj
    public IEnumerator LoadObjectAsync(string path, AsyncLoadObjectData data)
    {
        m_canCleanCount++;
        //load package
        AsyncLoadPackageData packageData = new AsyncLoadPackageData();
        IEnumerator e = LoadPackageAsync(packageData);
        while (true)
        {
            e.MoveNext();
            if (packageData.m_isFinish)
            {
                break;
            }
            yield return e.Current;
        }
        //load object
        if (m_state == ENState.enSucced)
        {
            if (m_assetbundle != null)
            {
                if (!m_assetbundle.Contains(path))
                {
                    Debug.LogWarning("asset bundle not contains res,name:" + path);
                }

                data.m_obj = m_assetbundle.LoadAsset(path);
                //
                if (m_loadType != ENLoadType.enPreLoad)
                {
                    GameResManager.Singleton.AddDelayCleanPackage(this);
                }
            }
            else
            {
                Debug.LogWarning("m_assetbundle is null, path:" + path);
            }
        }
        m_canCleanCount--;
        data.m_isFinish = true;
    }
    public IEnumerator LoadPackageAsync(AsyncLoadPackageData data)
    {
        m_beUsedCount++;
        if (m_state == ENState.enNone || m_state == ENState.enFailed)
        {
            m_state = ENState.enLoading;
            int dependFinishCount = 0;
            foreach (var item in DependPack)
            {//load depend
                AsyncLoadPackageData packageData = new AsyncLoadPackageData();
                IEnumerator e = item.LoadPackageAsync(packageData);
                while (true)
                {
                    e.MoveNext();
                    if (packageData.m_isFinish)
                    {
                        break;
                    }
                    yield return e.Current;
                }
                ++dependFinishCount;
            }
            if (m_isExist)
            {
                string url = ResPath.WLocalURLPrefix + ResPath.LocalPackagePath + PackName;
                if (m_wLoad == null)
                {
                    m_wLoad = new WWW(url);
                }
                while (!m_wLoad.isDone)
                {
                    yield return m_wLoad;
                }
                if (null != m_wLoad.error)
                {
                    Debug.LogWarning("WWW load fail, url:" + url + ",error:" + m_wLoad.error);
                    m_state = ENState.enFailed;
                    m_wLoad = null;
                }
                else
                {
                    m_assetbundle = m_wLoad.assetBundle;
                    if (m_assetbundle == null)
                    {
                        Debug.LogWarning("load package fail, assetbundle is null");
                    }
                    else
                    {
                        //UnityEngine.Object[] objList = m_assetbundle.LoadAll();
                        //foreach (var item in objList)
                        //{
                        //}
                        m_state = ENState.enSucced;
                        Debug.Log("load package succ, packname:" + PackName);
                    }
                }
            }
            else
            {
                while (true)
                {
                    if (dependFinishCount == DependPack.Count)
                    {
                        break;
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                m_state = ENState.enSucced;
                Debug.Log("load package is not exist, packname:" + PackName);
            }
        }
        if (m_state == ENState.enLoading)
        {
            while (true)
            {
                if (m_state != ENState.enLoading)
                {
                    break;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
        data.m_isFinish = true;
    }
    //卸载
    public void Unload()
    {
        try
        {
            if (m_wLoad != null)
            {
                if (m_wLoad.error == null)
                {
                    m_assetbundle.Unload(false);
                    //Debug.LogWarning("unload, packname:" + PackName);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("package clean fail, packname:" + PackName + ",error:" + e.Message);
        }
        m_assetbundle = null;
        m_wLoad = null;
        m_state = ENState.enNone;
    }
    //清除
    public void Clean()
    {
        if (m_loadType == ENLoadType.enPreLoad)
        {//预先加载的，不清除
            return;
        }
        m_beUsedCount--;
        if (m_beUsedCount <= 0)
        {
            foreach (var item in DependPack)
            {
                item.Clean();
            }
            Unload();
        }
    }
}