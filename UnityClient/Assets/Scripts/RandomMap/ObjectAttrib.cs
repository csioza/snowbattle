using UnityEngine;
using System.Collections;

public enum CObjectType
{
    Normal=0,
    LightObj,
    ObjectGrass,
    BillGrass,
    FlareObject,
    GroupObj,
    AnimationObj,
    BattleAreaObj,
    BatterDoorObj,
    NpcObj
};

public class ObjectAttrib : MonoBehaviour 
{
    private float TimeRange;

    private float ZoomRange;

    private bool Show;

    private Light LObj;

    private Vector3 LocalScale;

    public CObjectType  OType;

    public string AttribDes;

    private Material MainMat;

    public float UAnimation=0.0f;

    public float VAnimation=0.0f;

    public float LightSize=10.0f;

	public  Color LightColor;

    private float CUAnimation=0.0f;
    
    private float CVAnimation=0.0f;

    private bool FirstInit=false;

    private Transform CameraTrans;


    void Awake()
    {
        Show=false;

        TimeRange=3.0f+UnityEngine.Random.Range(-0.5f,0.5f);
        ZoomRange=2.0f+UnityEngine.Random.Range(-1.0f,1.0f);

        CameraTrans=Camera.main.transform;

        if(gameObject.name.Contains("Gcoll"))
        {
            OType=CObjectType.BattleAreaObj;
        }
        else if(gameObject.name.Contains("GDoor"))
        {
            OType=CObjectType.BatterDoorObj;
        }
        else if(gameObject.name.Contains("GPeople"))
        {
            OType=CObjectType.NpcObj;
        }
        else if(gameObject.name.Contains("ani"))
        {
            for(int i=0;i<gameObject.transform.childCount;i++)
            {
                if(!gameObject.transform.GetChild(i).name.Contains("collision"))
                {
                    if(gameObject.transform.GetChild(i).GetComponent<ObjectAttrib>()==null)
                    {
                        gameObject.transform.GetChild(i).gameObject.AddComponent<ObjectAttrib>();
                    }
                }
            }
            
            OType=CObjectType.AnimationObj;
        }
        else if(gameObject.name.Contains("group"))
        {
            for(int i=0;i<gameObject.transform.childCount;i++)
            {
                if(!gameObject.transform.GetChild(i).name.Contains("collision"))
                {
                  if(gameObject.transform.GetChild(i).GetComponent<ObjectAttrib>()==null)
                  {
                     gameObject.transform.GetChild(i).gameObject.AddComponent<ObjectAttrib>();
                  }
                }
            }

            OType=CObjectType.GroupObj;
        }
        else if(gameObject.name.Contains("light"))
        {
            LObj=gameObject.GetComponent<Light>();
            
            if(LObj==null)
            {
                LObj=gameObject.AddComponent<Light>();
            }

            LObj.range=LightSize;
            LObj.intensity=2.5f;

			LObj.color=LightColor;

            ZoomRange=UnityEngine.Random.Range(-0.3f,0.3f);

            OType=CObjectType.LightObj;
        }
        else if (gameObject.name.Contains ("grass")) 
        {
            if(gameObject.name.Contains("object"))
            {
                OType=CObjectType.ObjectGrass;
            }
            else
            {
                OType=CObjectType.BillGrass;

                gameObject.transform.eulerAngles=new Vector3(0,CameraTrans.eulerAngles.y,0);
            }
        }
        else if(gameObject.name.Contains("flare"))
        {    
            OType=CObjectType.FlareObject;

            LocalScale=new Vector3();
            LocalScale.x=gameObject.transform.localScale.x;
            LocalScale.y=gameObject.transform.localScale.y;
            LocalScale.z=gameObject.transform.localScale.z;

            gameObject.transform.eulerAngles=new Vector3(0,CameraTrans.eulerAngles.y,0);
        }
        else
        {
            OType=CObjectType.Normal;
        }

        if((OType!=CObjectType.AnimationObj)
           &&(OType!=CObjectType.GroupObj)
           &&(OType!=CObjectType.BattleAreaObj)
           &&(OType!=CObjectType.BatterDoorObj)
           &&(OType!=CObjectType.NpcObj))
        {
         MainMat=gameObject.GetComponent<Renderer>().sharedMaterial;
         MainMat.SetFloat("Uani",0);
         MainMat.SetFloat("Vani",0);
        }
    }

