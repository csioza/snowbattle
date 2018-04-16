
#define MY_DEBUG        // 显示调试信息用

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


#if (UNITY_EDITOR)
    using UnityEditor;      // 编辑模式下可以读取 Assets/ 目录下 Unity3D 支持的任何文件
#endif



//-------------------------------------------------------------------------------------------------------------------
//
// 资源加载
//
// 说明:
//      <1> 程序启动时自动搜索打包资源
//      <2> 优先从资源包里读文件
//      <3> 随后尝试从 Resources/ 目录里读文件(编辑模式)
//      <4> 再之后尝试用 AssetDatabase 读取 Assets/ 目录下 Unity3D 支持的任何文件(编辑模式)
//      <5> 要加载资源必须是 Assets/ 之后的完整路径(含扩展名), 如:
//          实际文件: E:\Unity3D\XProject\trunk\Assets\Resources\Images\map.png 则
//          输入路径: Resources\Images\map.png
//      <6> 路径中 \ 自动被转成 /
// 
public class GameResMng
{
    static GameResMng mResMng = null;
    public static bool ForcePackage=false;
    public static bool DisableOtherResource = false;
    bool                            mbLocal = false;
    bool                            mbLoad;      // true: 加载结束
    static public string            mGmVer="";   // 游戏版本
    static public string            mReVer="";   // 资源版本
    MonoBehaviour                   mEntry;      // 启动线程用
    Dictionary<string, ResPackItm>  mResDic;     // 资源文件(相对路径) -- 所在资源包
    public Dictionary<string, ResPackge>   mPackDic;    // 已存资源包/场景包
    public PackageVersion m_versionPackage = new PackageVersion();
    public PackageVersion m_remotePackage = new PackageVersion();

    List<string> m_keepAlivePackage = new List<string>();


    // 测试用
    string mResTest;
    string mMsg;
    static public string DebugMsg = "";


    public bool IsReadyGo
    {
        get
        {
            return IsCheckPackage && GetCurProgress() >= 100;
        }
    }
    bool IsCheckPackage = false;
    //-------------------------------------------------------------------------------------------------------------------
    //
    // 对外接口
    //
    //--------------------------------------------------------------------------------------------
    // objEntry 用来启动 Coroutine 的
    static public GameResMng CreateResMng(MonoBehaviour objEntry)
    {
        if (mResMng == null)
        {
            mResMng = new GameResMng();
            mResMng.SetEntryAs(objEntry);   // 启动线程用
            //mResMng.StartWWW(false);
        }
        else
        {
            mResMng.SetEntryAs(objEntry);
        }
        return mResMng ;
    }

    //--------------------------------------------------------------------------------------------
    // 获取唯一实例
    static public GameResMng GetResMng(){return mResMng;}
    
    //--------------------------------------------------------------------------------------------
    static public void SetLoading()
    {
        GameResMng pMng = GetResMng();
        if (pMng != null) pMng.mbLoad = false;
    }

