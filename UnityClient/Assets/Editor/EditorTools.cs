using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorTools
{
	[@MenuItem("EditorTools/Copy Animation")]
	public static void CopyAnim()
	{
		foreach (GameObject obj in Selection.gameObjects)
		{
			Animation anim = obj.GetComponent<Animation>();
			if (null == anim)
			{
				continue;
			}
			string path = AssetDatabase.GetAssetPath(anim.clip);
			path = Path.GetDirectoryName(path);
            path = path + "/";
			Debug.Log("Source Path : " + path);
			if (path.Contains("Resources"))
			{
				Debug.Log(path);
			}
			else
			{
				if (path.Contains("Import"))
				{
					int startIndex = path.IndexOf("Import") + "Import".Length;
					int length = path.LastIndexOf("/") - startIndex;
					path = path.Substring(startIndex, length) + "/" + anim.name;
					Debug.Log(path);
				}
				else
				{
					int startIndex = path.IndexOf("Assets/") + "Assets/".Length;
					int length = path.LastIndexOf("/") - startIndex;
					path = path.Substring(startIndex, length) + "/" + anim.name;
					Debug.Log(path);
				}
                path = "Assets/Extends" + path;
			}

			if (Directory.Exists(path))
			{
				Debug.Log("Exists:" + path);
				foreach (AnimationState state in anim)
				{
					string wholeFile = path + "/" + state.name + ".anim";
					Debug.Log(wholeFile);
					if (File.Exists(wholeFile))
					{
						AnimationClip clip = GameObject.Instantiate(state.clip) as AnimationClip;
						AnimationClip oldClip = AssetDatabase.LoadAssetAtPath(wholeFile, typeof(AnimationClip)) as AnimationClip;
                        AnimationEvent[] animArr = AnimationUtility.GetAnimationEvents(oldClip);
                        AnimationUtility.SetAnimationEvents(clip, animArr);

						AnimationClipCurveData[] curvesArray = AnimationUtility.GetAllCurves(oldClip);
						foreach (AnimationClipCurveData item in curvesArray)
						{
							if (item.type == typeof(Transform))
							{
								continue;
							}
							clip.SetCurve(item.path, item.type, item.propertyName, item.curve);
						}
						clip.wrapMode = oldClip.wrapMode;
						AssetDatabase.CreateAsset(clip, wholeFile);

					}
					else
					{
						AnimationClip clip = GameObject.Instantiate(state.clip) as AnimationClip;
						AssetDatabase.CreateAsset(clip, wholeFile);
					}
				}
				AssetDatabase.Refresh();
			}
			else
			{
				Debug.Log("!Exists");
				Directory.CreateDirectory(path);
				Debug.Log(path);
				foreach (AnimationState state in anim)
				{
					AnimationClip clip = GameObject.Instantiate(state.clip) as AnimationClip;
					Debug.Log(path + "/" + state.name + ".anim");
					AssetDatabase.CreateAsset(clip, path + "/" + state.name + ".anim");
				}
				AssetDatabase.Refresh();
			}
		}
	}
}