using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


[ExecuteInEditMode]
// 元素点位置配置
public class RMEAdjustPosInScene : RMEBaseInfoEdit
{

    public float height = 1.25f;
    public RMEAdjustPosInScene()
        :base(Type.enAdjustPos)
    {

    }
    void Start()
    {

        
    }

    void Update()
    {

         Vector3 localPos = transform.localPosition;
 
         int x = (int)(localPos.x / height);
 
         int y = (int)(localPos.z / height);
 
         transform.LocalPositionX( x * height);
 
         transform.LocalPositionZ(y * height);


    }
   
}




