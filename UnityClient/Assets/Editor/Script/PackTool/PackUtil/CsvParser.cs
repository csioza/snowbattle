
using System;
using System.IO;
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

// ',' 分割
// # 开头为注释行
public class CsvParser
{
    bool                            mLock = false;  // 保持原内容不变
	string							mFile;
	List<string>					mHdrLst;		// 表头名
	Dictionary<string, int>			mHeader;		// 表头名--编号, 表头名字区分大小写
	Dictionary<int, List<string> >	mRecords;		// 数据. 行号 -- 内容

    public void NtfLock(bool bLck) { mLock = bLck; }
	public string GetCsvFile(){return mFile;}
	public List<string> GetHdr(){return mHdrLst;}
	public Dictionary<int, List<string> > GetRecords(){return mRecords;}
	
	void clearCsvParser()
	{
		mHdrLst  = null;
		mHeader  = null;
		mRecords = null;
	}
			
	public bool loadCsvBuffer(byte[] pBf, bool bTable)
	{
		clearCsvParser();
		if(pBf.Length <= 0) return false;
        Encoding asEncd = Encoding.GetEncoding("GB2312");
		MemoryStream mryStm = new MemoryStream(pBf);
		StreamReader stm = new StreamReader(mryStm, asEncd);

		string line;
		mRecords = new Dictionary<int, List<string>>();
		while ((line = stm.ReadLine()) != null) 
		{
			if(line.Length <= 1) continue;
			if(!mLock && (line[0] == '#')) continue;	// 作为注释

			List<string> pList = GetRowCells(line);
			if((pList == null) || (pList.Count <= 0)) continue;

            string sz0 = pList[0];
            if(!mLock && sz0.Length > 0 && sz0[0] == '#') continue;
			if(bTable)	// 若是表则把第一个有效行作为表头
			{
				mHdrLst = pList;
				bTable = false;
				mHeader = new Dictionary<string, int>();
				for(int i = 0; i < mHdrLst.Count; ++i)
				{
					string str = mHdrLst[i].ToLower();
					
					mHeader[str] = i;
				}
				continue;
			}
			mRecords[mRecords.Count] = pList;	// 实际数据
		}
		stm.Close();
		mryStm.Close();
		
		return true;
	}
	
	public bool loadCsvFile(string file, bool bTable)
	{
		mFile = "NONE";
		FileStream fstm = null;
		try
		{
			fstm = new FileStream(file, FileMode.Open, FileAccess.Read);
		}
		catch(Exception exp)
		{
			clearCsvParser();
			Debug.Log("loadCsv Load failed: file = " + file + ", Msg = " + exp.Message);
			return false;
		}

		long dwLen = fstm.Length;
		if(dwLen <= 0) return false;
		
		mFile = file;
		
		byte[] array = new byte[fstm.Length];	//初始化字节数组
		fstm.Read(array, 0, array.Length);		//读取流中数据把它写到字节数组中
		fstm.Close();							//关闭流
		return loadCsvBuffer(array, bTable);
	}
	
	public int getColumnCounts(){return mHdrLst.Count;}	// 列的数量. 不带表头的返回值为 0
	public int getRecordsCounts(){return mRecords.Count;}	// 实际数据对应行的数量
	
	//----------------------------------------------------------------------------------------------------------------------
	//
	// 获取数据
	//
	// String
	public bool GetString(out string str, int nRow, string colName, string szDef = "")
	{
		if(mRecords.ContainsKey(nRow))
		{
			string szNm = colName.ToLower();
			if(mHeader.ContainsKey(szNm))
			{
				List<string> szRow = mRecords[nRow];
				str = szRow[mHeader[szNm]];
				return true;
			}
		}
		str = szDef;
		return false;
	}
	
	// 整型数值(long)
	public bool GetLong(out long iValue, int nRow, string colName, long iDef = 0)
	{
		iValue = iDef;
		string str = "";
		if(GetString(out str, nRow, colName))
		{
			try
			{
				iValue = Convert.ToInt64(str);
				return true;
			}catch{}
		}
		return false;
	}
	
	// 整型数值(int)
	public bool GetInt32(out int iValue, int nRow, string colName, int iDef = 0)
	{
		iValue = iDef;
		string str = "";
		if(GetString(out str, nRow, colName))
		{
			try
			{
				iValue = Convert.ToInt32(str);
				return true;
			}catch{}
		}
		return false;
	}
	
	// 整型数值(Bool)
	public bool GetBool(int nRow, string colName, bool iDef = false)
	{
		string str = "";
		if(GetString(out str, nRow, colName))
		{
			try
			{
				return Convert.ToBoolean(str);
			}catch{}
		}
		return iDef;
	}
	
