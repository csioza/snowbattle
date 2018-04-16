using UnityEngine;
using System.Collections.Generic;


public delegate void OnClickLvBtn(LvItm itm);

public enum LvAlign
{
    LIA_LEFT = 0,   // 从左往右
    LIA_CENTER,     // 平均
    LIA_RIGHT,      // 靠右
}

//-------------------------------------------------------------------------------------------------
// ListViewCtrl Item
public class LvItm
{
    public LvAlign          mTye  = 0;  // 位置类型:0(从左往右)/1(平均)/2(靠右)
    public int              mfont = 14; // 字体大小
    public bool             mEnable;    // 是否激活
    public float            mWidth;     // 宽度, 放在一行里时用到
    public string           mLabel;     // 显示的文字
    public Color            mColor;     // 文本颜色
    public TextAnchor       mAnchor;    // 文字对齐方式

    public LvItm()
    {
        mWidth = 200;
        mEnable = true;
        mLabel = "LvItm";
        mColor = Color.white;
        mAnchor = TextAnchor.MiddleLeft;
    }
    // 绘制时高度
    public float GetFontH()
    {
        // 12号字体对应 20 高度, 以此比例计算不同字体高度
        float fs = mfont/12.0f;
        return (20 * fs);
    }

    // 绘制时高度
    virtual public float GetDrawH(){ return GetFontH(); }
    virtual public bool IsColGrp(){ return false; }

    public void ApplyEnable(bool bEnable)
    {
        GUI.enabled = IsEnable() && bEnable;
    }

    virtual public void OnDrawLvItm(float cx, ref float cy, float fw, bool bEnable, OnClickLvBtn func)
    {
        float myH = GetDrawH();
        GUI.skin.label.fontSize  = mfont;
        GUI.skin.label.alignment = mAnchor;
        GUI.skin.label.normal.textColor = mColor;
        Rect rct = new Rect(cx, cy, fw, myH);
        GUI.Label(rct, mLabel);
        cy += myH;
    }

    virtual public bool IsEnable() { return mEnable; }
    //-------------------------------------------------------------------------------------------------
    virtual public LvItm GetSubItem(int nIndx){return null;}
    virtual protected void PushToAry(LvItm itm){}
    
    // bCol = true 取最大高度, 否则叠加
    static public float CalDrawH(List<LvItm> vAry, bool bCol)
    {
        float fH = 0;
        foreach (LvItm itm in vAry)
        {
            float fv = itm.GetDrawH();
            if(bCol) fH = Mathf.Max(fH, fv);
            else fH += fv;
        }
        return fH;
    }

    public LvItmLabel AddLvLabel(string sLabel, float fw, Color clr)
    {
        LvItmLabel itm = new LvItmLabel();
        itm.mLabel = sLabel;
        itm.mColor = clr;
        itm.mWidth = fw;
        PushToAry(itm);
        return itm;
    }

    public LvItmBtn AddLvBtn(string sLabel, float fw, Color clr)
    {
        LvItmBtn itm = new LvItmBtn();
        itm.mLabel = sLabel;
        itm.mColor = clr;
        itm.mWidth = fw;
        PushToAry(itm);
        return itm;
    }
    
    //-------------------------------------------------------------------------------------------------
    public LvItmLabelEx AddLvLabelEx(ResPackge pck)
    {
        LvItmLabelEx itm = new LvItmLabelEx();
        itm.mLabel  = pck.mFile;  // 显示的文字
        itm.mRefPck = pck;
        PushToAry(itm);
        return itm;
    }

    public LvItmBtnEx AddLvBtnEx(string sLabel, ResPackge pck)
    {
        LvItmBtnEx itm = new LvItmBtnEx();
        itm.mLabel  = sLabel;  // 显示的文字
        itm.mRefPck = pck;
        PushToAry(itm);
        return itm;
    }
    

