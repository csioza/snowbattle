

using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameBundleVersionControl : BundleVersionControl
{
    public PackageVersion m_version = new PackageVersion();
    public Dictionary<string, string> m_md5List = new Dictionary<string, string>();
    public override void IncreasePackageVersion(string packageName)
    {
        PackageVersion.Version v = new PackageVersion.Version();
        v.Init();
        v.m_version = 0;
        if (m_version.HavePackage(packageName))
        {
            v = m_version.LookupPackageVersion(packageName);
        }
        v.m_version++;
        m_version.AddPackage(packageName, v);
//        m_isDirty = true;
    }
    public override bool UpdateFileMD5(string fileWith,string pkg)
    {
        //resources/axxxxx.
        string checkID = fileWith + pkg;

        string realPath = Application.dataPath+"/" + fileWith;
        string realFileMd5 = GenMD5Util.NtfGenFileMD5(realPath);
        string savedFileMd5 = null;
        if (m_md5List.ContainsKey(checkID))
        {
            savedFileMd5 = m_md5List[checkID];
        }
        if (savedFileMd5 == null || realFileMd5 != savedFileMd5)
        {
            m_md5List[checkID] = realFileMd5;
            return true;
        }
        return false;
    }
    public void LoadProject(string folder)
    {
        m_version.LoadPackagesVersion(folder + "/" + ResPath.GetPackageVersionTxt());
        LoadAllSavedFileMD5(folder + "/PackedFileMD5.txt");
    }
    public void IncreaseAllVersion()
    {
        List<string> packages = new List<string>();
        foreach (KeyValuePair<string,PackageVersion.Version> p in m_version.m_versions)
        {
            packages.Add(p.Key);
        }
        foreach (string n in packages)
        {
            m_version.IncreasePackageVersion(n);
        }
    }
    public void SaveProject(string folder)
    {
        //if (m_isDirty)
        {
//            m_isDirty = false;
            m_version.SavePackageVersion(folder + "/" + ResPath.GetPackageVersionTxt());
            SaveAllFileMD5(folder + "/PackedFileMD5.txt");
        }
    }
    void LoadAllSavedFileMD5(string md5FileName)
    {
        m_md5List.Clear();
        byte[] buffer = ResPath.GetFileData(md5FileName);
        if (buffer == null)
        {
            return;
        }
        MemoryStream mryStm = new MemoryStream(buffer);
        StreamReader stm = new StreamReader(mryStm);
        string line = stm.ReadLine();
        while (line != null)
        {
            if (line.Length == 0)
            {
                continue;
            }
            string[] nameAndVersion = line.Split('|');

            if (nameAndVersion.Length > 1)
            {
                m_md5List[nameAndVersion[0]] = nameAndVersion[1];
            }
			line = stm.ReadLine();
        }
    }
    void SaveAllFileMD5(string localFile)
    {
        try
        {
            FileStream fstm = new FileStream(localFile, FileMode.Create);
            StreamWriter stream = new StreamWriter(fstm);

            foreach (KeyValuePair<string, string> p in m_md5List)
            {
                string data = p.Key + "|" + p.Value;
                data += "\n";
                stream.Write(data.ToCharArray());
            }
            stream.Close();
            fstm.Close();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error on save SaveAllFileMD5 :" + ex.Message);
        }
    }
}

