using UnityEngine;
using System.Collections;

public class MapCollision
{
    public static void StartGen(GameObject SceneRoot,bool CombineSubObj)
    {
     int TotalCollisionWallFaceCount=0;
     int TotalCollisionGroundFaceCount=0;
     
     int MaxTempVertex=4000*3;//per Roof+Turnnel Max Collision Face Less 400   

     Vector3[] WallVertices;
     Vector3[] WallNormals;
     int[] WallTriangles;
     
     Vector3[] GroundVertices;
     Vector3[] GroundNormals;
     int[] GroundTriangles;

     ////////////create Collison Struct //////////////////////////////////////////
     ////////////create Collison Struct ////////////////////////////////////////// 
     WallVertices  = new Vector3[MaxTempVertex];
     WallNormals   = new Vector3[MaxTempVertex];
     WallTriangles = new int[MaxTempVertex];
     
     GroundVertices  = new Vector3[MaxTempVertex];
     GroundNormals   = new Vector3[MaxTempVertex];
     GroundTriangles = new int[MaxTempVertex];
     ////////////End create Collison Struct ///////////////////////////////////////
     ////////////End create Collison Struct ///////////////////////////////////////

     for(int x=0;x<SceneRoot.transform.childCount;x++)
     {
        GameObject GObj = SceneRoot.transform.GetChild(x).gameObject;

        GroundAttrib Ga = GObj.GetComponent<GroundAttrib>();

        if(Ga!=null)
        {
            MeshCollider Mcollider=GObj.GetComponent<MeshCollider>();
            
            if(Mcollider!=null)
            {
               Mesh mesh=Mcollider.sharedMesh;
                    
               Vector3[] Vertices=mesh.vertices;
               Vector3[] Normals=mesh.normals;
               int[] Triangles=mesh.triangles;
               
               for(int i=0;i<Triangles.Length;i+=3)
               {
                   if(GObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i]]).y>=GObj.transform.localScale.y*0.5f)
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

               Mcollider.enabled=false;
             }

             if(CombineSubObj==true)
             {
                 for(int c=0;c<GObj.transform.childCount;c++)
                 {
                    GameObject SubGObj = GObj.transform.GetChild(c).gameObject;

                    Mcollider=SubGObj.GetComponent<MeshCollider>();
                    
                    if(Mcollider!=null)
                    {
                        Mesh mesh=Mcollider.sharedMesh;
                        
                        Vector3[] Vertices=mesh.vertices;
                        Vector3[] Normals=mesh.normals;
                        int[] Triangles=mesh.triangles;
                        
                        for(int i=0;i<Triangles.Length;i+=3)
                        {
                            if(GObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i]]).y>=GObj.transform.localScale.y*0.5f)
                            {
                                GroundVertices[TotalCollisionGroundFaceCount*3  ]=SubGObj.transform.localToWorldMatrix.MultiplyPoint(Vertices[Triangles[i  ]]);
                                GroundVertices[TotalCollisionGroundFaceCount*3+1]=SubGObj.transform.localToWorldMatrix.MultiplyPoint(Vertices[Triangles[i+1]]);
                                GroundVertices[TotalCollisionGroundFaceCount*3+2]=SubGObj.transform.localToWorldMatrix.MultiplyPoint(Vertices[Triangles[i+2]]);
                                
                                GroundNormals[TotalCollisionGroundFaceCount*3  ]=SubGObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i  ]]);
                                GroundNormals[TotalCollisionGroundFaceCount*3+1]=SubGObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i+1]]);
                                GroundNormals[TotalCollisionGroundFaceCount*3+2]=SubGObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i+2]]);
                                
                                GroundTriangles[TotalCollisionGroundFaceCount*3  ]  = TotalCollisionGroundFaceCount*3;
                                GroundTriangles[TotalCollisionGroundFaceCount*3+1]  = TotalCollisionGroundFaceCount*3+1;
                                GroundTriangles[TotalCollisionGroundFaceCount*3+2]  = TotalCollisionGroundFaceCount*3+2;
                                
                                TotalCollisionGroundFaceCount+=1;
                            }
                            else
                            {
                                WallVertices[TotalCollisionWallFaceCount*3  ]=SubGObj.transform.localToWorldMatrix.MultiplyPoint(Vertices[Triangles[i  ]]);
                                WallVertices[TotalCollisionWallFaceCount*3+1]=SubGObj.transform.localToWorldMatrix.MultiplyPoint(Vertices[Triangles[i+1]]);
                                WallVertices[TotalCollisionWallFaceCount*3+2]=SubGObj.transform.localToWorldMatrix.MultiplyPoint(Vertices[Triangles[i+2]]);
                                
                                WallNormals[TotalCollisionWallFaceCount*3  ]=SubGObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i  ]]);
                                WallNormals[TotalCollisionWallFaceCount*3+1]=SubGObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i+1]]);
                                WallNormals[TotalCollisionWallFaceCount*3+2]=SubGObj.transform.localToWorldMatrix.MultiplyVector(Normals[Triangles[i+2]]);
                                
                                WallTriangles[TotalCollisionWallFaceCount*3  ]  = TotalCollisionWallFaceCount*3;
                                WallTriangles[TotalCollisionWallFaceCount*3+1]  = TotalCollisionWallFaceCount*3+1;
                                WallTriangles[TotalCollisionWallFaceCount*3+2]  = TotalCollisionWallFaceCount*3+2;
                                
                                TotalCollisionWallFaceCount+=1;
                            }
                        }
                        
                        Mcollider.enabled=false;
                    }
                 }
             }
        }
     }

        GameObject CollisionWall =new GameObject();  
        CreateCollisionFace(CollisionWall,"CollisionWall",TotalCollisionWallFaceCount,WallVertices,WallNormals,WallTriangles);    
        
        GameObject CollisionGround =new GameObject();            
        CreateCollisionFace(CollisionGround,"CollisionGround",TotalCollisionGroundFaceCount,GroundVertices,GroundNormals,GroundTriangles);

        
        if(WallVertices!=null){WallVertices=null;}
        if(WallNormals!=null){WallNormals=null;}
        if(WallTriangles!=null){WallTriangles=null;}
        
        if(GroundVertices!=null){GroundVertices=null;}
        if(GroundNormals!=null){GroundNormals=null;}
        if(GroundTriangles!=null){GroundTriangles=null;}  
    }

    
    static void CreateCollisionFace(GameObject CollObj,string Name,int FaceCount,Vector3[] AVertices,Vector3[] ANormals,int[] ATriangles)
    {
        CollObj.name=Name;
        CollObj.transform.position=new Vector3(0,0,0);
        CollObj.transform.localScale=new Vector3(1,1,1);
        
        Mesh mesh = CollObj.AddComponent<MeshFilter>().mesh;
        MeshRenderer meshRender = CollObj.AddComponent<MeshRenderer>();
        
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
    }
}