    public LvColGrp AddLvColGrp(LvAlign nType = LvAlign.LIA_LEFT)
    {
        LvColGrp itm = new LvColGrp();
        itm.mTye   = nType;
        itm.mLabel = "";
        PushToAry(itm);
        return itm;
    }
}
//-------------------------------------------------------------------------------------------------
// 分割行
public class LvItmEmpty : LvItm
{
    public float mHeight = 5;
    public override float GetDrawH() { return mHeight; }
    public override void OnDrawLvItm(float cx, ref float cy, float fw, bool bEnable, OnClickLvBtn func)
    {
        cy += GetDrawH();
    }
}

//-------------------------------------------------------------------------------------------------
// Label
public class LvItmLabel : LvItm
{
    public override void OnDrawLvItm(float cx, ref float cy, float fw, bool bEnable, OnClickLvBtn func)
    {
        base.OnDrawLvItm(cx, ref cy, fw, bEnable, func);
    }
}

//-------------------------------------------------------------------------------------------------
// Button
public class LvItmBtn : LvItm
{
    public override void OnDrawLvItm(float cx, ref float cy, float fw, bool bEnable, OnClickLvBtn func)
    {
        ApplyEnable(bEnable);
        float myH = GetDrawH();
        GUI.skin.button.fontSize  = mfont;
        GUI.skin.button.alignment = mAnchor;
        GUI.skin.button.normal.textColor = mColor;
        Rect rct = new Rect(cx, cy, fw, myH);
        if (GUI.Button(rct, mLabel))
            func(this);
        cy += myH;
    }
}

//-------------------------------------------------------------------------------------------------
// Label Ex
public class LvItmLabelEx : LvItm
{
    public ResPackge mRefPck;
    public override void OnDrawLvItm(float cx, ref float cy, float fw, bool bEnable, OnClickLvBtn func)
    {
        mLabel = mRefPck.GetStateMsg() + ": " + mRefPck.mFile;
        base.OnDrawLvItm(cx, ref cy, fw, bEnable, func);
    }
}

//-------------------------------------------------------------------------------------------------
// Button Ex
public class LvItmBtnEx : LvItmBtn
{
    public ResPackge mRefPck;

    public override bool IsEnable()
    {
        if (null == mRefPck)
            return false;
        return mRefPck.IsDone();
    }

    public override void OnDrawLvItm(float cx, ref float cy, float fw, bool bEnable, OnClickLvBtn func)
    {
        base.OnDrawLvItm(cx, ref cy, fw, bEnable, func);
    }
}

//-------------------------------------------------------------------------------------------------
// 一行里的数据
public class LvColGrp : LvItm
{
    float           mGrpH = 0;
    List<LvItm>     mLvAry = new List<LvItm>();       // 一行里的 项, 只加 LvItm
    public  bool IsColGrp(){ return true; }
    public void ClearLvGrp(){mLvAry.Clear();}         // 未判断是否有效
    public int GetCount(){return mLvAry.Count;}       // 未判断是否有效
    public override float GetDrawH(){return mGrpH;}
    public void UpdateDrawH(){mGrpH = CalDrawH(mLvAry, true);}
    protected override void PushToAry(LvItm itm){mLvAry.Add(itm);}
    
    public override void OnDrawLvItm(float cx, ref float cy, float fw, bool bEnable, OnClickLvBtn func)
    {
        if (mLvAry.Count <= 0)
        {
            cy += mGrpH;
            return;
        }
        float fy = cy;
        bool bVal = mEnable && bEnable;
        switch (mTye)
        {
            case LvAlign.LIA_LEFT:
            {
                foreach (LvItm itm in mLvAry)
                {
                    fy = cy;
                    itm.OnDrawLvItm(cx, ref fy, itm.mWidth, bVal, func);
                    cx += itm.mWidth;
                }
                break;
            }
            case LvAlign.LIA_CENTER:
            {
                float fnw = fw / mLvAry.Count;
                foreach (LvItm itm in mLvAry)
                {
                    fy = cy;
                    itm.OnDrawLvItm(cx, ref fy, fnw, bVal, func);
                    cx += fnw;
                }
                break;
            }
            case LvAlign.LIA_RIGHT:
            {
                cx = fw;
                for (int i = mLvAry.Count - 1; i >= 0; --i)
                {
                    fy = cy;
                    LvItm itm = mLvAry[i];
                    cx -= itm.mWidth;
                    itm.OnDrawLvItm(cx, ref fy, itm.mWidth, bVal, func);
                }
                break;
            }
        }
        cy += mGrpH;
    }

