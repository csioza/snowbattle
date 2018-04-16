
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

// 文件搜索/目录创建等相关操作
public class ArchiveUtil
{
	static bool					mbLock = false;
	static public string		mArMsg = "";
	static public List<string>	mSkips = null;
	
	static void LockNextCall(){mbLock = true;}
	static void UnlockNextCall(){mbLock = false;}
	static public bool IsLocked(){return mbLock;}
	
	static public void NtfInitSkips()
	{
		if(mSkips == null)
		{
			mSkips = new List<string>();
		}
        mSkips.Clear();
		mSkips.Add(".svn");
		mSkips.Add(".meta");
		mSkips.Add("Editor");
		mSkips.Add("ReadMe");
		mSkips.Add("Example");
		mSkips.Add("Tutorial");
		mSkips.Add("GameTools");
        mSkips.Add(".DS_Store");
	}

    static public void AddSkip(string szSkip)
    {
        mSkips.Add(szSkip);
    }
    
    //--------------------------------------------------------------------------------------------
    static public string GetLastPath(string sz)
    {
        sz = sz.Replace("\\", "/");
        if (sz.EndsWith("/"))
        {
            sz = sz.Substring(0, sz.Length - 1);
        }
        int npos = sz.LastIndexOf('/');
        if (npos >= 0)
            sz = sz.Substring(npos + 1);
        return sz;
    }
	
    //--------------------------------------------------------------------------------------------
	// 仅文件名, 无扩展名
	static public string GetFileName(string file)
	{
		return Path.GetFileNameWithoutExtension(file);
	}
    
    //--------------------------------------------------------------------------------------------
    static public string GetPthLast(string sPth)
    {
        string sp = sPth.Replace("\\", "/");
        if (sp.EndsWith("/"))
            sp = sp.Remove(sp.Length - 1, 1);
        int npos = sp.LastIndexOf('/');
        if (npos >= 0) return sp.Substring(npos + 1);
        return sp;
    }

    //--------------------------------------------------------------------------------------------
    // 保留 Assets/ 之后的路径, 失败则直接返回
	static public string NtfPathAfterAssets(string file)
	{
		string nf = file.Replace('\\', '/');
		string sz = nf.ToLower();
		string sp = "assets/"; //Application.dataPath.ToLower();
		int npos = sz.IndexOf(sp);
		if(npos >= 0)
		{
			return nf.Substring((npos + sp.Length));
		}
		return nf;
	}
	
    //--------------------------------------------------------------------------------------------
    // 保留 Assets/ 之后的路径, 失败则直接返回
	static public string NtfPathAfter(string sBase, string file)
	{
		string nf = file.Replace('\\', '/');
		string sz = nf.ToLower();
		string sp = sBase.ToLower(); //Application.dataPath.ToLower();
		int npos = sz.IndexOf(sp);
		if(npos >= 0)
			return nf.Substring((npos + sp.Length));
		return nf;
	}
	
    //--------------------------------------------------------------------------------------------
    // 保留从 Assets/ 开始的路径, 失败则直接返回
	static public string NtfPathBegin(string sBase, string file)
	{
		string nf = file.Replace('\\', '/');
		string sz = nf.ToLower();
        string sp = sBase.ToLower();
		int npos = sz.IndexOf(sp);
		if(npos >= 0)
            return nf.Substring(npos);
		return nf;
	}
	
    //--------------------------------------------------------------------------------------------
    // 保留从 Assets/ 开始的路径, 失败则直接返回
	static public string NtfPathBeginAssets(string file)
	{
		string nf = file.Replace('\\', '/');
		string sz = nf.ToLower();
        string sp = "assets/"; //Application.dataPath.ToLower();
		int npos = sz.IndexOf(sp);
		if(npos >= 0)
		{
            return nf.Substring(npos);
			//return nf.Substring((npos + sp.Length - 6));
		}
		return nf;
	}

    //--------------------------------------------------------------------------------------------
    // 保留从 PackVers/ 开始的路径, 失败则直接返回
	static public string NtfPathBeginPackVers(string file)
	{
		string nf = file.Replace('\\', '/');
		string sz = nf.ToLower();
		string sp = "packvers/";
		int npos = sz.IndexOf(sp);
		if(npos >= 0)
		{
			return nf.Substring((npos));
		}
		return nf;
	}

