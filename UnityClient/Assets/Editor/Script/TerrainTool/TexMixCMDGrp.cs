
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TexMixCMDGrp
{
	List<TexMixCMD>		mCmds;
	
	public bool HasCmds(){return (mCmds.Count > 0);}
	public int GetCount(){return mCmds.Count;}
	
	public TexMixCMDGrp()
	{
		mCmds = new List<TexMixCMD>();
	}
	
	public void NtfClearCmds()
	{
		mCmds.Clear();
	}
	
	public void NtfAddCmd(TexMixCMD cmd)
	{
		mCmds.Add(cmd);
	}
		
	public bool ApplyCMDs(Texture2D tex, bool bUndo)
	{
		if(HasCmds())
		{
			if(bUndo)
			{
				int npos = mCmds.Count - 1;
				for (int i = npos; i >= 0; --i)
				{
					TexMixCMD cmd = mCmds[i];
					tex.SetPixels(cmd.mMinX, cmd.mMinY, cmd.mRctW, cmd.mRctH, cmd.mBack, 0);
				}
			}
			else
			{
				for (int i = 0; i < mCmds.Count; ++i)
				{
					TexMixCMD cmd = mCmds[i];
					tex.SetPixels(cmd.mMinX, cmd.mMinY, cmd.mRctW, cmd.mRctH, cmd.mNext, 0);
				}
			}
			return true;
		}
		return false;
	}
	
}