    //--------------------------------------------------------------------------------------------
    // szfile要求: Assets/ 之后的完整路径(含扩展名), 示例参考顶部说明
    static public UnityEngine.Object LoadResource(string szfileWithoutResources)
    {
        UnityEngine.Object obj = null;

        GameResMng pMng = GetResMng();
        if (null != pMng)
        {
            obj = pMng.NtfLoadResource(szfileWithoutResources);
        }
#if (UNITY_EDITOR) // 编辑模式下
        if (null == obj)
        {
            //Debug.LogWarning("Load Resource Failed:" + szfileWithoutResources);
        	//return obj;
        }
#endif

#if (UNITY_EDITOR) // 编辑模式下
        if (null == obj && !ForcePackage)
        {
            obj = AssetDatabase.LoadMainAssetAtPath("Assets/Extends/" + szfileWithoutResources);
        }
        if (null == obj)
        {
            obj = AssetDatabase.LoadMainAssetAtPath("Assets/Resources/" + szfileWithoutResources);
        }
#endif
        if (null == obj)    // 容错: 也许没有打包或用Unity打包
        {
            string szf = szfileWithoutResources;  // 影射到 Resources/ 目录下
            int npos = szf.LastIndexOf('.');                 // 去掉扩展名
            if (npos > 0) szf = szf.Substring(0, npos);
            obj = Resources.Load("Extends/" + szf);
            if (null == obj)
            {
                obj = Resources.Load(szf);
            }
        }

        
		if(null == obj)
		{
            Debug.LogWarning("Load Resource Failed:" + szfileWithoutResources);
		}
        return obj;
    }
    IEnumerator SimulateWait()
    {
        //yield return new WaitForSeconds(1);
        yield return 1;
    }
    //--------------------------------------------------------------------------------------------
    // szfile要求: Assets/ 之后的完整路径(含扩展名), 示例参考顶部说明
    public IEnumerator LoadResourceAsync(string szfileWithoutResources, ResPackge.AsyncLoadData data)
    {
        string szfile = "resources/" + szfileWithoutResources;
        ResPackge pck = null;
		bool isInPackage=false;
        if (mResDic != null)
        {
            ResPackItm itm = null;
            if (mResDic.TryGetValue(szfile.ToLower(), out itm))
            {
				isInPackage=true;
                if (mPackDic != null)
                {
                    if (mPackDic.TryGetValue(itm.mPack, out pck))
                    {
                        
                    }
                }
            }
            if (pck == null)
            {
                szfile = "Extends/" + szfileWithoutResources;
                itm = null;
                if (mResDic.TryGetValue(szfile.ToLower(), out itm))
                {
                    isInPackage = true;
                    if (mPackDic != null)
                    {
                        if (mPackDic.TryGetValue(itm.mPack, out pck))
                        {

                        }
                    }
                }
            }
        }
        if (pck != null)
        {
            UpdateDelayCleanPackageTime(pck);
            IEnumerator e = pck.LoadObjectAsync(szfile, mEntry, data);
            bool interrupted = false;
            data.AsyncCount++;
            while (!interrupted)
            {
                e.MoveNext();
                if (data.IsFinish)
                {
                    break;
                }
                yield return e.Current;
            }
            data.FinishCount++;
        }
        //////////////////////////////////////////////////////////////////////////
        //Simulate
#if (UNITY_EDITOR) // 编辑模式下
        if (GameResMng.ForcePackage && DisableOtherResource)
        {
            IEnumerator eSimulate = SimulateWait();
            data.AsyncCount++;
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
            data.FinishCount++;
            data.IsFinish = true;
        }
#endif
        //////////////////////////////////////////////////////////////////////////
#if (UNITY_EDITOR) // 编辑模式下
        if (null == data.m_res && !DisableOtherResource && !ForcePackage)
        {
            data.m_res = AssetDatabase.LoadMainAssetAtPath("Assets/Extends/" + szfileWithoutResources);
        }
        if (null == data.m_res )
        {
            data.m_res = AssetDatabase.LoadMainAssetAtPath("Assets/Resources/" + szfileWithoutResources);
        }
#endif
        if (data.m_res == null)    // 容错: 也许没有打包或用Unity打包
        {
            string szf = szfileWithoutResources;  // 影射到 Resources/ 目录下
            int npos = szf.LastIndexOf('.');                 // 去掉扩展名
            if (npos > 0) szf = szf.Substring(0, npos);
#if (UNITY_EDITOR) // 编辑模式下
            if (!DisableOtherResource)
            {
                data.m_res = Resources.Load("Extends/" + szf);
            }
#else
            data.m_res = Resources.Load("Extends/" + szf);
#endif
            if (null == data.m_res)
            {
                data.m_res = Resources.Load(szf);
            }
        }

        data.IsFinish = true;
        if (null == data.m_res)
        {
//			string msg = "";
			if(!isInPackage)
			{
            	Debug.LogWarning("Load Resource Failed:" + szfile);
			}
			else{
            	Debug.LogWarning("[Package]Load Resource Failed:" + szfile);
			}
        }
    }
    public delegate void LoadResourceCallback(UnityEngine.Object res);
    public void LoadResourceAsyncCallback(string szfile, LoadResourceCallback callback)
    {
        mEntry.StartCoroutine(LoadResourceAsyncCallback_Impl(szfile, callback));
    }
    IEnumerator LoadResourceAsyncCallback_Impl(string szfile, LoadResourceCallback callback)
    {
        ResPackge.AsyncLoadData data = new ResPackge.AsyncLoadData();
        IEnumerator e = LoadResourceAsync(szfile, data);
        while (true)
        {
            e.MoveNext();
            if (data.IsFinish)
            {
                break;
            }
            yield return e.Current;
        }
        callback(data.m_res);
    }
    public IEnumerator LoadSceneAsync(string sceneName,ResPackge.AsyncLoadData data,Action callback=null)
    {
        bool isLoaded=false;
        string szNm = Path.GetFileNameWithoutExtension(sceneName).ToLower();
        if (mPackDic != null)
        {
            ResPackItm itm = null;
            if (mResDic.TryGetValue(szNm + ".unity", out itm))
            {
                ResPackge pck = null;
                if (mPackDic.TryGetValue(itm.mPack, out pck))
                {
                    IEnumerator e = pck.LoadPackageAsync(data);
                    while (true)
                    {
                        e.MoveNext();
                        if (data.IsFinish)
                        {
                            break;
                        }
                        yield return e.Current;
                    }
                    isLoaded = pck.LoadLevel(sceneName);
                    AddDelayCleanPackage(pck);
                }
            }
        }
        data.IsFinish = true;
        if (!isLoaded)    // 也许没有打包或用Unity打包
        {
            string unitySceneName = Path.GetFileNameWithoutExtension(sceneName);
            Application.LoadLevel(unitySceneName.ToLower());
        }
        if (callback != null)
        {
            callback();
        }
    }
    //--------------------------------------------------------------------------------------------
    // szName 带路径但 不需要扩展名
    public void LoadSceneCallback(string szName,Action callback)
    {
        GameResMng pMng = GetResMng();
        if (null != pMng)
        {
            ResPackge.AsyncLoadData data = new ResPackge.AsyncLoadData();
            mEntry.StartCoroutine(pMng.LoadSceneAsync(szName, data, callback));
        }
    }
    
    //--------------------------------------------------------------------------------------------
    // 清理未使用的资源
    static public void ClearUnusedRes()
    {
        GameResMng pMng = GameResMng.GetResMng();
        if(null != pMng) pMng.NtfClearUnusedRes();
    }
    
    //--------------------------------------------------------------------------------------------
    // 统计资源包的总体加载进度[0, 100]
    static public float GetProgress()
    {
        GameResMng pMng = GameResMng.GetResMng();
        if(null != pMng) return pMng.GetCurProgress();
        return 100.0f;
    }
    
