
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public abstract class BundleVersionControl
{
    //更新文件在当前版本中对应的MD5,如果不同，返回true
    public abstract bool UpdateFileMD5(string fileWith, string pkg);
    //增加包的版本号
    public abstract void IncreasePackageVersion(string packageName);
}

//-------------------------------------------------------------------------------------------------------------------
public class BuildArgs
{
    // 输入
    public string       vPack;  // 包文件
    public List<string> fAry;   // 输入的文件列表

    // 输出
    public List<string> kAry = new List<string>();     // BuildAssetBundleExplicitAssetNames 用的 Name
    public Dictionary<string, ResPackItm> vTbl = new Dictionary<string,ResPackItm>();
}


//-------------------------------------------------------------------------------------------------------------------
//
// 命令集合: 构建 AssetBundle 文件
//
public class AssetBundleUtil
{
    //--------------------------------------------------------------------------------------------
    // 通过 Unity 创建 AssetBundle 文件
    static public bool BuildAssetBundle(Object[] objAry, string saveFile, BuildTarget tgt)
    {
        BuildAssetBundleOptions ops = BuildAssetBundleOptions.CollectDependencies;
        //ops |= BuildAssetBundleOptions.UncompressedAssetBundle;
        return BuildPipeline.BuildAssetBundle(null, objAry, saveFile, ops, tgt);
    }
    static public bool BuildAssetBundle(List<Object> objList, string saveFile, BuildTarget tgt)
    {
        UnityEngine.Object[] objAry = objList.ToArray();
        return BuildAssetBundle(objAry, saveFile, tgt);
    }
    
    //--------------------------------------------------------------------------------------------
    // 通过 Unity 创建 AssetBundle 文件, 自定义对应的KEY
    static public bool BuildAssetBundleWithNames(Object[] objAry, string [] kAry, string sfile, BuildTarget tgt)
    {
        if((objAry.Length <= 0) || (objAry.Length > kAry.Length)) return false;
        BuildAssetBundleOptions ops = BuildAssetBundleOptions.CollectDependencies;
        //ops |= BuildAssetBundleOptions.UncompressedAssetBundle;
        return BuildPipeline.BuildAssetBundleExplicitAssetNames(objAry, kAry, sfile, ops, tgt);
    }
    static public bool BuildAssetBundleWithNames(List<Object> objList, List<string> kAry, string sfile, BuildTarget tgt)
    {
        UnityEngine.Object[] objAry = objList.ToArray();
        string [] nmAry = kAry.ToArray();
        return BuildAssetBundleWithNames(objAry, nmAry, sfile, tgt);
    }

    //--------------------------------------------------------------------------------------------
    // 搜索打包用文件
    static public List<string> SearchPackFiles(List<string> pthLst)
    {
        List<string> files = new List<string>();
        foreach (string pth in pthLst)
        {
            List<string> fList = ArchiveUtil.NtfGetFiles(pth, true);
            files.AddRange(fList);
        }
        return files;
    }

    //--------------------------------------------------------------------------------------------
    // 生成对应文件的 Object 对象
    static public List<Object> BuildObjectList(ref BuildArgs vArgs, BundleVersionControl version,ref bool isNeedPackage)
    {
        isNeedPackage = false;
        string szPckNm = Path.GetFileName(vArgs.vPack);
        szPckNm = szPckNm.ToLower();
        List<Object> objList = new List<Object>();
        foreach (string fileName in vArgs.fAry)
        {
            string szt = fileName.ToLower();
            if (szt.EndsWith(".unity"))
            {
                Debug.LogWarning("BuildPipeline Skip file = " + fileName);
                continue;
            }
            if (szt.EndsWith(".unitypackage"))
            {
                Debug.LogWarning("BuildPipeline Skip file = " + fileName);
                continue;
            }

            string fl = ArchiveUtil.NtfPathBeginAssets(fileName);
            fl = fl.Replace('\\', '/');
            string szKey = ArchiveUtil.NtfPathAfterAssets(fl).ToLower();
            if (version != null && version.UpdateFileMD5(szKey, szPckNm) == false)
            {//MD5相同，不用打包
                
            }
            else
            {
                isNeedPackage = true;
            }

            Object obj = AssetDatabase.LoadMainAssetAtPath(fl);
            if (null == obj)
            {
                Debug.LogWarning("BuildPipeline LOAD failed file = " + fileName);
                continue;
            }
            objList.Add(obj);

            vArgs.kAry.Add(szKey);
    
            ResPackItm itm = new ResPackItm();
            itm.mType = 0;          // 普通资源
            itm.mfVer = 1;          //// 需要查找对应版本号
            itm.mFile = szKey;
            itm.mPack = szPckNm;
            vArgs.vTbl.Add(szKey, itm);
        }
        //更新package对应的版本号
        if (version != null && isNeedPackage)
        {
            version.IncreasePackageVersion(szPckNm);
        }
        return objList;
    }
    

    //--------------------------------------------------------------------------------------------
    //
    // 创建对应 平台的 AssetBundle 文件
    //
    // 根据目录构建. 注意: 忽略 ArchiveUtil.mSkips 里相关的
    static public bool ExportToBundle(string szPath, string saveFile, BuildTarget tgt, BundleVersionControl version=null)
    {
        List<string> fList = ArchiveUtil.NtfGetFiles(szPath, true);
        return ExportToBundle(fList, saveFile, tgt,version);
    }

    // 根据文件列表构建
    static public bool ExportToBundle(List<string> fList, string saveFile, BuildTarget tgt,BundleVersionControl version=null)
    {
        BuildArgs vArgs = new BuildArgs();
        vArgs.vPack = saveFile;
        vArgs.fAry  = fList;

        //检查文件对应的MD5与当前版本里的MD5是否对应，如果不对应，则加入打包列表，并且修改对应的包的版本号
        bool isNeedPackage = false;
        List<Object> objList = BuildObjectList(ref vArgs, version, ref isNeedPackage);
        if (isNeedPackage)
        {
            if (BuildAssetBundleWithNames(objList, vArgs.kAry, saveFile, tgt))
            {
                WriteResList(vArgs.vTbl, tgt);
                return true;
            }
            return false;
        }
        return true;
    }
    
    //--------------------------------------------------------------------------------------------
    // 保存资源列表, 存在则合并
    public static void WriteResList(Dictionary<string, ResPackItm> tbl, BuildTarget tgt)
    {
        Dictionary<string, ResPackItm> ntbl = tbl;
        string sPth = BuildGameCMD.GetBuildFolder(tgt);
        sPth = sPth.Replace("\\", "/");
        if (!sPth.EndsWith("/"))
        {
            sPth += "/";
        }
        string sf =  sPth + ResPath.GetResListTxt();
        if (File.Exists(sf))
        {
            try
            {
                FileStream stm = new FileStream(sf, FileMode.Open, FileAccess.Read);
                if (stm.Length > 0)
                {
                    byte[] vAry = new byte[stm.Length];
                    stm.Read(vAry, 0, vAry.Length);
                    Dictionary<string, ResPackItm> otbl = ResUtil.ReadResTable(vAry);
                    ntbl = ResUtil.ReplaceAndMergeResTable(tbl, otbl);
                }
                stm.Close();
            }
            catch (Exception exp)
            {
                Debug.LogWarning("CAN NOT Open file = " + sf + ", Msg = " + exp.Message);
            }
        }
        ResUtil.WriteResTable(sf, ntbl);
    }






}
