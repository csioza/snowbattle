using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class ArrayEditInfo
{
    [SerializeField]
    public GameObject obj;      // 阵型预制
    [SerializeField]
    public int weight  = 1;     // 此阵型出现的权重
}
// 阵型编辑
public class RMEArrayEdit : RMEBaseInfoEdit
{
    public List<ArrayEditInfo> list; //  阵型列表

    public RMEArrayEdit()
        : base(Type.enArray)
    {

    }

    public void FillTheFormation()
    {
        List<GameObject> gameObjList    = new List<GameObject>(); 
        Component[] children            = gameObject.GetComponentsInChildren(typeof(RMEMonsterEdit),true);

        foreach (Component child in children)
        {
            if (!gameObjList.Contains(child.transform.parent.gameObject))
            {
                gameObjList.Add(child.transform.parent.gameObject);
            }
        }
        list.Clear();

        foreach (GameObject obj in gameObjList)
        {
            ArrayEditInfo info = new ArrayEditInfo();

            info.obj = obj;
            info.weight = 1;
            list.Add(info);
        }
    }
}