    public void SetUvAdd(float UAdd,float VAdd)
    {
        if((OType!=CObjectType.AnimationObj)
           &&(OType!=CObjectType.GroupObj)
           &&(OType!=CObjectType.BattleAreaObj)
           &&(OType!=CObjectType.BatterDoorObj)
           &&(OType!=CObjectType.NpcObj))
       {
        UAnimation+=UAdd;
        if(UAnimation>1.0f)
        {
            UAnimation=0;
            CUAnimation=0;
            MainMat.SetFloat("Uani",0);
        }

        VAnimation+=VAdd;
        if(VAnimation>1.0f)
        {
            VAnimation=0;
            CVAnimation=0;
            MainMat.SetFloat("Vani",0);
        }
       }
    }

    public void SetLightSize()
    {
        if(OType==CObjectType.LightObj)
        {
            LightSize+=1.0f;
            if(LightSize>10.0f)
            {
                LightSize=1.0f;
            }

            LObj=gameObject.GetComponent<Light>();
            
            if(LObj!=null)
            {
                LObj.range=LightSize;
            }
        }
        else if((OType==CObjectType.GroupObj)||(OType==CObjectType.AnimationObj))
        {
            for(int i=0;i<gameObject.transform.childCount;i++)
            {
                if(gameObject.transform.GetChild(i).name.Contains("light"))
                {
                    ObjectAttrib Oa=gameObject.transform.GetChild(i).GetComponent<ObjectAttrib>();
                    if(Oa!=null)
                    {
                        Oa.LightSize+=1.0f;
                        if(Oa.LightSize>10.0f)
                        {
                            Oa.LightSize=1.0f;
                        }

                        LObj=gameObject.transform.GetChild(i).GetComponent<Light>();
                        
                        if(LObj!=null)
                        {
                            LObj.range=Oa.LightSize;
                        }
                    }                    
                }        
            }
        }
    }

    public Color GetLightColor()
    {
        if(OType==CObjectType.LightObj)
        {
			return LightColor;
        }
        else if((OType==CObjectType.GroupObj)||(OType==CObjectType.AnimationObj))
        {
            for(int i=0;i<gameObject.transform.childCount;i++)
            {
                if(gameObject.transform.GetChild(i).name.Contains("light"))
                {
                    ObjectAttrib Oa=gameObject.transform.GetChild(i).GetComponent<ObjectAttrib>();
                    if(Oa!=null)
                    {
						return Oa.LightColor;
                    }                    
                }        
            }
        }

        return new Color(1f,1f,1f,1f);
    }

    public void SetLightColor(Color Col)
    {
        if(OType==CObjectType.LightObj)
        {
			LightColor=Col;
            LObj=gameObject.GetComponent<Light>();                        
            if(LObj!=null)
            {
                LObj.color=Col;
            }
        }
        else if((OType==CObjectType.GroupObj)||(OType==CObjectType.AnimationObj))
        {
            for(int i=0;i<gameObject.transform.childCount;i++)
            {
                if(gameObject.transform.GetChild(i).name.Contains("light"))
                {
                    ObjectAttrib Oa=gameObject.transform.GetChild(i).GetComponent<ObjectAttrib>();
                    if(Oa!=null)
                    {
						Oa.LightColor=Col;
                        LObj=gameObject.transform.GetChild(i).GetComponent<Light>();                        
                        if(LObj!=null)
                        {
                           LObj.color=Col;
                        }
                    }                    
                }        
            }
        }
    }

    void OnBecameInvisible()
    {
        Show=false;
    }
    
    void OnBecameVisible()
    {
        Show=true;
    }