    //--------------------------------------------------------------------------------------------
    static public bool IsLoaded()
    {
        GameResMng pMng = GameResMng.GetResMng();
        if(null != pMng) return pMng.IsLoadOver();
        return true;
    }

    static public void DrawGui()
    {
        float fx = (Screen.width - 400) * 0.5f;
        float fy = (Screen.height - 95);
        float fProg = GameResMng.GetProgress();
        int bytesPersecond = GameResMng.GetResMng().GetDownloadBytesPerSecond();
        float kbPerSecond = bytesPersecond / 1024.0f;
        GUIStyle st = new GUIStyle();
        st.fontSize = 24;
        st.normal.textColor = UnityEngine.Color.black;
        if (GameResMng.GetResMng().m_failedDownloadPackage.Count != 0
            && GameResMng.GetResMng().IsDownloadQueueEmpty)
        {
        }
        else
        {
             string msg = string.Format("資源包下載中 --- {0:###.##}%    {1:.##}KB/S", fProg, kbPerSecond);
             GUI.Label(new Rect(fx, fy, 400, 95), msg, st);
        }
    }





    
    //--------------------------------------------------------------------------------------------
    protected GameResMng()
    {
        mMsg = "";
        mGmVer = "";      // 游戏版本
        mReVer = "";      // 资源版本
        mResTest= "";
        mbLoad  = false;
        if (!Application.isEditor && !ForcePackage)
            ResUtil.GetVersion(ref mGmVer, ref mReVer);
    }
    
    //--------------------------------------------------------------------------------------------
    public void SetEntryAs(MonoBehaviour objEntry)
    {
        mEntry = objEntry;
    }
    public void AddKeepAlivePackage(string package,bool startOpenImm=true)
    {
        for (int i = 0; i < m_keepAlivePackage.Count;i++ )
        {
            if (m_keepAlivePackage[i] == package)
            {
                return;
            }
        }
        ResPackge pack = GetPackge(package);
        if (pack != null)
        {
            pack.m_isKeepAlive = true;
            m_keepAlivePackage.Add(package);
            if (!pack.IsDone() && startOpenImm)
            {
                mEntry.StartCoroutine(pack.TryDownload(false,false));
            }
        }
    }
    public void RemoveKeepAlivePackage(string package)
    {
        for (int i = 0; i < m_keepAlivePackage.Count; i++)
        {
            if (m_keepAlivePackage[i] == package)
            {
                ResPackge pack = GetPackge(m_keepAlivePackage[i]);
                pack.Clean();
                pack.m_isKeepAlive = false;
                m_keepAlivePackage.RemoveAt(i);
                return;
            }
        }
    }
    public bool IsKeepAlivePackage(string package)
    {
        for (int i = 0; i < m_keepAlivePackage.Count; i++)
        {
            if (m_keepAlivePackage[i] == package)
            {
                return true;
            }
        }
        return false;
    }
    //--------------------------------------------------------------------------------------------
    public void StartWWW(bool bLocal)
    {
        IsCheckPackage = false;
        if (!Application.isEditor || ForcePackage)
        {
            mbLocal = bLocal;
            //if(!mbLocal) ResPath.ClearLocal();          // --------------先清理了
            if (mbLocal)
            {
                if (!ResPath.LocalResExist())
                {
                    DebugMsg = @"本地资源不存在";
                    //return;
                    mbLocal = false;
                }
            }
            mEntry.StartCoroutine(NtfDownLoadVer());    // 同步版本/资源列表
        }
        else
        {
            IsCheckPackage = true;
        }
    }
        
    //--------------------------------------------------------------------------------------------
    // 首先下载游戏版本
	IEnumerator NtfDownLoadVer()
	{
		if (!mbLocal)
		{
			string szfNm = ResPath.GetVersionTxt();
			string szUrl = ResPath.GetUrl(szfNm);
			string szLcl = ResPath.GetLocal(szfNm);
			mMsg = "Download ver file = " + szUrl;
            Debug.Log("StartDownload VerFile:" + szUrl);
			WWW wDwn = new WWW(szUrl);
            while (!wDwn.isDone)
            {
                yield return wDwn;
            }

			if (wDwn.isDone && (null == wDwn.error))
			{
				byte[] localVersionBuffer = ResPath.GetFileData(szLcl);
				if (localVersionBuffer != null)
				{
					ResUtil.GetVersion(localVersionBuffer, ref mGmVer, ref mReVer);
				}

				string sGmV = "", sReV = "";
				if (ResUtil.GetVersion(wDwn.bytes, ref sGmV, ref sReV))
				{
					mGmVer = sGmV;
					if (mReVer != sReV)
					{
						mReVer = sReV;
						mbLocal = false;
						ResPath.SaveToLocal(szLcl, wDwn.bytes);
					}
				}
			}
			else if (null != wDwn.error)
			{
				mMsg = "DownVer ERROR, Msg = " + wDwn.error;
				Debug.LogWarning("DownVer ERROR, Url = " + szUrl + ", Msg = " + wDwn.error);
				IsCheckPackage = true;
				mbLocal = true;
			}
		}
        m_versionPackage.LoadPackagesVersion(ResPath.GetLocal(ResPath.GetPackageVersionTxt()));
        if (!mbLocal)
        {//版本已经更新，需要重新下载包列表
            string pckVersionFileName = ResPath.GetPackageVersionTxt();
            string pckVersionFileUrl = ResPath.GetUrl(pckVersionFileName);
            string pckVersionFileLocal = ResPath.GetLocal(ResPath.GetRemotePackageVersionTxt());
            Debug.Log("StartDownload File Package Version List:" + pckVersionFileUrl);
            WWW wDownloadPckVersionFilr = new WWW(pckVersionFileUrl);
            while (!wDownloadPckVersionFilr.isDone)
            {
                yield return wDownloadPckVersionFilr;
            }
            if (wDownloadPckVersionFilr.isDone && (null == wDownloadPckVersionFilr.error))
            {

            }
            else
            {
                IsCheckPackage = true;
            }
            if (wDownloadPckVersionFilr.isDone && (null == wDownloadPckVersionFilr.error))
            {
                ResPath.SaveToLocal(pckVersionFileLocal, wDownloadPckVersionFilr.bytes);
            }
            else if (null != wDownloadPckVersionFilr.error)
            {
                mMsg = "DownPkgVer ERROR, Msg = " + wDownloadPckVersionFilr.error;
                Debug.LogWarning("DownPkgVer ERROR, Url = " + pckVersionFileUrl + ", Msg = " + wDownloadPckVersionFilr.error);
            }
            m_remotePackage.LoadPackagesVersion(pckVersionFileLocal);
        }
        NtfDownLoadResList(mbLocal);
    }
    
