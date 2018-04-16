using UnityEngine;
//using UnityEditor;
using System.Collections;

public class SceneRootAttrib : MonoBehaviour 
{
    public Material  SkyMat;

	// Use this for initialization
    void Awake()
    {
        RenderSettings.fog = false;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 100.0f;
        RenderSettings.fogEndDistance = 200.0f;

        if(SkyMat!=null)
        {
         Skybox Sk=Camera.main.gameObject.GetComponent<Skybox>();
         if(Sk==null)
         {
           Sk=Camera.main.gameObject.AddComponent<Skybox>();
         }
         Sk.material=SkyMat;
        }
    }

    public void SetSkyBoxMat(Material Mat)
    {
      SkyMat=Mat;

      Skybox Sk=Camera.main.gameObject.GetComponent<Skybox>();

      if(SkyMat==null)
      {
        if(Sk!=null)
        {
          Sk.material=null;
          GameObject.DestroyImmediate(Sk);
        }
      }
      else
      {
       if(Sk==null)
       {
          Sk=Camera.main.gameObject.AddComponent<Skybox>();
       }

       Sk.material=SkyMat;
      }
    }
}
