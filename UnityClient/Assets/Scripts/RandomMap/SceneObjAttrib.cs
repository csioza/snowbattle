using UnityEngine;
using System.Collections;

public class SceneObjAttrib : MonoBehaviour 
{
	private bool GrassShow;
	private GameObject OutObj;
	private GameObject OutminiObj;
	private GroundCheck GroundObjCheck;

	public void Awake()
	{
		GrassShow=false;

		for(int i = 0;i<gameObject.transform.childCount;i++)
		{
			GameObject SubObj= gameObject.transform.GetChild(i).gameObject;
			
			if(SubObj.name.Contains("grass"))
			{
				gameObject.transform.GetChild(i).gameObject.AddComponent<GrassAnimation>().SetGrassType(true);
				gameObject.transform.GetChild(i).gameObject.SetActive(false);
			}
			if(SubObj.name.Contains("ground"))
			{
				GroundObjCheck=SubObj.AddComponent<GroundCheck>();
			}
			else if(SubObj.name.Contains("flare"))
			{
				gameObject.transform.GetChild(i).gameObject.AddComponent<GrassAnimation>().SetGrassType(false);
				gameObject.transform.GetChild(i).gameObject.SetActive(false);
			}
			else if(SubObj.name.Contains("npc"))
			{
				//SceneEffect.Ins.AddNpcPos(gameObject.transform.GetChild(i).position);
				gameObject.transform.GetChild(i).gameObject.SetActive(false);
			}
			else if(SubObj.name.Contains("door"))
			{
				string DoorID=null;
				DoorID+=SubObj.name[4];
				
				string TagDoorID=null;
				TagDoorID+=SubObj.name[5];

				//SceneEffect.Ins.AddDoorPos(int.Parse(DoorID),int.Parse(TagDoorID),gameObject.transform.GetChild(i).transform.position,gameObject.transform.GetChild(i).up);
				gameObject.transform.GetChild(i).gameObject.SetActive(false);
			}
			else if(SubObj.name.Contains("aniuv"))
			{
				string Num=null;
				Num+=SubObj.name[5];
				float VNum=float.Parse(Num)*0.1f;
				
				Num=null;
				Num+=SubObj.name[6];
				float HNum=float.Parse(Num)*0.1f;
				
				gameObject.transform.GetChild(i).gameObject.AddComponent<UVAnimation>().SetUVType(HNum,VNum);
			}
			else if(SubObj.name.Contains("collision"))
			{
				//SceneEffect.Ins.GetCollisionFace(SubObj);
				gameObject.transform.GetChild(i).gameObject.SetActive(false);
			}
			else if(SubObj.name.Contains("node"))
			{
				//SceneEffect.Ins.GetCollisionFace(SubObj);
				gameObject.transform.GetChild(i).gameObject.SetActive(false);
			}
			else if(SubObj.name.Contains("out"))
			{
				if(!SubObj.name.Contains("outmini"))
				{
				  OutObj=SubObj;
				  gameObject.transform.GetChild(i).gameObject.SetActive(false);
				}
				else
				{
				  OutminiObj=SubObj;
				  gameObject.transform.GetChild(i).gameObject.SetActive(false);
				}
			}
		}
	}

	public GameObject GetOutObj()
	{
		if(OutObj!=null)
		{
		  return OutObj;
		}

		return gameObject;
	}

	public GameObject GetOutMiniObj()
	{
		return OutminiObj;
	}

	void ShowGrassObj(bool show)
	{
		GrassShow=show;

		GameObject SubObj;

		for(int i=0;i< gameObject.transform.childCount;i++)
		{
			SubObj=gameObject.transform.GetChild(i).gameObject;
			if(SubObj.name.Contains("grass")||SubObj.name.Contains("flare"))
			{
				SubObj.SetActive(GrassShow);
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
	  if(GroundObjCheck!=null)
	  {
		if(GroundObjCheck.Show==true)
		{
	      if(GrassShow==false)
		  {
			ShowGrassObj(true);
		  }
		}
		else
		{
		  if(GrassShow==true)
		  {
			ShowGrassObj(false);
		  }
		}
	  }
	}
}
