
#define RES_MNG_ENABLE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameData
{
    #region Singleton
    static GameData m_singleton;
    static public GameData Singleton
	{
		get
		{
			if (m_singleton == null)
			{
                m_singleton = new GameData();
			}
			return m_singleton;
		}
	}
	#endregion
    static string mBasePath { get { return ""; } }

    static System.Text.StringBuilder mStringBuilder = new System.Text.StringBuilder();
    //prefab
    static public string GetPrefabPath(string szfile)
    {
        mStringBuilder.Length = 0;
        mStringBuilder.Append(mBasePath);
        mStringBuilder.Append("Prefabs/");
        mStringBuilder.Append(szfile);
        mStringBuilder.Append(".prefab");
        return mStringBuilder.ToString();
    }
    //icon
    static public string GetIconPath(string filename)
    {
        mStringBuilder.Length = 0;
        mStringBuilder.Append(mBasePath);
        mStringBuilder.Append("Icon/");
        mStringBuilder.Append(filename);
        mStringBuilder.Append(".png");
        return mStringBuilder.ToString();
    }
    //prefabs/ui
    static public string GetUIPath(string filename)
    {
        mStringBuilder.Length = 0;
        mStringBuilder.Append(mBasePath);
        mStringBuilder.Append("Prefabs/UI/");
        mStringBuilder.Append(filename);
        mStringBuilder.Append(".prefab");
        return mStringBuilder.ToString();
    }
    //UIRes/Atlas
    static public string GetUIAtlasPath(string filename)
    {
        mStringBuilder.Length = 0;
        mStringBuilder.Append(mBasePath);
        mStringBuilder.Append("UIRes/Atlas/");
        mStringBuilder.Append(filename);
        mStringBuilder.Append(".prefab");
        return mStringBuilder.ToString();
    }
    //sound
    static public string GetSoundPath(string filename)
    {
        mStringBuilder.Length = 0;
        mStringBuilder.Append(mBasePath);
        mStringBuilder.Append("Sound/");
        mStringBuilder.Append(filename);
        return mStringBuilder.ToString();
    }
    //effect
    static public string GetEffectPath(string filename)
    {
        mStringBuilder.Length = 0;
        mStringBuilder.Append(mBasePath);
        mStringBuilder.Append("Prefabs/Effect/");
        mStringBuilder.Append(filename);
        mStringBuilder.Append(".prefab");
        return mStringBuilder.ToString();
    }
    //SkillAnim
    static public string GetSkillAnimPath(string filename)
    {
        mStringBuilder.Length = 0;
        mStringBuilder.Append(mBasePath);
        mStringBuilder.Append("SkillAnim/");
        mStringBuilder.Append(filename);
        mStringBuilder.Append(".anim");
        return mStringBuilder.ToString();
    }
    //config
    static public string GetConfigPath(string filename)
    {
        mStringBuilder.Length = 0;
        mStringBuilder.Append(mBasePath);
        mStringBuilder.Append("Config/");
        mStringBuilder.Append(filename);
        mStringBuilder.Append(".bytes");
        return mStringBuilder.ToString();
    }
    // path要求: Assets/ 之后的完整路径
    // 例如: E:\Unity3D\XProject\trunk\Assets\Scripts\Main.cs          则输入 Scripts\Main.cs
    // 例如: E:\Unity3D\XProject\trunk\Assets\Resources\Images\map.png 则输入 Resources\Images\map.png
    // 具体的参考 GameResMng 文件里顶部说明
    static public UnityEngine.Object Load(string path)
    {
        UnityEngine.Object obj = GameResManager.Singleton.LoadResource(mBasePath + path);
        if (obj == null)
        {
            Debug.LogWarning("load fail, path:" + path);
        }
        return obj;
    }

    static public T LoadPrefab<T>(string path) where T : UnityEngine.Object
    {
        mStringBuilder.Length = 0;
        mStringBuilder.Append(mBasePath);
        mStringBuilder.Append("Prefabs/");
        mStringBuilder.Append(path);
        mStringBuilder.Append(".prefab");
        return Load(mStringBuilder.ToString()) as T;
    }

    static public T LoadActor<T>(string path) where T : UnityEngine.Object
    {
        mStringBuilder.Length = 0;
        mStringBuilder.Append(mBasePath);
        mStringBuilder.Append("Prefabs/");
        mStringBuilder.Append(path);
        mStringBuilder.Append(".prefab");
        return Load(mStringBuilder.ToString()) as T;
    }
    static public T LoadConfig<T>(string path) where T : UnityEngine.Object
    {
        mStringBuilder.Length = 0;
        mStringBuilder.Append(mBasePath);
        mStringBuilder.Append("Config/");
        mStringBuilder.Append(path);
        mStringBuilder.Append(".bytes");
        return Load(mStringBuilder.ToString()) as T;
    }
}