//-------------------------------------------------------------------------------------------------------------------
//
// 命令: 发布游戏客户端:启动程序/资源包
//
// 注意: 
//
public class BuildGameCMD
{
    static public string GetBuildFolder(BuildTarget tgt)
    {
        string szPath = @"Publish/" + GetTargetName(tgt) + "/";
        if(!Directory.Exists(szPath)) Directory.CreateDirectory(szPath);
        return szPath;
    }

    
    //--------------------------------------------------------------------------------------------
    // 生成普通资源包
    static public void BuildGamePack(string file, List<string> fLst, BuildTarget tgt, bool silence,BundleVersionControl version)
    {
        file = file.ToLower();
        string szNm = GetTargetName(tgt);
        string msg = @"确定要生成 " + szNm + @" 客户端资源包吗? 这个过程可能会很漫长";
        if (silence || EditorUtility.DisplayDialog(@"游戏版本发布", msg, "Ok", "Cancel"))
        {
            if (AssetBundleUtil.ExportToBundle(fLst, file, tgt, version))
            {
                msg = szNm + @" 客户端资源包已生成, 存放于: " + file;
                if (!silence)
                {
                    EditorUtility.DisplayDialog(@"操作已完成", msg, "Ok");
                }
                else
                {
                    Debug.Log(msg);
                }
            }
            else
            {
                msg = szNm + @" 客户端资源包生成失败，是否继续？？？";
                for (int i = 0; i < fLst.Count;i++ )
                {
                    msg += "\r\n" + fLst[i];
                }
                //EditorUtility.DisplayDialog(@"操作失败", msg, "Ok");
                if (!EditorUtility.DisplayDialog(@"操作失败", msg, "Ok", "Cancel"))
                {
                    throw new System.Exception("error file:" + file);
                }
            }
        }
    }

    
    //--------------------------------------------------------------------------------------------
    // 打包场景文件
    static public void BuildGameScene(string file, List<string> fLst, BuildTarget tgt,GameBundleVersionControl version)
    {
        //BuildOptions op = BuildOptions.BuildAdditionalStreamedScenes;
        //string szMsg = BuildPipeline.BuildPlayer(fLst.ToArray(), file, tgt, op);
        file = file.ToLower();
        string pkgName = Path.GetFileName(file);
        pkgName = pkgName.ToLower();
        bool needPackage = false;
        foreach (string n in fLst)
        {
            string sceneFileName = ArchiveUtil.NtfPathAfterAssets(n).ToLower();

            if (version.UpdateFileMD5(sceneFileName,pkgName))
            {
                needPackage = true;
                break;
            }
        }
        if (!needPackage)
        {
            return;
        }

        string szMsg = BuildPipeline.BuildStreamedSceneAssetBundle(fLst.ToArray(), file, tgt);
        if (string.IsNullOrEmpty(szMsg))
        {
            AppendResPackItm(fLst.ToArray(), file, tgt);
            version.IncreasePackageVersion(pkgName);
            szMsg = @"操作成功";
        }
        EditorUtility.DisplayDialog(@"操作已结束", @"消息: " + szMsg, "Ok");
    }
    static public string BuildGameScene(string file, string szScene, BuildTarget tgt,GameBundleVersionControl version)
    {
        file = file.ToLower();
        string pkgName = Path.GetFileName(file);
        pkgName = pkgName.ToLower();
        string sceneFileName = ArchiveUtil.NtfPathAfterAssets(szScene).ToLower();

        if (!version.UpdateFileMD5(sceneFileName,pkgName))
        {
            return "";
        }
        string[] szAry = new string[] {szScene};
        string szMsg = BuildPipeline.BuildStreamedSceneAssetBundle(szAry, file, tgt);
        if (string.IsNullOrEmpty(szMsg))
        {
            AppendResPackItm(szAry, file, tgt);
            version.IncreasePackageVersion(pkgName);
        }
        return szMsg;
    }
    
    //--------------------------------------------------------------------------------------------
    // 启动程序
    static public void BuildGamePlayer(string file, string szScene, BuildTarget tgt, bool bMsgBx)
    {
        BuildOptions op = BuildOptions.None;
        string[] szAry = new string[] {szScene};
        string szMsg = BuildPipeline.BuildPlayer(szAry, file, tgt, op);
        if (bMsgBx)
        {
            if (string.IsNullOrEmpty(szMsg)) szMsg = @"操作成功";
            EditorUtility.DisplayDialog(@"操作已结束", @"消息: " + szMsg, "Ok");
        }
        else
        {
            if (!string.IsNullOrEmpty(szMsg))
                Debug.Log("BuildGamePlayer: Msg = " + szMsg);
        }
    }

