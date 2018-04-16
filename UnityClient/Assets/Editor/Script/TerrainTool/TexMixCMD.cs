using UnityEngine;
using System.Collections;

// Undo/Redo Command


public class TexMixCMD
{
	public int			mMinX;
	public int			mMinY;
	public int			mRctW;
	public int			mRctH;
	public Color [] 	mBack;	// Undo Data
	public Color [] 	mNext;	// Redo Data

}
