using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using PF;

[Serializable]  
public class ColorAniMaterial
{
	public float AddSpeed;         //time cy speed
	public float AddRange;         //vertex light power range
	public float BaseLight;        //vertex base light power
	public Material[] LightMat;    //push material in this array what u want to change

	public ColorAniMaterial()
	{
		AddSpeed=2.0f;
		AddRange=0.1f;
		BaseLight=1.0f;
	}
}

[Serializable]  
public class RandomSceneObj
{
	public int RoofNum;            //how many Roofs in this scene
	public GameObject   RoofFrist; //First roof object
	public GameObject   RoofEnd;   //End roof object

	public GameObject[] Roof;      //roof object array
	public GameObject[] Turnnel;   //turnnel object array

	public GameObject[] RoofMini;  //Sub roof object array
}

[Serializable]  
public class GameCharactorPos
{
   public Vector3 StartPos;
	
   public Vector3 EndPos;
	
   public Vector3[] NpcPosArray;	
}

public class SceneOneRoomConfig
{
    public int m_id;
    //!定位用的对象
    public GameObject m_object;
    //!实际使用的对象
    public GameObject m_modelObject;
    public GameObject[] RoofMini;  //Sub roof object array
    public GameObject[] ChildObj;
}

public class SceneEffect : MonoBehaviour 
{
	public static SceneEffect Ins;

	public  bool GenNewScene;
	
	public  GameCharactorPos SceneNpcPos;
	
	public  ColorAniMaterial[] ColorMat;

	public  RandomSceneObj SceneObj;
	
	public  Material CollisionMat;

	public  GameObject MainPlayer;

	private  int TotalCollisionWallFaceCount;

	private  int TotalCollisionGroundFaceCount;
	
	private  GameObject CollisionWall;
	private  GameObject CollisionGround;
	
	private RandomSceneObj SceneObjIns;

	private Vector3[] WallVertices;
	private Vector3[] WallNormals;
	private int[] WallTriangles;

	private Vector3[] GroundVertices;
	private Vector3[] GroundNormals;
	private int[] GroundTriangles;
	private int MaxTempVertex;

	private int MaxNpcPosCount;
	
	private int CurrentNpcPosCount;

	public  Vector3[] NpcPos;

	private ArrayList SDoor;

    private PF.Pathfinder PathFind;
	public int TestCount; 
	void Awake()
	{
		Ins=this;

		GenNewScene=false;

        PathFind = new PF.Pathfinder();

		SceneNpcPos = new GameCharactorPos();
		SDoor       = new ArrayList();

		RenderSettings.fog=true;
		RenderSettings.fogMode=FogMode.ExponentialSquared;
		RenderSettings.fogColor=Camera.main.backgroundColor;
		RenderSettings.fogDensity=0.06f;
	}