    public override LvItm GetSubItem(int nIndx)
    {
        if(mLvAry.Count <= nIndx) return null;
        return mLvAry[nIndx];
    }
}

//-------------------------------------------------------------------------------------------------
// 一行里的数据
public class LvItmGrp : LvItm
{
    bool            mbExp = true;
    float           mGrpH = 0;
    List<LvItm>     mLvAry = new List<LvItm>();       // 一行里的 项, 只加 LvItm
    public void ClearLvGrp(){mLvAry.Clear();}         // 未判断是否有效
    public int GetCount(){return mLvAry.Count;}       // 未判断是否有效
    public override float GetDrawH(){return mGrpH;}
    public void UpdateDrawH(){mGrpH = CalDrawH(mLvAry, true);}
    protected override void PushToAry(LvItm itm){mLvAry.Add(itm);}
    
    public override void OnDrawLvItm(float cx, ref float cy, float fw, bool bEnable, OnClickLvBtn func)
    {
        float myH = GetFontH();
        float dx = NtfDrawBtn(cx, cy, myH);

        // 描述
        GUI.skin.label.fontSize  = mfont;
        GUI.skin.label.alignment = mAnchor;
        GUI.skin.label.normal.textColor = mColor;
        Rect rct = new Rect(cx + dx, cy, fw, myH);
        GUI.Label(rct, mLabel);
        cy += GetFontH();

        if (mbExp && (mLvAry.Count > 0))   // 若展开则显示子项
        {
            cx += myH;   // 往后缩
            bool bVal = mEnable && bEnable;
            foreach (LvItm itm in mLvAry)
            {
                itm.OnDrawLvItm(cx, ref cy, fw, bVal, func);
                cy += GetDrawH();
            }
        }
    }

    float NtfDrawBtn(float cx, float cy, float fh)
    {
        if (mLvAry.Count > 0)
        {
            float fch = fh * 0.7f;
            float dy = (fh - fch) * 0.5f;
            Rect rct = new Rect(cx, cy + dy, fch, fch);
            GUI.skin.button.fontSize         = mfont;
            GUI.skin.button.normal.textColor = mColor;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            if (GUI.Button(rct, mbExp ? "-" : "+"))
                mbExp = !mbExp;
            return fch;
        }
        return 0;
    }

    public override LvItm GetSubItem(int nIndx)
    {
        if(mLvAry.Count <= nIndx) return null;
        return mLvAry[nIndx];
    }
}

public delegate void OnClickLvItem(LvItm itm, UIListViewCtrl frm);

//-------------------------------------------------------------------------------------------------
// ListViewCtrl: 内部使用
public class UIListViewCtrl
{
    public Vector2         mRctWH;         // 裁剪区域大小
    public Vector2         mViewWH;        // 数据区域大小
    public OnClickLvItem   OnClick;        // Button 有效
    Vector2                mCurPos;        // 显示位置
    List<LvItm>            mItmAry;        // 列表

    public UIListViewCtrl()
    {
        mRctWH  = new Vector2(200, 350);   // 裁剪区域大小
        mViewWH = new Vector2(200, 350);   // 数据区域大小
        mItmAry = new List<LvItm>();
        mCurPos = Vector2.zero;
    }

    //-------------------------------------------------------------------------------------------------
    public void ClearLvItems() { mItmAry.Clear(); }
    public bool IsEmpty()
    {
        return (mItmAry.Count <= 0);
    }

    void OnClickLvBtnItm(LvItm itm)
    {
        if (null != OnClick)
            OnClick(itm, this);
    }
    
