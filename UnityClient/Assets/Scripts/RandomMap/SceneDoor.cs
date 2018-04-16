using UnityEngine;
using System.Collections;
using System;

public class SceneDoor  
{
	private int DoorID=0;
	private int TagDoorID=0;

	private Vector3 DoorPos;
	private Vector3 Forward;

	// Use this for initialization
	public SceneDoor(int DOORID,int TAGDOORID,Vector3 DOORPOS,Vector3 FORWORD) 
	{
		DoorID=DOORID;
		TagDoorID=TAGDOORID;
		DoorPos=new Vector3(DOORPOS.x,DOORPOS.y,DOORPOS.z);
		Forward=new Vector3(FORWORD.x,0,FORWORD.z);
	}

	public int RunToDoor()
	{
		if(DoorID==TagDoorID)
		{
		  return -1;
		}

		if(Vector3.Distance(SceneEffect.Ins.MainPlayer.transform.position,DoorPos)<1.5f)
		{
			return TagDoorID;
		}

		return -1;
	}

	public void RuntoTag(int TAGDOORID)
	{
		if(TAGDOORID>=0)
		{
		  if(DoorID==TAGDOORID)
		  {
			 SceneEffect.Ins.MainPlayer.transform.position=DoorPos-Forward*2.5f;
		  }
		}
	}
}
