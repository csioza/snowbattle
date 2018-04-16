
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

//-------------------------------------------------------------------------------------------------------------------
//
// 菜单: 制作资源包(不包含 *.unity 场景文件)
// 包括:
//      <1> 阶段性整体资源包
//      <2> 更新资源包
//
public class CMenuBuildPack
{
    /////////////////////BuildGamePackageFromXML///////////////////////////////////////////////
    //[@MenuItem(@"Package/BuildTest.xml(StandaloneWindows)")]
    public static void BuildTest_StandaloneWindows()
    {
        BuildPackFromXML.Singleton.Build("Package/BuildTest.xml", BuildTarget.StandaloneWindows, ResPath.Win32PathName);
    }
    [@MenuItem(@"Package/BuildGamePackage.xml(StandaloneWindows)")]
    public static void BuildGamePackage_StandaloneWindows()
    {
        BuildPackFromXML.Singleton.Build("Package/BuildGamePackage.xml", BuildTarget.StandaloneWindows, ResPath.Win32PathName);
    }
    [@MenuItem(@"Package/BuildGamePackage.xml(Android)")]
    public static void BuildGamePackage_Android()
    {
        BuildPackFromXML.Singleton.Build("Package/BuildGamePackage.xml", BuildTarget.Android, ResPath.AndroidPathName);
    }
    [@MenuItem(@"Package/BuildGamePackage.xml(iPhone)")]
    public static void BuildGamePackage_iPhone()
    {
        BuildPackFromXML.Singleton.Build("Package/BuildGamePackage.xml", BuildTarget.iOS, ResPath.iPhonePathName);
    }
    ///////////////////////////////////////////////////////////////////////////////////////////
    //--------------------------------------------------------------------------------------------
    //[@MenuItem(@"Package/游戏发布")]
    public static void BuildGameWindow()
    {
        EditorWindow.GetWindow(typeof(BuildGameWindow));
    }

    //[@MenuItem(@"Package/版本控制")]
    public static void BuildPack()
    {
        EditorWindow.GetWindow(typeof(BuildPackWindow));
    }
    //[@MenuItem(@"打包版本/编译三国（简单测试用）")]
    //public static void BuildResourcesB()
    //{
		
    //}

    //[@MenuItem(@"Package/构建场景列表")]
	public static void BuildSettingsScene()
	{
        ArchiveUtil.NtfInitSkips();
        ArchiveUtil.AddSkip("NGUI");
        ArchiveUtil.AddSkip("Plugins");
		List<string> vScn = ArchiveUtil.NtfGetFiles(Application.dataPath, true, "*.unity");
        if (vScn.Count > 0)
        {
            EditorBuildSettingsScene [] scnAry = new EditorBuildSettingsScene[vScn.Count];
            for (int i = 0; i < vScn.Count; ++i )
            {
                scnAry[i] = new EditorBuildSettingsScene(vScn[i], true);
            }
            EditorBuildSettings.scenes = scnAry;
        }
        ArchiveUtil.NtfInitSkips();
        Debug.Log(@"场景列表构建结束, 共加入 [ " + vScn.Count.ToString() + @" ] 个场景");
	}
}
