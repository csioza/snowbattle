using UnityEngine;
using System.Collections;
using UnityEditor;


//-------------------------------------------------------------------------------------------------------------------
//
// 控件: ViewList
//
//-----------------------------------------------------------------------------------------------
public class CViewList
{
    
    public int             mHitRow = -1;    // 点击选中的
    public int             mSelRow;         // 高亮显示的行
    public float           mCeilH;
    public float           mViewH;
    public float           mViewW;
    public GUIStyle        mStyle;
    Vector2                mCurPos;
    
    //-----------------------------------------------------------------------------
    public CViewList(float fw, float fh)
    {
        mCeilH  = 20;
        mSelRow = 0;
        mHitRow = -1;
        mViewH  = fh;
        mViewW  = fw;

        mStyle = new GUIStyle(EditorStyles.miniButton);
        mStyle.alignment = TextAnchor.MiddleLeft;

        mCurPos = Vector2.zero;
    }
    
    //-----------------------------------------------------------------------------
    public void NtfResetView()
    {
        mSelRow = 0;
        mHitRow = -1;
        mCurPos = Vector2.zero;
    }

    //-----------------------------------------------------------------------------
    // 显示列表内容
    public bool OnDrawView(float fx, float fy, string [] szAry)
    {
        bool bDirty = false;
        GUI.Box(new Rect(fx, fy, mViewW, mViewH), "");
        {
            //GUI.Label(new Rect(fx, fy, mViewW, mViewH + 10), "asdfasdf");
            float nH = szAry.Length * mCeilH;
            float fw = (nH >= mViewH) ? mViewW - 15 : mViewW - 5;
            mCurPos = GUI.BeginScrollView(new Rect(fx, fy, mViewW, mViewH - 5), mCurPos, new Rect(0, 0, fw, nH+5));
            {
                mSelRow = GUI.SelectionGrid(new Rect(0, 0, fw, nH), mSelRow, szAry, 1, mStyle);
                bDirty = (mHitRow != mSelRow);
                mHitRow = mSelRow;
            }
            GUI.EndScrollView();
        }
        return bDirty;
    }
}