    //--------------------------------------------------------------------------------------------
    // 下载需要的资源列表 [ 每次启动都要同步 ]
    void NtfDownLoadResList(bool bLcl)
    {
        if (Application.isEditor && !ForcePackage)
        {
            return;   // 编辑器直接读本地资源
        }
        SetLoading();
        mMsg = @"正在同步资源列表, ...";
        string szfNm = ResPath.GetResListTxt();
        string szUrl = ResPath.GetUrl(szfNm);
        string szLcl = ResPath.GetLocal(szfNm);
        if (bLcl)
        {
            mMsg = @"从本地读取资源列表, ...";
            Debug.LogWarning("Try local0, File = " + szLcl);
            mResDic = ResUtil.ReadResTable(szLcl);
            //if (mResDic != null)
            {// 构建资源与数据包的对应关系并下载
                BuildPackListAndDownLoad();   
            }
            return;
        }
        mMsg = @"从网络读取资源列表, ...";
        mEntry.StartCoroutine(DownLoadResList(szUrl, szLcl));
    }
    void MergePakcageList()
    {
        MergePakcageList(m_remotePackage);
        MergePakcageList(m_versionPackage);
    }
    void MergePakcageList(PackageVersion pkgVersion)
    {
        foreach (KeyValuePair<string, PackageVersion.Version> p in pkgVersion.m_versions)
        {
            if (!mPackDic.ContainsKey(p.Key))
            {
                ResPackge pck = new ResPackge();
                pck.mType = 0;
                pck.mFile = p.Key;
                mPackDic.Add(pck.mFile, pck);
            }
        }
    }
    
    //--------------------------------------------------------------------------------------------
    // 下载资源列表文件
    IEnumerator DownLoadResList(string szUrl, string szLcl)
    {
        mMsg = "Download file " + szUrl;
        WWW wDwn = new WWW(szUrl);
		while(!wDwn.isDone) yield return wDwn;
        if (wDwn.isDone && (null == wDwn.error))
        {
            mResDic = ResUtil.ReadResTable(wDwn.bytes);
            if((mResDic != null) && (mResDic.Count > 0))
                ResPath.SaveToLocal(szLcl, wDwn.bytes);
        }
        else if (null != wDwn.error)
        {
            mMsg = "DownResList ERROR, Msg = " + wDwn.error;
            Debug.LogWarning("DownResList ERROR, Url = " + szUrl + ", Msg = " + wDwn.error);
        }
        if ((mResDic == null) || (mResDic.Count <= 0))
        {
            Debug.LogWarning("Try local1, File = " + szLcl);
            mResDic = ResUtil.ReadResTable(szLcl);
        }
        BuildPackListAndDownLoad();   // 构建资源与数据包的对应关系并下载
    }

    public struct NeedDownloadData 
    {
        public ResPackge m_pck;
        public bool m_isKeepAlive;
        public bool m_forceDownload;
    }