	//----------------------------------------------------------------------------------------------------------------------
	string ModifySingleRow(string szLine)
	{
        if (szLine.Length > 0)
        {
            char lst = szLine[szLine.Length - 1];
            if (lst == '\n' || lst == '\r')
            {
                szLine = szLine.Remove(szLine.Length - 1);
                lst = szLine[szLine.Length - 1];
                if (lst == '\n' || lst == '\r')
                    szLine = szLine.Remove(szLine.Length - 1);
            }
        }
		return szLine;
	}
	
	//----------------------------------------------------------------------------------------------------------------------
	// 拆解一条记录
	List<string> GetRowCells(string line)
	{
		line = ModifySingleRow(line);
		int iRowLen = line.Length;
		if(iRowLen <= 0) return null;
		
		List<string> pLst = new List<string>();

		string sCell = "";	// 拆解出来的记录
		bool bBeg = false;	// Cell 的起始标记 ""
		bool bEnd = false;	// Cell 的结束标记
		bool bCol = false;	// Cell 拆解解束		
		
	    for(int i = 0; i < iRowLen; ++i)
	    {
	        switch(line[i])
	        {
	            case '"':// 当前记录里含有 ','、'"'等CSV标记用的字符
				{
					if(!bBeg)	// 首次测到则为整体标记
					{
						bBeg = true;
						break;
					}
				
					// 如果不是首次
					if((i + 1) < iRowLen)
					{
						if(line[i + 1] == '"')	// 意味着是可见的 " , 否则就是结束标记
						{
							++i;
							bEnd = false;
							sCell += "\"";
						}
						else bEnd = true;	// 结束标记
					}
	                break;
				}
	            case ',':	// CSV分割符号
				{
					if(bBeg)
					{
						if(!bEnd)
						{
							sCell += (line[i]);
							continue;
						}
					}
					bCol = true;
	                break;
				}
	            default:
				{
					sCell += (line[i]);
					break;
				}
	        }// switch over
						
			if(bCol)
			{
				bCol = false;
				bBeg = false;
				bEnd = false;
				pLst.Add(sCell);
				sCell = "";
			}
		}// for over
		
		pLst.Add(sCell);
		
	    return pLst;
	}
	
	//----------------------------------------------------------------------------------------------------------------------
	// 存储为CSV格式. 可以没有表头数据(tblHeader)
	static public bool writeCsvFile(string file, List<string> tblHdr, Dictionary<int, List<string> > tblRcd)
	{
		if((tblHdr == null) && (tblRcd == null))
			return false;
		
		FileStream fstm = null;
		try
		{
			fstm = new FileStream(file, FileMode.Create);
			
		}
		catch(Exception exp)
		{
			Debug.Log("Write failed: file = " + file + ", Msg = " + exp.Message);
			return false;
		}
        Encoding asEncd = Encoding.GetEncoding("GB2312");
		StreamWriter stm = new StreamWriter(fstm, asEncd);
       
		NtfWriteCsvRow(stm, tblHdr);
		
		if(tblRcd != null)
		{
	        foreach (KeyValuePair<int, List<string> > row in tblRcd)
			{
				NtfWriteCsvRow(stm, row.Value);
			}
		}
		stm.Flush();
		stm.Close();
		fstm.Close();
		return true;
	}
	//----------------------------------------------------------------------------------------------------------------------
	static void NtfWriteCsvRow(StreamWriter stm, List<string> inLst)
	{
		List<string> rwLst = NtfRepairRow(inLst);
		if((rwLst == null) || (rwLst.Count <= 0)) return;
		string szWrite = "";
		for(int i = 0; i < rwLst.Count; ++i)
		{
			szWrite = rwLst[i];
			stm.Write(szWrite.ToCharArray());
			if(i < (rwLst.Count - 1))stm.Write(",");
		}
		stm.Write("\n");
	}
	
	//----------------------------------------------------------------------------------------------------------------------
	static List<string> NtfRepairRow(List<string> inLst)
	{
		if(inLst == null) return null;
		List<string> ouLst = new List<string>();
		for(int i = 0; i < inLst.Count; ++i)
		{
			string sItem = inLst[i];
			string sOutput = sItem;
			if(sOutput.Length > 0)
			{
				bool bIsSpecial = HasSpecial(sItem);
				
				int nfnd = sOutput.IndexOf('"', 0);
				while(nfnd >= 0)
				{
					bIsSpecial = true;
					sOutput = sOutput.Insert(nfnd, "\"");
					nfnd = sOutput.IndexOf('"', nfnd + 2);
				}
				if(bIsSpecial)
				{
					sOutput = "\"" + sOutput + "\"";
				}
			}
			ouLst.Add(sOutput);
		}
		
		return ouLst;
	}
	
	//----------------------------------------------------------------------------------------------------------------------
	static bool HasSpecial(string sItem)
	{
		if((sItem.IndexOf('\n') >= 0) || (sItem.IndexOf('\r') >= 0))
			return true;
		
		if(sItem.IndexOf(',') >= 0)	// 包含分割符
			return true;
				
		if(sItem.IndexOf(' ') >= 0)
			return true;
		
		return false;
	}
}
