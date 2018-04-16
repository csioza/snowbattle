using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;



//-------------------------------------------------------------------------------------------------------------------
//
// 控件: ListView Item
//
//-----------------------------------------------------------------------------------------------
public class CLViewItm
{
    public GUIStyle mStyle;
    public float    mWidth;     // 不要修改此值
    public string   mText;
    
    //----------------------------------------------------------------
    public CLViewItm()
    {
        mStyle = new GUIStyle(EditorStyles.label);
        mWidth = 100;
        mText  = "";
    }

    //----------------------------------------------------------------
    public CLViewItm(string szTxt)
    {
        mStyle = new GUIStyle(EditorStyles.label);
        mWidth = 100;
        mText  = szTxt;
    }

    //----------------------------------------------------------------
    public CLViewItm(string szTxt, float fw)
    {
        mStyle = new GUIStyle(EditorStyles.label);
        mWidth = fw;
        mText  = szTxt;
    }

    //----------------------------------------------------------------
    public CLViewItm(string szTxt, float fw, GUIStyle stl)
    {
        if(stl != null) mStyle = new GUIStyle(stl);
        else mStyle = new GUIStyle(EditorStyles.label);
        mWidth = fw;
        mText  = szTxt;
    }

    //----------------------------------------------------------------
    public CLViewItm(string szTxt, GUIStyle stl)
    {
        if(stl != null) mStyle = new GUIStyle(stl);
        else mStyle = new GUIStyle(EditorStyles.label);
        mWidth = 100;
        mText  = szTxt;
    }
    
    //----------------------------------------------------------------
    virtual public void OnDrawItem(ref Rect rct)
    {
        rct.width = mWidth;
        GUI.Label(rct, mText, mStyle);
        rct.x += mWidth;
    }
}

//-------------------------------------------------------------------------------------------------------------------
//
// 控件: ListView 头
//
//-----------------------------------------------------------------------------------------------
public class CLViewHdr
{
    public float        mHeight;
    List<CLViewItm>     mColItm;
    
    //----------------------------------------------------------------
    public CLViewHdr()
    {
        mHeight = 20;
        mColItm = new List<CLViewItm>();
    }
    
    //----------------------------------------------------------------
    public void OnClear()
    {
        mColItm.Clear();
    }

    //----------------------------------------------------------------
    public void NtfAddCol(CLViewItm itm)
    {
        mColItm.Add(itm);
    }
    public void NtfAddCol(string szTxt)
    {
        mColItm.Add(new CLViewItm(szTxt));
    }
    public void NtfAddCol(string szTxt, float fw)
    {
        mColItm.Add(new CLViewItm(szTxt ,fw));
    }
    public void NtfAddCol(string szTxt, float fw, GUIStyle stl)
    {
        mColItm.Add(new CLViewItm(szTxt ,fw, stl));
    }
    
    //----------------------------------------------------------------
    public CLViewItm GetCol(int nCol)
    {
        if((nCol >= 0) && (nCol < mColItm.Count))
            return mColItm[nCol];
        return null;
    }
    
    //----------------------------------------------------------------
    // 添加列或修改列宽时需要手动调用此函数进行重新计算位置
    public float GetViewHdrW()
    {
        float fw = 0;
        for (int i = 0; i < mColItm.Count; ++i)
        {
            CLViewItm itm = mColItm[i];
            fw += itm.mWidth;
        }
        return fw;
    }
    
    //----------------------------------------------------------------
    public void OnDrawLayer(float fx, float fy)
    {
        Rect rct  = new Rect(fx, fy, 50, mHeight);
        for (int i = 0; i < mColItm.Count; ++i)
        {
            CLViewItm itm = mColItm[i];
            itm.OnDrawItem(ref rct);
        }
    }

}

//-------------------------------------------------------------------------------------------------------------------
//
// 控件: ListView
//
//-----------------------------------------------------------------------------------------------
public class CListViewCtrl
{
    public bool                         mbBtn;     // true: 显示按钮
    public float                        mBBoxW;     // 裁剪边框
    public float                        mBBoxH;
    float                               mViewW;     // 数据承载区域
    float                               mViewH;
    int                                 mCurSel;
    Vector2                             mCurPos;
    CLViewHdr                           mLVHdr;     // 头
    Dictionary<int, List<CLViewItm> >	mRecords;   // 数据
    
    //----------------------------------------------------------------
    public CListViewCtrl(float fw, float fh)
    {
        mBBoxW  = fw;
        mBBoxH  = fh;
        mViewW  = 0;
        mViewH  = 0;
        mCurSel = 0;
        mbBtn   = false;
        mCurPos = Vector2.zero;
        mLVHdr  = new CLViewHdr();
        mRecords = new Dictionary<int, List<CLViewItm>>();
    }
    
    //-----------------------------------------------------------------------------
    public void NtfClearList()
    {
        mCurPos = Vector2.zero;
        mRecords.Clear();
        mViewW  = 0;
        mViewH  = 0;
        mCurSel = 0;
    }

    public Dictionary<int, List<CLViewItm>> GetRecord() { return mRecords; }
    
    //-----------------------------------------------------------------------------
    float GetColW(int nCol)
    {
        CLViewItm itm = mLVHdr.GetCol(nCol);
        if (itm != null) return itm.mWidth;
        return 50;
    }