    //--------------------------------------------------------------------------------------------
    // 启动程序
    static public void BuildGamePlayer(string file, List<string> scAry, BuildTarget tgt, bool bMsgBx)
    {
        BuildOptions op = BuildOptions.None;
        string szMsg = BuildPipeline.BuildPlayer(scAry.ToArray(), file, tgt, op);
        if (bMsgBx)
        {
            if (string.IsNullOrEmpty(szMsg)) szMsg = @"操作成功. 包含 [" + scAry.Count + @"] 个场景";
            EditorUtility.DisplayDialog(@"操作已结束", @"消息: " + szMsg, "Ok");
        }
        else
        {
            if (!string.IsNullOrEmpty(szMsg))
                Debug.Log("BuildGamePlayer: Msg = " + szMsg);
        }
    }
    
    
    //--------------------------------------------------------------------------------------------
    // 把场景文件也加到资源索引表里
    static void AppendResPackItm(string[] szAry, string fpck, BuildTarget tgt)
    {
        Dictionary<string, ResPackItm> tbl = new Dictionary<string, ResPackItm>();
        string szPckNm = Path.GetFileName(fpck);
        szPckNm = szPckNm.ToLower();
        foreach (string sf in szAry)
        {
            string szKey = Path.GetFileName(sf).ToLower();
            ResPackItm itm = new ResPackItm();
            itm.mType = 1;          // 场景资源
            itm.mfVer = 1;          //// 需要查找对应版本号
            itm.mFile = szKey;
            itm.mPack = szPckNm;
            tbl.Add(szKey, itm);
        }
        AssetBundleUtil.WriteResList(tbl, tgt);
    }












    
    //--------------------------------------------------------------------------------------------
    // 目标平台名字
    static public string GetTargetName(BuildTarget Tgt)
    {
        switch (Tgt)
        {
            case BuildTarget.StandaloneOSXIntel:  { return "MACOS"; }
            case BuildTarget.StandaloneWindows:   { return "Win32"; }
            case BuildTarget.WebPlayer:           { return "WebPlayer"; }
            case BuildTarget.WebPlayerStreamed:   { return "WebPlayerStm"; }
            case BuildTarget.iOS:              { return "iPhone"; }
            case BuildTarget.PS3:                 { return "PS3"; }
            case BuildTarget.XBOX360:             { return "XBOX360"; }
            case BuildTarget.Android:             { return "Android"; }
            //case BuildTarget.StandaloneGLESEmu:   { return "GLESEmu"; }
            case BuildTarget.StandaloneLinux:     { return "LinuxStand"; }
            case BuildTarget.StandaloneWindows64: { return "Win64"; }
            case BuildTarget.WSAPlayer:      { return "Metro86"; }
            //case BuildTarget.MetroPlayerX64:      { return "MetroX64"; }
            //case BuildTarget.MetroPlayerARM:      { return "MetroARM"; }
            case BuildTarget.StandaloneLinux64:   { return "Linux64"; }
            case BuildTarget.StandaloneLinuxUniversal: { return "LinuxUniversal"; }
        }
        return "Unknown";
    }
    
    //--------------------------------------------------------------------------------------------
    // 目标平台扩展名
    static public string GetTargetExt(BuildTarget Tgt)
    {
        switch (Tgt)
        {
            case BuildTarget.StandaloneOSXIntel:  { return "app"; }
            case BuildTarget.StandaloneWindows:   { return "exe"; }
            case BuildTarget.WebPlayer:           { return "exe"; }
            case BuildTarget.WebPlayerStreamed:   { return "exe"; }
            //case BuildTarget.Wii:                 { return "Wii"; }
            case BuildTarget.iOS:              { return "ipa"; }
            case BuildTarget.PS3:                 { return ""; }
            case BuildTarget.XBOX360:             { return ""; }
            case BuildTarget.Android:             { return "apk"; }
            //case BuildTarget.StandaloneGLESEmu:   { return ""; }
            case BuildTarget.StandaloneLinux:     { return ""; }
            case BuildTarget.StandaloneWindows64: { return "exe"; }
            //case BuildTarget.MetroPlayerX86:      { return ""; }
            //case BuildTarget.MetroPlayerX64:      { return ""; }
            //case BuildTarget.MetroPlayerARM:      { return ""; }
            case BuildTarget.StandaloneLinux64:   { return ""; }
            case BuildTarget.StandaloneLinuxUniversal: { return ""; }
        }
        return "Unknown";
    }


}