    List<List<NeedDownloadData>> m_needDownloadPackage = new List<List<NeedDownloadData>>();
    public List<NeedDownloadData> m_failedDownloadPackage = new List<NeedDownloadData>();
    //--------------------------------------------------------------------------------------------
    // 构建资源列表
	void BuildPackListAndDownLoad()
	{
		mEntry.StartCoroutine(BuildPackListAndDownLoad_Impl());
	}
    IEnumerator BuildPackListAndDownLoad_Impl()
    {
        int downloadQueueCount = 1;
        for (int i = m_needDownloadPackage.Count; i < downloadQueueCount; i++)
        {
            m_needDownloadPackage.Add(new List<NeedDownloadData>());
        }
        string szLast = "";
        mMsg = "Build Pack List ...";
        mPackDic = new Dictionary<string, ResPackge>();
        if (mResDic != null)
        {
            foreach (ResPackItm itm in mResDic.Values)
            {
                if (szLast == itm.mPack) continue;
                szLast = itm.mPack;
//                string szNm = Path.GetFileName(itm.mPack);
                if (mPackDic.ContainsKey(itm.mPack)) continue;
                ResPackge pck = new ResPackge();
                pck.mType = itm.mType;
                pck.mFile = itm.mPack;
                mPackDic.Add(itm.mPack, pck);
            }
        }
        MergePakcageList();

        mMsg = "Down Pack Files ...";
        int downloadIndex = 0;
        foreach (KeyValuePair<string, ResPackge> res in mPackDic)
        {
            ResPackge pck = res.Value;
            mMsg = "DownOrLoad ... " + pck.mFile;
            //mEntry.StartCoroutine(pck.NtfDownOrLoad(mbLocal));
            bool forceDownload = false;
            bool keepAlive = false;
            if (!m_versionPackage.HavePackage(pck.mFile))
            {
                forceDownload = true;
            }
            else
            {
                PackageVersion.Version localVersion = m_versionPackage.LookupPackageVersion(pck.mFile);
                if (localVersion.m_isAliveInRuntime)
                {
                    keepAlive = true;
                }
                if (m_remotePackage.HavePackage(pck.mFile))
                {
                    PackageVersion.Version remoteVersion = m_remotePackage.LookupPackageVersion(pck.mFile);
                    if (remoteVersion.m_version != localVersion.m_version)
                    {
                        forceDownload = true;
                    }
                }
            }

            if (m_remotePackage.HavePackage(pck.mFile))
            {
                PackageVersion.Version remoteVersion = m_remotePackage.LookupPackageVersion(pck.mFile);
                if (remoteVersion.m_isAliveInRuntime)
                {
                    keepAlive = true;
                }
            }
            if (keepAlive)
            {
                AddKeepAlivePackage(pck.mFile, false);
            }
            keepAlive = IsKeepAlivePackage(res.Key);
            //if (forceDownload || keepAlive)
            {
                NeedDownloadData data = new NeedDownloadData();
                data.m_forceDownload = forceDownload;
                data.m_isKeepAlive = keepAlive;
                data.m_pck = pck;
                List<NeedDownloadData> downQueue = m_needDownloadPackage[downloadIndex % m_needDownloadPackage.Count];
                downQueue.Add(data);
                downloadIndex++;
            }
            //else
            //{
            //    IEnumerator e = pck.TryDownload(!keepAlive, forceDownload);
            //    while (true)
            //    {
            //        e.MoveNext();
            //        if (pck.m_state != ResPackge.State.Downloading)
            //        {
            //            break;
            //        }
            //        yield return e.Current;
            //    }
            //}
        }
        IsCheckPackage = true;
        mMsg = "";
        m_calcDownloadBytesTime = Time.realtimeSinceStartup;
        for (int i = 0; i < m_needDownloadPackage.Count;i++ )
        {
            List<NeedDownloadData> downQueue = m_needDownloadPackage[i];
            if (downQueue.Count>0)
            {
                mEntry.StartCoroutine(ProcDownloadQueue(i));
				mEntry.StartCoroutine(OverTimeCheck());
            }
        }

#if(MY_DEBUG) // 显示出来
        AddReToViewCtrl();
#endif
        yield return 1;
    }

	int now_dl_index = 0;
	int flag_indx = 0;
	int flag_time = 0;
	public bool IsOverTime = false;
	public bool IsDlOver = true;
	public int err_pack_times = 0;
	public bool isDlFailed = false;

