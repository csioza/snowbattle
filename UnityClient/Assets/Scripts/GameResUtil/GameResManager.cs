using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR //编辑模式
using UnityEditor;
#endif

//游戏资源管理器
public class GameResManager
{
    #region Singleton
    static GameResManager m_singleton = null;
    static public GameResManager Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new GameResManager();
            }
            return m_singleton;
        }
    }
    #endregion
    //入口
    public MonoBehaviour Entry { get; private set; }
    //资源更新
    GameResUpdate Update = new GameResUpdate();
    //所有文件名和GameResPackage的对应
    Dictionary<string, GameResPackage> AllGameResMap = new Dictionary<string, GameResPackage>();
    //所有包名字和GameResPackage的对应
    Dictionary<string, GameResPackage> AllPackageMap = new Dictionary<string, GameResPackage>();

    //需要下载的资源总量(单位：MB)
    float m_totalDownSize = 0;
    public float TotalDownSize
    {
        get
        {
            if (m_totalDownSize == 0)
            {
                m_totalDownSize = (float)Update.TotalSize / 1024 / 1024;
            }
            return m_totalDownSize;
        }
    }
    //下载资源的进度
    public float DownPercent { get { return Update.DownPercent; } }

    //是否使用package加载
    public bool UsePackage = false;
    //测试异步加载
    public bool TestAsync = false;

    //设置入口
    public void SetEntryAs(MonoBehaviour objEntry)
    {
        Entry = objEntry;
        Update.Entry = objEntry;
    }
    #region Update//更新、下载
    //CheckUpdate的回调函数，isUpdate为true时，有更新
    public delegate void CheckUpdateCallback(bool isUpdate);
    //检查更新
    public void CheckUpdate(string serverURL, CheckUpdateCallback callback)
    {
        //设置服务器
        ResPath.ServerResourceURL = serverURL;
        Update.CheckUpdate(callback);
    }
    //下载更新包
    public void DownloadUpdatePackage()
    {
        Update.DownloadUpdatePackage();
    }
    //下载更新失败的包
    public void RetryDownloadFailPackage()
    {
        Update.RetryDownloadFailPackage();
    }
    #endregion
    #region Build//构建资源Map
    //构建资源Map
    public void BuildGameResMap()
    {
        Update.LoadConfigFile();
        List<XML_PackMD5.Info> packMD5List = Update.GetPackMD5List();
        List<XML_FilePack.Info> filePackList = Update.GetFilePackList();
        List<XML_PackDepend.Info> packDependList = Update.GetPackDependList();
        foreach (var fileInfo in filePackList)
        {
            string tag = "";
            XML_PackMD5.Info md5Info = packMD5List.Find(item => item.PackName == fileInfo.PackName);
            if (md5Info != null)
            {
                tag = md5Info.Tag;
            }
            GameResPackage p = GetPackage(fileInfo.PackName, tag);

            XML_PackDepend.Info info = packDependList.Find(item => item.PackName == fileInfo.PackName);
            if (info != null)
            {//构建包的依赖
                foreach (var depend in info.DependPackMap)
                {
                    tag = "";
                    md5Info = packMD5List.Find(item => item.PackName == depend.Key);
                    if (md5Info != null)
                    {
                        tag = md5Info.Tag;
                    }
                    GameResPackage dependP = GetPackage(depend.Key, tag, depend.Value);
                    if (!p.DependPack.Contains(dependP))
                    {
                        p.DependPack.Add(dependP);
                    }
                }
            }
            else
            {
                Debug.LogWarning("pack depend is null, packname:" + fileInfo.PackName);
            }

            //add
            if (!AllGameResMap.ContainsKey(fileInfo.FileName.ToLower()))
            {
                AllGameResMap.Add(fileInfo.FileName.ToLower(), p);
            }
        }
        //构建完毕
        //清除AllPackageMap
        AllPackageMap.Clear();
        //
        //UsePackage = true;
    }
    //从AllPackageMap中获取一个GameResPackage
    GameResPackage GetPackage(string packname, string tag, bool isExist = true)
    {
        GameResPackage p = null;
        if (AllPackageMap.ContainsKey(packname))
        {
            AllPackageMap.TryGetValue(packname, out p);
            if (!isExist)
            {
                p.m_isExist = isExist;
            }
        }
        if (p == null)
        {
            p = new GameResPackage(packname, tag, isExist);
            AllPackageMap.Add(p.PackName, p);
        }
        return p;
    }
    #endregion
    #region Load//加载资源
    //异步加载资源，filename:Assets/Resources/之后的完整路径
    public UnityEngine.Object LoadResource(string filename)
    {
        UnityEngine.Object obj = null;
        if (UsePackage)
        {
            string resourcesPath = "Assets/Resources/" + filename;
            string extendsPath = "Assets/Extends/" + filename;
            string path = "";
            if (AllGameResMap.ContainsKey(resourcesPath.ToLower()))
            {
                path = resourcesPath;
            }
            else if (AllGameResMap.ContainsKey(extendsPath.ToLower()))
            {
                path = extendsPath;
            }
            else
            {
                int index = resourcesPath.LastIndexOf("/");
                if (index > 0)
                {
                    string dir = resourcesPath.Substring(0, index);
                    //Debug.Log("dir:" + dir);
                    if (AllGameResMap.ContainsKey(dir.ToLower()))
                    {
                        path = dir;
                    }
                }
            }
            if (!string.IsNullOrEmpty(path))
            {
                GameResPackage p = null;
                AllGameResMap.TryGetValue(resourcesPath.ToLower(), out p);
                if (p != null)
                {
                    obj = p.LoadObject(path.ToLower());
                }
            }
            else
            {
                //Debug.LogWarning("load res from package fail, filename:" + filename);
            }
        }
#if (UNITY_EDITOR) // 编辑模式下
        if (null == obj)
        {
            obj = AssetDatabase.LoadMainAssetAtPath("Assets/Extends/" + filename);
        }
        if (null == obj)
        {
            obj = AssetDatabase.LoadMainAssetAtPath("Assets/Resources/" + filename);
        }
#endif
        if (null == obj)    // 容错: 也许没有打包或用Unity打包
        {
            string szf = filename;  // 影射到 Resources/ 目录下
            int npos = szf.LastIndexOf('.');    // 去掉扩展名
            if (npos > 0) szf = szf.Substring(0, npos);
            obj = Resources.Load("Extends/" + szf);
            if (null == obj)
            {
                obj = Resources.Load(szf);
            }
        }
        if (null == obj)
        {
            Debug.LogWarning("load resource fail, filename:" + filename);
        }
        return obj;
    }
    IEnumerator SimulateWait()
    {
        //yield return new WaitForSeconds(1);
        yield return 1;
    }
    //异步加载资源，filename:Assets/之后的完整路径
    public IEnumerator LoadResourceAsync(string filename, GameResPackage.AsyncLoadObjectData data)
    {
        if (UsePackage)
        {
            string resourcesPath = "Assets/Resources/" + filename;
            string extendsPath = "Assets/Extends/" + filename;
            string path = "";
            if (AllGameResMap.ContainsKey(resourcesPath.ToLower()))
            {
                path = resourcesPath;
            }
            else if (AllGameResMap.ContainsKey(extendsPath.ToLower()))
            {
                path = extendsPath;
            }
            else
            {
                int index = resourcesPath.LastIndexOf("/");
                if (index > 0)
                {
                    string dir = resourcesPath.Substring(0, index);
                    //Debug.Log("dir:" + dir);
                    if (AllGameResMap.ContainsKey(dir.ToLower()))
                    {
                        path = dir;
                    }
                }
            }
            if (!string.IsNullOrEmpty(path))
            {
                GameResPackage p = null;
                AllGameResMap.TryGetValue(path.ToLower(), out p);
                if (p != null)
                {
                    UpdateDelayCleanPackageTime(p);
                    IEnumerator e = p.LoadObjectAsync(path.ToLower(), data);
                    while (true)
                    {
                        e.MoveNext();
                        if (data.m_isFinish)
                        {
                            break;
                        }
                        yield return e.Current;
                    }
                    //if (data.m_obj != null)
                    //{
                    //    Debug.Log("load res from package succ, filename:" + filename);
                    //}
                }
                else
                {
                    //Debug.LogWarning("load resource fail, package is null, file:" + filename);
                }
            }
            else
            {
                //Debug.LogWarning("load resource fail, resource is not exists, file:" + filename);
            }
        }
#if (UNITY_EDITOR) // 编辑模式下
        if (TestAsync)
        {
            IEnumerator eSimulate = SimulateWait();
            int testcount = 0;
            while (true)
            {
                eSimulate.MoveNext();
                testcount++;
                if (testcount > 1)
                {
                    break;
                }
                yield return eSimulate.Current;
            }
        }
        if (null == data.m_obj)
        {
            data.m_obj = AssetDatabase.LoadMainAssetAtPath("Assets/Extends/" + filename);
        }
        if (null == data.m_obj)
        {
            data.m_obj = AssetDatabase.LoadMainAssetAtPath("Assets/Resources/" + filename);
        }
#endif
        if (null == data.m_obj)    // 容错: 也许没有打包或用Unity打包
        {
            string szf = filename;  // 影射到 Resources/ 目录下
            int npos = szf.LastIndexOf('.');    // 去掉扩展名
            if (npos > 0) szf = szf.Substring(0, npos);
            data.m_obj = Resources.Load("Extends/" + szf);
            if (null == data.m_obj)
            {
                data.m_obj = Resources.Load(szf);
            }
        }
        if (null == data.m_obj)
        {
            Debug.LogWarning("load resource async fail, filename:" + filename);
        }
        data.m_isFinish = true;
    }
    //异步加载的回调函数
    public delegate void LoadResourceCallback(UnityEngine.Object res);
    #endregion
    #region Clean//清除无用的资源包
    List<GameResPackage> m_delayCleanPackage = new List<GameResPackage>();
    public void AddDelayCleanPackage(GameResPackage package)
    {
        GameResPackage delayPackage = m_delayCleanPackage.Find(item => item.PackName == package.PackName);
        if (delayPackage == null)
        {
            package.m_delayCleanStartTime = Time.realtimeSinceStartup;
            m_delayCleanPackage.Add(package);
        }
        else
        {
            delayPackage.m_delayCleanStartTime = Time.realtimeSinceStartup;
        }
    }
    public void UpdateDelayCleanPackageTime(GameResPackage package)
    {
        GameResPackage delayPackage = m_delayCleanPackage.Find(item => item.PackName == package.PackName);
        if (delayPackage != null)
        {
            delayPackage.m_delayCleanStartTime = Time.realtimeSinceStartup;
        }
    }
    public void CheckDelayCleanPackage()
    {
        bool isCleanOccur = false;
        for (int i = 0; i < m_delayCleanPackage.Count;)
        {
            GameResPackage package = m_delayCleanPackage[i];
            if (Time.realtimeSinceStartup > package.m_delayCleanStartTime + 1 && package.m_canClean)
            {
                package.Clean();
                isCleanOccur = true;
                m_delayCleanPackage.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
        if (isCleanOccur)
        {
            ClearUnusedRes();
        }
    }
    //清除不使用的资源
    public void ClearUnusedRes()
    {
        Resources.UnloadUnusedAssets();
    }
    #endregion

    public void FixedUpdate()
    {
        CheckDelayCleanPackage();
    }

    public void LoadAllResource()
    {
        Entry.StartCoroutine(Coroutine_LoadAll());
    }
    IEnumerator Coroutine_LoadAll()
    {
        foreach (var item in AllGameResMap)
        {
            GameResPackage.AsyncLoadPackageData data = new GameResPackage.AsyncLoadPackageData();
            IEnumerator e = item.Value.LoadPackageAsync(data);
            while (true)
            {
                e.MoveNext();
                if (data.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
        }
    }
    public bool m_isPreLoadFinish = false;
    //预先加载
    public void PreLoad()
    {
        m_isPreLoadFinish = false;
        Entry.StartCoroutine(Coroutine_PreLoad());
    }
    IEnumerator Coroutine_PreLoad()
    {
        foreach (var item in AllGameResMap)
        {
            if (item.Value.m_loadType != GameResPackage.ENLoadType.enPreLoad) continue;
            GameResPackage.AsyncLoadPackageData data = new GameResPackage.AsyncLoadPackageData();
            IEnumerator e = item.Value.LoadPackageAsync(data);
            while (true)
            {
                e.MoveNext();
                if (data.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
        }
        m_isPreLoadFinish = true;
    }
    public void CleanAll()
    {
        foreach (var item in AllGameResMap.Values)
        {
            item.Unload();
        }
    }

    public void DrawGui()
    {
        float fx = (Screen.width - 400) * 0.5f;
        float fy = (Screen.height - 95);
        float fProg = DownPercent*100;
        int bytesPersecond = 0;// GameResMng.GetResMng().GetDownloadBytesPerSecond();
        //float kbPerSecond = bytesPersecond / 1024.0f;
        GUIStyle st = new GUIStyle();
        st.fontSize = 24;
        st.normal.textColor = UnityEngine.Color.green;

        string msg = string.Format("資源包下載中 --- {0:###.##}%   {1}/{2} KB", fProg, Update.DownSize/1024,Update.TotalSize/1024);
        GUI.Label(new Rect(fx, fy, 400, 95), msg, st);
    }
}
