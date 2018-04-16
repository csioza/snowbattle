using UnityEngine;
using System.Collections;
using System.IO;
using System;


//-------------------------------------------------------------------------------------------------------------------
// 本地不存在则下载
public class ResPackge
{
    public class AsyncLoadData
    {
        public UnityEngine.Object m_res=null;
        public ResPackge m_owner=null;
        public bool IsFinish = false; //{get{return FinishCount == AsyncCount;}}
        public int FinishCount = 0;
        public int AsyncCount = 1;
    }
    public float m_delayCleanStartTime=0;
    int m_lockCount = 0;
    public int      mType;      // 0(普通资源)/1(场景 .unity)
    public WWW      mWLoad;
    public string   mFile;
    public enum State
    {
        None,
        Downloading,
        LocalReady,
        Failed,
    }
    public bool m_isKeepAlive = false;
    public State m_state= State.None;
    public bool m_canClean{get{return m_canCleanCount == 0;}}
    int m_canCleanCount = 0;
    //--------------------------------------------------------------------------------------------
    public float GetProgress()
    {
        if (m_state == State.LocalReady)
        {
            return 1.0f;
        }
        if (mWLoad != null)
        {
            if (mWLoad.error != null)
                return 1.0f;
            return mWLoad.isDone ? 1.0f : mWLoad.progress;
        }
        return 0.0f;
    }

    //--------------------------------------------------------------------------------------------
    public UnityEngine.Object LoadObject(string sfile)
    {
        if (IsDone())
        {
            AssetBundle bundle = mWLoad.assetBundle;
            return bundle.LoadAsset(sfile, typeof(UnityEngine.Object));
        }
        else
        {

        }
        return null;
    }
    public IEnumerator LoadObjectAsync(string sfile, MonoBehaviour delegateYield, ResPackge.AsyncLoadData data)
    {
        string sUrl = ResPath.WLocalUrl(mFile);
        m_canCleanCount++;
        if (m_state == State.LocalReady)
        {
            if (!IsDone() && m_state == State.LocalReady)
            {
                Debug.Log("LoadObjectAsync open package, URL = " + sUrl);
                if (mWLoad == null)
                {
                    mWLoad = new WWW(sUrl);
                }
                m_lockCount++;
                yield return mWLoad;
                //yield return new WaitForSeconds(2);
                if (mWLoad == null || null != mWLoad.error)
                {
                    m_state = State.Failed;
                    if (mWLoad != null)
                    {
                        Debug.LogError(mWLoad.error);
                    }
                }

                ApplyPackge();
                m_lockCount--;
            }
            if (IsDone())
            {
                AssetBundle bundle = mWLoad.assetBundle;
                data.m_res = bundle.LoadAsset(sfile, typeof(UnityEngine.Object));
                data.m_owner = this;
                //Clean();
                if (!m_isKeepAlive && m_lockCount == 0)
                {
                    GameResMng.GetResMng().AddDelayCleanPackage(this);
                    //Clean();
                }
            }
			else{
                Debug.LogError("Open package Failed, URL = " + sUrl);
			}
        }
        m_canCleanCount--;
        data.IsFinish = true;
    }
    public IEnumerator LoadPackageAsync(ResPackge.AsyncLoadData data)
    {
        string sUrl = ResPath.WLocalUrl(mFile);
        m_canCleanCount++;
        if (m_state == State.LocalReady)
        {
            if (!IsDone() && m_state == State.LocalReady)
            {
//				float t = Time.realtimeSinceStartup;
                Debug.Log("Open package, URL = " + sUrl);
                if (mWLoad == null)
                {
                    mWLoad = new WWW(sUrl);
                }
                m_lockCount++;
                yield return mWLoad;
                //yield return new WaitForSeconds(1);
                if (null != mWLoad.error)
                {
                    m_state = State.Failed;
                    Debug.LogError(mWLoad.error);
                }

                ApplyPackge();
                m_lockCount--;
            }
            if (IsDone())
            {
                
            }
            else
            {
                Debug.LogError("Open package Failed, URL = " + sUrl);
            }
        }
        m_canCleanCount--;
        data.IsFinish = true;
    }
    //--------------------------------------------------------------------------------------------
    // 注意: AssetBundle 正确就返回 true
    public bool LoadLevel(string sName)
    {
        if (IsDone())
        {
//            AssetBundle bundle = mWLoad.assetBundle;
            Application.LoadLevel(sName);
            return true;
        }
        return false;
    }

    //--------------------------------------------------------------------------------------------
    // 0(未启动)/1(加载中)/2(加载完)/3(加载失败)
    public State GetState()
    {
        return m_state;
    }
    
    //--------------------------------------------------------------------------------------------
    public string GetStateMsg()
    {
        State nState = GetState();
        if (State.Downloading == nState) return @"加载中";
        if (State.LocalReady == nState) return @"已加载";
        if (State.Failed == nState) return (@"出错了, " + mWLoad.error);
        return @"未启动";
    }
    