    public PF.Pathfinder GetPathFind()
	{
		return PathFind;
	}
    public void Hide(bool isHide)
    {
        SceneObjIns.RoofFrist.SetActive(!isHide);
        SceneObjIns.RoofEnd.SetActive(!isHide);

        int currentId = 0;
        for (int i = 0; i < SceneObj.RoofNum; i++)
        {
            if (SceneObjIns.Turnnel[currentId] != null)
            {
                SceneObjIns.Turnnel[currentId].SetActive(!isHide);
            }
            if (SceneObjIns.Roof[currentId] != null)
            {
                SceneObjIns.Roof[currentId].SetActive(!isHide);
            }
            if (SceneObjIns.RoofMini[currentId] != null)
            {
                SceneObjIns.RoofMini[currentId].SetActive(!isHide);
            }
            currentId += 1;
        }
    }
    public void InitMap(List<SceneOneRoomConfig> roofCfgs)
    {
        SceneObj.RoofNum = roofCfgs.Count - 2;
        if (SceneObj.RoofNum == 0) { return; }

        CleanSceneData();

        MaxTempVertex = SceneObj.RoofNum * 400 * 3;//per Roof+Turnnel Max Collision Face Less 400

        MaxNpcPosCount = SceneObj.RoofNum * 10;//per Roof+Turnnel Max Npc Less 10
        ////////////create Npc Pos Struct //////////////////////////////////////////
        ////////////create Npc Pos Struct ////////////////////////////////////////// 
        CurrentNpcPosCount = 0;

        NpcPos = new Vector3[MaxNpcPosCount];
        ////////////End create Npc Pos Struct ///////////////////////////////////////
        ////////////End create Npc Pos Struct ///////////////////////////////////////

        ////////////create Collison Struct //////////////////////////////////////////
        ////////////create Collison Struct ////////////////////////////////////////// 
        TotalCollisionWallFaceCount = 0;
        TotalCollisionGroundFaceCount = 0;

        WallVertices = new Vector3[MaxTempVertex];
        WallNormals = new Vector3[MaxTempVertex];
        WallTriangles = new int[MaxTempVertex];

        GroundVertices = new Vector3[MaxTempVertex];
        GroundNormals = new Vector3[MaxTempVertex];
        GroundTriangles = new int[MaxTempVertex];
        ////////////End create Collison Struct ///////////////////////////////////////
        ////////////End create Collison Struct ///////////////////////////////////////

        SceneObjIns = new RandomSceneObj();
        SceneObjIns.RoofNum = SceneObj.RoofNum;

        SceneObjIns.Roof = new GameObject[SceneObjIns.RoofNum];
        SceneObjIns.Turnnel = new GameObject[SceneObjIns.RoofNum + 1];
        SceneObjIns.RoofMini = new GameObject[SceneObjIns.RoofNum];

        if (SceneObj.RoofFrist != null)
        {
            SceneObjIns.RoofFrist = GameObject.Instantiate(SceneObj.RoofFrist) as GameObject;
            SceneObjIns.RoofFrist.GetComponent<Renderer>().enabled = false;

            roofCfgs[0].m_modelObject = SceneObjIns.RoofFrist;
            roofCfgs[0].m_object = new GameObject("PositionDigg");
            roofCfgs[0].m_object.transform.parent = SceneObjIns.RoofFrist.transform;
            SceneObjIns.RoofFrist.transform.position = new Vector3(0, 0, 0);

            int currentId = 0;

            SceneObjIns.RoofFrist.AddComponent<SceneObjAttrib>();

            SceneNpcPos.StartPos = SceneObjIns.RoofFrist.transform.position;

            GameObject OutObj = SceneObjIns.RoofFrist.transform.Find("out").gameObject;
            OutObj.SetActive(false);

            for (int i = 0; i < SceneObj.RoofNum; i++)
            {
                OutObj = GenMidRoof(currentId, OutObj.transform.position, OutObj.transform.rotation, false, roofCfgs[i + 1]);
                currentId += 1;
            }

            GenMidRoof(currentId, OutObj.transform.position, OutObj.transform.rotation, true, roofCfgs[SceneObj.RoofNum + 1]);
        }

        CollisionWall = new GameObject();
        CollisionGround = new GameObject();

        MeshCollider cc = CreateCollisionFace(CollisionWall, "CollisionWall", TotalCollisionWallFaceCount, WallVertices, WallNormals, WallTriangles);
        cc.isTrigger = false;
        cc.gameObject.isStatic = true;
        cc.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	cc.enabled = false;
        cc = CreateCollisionFace(CollisionGround, "CollisionGround", TotalCollisionGroundFaceCount, GroundVertices, GroundNormals, GroundTriangles);
        cc.isTrigger = false;
        cc.gameObject.isStatic = true;
        cc.tag = "Terrain";
        cc.enabled = false;

        //PathFind.init(CollisionGround.GetComponent<MeshCollider>());

        CreateSceneNpcPos();
    }
    public void InitSceneObject()
	{
		if(SceneObj.RoofNum==0){return;}
		
		CleanSceneData();
		
		MaxTempVertex=SceneObj.RoofNum*400*3;//per Roof+Turnnel Max Collision Face Less 400
		
		MaxNpcPosCount=SceneObj.RoofNum*10;//per Roof+Turnnel Max Npc Less 10
		////////////create Npc Pos Struct //////////////////////////////////////////
		////////////create Npc Pos Struct ////////////////////////////////////////// 
	    CurrentNpcPosCount=0;
		
	    NpcPos= new Vector3[MaxNpcPosCount];
		////////////End create Npc Pos Struct ///////////////////////////////////////
		////////////End create Npc Pos Struct ///////////////////////////////////////
		
		////////////create Collison Struct //////////////////////////////////////////
		////////////create Collison Struct ////////////////////////////////////////// 
		TotalCollisionWallFaceCount=0;
		TotalCollisionGroundFaceCount=0;
		
		WallVertices  = new Vector3[MaxTempVertex];
		WallNormals   = new Vector3[MaxTempVertex];
		WallTriangles = new int[MaxTempVertex];

		GroundVertices  = new Vector3[MaxTempVertex];
		GroundNormals   = new Vector3[MaxTempVertex];
		GroundTriangles = new int[MaxTempVertex];
		////////////End create Collison Struct ///////////////////////////////////////
		////////////End create Collison Struct ///////////////////////////////////////

		SceneObjIns = new RandomSceneObj();
		SceneObjIns.RoofNum=SceneObj.RoofNum;

		SceneObjIns.Roof =new GameObject[SceneObjIns.RoofNum]; 
		SceneObjIns.Turnnel=new GameObject[SceneObjIns.RoofNum+1];
		SceneObjIns.RoofMini =new GameObject[SceneObjIns.RoofNum];

		if(SceneObj.RoofFrist!=null)
		{
			SceneObjIns.RoofFrist=GameObject.Instantiate(SceneObj.RoofFrist)as GameObject;
			SceneObjIns.RoofFrist.GetComponent<Renderer>().enabled=false;

			SceneObjIns.RoofFrist.transform.position=new Vector3(0,0,0);
			
			int currentId=0;

			SceneObjIns.RoofFrist.AddComponent<SceneObjAttrib>();
			
			SceneNpcPos.StartPos=SceneObjIns.RoofFrist.transform.position;

			GameObject OutObj =SceneObjIns.RoofFrist.transform.Find("out").gameObject;
			OutObj.SetActive(false);

			for(int i=0;i<SceneObj.RoofNum;i++)
			{
				OutObj=GenMidRoof(currentId,OutObj.transform.position,OutObj.transform.rotation,false);
				currentId+=1;
			}

			GenMidRoof(currentId,OutObj.transform.position,OutObj.transform.rotation,true);
		}
		
		CollisionWall =new GameObject();
		CollisionGround =new GameObject();			
		
		CreateCollisionFace(CollisionWall,"CollisionWall",TotalCollisionWallFaceCount,WallVertices,WallNormals,WallTriangles);	
		CreateCollisionFace(CollisionGround,"CollisionGround",TotalCollisionGroundFaceCount,GroundVertices,GroundNormals,GroundTriangles);

		PathFind.init(CollisionGround.GetComponent<MeshCollider>());

		CreateSceneNpcPos();
	}
	
	
	void CleanSceneData()
	{
		MaxTempVertex=0;
		
		TotalCollisionWallFaceCount=0;
		TotalCollisionGroundFaceCount=0;
		
		if(WallVertices!=null){WallVertices=null;}
		if(WallNormals!=null){WallNormals=null;}
		if(WallTriangles!=null){WallTriangles=null;}

		if(GroundVertices!=null){GroundVertices=null;}
		if(GroundNormals!=null){GroundNormals=null;}
		if(GroundTriangles!=null){GroundTriangles=null;}		
		///////////////////////////////////////////////////////////////////////////////
		if(CollisionWall!=null)
		{
		 GameObject.Destroy(CollisionWall);
		}
		
		if(CollisionGround!=null)
		{			
		 GameObject.Destroy(CollisionGround);
		}
		
		if(SceneObjIns!=null)
		{
		 if(SceneObjIns.RoofEnd!=null)
		 {
		  GameObject.Destroy(SceneObjIns.RoofEnd);
		 }
		
		 if(SceneObjIns.RoofFrist!=null)
		 {
          GameObject.Destroy(SceneObjIns.RoofFrist);
		 }
		
		 if(SceneObjIns.Roof!=null)
		 {					
		  for(int i=0;i<SceneObjIns.Roof.Length;i++)
		  {
		   if(SceneObjIns.Roof[i]!=null)
		   {						
		    GameObject.Destroy(SceneObjIns.Roof[i]);
		   }
		  }
		 }
			
		 if(SceneObjIns.Turnnel!=null)
		 {
		  for(int i=0;i<SceneObjIns.Turnnel.Length;i++)
		  {
		   if(SceneObjIns.Turnnel[i]!=null)
		   {						
		     GameObject.Destroy(SceneObjIns.Turnnel[i]);
		   }
		  }
		 }

		 if(SceneObjIns.RoofMini!=null)
		 {
		   for(int i=0;i<SceneObjIns.RoofMini.Length;i++)
		   {
			if(SceneObjIns.RoofMini[i]!=null)
			{						
			 GameObject.Destroy(SceneObjIns.RoofMini[i]);
			}
		   }
		  }

	     SceneObjIns.RoofNum=0;		
		 SceneObjIns=null;			
		}			
		///////////////////////////////////////////////////////////////////////////////
		MaxNpcPosCount=0;
		
	    CurrentNpcPosCount=0;			
		
		if(NpcPos!=null){NpcPos=null;}
		
        if(SceneNpcPos!=null)
		{	
		 if(SceneNpcPos.NpcPosArray!=null){SceneNpcPos.NpcPosArray=null;}
		}

		if(SDoor!=null)
		{	
		   SDoor.Clear();
		}
		///////////////////////////////////////////////////////////////////////////////
		GC.Collect();
	}
	
