
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//----------------------------------------------------------------------------------------------------------
public class VerItemMng
{
	List<string>		mVerList;				// 版本文件列表
    RyCompareUtil       mpCmpUtl;               // 比较用的
	Dictionary<string, VerItemGrp >	mItemGrp = new Dictionary<string, VerItemGrp>();	// <csvFile, VerItemGrp >
	Dictionary<string, RyVerItem >	mItemVer = new Dictionary<string, RyVerItem>();		// <file, RyVerItem>

    public VerItemMng()
    {
        mpCmpUtl = new RyCompareUtil();
    }

	//------------------------------------------------------------------------------
	// 已存在版本文件
	public List<string> GetVerList(){return mVerList;}
    public string GetVerFile(int nIndx)
    {
        if((nIndx >= 0) && (nIndx < mVerList.Count))
            return mVerList[nIndx];
        return "";
    }

	public void NtfSearchPackVers()
	{
		string verPth = "PackVers/";
		mVerList = ArchiveUtil.NtfGetFiles(verPth, false, "*.csv");
        PackVerCfg.NtfLoadVerCfg("PackVers/VerConfig.txt");
        NtfRefreshItemVer();
	}
	
   public RyCompareUtil GetCompareUtl() { return mpCmpUtl; }
	
	//------------------------------------------------------------------------------
	bool HasItemGrp(string csvFile)
	{
		string szfile = csvFile.ToLower();
		if(mItemGrp.ContainsKey(szfile))
			return true;
		return false;
	}
	
	//------------------------------------------------------------------------------
	public bool NtfLoadVerFile(string csvFile)
	{
		VerItemGrp grp = VerItemGrp.CreateVerItemGrp(csvFile);
		if(grp != null)
		{
			mItemGrp.Add(csvFile.ToLower(), grp);
			return true;
		}
		return false;
	}
	
	//------------------------------------------------------------------------------
	public VerItemGrp GetItemGrp(string csvFile)
	{
		string szfile = csvFile.ToLower();
		if(mItemGrp.ContainsKey(szfile))
			return mItemGrp[szfile];
		return null;
	}
	
	//------------------------------------------------------------------------------
	public void GenItemGrp(string csvFile, List<string> sList)
	{
		//string sVer = GenNextVer();
		string sVer = ArchiveUtil.GetFileName(csvFile);
		string format = "yyyy-MM-dd HH:mm:ss";
		VerItemGrp grp = new VerItemGrp();
		grp.mCsvFile = csvFile;
		for(int i = 0; i < sList.Count; ++i)
		{
			DateTime dt = DateTime.Now;
			string sf = sList[i];
			RyVerItem itm = new RyVerItem();
			itm.mItmFile = ArchiveUtil.NtfPathBeginAssets(sf);
			itm.mItmMd5  = GenMD5Util.NtfGenFileMD5(sf).ToUpper();
			itm.mGenTM   = dt.ToString(format);
			OnTestVer(ref itm, sVer);
			grp.NtfAddVerItem(itm.mItmFile, itm);
		}
		mItemGrp.Add(csvFile.ToLower(), grp);
		VerItemGrp.NtfWriteToFile(csvFile, grp.GetVerItems());
	}
	
	void OnTestVer(ref RyVerItem itm, string sVer)
	{
		string szNm = itm.mItmFile.ToLower();
		if(mItemVer.ContainsKey(szNm))
		{
			RyVerItem fit = mItemVer[szNm];
			if(fit.mItmMd5 == itm.mItmMd5)
			{
				itm.mCurVer  = fit.mCurVer;
				itm.mPreVer  = fit.mPreVer;
				return;
			}
			itm.mPreVer  = fit.mCurVer;
		}
		itm.mCurVer = sVer;
	}
	
	//------------------------------------------------------------------------------
	void NtfRefreshItemVer()
	{
		mItemVer.Clear();	//
		for(int i = mVerList.Count - 1; i >= 0; --i)
		{
			VerItemGrp grp = VerItemGrp.CreateVerItemGrp(mVerList[i]);
			grp.NtfFatchItemVer(ref mItemVer);
		}
		
		//string file = "PackVers/asdfasdf.csv";
		//VerItemGrp.NtfWriteToFile(file, mItemVer);
	}
	
	//------------------------------------------------------------------------------
	// 版本文件比较
	public void NtfCompareItemGrpFile(string sfile, string dfile)
	{
		VerItemGrp sGrp = VerItemGrp.CreateVerItemGrp(sfile);
		VerItemGrp dGrp = VerItemGrp.CreateVerItemGrp(dfile);
        mpCmpUtl.NtfCompare(sGrp, dGrp);

        /*
        // 比较结果存文件
        string sNm = ArchiveUtil.GetFileName(sfile);
        string dNm = ArchiveUtil.GetFileName(dfile);
        string file = "PackVers/" + sNm + "-" + dNm + ".csv";
        mpCmpUtl.NtfWriteToFile(file);
        */
	}
}
