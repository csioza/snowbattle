using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

// PopupList http://wiki.unity3d.com/index.php?title=PopupList

public class GamePackUI
{
    string          mGame = ""; // 游戏名
    string          mPath = ""; // 主资源目录, 相对目录
    string          mLaunch;    // 启动场景
    bool            mbSingle;   // 单场景包
    UIToggleCtrl    mScnCtrl;   // 场景列表
    UIToggleCtrl    mPthCtrl;   // 主要子目录
    BuildGameWindow mGmWind;
    GameBundleVersionControl m_versionControl=new GameBundleVersionControl();
    bool m_useConfig=true;
    public GamePackUI(string sNm, string sPth, BuildGameWindow win)
    {
        mGame    = sNm;
        mPath    = (sPth);
        mGmWind  = win;
        mbSingle = false;
        mLaunch  = "ClientLaunch.unity";
        mPthCtrl = new UIToggleCtrl(@"子目录列表");
        mScnCtrl = new UIToggleCtrl(@"场景列表");
        
        mPthCtrl.AddButton(new BtnItem(101, 40, @"刷新", this.OnClickCtrlBtn));
        mPthCtrl.AddButton(new BtnItem(102, 40, @"打包", this.OnClickCtrlBtn));
        mPthCtrl.AddButton(new BtnItem(103, 40, @"反选", this.OnClickCtrlBtn));
        mPthCtrl.AddButton(new BtnItem(104, 40, @"增加版本", this.OnClickCtrlBtn));

        mScnCtrl.AddButton(new BtnItem(201, 40, @"刷新", this.OnClickCtrlBtn));
        mScnCtrl.AddButton(new BtnItem(202, 40, @"打包", this.OnClickCtrlBtn));

        mPath = sPth.Replace("\\", "/");
        if (!mPath.EndsWith("/"))
            mPath += "/";
        NtfRefreshPath();
        NtfRefreshScene();
        mLaunch  = FindGameLaunch();
    }
    
    //--------------------------------------------------------------------------------------------
    void OnClickCtrlBtn(int nID, UIToggleCtrl frm)
    {
        if (101 == nID) NtfRefreshPath();
        else if (102 == nID) NtfBuildResPack();
        else if (103 == nID) NtfSelectAll();
        else if (104 == nID) NtfIncreaseVersion();
        else if (201 == nID) NtfRefreshScene();
        else if (202 == nID) NtfBuildScenePack();
    }

    //--------------------------------------------------------------------------------------------
    public Rect OnLayout(float fx, float fy, float fw, float fh)
    {
        Rect rct = new Rect(fx, fy + 5, fw, fh);
        GUI.Box(rct, mGame);

        float hw = fw * 0.5f;
        fy += OnBaseUIItems(fx, fy + 5, fw) + 5;
        rct = mPthCtrl.OnLayout(fx, fy, hw, fh - 30);
        mScnCtrl.OnLayout(fx + hw + 10, fy, hw - 10, fh - 30);

        return rct;
    }

    public string GetGamePath() { return (Application.dataPath + "/" + mPath); }

    //--------------------------------------------------------------------------------------------
    float OnBaseUIItems(float fx, float fy, float fw)
    {
        Rect rct = new Rect(fx, fy, fw, 60);
        GUI.Box(rct, "");

        fy += 5;
        float fh = 0;
        GUI.Label(new Rect(fx, fy, fw - 80, 20), @"游戏名称: " + mGame);

        fh += 20;
        string sPth = GetGamePath();// ArchiveUtil.NtfPathAfterAssets(GetGamePath());
        GUI.Label(new Rect(fx, fy + fh, fw - 80, 20), @"资源目录: " + sPth);
        mbSingle = GUI.Toggle(new Rect(fx + fw - 80, fy, 100, 20), mbSingle, @"单个场景包");

        fh += 20;
        GUI.Label(new Rect(fx, fy + fh, fw - 85, 20), @"启动场景: " + mLaunch);
        if (GUI.Button(new Rect(fw - 150, fy + fh - 5, 70, 20), @"启动场景"))
        {
            string szPth = GetGamePath();
            string fl = EditorUtility.OpenFilePanel(@"场景文件", szPth, "unity");
            if(fl.Length > 0) mLaunch = fl;
        }
        
        if (GUI.Button(new Rect(fw - 70, fy + fh - 5, 70, 20), @"启动程序"))
        {
            string lch = Application.dataPath + "/" + FindGameLaunch();
            OnBuildPlayer(mGmWind.GetTarget(), lch);
        }

        fh += 20;

        return fh;
    }

