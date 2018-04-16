using UnityEngine;
using System.Collections;

// http://answers.unity3d.com/questions/175692/getpixelsetpixels-or-stencil-eraser-brush.html


public class TexMixBrush
{
	static Color		mColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);		// Brush Color
	static Vector3		mSize  = new Vector3(1.0f, 0.0f, 1.0f);		// Brush Color
	static Texture2D	mTexObj;
	
	static public bool IsBrushOK(){return (mTexObj != null);}
	
	static public Texture2D GetTexObj(){return mTexObj;}
	
	static public Color[] GetColors() // 外部需确定 mTexObj 有效
	{
		return mTexObj.GetPixels(0, 0, mTexObj.width, mTexObj.height, 0);
	}
	
	
	static public bool NtfCreateBrush(string szfile)
	{
		mTexObj = null;
		//string szfile = "Brushes/brush_14";
		Object objT = Resources.Load(szfile);
		//Debug.Log("mTexObj0 = " + objT.ToString());
		if(objT)
		{
			mTexObj = (objT as Texture2D);
			mSize.x = mTexObj.width  * 3.0f/64.0f;
			mSize.z = mTexObj.height * 3.0f/64.0f;
			Debug.Log("w = " + mTexObj.width.ToString() + ", h = " + mTexObj.height.ToString());
			/*
			Color[] col0 = mTexObj.GetPixels(0, 0, mTexObj.width, mTexObj.height, 0);
		
			for(int i = 0; i < col0.Length; ++i)
			{
				Color clr = col0[i];
				if((clr.r > 0.0f) || (clr.g > 0.0f) || (clr.b > 0.0f) || (clr.a > 0.0f))
					Debug.Log("Col[" + i + "] = " + col0[i].ToString());
			}
			*/
		}
		//Debug.Log("mTexObj = " + mTexObj.ToString());
		return (mTexObj != null);
	}
	
	static public void NtfDrawBrush(Vector3 vPos)
	{
		Gizmos.color= mColor;
		Gizmos.DrawWireCube(vPos, mSize);
	}
	
}