	void CreateSceneNpcPos()
	{
		if(CurrentNpcPosCount==0){return;}
		
		SceneNpcPos.NpcPosArray=new Vector3[CurrentNpcPosCount];
		
		for(int i=0;i<CurrentNpcPosCount;i++)
		{
	      SceneNpcPos.NpcPosArray[i]=NpcPos[i];
		}
	}

    MeshCollider CreateCollisionFace(GameObject CollObj, string Name, int FaceCount, Vector3[] AVertices, Vector3[] ANormals, int[] ATriangles)
	{
		CollObj.name=Name;
		CollObj.transform.position=new Vector3(0,0,0);
		CollObj.transform.localScale=new Vector3(1,1,1);

		Mesh mesh = CollObj.AddComponent<MeshFilter>().mesh;
		MeshRenderer meshRender = CollObj.AddComponent<MeshRenderer>();

		meshRender.material=CollisionMat;
		meshRender.GetComponent<Renderer>().enabled=false;
			
		mesh.vertices  = new Vector3[FaceCount*3];
		mesh.normals   = new Vector3[FaceCount*3];
		mesh.triangles = new int[FaceCount*3];

		Vector3[] TVertices=mesh.vertices;		
		Vector3[] TNormals=mesh.normals;
		int[] TTriangles=mesh.triangles;

		for(int i=0;i<FaceCount;i++)
		{
		  TVertices[i*3  ]= new Vector3(AVertices[i*3  ].x,AVertices[i*3  ].y,AVertices[i*3  ].z);
		  TVertices[i*3+1]= new Vector3(AVertices[i*3+1].x,AVertices[i*3+1].y,AVertices[i*3+1].z);
		  TVertices[i*3+2]= new Vector3(AVertices[i*3+2].x,AVertices[i*3+2].y,AVertices[i*3+2].z);	
			
		  TNormals[i*3  ]= new Vector3(ANormals[i*3  ].x,ANormals[i*3  ].y,ANormals[i*3  ].z);
		  TNormals[i*3+1]= new Vector3(ANormals[i*3+1].x,ANormals[i*3+1].y,ANormals[i*3+1].z);
		  TNormals[i*3+2]= new Vector3(ANormals[i*3+2].x,ANormals[i*3+2].y,ANormals[i*3+2].z);
			
		  TTriangles[i*3  ]= i*3;
		  TTriangles[i*3+1]= i*3+1;
		  TTriangles[i*3+2]= i*3+2;
		}
		
		mesh.vertices=TVertices;		
		mesh.normals=TNormals;
		mesh.triangles=TTriangles;
		
		mesh.RecalculateBounds();	

		MeshCollider Collison=CollObj.AddComponent<MeshCollider>();		
		Collison.isTrigger=false;
        return Collison;
	}

