using UnityEngine;
using System.IO;
using System;

// http://www.comedreams.com/index.php/archives/761
// http://docs.unity3d.com/Documentation/Manual/PlatformDependentCompilation.html

//--------------------------------------------------------------------------------------------
// 各平台运行时资源路径维护
public class ResPath
{
    static public string ServerResourceUrl = "";
    static public string GetResListTxt() { return "GameResList.txt"; }
    static public string GetVersionTxt() { return "GameVersion.txt"; }
    static public string GetPackageVersionTxt() { return "PackageVersion.txt"; }
    static public string GetRemotePackageVersionTxt() { return "RemotePackageVersion.txt"; }

    static public string GetUrl(string sf)
    {
        string sUrl = GetUrl();
        if (string.IsNullOrEmpty(sUrl))
            return "";
        return (sUrl + sf);
    }

    //------------------------------------------------------------------
    // 资源下载地址
	static string GetUrl()
	{
        string szSvr = ServerResourceUrl;
            //"http://192.168.3.87:8080/GameResource/";// 
		//szSvr = "file://F:/PrjDancer/Publish/";
		//szSvr = "http://192.168.3.122/tgame/Publish/";

		if (!string.IsNullOrEmpty(szSvr))
		{
			string szPth = "";
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                {
                    szPth = "Android/";
                    break;
                }
                case RuntimePlatform.IPhonePlayer:
                {
                    szPth = "iPhone/";
                    break;
                }
                case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
                {
                    szPth = "Win32/";
                    break;
                }
                case RuntimePlatform.OSXPlayer:
                {
                    szPth = "MACOS/";
                    break;
                }
                default:
                {
                    Debug.LogWarning("GetUrl(): Unknown, Platform = " + Application.platform.ToString());
                    break;
                }
            }
            GameResMng.DebugMsg = "Url = " + szSvr + szPth;
            return (szSvr + szPth);
        }
        Debug.Log("Download Closed. Url Unknown");
        return "";
    }
    
    //------------------------------------------------------------------
    static public string GetLocal(string sf)
    {
        string sPth = GetLocal();
        return (sPth + sf);
    }

    static public string WLocalUrl(string sf)
    {
        string sPth = "";
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            sPth = "file://" + Application.persistentDataPath;
        }
        else sPth = "file://" + Application.streamingAssetsPath;
        
        return (sPth + "/" + sf);
    }

    //------------------------------------------------------------------
    // 本地资源路径, 不存在则创建
    static string GetLocal()
    {
        //Debug.Log("=====> Application.dataPath = " + Application.dataPath);
        //Debug.Log("=====> persistentDataPath   = " + Application.persistentDataPath);
        //Debug.Log("=====> streamingAssetsPath  = " + Application.streamingAssetsPath);
        //Debug.Log("=====> temporaryCachePath   = " + Application.temporaryCachePath);
        //Debug.Log("=====> webSecurityHostUrl   = " + Application.webSecurityHostUrl);

        string sPth = "";
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            sPth = Application.persistentDataPath + "/";
        }
        else sPth = Application.streamingAssetsPath + "/";
        
        try
        {
            if (!Directory.Exists(sPth))
                Directory.CreateDirectory(sPth);
        }
        catch(Exception exp)
        {
            Debug.Log("GetLocal() DirEr = " + sPth + ", Msg = " + exp.Message);
        }

        return sPth;
    }

    // 测试本地资源是否存在
    static public bool LocalResExist()
    {
        string sPth = GetLocal();
        try
        {
            if (!Directory.Exists(sPth))
                return false;
        }
        catch(Exception exp)
        {
            Debug.Log("LocalResExist() DirEr = " + sPth + ", Msg = " + exp.Message);
            return false;
        }

        string sf = GetLocal(ResPath.GetResListTxt());
        try
        {
            if (File.Exists(sf))
                return true;
        }
        catch(Exception exp)
        {
            Debug.Log("LocalResExist() File.DirEr = " + sf + ", Msg = " + exp.Message);
        }
        return false;
    }

    static public void ClearLocal()
    {
        string sPth = GetLocal();
        try
        {
            if (Directory.Exists(sPth))
            {
                string[] fls = Directory.GetFiles(sPth);
                foreach (string sf in fls)
                {
                    try
                    {
                        File.Delete(sf);
                    }
                    catch(Exception exp)
                    {
                        Debug.Log("ClearLocal() File.Delete DirEr = " + sf + ", Msg = " + exp.Message);
                    }
                }
            }
        }
        catch(Exception exp)
        {
            Debug.Log("ClearLocal() DirEr = " + sPth + ", Msg = " + exp.Message);
        }
    }

    //------------------------------------------------------------------
    // 目录不存在则创建, 文件存在则删除
    static public void FileTest(string sf)
    {
        string folder = Path.GetDirectoryName(sf);
        try
        {
		    if (!Directory.Exists(folder))
			    Directory.CreateDirectory(folder);
        }
        catch(Exception exp)
        {
            Debug.Log("FileTest() DirEr = " + folder + ", Msg = " + exp.Message);
        }
        try
        {
            if (File.Exists(sf))
                File.Delete(sf);
        }
        catch(Exception exp)
        {
            Debug.Log("FileTest() File.DirEr = " + sf + ", Msg = " + exp.Message);
        }
    }
    
    //------------------------------------------------------------------
    // 保存到本地
    static public bool SaveToLocal(string sf, byte [] dt)
    {
        try
        {
            FileTest(sf);
            FileStream fs = new FileStream(sf, FileMode.CreateNew);
            fs.Write(dt, 0, dt.Length);
            fs.Close();
            return true;
        }
        catch (Exception exp)
        {
            Debug.Log("SaveToLocal failed: file = " + sf + ", Msg = " + exp.Message);
        }
        return false;
    }
    
    //------------------------------------------------------------------
    static public byte[] GetFileData(string sf)
    {
        byte[] vAry = null;
        try
        {
            FileStream stm = new FileStream(sf, FileMode.Open, FileAccess.Read);
            if (stm.Length > 0)
            {
                vAry = new byte[stm.Length];
                stm.Read(vAry, 0, vAry.Length);
            }
            stm.Close();
        }
        catch (Exception exp)
        {
            Debug.LogWarning("CAN NOT Open file = " + sf + ", Msg = " + exp.Message);
        }
        return vAry;
    }
    //////////////////////////////////////////////////////////////////////////////
    //new
    //服务器资源URL
    #region ServerResourceURL
    static string url = "";
    static public string ServerResourceURL
    {
        get
        {
            return url;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                string szPth = "";
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        {
                            szPth = "Android/";
                            break;
                        }
                    case RuntimePlatform.IPhonePlayer:
                        {
                            szPth = "iPhone/";
                            break;
                        }
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.WindowsEditor:
                        {
                            szPth = "Win32/";
                            break;
                        }
                    case RuntimePlatform.OSXPlayer:
                        {
                            szPth = "MACOS/";
                            break;
                        }
                    default:
                        {
                            Debug.LogWarning("GetUrl(): Unknown, Platform = " + Application.platform.ToString());
                            break;
                        }
                }
                url = value + szPth;
                //test
                Debug.LogWarning("server url:" + url);
            }
        }
    }
    #endregion
    //版本XML文件的名字
    static public string VersionFileName { get { return "Version.xml"; } }
    //PackMD5.xml
    static public string PackMD5FileName { get { return "PackMD5.xml"; } }
    //FilePack.xml
    static public string FilePackFileName { get { return "FilePack.xml"; } }
    //PackDepend.xml
    static public string PackDependFileName { get { return "PackDepend.xml"; } }
    //包文件的扩展名
    static public string PackageExtension { get { return ".pack"; } }
    //www读取包时的本地URL的前缀
    static public string WLocalURLPrefix { get { return "file://"; } }
    //获得服务器的URL
    static public string GetServerURL(string filename)
    {
        if (string.IsNullOrEmpty(ServerResourceURL))
        {
            Debug.LogWarning("ServerResourceURL is null!");
            return "";
        }
        return ServerResourceURL + filename;
    }
    //获得服务器的包的url
    static public string GetServerPackURL(string filename)
    {
        if (string.IsNullOrEmpty(ServerResourceURL))
        {
            Debug.LogWarning("ServerResourceURL is null!");
            return "";
        }
        return ServerResourceURL + "Package/" + filename;
    }
    //编译不同版本的目录后缀
    static public string AndroidPathName { get { return "/Android/"; } }
    static public string iPhonePathName { get { return "/iPhone/"; } }
    static public string Win32PathName { get { return "/Win32/"; } }
    static public string CurrentPathName
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return AndroidPathName;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return iPhonePathName;
            }
            else
            {
                return Win32PathName;
            }
        }
    }
    //本地路径
    #region LocalPath
    static string m_localPath = "";
    static public string LocalPath
    {
        get
        {
            if (string.IsNullOrEmpty(m_localPath))
            {
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    m_localPath = Application.persistentDataPath;
                }
                else
                {
                    m_localPath = Application.streamingAssetsPath;
                }
                try
                {
                    if (!Directory.Exists(m_localPath))
                    {
                        Directory.CreateDirectory(m_localPath);
                    }
                }
                catch (Exception exp)
                {
                    Debug.Log("create m_localPath error, dir = " + m_localPath + ", Msg = " + exp.Message);
                }
            }
            return m_localPath;
        }
    }
    #endregion
    //本地临时缓存目录（所有下载下来的文件保存在此目录下）
    #region LocalTempCachePath
    static string m_localTempCachePath = "";
    static public string LocalTempCachePath
    {
        get
        {
            if (string.IsNullOrEmpty(m_localTempCachePath))
            {
                m_localTempCachePath = LocalPath + CurrentPathName + "Temp/";
                try
                {
                    if (!Directory.Exists(m_localTempCachePath))
                    {
                        Directory.CreateDirectory(m_localTempCachePath);
                    }
                }
                catch (Exception exp)
                {
                    Debug.LogWarning("create m_localTempCachePath error, dir = " + m_localTempCachePath + ", Msg = " + exp.Message);
                }
            }
            return m_localTempCachePath;
        }
    }
    #endregion
    //本地包路径
    #region LocalPackagePath
    static string m_localPackagePath = "";
    static public string LocalPackagePath
    {
        get
        {
            if (string.IsNullOrEmpty(m_localPackagePath))
            {
                m_localPackagePath = LocalPath + CurrentPathName + "Package/";
                try
                {
                    if (!Directory.Exists(m_localPackagePath))
                    {
                        Directory.CreateDirectory(m_localPackagePath);
                    }
                }
                catch (Exception exp)
                {
                    Debug.LogWarning("create m_localPackagePath error, dir = " + m_localPackagePath + ", Msg = " + exp.Message);
                }
            }
            return m_localPackagePath;
        }
    }
    #endregion
}
