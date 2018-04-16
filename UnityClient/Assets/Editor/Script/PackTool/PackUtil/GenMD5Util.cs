
using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

public class GenMD5Util
{
	static public string NtfGenFileMD5(string file)
	{
		FileStream fstm = null;
		try
		{
			fstm = new FileStream(file, FileMode.Open, FileAccess.Read);
		}
		catch(Exception exp)
		{
			Debug.Log("NtfGenMD5 Load failed: file = " + file + ", Msg = " + exp.Message);
		//	return "";
		}

		if(fstm.Length <= 0) return "";
		
		int nLen = (int)fstm.Length;
		byte[] vAry = new byte[nLen];	//初始化字节数组
		fstm.Read(vAry, 0, vAry.Length);		//读取流中数据把它写到字节数组中
		fstm.Close();							//关闭流
		string buffMD5 = NtfGenBufMD5(vAry);
        string filenameMD5 = NtfGenStringMD5(file);
        //返回文件内容的md5和文件名的md5的md5
        return NtfGenStringMD5(buffMD5 + filenameMD5);
	}
	
	static public string NtfGenBufMD5(byte[] data)
	{
		MD5 md5 = new MD5CryptoServiceProvider();
		byte[] md5data = md5.ComputeHash(data);//计算data字节数组的哈希值
		md5.Clear();
		string str = "";
		for(int i = 0; i < md5data.Length; i++)
		{
			str += md5data[i].ToString("x").PadLeft(2, '0');
		}
		return str;
	}

    static public string NtfGenStringMD5(string data)
    {
        byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);
        return GenMD5Util.NtfGenBufMD5(buffer);
    }
}
