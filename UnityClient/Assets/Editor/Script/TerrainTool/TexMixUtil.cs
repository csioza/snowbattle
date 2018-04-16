
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class TexMixUtil
{
	static TexMixUndoMng	mUndoMng = new TexMixUndoMng();
	static Texture2D		mTexBrsh;
	static Texture2D		mTexObj;
	static GameObject		mTarget;
	static string			mTexName;
	static string			mTexFile;
	
	static public bool		mbErase = false;	// 鎿﹂櫎
	static public int		mMixLvl = 0;		// 鏄剧ず灞傛帶鍒电RGBA)
	static public float		mfValue = 0.01f;	// Force[0.0, 1.0]
	
	//TextureImporter		mTexImp; // http://docs.unity3d.com/Documentation/ScriptReference/TextureImporter.html
		
	static public GameObject GetTarget(){return mTarget;}
	static public Texture2D GetTexObj(){return mTexObj;}
	static public string GetTexName(){return mTexName;}
	static public string GetTexFile(){return mTexFile;}
	static bool IsValid(){return (mTexObj != null);}
	static bool IsSick(){return (mTexObj == null);}
	
	// Undo/Redo
	static public void BeginCMD()
	{
		mUndoMng.NtfBeginCMD();
	}
	static public void EndCMD()
	{
		mUndoMng.NtfEndCMD();
	}
	
	static public void NtfExcuteUndo()
	{
		if(IsSick()) return;
		if(mUndoMng.ExcuteUndo(mTexObj))
		{
			mTexObj.Apply();
			mTarget.GetComponent<Renderer>().material.SetTexture("_TexMix", mTexObj);
		}
	}
	
	static public void NtfExcuteRedo()
	{
		if(IsSick()) return;
		if(mUndoMng.ExcuteRedo(mTexObj))
		{
			mTexObj.Apply();
			mTarget.GetComponent<Renderer>().material.SetTexture("_TexMix", mTexObj);
		}
	}
	
	static public void NtfExcuteReset()
	{
		mUndoMng.NtfResetCmds();
	}
	
	static public bool NtfAttachTarget(GameObject obj)
	{
		mTexName= "";
		mTexFile= "";
		mTarget = null;
		mTexObj = null;
		if(obj)
		{
			if(obj.GetComponent("Renderer") == null)
			{
				Debug.Log("Attach Target Failed(Lost Renderer): Target = " + obj.name);
				return false;
			}
			

			Texture texMix  = obj.GetComponent<Renderer>().material.GetTexture("_TexMix");
			if(texMix != null)
			{
				string szImg = AssetDatabase.GetAssetPath(texMix);	
				mTexFile = szImg;
				string szfile = szImg.ToLower();
				string szBase = "resources/";
				int nRes = szfile.IndexOf(szBase);
				if(nRes >= 0)
				{
					szfile = szfile.Substring(nRes + szBase.Length);
				}
				Debug.Log("File = " + szfile);
				//szfile = szfile.Replace("assets/resources/", "");
				int nIdx = szfile.LastIndexOf(".");
				if(nIdx > 0)
				{
					szfile = szfile.Substring(0, nIdx);
				}
				
				Object objT = Resources.Load(szfile);
				if(objT == null)
				{
					Debug.Log("Attach Target Failed(Load Image failed): file = " + szImg);
					return false;
				}
				
				mTarget = obj;
				mTexObj = (objT as Texture2D);
				mTexName= texMix.name;
				Debug.Log("Attach Target OK: Target = " + obj.name);
				return true;
				
			}
		}
		Debug.Log("Attach Target Failed: Target = NULL");
		return false;
	}
	
	static public bool NtfModifyTexture(Vector3 hitPos)
	{
		if(mTexObj == null) return false;
		
		if(!TexMixBrush.IsBrushOK()) return false;
		
		Vector3 vs = mTarget.GetComponent<Collider>().bounds.size;
		float fu = (vs.x * 0.5f + hitPos.x)/vs.x;
		float fv = 1.0f - (vs.z * 0.5f + hitPos.z)/vs.z;
		
		
		//float fu = hitPos.x/vs.x;
		//float fv = 1.0f - hitPos.z/vs.z;
		
		Vector2 uv = new Vector2(fu, fv);
	//	Debug.Log("UV = " + uv.ToString());
		return NtfModifyTexture(uv);
	}
	
	// 淇?敼娣峰悎姣斾緥鍥诫 杈撳叆鐐瑰嚮鐐箄v, 涔嬪悗鏍规嵁閰嶇疆澶勭悊
	static public bool NtfModifyTexture(Vector2 uv)
	{
		if(mTexObj == null) return false;
		
		if(!TexMixBrush.IsBrushOK()) return false;
		Texture2D brushTex = TexMixBrush.GetTexObj();
		
		int mBrushW = brushTex.width/2;
		int mBrushH = brushTex.height/2;
		
		int cx = (int)(uv.x * mTexObj.width);
		int cy = (int)(uv.y * mTexObj.height);
		
		int xMin = Mathf.Max(0, cx - mBrushW);
		int yMin = Mathf.Max(0, cy - mBrushH);
				
		int xMax = Mathf.Min(mTexObj.width - 1,  cx + mBrushW);
		int yMax = Mathf.Min(mTexObj.height - 1, cy + mBrushH);
		
		int myW = xMax - xMin;
		int myH = yMax - yMin;
		float fVal = mbErase ? -mfValue : mfValue;
		
		Color[] cold = mTexObj.GetPixels(xMin, yMin, myW, myH, 0);
		
		if(cold.Length <= 0) return false;

		TexMixCMD cmd = new TexMixCMD();
		cmd.mMinX  = xMin;
		cmd.mMinY  = yMin;
		cmd.mRctW  = myW;
		cmd.mRctH  = myH;
		cmd.mBack  = new Color[cold.Length];
		cmd.mNext  = new Color[cold.Length];

		Color[] cols = TexMixBrush.GetColors();
		for(int i = 0; i < cold.Length; ++i)
		{	
			cmd.mBack[i].r = cold[i].r;
			cmd.mBack[i].g = cold[i].g;
			cmd.mBack[i].b = cold[i].b;
			cmd.mBack[i].a = cold[i].a;
			//Debug.Log(cmd.mBack[i]);
			float fval = cold[i][mMixLvl] + cols[i].a * fVal;
			cold[i][mMixLvl] = Mathf.Clamp(fval, 0.0f, 1.0f);

			cmd.mNext[i].r = cold[i].r;
			cmd.mNext[i].g = cold[i].g;
			cmd.mNext[i].b = cold[i].b;
			cmd.mNext[i].a = cold[i].a;
		}
		
		mUndoMng.NtfAddUndoCmd(cmd);

		mTexObj.SetPixels(xMin, yMin, myW, myH, cold, 0);
		mTexObj.Apply();
		mTarget.GetComponent<Renderer>().material.SetTexture("_TexMix", mTexObj);
		return true;
		
	}
	
	static public bool NtfClearCurLvl(int nLvl)
	{
		if(mTexObj == null) return false;
		
				
		int xMin = 0;
		int yMin = 0;
		
		int myW = mTexObj.width;
		int myH = mTexObj.height;
				
		Color[] cold = mTexObj.GetPixels(xMin, yMin, myW, myH, 0);
		
		if(cold.Length <= 0) return false;
		/*
		TexMixCMD cmd = new TexMixCMD();
		cmd.mMinX  = xMin;
		cmd.mMinY  = yMin;
		cmd.mRctW  = myW;
		cmd.mRctH  = myH;
		cmd.mBack  = new Color[cold.Length];
		cmd.mNext  = new Color[cold.Length];
		 */
		for(int i = 0; i < cold.Length; ++i)
		{
			/*
			cmd.mBack[i].r = cold[i].r;
			cmd.mBack[i].g = cold[i].g;
			cmd.mBack[i].b = cold[i].b;
			cmd.mBack[i].a = cold[i].a;
			 */
			cold[i][mMixLvl] = 0.0f;
			/*
			cmd.mNext[i].r = cold[i].r;
			cmd.mNext[i].g = cold[i].g;
			cmd.mNext[i].b = cold[i].b;
			cmd.mNext[i].a = cold[i].a;
			*/
		}
		
		//mUndoMng.NtfAddUndoCmd(cmd);

		mTexObj.SetPixels(xMin, yMin, myW, myH, cold, 0);
		mTexObj.Apply();
		mTarget.GetComponent<Renderer>().material.SetTexture("_TexMix", mTexObj);
		return true;
	}
	
	
	static public bool NtfSaveTexture(string file)
	{
#if UNITY_STANDALONE_WIN	
		if(file.Length != 0)
		{
			Texture2D newTexture = mTexObj;
			// Convert the texture to a format compatible with EncodeToPNG 
			if(mTexObj.format != TextureFormat.ARGB32 && mTexObj.format != TextureFormat.RGB24)
			{
				newTexture = new Texture2D(mTexObj.width, mTexObj.height);
				newTexture.SetPixels(mTexObj.GetPixels(0),0);
			}
			var pngData = newTexture.EncodeToPNG();
			if(pngData != null)
			{
				File.WriteAllBytes(file, pngData);
				// As we are saving to the asset folder, tell Unity to scan for modified or new assets
				AssetDatabase.Refresh();
				
				Debug.Log("Save Texture: file = " + file);
				return true;
			}
		}
#endif
		return false;
	}
}
