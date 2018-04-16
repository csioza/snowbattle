using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class TglItem
{
    public bool     mbSel = false;
    public string mName = "Item";
    public bool mIsPackedEachFile = false;
    public bool m_isPackedExtern = false;
    //长久驻留在内存
    public bool m_isPackedAlive = false;
    public string Text
    {
        get
        {
            if (m_viewText != null)
            {
                return m_viewText;
            }
            return mName;
        }
    }
    public string m_viewText;

    public TglItem(string sName)
    {
        mName = sName;
    }

    public TglItem(bool bSel, string sName)
    {
        mName = sName;
        mbSel = bSel;
    }

    public TglItem(bool bSel, string sName, string sDt)
    {
        mName = sName;
        mbSel = bSel;
    }

    public TglItem(bool bSel, string sName, bool packedEachFile)
    {
        mName = sName;
        mbSel = bSel;
        mIsPackedEachFile = packedEachFile;
    }
}

public delegate void TglCtrlOnClickBtn(int nBtnID, UIToggleCtrl frm);
public class BtnItem
{
    public int                  mID;
    public float                mfW;
    public string               mName;
    public TglCtrlOnClickBtn    mCall;

    public BtnItem(int nID, float fw, string sNm, TglCtrlOnClickBtn func)
    {
        mID   = nID;
        mfW   = fw;
        mName = sNm;
        mCall = func;
    }
}

public delegate void TglCtrlDelegate(TglItem itm, UIToggleCtrl frm);

public class UIToggleCtrl
{
    bool            mbLeft;     // true: 标题靠左
    bool            mbHori;
    //bool            mbLock;     // true: 禁止状态改变
    bool            mbDirty;
    string          mTitle;
    Vector2         mCurPos;
    List<TglItem>   mItmAry;
    List<BtnItem>   mBtnAry;
    TglCtrlDelegate mCall;

    //--------------------------------------------------------------------------------------------
    public UIToggleCtrl(string sTitle, TglCtrlDelegate func = null)
    {
        mCall   = func;
        mbLeft  = false;
        //mbLock  = false;
        mbHori  = false;
        mbDirty = false;
        mTitle  = sTitle;
        mCurPos = Vector2.zero;
        mItmAry = new List<TglItem>();
        mBtnAry = new List<BtnItem>();

    }
    
    //--------------------------------------------------------------------------------------------
    public void AddButton(BtnItem itm){mBtnAry.Add(itm);}
    public void AddItem(TglItem itm){mItmAry.Add(itm);}
    public List<TglItem> GetItems() { return mItmAry;}
    public void EnableHori(bool bVal){mbHori = bVal;}
    public void LeftTitle(bool bLft){mbLeft = bLft;}
    public bool IsDirty() { return mbDirty;}
    public void ResetCtrl()
    {
        mItmAry.Clear();
        mbDirty = false;
        mCurPos = Vector2.zero;
    }
    
    //--------------------------------------------------------------------------------------------
    public List<TglItem> GetSelItems()
    {
        List<TglItem> vAry = new List<TglItem>();
        foreach (TglItem itm in mItmAry)
        {
            if (itm.mbSel) vAry.Add(itm);
        }
        return vAry;
    }

    public void InvertSelect()
    {
        foreach (TglItem itm in mItmAry)
        {
            itm.mbSel = !itm.mbSel;
        }
    }

    void PopMenuListCallBack()
    {
    }

    //--------------------------------------------------------------------------------------------
    public Rect OnLayout(float fx, float fy, float fw, float fh)
    {
        Rect rct = new Rect(fx, fy, fw, fh);
        GUI.Box(rct, mbLeft ? "" : mTitle);
        if(mbLeft) GUI.Label(new Rect(fx, fy, 100, 20), mTitle);
        
        float cx = fx + fw;
        foreach (BtnItem btn in mBtnAry)
        {
            cx -= btn.mfW;
            if (GUI.Button(new Rect(cx, fy, btn.mfW, 20), btn.mName))
            {
                if (btn.mCall != null) btn.mCall(btn.mID, this);
            }
            //cx -= 5;
        }


        float vh = mItmAry.Count * 20 + 10;
        GUI.Box(new Rect(fx, fy + 20, fw, fh - 20), "");
        mCurPos = GUI.BeginScrollView(new Rect(fx, fy + 30, fw, fh - 30), mCurPos, new Rect(0, 0, GetClipW(fw), vh));
        {
            float cy = 5;
            mbDirty = false;
            foreach (TglItem itm in mItmAry)
            {
                bool bT = GUI.Toggle(new Rect(0, cy, fw, 20), itm.mbSel, itm.Text);
                if (bT != itm.mbSel)
                {
                    mbDirty = true;
                    itm.mbSel = bT;
                    if (mCall != null) mCall(itm, this);
                }
                cy += 20;
            }
        }
        GUI.EndScrollView();

        return rct;
    }

    float GetClipW(float fw){return mbHori ? fw : (fw - 30);}

    
}
