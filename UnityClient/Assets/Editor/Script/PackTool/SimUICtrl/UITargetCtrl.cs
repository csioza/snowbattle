using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TgtItm
{
    public string           mName;
    public BuildTarget      mTarget;        // 鏋勫缓鐩?爣骞冲彴

    public TgtItm(BuildTarget tgt, string sNm)
    {
        mTarget = tgt;
        mName   = sNm;
    }
}

public class UITargetCtrl
{
    Vector2 mScrPos = Vector2.zero;
    public BuildTarget mCurTgt;
    List<TgtItm> mTgtAry;

    public UITargetCtrl()
    {
        mTgtAry = new List<TgtItm>();
        mTgtAry.Add(new TgtItm(BuildTarget.Android, "Android"));
        mTgtAry.Add(new TgtItm(BuildTarget.iOS,  "iPhone"));
        mTgtAry.Add(new TgtItm(BuildTarget.StandaloneWindows, "Win32"));
        //mTgtAry.Add(new TgtItm(, ));
        mCurTgt = BuildTarget.StandaloneWindows;
    }

    public BuildTarget GetSelTarget() { return mCurTgt; }

    public Rect OnLayout(float fx, float fy)
    {
        float fbh = 200;
        Rect rct = new Rect(fx, fy, 120, fbh);
        GUI.Box(rct, "");
        
        float fh = mTgtAry.Count * 20 + 10;
        GUI.Label(new Rect(fx, fy, 100, 20), "Build Target");
        GUI.Box(new Rect(fx, fy + 20, 120, fbh - 20), "");
        mScrPos = GUI.BeginScrollView(new Rect(fx, fy + 20, 130, fbh - 20), mScrPos, new Rect(0, 0, 110, fh));
        {
            float cy = 5;
            foreach (TgtItm itm in mTgtAry)
            {
                if (GUI.Toggle(new Rect(0, cy, 120, 20), itm.mTarget == mCurTgt, itm.mName))
                {
                    mCurTgt = itm.mTarget;
                }
                cy += 20;
            }
        }
        GUI.EndScrollView();

        return rct;
    }
}
