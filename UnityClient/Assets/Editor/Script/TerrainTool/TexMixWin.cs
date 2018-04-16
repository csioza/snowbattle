
using System.IO;
using UnityEngine;
using UnityEditor;

using System.Collections;

public class TexMixWin : EditorWindow
{
//	TerrainEditor	mEditor;
	
	public void Init()
	{
//		mEditor = FindObjectOfType(typeof(TerrainEditor)) as TerrainEditor;
	}
	
	
	void OnGUI()
	{
		TexMixUtil.mMixLvl = EditorGUILayout.IntField("Edit Lvl:", TexMixUtil.mMixLvl);
		TexMixUtil.mMixLvl = Mathf.Clamp(TexMixUtil.mMixLvl, 0, 3);
		TexMixUtil.mbErase = EditorGUILayout.Toggle("Erase", TexMixUtil.mbErase);
		TexMixUtil.mfValue = EditorGUILayout.Slider("Force", TexMixUtil.mfValue, 0.0f, 1.0f);
		GUILayout.BeginHorizontal();
		GUILayout.Label(TexMixBrush.GetTexObj(), GUILayout.Width(64.0f), GUILayout.Height(64.0f));
		GUILayout.Space(80.0f);
		if(GUILayout.Button("Load    Brush", GUILayout.Width(100.0f)))
		{
			LoadBrushImage();
		}
		GUILayout.EndHorizontal();
	}
	
	void LoadBrushImage()
	{
		string szDir = Application.dataPath + "/Editor/Resources/Brushes/";
		string file = EditorUtility.OpenFilePanel("Load Brush", szDir, "");
		if(file.Length > 0)
		{
			string szFNm = "Brushes/" + Path.GetFileNameWithoutExtension(file);
			TexMixBrush.NtfCreateBrush(szFNm);
		}
	}
	
}