    IEnumerator ProcDownloadQueue(int index)
    {
        List<NeedDownloadData> downQueue = m_needDownloadPackage[index % m_needDownloadPackage.Count];
		now_dl_index = 0;
		err_pack_times = 0;
		isDlFailed = false;
        foreach (NeedDownloadData d in downQueue)
        {
			now_dl_index++;
			flag_time = 0;
            IEnumerator e = d.m_pck.TryDownload(!d.m_isKeepAlive, d.m_forceDownload);			
            while (true)
            {
                e.MoveNext();
                if (d.m_pck.m_state != ResPackge.State.Downloading)
                {
                    break;
                }
                yield return e.Current;
            }
			if (err_pack_times == 5)
			{
				isDlFailed = true;
				break;
			}
        }
        downQueue.Clear();
    }
	IEnumerator OverTimeCheck()
	{
		IsDlOver = false;
		while (true)
		{
			if (flag_indx != now_dl_index)
			{
				flag_indx = now_dl_index;
				flag_time = 0;
			}
			yield return new WaitForSeconds(1);
			flag_time++;
			//Debug.Log("===============dl time :" + flag_time);
			if (flag_time >= 60)
			{
				IsOverTime = true;
				flag_time = 0;
			}
			if (IsDlOver)
			{
				break;
			}
		}
	}
    public int m_downloadBytes = 0;
    public float m_calcDownloadBytesTime = 0;
    public void OnPackageDownload(string pckFile,int bytes)
    {
        m_downloadBytes += bytes;
        PackageVersion.Version v = new PackageVersion.Version();
        v.Init();
        if (m_versionPackage.HavePackage(pckFile))
        {
            v = m_versionPackage.LookupPackageVersion(pckFile);
        }
        if (m_remotePackage.HavePackage(pckFile))
        {
            v = m_remotePackage.LookupPackageVersion(pckFile);
        }
        m_versionPackage.AddPackage(pckFile, v);

        m_versionPackage.SavePackageVersion(ResPath.GetLocal(ResPath.GetPackageVersionTxt()));
    }
    public void OnPackageDownloadFailed(string pckFile)
    {
        NeedDownloadData data = new NeedDownloadData();
        data.m_forceDownload = true;
        data.m_isKeepAlive = IsKeepAlivePackage(pckFile);
        data.m_pck = GetPackge(pckFile);
        m_failedDownloadPackage.Add(data);
		//失败次数+1
		if (!isDlFailed)
		{
			err_pack_times++;
		}
    }
    public void RetryFailedDownload()
    {
        int downloadIndex = 0;
        for (int i = 0; i < m_failedDownloadPackage.Count;i++ )
        {
            NeedDownloadData data = m_failedDownloadPackage[i];
            List<NeedDownloadData> downQueue = m_needDownloadPackage[downloadIndex % m_needDownloadPackage.Count];
            downQueue.Add(data);
            downloadIndex++;
        }
        m_failedDownloadPackage.Clear();

        m_calcDownloadBytesTime = Time.realtimeSinceStartup;
        for (int i = 0; i < m_needDownloadPackage.Count; i++)
        {
            List<NeedDownloadData> downQueue = m_needDownloadPackage[i];
            if (downQueue.Count > 0)
            {
                mEntry.StartCoroutine(ProcDownloadQueue(i));
            }
        }
    }
    public bool IsDownloadQueueEmpty
    {
        get
        {
            for (int i = 0; i < m_needDownloadPackage.Count; i++)
            {
                List<NeedDownloadData> downQueue = m_needDownloadPackage[i];
                if (downQueue.Count > 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public bool IsAllKeepAlivePackageLoaded()
    {
        for (int i = 0; i < m_keepAlivePackage.Count; i++)
        {
            string pkgName = m_keepAlivePackage[i];
            ResPackge cfgPackage = GetPackge(pkgName);
            if (cfgPackage != null)
            {
                if (!cfgPackage.IsDone())
                {
                    return false;
                }
            }
        }
        return true;
    }
    //--------------------------------------------------------------------------------------------
    public bool IsPacksLoading()
    {
        if (mPackDic != null)
        {
            foreach (KeyValuePair<string, ResPackge> pck in mPackDic)
            {
                if (pck.Value.GetState() == ResPackge.State.Downloading || pck.Value.GetState() == ResPackge.State.None)
                    return true;
            }
        }
        return false;
    }

    //--------------------------------------------------------------------------------------------
    void CalProgress(ref float fv, Dictionary<string, ResPackge> dic, float fs)
    {
        
    }
    //--------------------------------------------------------------------------------------------
    // 统计资源包的总体加载进度[0, 100]
    float GetCurProgress()
    {
        if (!IsLoadOver())
        {
            if (mPackDic.Count == 0)
            {
                return 100.0f;
            }
            float fv = 0.0f;
            foreach (ResPackge pck in mPackDic.Values)
            {
                fv += pck.GetProgress();
            }
            fv /= mPackDic.Count;
            float per = fv * 100.0f;
            if (per >= 99.9f)
            {
                per -= 1.0f;
            }
            return per;
        }
        return 100.0f;
    }
    int m_curDownloadBytePerSecond = 0;
    public int GetDownloadBytesPerSecond()
    {
        float curTime = Time.realtimeSinceStartup;
        //if(false)// (curTime - m_calcDownloadBytesTime>2)
        //{
        //    m_curDownloadBytePerSecond = (int)((float)m_downloadBytes / (curTime - m_calcDownloadBytesTime));
        //    m_downloadBytes = 0;
        //    m_calcDownloadBytesTime = Time.realtimeSinceStartup;
        //}
        //else
        {
            m_curDownloadBytePerSecond = (int)((float)m_downloadBytes / (curTime - m_calcDownloadBytesTime));    
        }
        return m_curDownloadBytePerSecond;
    }
    
    //--------------------------------------------------------------------------------------------
    int CalLoadedCount(Dictionary<string, ResPackge> dic)
    {
        int nNum = 0;
        foreach (ResPackge pck in dic.Values)
        {
            //|| pck.GetState() == ResPackge.State.Failed
            if (pck.GetState() == ResPackge.State.LocalReady)
            {
                ++nNum;
            }
        }
        return nNum;
    }

    //--------------------------------------------------------------------------------------------
    bool IsLoadOver()
    {
        if ((!mbLoad) && (mPackDic != null))
        {
            int nNum = CalLoadedCount(mPackDic);
            mbLoad = (nNum == mPackDic.Count);
        }
        else mbLoad = true;
        return mbLoad;
    }
    
    //--------------------------------------------------------------------------------------------
    // 从包里加载资源
    UnityEngine.Object NtfLoadResource(string szfileWithoutResources)
    {
        string szfile;
        if (mResDic != null)
        {
            ResPackItm itm = null;
            szfile = "Extends/" + szfileWithoutResources;
            if (mResDic.TryGetValue(szfile.ToLower(), out itm))
            {
                if (mPackDic != null)
                {
                    ResPackge pck = null;
                    if (mPackDic.TryGetValue(itm.mPack, out pck))
                        return pck.LoadObject(szfile);
                }
            }
            szfile = "resources/" + szfileWithoutResources;
            if (mResDic.TryGetValue(szfile.ToLower(), out itm))
            {
                if (mPackDic != null)
                {
                    ResPackge pck = null;
                    if (mPackDic.TryGetValue(itm.mPack, out pck))
                        return pck.LoadObject(szfile);
                }
            }
        }

        return null;
    }
    
    //--------------------------------------------------------------------------------------------
    // 从包里加载场景
    bool NtfLoadScene(string szScene)
    {
        if (mPackDic != null)
        {
            ResPackItm itm = null;
            string szNm = Path.GetFileNameWithoutExtension(szScene).ToLower();
            if (mResDic.TryGetValue(szNm + ".unity", out itm))
            {
                ResPackge pck = null;
                if (mPackDic.TryGetValue(itm.mPack, out pck))
                    return pck.LoadLevel(szNm);
            }
        }
        return false;
    }

    public ResPackge GetPackge(string sPck)
    {
        if(mPackDic != null)
        {
            ResPackge pck = null;
            if (mPackDic.TryGetValue(sPck, out pck))
                return pck;
        }
        return null;
    }
    
    //--------------------------------------------------------------------------------------------
    public void NtfClearUnusedRes()
    {
        Resources.UnloadUnusedAssets();
    }
    
    //--------------------------------------------------------------------------------------------
    public bool IsReady()
    {
        if ((mPackDic != null) && (mResDic != null))
            return true;
        return false;
    }
    
    //--------------------------------------------------------------------------------------------
    public Dictionary<string, ResPackItm> GetSceneItms()
    {
        Dictionary<string, ResPackItm> vAry = new Dictionary<string, ResPackItm>();
        if (mResDic != null)
        {
            foreach (ResPackItm itm in mResDic.Values)
            {
                if (itm.mType != 1) continue;
                string sf = Path.GetFileNameWithoutExtension(itm.mFile);
                if (!vAry.ContainsKey(sf))
                {
                    vAry.Add(sf, itm);
                }
            }
        }
        return vAry;
    }

    void AddReToViewCtrl()
    {
#if(MY_DEBUG) // 显示出来
        UIListViewCtrl LvCtrl = m_lvCtrl;
        if (LvCtrl == null)
        {
            return;
        }
        LvCtrl.ClearLvItems();
        LvItm lvHdr = LvCtrl.AddLvLabel( @"调试信息", Color.red);
        lvHdr.mfont = 24;
        lvHdr.mAnchor = TextAnchor.MiddleCenter;

        LvColGrp iGrp = LvCtrl.AddLvColGrp();

        //LvColGrp iGrp0 = iGrp.AddLvColGrp(0);
        string szVer = @"游戏版本:" + GameResMng.mGmVer;
        iGrp.AddLvLabel( szVer, 200, Color.green);

        szVer = @"资源版本:" + GameResMng.mReVer;
        iGrp.AddLvLabel( szVer, 200, Color.blue);

        //LvColGrp iGrp1 = iGrp.AddLvColGrp(2);
        //lvHdr = iGrp1.AddLvBtn(@"网络资源", 80, Color.blue);
        //lvHdr.mAnchor = TextAnchor.MiddleCenter;
        
        //lvHdr = iGrp1.AddLvBtn(@"本地资源", 80, Color.blue);
        //lvHdr.mAnchor = TextAnchor.MiddleCenter;

        iGrp.UpdateDrawH();
               

        LvCtrl.AddLvEmpty(5);
        
        LvCtrl.AddLvLabel( @"运行信息:", Color.green);

        LvItmGrp itmGrp = LvCtrl.AddLvItmGrp( @"搜索到的资源包:", Color.green);
        //itmGrp.AddLvLabel("asdf", 0, Color.white);
        //itmGrp.AddLvLabel("234234asdf", 0, Color.white);
        if (null != mPackDic)
        {
            foreach (ResPackge pck in mPackDic.Values)
            {
                Debug.Log("pck.mType = " + pck.mType.ToString());
                if (pck.mType == 0)
                {
                    itmGrp.AddLvLabelEx(pck);
                }
            }
        }
        
        itmGrp = LvCtrl.AddLvItmGrp( @"搜索到的场景:", Color.green);
        //itmGrp.AddLvLabel("asdf", 0, Color.white);
        //itmGrp.AddLvLabel("234234asdf", 0, Color.white);
        if (null != mPackDic)
        {
            Dictionary<string, ResPackItm> vDic = GetSceneItms();
            foreach (ResPackItm itm in vDic.Values)
            {
                ResPackge pck = GetPackge(itm.mPack);
                string sf = Path.GetFileNameWithoutExtension(itm.mFile);
                itmGrp.AddLvBtnEx( @"Load " + sf, pck);
            }
        }
#endif
    }

    //--------------------------------------------------------------------------------------------
    // szName 带路径但 不需要扩展名
    static public void TestLoadScene(LvItm itm, UIListViewCtrl frm)
    {
        bool bLoad = false;
        LvItmBtnEx itx = itm as LvItmBtnEx;
        if(itx != null)
        {
            ResPackge pck = itx.mRefPck;
            if (pck != null)
            {
                string szName = Path.GetFileNameWithoutExtension(pck.mFile);
                GameResMng pMng = GetResMng();
                if (null != pMng)
                    bLoad = pMng.NtfLoadScene(szName);
                if (!bLoad)    // 也许没有打包或用Unity打包
                {
                    string szNm = Path.GetFileNameWithoutExtension(szName);
                    Application.LoadLevel(szNm.ToLower());
                }
            }
        }
    }




















    







    
    //---------------------------------------------------------------------------------------------------
    //
    // 测试用
    //
    static public void OnResMngGUI()
    {
        GameResMng pMng = GameResMng.GetResMng();
        if(null != pMng) pMng.NtfOnResMngGUI();
    }
    public UIListViewCtrl m_lvCtrl;
    void NtfOnResMngGUI()
    {
        
#if(MY_DEBUG) // 显示出来
        UIListViewCtrl LvCtrl = m_lvCtrl;
        if (LvCtrl == null)
        {
            return;
        }
        if(LvCtrl.IsEmpty()) AddReToViewCtrl();
#endif
        float fy = 5.0f;
        float flw = Screen.width - 80;
        if (GUI.Button(new Rect(flw, fy, 80, 20), @"本地资源"))
        {
            mResMng.StartWWW(true);
        }
        if (GUI.Button(new Rect(flw - 80, fy, 80, 20), @"网络资源"))
        {
            mResMng.StartWWW(false);
        }

        fy += 40;
        //GUI.Label(new Rect( 0, fy, Screen.width, 20), @"调试信息:" + GameResMng.DebugMsg);

        //fy += 40;
        //mResTest = GUI.TextField(new Rect( 0, fy, 400, 20), mResTest);
        
        if (GUI.Button(new Rect(410, fy, 40, 20), @"查找"))
        {
            mMsg = "";
            UnityEngine.Object myObj = LoadResource(mResTest);
            if (myObj == null)
            {
                string szMsg = @"加载失败: " + mResTest;
                Debug.Log(szMsg);
            }
            else Debug.Log("myObj = " + myObj.name);
        }

        if (GUI.Button(new Rect(460, fy, 40, 20), @"清除"))
        {
            mMsg = "";
            mResTest = "";
        }

        if (GUI.Button(new Rect(510, fy, 40, 20), @"测试"))
        {
            if(null != mResDic) OnTestPack();
        }
        if (GUI.Button(new Rect(560, fy, 40, 20), @"场景"))
        {
        	GameResMng pMng = GameResMng.GetResMng();
			if(pMng != null)
			{
                pMng.LoadSceneCallback(mResTest, null);
			}
        }
        if (GUI.Button(new Rect(610, fy, 40, 20), @"列表"))
        {
            OnListPackFiles();
        }

        //fy += 40;
        //GUI.Label(new Rect( 0, fy, Screen.width, 20), @"提示消息:" + mMsg);
    }

    void OnTestPack()
    {
        if (null == mResDic) return;
        foreach (ResPackItm itm in mResDic.Values)
        {
            ResPackge pck = null;
            
            if (mPackDic.TryGetValue(itm.mPack, out pck))
            {
                string szfnd = itm.mFile;
                AssetBundle bundle = pck.mWLoad.assetBundle;
                bundle.LoadAllAssets();

                if (bundle.Contains(szfnd))
                {
                    Debug.Log("Find KEY = " + szfnd);
                    continue;
                }
                szfnd = Path.GetFileName(itm.mFile);
                if (bundle.Contains(szfnd))
                {
                    Debug.Log("Find KEY = " + szfnd);
                    continue;
                }
                szfnd = Path.GetFileNameWithoutExtension(itm.mFile);
                if (bundle.Contains(szfnd))
                {
                    Debug.Log("Find KEY = " + szfnd);
                    continue;
                }

            }
            Debug.LogWarning("NO file = " + itm.mFile);
        }
    }
    
    void OnListPackFiles()
    {
        if (null != mResDic)
        {
            Debug.Log("\nResDic Count = " + mResDic.Count);
            foreach (ResPackItm itm in mResDic.Values)
            {
                Debug.Log("Pack = " + itm.mPack + ", KEY = " + itm.mFile);
            }
        }

        if (null != mPackDic)
        {
            Debug.Log("\nPackDic Count = " + mPackDic.Count);
            foreach (KeyValuePair<string, ResPackge> pk in mPackDic)
            {
                Debug.Log("KEY = " + pk.Key + ", PckNm = " + pk.Value.mFile + ", File = " + pk.Value.mFile);
            }
        }
    }

    List<ResPackge> m_delayCleanPackage = new List<ResPackge>();
    public void AddDelayCleanPackage(ResPackge package)
    {
        foreach (ResPackge pkg in m_delayCleanPackage)
        {
            if (pkg == package)
            {
                pkg.m_delayCleanStartTime = Time.realtimeSinceStartup;
                return;
            }
        }
        package.m_delayCleanStartTime = Time.realtimeSinceStartup;
        m_delayCleanPackage.Add(package);
    }
    public void UpdateDelayCleanPackageTime(ResPackge package)
    {
        for (int i = 0; i < m_delayCleanPackage.Count; )
        {
            ResPackge pkg = m_delayCleanPackage[i];
            if (pkg == package)
            {
                pkg.m_delayCleanStartTime = Time.realtimeSinceStartup;
				return;
            }
			else
			{
				i++;
			}
        }
    }
    public void CheckDelayCleanPackage()
    {
        bool isCleanOccur = false;
        for (int i = 0; i < m_delayCleanPackage.Count; )
        {
            ResPackge pkg = m_delayCleanPackage[i];
            if (Time.realtimeSinceStartup > pkg.m_delayCleanStartTime + 1 && pkg.m_canClean)
            {
                pkg.Clean();
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
            NtfClearUnusedRes();            
        }
    }
}
