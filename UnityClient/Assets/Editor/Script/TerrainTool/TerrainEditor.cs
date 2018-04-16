
using UnityEngine;
using UnityEditor;
//using System.Collections.Generic;
using System.IO;
using System.Collections;
using Object = UnityEngine.Object;

// 鐢ㄩ紶鏍囨嬀鍙栨ā鍨嬮《鐐根  http://www.cosdiv.com/page/M0/S830/830846.html
// Add Your Own Tools : http://active.tutsplus.com/tutorials/workflow/how-to-add-your-own-tools-to-unitys-editor/

//public class RayEditorBase<T> : Editor where T : Object
//{
//	protected T Target { get { return (T) target; } }
//}

// MouseMove
// Modify Texture
// Refresh Texture
// Write To File  ====> PNG !!!
// Brush,...
// Smooth,...
// Undo-Redo

// http://docs.unity3d.com/Documentation/ScriptReference/ExecuteInEditMode.html
[ExecuteInEditMode]


[CustomEditor (typeof(TerrainBrush))] 
public class TerrainEditor : Editor
{
	const int TEOM_CLOSE  = 0;
	const int TEOM_ENABLE = 1;
	const int TEOM_PAUSE  = 2;
	
	static int			mCurOp=0;	// 0(鍏抽棴)/1(鍚?姩)/2(鏆傚仠)
	TerrainBrush		mEditor;
	static GameObject	mSelObj;	// 缂栬緫瀵硅薄
	static Vector2		mLastClk;
	
	
	
	static bool IsEditEnable(){return (mCurOp == TEOM_ENABLE);}
	static bool IsEditPause(){return (mCurOp == TEOM_PAUSE);}
	
	public void OnEnable()
	{
		mEditor = target as TerrainBrush;			// 
		SceneView.onSceneGUIDelegate += OnEditorEvent;	// Add a Delegate
	}
	
	public void OnDisabel()
	{
		SceneView.onSceneGUIDelegate -= OnEditorEvent;	// Remove our Delegate
		//mEditor = null;
	}
	//---------------------------------------------------------------------------------------------------------
	//
	// 鑿滃崟
	//
	[MenuItem("EditorTools/Terrain Editor/Blend Texture")]
	static void Execute()
	{
#if !UNITY_STANDALONE_WIN
		Debug.LogError("Only WORK on Windows Platform: file--> Build Setting --> PC, Mac.. --> Windows");
#endif
		GameObject obj = EditorUtil.GetEditTarget();
		//if(obj != mSelObj)
		{
			mSelObj = obj;
			TexMixBrush.NtfCreateBrush("Brushes/brush_14");
			if(TexMixUtil.NtfAttachTarget(mSelObj))
			{
				mCurOp = TEOM_ENABLE;
			}
		}
		
	}
	
	[MenuItem("EditorTools/Terrain Editor/Save Blend")]
	static void SaveBlendTexture()
	{
		if(mSelObj == null)
		{
			EditorUtility.DisplayDialog("Save Texture", "No Texture!", "Ok");
			return;
		}
		
		string szFl  = TexMixUtil.GetTexFile();//GetTexName();
		string szDir = Application.dataPath + "/";
		string szFNm = "";
		if(szFl.Length > 0)
		{
			szDir = Path.GetDirectoryName(szFl);
			szFNm = Path.GetFileName(szFl);
		}
		string file = EditorUtility.SaveFilePanel("Save Texture", szDir, szFNm, "png");
		TexMixUtil.NtfSaveTexture(file);
		
	}
		
