﻿
using System;
using System.Text;
using System.Runtime.InteropServices;

namespace NGE.Util{
/// <summary>
/// ini 文件处理类
/// </summary>
	public class Ini
    {

		[DllImport("kernel32", SetLastError=true)] private static extern int WritePrivateProfileString(string pSection, string pKey, string pValue, string pFile);
		[DllImport("kernel32", SetLastError=true)] private static extern int WritePrivateProfileStruct(string pSection, string pKey, string pValue, int pValueLen, string pFile);
		[DllImport("kernel32", SetLastError=true)] private static extern int GetPrivateProfileString(string pSection, string pKey, string pDefault, byte[] prReturn, int pBufferLen, string pFile);
		[DllImport("kernel32", SetLastError=true)] private static extern int GetPrivateProfileStruct(string pSection, string pKey, byte[] prReturn, int pBufferLen, string pFile);

		private string ls_IniFilename;
		private int li_BufferLen = 256;		


		public Ini(string pIniFilename){
			ls_IniFilename = pIniFilename;
		}

		/// <summary>
		/// INI filename (If no path is specifyed the function will look with-in the windows directory for the file)
		/// </summary>
		public string IniFile{
			get{return(ls_IniFilename);}
			set{ls_IniFilename=value;}
		}

		/// <summary>
		/// Max return length when reading data (Max: 32767)
		/// </summary>
		public int BufferLen{
			get{return li_BufferLen;}
			set{
				if(value>32767){li_BufferLen=32767;}
				else if(value<1){li_BufferLen=1;}
				else{li_BufferLen=value;}
			}
		}

		/// <summary>
		/// Read value from INI File
		/// </summary>
		public string ReadValue(string pSection, string pKey, string pDefault){
			
			return(z_GetString(pSection,pKey,pDefault));

		}

		/// <summary>
		/// Read value from INI File, default = ""
		/// </summary>
		public string ReadValue(string pSection, string pKey){
			
			return(z_GetString(pSection,pKey,""));
		
		}

		/// <summary>
		/// Write value to INI File
		/// </summary>
		public void WriteValue(string pSection, string pKey, string pValue){

			WritePrivateProfileString(pSection, pKey, pValue, this.ls_IniFilename);			

		}

		/// <summary>
		/// Remove value from INI File
		/// </summary>
		public void RemoveValue(string pSection, string pKey){

			WritePrivateProfileString(pSection, pKey, null, this.ls_IniFilename);	
		
		}

		/// <summary>
		/// Read values in a section from INI File
		/// </summary>
		public void ReadValues(string pSection, ref Array pValues){

			pValues = z_GetString(pSection,null,null).Split((char) 0);

		}

		/// <summary>
		/// Read sections from INI File
		/// </summary>
		public void ReadSections(ref Array pSections){

			pSections = z_GetString(null,null,null).Split((char) 0);

		}

		/// <summary>
		/// Remove section from INI File
		/// </summary>
		public void RemoveSection(string pSection){

			WritePrivateProfileString(pSection, null, null, this.ls_IniFilename);

		}

		/// <summary>
		/// Call GetPrivateProfileString / GetPrivateProfileStruct API
		/// </summary>
		private string z_GetString(string pSection, string pKey, string pDefault){

			string sRet=pDefault;byte[] bRet=new byte[li_BufferLen];
			int i=GetPrivateProfileString(pSection, pKey, pDefault, bRet, li_BufferLen, ls_IniFilename);
			sRet=System.Text.Encoding.GetEncoding(1252).GetString(bRet,0,i).TrimEnd((char)0);
			return(sRet);

		}

	}

}