    //-----------------------------------------------------------------------------
    public void NtfResetHdr()
    {
        mLVHdr.OnClear();
        NtfClearList();
    }
    
    //-----------------------------------------------------------------------------
    public void NtfEndInsert()
    {
        mViewW = mLVHdr.GetViewHdrW();
        mViewH = mLVHdr.mHeight;

        foreach (KeyValuePair<int, List<CLViewItm> > row in mRecords)
        {
            for (int i = 0; i < row.Value.Count; ++i )
            {
                CLViewItm itm = row.Value[i];
                itm.mWidth = GetColW(i);
            }
            mViewH += mLVHdr.mHeight;
        }
    }
        
    //----------------------------------------------------------------
    public void NtfAddColumn(CLViewItm itm)
    {
        mLVHdr.NtfAddCol(itm);
    }
    public void NtfAddColumn(string szTxt)
    {
        mLVHdr.NtfAddCol(szTxt);
    }
    public void NtfAddColumn(string szTxt, float fw)
    {
        mLVHdr.NtfAddCol(szTxt, fw);
    }
    public void NtfAddColumn(string szTxt, float fw, GUIStyle stl)
    {
        mLVHdr.NtfAddCol(szTxt, fw, stl);
    }

    //----------------------------------------------------------------
    // 添加 一整行
    public void NtfInsertItem(List<string> szRow)
    {
        if (szRow.Count > 0)
        {
            List<CLViewItm> row = new List<CLViewItm>();
            foreach (string szTxt in szRow)
            {
                row.Add(new CLViewItm(szTxt));
            }
            mRecords.Add(mRecords.Count, row);
        }
    }
    //----------------------------------------------------------------
    // 添加 一整行
    public void NtfInsertItem(List<CLViewItm> row)
    {
        if (row.Count > 0)
        {
            mRecords.Add(mRecords.Count, row);
        }
    }

    //----------------------------------------------------------------
    // 添加 一行的第一列
    public int NtfInsertItem(string szTxt)
    {
        List<CLViewItm> row = new List<CLViewItm>();
        row.Add(new CLViewItm(szTxt));
        int nCount = mRecords.Count;
        mRecords.Add(nCount, row);
        return nCount;
    }
    
    //----------------------------------------------------------------
    // 添加 一行的第一列
    public int NtfInsertItem(CLViewItm itm)
    {
        List<CLViewItm> row = new List<CLViewItm>();
        row.Add(itm);
        int nCount = mRecords.Count;
        mRecords.Add(nCount, row);
        return nCount;
    }
    
    //----------------------------------------------------------------
    // 顺序增加列
    public void NtfAddSubItem(int nRow, int nCol, string szTxt)
    {
        if (nCol < 0) return;
        List<CLViewItm> row = null;
        if (mRecords.TryGetValue(nRow, out row))
        {
            if (nCol >= row.Count)
            {
                row.Add(new CLViewItm(szTxt));
                return;
            }
            row[nCol].mText = szTxt;
        }
    }
    
    //----------------------------------------------------------------
    // 顺序增加列
    public void NtfAddSubItem(int nRow, int nCol, CLViewItm itm)
    {
        if (nCol < 0) return;
        List<CLViewItm> row = null;
        if (mRecords.TryGetValue(nRow, out row))
        {
            if (nCol >= row.Count)
            {
                row.Add(itm);
                return;
            }
            row[nCol] = itm;
        }
    }
    
    //----------------------------------------------------------------
    public CLViewItm GetItem(int nRow, int nCol)
    {
        if (nCol < 0) return null;
        List<CLViewItm> row = null;
        if (mRecords.TryGetValue(nRow, out row))
        {
            if (nCol < row.Count)
                return row[nCol];
        }
        return null;
    }
    //----------------------------------------------------------------
    public bool OnDrawListView(float fx, float fy)
    {
        bool bDirty = false;
        GUI.Box(new Rect(fx, fy, mBBoxW, mBBoxH), "");
        {
            float foff = mLVHdr.mHeight;
            mLVHdr.OnDrawLayer(fx -mCurPos.x, fy);
            mCurPos = GUI.BeginScrollView(new Rect(fx, fy+foff, mBBoxW, mBBoxH), mCurPos, new Rect(0, foff, mViewW, mViewH));
            {
                fx = 0; fy = 0;

                float fBtn = mLVHdr.mHeight * 0.7f;
                Rect rct  = new Rect(fx, fy, 50, mLVHdr.mHeight);
                foreach (KeyValuePair<int, List<CLViewItm> > row in mRecords)
                {
                    rct.x = fx;
                    rct.y += mLVHdr.mHeight;
                    if (mbBtn)
                    {
                        if (GUI.Button(new Rect(rct.x, rct.y, 20, fBtn), @"  "))
                        {
                            if (row.Key != mCurSel)
                            {
                                bDirty = true;
                                mCurSel = row.Key;
                            }
                        }
                        rct.x += 20;
                    }
                    
                    foreach (CLViewItm itm in row.Value)
                    {
                        itm.OnDrawItem(ref rct);
                    }
                }
            }
            GUI.EndScrollView();
        }
        return bDirty;
    }
}
