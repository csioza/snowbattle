
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//----------------------------------------------------------------------------------------------------------
// Csv文件里保存的文件版本信息
public class VerItemGrp
{
	public string					mCsvFile = "";
	Dictionary<string, RyVerItem >	mRecords = new Dictionary<string, RyVerItem>();	// <file, ver>
	
	public Dictionary<string, RyVerItem > GetVerItems(){return mRecords;}
	
	public void NtfAddVerItem(string file, RyVerItem itm)
	{
		try{
			mRecords.Add(file.ToLower(), itm);
		}
		catch{
			Debug.Log("file exist: file = " + file + ", MD5 = " + itm.mItmMd5);
		}
	}
	
	public RyVerItem GetVerItem(string file)
	{
		string szLowr = file.ToLower();
		if(mRecords.ContainsKey(szLowr))
			return mRecords[szLowr];
		return null;
	}
	
	public void NtfFatchItemVer(ref Dictionary<string, RyVerItem > dic)
	{
		foreach(KeyValuePair<string, RyVerItem > row in mRecords)
		{
			RyVerItem itm0 = row.Value;
			string szk = itm0.mItmFile.ToLower();
			if(dic.ContainsKey(szk))
			{
				RyVerItem itm1 = dic[szk];
				if(itm1.mItmMd5 != itm0.mItmMd5)
				{
					if(SplitVerNum(itm0.mCurVer) > SplitVerNum(itm1.mCurVer))
					{
						itm1.mItmMd5 = itm0.mItmMd5;
						itm1.mCurVer = itm0.mCurVer;
						itm1.mPreVer = itm0.mPreVer;
						itm1.mGenTM = itm0.mGenTM;
					}
				}
				continue;
			}
			dic[szk] = itm0.Clone();
		}
	}
	
	static public VerItemGrp CreateVerItemGrp(string file)
	{
		CsvParser csvPrs = new CsvParser();
		if(!csvPrs.loadCsvFile(file, true)) return null;
		
		VerItemGrp grp = new VerItemGrp();
		grp.mCsvFile = file;
		
		string sItm = "";
		int nCount = csvPrs.getRecordsCounts();
		for(int i = 0; i < nCount; ++i)
		{
			csvPrs.GetString(out sItm, i, "File");
			if(sItm.Length > 0)
			{
				RyVerItem itm = new RyVerItem();
				csvPrs.GetString(out itm.mItmMd5,  i, "MD5");
				csvPrs.GetString(out itm.mCurVer,  i, "CurVer");
				csvPrs.GetString(out itm.mPreVer,  i, "PreVer");
				csvPrs.GetString(out itm.mGenTM,   i, "GenTM");
				itm.mItmFile = sItm;
				grp.NtfAddVerItem(itm.mItmFile, itm);
				continue;
			}
			Debug.LogWarning("LOST [File] Index = " + i);
		}
		return grp;
	}
	
	static public bool NtfWriteToFile(string file, Dictionary<string, RyVerItem > dic)
	{
		 List<string> hdr = NtfGetCsvHdr(); // 表头
		
		Dictionary<int, List<string> > rcd = new Dictionary<int, List<string> >();
		foreach(KeyValuePair<string, RyVerItem > row in dic)
		{
			List<string> sLst = new List<string>();
			RyVerItem itm = row.Value;
			sLst.Add(itm.mItmFile);
			sLst.Add(itm.mItmMd5);
			sLst.Add(itm.mCurVer);
			sLst.Add(itm.mPreVer);
			sLst.Add(itm.mGenTM);
			rcd.Add(rcd.Count, sLst);
		}
		return CsvParser.writeCsvFile(file, hdr, rcd);
	}

    static public bool NtfWriteToFile(string file, List<RyVerItem> dic)
    {
        List<string> hdr = NtfGetCsvHdr(); // 表头
        Dictionary<int, List<string>> rcd = new Dictionary<int, List<string>>();
        foreach (RyVerItem row in dic)
        {
            List<string> sLst = RyVerItem.GenStringAry(row);
            rcd.Add(rcd.Count, sLst);
        }
        return CsvParser.writeCsvFile(file, hdr, rcd);
    }

    // 表头
    static public List<string> NtfGetCsvHdr()
    {
        List<string> hdr = new List<string>();
        hdr.Add("File");
        hdr.Add("MD5");
        hdr.Add("CurVer");
        hdr.Add("PreVer");
        hdr.Add("GenTM");
        return hdr;
    }

	static public int SplitVerNum(string szVer)
	{
		int npos = szVer.LastIndexOf('.');
		if(npos > 0)
		{
			try
			{
				return Convert.ToInt32(szVer.Substring(npos + 1));
			}
			catch
			{
			}
		}
		return -1;
	}
}
