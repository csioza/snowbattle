using UnityEngine;
//using UnityEditor;
using System.Collections;

public class GroundAttrib : MonoBehaviour 
{

    public Color FogColor=new Color(1,1,1);
    public Color AmbiColor=new Color(1,1,1);
    public Color LightColor=new Color(1,1,1);

    public float FogDesity=0.5f;

    public Light SceneLight;
    public bool IsPublish=false;

    public string AttribDes;

    public float UAnimation=0.0f;
    
    public float VAnimation=0.0f;
    
    private float CUAnimation=0.0f;
    
    private float CVAnimation=0.0f;
    
    private Material MainMat;

    private bool Show;

    public float _TileRepeat0;
    public float _TileRepeat1;
    public float _TileRepeat2;

    public Texture2D Texture0;
    public Texture2D Texture1;
    public Texture2D Texture2;

    public Texture2D BlendTexture;

	// Use this for initialization
    void Awake()
    {
//        GameObject SceneL;
		/*
        if(gameObject.transform.parent.FindChild("SceneLight")==null)
        {
            SceneL=new GameObject();
            SceneL.transform.position=new Vector3(-5,5,-5);
            SceneL.transform.localRotation=new Quaternion(0.3f,0.6f,-0.3f,0.6f);
            SceneL.name="SceneLight";
            SceneL.transform.parent=gameObject.transform.parent;

            SceneLight=SceneL.AddComponent<Light>();
            SceneLight.color=new Color(1,1,1);
            SceneLight.type=LightType.Directional;
            SceneLight.intensity=0.6f;
        }
        else
        {
            SceneL=gameObject.transform.parent.FindChild("SceneLight").gameObject;
            SceneLight=SceneL.GetComponent<Light>();
        }
		*/
        Show=false;

		if(IsPublish==false)
		{
         if(gameObject.GetComponent<Renderer>().sharedMaterial==null)
         {
           gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load("EnvMaterial/mutillayer")as Material;      
         }
         else if(!gameObject.GetComponent<Renderer>().sharedMaterial.name.Contains("mutillayer"))
         {
           gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load("EnvMaterial/mutillayer")as Material; 
         }

         MainMat=gameObject.GetComponent<Renderer>().sharedMaterial;
		}
		else
		{
		 MainMat=gameObject.GetComponent<Renderer>().material;
		}

        MainMat.SetFloat("Uani",0);
        MainMat.SetFloat("Vani",0);

        
        if(BlendTexture!=null)
        {
            MainMat.mainTexture=BlendTexture;
        }
        else
        {
            BlendTexture=new Texture2D(1024, 1024, TextureFormat.RGB24,false);
            
            for(int x =0; x<1024; x++)
            {
                for(int y =0; y<1024; y++)
                {
                    BlendTexture.SetPixel(x, y, new Color(1,1,1));
                }
            }
            
            BlendTexture.Apply();
            
            MainMat.mainTexture=BlendTexture;
        }

        if(Texture0!=null)
        {
            MainMat.SetTexture("_Layer0",Texture0);
            MainMat.SetFloat("_TileRepeat0",_TileRepeat0);
        }

        if(Texture1!=null)
        {
            MainMat.SetTexture("_Layer1",Texture1);
            MainMat.SetFloat("_TileRepeat1",_TileRepeat1);
        }

        if(Texture2!=null)
        {
            MainMat.SetTexture("_Layer2",Texture2);
            MainMat.SetFloat("_TileRepeat2",_TileRepeat2);
        }
    }

    public void SetUvAdd(float UAdd,float VAdd)
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

    void OnBecameInvisible()
    {
        Show=false;
    }
    
    void OnBecameVisible()
    {
        Show=true;
    }

    public void InitEdit()
    {
       //if(IsPublish==false){return;}

       MeshCollider GroundColl=gameObject.GetComponent<MeshCollider>();
        
      if(GroundColl==null)
      {
           gameObject.AddComponent<MeshCollider>();
      }
      else 
      {
            GroundColl.sharedMesh=gameObject.GetComponent<MeshFilter>().sharedMesh;
      }

      for(int i=0;i<gameObject.transform.childCount;i++)
      {
          if(gameObject.transform.GetChild(i).name.Contains("collision"))
          {
              gameObject.transform.GetChild(i).gameObject.GetComponent<Renderer>().enabled=false;
          }
      }

      if(Texture0==null)
      {
       Texture0=MainMat.GetTexture("_Layer0")as Texture2D;
       _TileRepeat0=10.0f;
       MainMat.SetFloat("_TileRepeat0",_TileRepeat0);
      }

      if(Texture1==null)
      {
       Texture1=MainMat.GetTexture("_Layer1")as Texture2D;
       _TileRepeat1=10.0f;
       MainMat.SetFloat("_TileRepeat1",_TileRepeat1);
      }

      if(Texture2==null)
      {
       Texture2=MainMat.GetTexture("_Layer2")as Texture2D;
       _TileRepeat2=10.0f;
       MainMat.SetFloat("_TileRepeat2",_TileRepeat2);
      }
    }