    void Update()
    {
        if(Show==false){return;}

        if((OType==CObjectType.AnimationObj)
         ||(OType==CObjectType.GroupObj)
         ||(OType==CObjectType.BattleAreaObj)
         ||(OType==CObjectType.BatterDoorObj)
         ||(OType==CObjectType.NpcObj))
         {return;}

        if(UAnimation>0)
        {
           CUAnimation+=Time.deltaTime*UAnimation;
           if(CUAnimation>1.0f){CUAnimation=0.0f;}
           MainMat.SetFloat("Uani",CUAnimation);
        }

        if(VAnimation>0)
        {
           CVAnimation+=Time.deltaTime*VAnimation;
           if(CVAnimation>1.0f){CVAnimation=0.0f;}
           MainMat.SetFloat("Vani",CVAnimation);
        }
       
        if(OType==CObjectType.Normal)
        {
            return;
        }

        float Change=Mathf.Sin(Time.time*TimeRange)*ZoomRange;

        if(OType==CObjectType.BillGrass)
        {
          gameObject.transform.eulerAngles=new Vector3(0,CameraTrans.eulerAngles.y,Change);
        }
        else  if(OType==CObjectType.ObjectGrass)
        {
           gameObject.transform.eulerAngles=new Vector3(gameObject.transform.eulerAngles.x+Change*0.4f,gameObject.transform.eulerAngles.y,gameObject.transform.eulerAngles.z);
        }
        else  if(OType==CObjectType.FlareObject)
        {
           gameObject.transform.eulerAngles=new Vector3(-CameraTrans.eulerAngles.x,CameraTrans.eulerAngles.y,-CameraTrans.eulerAngles.z);
           gameObject.transform.localScale = new Vector3(LocalScale.x+Change*0.05f,LocalScale.y+Change*0.05f,LocalScale.z+Change*0.05f);
        }
        else  if(OType==CObjectType.LightObj)
        {
           if(LObj!=null)
           {
             LObj.intensity+=Change;

             if(FirstInit==false)
             {
               FirstInit=true;

               if(gameObject.transform.parent.name.Contains("ground"))
               {
                   LObj.range=LightSize*gameObject.transform.localScale.x*gameObject.transform.parent.localScale.x;
               }
               else
               {
                   LObj.range=LightSize*gameObject.transform.localScale.x*gameObject.transform.parent.localScale.x*gameObject.transform.parent.parent.localScale.x;
               }
             }
           }
        }
    }

	public void InitEdit()
	{
        if((OType==CObjectType.BattleAreaObj)||(OType==CObjectType.BatterDoorObj)||(OType==CObjectType.NpcObj))
        {
            if(OType==CObjectType.NpcObj)
            {
             BoxCollider BTemp=gameObject.GetComponent<BoxCollider>();
             if(BTemp!=null)
             {
                    Object.DestroyImmediate(BTemp);       
             }
            }
        }
        else
        {           
           MeshCollider ObjectColl=gameObject.GetComponent<MeshCollider>();
           if(ObjectColl!=null)
           {
               Object.DestroyImmediate(ObjectColl);
           }
           
           BoxCollider BTemp=gameObject.GetComponent<BoxCollider>();
           if(BTemp==null)
           {
               BTemp=gameObject.AddComponent<BoxCollider>();         
           }
        }
	}

    public void PublishEdit()
    {
        if((OType==CObjectType.BattleAreaObj)||(OType==CObjectType.BatterDoorObj)||(OType==CObjectType.NpcObj))
        {
            gameObject.GetComponent<Renderer>().enabled=false;
            if(OType==CObjectType.NpcObj)
            {
                BoxCollider BTemp=gameObject.GetComponent<BoxCollider>();
                if(BTemp!=null)
                {
                    Object.DestroyImmediate(BTemp);       
                }
            }
        }
        else
        {
          BoxCollider Temp=gameObject.GetComponent<BoxCollider>();
          if(Temp!=null)
          {
            Object.DestroyImmediate(Temp);
          }

          Debug.LogWarning("publish"+gameObject.name);
          for(int i=0;i<gameObject.transform.childCount;i++)
          {
            if(gameObject.transform.GetChild(i).name.Contains("collision"))
            {
                MeshCollider ObjectColl=gameObject.AddComponent<MeshCollider>();
                ObjectColl.sharedMesh=gameObject.transform.GetChild(i).GetComponent<MeshFilter>().sharedMesh;
                gameObject.transform.GetChild(i).GetComponent<Renderer>().enabled=false;

            }
            else
            {
                if(gameObject.transform.GetChild(i).GetComponent<ObjectAttrib>()!=null)
                {
                    if(gameObject.transform.GetChild(i).GetComponent<ObjectAttrib>().OType==CObjectType.Normal) 
                    {
                        Object.DestroyImmediate(gameObject.transform.GetChild(i).GetComponent<ObjectAttrib>());
                    }
                }
            }
          }

         if(OType==CObjectType.Normal) 
         {
            if((gameObject.GetComponent<ObjectAttrib>().UAnimation==0.0f)
               &&(gameObject.GetComponent<ObjectAttrib>().VAnimation==0.0f))
            {
                Object.DestroyImmediate(gameObject.GetComponent<ObjectAttrib>());
            }
         }
       }
    }

}
