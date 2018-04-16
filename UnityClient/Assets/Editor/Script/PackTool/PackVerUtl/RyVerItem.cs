
using System.Collections;
using System.Collections.Generic;

public class RyVerItem
{
	public	string	mItmFile = "";	// 相对于 Application.dataPath
	public	string	mItmMd5  = "";
	public	string	mCurVer  = "";	// 当前版本: 0.0.0
	public	string	mPreVer  = "";	// 前置版本
	public	string	mState   = "";	// NEW/MDY/DEL
	public	string	mGenTM   = "";	// 生成时的时间
	
	public RyVerItem Clone()
	{
		RyVerItem cln = new RyVerItem();
		cln.mItmFile = mItmFile;
		cln.mItmMd5  = mItmMd5 ;
		cln.mCurVer  = mCurVer ;
		cln.mPreVer  = mPreVer ;
		cln.mState   = mState  ;
		cln.mGenTM   = mGenTM  ;
		return cln;
	}

    // 注意: Md5 一样而文件名不一样则认为不同
    static public bool IsRyEqual(RyVerItem sI, RyVerItem dI)
    {
        if (sI.mItmMd5 == dI.mItmMd5)
        {
            string sf0 = sI.mItmFile.ToLower();
            string sf1 = dI.mItmFile.ToLower();
            return (sf0 == sf1);
        }
        return false;
    }

    static public List<string> GenStringAry(RyVerItem itm)
    {
        List<string> sLst = new List<string>();
        sLst.Add(itm.mItmFile);
        sLst.Add(itm.mItmMd5);
        sLst.Add(itm.mCurVer);
        sLst.Add(itm.mPreVer);
        sLst.Add(itm.mGenTM);
        return sLst;
    }
}

//------------------------------------------------------------------------------
// 比较结果
public class RyCompareUtil
{
    public List<RyVerItem>  mSames  = new List<RyVerItem>();   // 相同的
    public List<RyVerItem>  mDifSrc = new List<RyVerItem>();    // 源 与 目标 VerItemGrp 里的不一样
    public List<RyVerItem>  mDifDis = new List<RyVerItem>();    // 目标 与 源 VerItemGrp 里的不一样
    
    public List<RyVerItem>  mLstSrc = new List<RyVerItem>();    // 源 在 目标 VerItemGrp 里找不到
    public List<RyVerItem>  mLstDis = new List<RyVerItem>();    // 目标 在 源 VerItemGrp 里找不到

    // 源数据   = mSameLst + mDifDis + mLstSrc
    // 目标数据 = mSameLst + mDifSrc + mLstDis


    //------------------------------------------------------------------------------
    // 比较这两组数据
    public void NtfCompare(VerItemGrp sGrp, VerItemGrp dGrp)
    {
        mSames.Clear();
        mDifSrc.Clear();
        mDifDis.Clear();
        mLstSrc.Clear();
        mLstDis.Clear();

        Dictionary<string, RyVerItem> sDic = sGrp.GetVerItems();
        Dictionary<string, RyVerItem> dDic = dGrp.GetVerItems();
        foreach (KeyValuePair<string, RyVerItem> row in sDic)
        {
            RyVerItem dItm = null;
            RyVerItem sItm = row.Value;
            string szKey = sItm.mItmFile.ToLower();
            if (dDic.TryGetValue(szKey, out dItm))
            {
                // dGrp 里存在同文件名的文件
                if (RyVerItem.IsRyEqual(sItm, dItm))
                {
                    mSames.Add(sItm); // MD5一致
                }
                else
                {
                    // MD5不一致
                    mDifSrc.Add(sItm);
                    mDifDis.Add(dItm);
                }

                dDic.Remove(szKey); // 剩下的就是 sGrp 里不存在的
                continue;
            }
            mLstSrc.Add(sItm);
        }
        foreach (KeyValuePair<string, RyVerItem> row in dDic)
        {
            mLstDis.Add(row.Value);
        }

    }
	
	public void NtfWriteToFile(string szfile)
	{
        /*
        VerItemGrp.NtfWriteToFile(szfile + "_same.csv",   mSames);

        VerItemGrp.NtfWriteToFile(szfile + "_difSrc.csv", mDifSrc);
        VerItemGrp.NtfWriteToFile(szfile + "_difDis.csv", mDifDis);

        VerItemGrp.NtfWriteToFile(szfile + "_lstSrc.csv", mLstSrc);
        VerItemGrp.NtfWriteToFile(szfile + "_lstDis.csv", mLstDis);
        */

        List<string> hdr = VerItemGrp.NtfGetCsvHdr(); // 表头
        Dictionary<int, List<string>> rcd = new Dictionary<int, List<string>>();

        rcd.Add(rcd.Count, NtfAddCsvMsg(""));
        rcd.Add(rcd.Count, NtfAddCsvMsg("# 单一存在(File1)"));
        GenCsvFile(ref rcd, mLstSrc);
        rcd.Add(rcd.Count, NtfAddCsvMsg("# 单一存在(File2)"));
        GenCsvFile(ref rcd, mLstDis);
        
        rcd.Add(rcd.Count, NtfAddCsvMsg(""));
        rcd.Add(rcd.Count, NtfAddCsvMsg("# MD5不一样"));
        GenCsvFile(ref rcd, mDifSrc);
        GenCsvFile(ref rcd, mDifDis);
        
        rcd.Add(rcd.Count, NtfAddCsvMsg(""));
        rcd.Add(rcd.Count, NtfAddCsvMsg("# 完全一样"));
        GenCsvFile(ref rcd, mSames);
        CsvParser.writeCsvFile(szfile, hdr, rcd);
	}

   void GenCsvFile(ref Dictionary<int, List<string>> rcd, List<RyVerItem> pLst)
    {
        for (int i = 0; i < pLst.Count; ++i)
        {
            rcd.Add(rcd.Count, RyVerItem.GenStringAry(pLst[i]));
        }
    }

   static List<string> NtfAddCsvMsg(string msg)
   {
        List<string> sLst = new List<string>();
        sLst.Add(msg);
        return sLst;
   }
}