	static public List<string> NtfToLower(List<string> inList)
	{
		List<string> oList = new List<string>();
		for(int i = 0; i < inList.Count; ++i)
		{
			oList.Add(inList[i].ToLower());
		}
		return oList;
	}
	
	static public bool IsContains(string sz, List<string> inList)
	{
		for(int i = 0; i < inList.Count; ++i)
		{
			if(sz.Contains(inList[i]))
				return true;
		}
		return false;
	}
	
	//----------------------------------------------------------------------------------------------------------
	// 创建文件夹
	static public void NtfGenFolder(string szPath)
	{
		if(!Directory.Exists(szPath))
			Directory.CreateDirectory(szPath);
	}
	
	//----------------------------------------------------------------------------------
	// 清空目录里的内容
	static public void NtfClearFolder(string szPath)
	{
		LockNextCall();
		string[] szFiles = Directory.GetFiles(szPath);
		foreach(string szfile in szFiles)
		{
			File.Delete(szfile);
		}
		UnlockNextCall();
		mArMsg = "";
	}
	
	//----------------------------------------------------------------------------------
	// 清空目录里的内容
	static public void NtfClearFolder(string szPath, List<string> skipLst)
	{
		LockNextCall();
		if((skipLst == null) || (skipLst.Count <= 0))
		{
			NtfClearFolder(szPath);
			return;
		}
		skipLst = NtfToLower(skipLst);
		string[] szFiles = Directory.GetFiles(szPath);
		foreach(string szfile in szFiles)
		{
			if(!IsContains(szfile.ToLower(), skipLst))
				File.Delete(szfile);
		}
		UnlockNextCall();
		mArMsg = "";
	}
	
	//----------------------------------------------------------------------------------------------------------
	// 搜索文件
	static public string[] NtfGetFileAry(string szPath)
	{
		return Directory.GetFiles(szPath);
	}

    // 没有忽略约束
	static public List<string> NtfGetAllFiles(string szPath, bool bSub, string searchPattern = "*")
	{
		LockNextCall();
		List<string> sList = new List<string>();
        if (Directory.Exists(szPath))
        {
            List<string> dList = bSub ? NtfGetDirs(szPath, bSub) : new List<string>();

            dList.Insert(0, szPath);
            for (int i = 0; i < dList.Count; ++i)
            {
                //mArMsg = @"正在搜索目录:" + dList[i];
                string[] sfls = Directory.GetFiles(dList[i], searchPattern);
                foreach (string sf in sfls)
                {
                    sList.Add(NtfPathBeginAssets(sf));
                }
            }
        }
		mArMsg = "";
		UnlockNextCall();
		return sList;
	}
	
    // 可设置忽略约束列表(skipLst)
	static public List<string> NtfGetFiles(string szPath, bool bSub, List<string> skipLst, string searchPattern = "*")
	{
		LockNextCall();
		if((skipLst == null) || (skipLst.Count <= 0))
		{
			return NtfGetAllFiles(szPath, bSub, searchPattern);
		}
		
		
		List<string> sList = new List<string>();
        if (Directory.Exists(szPath))
        {
            List<string> dList = bSub ? NtfGetDirs(szPath, bSub, skipLst) : new List<string>();
            skipLst = NtfToLower(skipLst);

            dList.Insert(0, szPath);
            for (int i = 0; i < dList.Count; ++i)
            {
                //mArMsg = @"正在搜索目录:" + dList[i];

                string[] sfls = Directory.GetFiles(dList[i], searchPattern);
                foreach (string sf in sfls)
                {
                    if (!IsContains(sf.ToLower(), skipLst))
                        sList.Add(NtfPathBeginAssets(sf));
                }
            }
        }
		mArMsg = "";
		UnlockNextCall();
		return sList;
	}
	
