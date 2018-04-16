using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//游戏资源更新
public class GameResUpdate
{
    //入口
    public MonoBehaviour Entry;
    //PackMD5.xml文件的服务器和本地信息
    XML_PackMD5 PackMD5Remote;
    XML_PackMD5 PackMD5Local;
    //FilePack.xml文件的服务器和本地信息
    XML_FilePack FilePackRemote;
    XML_FilePack FilePackLocal;
    //PackDepend.xml文件的服务器和本地信息
    XML_PackDepend PackDependRemote;
    XML_PackDepend PackDependLocal;

    //更新列表
    List<XML_PackMD5.Info> UpdateList = new List<XML_PackMD5.Info>();
    //更新成功列表
    List<XML_PackMD5.Info> SuccList = null;
    //更新失败列表
    List<XML_PackMD5.Info> FailList = null;

    //下载xml文件的数量
    int DownXmlCount = 0;
    //一次下载包的数量的最大值
    static int MaxDownloadPackageCount = 5;
    //下载索引
    int DownIndex = 0;
    //下载总大小(单位：byte)
    public long TotalSize = 0;
    //已经下载的大小(单位：byte)
    public long DownSize = 0;
    //下载资源的进度
    public float DownPercent { get { return (float)DownSize / TotalSize; } }

    //检查更新
    public bool CheckUpdate(GameResManager.CheckUpdateCallback callback)
    {
        if (Entry == null)
        {
            return false;
        }
        //清除更新残留数据
        ClearUpdateData();
        Entry.StartCoroutine(Coroutine_DownloadVersion(callback));
        return true;
    }
    //下载更新包
    public void DownloadUpdatePackage()
    {
        SuccList = new List<XML_PackMD5.Info>(UpdateList.Count);
        FailList = new List<XML_PackMD5.Info>();
        //开启下载
        DownIndex = 0;
        for (int i = 0; i < MaxDownloadPackageCount; ++i)
        {
            Entry.StartCoroutine(Coroutine_DownloadUpdatePackage());
        }
        //检查下载是否完毕
        Entry.StartCoroutine(Coroutine_UpdateIsDone());
    }
    //下载更新失败的包
    public void RetryDownloadFailPackage()
    {
        UpdateList.Clear();
        foreach (var item in FailList)
        {
            UpdateList.Add(new XML_PackMD5.Info(item));
        }
        SuccList.Clear();
        FailList.Clear();
        DownloadUpdatePackage();
    }
    //加载配置文件
    public bool LoadConfigFile()
    {
        FilePackLocal = new XML_FilePack(ResPath.LocalPath + ResPath.CurrentPathName);
        PackMD5Local = new XML_PackMD5(ResPath.LocalPath + ResPath.CurrentPathName);
        PackDependLocal = new XML_PackDepend(ResPath.LocalPath + ResPath.CurrentPathName);
        return true;
    }
    //获取file和pack的对应关系列表
    public List<XML_FilePack.Info> GetFilePackList()
    {
        return FilePackLocal.InfoList;
    }
    //
    public List<XML_PackMD5.Info> GetPackMD5List()
    {
        return PackMD5Local.InfoList;
    }
    //获取pack和依赖pack的对应关系列表
    public List<XML_PackDepend.Info> GetPackDependList()
    {
        return PackDependLocal.InfoList;
    }
    //清除更新残留数据
    public void ClearUpdateData()
    {
        if (PackMD5Remote != null)
        {
            PackMD5Remote.Clear();
            PackMD5Remote = null;
        }
        if (PackMD5Local != null)
        {
            PackMD5Local.Clear();
            PackMD5Local = null;
        }
        if (FilePackRemote != null)
        {
            FilePackRemote.Clear();
            FilePackRemote = null;
        }
        if (FilePackLocal != null)
        {
            FilePackLocal.Clear();
            FilePackLocal = null;
        }
        if (PackDependRemote != null)
        {
            PackDependRemote.Clear();
            PackDependRemote = null;
        }
        if (PackDependLocal != null)
        {
            PackDependLocal.Clear();
            PackDependLocal = null;
        }

        if (UpdateList != null)
        {
            UpdateList.Clear();
            UpdateList = null;
        }
        if (SuccList != null)
        {
            SuccList.Clear();
            SuccList = null;
        }
        if (FailList != null)
        {
            FailList.Clear();
            FailList = null;
        }
    }