	[MenuItem("EditorTools/Terrain Editor/Blend Win")]
	static void BlendTextureWin()
	{
		TexMixWin win = (TexMixWin) EditorWindow.GetWindow(typeof(TexMixWin));
		win.Init();
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
	
	public override void OnInspectorGUI()
	{
		if(mSelObj != null)
		{
			Vector3 vsize = mSelObj.GetComponent<Collider>().bounds.size;
			float fu = 1.0f / vsize.x;
			float fv = 1.0f / vsize.z;
			string szScl = fu.ToString() + ", " + fv.ToString();
			EditorGUILayout.TextField("Bound Box:", vsize.ToString());
			EditorGUILayout.TextField("UVScale:", szScl);
		}
		
		TexMixUtil.mMixLvl = EditorGUILayout.IntField("Edit Lvl:", TexMixUtil.mMixLvl);
		TexMixUtil.mMixLvl = Mathf.Clamp(TexMixUtil.mMixLvl, 0, 3);
		TexMixUtil.mbErase = EditorGUILayout.Toggle("Erase", TexMixUtil.mbErase);
		TexMixUtil.mfValue = EditorGUILayout.Slider("Force", TexMixUtil.mfValue, 0.0f, 1.0f);
		GUILayout.BeginHorizontal();
		GUILayout.Label(TexMixBrush.GetTexObj(), GUILayout.Width(64.0f), GUILayout.Height(64.0f));
		GUILayout.Space(75.0f);
		if(GUILayout.Button("Load  Brush", GUILayout.Width(100.0f)))
		{
			LoadBrushImage();
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(12.0f);
		if(GUILayout.Button("Undo", GUILayout.Width(80.0f)))
		{
			TexMixUtil.NtfExcuteUndo();
		}
		GUILayout.Space(12.0f);
		if(GUILayout.Button("Redo", GUILayout.Width(80.0f)))
		{
			TexMixUtil.NtfExcuteRedo();
		}
		GUILayout.Space(12.0f);
		if(GUILayout.Button("Clear", GUILayout.Width(80.0f)))
		{
			TexMixUtil.NtfClearCurLvl(TexMixUtil.mMixLvl);
		}
		GUILayout.EndHorizontal();
		 SceneView.RepaintAll(); 	// Repaint SceneView
	}
	
	// Event
	void OnEditorEvent(SceneView sceneview)
	{
		if(OnMouseDownEvent()) return;
		if(OnMouseMoveEvent()) return;
		if(OnKeyDownEvent())   return;
		OnKeyUpEvent();
		
	}
	
	// Mouse Down Event
	bool OnMouseDownEvent()
	{
		Event evn = Event.current;
		if(!evn.isMouse || (evn.type != EventType.MouseDown))
			return false;
		mLastClk = evn.mousePosition;
		if(!IsEditEnable()) return true;
		if(evn.button == 0)	// Left Button
		{
		}
		else if(evn.button == 1)	// Right Button
		{
			RaycastHit hit;
			if(EditorUtil.OnHitTest(out hit))
			{
				//EditorUtil.NtfLogHitMsg("Hit", hit);
				TexMixUtil.BeginCMD();
				//TexMixUtil.NtfModifyTexture(hit.textureCoord);
				TexMixUtil.NtfModifyTexture(hit.point);
				TexMixUtil.EndCMD();
			}
		}
		return true;
	}
	
	// Mouse Move Event
	bool OnMouseMoveEvent()
	{
		Event evn = Event.current;
		if(!evn.isMouse || (evn.type != EventType.MouseMove))
			return false;
		if(mLastClk == evn.mousePosition) return true;
		mLastClk = evn.mousePosition;
		
		
		if(!IsEditEnable()) return true;
		
		//if(!evn.shift) return true;
		//if(!evn.control) return true;
		
		if(!evn.control)
		{
			TexMixUtil.EndCMD();
		}
		
		RaycastHit hit;
		if(EditorUtil.OnHitTest(out hit))
		{
			mEditor.mDrawPos = hit.point;
			
			if(evn.control)
			{
				TexMixUtil.BeginCMD();
				//TexMixUtil.NtfModifyTexture(hit.textureCoord);
				TexMixUtil.NtfModifyTexture(hit.point);
			}
			//EditorUtil.NtfLogHitMsg("MousMv", hit);
			
		}
		return true;
	}
	
	// Key Down Event
	bool OnKeyDownEvent()
	{
		Event evn = Event.current;
		if(!evn.isKey || (evn.type != EventType.KeyDown))
			return false;

		switch(evn.keyCode)
		{
			case KeyCode.P:	// 鏆傚仠/缁х画
			{
				if(mCurOp > TEOM_CLOSE)
				{
					if(IsEditPause()) mCurOp = TEOM_ENABLE;
					else mCurOp = TEOM_PAUSE;
				}
				break;
			}
			case KeyCode.I:	// 鏆傚仠/缁х画
			{
				if(mSelObj != null)
				{
					//mSelObj.transform.localScale += new Vector3(1.0f, 1.0f, 1.0f);
					//mSelObj.transform. += 1.0f;// += Vector3(1.0, 1.0, 1.0);
					//mSelObj.transform.LocalScaleY(1.0f);
					//mSelObj.transform.localScale.z += 1.0f;
				}
				break;
			}
		}
		
		return false;
	}
	
	bool OnKeyUpEvent()
	{
		Event evn = Event.current;
		if(!evn.isKey || (evn.type != EventType.KeyDown))
			return false;
		return false;
	}
}