    // 使用默认忽略约束列表(mSkips)
	static public List<string> NtfGetFiles(string szPath, bool bSub, string searchPattern = "*")
	{
		LockNextCall();
		if((mSkips == null) || (mSkips.Count <= 0))
		{
			return NtfGetAllFiles(szPath, bSub, searchPattern);
		}
		
		
		List<string> sList = new List<string>();
        if (Directory.Exists(szPath))
        {
            List<string> dList = bSub ? NtfGetDirs(szPath, bSub, mSkips) : new List<string>();
            List<string> skipLst = NtfToLower(mSkips);

            dList.Insert(0, szPath);
            for (int i = 0; i < dList.Count; ++i)
            {
                mArMsg = @"正在搜索目录:" + dList[i];

                string[] sfls = Directory.GetFiles(dList[i], searchPattern);
                foreach (string sf in sfls)
                {
                    if (!IsContains(sf.ToLower(), skipLst))
                        sList.Add(NtfPathBeginAssets(sf));
                }
            }
        }
		mArMsg = "";
		UnlockNextCall();
		return sList;
	}
	
	//----------------------------------------------------------------------------------------------------------
	// 搜索目录, !!! 不包含自己 !!!
	static public List<string> NtfGetDirs(string szPath, bool bSub, List<string> skipLst)
	{
		LockNextCall();
		//mArMsg = @"正在搜索目录:" + szPath;
		skipLst = NtfToLower(skipLst);
		List<string> sList = new List<string>();
        if (Directory.Exists(szPath))
		    GetDirs(ref sList, szPath, bSub, skipLst);
		mArMsg = "";
		UnlockNextCall();
		return sList;
	}
	
	static public List<string> NtfGetDirs(string szPath, bool bSub)
	{
		LockNextCall();
		List<string> sList = new List<string>();
        if (Directory.Exists(szPath))
		    GetDirs(ref sList, szPath, bSub);
		mArMsg = "";
		UnlockNextCall();
		return sList;
	}
	
	static void GetDirs(ref List<string> sList, string szPath, bool bSub)
	{
		mArMsg = @"正在搜索目录:" + szPath;
		string[] dirAry = Directory.GetDirectories(szPath);
		foreach(string sd in dirAry)
		{
			sList.Add(sd);
			if(bSub) GetDirs(ref sList, sd, bSub);
		}
	}
	
	static void GetDirs(ref List<string> sList, string szPath, bool bSub, List<string> skipLst)
	{
        if (szPath.Length <= 0) return;
		mArMsg = @"正在搜索目录:" + szPath;
		string[] dirAry = Directory.GetDirectories(szPath);
		foreach(string sd in dirAry)
		{
			if(!IsContains(sd.ToLower(), skipLst))
			{
				sList.Add(NtfPathBeginAssets(sd));
				if(bSub) GetDirs(ref sList, sd, bSub, skipLst);
			}
		}
	}


    //---------------------------------------------------------------------------------------------------------------
    //
    // 文件拷贝
    //
    // 将文件拷贝到指定目录. 保持目录结构
    // sPthAry: 原始目录
    // dPthAry: 目标目录
    static public void NtfCopyFiles(string sPth, string dPth)
    {
        sPth = sPth.Replace("\\", "/");
        dPth = dPth.Replace("\\", "/");
        if (!sPth.EndsWith("/")) sPth += "/";
        if (!dPth.EndsWith("/")) dPth += "/";
        List<string> sfls = NtfGetFiles(sPth, true, mSkips);    // 检索文件
        if (sfls.Count > 0)
        {
            int nNum = 0;
            float fs = 1.0f / (float)sfls.Count;
            EditorUtility.DisplayProgressBar(@"拷贝文件 ...", "", 0.0f);
            foreach (string sf in sfls)
            {
                ++nNum;
                string fn = NtfPathAfter(sPth, sf);
                string df = dPth + fn;      // 目标文件
                string pth = Path.GetDirectoryName(df);
                if (!Directory.Exists(pth))
                    Directory.CreateDirectory(pth);
                File.Copy(sf, df, true);
                EditorUtility.DisplayProgressBar(@"拷贝文件 ...", fn, nNum * fs);
            }
            EditorUtility.ClearProgressBar();
        }
    }
	
}
