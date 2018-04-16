using System;
using UnityEditor;
using UnityEngine;

public class BuildPackFromXML
{
    #region Singleton
    static BuildPackFromXML m_singleton;
    static public BuildPackFromXML Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new BuildPackFromXML();
            }
            return m_singleton;
        }
    }
    #endregion
    public PackageTree PackTree = null;
    public BuildTarget Target = BuildTarget.Android;
    string ExpendName = "";
    public void Build(string filename, BuildTarget target, string expendName = "")
    {
        SuccPackageCount = 0;
        FailPackageCount = 0;
        NullPackageCount = 0;
        PushPackageCount = 0;
        Target = target;
        ExpendName = expendName;

        PackTree = new PackageTree();
        PackTree.BuildPackage(filename);

        Debug.Log("package total:" + PackTree.AllPackageList.Count +
            ",succ:" + SuccPackageCount +
            ",fail:" + FailPackageCount +
            ",null:" + NullPackageCount);
    }
    //保存xml的目录
    public string XMLDir
    {
        get
        {
            //return ResPath.LocalPath + ExpendName;
            return Application.dataPath + "/../Package" + ExpendName;
            //return ResPath.LocalPath + ExpendName;
        }
    }
    //保存package的目录
    string m_packageDir = "";
    public string PackageDir
    {
        get
        {
            if (string.IsNullOrEmpty(m_packageDir))
            {
                m_packageDir = XMLDir + "Package/";
                if (!System.IO.Directory.Exists(m_packageDir))
                {
                    System.IO.Directory.CreateDirectory(m_packageDir);
                }
            }
            return m_packageDir;
        }
    }
    //打包计数
    public int SuccPackageCount = 0;
    public int FailPackageCount = 0;
    public int NullPackageCount = 0;
    public int PushPackageCount = 0;
}