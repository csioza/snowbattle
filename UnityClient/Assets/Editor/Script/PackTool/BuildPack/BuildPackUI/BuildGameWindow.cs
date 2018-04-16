
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BuildGameWindow : EditorWindow
{
    UITargetCtrl            mTgtCtrl;
    UIToggleCtrl            mGameLst;       // 游戏列表
    UIToggleCtrl            mPubLst;        // 公共文件
    public VerItemMng       mVerMng;        // 版本管理器

    GamePackUI mpSelGM;
    Dictionary<string, GamePackUI> mGameRes;    // 每款游戏对应一个资源目录
    public BuildTarget GetTarget() { return mTgtCtrl.mCurTgt; }
    //--------------------------------------------------------------------------------------------
    // 启动
    void OnEnable()
    {
        ArchiveUtil.NtfInitSkips();
        PackVerCfg.NtfLoadVerCfg("PackVers/VerConfig.txt");
        mVerMng = new VerItemMng();
        mVerMng.NtfSearchPackVers();	// 搜索已经存在的版本文件
        mTgtCtrl = new UITargetCtrl();
        mGameLst = new UIToggleCtrl(@"游戏列表", this.OnClickGameList);
        mPubLst  = new UIToggleCtrl(@"公共文件");        // 场景列表
        mGameRes = new Dictionary<string, GamePackUI>();
        mPubLst.EnableHori(true);
        mPubLst.LeftTitle(true);
        NtfRefreshGames();
        NtfRefreshPublic();

        mGameLst.AddButton(new BtnItem(101, 40, @"刷新", this.OnClickCtrlBtn));
        
        mPubLst.AddButton(new BtnItem(201, 40, @"刷新", this.OnClickCtrlBtn));
        mPubLst.AddButton(new BtnItem(202, 40, @"打包", this.OnClickCtrlBtn));
    }
    
    //--------------------------------------------------------------------------------------------
    // 界面布局
    void OnGUI()
    {
        //mTgtLyr.OnLayout(new Vector2(0, 5));
        Rect rct = mTgtCtrl.OnLayout(0, 5);
        rct = mGameLst.OnLayout(rct.xMax + 10, 5, 300, 200);        // 游戏列表
        Rect rct2 = mPubLst.OnLayout(rct.xMax + 10, 5, 200, 200);   // 公共资源
        if (mpSelGM != null)                                        // 选中游戏信息
        {
            mpSelGM.OnLayout(0, rct.yMax + 10, Screen.width, Screen.height - rct.yMax);
        }

        float tx = rct2.xMax + 10;
        OnBuildSelGame(new Rect(tx + 10, 5, 70, 20));   // 制作选中游戏, 包括: 启动程序/资源包/场景包
        OnBuildGameTest(new Rect(tx + 80, 5, 70, 20));  // 发布测试版游戏

    }
    
    //--------------------------------------------------------------------------------------------
    // 刷新游戏列表
    void NtfRefreshGames()
    {
        mpSelGM = null;
        mGameRes.Clear();
        mGameLst.ResetCtrl();
        List<string> pths = ArchiveUtil.NtfGetDirs(Application.dataPath, false, ArchiveUtil.mSkips);
        foreach (string sPth in pths)
        {
            string sl = sPth.ToLower();
            if (sl.Contains("package") || sl.Contains("resources") || sl.Contains("extends"))
            {
                mGameLst.AddItem(new TglItem(sPth));
                string sGm = ArchiveUtil.GetLastPath(sPth);
                string sPt = ArchiveUtil.NtfPathAfterAssets(sPth);
                GamePackUI gpu = new GamePackUI(sGm, sPt, this);
                mGameRes.Add(sGm.ToLower(), gpu);
            }
        }
        OnFirstGame();
    }
    
    //--------------------------------------------------------------------------------------------
    // 刷新公共资源列表
    void NtfRefreshPublic()
    {
        mPubLst.ResetCtrl();
        List<string> pths = ArchiveUtil.NtfGetDirs(Application.dataPath, false, ArchiveUtil.mSkips);
        foreach (string sPth in pths)
        {
            string sl = sPth.ToLower();
            if (!sl.Contains("resources"))
            {
                mPubLst.AddItem(new TglItem(true, sPth));
            }
        }
    }
    
    //--------------------------------------------------------------------------------------------
    // 打包公共资源
    void NtfBuildPublicPack()
    {
        Debug.LogError("we can't support NtfBuildPublicPack");
        return;
        //List<string> Pths = GetPublicPath(false);
        //if (Pths.Count > 0)
        //{
        //    string szPath = BuildGameCMD.GetBuildFolder(mTgtCtrl.GetSelTarget());
        //    string file = EditorUtility.SaveFilePanel("Save Pack File", szPath, "Public", "pack");
        //    if (file.Length > 0) GamePackUI.NtfPack(Pths, file, mTgtCtrl.GetSelTarget(),null);
        //    return;
        //}
        //EditorUtility.DisplayDialog(@"操作提示", @"请选择要打包的目录", "Ok");
    }
    
    //--------------------------------------------------------------------------------------------
    // 点击游戏列表
    void OnClickGameList(TglItem itm, UIToggleCtrl frm)
    {
        //if (itm.mbSel)
        {
            GamePackUI gm = null;
            string sGm = ArchiveUtil.GetLastPath(itm.mName);
            mGameRes.TryGetValue(sGm.ToLower(), out gm);
            if (gm != mpSelGM)    mpSelGM = gm;
        }
    }
    
    //--------------------------------------------------------------------------------------------
    void OnClickCtrlBtn(int nID, UIToggleCtrl frm)
    {
        if (101 == nID) NtfRefreshGames();
        else if (201 == nID) NtfRefreshPublic();
        else if (202 == nID) NtfBuildPublicPack();
    }

    //--------------------------------------------------------------------------------------------
    // 对选中的游戏打包: 启动程序/资源包/场景包 
    void OnBuildSelGame(Rect rct)
    {
        if (GUI.Button(rct, @"全部制作"))
        {
            string szMsg = "";
            List<TglItem> selGms = mGameLst.GetSelItems();
            if (selGms.Count <= 0)
            {
                szMsg = @"没有需要构建的游戏";
                EditorUtility.DisplayDialog(@"操作提示", szMsg, "Ok");
                return;
            }
            szMsg = @"确定要构建选中的游戏吗? 这个过程会很漫长,请耐心等待直到弹出结果窗口.";
            if (EditorUtility.DisplayDialog(@"游戏发布", szMsg, "Ok", "Cancel"))
            {
                int nCount = 0;
                foreach (TglItem itm in selGms)
                {
                    GamePackUI gm = null;
                    string sGm = ArchiveUtil.GetLastPath(itm.mName);
                    if (mGameRes.TryGetValue(sGm.ToLower(), out gm))
                    {
                        ++nCount;
                        gm.BuildGameAndPack(mTgtCtrl.GetSelTarget());
                    }
                }
                szMsg = @"消息: 成功打包 [";
                szMsg += (nCount.ToString() + @"] 个游戏, 有 [");
                szMsg += ((selGms.Count - nCount).ToString() + @"] 个游戏打包失败");
                EditorUtility.DisplayDialog(@"操作已结束", szMsg, "Ok");
            }
        }
    }
    //--------------------------------------------------------------------------------------------
    // 测试版游戏打包: 启动程序/资源包/场景包 
    void OnBuildGameTest(Rect rct)
    {
        if (GUI.Button(rct, @"临时版本"))
        {
            string szMsg = "";
            List<TglItem> selGms = mGameLst.GetSelItems();
            if (selGms.Count <= 0)
            {
                szMsg = @"请选择要构建的游戏";
                EditorUtility.DisplayDialog(@"操作提示", szMsg, "Ok");
                return;
            }
            szMsg = @"确定要构建选中的游戏吗? 这个过程会很漫长,请耐心等待直到弹出结果窗口.";
            if (EditorUtility.DisplayDialog(@"游戏发布", szMsg, "Ok", "Cancel"))
            {
                int nCount = 0;
                GamePackUI gm = null;
                foreach (TglItem itm in selGms)
                {
                    string sGm = ArchiveUtil.GetLastPath(itm.mName);
                    if (mGameRes.TryGetValue(sGm.ToLower(), out gm))
                    {
                        if(gm.BuildTempGame(mTgtCtrl.GetSelTarget()))
                            ++nCount;
                    }
                }
                szMsg = @"消息: 成功打包 [";
                szMsg += (nCount.ToString() + @"] 个游戏, 有 [");
                szMsg += ((selGms.Count - nCount).ToString() + @"] 个游戏打包失败");
                EditorUtility.DisplayDialog(@"操作已结束", szMsg, "Ok");
            }
        }
    }
    
    //--------------------------------------------------------------------------------------------
    public List<string> GetPublicPath(bool bAll)
    {
        List<string> vPths = new List<string>();
        List<TglItem> vItms = mPubLst.GetItems();
        foreach(TglItem itm in vItms)
        {
            if (bAll || itm.mbSel)
            {
                vPths.Add(itm.mName);
            }
        }
        return vPths;
    }
    
    //--------------------------------------------------------------------------------------------
    void OnFirstGame()
    {
        List<TglItem> itms = mGameLst.GetItems();
        if (itms.Count > 0)
        {
            itms[0].mbSel = true;
            GamePackUI gm = null;
            string sGm = ArchiveUtil.GetLastPath(itms[0].mName);
            mGameRes.TryGetValue(sGm.ToLower(), out gm);
            if (gm != mpSelGM)    mpSelGM = gm;
        }
    }
}