	public void AddNpcPos(Vector3 Pos)
	{
	  NpcPos[CurrentNpcPosCount]=Pos;
	  CurrentNpcPosCount+=1;
	}

	public void AddDoorPos(int DoorID,int TagDoorID,Vector3 Pos,Vector3 Dir)
	{
		SDoor.Add(new SceneDoor(DoorID,TagDoorID,Pos,Dir));
	}

	public void GetCollisionFace(GameObject GObj)
	{
		Mesh mesh=GObj.GetComponent<MeshFilter>().mesh;
		
		Vector3[] Vertices=mesh.vertices;
		Vector3[] Normals=mesh.normals;
		int[] Triangles=mesh.triangles;

		for(int i=0;i<Triangles.Length;i+=3)
		{
			if(GObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i]]).y>0.9f)
			{
			 GroundVertices[TotalCollisionGroundFaceCount*3  ]=GObj.transform.localToWorldMatrix.MultiplyPoint(Vertices[Triangles[i  ]]);
			 GroundVertices[TotalCollisionGroundFaceCount*3+1]=GObj.transform.localToWorldMatrix.MultiplyPoint(Vertices[Triangles[i+1]]);
			 GroundVertices[TotalCollisionGroundFaceCount*3+2]=GObj.transform.localToWorldMatrix.MultiplyPoint(Vertices[Triangles[i+2]]);

			 GroundNormals[TotalCollisionGroundFaceCount*3  ]=GObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i  ]]);
			 GroundNormals[TotalCollisionGroundFaceCount*3+1]=GObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i+1]]);
			 GroundNormals[TotalCollisionGroundFaceCount*3+2]=GObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i+2]]);

			 GroundTriangles[TotalCollisionGroundFaceCount*3  ]  = TotalCollisionGroundFaceCount*3;
			 GroundTriangles[TotalCollisionGroundFaceCount*3+1]  = TotalCollisionGroundFaceCount*3+1;
			 GroundTriangles[TotalCollisionGroundFaceCount*3+2]  = TotalCollisionGroundFaceCount*3+2;

			 TotalCollisionGroundFaceCount+=1;
			}
			else
			{
			 WallVertices[TotalCollisionWallFaceCount*3  ]=GObj.transform.localToWorldMatrix.MultiplyPoint(Vertices[Triangles[i  ]]);
			 WallVertices[TotalCollisionWallFaceCount*3+1]=GObj.transform.localToWorldMatrix.MultiplyPoint(Vertices[Triangles[i+1]]);
			 WallVertices[TotalCollisionWallFaceCount*3+2]=GObj.transform.localToWorldMatrix.MultiplyPoint(Vertices[Triangles[i+2]]);

			 WallNormals[TotalCollisionWallFaceCount*3  ]=GObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i  ]]);
			 WallNormals[TotalCollisionWallFaceCount*3+1]=GObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i+1]]);
			 WallNormals[TotalCollisionWallFaceCount*3+2]=GObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i+2]]);

			 WallTriangles[TotalCollisionWallFaceCount*3  ]  = TotalCollisionWallFaceCount*3;
			 WallTriangles[TotalCollisionWallFaceCount*3+1]  = TotalCollisionWallFaceCount*3+1;
			 WallTriangles[TotalCollisionWallFaceCount*3+2]  = TotalCollisionWallFaceCount*3+2;

			 TotalCollisionWallFaceCount+=1;
			}
		}
	}

    GameObject GenMidRoof(int currentId, Vector3 PrePos, Quaternion PreRot, bool End, SceneOneRoomConfig cfg=null)
	{
		SceneObjIns.Turnnel[currentId]=GameObject.Instantiate(SceneObj.Turnnel[UnityEngine.Random.Range(0,SceneObj.Turnnel.Length)])as GameObject;
		SceneObjIns.Turnnel[currentId].GetComponent<Renderer>().enabled=false;
		SceneObjIns.Turnnel[currentId].transform.position=PrePos;
		SceneObjIns.Turnnel[currentId].transform.rotation=PreRot;

		SceneObjAttrib SAT=SceneObjIns.Turnnel[currentId].AddComponent<SceneObjAttrib>();
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		if(End==false)
		{
            int roomid = 0;
            if (cfg == null || cfg.m_id < 0 || cfg.m_id >= SceneObj.Roof.Length)
            {
                roomid = UnityEngine.Random.Range(0, SceneObj.Roof.Length);
            }
            else
            {
                roomid = cfg.m_id;
            }
            SceneObjIns.Roof[currentId] = GameObject.Instantiate(SceneObj.Roof[roomid]) as GameObject;
		   SceneObjIns.Roof[currentId].GetComponent<Renderer>().enabled=false;
           if (cfg != null)
           {
               cfg.m_object = new GameObject("PositionDigg");
               cfg.m_object.transform.parent = SceneObjIns.Roof[currentId].transform;
               cfg.m_modelObject = SceneObjIns.Roof[currentId];
           }
		   SceneObjIns.Roof[currentId].transform.position=SAT.GetOutObj().transform.position;
		   SceneObjIns.Roof[currentId].transform.rotation=SAT.GetOutObj().transform.rotation;

		   SceneObjAttrib SA=SceneObjIns.Roof[currentId].AddComponent<SceneObjAttrib>();

		   GameObject RoofMiniObj =SA.GetOutMiniObj();

		   if(RoofMiniObj!=null)
		   {
			  //if(UnityEngine.Random.Range(-0.5f,0.5f)<0.0f)
		      {
                if (cfg.RoofMini != null)
				//if(SceneObj.RoofMini!=null)
				{
                    if (cfg.RoofMini.Length > 0)
				  //if(SceneObj.RoofMini.Length>0)
				  {
                      //GameObject RMini = SceneObj.RoofMini[UnityEngine.Random.Range(0, SceneObj.RoofMini.Length)];
                      GameObject RMini = cfg.RoofMini[0];
                      //GameObject RMini = SceneObj.RoofMini[cfg.m_id];
					if(RMini!=null)
					{
					  SceneObjIns.RoofMini[currentId]=GameObject.Instantiate(RMini)as GameObject;
					  SceneObjIns.RoofMini[currentId].GetComponent<Renderer>().enabled=false;
                      //!add by guopengfei
                      if (cfg != null)
                      {
                          cfg.ChildObj[0] = new GameObject("PositionDiggChild");
                          cfg.ChildObj[0].transform.parent = SceneObjIns.RoofMini[currentId].transform;
                          cfg.m_modelObject = SceneObjIns.RoofMini[currentId];
                      }
					  SceneObjIns.RoofMini[currentId].transform.position=RoofMiniObj.transform.position;
					  SceneObjIns.RoofMini[currentId].transform.rotation=RoofMiniObj.transform.rotation;
                     
					  SceneObjIns.RoofMini[currentId].AddComponent<SceneObjAttrib>();							
					}
				   }
				 }
			   }
		    }

		   return SA.GetOutObj();
		}
		else
		{
			SceneObjIns.RoofEnd=GameObject.Instantiate(SceneObj.RoofEnd)as GameObject;
			SceneObjIns.RoofEnd.GetComponent<Renderer>().enabled=false;
            if (cfg != null)
            {
                cfg.m_object = new GameObject("PositionDigg");
                cfg.m_object.transform.parent = SceneObjIns.RoofEnd.transform;
                cfg.m_modelObject = SceneObjIns.RoofEnd;
            }
			SceneObjIns.RoofEnd.transform.position=SAT.GetOutObj().transform.position;
			SceneObjIns.RoofEnd.transform.rotation=SAT.GetOutObj().transform.rotation;

			SceneObjAttrib SAE=SceneObjIns.RoofEnd.AddComponent<SceneObjAttrib>();

			GameObject OutRoofEndObj=SAE.GetOutObj();

			if(OutRoofEndObj!=null)
			{
			  SceneNpcPos.EndPos=OutRoofEndObj.transform.position;
			}
			else
			{
			  SceneNpcPos.EndPos=SceneObjIns.RoofEnd.transform.position;
			}
			return OutRoofEndObj;
		}

//		return SAT.GetOutObj();
	}

	void SetVertxColor()
	{
		float ColorChange=0.0f;

		if(ColorMat!=null)
		{
			for(int i=0;i<ColorMat.Length;i++)
			{
				if(ColorMat[i]==null){continue;}
				if(ColorMat[i].LightMat==null){continue;}

				ColorChange=ColorMat[i].BaseLight+Mathf.Sin(Time.time*ColorMat[i].AddSpeed)*ColorMat[i].AddRange;

				for(int k=0;k<ColorMat[i].LightMat.Length;k++)
				{
				  if(ColorMat[i].LightMat[k]==null){continue;}
					ColorMat[i].LightMat[k].SetFloat("LightyAni",ColorChange);
				}
			}
		}
	}

	
	//Update is called once per frame
	void Update () 
	{
		if(GenNewScene==true)
		{	
		 InitSceneObject();
		 GenNewScene=false;
		}
		
		SetVertxColor();

		/*
		int NearDoorID=-1;

		foreach(SceneDoor SceneSubDoor in SDoor)
		{
			int DID=SceneSubDoor.RunToDoor();
			if(DID>=0)
			{
			  NearDoorID=DID;
			}
		}

		if(NearDoorID!=-1)
		{
		 foreach(SceneDoor SceneSubDoor in SDoor)
		 {
			SceneSubDoor.RuntoTag(NearDoorID);
		 }
		}
		*/
	}	
}
