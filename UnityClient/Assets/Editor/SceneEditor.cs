using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScenePathData
{
    public List<Vector3> PosList = new List<Vector3>();

}
public class SceneEditor : EditorWindow
{
    static int mblockCount = 100;
    static float mTileSize = 0.5f;
    static float mScale = 1.1f;
    static float mGateScale = 0.8f;
    static bool CheckCollider(GameObject parent, Bounds bounds)
    {
        Transform trans = parent.transform.Find("Collider");
        if (null != trans)
        {
            //trans.gameObject.layer = LayerMask.NameToLayer("EnableCollider");
            for (int i = 0; i < trans.childCount; i++)
            {
                Transform childTrans = trans.GetChild(i);
                BoxCollider childBox = childTrans.GetComponent<BoxCollider>();
                if (null == childBox)
                {
                    continue;
                }
                //childTrans.gameObject.layer = LayerMask.NameToLayer("EnableCollider");
                if (bounds.Intersects(childBox.bounds) && childBox.size.y > 1.0f)
                {
                    //Debug.Log("childtrans collider name =" + childTrans.name);
                    return true;
                }
            }
        }
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Transform tra = parent.transform.GetChild(i);
            //Debug.Log("tra=" + tra.name);
            if (CheckCollider(tra.gameObject, bounds))
            {
                return true;
            }
        }
        return false;
    }
    static void ChangeColliderLayerName(GameObject parent)
    {
        Transform trans = parent.transform.Find("Collider");
        if (null != trans)
        {
            bool isScale = true;
            if (trans.parent.gameObject.name.Contains("RemovableGate_"))
            {
                isScale = false;
            }
            trans.gameObject.layer = LayerMask.NameToLayer("EnableCollider");
            for (int i = 0; i < trans.childCount; i++)
            {
                Transform childTrans = trans.GetChild(i);
                float fScale = (isScale ? mScale : mGateScale);
                childTrans.localScale = new Vector3(childTrans.localScale.x * fScale, childTrans.localScale.y * fScale, childTrans.localScale.z * fScale);
                childTrans.gameObject.layer = LayerMask.NameToLayer("EnableCollider");
            }
        }
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Transform tra = parent.transform.GetChild(i);
            ChangeColliderLayerName(tra.gameObject);
        }
    }
    [@MenuItem("SceneEditor/GenRoomPathData")]
    public static void GenRoomPathData()
    {
        GameObject[] objArray = Selection.gameObjects;
        if (null == objArray)
        {
            Debug.Log("please select room res!!!");
            return;
        }
        if (objArray.Length <= 0)
        {
            return;
        }
        if (null == objArray[0])
        {
            return;
        }
        // 选择的物件
        GameObject obj = GameObject.Instantiate(objArray[0]) as GameObject;
        Transform locator = obj.transform.Find("Locator");
        if (null == locator)
        {
            Debug.Log("not exist locator!!!!");
            return;
        }
        ChangeColliderLayerName(obj);
        GameObject colliderObj = GameObject.Find("collider");
        BoxCollider box = null;
        if (null == colliderObj)
        {
            colliderObj = new GameObject("collider");
            box = colliderObj.AddComponent<BoxCollider>();
            box.size = new Vector3(mTileSize, 1f, mTileSize);
        }
        else
        {
            box = colliderObj.GetComponent<BoxCollider>();
        }
        Vector3 offsetPos = new Vector3(locator.position.x, locator.position.y, locator.position.z);
        int nMaxSize = 0;
        for (int x = 0; x < mblockCount; x++)
        {
            for (int y = 0; y < mblockCount; y++)
            {
                Vector3 pos = new Vector3(x * mTileSize + mTileSize / 2, 0.5f, y * mTileSize + mTileSize / 2);
                colliderObj.transform.position = new Vector3(offsetPos.x + pos.x, offsetPos.y, offsetPos.z + pos.z);
                bool ishave = CheckCollider(obj, box.bounds);
                if (ishave)
                {
                    if (nMaxSize < y)
                    {
                        nMaxSize = y;
                    }
                    if (nMaxSize < x)
                    {
                        nMaxSize = x;
                    }
                }
            }
        }
        Debug.Log("nMaxSize=" + nMaxSize);
        GameObject preObj = GameObject.Find("preObj");
        if (null == preObj)
        {
            preObj = new GameObject("preObj");
        }
        offsetPos = new Vector3(locator.position.x, locator.position.y + 10.0f, locator.position.z);
        SM.SceneObstacleData obstacleData = new SM.SceneObstacleData();
        obstacleData.MaxSize = nMaxSize;
        //nMaxSize += 2;
        for (int x = 0; x < nMaxSize; x++)
        {
            for (int y = 0; y < nMaxSize; y++)
            {
                Vector3 pos = new Vector3(x * mTileSize + mTileSize / 2, 0.5f, y * mTileSize + mTileSize / 2);
                colliderObj.transform.position = new Vector3(offsetPos.x + pos.x, offsetPos.y, offsetPos.z + pos.z);
                RaycastHit[] hit;
                hit = Physics.SphereCastAll(colliderObj.transform.position, mTileSize / 2, Vector3.down, 1000f);
                bool ishave = false;
                SM.SceneObstacleData.ObData obData = new SM.SceneObstacleData.ObData();
                foreach (RaycastHit h in hit)
                {
                    //Debug.Log("h.collider.gameObject.transform:" + h.collider.gameObject.transform.parent.name);
                    if ("Collider" == h.collider.gameObject.transform.parent.name)
                    //if (LayerMask.NameToLayer("EnableCollider") == h.collider.gameObject.layer)
                    {
                        if (h.collider.gameObject.transform.parent.parent.name.Contains("RemovableGate_E"))
                        {
                            obData.enDirection = SM.ENGateDirection.enEAST;
                            //Debug.Log("name:" + h.collider.gameObject.transform.parent.parent.name);
                        }
                        else if (h.collider.gameObject.transform.parent.parent.name.Contains("RemovableGate_W"))
                        {
                            obData.enDirection = SM.ENGateDirection.enWEST;
                            //Debug.Log("name:" + h.collider.gameObject.transform.parent.parent.name);
                        }
                        else if (h.collider.gameObject.transform.parent.parent.name.Contains("RemovableGate_N"))
                        {
                            obData.enDirection = SM.ENGateDirection.enNORTH;
                            //Debug.Log("name:" + h.collider.gameObject.transform.parent.parent.name);
                        }
                        else if (h.collider.gameObject.transform.parent.parent.name.Contains("RemovableGate_S"))
                        {
                            obData.enDirection = SM.ENGateDirection.enSOUTH;
                            //Debug.Log("name:" + h.collider.gameObject.transform.parent.parent.name);
                        }
                        ishave = true;
                        break;
                    }
                }
                obData.obstacleArray = new Vector3(x, y, ishave ? 1 : 0);
                obstacleData.mData.Add(obData);
                ////////////////////////////////////////////////////
                GameObject childObj = new GameObject("x_" + x.ToString("00") + "_y_" + y.ToString("00") + "(" + ishave.ToString() + ")");
                childObj.transform.parent = preObj.transform;
                childObj.transform.position = colliderObj.transform.position;
                if (!ishave)
                {
                    BoxCollider box1 = childObj.AddComponent<BoxCollider>();
                    box1.size = new Vector3(mTileSize, 0.1f, mTileSize);
                }
            }
        }
        preObj.LocalPositionY(-10f);
//         for (int i = 0; i < Map.GetLength(1); i++)
//         {
//             for (int j = 0; j < Map.GetLength(0); j++)
//             {
//                 bool isWalkable = Mathf.CeilToInt(Map[j, i].obstacleArray.z) == 1 ? false : true;
//                 if (!isWalkable)
//                     continue;
//                 for (int y = i - 1; y < i + 2; y++)
//                 {
//                     for (int x = j - 1; x < j + 2; x++)
//                     {
//                         if (y < 0 || x < 0 || y >= Map.GetLength(1) || x >= Map.GetLength(0))
//                             continue;
//                         isWalkable = Mathf.CeilToInt(Map[x, y].obstacleArray.z) == 1 ? false : true;
//                         if (!isWalkable)
//                             continue;
//                         Vector3 pos1 = new Vector3(j * mTileSize/* + mTileSize / 2*/, 0.5f, i * mTileSize/* + mTileSize / 2*/);
//                         Vector3 start = new Vector3(offsetPos.x + pos1.x, offsetPos.y, offsetPos.z + pos1.z);
// 
//                         Vector3 pos = new Vector3(x * mTileSize/* + mTileSize / 2*/, 0.5f, y * mTileSize/* + mTileSize / 2*/);
//                         Vector3 end = new Vector3(offsetPos.x + pos.x, offsetPos.y, offsetPos.z + pos.z);
// 
//                         UnityEngine.Debug.DrawLine(start, end, Color.green);
//                     }
//                 }
//             }
//         }
        string path = AssetDatabase.GetAssetPath(objArray[0]);
        Debug.Log("path=" + path);
        path = Path.GetDirectoryName(path);
        path += "/";
        string fileName = path + objArray[0].name + "-bytes.bytes";
        using (FileStream targetFile = new FileStream(fileName, FileMode.Create))
        {
            byte[] buff = obstacleData.Save();
            targetFile.Write(buff, 0, buff.Length);
        }
        Debug.Log("fileName=" + fileName);
        GameObject.DestroyImmediate(colliderObj);
    }
//     static public byte[] Save(List<Vector3> PosList, int nMaxSize)
//     {
//         BinaryHelper helper = new BinaryHelper();
//         helper.Write(nMaxSize);
//         helper.Write(PosList.Count);
//         for (int i = 0; i < PosList.Count; i++ )
//         {
//             helper.Write(PosList[i].x);
//             helper.Write(PosList[i].y);
//             helper.Write(PosList[i].z);
//         }
//         return helper.GetBytes();
//     }
}