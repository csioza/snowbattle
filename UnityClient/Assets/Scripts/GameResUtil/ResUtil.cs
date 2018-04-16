
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;

// Unity3D 宏定义 http://www.unitymanual.com/wiki//index.php?edition-view-165-1.html

//-------------------------------------------------------------------------------------------------------------------
// 资源列表文件里 一行纪录对应内容
public class ResPackItm
{
    public int      mType;       // 0(普通资源)/1(场景 .unity)
    public int      mfVer;       // 版本
    public string   mFile;
    public string   mPack;
    
    //--------------------------------------------------------------------------------------------
    public string MakeString()
    {
        string sz = mType.ToString() + "|";
        sz += mfVer.ToString() + "|";
        sz += mFile + "|";
        sz += mPack;
        return sz;
    }
    
    //--------------------------------------------------------------------------------------------
    static public ResPackItm MakeFrom(string sz)
    {
        string[] szAry = sz.Split('|');
        if (szAry.Length >= 4)
        {
            ResPackItm itm = new ResPackItm();
            itm.mType = ResUtil.TryToInt(szAry[0]);
            itm.mfVer = ResUtil.TryToInt(szAry[1]);
            itm.mFile = szAry[2];
            itm.mPack = szAry[3];
            return itm;
        }
        return null;
    }
}

//-------------------------------------------------------------------------------------------------------------------
//
// 资源文件读取
//
public class ResUtil
{
    //--------------------------------------------------------------------------------------------
	/// <summary>
	/// 读取从Server上下载的配置文件。
	/// </summary>
    static public Dictionary<string, ResPackItm> ReadResTable(string sf)
    {
        Dictionary<string, ResPackItm> tbl = null;
        try
        {
            FileStream stm = new FileStream(sf, FileMode.Open, FileAccess.Read);
            if (stm.Length > 0)
            {
                byte[] vAry = new byte[stm.Length];
                stm.Read(vAry, 0, vAry.Length);
                tbl = ResUtil.ReadResTable(vAry);
            }
            stm.Close();
        }
        catch (Exception exp)
        {
            Debug.LogWarning("CAN NOT Open file = " + sf + ", Msg = " + exp.Message);
        }
        return tbl;
    }

    //--------------------------------------------------------------------------------------------
    static public Dictionary<string, ResPackItm> ReadResTable(byte[] pBf)
    {
        Dictionary<string, ResPackItm> tbl = new Dictionary<string, ResPackItm>();

        if (pBf.Length > 0)
        {
            string line;
            MemoryStream mryStm = new MemoryStream(pBf);
            StreamReader stm = new StreamReader(mryStm);
            while ((line = stm.ReadLine()) != null)
            {
                if (line.Length <= 2) continue;
                line = line.Replace("\n", "");
                line = line.Replace("\r", "");
                ResPackItm itm = ResPackItm.MakeFrom(line);
                if(itm != null) tbl.Add(itm.mFile.ToLower(), itm);
            }
            stm.Close();
            mryStm.Close();
        }
        return tbl;
    }
    
    //--------------------------------------------------------------------------------------------
    static public bool WriteResTable(string file, Dictionary<string, ResPackItm> tbl)
    {
        if (tbl.Count > 0)
        {
            string szWrite = "";
            FileStream fstm = new FileStream(file, FileMode.Create);
            StreamWriter stm = new StreamWriter(fstm);
            foreach(ResPackItm itm in tbl.Values)
            {
                szWrite = itm.MakeString() + "\n";
			    stm.Write(szWrite.ToCharArray());
            }
            stm.Close();
            fstm.Close();
            return true;
        }
        return false;
    }

    //--------------------------------------------------------------------------------------------
    static public Dictionary<string, ResPackItm> ReplaceAndMergeResTable(Dictionary<string, ResPackItm> vNew, Dictionary<string, ResPackItm> vOld)
    {
        Dictionary<string, ResPackItm> tbl = new Dictionary<string, ResPackItm>(vOld);
        foreach (ResPackItm v0 in vNew.Values)
        {
            ResPackItm val = null;
            if (tbl.TryGetValue(v0.mFile.ToLower(), out val))
            {
                //if (v0.mfVer > val.mfVer)
                {
                    tbl.Remove(v0.mFile.ToLower());
                    tbl.Add(v0.mFile.ToLower(), v0);
                }

                continue;
            }
            tbl.Add(v0.mFile.ToLower(), v0);
        }
        return tbl;
    }
    
    //--------------------------------------------------------------------------------------------
    static public int TryToInt(string str, int nDef = 0)
    {
        try{return Convert.ToInt32(str);}catch{}
        return nDef;
    }

    //--------------------------------------------------------------------------------------------
    // 保留 Assets/ 之后的路径, 失败则直接返回
	static public string PathAfterResource(string file)
	{
		string nf = file.Replace('\\', '/');
		string sz = nf.ToLower();
		string sp = "resources/"; //Application.dataPath.ToLower();
		int npos = sz.IndexOf(sp);
		if(npos >= 0)
		{
			return nf.Substring((npos + sp.Length));
		}
		return nf;
	}
    
    //--------------------------------------------------------------------------------------------
    static public bool GetVersion(ref string sGmV, ref string sResV)
    {
        string szfNm = ResPath.GetVersionTxt();
        string szLcl = ResPath.GetLocal(szfNm);
        byte[] dt = ResPath.GetFileData(szLcl);
        return GetVersion(dt, ref sGmV, ref sResV);
    }

    static public bool GetVersion(byte[] pBf, ref string sGmV, ref string sResV)
    {
        if ((pBf != null) && (pBf.Length > 0))
        {
            MemoryStream mryStm = new MemoryStream(pBf);
            StreamReader stm = new StreamReader(mryStm);
            string line = stm.ReadLine();
            if (line != null)
            {
                line = line.Replace("\n", "");
                line = line.Replace("\r", "");
                string [] sAry = line.Split('|');
                Debug.Log("line = " + line);
                if (sAry.Length > 1)
                {
                    sGmV  = sAry[0];
                    sResV = sAry[1];
                    return true;
                }
            }
        }
        return false;
    }
    ////////////////////////////////////////////////////////////////////////////////
    //new
    //保存文件
    static public bool SaveFile(byte[] bytes, string path, string filename)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo dirInfo = Directory.CreateDirectory(path);
                if (!dirInfo.Exists)
                {
                    Debug.LogWarning("CreateDirectory fail, dir = " + path);
                    return false;
                }
            }
            File.WriteAllBytes(path + filename, bytes);
            return true;
        }
        catch (Exception exp)
        {
            Debug.LogWarning("CreateDirectory exception, dir = " + path + ", Msg = " + exp.Message);
        }
        return false;
    }
    //文件是否存在
    static public bool IsExistFile(string filePath)
    {
        return File.Exists(filePath);
    }
    //删除文件
    static public void DeleteFile(string filePath)
    {
        if (ResUtil.IsExistFile(filePath))
            File.Delete(filePath);
    }
}
