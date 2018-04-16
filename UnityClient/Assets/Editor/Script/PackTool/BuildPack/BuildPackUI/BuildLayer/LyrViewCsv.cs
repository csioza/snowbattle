using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class CLyrViewCsv
{
    CListViewCtrl   mLstCtrl;  // Csv文件内容/版本比对
    public string   mMsg = "";

    public CLyrViewCsv(float fw, float fh)
    {
        mLstCtrl  = new CListViewCtrl(fw, fh);
        NtfInitCsvViewHdr(mLstCtrl);
    }

    CListViewCtrl GetListCtrl() { return mLstCtrl; }

    static public void NtfInitCsvViewHdr(CListViewCtrl view)
    {
        GUIStyle stl   = new GUIStyle(EditorStyles.label);
        //stl.alignment = TextAnchor.MiddleCenter;
        view.NtfAddColumn(new CLViewItm(@"File", 320, stl));
        view.NtfAddColumn(new CLViewItm(@"MD5",  280, stl));
        view.NtfAddColumn(new CLViewItm(@"Cur Ver", 80, stl));
        view.NtfAddColumn(new CLViewItm(@"Pre Ver", 80, stl));
        view.NtfAddColumn(new CLViewItm(@"GenTM",   150, stl));
        view.NtfEndInsert();
    }

    
    //--------------------------------------------------------------------------------------------
    // 察看版本表内容
    public Vector2 OnLayout(float fx, float fy)
    {
        GUI.Box(new Rect(fx, fy, 950, 300), "");
        GUI.Label(new Rect(fx, fy + 2, 200, 20), @"当前文件:" + mMsg);

        mLstCtrl.OnDrawListView(fx, fy + 22);

        return new Vector2(950, 300);
    }
    
    //--------------------------------------------------------------------------------------------
    // 把表数据转换成 ViewList 可用数据
    public void GenViewData(CsvParser csv, string szMsg)
    {
        mMsg = szMsg;
        mLstCtrl.NtfClearList();

		Dictionary<int, List<string> > dic = csv.GetRecords();
		if(dic != null)
		{
            mLstCtrl.NtfClearList();
	        foreach (KeyValuePair<int, List<string> > row in dic)
			{
                mLstCtrl.NtfInsertItem(row.Value);
			}
		}
        mLstCtrl.NtfEndInsert();
    }

    
    
    //-----------------------------------------------------------------------------
    // 显示比较结果
    public void GenCompareViewData(VerItemMng mng, string szMsg)
    {
        mMsg = szMsg;
        mLstCtrl.NtfClearList();

        List<CLViewItm> row;
        Dictionary<int, List<CLViewItm>> itmDic = mLstCtrl.GetRecord();

        RyCompareUtil pUtl = mng.GetCompareUtl();
        GUIStyle stlRed = new GUIStyle(EditorStyles.label);
        GUIStyle stlGrn = new GUIStyle(EditorStyles.label);
        stlRed.normal.textColor = Color.red;
        stlGrn.normal.textColor = Color.green;

        row = new List<CLViewItm>();
        row.Add(new CLViewItm(@"单一存在" + "(" + pUtl.mLstSrc.Count + "/" + pUtl.mLstDis.Count + ")"));
        itmDic.Add(itmDic.Count, row);
        AddCmpMulRow(ref itmDic, pUtl.mLstSrc, stlGrn, null);
        AddCmpMulRow(ref itmDic, pUtl.mLstDis, stlRed, null);
        
        row = new List<CLViewItm>();
        row.Add(new CLViewItm(@""));
        itmDic.Add(itmDic.Count, row);
        
        // 都存在但MD5不一样
         row = new List<CLViewItm>();
        row.Add(new CLViewItm(@"MD5不一样" + "(" + pUtl.mDifSrc.Count + "/" + pUtl.mDifDis.Count + ")"));
        itmDic.Add(itmDic.Count, row);
        for (int i = 0; i < pUtl.mDifSrc.Count; ++i)
        {
            AddCmpRow(ref itmDic, pUtl.mDifSrc[i], null, stlGrn);
            AddCmpRow(ref itmDic, pUtl.mDifDis[i], null, stlRed);
        }
        row = new List<CLViewItm>();
        row.Add(new CLViewItm(@""));
        itmDic.Add(itmDic.Count, row);

        // 完全一样的
        row = new List<CLViewItm>();
        row.Add(new CLViewItm(@"完全一样" + "(" + pUtl.mSames.Count + ")"));
        itmDic.Add(itmDic.Count, row);
        AddCmpMulRow(ref itmDic, pUtl.mSames, null, null);
        mLstCtrl.NtfEndInsert();
    }
    
	//------------------------------------------------------------------------------
    void AddCmpMulRow(ref Dictionary<int, List<CLViewItm>> itmDic, List<RyVerItem> pLst, GUIStyle stlGrn, GUIStyle stlRed)
    {
        foreach (RyVerItem val in pLst)
        {
            AddCmpRow(ref itmDic, val, stlGrn, stlRed);
        }
    }
    
	//------------------------------------------------------------------------------
    void AddCmpRow(ref Dictionary<int, List<CLViewItm>> itmDic, RyVerItem itm, GUIStyle stlGrn, GUIStyle stlRed)
    {
        List<CLViewItm> row = new List<CLViewItm>();
        row.Add(new CLViewItm(itm.mItmFile, stlGrn));
        row.Add(new CLViewItm(itm.mItmMd5, stlRed));
        row.Add(new CLViewItm(itm.mCurVer));
        row.Add(new CLViewItm(itm.mPreVer));
        row.Add(new CLViewItm(itm.mGenTM));
        itmDic.Add(itmDic.Count, row);
    }



}