    //协程 下载版本
    IEnumerator Coroutine_DownloadVersion(GameResManager.CheckUpdateCallback callback)
    {
        string url = ResPath.GetServerURL(ResPath.VersionFileName);
        WWW downVer = new WWW(url);
        while (!downVer.isDone)
        {
            yield return downVer;
        }
        if (downVer.isDone && downVer.error == null)
        {
            //保存版本文件到临时目录下
            if (ResUtil.SaveFile(downVer.bytes, ResPath.LocalTempCachePath, ResPath.VersionFileName))
            {
                //比较本地版本和server版本
                VersionInfo verLocal = new VersionInfo(ResPath.LocalPath + ResPath.CurrentPathName + ResPath.VersionFileName);
                VersionInfo verServer = new VersionInfo(ResPath.LocalTempCachePath + ResPath.VersionFileName);

                if (0 > VersionInfo.CompareVersion(verLocal, verServer))
                {//服务器版本较新，更新
                    DownXmlCount = 3;
                    Entry.StartCoroutine(Coroutine_DownloadPackMD5());
                    Entry.StartCoroutine(Coroutine_DownloadFilePack());
                    Entry.StartCoroutine(Coroutine_DownloadPackDepend());
                    while (DownXmlCount > 0)
                    {//下载xml文件中
                        yield return new WaitForSeconds(1);
                    }
                    UpdateList = XML_PackMD5.GetUpdateList(PackMD5Remote, PackMD5Local, out TotalSize);
                }
            }
        }
        else
        {
            Debug.LogWarning("WWW failed, url:" + url + ", isDone:" + downVer.isDone + ", error:" + downVer.error);
        }
        //回调
        callback(UpdateList!=null && UpdateList.Count > 0);
    }
    //协程 下载PackMD5.xml
    IEnumerator Coroutine_DownloadPackMD5()
    {
        string url = ResPath.GetServerURL(ResPath.PackMD5FileName);
        WWW down = new WWW(url);
        while (!down.isDone)
        {
            yield return down;
        }
        if (down.isDone && down.error == null)
        {//保存
            ResUtil.SaveFile(down.bytes, ResPath.LocalTempCachePath, ResPath.PackMD5FileName);
        }
        else
        {
            Debug.LogWarning("WWW failed, url:" + url + ", isDone:" + down.isDone + ", error:" + down.error);
        }
        PackMD5Remote = new XML_PackMD5(ResPath.LocalTempCachePath);
        PackMD5Local = new XML_PackMD5(ResPath.LocalPath + ResPath.CurrentPathName);
        --DownXmlCount;
    }
    //协程 下载FilePack.xml
    IEnumerator Coroutine_DownloadFilePack()
    {
        string url = ResPath.GetServerURL(ResPath.FilePackFileName);
        WWW down = new WWW(url);
        while (!down.isDone)
        {
            yield return down;
        }
        if (down.isDone && down.error == null)
        {//保存
            ResUtil.SaveFile(down.bytes, ResPath.LocalTempCachePath, ResPath.FilePackFileName);
        }
        else
        {
            Debug.LogWarning("WWW failed, url:" + url + ", isDone:" + down.isDone + ", error:" + down.error);
        }
        FilePackRemote = new XML_FilePack(ResPath.LocalTempCachePath);
        FilePackLocal = new XML_FilePack(ResPath.LocalPath + ResPath.CurrentPathName);
        --DownXmlCount;
    }
    //协程 下载PackDepend.xml
    IEnumerator Coroutine_DownloadPackDepend()
    {
        string url = ResPath.GetServerURL(ResPath.PackDependFileName);
        WWW down = new WWW(url);
        while (!down.isDone)
        {
            yield return down;
        }
        if (down.isDone && down.error == null)
        {//保存
            ResUtil.SaveFile(down.bytes, ResPath.LocalTempCachePath, ResPath.PackDependFileName);
        }
        else
        {
            Debug.LogWarning("WWW failed, url:" + url + ", isDone:" + down.isDone + ", error:" + down.error);
        }
        PackDependRemote = new XML_PackDepend(ResPath.LocalTempCachePath);
        PackDependLocal = new XML_PackDepend(ResPath.LocalPath + ResPath.CurrentPathName);
        --DownXmlCount;
    }
    //协程-下载更新包
    IEnumerator Coroutine_DownloadUpdatePackage()
    {
        while (DownIndex < UpdateList.Count)
        {
            XML_PackMD5.Info info = UpdateList[DownIndex];
            ++DownIndex;
            bool isSucc = false;
            if (long.Parse(info.FileSize) != 0)
            {//下载
                string filename = info.PackName;
                string url = ResPath.GetServerPackURL(filename);
                WWW down = new WWW(url);
                while (!down.isDone)
                {
                    yield return down;
                }
                if (down.isDone && down.error == null)
                {
                    if (ResUtil.SaveFile(down.bytes, ResPath.LocalPackagePath, filename))
                    {//保存文件成功
                        isSucc = true;
                    }
                    else
                    {//保存文件失败
                        Debug.LogWarning("save file failed! file = " + ResPath.LocalPackagePath + filename);
                    }
                }
                else
                {
                    Debug.LogWarning("download failed, url = " + url + ",isDone:" + down.isDone + ",error:" + down.error);
                }
            }
            else
            {
                isSucc = true;
            }
            if (isSucc)
            {//更新成功
                SuccList.Add(info);
                //更新本地
                PackMD5Local.Update(info);
                FilePackLocal.Update(info.PackName, FilePackRemote);
                PackDependLocal.Update(info.PackName, PackDependRemote);

                DownSize += long.Parse(info.FileSize);
            }
            else
            {
                FailList.Add(info);

                //DownSize += long.Parse(info.FileSize);
            }
        }
    }
    //协程-检查下载是否完毕
    IEnumerator Coroutine_UpdateIsDone()
    {
        while (SuccList.Count + FailList.Count != UpdateList.Count)
        {
            yield return new WaitForSeconds(1);
        }
        PackMD5Local.Save();
        FilePackLocal.Save();
        PackDependLocal.Save();

        if (FailList.Count == 0)
        {//全部下载成功，更改版本文件
            try
            {
                System.IO.File.Copy(ResPath.LocalTempCachePath + ResPath.VersionFileName, ResPath.LocalPath + ResPath.CurrentPathName + ResPath.VersionFileName, true);
            }
            catch (SystemException e)
            {
                Debug.LogWarning("copy file failed, filename:" + ResPath.VersionFileName + ",error:" + e.Message);
            }
        }
    }
}