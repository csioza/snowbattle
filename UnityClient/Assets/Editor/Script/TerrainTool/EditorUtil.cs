

using UnityEngine;
using UnityEditor;
using SysIO = System.IO;
using System.IO;
using System.Collections;

public class EditorUtil
{
	//---------------------------------------------------------------------------------------
	// Hit Test
	static public bool OnHitTest(out RaycastHit hit)
	{
		Vector2 vPos = Event.current.mousePosition;
		Vector3 val = new Vector3(vPos.x, Camera.current.pixelHeight - vPos.y);
		Ray ry = Camera.current.ScreenPointToRay(val);
		if(Physics.Raycast(ry, out hit, 100))
		{
			return true;
		}
		return false;
	}
	
	static public void NtfLogHitMsg(string szNm, RaycastHit hit)
	{
		string hitNm = szNm + ": " + hit.collider.gameObject.name;
		hitNm += ", hitTri = " + hit.triangleIndex.ToString();
		hitNm += ", hitPos = " + hit.point.ToString();
		hitNm += ", hitCrd = " + hit.textureCoord.ToString();
		Debug.Log(hitNm);
	}
	
	static public GameObject GetHitTarget()
	{
		RaycastHit hit;
		Vector2 vPos = Event.current.mousePosition;
		Vector3 val  = new Vector3(vPos.x, Camera.current.pixelHeight - vPos.y);
		Ray ry = Camera.current.ScreenPointToRay(val);
		if(Physics.Raycast(ry, out hit, 100))
		{
			return ToEditTarget(hit.collider.gameObject);
		}
		return null;
	}
	
	static public GameObject ToEditTarget(Object obj)
	{
		GameObject objSel = obj as GameObject;
		if(objSel == null)
		{
			Debug.LogWarning("Object Name: " + obj.name);
			Debug.LogWarning("Unknown Object: " + obj.GetType().ToString());
			return null;
		}
		
		if(objSel.GetComponent("Renderer") == null)
		{
			Debug.Log("Lost Renderer: name = " + objSel.name);
			return null;
		}
		
		string shaderf  = objSel.GetComponent<Renderer>().sharedMaterial.shader.name;
		if((shaderf == "Custom/TexBlen") || (shaderf == "TexMix/Tex4Mix"))
		{
			return objSel;
			
		}
		Debug.Log("Uncomfortable Sel: name = " + objSel.name + ", Shader = " + shaderf);
		return null;
	}
	
	//---------------------------------------------------------------------------------------------
	//
	// 缂栬緫瀵硅薄
	//
	static public GameObject GetEditTarget()
	{
		return ToEditTarget(Selection.activeObject);
	}
	
	
	static public void NtfLogEditTarget(GameObject objSel)
	{
		if(objSel == null)
		{
			Debug.LogWarning("Target Object: Obj = NULL");
			return;
		}
		
		string name = objSel.name;
		if(objSel.GetComponent("Renderer") == null)
		{
			Debug.Log("Lost Renderer: name = " + name);
			return;
		}
		
		Material mtrObj = objSel.GetComponent<Renderer>().material;
		string shaderf  = mtrObj.shader.name;
		Texture texLvl0 = mtrObj.GetTexture("_MainTex");
		Texture texLvl1 = mtrObj.GetTexture("_TexLvl1");
		Texture texLvl2 = mtrObj.GetTexture("_TexLvl2");
		Texture texLvl3 = mtrObj.GetTexture("_TexLvl3");
		Texture texMixd = mtrObj.GetTexture("_TexMix");
		
		Debug.Log("name = " + name + ", Shader = " + shaderf);
		NtfLogTexture("_MainTex", texLvl0);
		NtfLogTexture("_TexLvl1", texLvl1);
		NtfLogTexture("_TexLvl2", texLvl2);
		NtfLogTexture("_TexLvl3", texLvl3);
		NtfLogTexture("_TexMix" , texMixd);
	}
	
	// 杈撳嚭璋冭瘯鍐呭?
	static void NtfLogTexture(string szNm, Texture txObj)
	{
		if(txObj == null)
		{
			Debug.LogWarning("TexName = " + szNm + ", txObj = NULL");
			return;
		}
		string szMsg = "TexName = " + szNm + " ,";
		szMsg = szMsg + "TexSize = (" + txObj.width + ", " + txObj.height + ") ,";
		szMsg = szMsg + "TexFile = " + txObj.name;
		
		Debug.Log(szMsg);
		//Debug.Log("");
		
	}
	
	//---------------------------------------------------------------------------------------------
	//
	// 鎿嶄綔鐨勬贩鍚堟瘮渚嬪浘
	//
	static public Texture2D GetMixTexture(GameObject obj)
	{
		if(obj == null) return null;
		if(obj.GetComponent("Renderer") == null)
		{
			Debug.Log("Lost Renderer: name = " + obj.name);
			return null;
		}
		
		Material mtrObj = obj.GetComponent<Renderer>().material;
		Texture texMix  = mtrObj.GetTexture("_TexMix");
		if(texMix != null)
		{
			string szImg = AssetDatabase.GetAssetPath(texMix);			
			string szfile = szImg.ToLower();			
			szfile = szfile.Replace("assets/resources/", "");
			int nIdx = szfile.LastIndexOf(".");
			if(nIdx > 0)
			{
				szfile = szfile.Substring(0, nIdx);
			}
			
			Object objT = Resources.Load(szfile);
			if(objT == null)
			{
				Debug.Log("LoadImage failed: file = " + szImg);
				return null;
			}
			//Texture2D mixTex0 = (Texture2D)objT;
			//string szMsgA = "TexSizeB = (" + mixTex0.width + ", " + mixTex0.height + ")";
			//Debug.Log(szMsgA);
			return (objT as Texture2D);
			
		}
		return null;
	}
	
	// http://www.cocoachina.com/bbs/simple/?t91141.html
	// http://answers.unity3d.com/questions/253442/what-is-the-best-way-to-save-a-modified-texture-to.html
	static public Texture2D LoadTextureFromFile(string filename)
	{
		Texture2D texture = new Texture2D(2048, 2048);
		FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
		byte[] imageData = new byte[fs.Length];
		fs.Read(imageData, 0, (int)fs.Length);
		texture.LoadImage(imageData);
		return texture;
	}
}
