
using System;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine;

public class PackVerCfg
{
    static string mFile;
    static	public int mAppVer = 0; // exe 版本
    static	public int mGmsVer = 0; // 游戏版本
    static	public int mResVer = 0; // 当前游戏资源版本
    
    static string   mPreVer = "";
    static string   mCurVer = "";	// 当前版本
    static string   mNxtVer = "";   // 随后游戏资源版本

    
	static	public string GetPreVer(){return mPreVer;}
	static	public string GetCurVer(){return mCurVer;}
    static	public string GenNextVer(){return mNxtVer;}

    static public void NtfUpdateVer()
    {
        mPreVer = GetCurVer();
        ++mResVer;
        NtfWriteVerCfg(mFile);
    }

    static public bool NtfLoadVerCfg(string file)
    {
        mFile = file;
		FileStream fstm = null;
		try
		{
			fstm = new FileStream(file, FileMode.Open, FileAccess.Read);
		    long dwLen = fstm.Length;
            if (dwLen > 0)
            {
                byte[] pBf = new byte[fstm.Length];	//初始化字节数组
                fstm.Read(pBf, 0, pBf.Length);		//读取流中数据把它写到字节数组中
                fstm.Close();							//关闭流

                Encoding asEncd = Encoding.GetEncoding("GB2312");
                MemoryStream mryStm = new MemoryStream(pBf);
                StreamReader stm = new StreamReader(mryStm, asEncd);

                string line;
                while ((line = stm.ReadLine()) != null)
                {
                    if (line.Length <= 1) continue;
                    if (line[0] == '#') continue;
                    DoParserLine(line);
                }
            }
		}
		catch(Exception)
		{
			
		}
        string szVer = mAppVer.ToString() + "." + mGmsVer.ToString();
        if (mResVer > 0)
        {
            mPreVer = szVer + "." + (mResVer - 1).ToString();
        }
        mCurVer = szVer + "." + mResVer.ToString();
        mNxtVer = szVer + "." + (mResVer + 1).ToString();
        return true;
    }

    static void DoParserLine(string line)
    {
        string szLn = "";
        for(int i = 0; i < line.Length; ++i)
        {
            if(line[i] == '#') break;
            if(line[i] == '\t' || line[i] == ' ')
                continue;
            szLn += line[i];
        }

        string[] szAry = szLn.Split('=');
        if (szAry.Length != 2) return;
        if (szAry[0] == "AppVer")
            NtfTryConvert(ref mAppVer, szAry[1]);
        else if (szAry[0] == "GameVer")
            NtfTryConvert(ref mGmsVer, szAry[1]);
        else if (szAry[0] == "CurResVer")
            NtfTryConvert(ref mResVer, szAry[1]);
        
    }

    static int NtfTryConvert(ref int iVal, string szNum)
    {
		try
		{
			iVal = Convert.ToInt32(szNum);
		}catch{}
        return iVal;
    }

    static public bool NtfWriteVerCfg(string file)
    {
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
        string szWrite = "";
        Encoding asEncd = Encoding.GetEncoding("GB2312");
		StreamWriter stm = new StreamWriter(fstm, asEncd);

        szWrite = "AppVer = " + mAppVer.ToString() + "	# exe 版本\r\n";
        stm.Write(szWrite.ToCharArray());

        szWrite = "GameVer = " + mGmsVer.ToString() + "	# 游戏版本\r\n";
        stm.Write(szWrite.ToCharArray());
        
        szWrite = "CurResVer = " + mResVer.ToString() + "	# 当前游戏资源版本\r\n";
        stm.Write(szWrite.ToCharArray());

        szWrite = "NxtResVer = " + (mResVer + 1).ToString() + "	# 随后游戏资源版本";
        stm.Write(szWrite.ToCharArray());
		stm.Flush();
		stm.Close();
		fstm.Close();
        return true;
    }
}