    //--------------------------------------------------------------------------------------------
    void NtfRefreshPath()
    {
        if (m_useConfig)
        {
            NtfRefreshPathFromConfig();
            return;
        }
        mPthCtrl.ResetCtrl();
        List<string> pths = ArchiveUtil.NtfGetDirs(GetGamePath(), false, ArchiveUtil.mSkips);
        foreach (string sPth in pths)
        {
            string sf = ArchiveUtil.NtfPathAfter(mPath, sPth) + "/";
            mPthCtrl.AddItem(new TglItem(true, sf));
        }
        string szPth = GetGamePath();
        List<string> vfs = ArchiveUtil.NtfGetFiles(szPth, false, ArchiveUtil.mSkips);
        foreach (string sf in vfs)
        {
            string nf = ArchiveUtil.NtfPathAfter(mPath, sf);
            mPthCtrl.AddItem(new TglItem(true, nf));
        }
    }
    void NtfRefreshPathFromConfig()
    {
        mPthCtrl.ResetCtrl();
        try
        {
            TextAsset txt = Resources.Load("Versions/PackageFolder") as TextAsset;
            string[] fileList = txt.text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string n in fileList)
            {
                string[] fileAndArg = n.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (fileAndArg != null && fileAndArg.Length > 0)
                {
                    TglItem item = null;
                    item = new TglItem(true, fileAndArg[0], false);
                    item.m_viewText = item.mName;
                    for (int i = 1; i < fileAndArg.Length;i++ )
                    {
                        if (fileAndArg[i] == "*")
                        {
                            item.mIsPackedEachFile = true;
                            item.m_viewText = item.m_viewText + "---[Each]";
                        }
                        else if (fileAndArg[i] == "A")
                        {
                            item.m_isPackedAlive = true;
                            item.m_viewText = item.m_viewText + "---[KeepAlive]";
                        }
                        else if (fileAndArg[i] == "E")
                        {
                            item.m_isPackedExtern = true;
                            item.m_viewText = item.m_viewText + "---[Extern]";
                        }
                    }
                    mPthCtrl.AddItem(item);
                }
                else
                {
                    mPthCtrl.AddItem(new TglItem(true, n));
                }
            }
        }
        catch (System.Exception)
        {

        }
    }
    //--------------------------------------------------------------------------------------------
    // 打包选中资源
    void NtfSelectAll()
    {
        mPthCtrl.InvertSelect();
    }
    void NtfIncreaseVersion()
    {
        string szDefaultPath = BuildGameCMD.GetBuildFolder(mGmWind.GetTarget());
        string saveFolder = EditorUtility.SaveFolderPanel("Open project", szDefaultPath, "");
        if (saveFolder == "")
        {
            return;
        }
        m_versionControl.LoadProject(saveFolder);
        m_versionControl.IncreaseAllVersion();
        m_versionControl.SaveProject(saveFolder);
        EditorUtility.DisplayDialog(@"操作提示", @"所有包的版本号已经增加", "Ok");
    }

    void NtfBuildResPack()
    {
        string sPth = GetGamePath();
        if (m_useConfig)
        {
            //sPth = Application.dataPath+"/";
        }
        List<string> vPths = mGmWind.GetPublicPath(false); // 选中的公共资源列表
        List<string> vfs = ArchiveUtil.NtfGetFiles(sPth, false, ArchiveUtil.mSkips);
        List<TglItem> vItms = mPthCtrl.GetSelItems();
        string szDefaultPath = BuildGameCMD.GetBuildFolder(mGmWind.GetTarget());
        string saveFolder = EditorUtility.SaveFolderPanel("Save Pack File", szDefaultPath, "");
        if (saveFolder == "")
        {
            return;
        }
        m_versionControl.LoadProject(saveFolder);
        foreach (TglItem itm in vItms)
        {
            string selPath = itm.mName;
            vPths.Clear();
            vPths.Add(sPth + selPath);
            string saveFileName = saveFolder+"/" + selPath.Replace('/', '_')+".pack";
            PackageVersion.Version cfgPackage = new PackageVersion.Version();
            cfgPackage.Init();
            cfgPackage.m_isAliveInRuntime = itm.m_isPackedAlive;
            cfgPackage.m_isExternPackage = itm.m_isPackedExtern;
            if (itm.mIsPackedEachFile)
            {
                NtfPackEachFile(sPth + selPath, saveFolder, mGmWind.GetTarget(), true, m_versionControl, cfgPackage);
            }
            else
            {
                NtfPack(vPths, vfs, saveFileName, mGmWind.GetTarget(), true, m_versionControl, cfgPackage);
            }
        }
        m_versionControl.SaveProject(saveFolder);
        EditorUtility.DisplayDialog(@"操作提示", @"打包完成", "Ok");
    }
    //--------------------------------------------------------------------------------------------
    void NtfRefreshScene()
    {
        mScnCtrl.ResetCtrl();
        List<string> vAry = ArchiveUtil.NtfGetFiles(GetGamePath(), true, "*.unity");
        foreach (string sPth in vAry)
        {
            string sf = ArchiveUtil.NtfPathAfter(mPath, sPth);
            mScnCtrl.AddItem(new TglItem(true, sf));
        }
    }
    
    //--------------------------------------------------------------------------------------------
    // 打包选中场景
    void NtfBuildScenePack()
    {
        OnBuildScene(mGmWind.GetTarget());// 打包场景
    }
    
    //--------------------------------------------------------------------------------------------
    // 生成游戏 启动程序/资源包/场景包
    public void BuildGameAndPack(BuildTarget tgt)
    {
        string sLaunch = Application.dataPath + "/" + FindGameLaunch();
        OnBuildPlayer(tgt, sLaunch);// 生成启动程序

        OnBuildPack(tgt);// 打包资源

        OnBuildScene(tgt);// 打包场景
    }

    //-----------------------------------------------------------------------------
    // 构建游戏启动程序
    public void OnBuildPlayer(BuildTarget tgt, string sLaunch)
    {
        string szTgtNm = BuildGameCMD.GetTargetName(tgt);
        string szPath  = BuildGameCMD.GetBuildFolder(tgt);
        string szEx    = BuildGameCMD.GetTargetExt(tgt);
        string szTitle = "Save " + szTgtNm + " App";
        string szApp   = szTgtNm + "_" + mGame;
        string file = "";
        if (BuildTarget.iOS == tgt)  // 取目录
        {
            file = EditorUtility.SaveFolderPanel(szTitle, szPath, mGame);
        }
        else  // 取文件
        {
            file = EditorUtility.SaveFilePanel(szTitle, szPath, szApp, szEx);
        }

        if (file.Length > 0)
        {
            BuildGameCMD.BuildGamePlayer(file, sLaunch, tgt, false);
        }
    }
    
    //-----------------------------------------------------------------------------
    // 打包除场景(*.unity)以外的资源
    public void OnBuildPack(BuildTarget tgt)
    {
        NtfBuildResPack();
        //string sPth = GetGamePath();
        //List<string> vPths = mGmWind.GetPublicPath(false); // 选中的公共资源列表
        //List<string> vfs = ArchiveUtil.NtfGetFiles(sPth, false, ArchiveUtil.mSkips);
        //List<TglItem> vItms = mPthCtrl.GetSelItems();
        //foreach (TglItem itm in vItms)
        //{
        //    vPths.Add(sPth + itm.mName);
        //}
        //if (vPths.Count > 0 || vfs.Count > 0)
        //{
        //    string sPckNm = mGame;
        //    if (vPths.Count > 0) sPckNm = ArchiveUtil.GetPthLast(vPths[0]);
        //    string szPath = BuildGameCMD.GetBuildFolder(mGmWind.GetTarget());
        //    string file = EditorUtility.SaveFilePanel("Save Pack File", szPath, sPckNm, "pack");
        //    if (file.Length > 0)
        //    {
        //        m_versionControl.LoadProject(szPath);
        //        NtfPack(vPths, vfs, file, mGmWind.GetTarget(),false,m_versionControl);
        //        m_versionControl.SaveProject(szPath);
        //    }
        //    return;
        //}
        //EditorUtility.DisplayDialog(@"操作提示", @"请选择要打包的目录", "Ok");


    }
    
    //-----------------------------------------------------------------------------
    // 打包场景(*.unity)资源
    public void OnBuildScene(BuildTarget tgt)
    {
        List<string> vScn = new List<string>();
        List<TglItem> vItms = mScnCtrl.GetSelItems();
        foreach (TglItem itm in vItms)
        {
            string sf = GetGamePath() + itm.mName;
            vScn.Add(sf);
        }

        string szTgtNm = BuildGameCMD.GetTargetName(tgt);
        string szPath = BuildGameCMD.GetBuildFolder(tgt);

        string szDefaultPath = BuildGameCMD.GetBuildFolder(mGmWind.GetTarget());
        string saveFolder = EditorUtility.SaveFolderPanel("Save Pack File", szDefaultPath, "");
        if (saveFolder == "")
        {
            return;
        }
        m_versionControl.LoadProject(saveFolder);
        if (mbSingle)
        {
            string file = EditorUtility.SaveFilePanel("Save Scene File", szPath, mGame + szTgtNm, "unity3d");
            if (file.Length > 0)
                BuildGameCMD.BuildGameScene(file, vScn, tgt, m_versionControl);
        }
        else
        {
            string szMsg;
            int nCount = 0;
            foreach (string sf in vScn)
            {
                string nsf = szPath + "/" + ArchiveUtil.GetFileName(sf) + ".unity3d";
                szMsg = BuildGameCMD.BuildGameScene(nsf, sf, tgt, m_versionControl);
                if (!string.IsNullOrEmpty(szMsg)) Debug.LogWarning("BuildGameScene Msg = " + szMsg);
                else ++nCount;
            }
            szMsg = @"消息: 成功打包 [";
            szMsg += (nCount.ToString() + @"] 个场景, 有 [");
            szMsg += ((vScn.Count - nCount).ToString() + @"] 个场景打包失败");
            EditorUtility.DisplayDialog(@"操作已结束", szMsg, "Ok");
        }
        m_versionControl.SaveProject(saveFolder);
    }
    
    //-----------------------------------------------------------------------------
    public static void NtfPack(List<string> vPths, string file, BuildTarget tgt,GameBundleVersionControl version
        , PackageVersion.Version cfgVersion)
    {
        List<string> fAry = new List<string>();
        foreach (string sPth in vPths)
        {
            List<string> vfs = ArchiveUtil.NtfGetFiles(sPth, true, ArchiveUtil.mSkips);
            fAry.AddRange(vfs);
        }
        BuildGameCMD.BuildGamePack(file, fAry, tgt, false, version);
        string packageName = Path.GetFileName(file);
        packageName = packageName.ToLower();
        if (version.m_version.HavePackage(packageName))
        {
            PackageVersion.Version v = version.m_version.LookupPackageVersion(packageName);
            v.m_isExternPackage = cfgVersion.m_isExternPackage;
            v.m_isAliveInRuntime = cfgVersion.m_isAliveInRuntime;
            version.m_version.AddPackage(packageName, v);
        }
    }

    //-----------------------------------------------------------------------------
    public static string NtfPack(List<string> vPths, List<string> vfAry, string file, BuildTarget tgt,bool silence
        ,GameBundleVersionControl version,PackageVersion.Version cfgVersion)
    {
        List<string> fAry = new List<string>();
        if(vfAry != null) fAry.AddRange(vfAry);
        foreach (string sPth in vPths)
        {
            List<string> vfs = ArchiveUtil.NtfGetFiles(sPth, true, ArchiveUtil.mSkips);
            fAry.AddRange(vfs);
        }

        BuildGameCMD.BuildGamePack(file, fAry, tgt, silence, version);
        string packageName = Path.GetFileName(file);
        packageName = packageName.ToLower();
        if (version.m_version.HavePackage(packageName))
        {
            PackageVersion.Version v = version.m_version.LookupPackageVersion(packageName);
            v.m_isExternPackage = cfgVersion.m_isExternPackage;
            v.m_isAliveInRuntime = cfgVersion.m_isAliveInRuntime;
            version.m_version.AddPackage(packageName, v);
        }
        return packageName;
    }

    //-----------------------------------------------------------------------------
    public static void NtfPackEachFile(string srcFolder, string targetFolder, BuildTarget tgt, bool silence
        , GameBundleVersionControl version,PackageVersion.Version cfgVersion)
    {
        List<string> allNeedPackedFile = ArchiveUtil.NtfGetFiles(srcFolder, true, ArchiveUtil.mSkips);
        List<string> procFileList = new List<string>();
        string targetPackFile;
        foreach (string singleFile in allNeedPackedFile)
        {
            procFileList.Clear();
            procFileList.Add(singleFile);
            string[] pathsOfThisFile = singleFile.Split(new char[] { '/', '\\' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (pathsOfThisFile.Length<3)
            {
                targetPackFile = singleFile.Replace('/','_');
                targetPackFile = targetPackFile.Replace('\\','_');
                targetPackFile = targetFolder + "/" + targetPackFile + ".pack";
            }
            else
            {
                targetPackFile = targetFolder+"/";
                for (int i = 2; i < pathsOfThisFile.Length;i++ )
                {
                    targetPackFile += "_" + pathsOfThisFile[i];
                }
                targetPackFile += ".pack";
            }
            targetPackFile = targetPackFile.Replace(" ", "");
            targetPackFile = targetPackFile.ToLower();
            string packageName = Path.GetFileName(targetPackFile);
            packageName = packageName.ToLower();
            BuildGameCMD.BuildGamePack(targetPackFile, procFileList, tgt, silence,version);
            if (version.m_version.HavePackage(packageName))
            {
                PackageVersion.Version v = version.m_version.LookupPackageVersion(packageName);
                v.m_isExternPackage = cfgVersion.m_isExternPackage;
                v.m_isAliveInRuntime = cfgVersion.m_isAliveInRuntime;
                version.m_version.AddPackage(packageName, v);
            }
        }
    }

    
    //-----------------------------------------------------------------------------
    // 找到当前游戏的启动场景  !!!! 这里要按游戏来搜索 !!!!
    string FindGameLaunch()
    {
        string sLaunch = "";
        string szPth = Application.dataPath;//GetGamePath();// Application.dataPath;
        List<string> vAry = ArchiveUtil.NtfGetFiles(szPth, true, "*.unity");
        foreach (string sPth in vAry)
        {
            string sf = sPth.ToLower();
            if (sf.Contains("launch"))
            {
                sLaunch = sPth;
                break;
            }
        }
        //sLaunch = ArchiveUtil.NtfPathAfter(szPth, sLaunch);
        sLaunch = ArchiveUtil.NtfPathAfterAssets(sLaunch);
        return sLaunch;
    }
    
    //--------------------------------------------------------------------------------------------
    // 临时版本, 资源拷贝 Resources/ 目录下, 完成之后删除
    public bool BuildTempGame(BuildTarget tgt)
    {
        // 保存路径
        string szTgtNm = BuildGameCMD.GetTargetName(tgt);
        string szPath  = BuildGameCMD.GetBuildFolder(tgt);
        string szEx    = BuildGameCMD.GetTargetExt(tgt);
        string szTitle = "Save " + szTgtNm + " App";
        string szApp   = szTgtNm + "_" + mGame;
        string file = EditorUtility.SaveFilePanel(szTitle, szPath, szApp, szEx);
        if (file.Length > 0)
        {
			//string sPth = "Assets/" + mPath;   // 原始资源目录
            string dPth = "Assets/Resources/";
            //string dfil = "Assets/Resources.meta";

            // 若存在则删除
            //if (File.Exists(dfil)) FileUtil.DeleteFileOrDirectory(dfil);
            //if (Directory.Exists(dPth)) FileUtil.DeleteFileOrDirectory(dPth);
            //AssetDatabase.Refresh();
            //ArchiveUtil.NtfCopyFiles(sPth, dPth);   // 拷贝文件, 内部有过滤
            //AssetDatabase.Refresh();


            List<string> vAry = ArchiveUtil.NtfGetFiles(dPth, true, "*.unity"); // 搜索场景文件
            foreach (string sf in vAry) // 找到启动用场景
            {
                string ft = sf.ToLower();
                //if (ft.Contains("launch"))
                if(ft.Contains("demo-summer-a-am"))
                {
                    vAry.Remove(sf);
                    vAry.Insert(0, sf);
                    break;
                }
            }
            BuildGameCMD.BuildGamePlayer(file, vAry, tgt, true);
            // 结束后删除这些文件
            //if (Directory.Exists(dPth)) FileUtil.DeleteFileOrDirectory(dPth);
            //if (File.Exists(dfil)) FileUtil.DeleteFileOrDirectory(dfil);
            //AssetDatabase.Refresh();
            return true;
        }
        return false;
    }

}
