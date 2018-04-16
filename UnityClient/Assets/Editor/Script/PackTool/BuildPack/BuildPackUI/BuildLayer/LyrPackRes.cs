using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

//-------------------------------------------------------------------------------------------------------------------
//
// 窗口布局: 资源打包, 包括整包/更新包/手动指定
//
//-----------------------------------------------------------------------------------------------
public class CLyrPackRes
{
    List<string>    mFileLst;   // 打包用的文件

    // 打包全部资源
    CsvParser	    mCsvFile;   // 对应Csv文件
    List<string>    mFixFile;   // Csv里的文件
    CViewList       mFixView;
    
    // 打包差异资源: 打包 mfCsv0 与 mfCsv1 的差异文件
    // 差异文件: 1> mfCsv0 里存在而 mfCsv1 里不存在
    //           2> 都存在但 MD5 不一样
    string          mfCsv0;     // Csv 版本文件
    string          mfCsv1;     // Csv 版本文件
    List<string>    mCmpFile;
    CViewList       mCmpView;
    
    // EditorBuildSettings
    Vector2                mCurPos;         // BuildSetting列表

    //--------------------------------------------------------------------------------------------
    public CLyrPackRes()
    {
        mCsvFile = new CsvParser();
        mFileLst = new List<string>();
        mFixFile = new List<string>();
        mFixView = new CViewList(400, 225);

        mfCsv0 = "PackVers/" + PackVerCfg.GetCurVer() + ".csv";
        mfCsv1 = PackVerCfg.GetPreVer();
        if (mfCsv1.Length > 0) mfCsv1 = "PackVers/" + mfCsv1 + ".csv";
        mCmpFile = new List<string>();
        mCmpView = new CViewList(400, 175);

    }
    
    public List<string> GetPackFiles() { return mFileLst; }

    //--------------------------------------------------------------------------------------------
    public Vector2 OnLayout(BuildPackWindow src, float fx, float fy)
    {
        GUI.Box(new Rect(fx, fy, 400, 300), "");
        return new Vector2(400, 300);
    }
    
    //------------------------------------------------------------------------------------------------------------------
    //
    // 将全部资源进行打包
    //
    //--------------------------------------------------------------------------------------------
    // 将全部资源进行打包
    void OnLyrPackClient(BuildPackWindow src, float fx, float fy)
    {
        if (GUI.Button(new Rect(fx, fy - 2, 40, 20), @"查找"))
        {
		    string verPth = "PackVers/";
		    string szf = EditorUtility.OpenFilePanel(@"版本文件", verPth, "csv");
            if (szf.Length > 0)
            {
                mCsvFile.loadCsvFile(szf, true);
                src.GenViewData(mCsvFile, szf);
                GenFromCsv();
            }
        }

        mFixView.OnDrawView(fx, fy + 25, mFixFile.ToArray());
    }

    //--------------------------------------------------------------------------------------------
    void GenFromCsv()
    {
        mFixFile.Clear();
        string sItm = "";
        int nCount = mCsvFile.getRecordsCounts();
        for (int i = 0; i < nCount; ++i)
        {
            mCsvFile.GetString(out sItm, i, "File");
            if (sItm.Length > 0)
            {
                mFixFile.Add(sItm);
            }
        }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    //
    // 打包升级用资源
    //
    //--------------------------------------------------------------------------------------------
    // 打包升级用资源
    void OnLyrPackUpdate(BuildPackWindow src, float fx, float fy)
    {
        string verPth = "PackVers/";

        if (GUI.Button(new Rect(fx + 355, fy, 40, 20), @"比较"))
        {
            NtfCompare(src);
        }

        float fw = 335;
        GUI.Label(new Rect(fx, fy + 25, fw, 20), @"基础版本: " + mfCsv0);
        if (GUI.Button(new Rect(fx + fw + 20, fy + 25, 40, 20), @"查找"))
        {
            string fl = EditorUtility.OpenFilePanel(@"源版本文件", verPth, "csv");
            if (fl.Length > 0)
            {
                mfCsv0 = ArchiveUtil.NtfPathBeginPackVers(fl);
                NtfCompare(src);
            }
        }
        
        GUI.Label(new Rect(fx, fy + 50, fw, 20), @"比对版本: " + mfCsv1);
        if (GUI.Button(new Rect(fx + fw + 20, fy + 50, 40, 20), @"查找"))
        {
            string fl = EditorUtility.OpenFilePanel(@"目标版本文件", verPth, "csv");
            if (fl.Length > 0)
            {
                mfCsv1 = ArchiveUtil.NtfPathBeginPackVers(fl);
                NtfCompare(src);
            }
        }

        mCmpView.OnDrawView(fx, fy + 75, mCmpFile.ToArray());

    }
    
    //--------------------------------------------------------------------------------------------
    public void NtfCompare(BuildPackWindow src)
    {
        if((mfCsv0.Length > 0) && (mfCsv1.Length > 0))
        {
            VerItemMng vMng = src.mVerMng;
            vMng.NtfCompareItemGrpFile(mfCsv0, mfCsv1);
            src.GenCompareView(@"比较结果");

            RyCompareUtil pUtl = vMng.GetCompareUtl();
            mCmpFile.Clear();
            NtfAddCmpResult(ref mCmpFile, pUtl.mLstSrc);
            NtfAddCmpResult(ref mCmpFile, pUtl.mDifSrc);
        }
    }
    
    //--------------------------------------------------------------------------------------------
    void NtfAddCmpResult(ref List<string> fLst, List<RyVerItem> pLst)
    {
        foreach (RyVerItem val in pLst)
        {
            fLst.Add(val.mItmFile);
        }
    }
        
    //-----------------------------------------------------------------------------
    // 删除记录
    void NtfDelRow(ref List<string> pLst, ref int nSel)
    {
        if (pLst.Count > 0)
        {
            if (nSel >= pLst.Count)
                nSel = pLst.Count - 1;
            pLst.RemoveAt(nSel);
            if (nSel >= pLst.Count)
                nSel = pLst.Count - 1;
        }
    }

}
