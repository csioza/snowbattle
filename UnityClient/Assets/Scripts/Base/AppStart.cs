
using UnityEngine;


public class AppStart : MonoBehaviour
{
    public static UIListViewCtrl mLvCtrl;
    public bool ForceReadFromPackage = false;
	// Use this for initialization
	void Start ()
	{
        if(null == mLvCtrl)
        {
            float fw = Screen.width;
            float fh = Screen.height;
            float f3 = fh / 3.0f * 2;
            mLvCtrl = new UIListViewCtrl();
            mLvCtrl.mRctWH  = new Vector2(fw, f3);
            mLvCtrl.mViewWH = new Vector2(fw, f3);
            mLvCtrl.OnClick = GameResMng.TestLoadScene;
        }
        GameResMng.ForcePackage = ForceReadFromPackage;
        GameResMng.CreateResMng(this);
	}

    static public void NtfAddBaseToLv()
    {
    }
	
	// Update is called once per frame
	void Update ()
	{
	}
	

    void OnGUI()
    {
        //GUI.Slider();
        GameResMng.OnResMngGUI();
        if (null != mLvCtrl)
        {
            mLvCtrl.mRctWH.x  = Screen.width;
            mLvCtrl.mViewWH.x = Screen.width;
            float py = Screen.height - mLvCtrl.mRctWH.y;
            mLvCtrl.OnDrawListView(0, py);
        }
        if (!GameResMng.IsLoaded())
        {
            float fx = (Screen.width - 300) * 0.5f;
            float fy = (Screen.height - 60);
            float fw = Screen.width * 0.8f;
            fx = (Screen.width - fw) * 0.5f;
            float fProg = GameResMng.GetProgress();
            GUI.Label(new Rect(fx, fy, 300, 80), @"进度: " + fProg + "%");
            //GUI.HorizontalScrollbar (new Rect (fx, fy, fw, 30), fProg, 1.0f, 0.0f, 100.0f);
        }
    }

    
}