    public void SaveBlendTexture(string SceneFoldName,string SaveFileName,bool Publish)
    {
		/*
        if(MainMat.mainTexture!=null)
        {
		  string path ="";
          byte[] bitarray=((Texture2D)(MainMat.mainTexture)).EncodeToPNG();
		  if(Publish)
		  {
		   path="Assets/Resources/Scene/Publish/"+SceneFoldName+"/"+name+SaveFileName+".png";
		   System.IO.File.WriteAllBytes(Application.dataPath+"/Resources/Scene/Publish/"+SceneFoldName+"/"+name+SaveFileName+".png", bitarray);
		  }
		  else
		  {
		   path="Assets/Resources/Scene/"+SceneFoldName+"/"+name+SaveFileName+".png";
		   System.IO.File.WriteAllBytes(Application.dataPath+"/Resources/Scene/"+SceneFoldName+"/"+name+SaveFileName+".png", bitarray);
		  }

		  System.Threading.Thread.Sleep(1000);

		  AssetDatabase.Refresh();

          TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
          if(textureImporter!=null)
          {
            textureImporter.textureType=TextureImporterType.Advanced;
            textureImporter.maxTextureSize=1024;
            textureImporter.wrapMode=TextureWrapMode.Repeat;
            textureImporter.filterMode=FilterMode.Bilinear;
            textureImporter.mipmapEnabled=false;
		    
            textureImporter.isReadable = true; 
            textureImporter.textureFormat=TextureImporterFormat.RGB24;
            textureImporter.filterMode=FilterMode.Bilinear;
            AssetDatabase.ImportAsset(path);
          }

		  if(Publish)
		  {
		   BlendTexture=Resources.Load("Scene/Publish/"+SceneFoldName+"/"+name+SaveFileName)as Texture2D;

		  }
		  else
		  {
		   BlendTexture=Resources.Load("Scene/"+SceneFoldName+"/"+name+SaveFileName)as Texture2D;
		  }
        }

        Texture0=MainMat.GetTexture("_Layer0")as Texture2D;
        Texture1=MainMat.GetTexture("_Layer1")as Texture2D;
        Texture2=MainMat.GetTexture("_Layer2")as Texture2D;

        _TileRepeat0=MainMat.GetFloat("_TileRepeat0");
        _TileRepeat1=MainMat.GetFloat("_TileRepeat1");
        _TileRepeat2=MainMat.GetFloat("_TileRepeat2");
        */
    }

    void Update()
    {
        if(Show==false){return;}

        if(UAnimation>0)
        {
            CUAnimation+=Time.deltaTime*UAnimation*0.01f;
            if(CUAnimation>1.0f){CUAnimation=0.0f;}
            MainMat.SetFloat("Uani",CUAnimation);
        }
        
        if(VAnimation>0)
        {
            CVAnimation+=Time.deltaTime*VAnimation*0.01f;
            if(CVAnimation>1.0f){CVAnimation=0.0f;}
            MainMat.SetFloat("Vani",CVAnimation);
        }
    }

    public void PublishEdit()
    {
        IsPublish=true;
			
        for(int i=0;i<gameObject.transform.childCount;i++)
        {
            if(gameObject.transform.GetChild(i).name.Contains("collision"))
            {
                gameObject.GetComponent<MeshCollider>().sharedMesh=gameObject.transform.GetChild(i).GetComponent<MeshFilter>().sharedMesh;
                gameObject.transform.GetChild(i).gameObject.GetComponent<Renderer>().enabled=false;
            }
            else
            {
                ObjectAttrib Oa=gameObject.transform.GetChild(i).GetComponent<ObjectAttrib>();
                if(Oa!=null)
                {
                    Oa.PublishEdit();
                }
            }
        }
    }

    public void SetFogDesity()
    {
        RenderSettings.fogEndDistance=200.0f*(FogDesity*2);
        RenderSettings.fogStartDistance=200.0f*FogDesity;
    }

    public void SetFogColor()
    {
        Camera.main.backgroundColor=FogColor;
        RenderSettings.fogColor=FogColor;
    }

    public void SetAmbiColor()
    {
        RenderSettings.ambientLight=AmbiColor;        
    }

    public void SetLightColor()
    {
        //SceneLight.color=LightColor;
    }

    public void SetEnvironment()
    {
        Camera.main.backgroundColor=FogColor;
        RenderSettings.fogColor=FogColor;
        RenderSettings.ambientLight=AmbiColor;
        RenderSettings.fogEndDistance=200.0f*(FogDesity*2);
        RenderSettings.fogStartDistance=200.0f*FogDesity;

        //SceneLight.color=LightColor;
    }

}