    //--------------------------------------------------------------------------------------------
    public bool IsDone()
    {
        if ((mWLoad != null) && (mWLoad.error == null))
        {
            return mWLoad.isDone;
        }
        return false;
    }
    
    //--------------------------------------------------------------------------------------------
    public bool ApplyPackge()
    {
        if (IsDone())
        {
            AssetBundle bundle = mWLoad.assetBundle;
            NtfClipWarning(bundle);
            return true;
        }
        return false;
    }

    public void Clean()
    {
        try
        {
            if (mWLoad != null)
            {
                if (mWLoad.error == null)
                {
                    AssetBundle bundle = mWLoad.assetBundle;
                    bundle.Unload(false);
                }
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("Error OnCleanPackage:" + mFile);
        }
        mWLoad = null;
    }
    string GetLocalUrl()
    {
        string sf = ResPath.GetLocal(mFile);
        try
        {
            if (!File.Exists(sf))   // 本地不存在依然从网上下载
            {

            }
        }
        catch (Exception exp)
        {
            Debug.LogWarning("Call File.Exists() Er, file = " + sf + ", Msg = " + exp.Message);
        }
        return ResPath.WLocalUrl(mFile);
    }

    public IEnumerator TryDownload(bool isClean,bool forceDownload)
    {
        if (mWLoad != null)
        {
            yield break;
        }
        GameResMng.SetLoading();
        string sf = ResPath.GetLocal(mFile);
        bool isNeedDownload = forceDownload;
        try
        {
            if (!File.Exists(sf))   // 本地不存在依然从网上下载
            {
                isNeedDownload = true;
                m_state = State.Downloading;
            }
            else if(isClean)
            {
                m_state = State.LocalReady;
            }
        }
        catch (Exception exp)
        {
            Debug.LogWarning("Call File.Exists() Er, file = " + sf + ", Msg = " + exp.Message);
        }
        string sUrl = "";
        if (isNeedDownload || !isClean)
        {
            if (isNeedDownload)
            {
                sUrl = ResPath.GetUrl(mFile);
            }
            else
            {
                sUrl = ResPath.WLocalUrl(mFile);
            }
            Debug.Log("BeginDownload, URL = " + sUrl);
            GameResMng.DebugMsg = "Download, URL = " + sUrl;
            m_state = State.Downloading;
            mWLoad = new WWW(sUrl);
            yield return mWLoad;
            if (null != mWLoad.error)
            {
                m_state = State.Failed;
                Debug.LogError("Error, URL = " + sUrl + "   " + mWLoad.error);
                GameResMng.GetResMng().OnPackageDownloadFailed(mFile);
            }
            else if (IsDone())
            {
                m_state = State.LocalReady;
                if (isNeedDownload)
                {
                    ResPath.SaveToLocal(sf, mWLoad.bytes);
                }
                GameResMng.GetResMng().OnPackageDownload(mFile, mWLoad.bytes.Length);
            }
            if (isClean)
            {
                Clean();
        		Resources.UnloadUnusedAssets();
				GC.Collect();
            }
			string msg = "EndDownload, URL = " + sUrl;
			if(isClean)
			{
				msg += "[Clean]";
			}
            Debug.Log(msg);
            //
        }
        ApplyPackge();
    }
    //--------------------------------------------------------------------------------------------
    public IEnumerator NtfDownOrLoad(bool bLocal)
    {
        GameResMng.SetLoading();
        string sf = ResPath.GetLocal(mFile);
        if (bLocal) // 从本地读取的话要确保资源存在
        {
            try
            {
                if (!File.Exists(sf))   // 本地不存在依然从网上下载
                {
                    bLocal = false;
                }
            }
            catch(Exception exp)
            {
                Debug.LogWarning("Call File.Exists() Er, file = " + sf + ", Msg = " + exp.Message);
            }
        }
        string sUrl = "";
        if (bLocal)
        {
            sUrl = ResPath.WLocalUrl(mFile);
        }
        else // 从网上下载
        {
            sUrl = ResPath.GetUrl(mFile);
        }
        Debug.Log("Download, URL = " + sUrl);
        GameResMng.DebugMsg = "Download, URL = " + sUrl;

        //WWW.LoadFromCacheOrDownload(sUrl,1);//
        mWLoad = new WWW(sUrl);
        yield return mWLoad;

		if ((!bLocal) && mWLoad.isDone)
		{
            ResPath.SaveToLocal(sf, mWLoad.bytes);
            Clean();
		}
		if (null != mWLoad.error)
		{
			Debug.LogError(mWLoad.error);
		}
        ApplyPackge();
    }
    void NtfClipWarning(AssetBundle bundle){}
}
