using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Xml;


public class EDCompileEngineEditor : EditorWindow
{
    [@MenuItem("DllCompile/CompileEngine")]
    public static void CompileEngine()
    {
        EDCompileEngine.Instance.CompileEngine();
    }
    [@MenuItem("DllCompile/CompileEditor")]
    public static void CompileEditor()
    {
        EDCompileEngine.Instance.CompileEditor();
    }
}

public class EDCompileEngine
{
    public string ScriptPath
    {
        get
        {
            return Application.dataPath + "/Scripts";
        }
    }
    public string ToolsPath
    {
        get
        {
            return Application.dataPath + "/../../dll_generator";
        }
    }
    List<string> _scriptPaths;
    public List<string> AllScriptPath
    {
        get
        {
            if (_scriptPaths == null)
            {
                _scriptPaths = new List<string>();
                _scriptPaths.Add(ScriptPath);
                _scriptPaths.Add(Application.dataPath + "/Dependencies/NGUI/Scripts");
                _scriptPaths.Add(Application.dataPath + "/Plugins");
                
            }
            return _scriptPaths;
        }
    }
    List<string> _editorPaths;
    public List<string> AllEditorScriptPath
    {
        get
        {
            if (_editorPaths == null)
            {
                _editorPaths = new List<string>();
                _editorPaths.Add(Application.dataPath + "/Editor");
            }
            return _editorPaths;
        }
    }
    static EDCompileEngine m_instance;
    static public EDCompileEngine Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new EDCompileEngine();
            }
            return m_instance;
        }
    }
    public void SearchAllCompileFiles(string szPath,List<string> files,bool isOnlyEditor)
    {
        string[] allfiles = Directory.GetFiles(szPath, "*.cs");
        for (int i = 0; i < allfiles.Length;i++ )
        {
            string f = allfiles[i];
            files.Add(f);
        }
        allfiles = Directory.GetDirectories(szPath);
        for (int i = 0; i < allfiles.Length; i++)
        {
            string f = allfiles[i];
            if (isOnlyEditor)
            {
                if (f.Contains("Editor"))
                {
                    SearchAllCompileFiles(f, files, isOnlyEditor);
                }
            }
            else
            {
                if (f.Contains("Editor"))
                {
                    continue;
                }
                SearchAllCompileFiles(f, files, isOnlyEditor);
            }
        }
    }

    XmlDocument m_srcPrjXml;
    XmlElement m_itemGroupXml;
    void ReadMono2012(string monoPrjName)
    {
        m_srcPrjXml = new XmlDocument();
        m_srcPrjXml.Load(monoPrjName);
        m_itemGroupXml = null;
        XmlElement src_root = m_srcPrjXml.DocumentElement;
        XmlNodeList itemGroup = src_root.GetElementsByTagName("ItemGroup");
        foreach (XmlNode groupNode in itemGroup)
        {
            XmlElement group_elem = (XmlElement)groupNode;
            foreach (XmlNode cnode in group_elem.ChildNodes)
            {
                if (cnode is XmlElement)
                {
                    if (cnode.Name == "Compile")
                    {
                        m_itemGroupXml = group_elem;
                        return;
                    }
                }
            }
        }
        m_itemGroupXml = m_srcPrjXml.CreateElement("ItemGroup") as XmlElement;
        src_root.AppendChild(m_itemGroupXml);
    }

    public void CompileEditor()
    {
        string text = "";
        List<string> allfiles = new List<string>();
        List<string> allPath = AllEditorScriptPath;
        for (int i = 0; i < allPath.Count; i++)
        {
            SearchAllCompileFiles(allPath[i], allfiles, true);
        }
        ReadMono2012(ToolsPath + "/arpg_editor/arpg_editor.csproj");
        m_itemGroupXml.RemoveAll();
        for (int i = 0; i < allfiles.Count; i++)
        {
            string f = allfiles[i];
            text += "\"";
            text += f;
            text += "\" ";
            //Debug.Log(string.Format("compile:{0}",f));
            XmlElement elem = m_srcPrjXml.CreateElement("Compile") as XmlElement;
            XmlAttribute attr_elem = m_srcPrjXml.CreateAttribute("Include");
            attr_elem.InnerText = f;
            elem.Attributes.Append(attr_elem);
            m_itemGroupXml.AppendChild(elem);
        }
        m_srcPrjXml.Save(ToolsPath + "/arpg_editor/arpg_editor.csproj");
        //try
        //{
        //    string complieListFileName = ToolsPath + "/all_script.txt";
        //    FileStream fstm = new FileStream(complieListFileName, FileMode.Create);
        //    StreamWriter stream = new StreamWriter(fstm);
        //    stream.Write(text);
        //    stream.Close();
        //    fstm.Close();
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.LogError("Error on write complieListFile version:" + ex.Message);
        //}
    }
    public void CompileEngine()
    {
        string text = "";
        List<string> allfiles = new List<string>();
        List<string> allPath = AllScriptPath;
        for (int i = 0; i < allPath.Count;i++ )
        {
            SearchAllCompileFiles(allPath[i], allfiles,false);
        }
        ReadMono2012(ToolsPath + "/arpg_engine/arpg_engine.csproj");
        m_itemGroupXml.RemoveAll();
        for (int i=0;i<allfiles.Count;i++)
        {
            string f = allfiles[i];
            text += "\"";
            text += f;
            text += "\" ";
            //Debug.Log(string.Format("compile:{0}",f));
            XmlElement elem = m_srcPrjXml.CreateElement("Compile") as XmlElement;
            XmlAttribute attr_elem = m_srcPrjXml.CreateAttribute("Include");
            attr_elem.InnerText = f;
            elem.Attributes.Append(attr_elem);
            m_itemGroupXml.AppendChild(elem);
        }
        m_srcPrjXml.Save(ToolsPath + "/arpg_engine/arpg_engine.csproj");
        //try
        //{
        //    string complieListFileName = ToolsPath + "/all_script.txt";
        //    FileStream fstm = new FileStream(complieListFileName, FileMode.Create);
        //    StreamWriter stream = new StreamWriter(fstm);
        //    stream.Write(text);
        //    stream.Close();
        //    fstm.Close();
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.LogError("Error on write complieListFile version:" + ex.Message);
        //}
    }
}
