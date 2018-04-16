using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class FindReferences : EditorWindow
{
	static List<UnityEngine.Object> RefObjectList = new List<UnityEngine.Object>();
	static List<string> PrefabPathList = new List<string>();
	static int CurReferenceIndex = 0;
	
	[MenuItem("Assets/Find References")]
	static void FindObjects()
	{
		if (null != Selection.activeObject)
		{
			FindReferences window = GetWindow<FindReferences>();
			window.Show();
			window.title = "Find References";
			window.selectedObject = Selection.activeObject;
			SetReferenceList();
		}
	}
	
	static void SetReferenceList(DirectoryInfo folder = null)
	{
		if (null == folder)
		{
			CurReferenceIndex = 0;
			PrefabPathList.Clear();
			RefObjectList.Clear();
			folder = new DirectoryInfo(Application.dataPath);
		}
		
		foreach(FileInfo file in folder.GetFiles())
		{
			if (file.Extension.Equals(".prefab"))
			{
				PrefabPathList.Add(file.FullName.Substring(file.FullName.IndexOf("Assets"+Path.DirectorySeparatorChar)));
			}
		}
		
		foreach (DirectoryInfo dir in folder.GetDirectories())
		{
			SetReferenceList(dir);
		}
	}
	
	static void FindObjectRef(string assetPath)
	{
		UnityEngine.Object refObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
		string[] dependencies = AssetDatabase.GetDependencies(new string[] { assetPath });
		foreach (string depend in dependencies)
		{
			UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(depend, typeof(UnityEngine.Object));
			if (obj.GetType().Equals(Selection.activeObject.GetType()/*typeof(MonoScript)*/) && obj.name.Equals(Selection.activeObject.name))
			{
				RefObjectList.Add(refObj);
			}
		}
	}
	
	void OnGUI()
	{
		if (CurReferenceIndex < PrefabPathList.Count)
		{
			float percent = (float)CurReferenceIndex / PrefabPathList.Count;
			EditorUtility.DisplayProgressBar("Find References", PrefabPathList[CurReferenceIndex], percent);
			FindObjectRef(PrefabPathList[CurReferenceIndex]);
			CurReferenceIndex++;
		}
		else
		{
			EditorUtility.ClearProgressBar();
		}
		
		EditorGUILayout.ObjectField(selectedObject, typeof(UnityEngine.Object), false);
		EditorGUILayout.LabelField("Find References Count : " + RefObjectList.Count);
		foreach (UnityEngine.Object obj in RefObjectList)
		{
			EditorGUILayout.ObjectField(obj, typeof(UnityEngine.Object), false);
		}
	}
	
	void OnInspectorUpdate()
	{
		this.Repaint();
	}
	
	public UnityEngine.Object selectedObject { get; set; }
}
