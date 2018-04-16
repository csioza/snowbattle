
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

//-------------------------------------------------------------------------------------------------------------------
//
// 窗口: 游戏资源包发布窗口基类
//
public class BuildPackWindow : EditorWindow
{
    protected CLyrPackRes       mPckLyr;        // 打包界面
    protected CLyrVerInfor      mVerLyr;        // 底部
    CLyrViewCsv                 mCsvView;
    public VerItemMng           mVerMng;        // 版本管理器

    
    //--------------------------------------------------------------------------------------------
    // 启动
    void OnEnable()
    {
        ArchiveUtil.NtfInitSkips();
        PackVerCfg.NtfLoadVerCfg("PackVers/VerConfig.txt");
        mVerMng = new VerItemMng();
        mPckLyr = new CLyrPackRes();
        mVerLyr = new CLyrVerInfor();

        mCsvView  = new CLyrViewCsv(950, 280);
        

        mVerMng.NtfSearchPackVers();	// 搜索已经存在的版本文件
        //mPckLyr.NtfCompare(this);
    }

    //--------------------------------------------------------------------------------------------
    // 界面布局
    void OnGUI()
    {
        if ((null == mPckLyr) || (null == mVerLyr))
            return;
        {
            Vector2 vS1 = mPckLyr.OnLayout(this, 5, 5);
            mVerLyr.OnLyrVerTool(this, new Vector2(vS1.x + 10, 5));

            mCsvView.OnLayout(0, 10);
        }
    }

    //--------------------------------------------------------------------------------------------
    //
    // 具体布局
    //
    //-----------------------------------------------------------------------------
    // 打包除场景(*.unity)以外的资源
    //public void OnClickBtnBuildPack()
    //{
    //    string szPath = BuildGameCMD.GetBuildFolder(mTgtLyr.mTarget);
    //    string file = EditorUtility.SaveFilePanel("Save Pack File", szPath, mPckLyr.mResName, "pack");
    //    if(file.Length > 0)
    //        BuildGameCMD.BuildGamePack(file, mPckLyr.GetPackFiles(), mTgtLyr.mTarget);
    //    this.Close();
    //}
    
    //-----------------------------------------------------------------------------
    // 打包场景(*.unity)资源
    //public void OnClickBtnBuildScene()
    //{
    //    BuildTarget tgt = mTgtLyr.mTarget;
    //    string szTgtNm = BuildGameCMD.GetTargetName(tgt);
    //    string szPath = BuildGameCMD.GetBuildFolder(tgt);
    //    if (mPckLyr.IsSingleUnity())
    //    {
    //        string file = EditorUtility.SaveFilePanel("Save Scene File", szPath, szTgtNm, "unity3d");
    //        if (file.Length > 0)
    //            BuildGameCMD.BuildGameScene(file, mPckLyr.GetScnFiles(), tgt);
    //    }
    //    else
    //    {
    //        string szMsg;
    //        int nCount = 0;
    //        List<string> pLst = mPckLyr.GetScnFiles();
    //        foreach (string sf in pLst)
    //        {
    //            string nsf = szPath + "/" + ArchiveUtil.GetFileName(sf) + ".unity3d";
    //            szMsg = BuildGameCMD.BuildGameScene(nsf, sf, tgt);
    //            if (!string.IsNullOrEmpty(szMsg)) Debug.LogWarning("BuildGameScene Msg = " + szMsg);
    //            else ++nCount;
    //        }
    //        szMsg = @"消息: 成功打包 [";
    //        szMsg += (nCount.ToString() + @"] 个文件, 有 [");
    //        szMsg += ((pLst.Count - nCount).ToString() + @"] 个文件打包失败");
 
    //        EditorUtility.DisplayDialog(@"操作已结束", szMsg, "Ok");
    //    }
    //    this.Close();
    //}
   
    //-----------------------------------------------------------------------------
    // 构建游戏启动程序
    //public void OnClickBtnBuildPlayer()
    //{
    //     BuildTarget tgt = mTgtLyr.mTarget;
    //    string szTgtNm = BuildGameCMD.GetTargetName(tgt);
    //    string szPath  = BuildGameCMD.GetBuildFolder(tgt);
    //    string szEx    = BuildGameCMD.GetTargetExt(tgt);
    //    string szTitle = "Save " + szTgtNm + " App";
    //    string szApp   = szTgtNm + "App";
    //    string file = EditorUtility.SaveFilePanel(szTitle, szPath, szApp, szEx);
    //    if (file.Length > 0)
    //    {
    //        BuildGameCMD.BuildGamePlayer(file, mPckLyr.GetStartScene(), tgt, true);
    //    }
    //    this.Close();
    //}

    //-----------------------------------------------------------------------------
    public string GetVerFile(int nIndx)
    {
        return mVerMng.GetVerFile(nIndx);
    }

    public void GenViewData(CsvParser csv, string szMsg)
    {
        mCsvView.GenViewData(csv, szMsg);
    }
    public void GenCompareView(string szMsg)
    {
        mCsvView.GenCompareViewData(mVerMng, szMsg);
    }












}