    //-------------------------------------------------------------------------------------------------
    public void OnDrawListView(float fx, float fy)
    {
        bool bEnable = GUI.enabled;
        int bfs = GUI.skin.button.fontSize;
        int lfs = GUI.skin.label.fontSize;
        TextAnchor lblAnch = GUI.skin.label.alignment;
        TextAnchor btnAnch = GUI.skin.button.alignment;
                
        GUI.Box(new Rect(fx, fy, mRctWH.x, mRctWH.y), "");
        {
            Rect rct0 = new Rect(fx, fy, mRctWH.x, mRctWH.y);
            Rect rct1 = new Rect(0, 0, mViewWH.x, mViewWH.y);
            mCurPos = GUI.BeginScrollView(rct0, mCurPos, rct1);
            {
                float cx = 0;
                float cy = 0;
                foreach (LvItm itm in mItmAry)
                {
                    itm.OnDrawLvItm(cx, ref cy, mViewWH.x, true, OnClickLvBtnItm);
                }
            }
            GUI.EndScrollView();
        }
        GUI.enabled = bEnable;
        GUI.skin.label.fontSize  = lfs;
        GUI.skin.button.fontSize = bfs;
        GUI.skin.label.alignment  = lblAnch;
        GUI.skin.button.alignment = btnAnch;
        GUI.skin.button.normal.textColor = Color.white;
        GUI.skin.label.normal.textColor = Color.white;
    }
    
    //-------------------------------------------------------------------------------------------------
    // 添加分割行
    public void AddLvEmpty(float fh)
    {
        LvItmEmpty itm = new LvItmEmpty();
        itm.mHeight = fh;
        AddLvItem(itm);
    }
    
    //-------------------------------------------------------------------------------------------------
    public LvItmLabel AddLvLabel(string sLabel, Color clr)
    {
        LvItmLabel itm = new LvItmLabel();
        itm.mWidth  = mViewWH.x;
        itm.mLabel  = sLabel;  // 显示的文字
        itm.mColor  = clr;
        AddLvItem(itm);
        return itm;
    }
    
    //-------------------------------------------------------------------------------------------------
    public LvItmBtn AddLvBtn(string sLabel, Color clr)
    {
        LvItmBtn itm = new LvItmBtn();
        itm.mWidth  = mViewWH.x;
        itm.mLabel  = sLabel;  // 显示的文字
        itm.mColor  = clr;
        AddLvItem(itm);
        return itm;
    }
    //-------------------------------------------------------------------------------------------------
    // 子列表
    public LvItmGrp AddLvItmGrp(string sLabel, Color clr)
    {
        LvItmGrp itm = new LvItmGrp();
        itm.mWidth  = mViewWH.x;
        itm.mLabel  = sLabel;  // 显示的文字
        itm.mColor  = clr;
        AddLvItem(itm);
        return itm;
    }
    
    //-------------------------------------------------------------------------------------------------
    public LvColGrp AddLvColGrp(LvAlign nType = LvAlign.LIA_LEFT)
    {
        LvColGrp itm = new LvColGrp();
        itm.mWidth = mViewWH.x;
        itm.mTye   = nType;
        AddLvItem(itm);
        return itm;
    }
    
    //-------------------------------------------------------------------------------------------------
    public void AddLvItem(LvItm itm)
    {
        mItmAry.Add(itm);
    }
    
    //-------------------------------------------------------------------------------------------------
    LvItm GetLvItem(int nRow)
    {
        if (mItmAry == null) return null;
        if(mItmAry.Count <= nRow) return null;
        return mItmAry[nRow];
    }
    
    //-------------------------------------------------------------------------------------------------
    public LvItmLabelEx AddLvLabelEx(ResPackge pck)
    {
        LvItmLabelEx itm = new LvItmLabelEx();
        itm.mWidth  = mViewWH.x;
        itm.mLabel  = pck.mFile;  // 显示的文字
        itm.mRefPck = pck;
        AddLvItem(itm);
        return itm;
    }

    public LvItmBtnEx AddLvBtnEx(string sLabel, ResPackge pck)
    {
        LvItmBtnEx itm = new LvItmBtnEx();
        itm.mWidth  = mViewWH.x;
        itm.mLabel  = sLabel;  // 显示的文字
        itm.mRefPck = pck;
        AddLvItem(itm);
        return itm;
    }
}
