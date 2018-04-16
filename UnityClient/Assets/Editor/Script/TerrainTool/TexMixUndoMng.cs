using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TexMixUndoMng
{
	TexMixCMDGrp			mCurGrp;
	List<TexMixCMDGrp>		mUndoCmd;
	List<TexMixCMDGrp>		mRedoCmd;
	
	bool CanUndo(){return (mUndoCmd.Count > 0);}
	bool CanRedo(){return (mRedoCmd.Count > 0);}
	
	public TexMixUndoMng()
	{
		mCurGrp  = null;
		mUndoCmd = new List<TexMixCMDGrp>();
		mRedoCmd = new List<TexMixCMDGrp>();
	}
	
	public void NtfResetCmds()
	{
		mCurGrp = null;
		mUndoCmd.Clear();
		mRedoCmd.Clear();
	}
	
	public void NtfBeginCMD()
	{
		if(mCurGrp == null)
			mCurGrp = new TexMixCMDGrp();
	}
	
	public void NtfEndCMD()
	{
		if(mCurGrp == null) return;
		if(mUndoCmd.Count >= 100)
		{
			mUndoCmd.RemoveAt(0);
		}

		if(mCurGrp.HasCmds())
		{
			AddUndoCmdGrp(mCurGrp);
			mCurGrp = null;
		}
	}
	
	public void NtfAddUndoCmd(TexMixCMD cmd)
	{
		if(mCurGrp != null)
		{
			mCurGrp.NtfAddCmd(cmd);
		}
	}
	
	void AddUndoCmdGrp(TexMixCMDGrp grp)
	{
		if(mUndoCmd.Count >= 100)
		{
			mUndoCmd.RemoveAt(0);
		}
		mUndoCmd.Add(grp);
	}
	
	void AddRedoCmdGrp(TexMixCMDGrp grp)
	{
		if(mRedoCmd.Count >= 50)
		{
			mRedoCmd.RemoveAt(0);
		}
		mRedoCmd.Add(grp);
	}
	
	public bool ExcuteUndo(Texture2D tex)
	{
		if(CanUndo())
		{
			int npos = mUndoCmd.Count - 1;
			TexMixCMDGrp grp = mUndoCmd[npos];
			bool bVal = grp.ApplyCMDs(tex, true);
			AddRedoCmdGrp(grp);
			mUndoCmd.RemoveAt(npos);
			return bVal;
		}
		return false;
	}
	
	public bool ExcuteRedo(Texture2D tex)
	{
		if(CanRedo())
		{
			int npos = mRedoCmd.Count - 1;
			TexMixCMDGrp grp = mRedoCmd[npos];
			bool bVal = grp.ApplyCMDs(tex, false);
			AddUndoCmdGrp(grp);
			mRedoCmd.RemoveAt(npos);
			return bVal;
		}
		return false;
	}
}
