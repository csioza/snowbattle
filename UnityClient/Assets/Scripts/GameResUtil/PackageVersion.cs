using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

//打包文件的版本号控制
public class PackageVersion {
    public struct Version
    {
        public int m_version;
        public bool m_isExternPackage;
        public bool m_isAliveInRuntime;
        public void Init()
        {
            m_isExternPackage = false;
            m_isAliveInRuntime = false;
        }
    }
    public Dictionary<string, Version> m_versions = new Dictionary<string, Version>();

    public Version LookupPackageVersion(string name)
    {
        if (m_versions.ContainsKey(name))
        {
            return m_versions[name];
        }
        return new Version();
    }

    public bool HavePackage(string name)
    {
        return m_versions.ContainsKey(name);
    }
    public void AddPackage(string name,int version,bool externPackage, bool aliveInRuntime)
    {
        Version v = new Version();
        v.Init();
        v.m_isExternPackage = externPackage;
        v.m_isAliveInRuntime = aliveInRuntime;
        v.m_version = version;
        m_versions[name] = v;
    }
    public void AddPackage(string name, Version v)
    {
        m_versions[name] = v;
    }
    public void IncreasePackageVersion(string name)
    {
        if (m_versions.ContainsKey(name))
        {
            Version v = m_versions[name];
            v.m_version++;
            m_versions[name] = v;
        }
    }

    //从本地加载对应的包的版本号
    public void LoadPackagesVersion(string localFile)
    {
        byte[] dt = ResPath.GetFileData(localFile);
        LoadPackagesVersion(dt);
    }
    public void SavePackageVersion(string localFile)
    {
        try
        {
            FileStream fstm = new FileStream(localFile, FileMode.Create);
            StreamWriter stream = new StreamWriter(fstm);

            foreach (KeyValuePair<string, Version> p in m_versions)
            {
                Version v = p.Value;
                string data = p.Key + "|" + v.m_version.ToString();
                if (v.m_isAliveInRuntime)
                {
                    data += "|A";
                }
                if (v.m_isExternPackage)
                {
                    data += "|E";
                }
                data += "\n";
                stream.Write(data.ToCharArray());
            }
            stream.Close();
            fstm.Close();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error on save package version:" + ex.Message);
        }
    }
    void LoadPackagesVersion(byte[] buffer)
    {
        if (buffer==null)
        {
            return;
        }
        MemoryStream mryStm = new MemoryStream(buffer);
        StreamReader stm = new StreamReader(mryStm);
        string line = stm.ReadLine();
        while (line != null)
        {
            if (line.Length == 0)
            {
                continue;
            }
            Version v = new Version();
            string[] nameAndVersion = line.Split('|');
            
            if (nameAndVersion.Length>1)
            {
                v.m_version = int.Parse(nameAndVersion[1]);
            }
            for (int i = 2; i < nameAndVersion.Length;i++ )
            {
                if (nameAndVersion[i] == "E")
                {
                    v.m_isExternPackage = true;
                }
                else if (nameAndVersion[i] == "A")
                {
                    v.m_isAliveInRuntime = true;
                }
            }
            m_versions[nameAndVersion[0]] = v;
            line = stm.ReadLine();
        }
    }
}
