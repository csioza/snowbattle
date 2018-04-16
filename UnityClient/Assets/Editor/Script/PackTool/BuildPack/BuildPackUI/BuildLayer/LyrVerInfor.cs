using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

//-------------------------------------------------------------------------------------------------------------------
//
// 窗口布局: 版本信息
//
//-----------------------------------------------------------------------------------------------
public class CLyrVerInfor
{
    int         mCurPg = 0;
    CViewList   mViewList;      // 版本文件列表
    CsvParser	mCsvFile;

    // 版本制作以及比对
    List<string>	mMD5List;	// 需要生成MD5的文件列表
    CViewList       mMD5View;



    //--------------------------------------------------------------------------------------------
    public CLyrVerInfor()
    {
        mCsvFile  = new CsvParser();
        mViewList = new CViewList(340, 255);

        // 版本制作
        mMD5List  = new List<string>();
        mMD5View  = new CViewList(340, 255);
       
    }


    //--------------------------------------------------------------------------------------------
    // 版本制作
    public Vector2 OnLyrVerTool(BuildPackWindow src, Vector2 vPos)
    {
        GUI.Box(new Rect(vPos.x, vPos.y, 340, 300), "");
        string [] ary = {@"版本信息", @"版本制作"};
        mCurPg = GUI.Toolbar(new Rect(vPos.x, vPos.y, 340, 20), mCurPg, ary);
        if (0 == mCurPg)
            OnLyrVerInfor(src, vPos.x, vPos.y + 25);
        else if(1 == mCurPg)
            OnLyrMakeVerInfor(src, vPos.x, vPos.y + 25);

        return new Vector2(340, 300);
    }

    
    //--------------------------------------------------------------------------------------------
    Vector2 OnLyrVerInfor(BuildPackWindow src, float fx, float fy)
    {
        GUI.Label(new Rect(fx, fy + 2, 150, 20), @"当前版本:" + PackVerCfg.GetCurVer());
        
		if(GUI.Button(new Rect(fx + 150, fy - 2, 50, 20), @"刷 新"))
		{
			src.mVerMng.NtfSearchPackVers();
		}
        
        GUI.Label(new Rect(fx + 210, fy + 2, 80, 20), @"版本文件列表:");
        List<string> sList = src.mVerMng.GetVerList();
        if (mViewList.OnDrawView(fx, fy + 20, sList.ToArray()))
        {
            string szf = src.GetVerFile(mViewList.mHitRow);
            mCsvFile.loadCsvFile(szf, true);
            src.GenViewData(mCsvFile, szf);
        }

        return new Vector2(340, 280);
    }
    
        
	//------------------------------------------------------------------------------
    // 版本制作
    Vector2 OnLyrMakeVerInfor(BuildPackWindow src, float fx, float fy)
    {
		if(GUI.Button(new Rect(fx, fy - 2, 70, 20), @"设置目录"))
		{
			string sel = EditorUtility.OpenFolderPanel("Search Files", Application.dataPath, "");
            if (sel.Length > 0)
            {
                mMD5List.Clear();
			    mMD5List = null;
                mMD5List = ArchiveUtil.NtfGetFiles(sel, true, ArchiveUtil.mSkips);
            }
		}
        if (GUI.Button(new Rect(fx + 75, fy - 2, 40, 20), @"生成"))
        {
            OnClickBtnGenMD5(src);
        }
        if (GUI.Button(new Rect(fx + 115, fy - 2, 40, 20), @"比对"))
        {
            OnClickBtnCompare(src);
        }

        string szTxt = "";
		if(ArchiveUtil.mArMsg.Length > 0)
		{
			szTxt = ArchiveUtil.mArMsg;
		}
		else if(mMD5List != null)
		{
			szTxt = @"选中文件: " + mMD5List.Count;
		}
        GUI.Label(new Rect(fx + 220, fy + 2, 150, 20), szTxt);

        if (mMD5List != null)
        {
            mMD5View.OnDrawView(fx, fy + 20, mMD5List.ToArray());
        }
        return new Vector2(340, 280);
    }
    
	//------------------------------------------------------------------------------
    void OnClickBtnGenMD5(BuildPackWindow src)
    {
        if (mMD5List.Count > 0)
        {
            string verPth = "PackVers/";
            string fnm = PackVerCfg.GenNextVer();
            string file = EditorUtility.SaveFilePanel("Save Version", verPth, fnm, "csv");
            if (file.Length > 0)
            {
                src.mVerMng.GenItemGrp(file, mMD5List);
                PackVerCfg.NtfUpdateVer();
                src.mVerMng.NtfSearchPackVers();		// 刷新版本文件列表
                string msg = @"版本文件列表已生成, 保存于[" + file + "]";
                EditorUtility.DisplayDialog(@"版本文件生成", msg, "Ok");
            }
        }
    }

	//------------------------------------------------------------------------------
    void OnClickBtnCompare(BuildPackWindow src)
    {
		string verPth = "PackVers/";
		string file1 = EditorUtility.OpenFilePanel(@"源版本文件", verPth, "csv");
		if(file1.Length > 0)
		{
			string file2 = EditorUtility.OpenFilePanel(@"目标版本文件", verPth, "csv");
			if(file2.Length > 0)
			{
                src.mVerMng.NtfCompareItemGrpFile(file1, file2);
                src.GenCompareView(@"比较结果");
			}
		}
    }


}
