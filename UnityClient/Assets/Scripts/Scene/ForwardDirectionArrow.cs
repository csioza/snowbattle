using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionArrowData
{
    public GameObject mObjArea = null;
    public GameObject mObjArrow = null;
    public bool mActive = false;
}
public class ForwardDirectionArrow
{
    static public ForwardDirectionArrow Singleton { get; private set; }
    public ForwardDirectionArrow()
    {
        if (null == Singleton)
        {
            Singleton = this;
        }
        else
        {
            Debug.LogWarning("ForwardDirectionArrow Recreated");
        }
    }
    Dictionary<GameObject, DirectionArrowData> mArrowDataDict = new Dictionary<GameObject, DirectionArrowData>();
    public void BuildArrow(GameObject obj)
    {
		return;
//        MainGame.Singleton.StartCoroutine(Coroutine_LoadArrow(obj));
    }
    IEnumerator Coroutine_LoadArrow(GameObject obj)
    {
        if (!mArrowDataDict.ContainsKey(obj))
        {
            DirectionArrowData arrowData = new DirectionArrowData();
            arrowData.mObjArea = obj;
            arrowData.mActive = true;
            GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
            IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath("ef-e-directionpoint-E01"), data);
            while (true)
            {
                e.MoveNext();
                if (data.m_isFinish)
                {
                    break;
                }
                yield return e.Current;
            }
            if (data.m_obj != null)
            {
                arrowData.mObjArrow = data.m_obj as GameObject;
                //string prefabName = "Effect/ef-e-directionpoint-E01";
                //arrowData.mObjArrow = GameObject.Instantiate(GameData.LoadPrefab<GameObject>(prefabName)) as GameObject;
                arrowData.mObjArrow.transform.parent = MainGame.Singleton.MainObject.transform;
                Vector3 vDirection = arrowData.mObjArea.transform.position - ActorManager.Singleton.MainActor.CenterPartWorldPos;
                vDirection.y = 0.0f;
                vDirection.Normalize();
                arrowData.mObjArrow.transform.localPosition = ActorManager.Singleton.MainActor.CenterPartWorldPos;
                arrowData.mObjArrow.transform.rotation = Quaternion.LookRotation(vDirection.normalized) * Quaternion.LookRotation(Vector3.forward);
                arrowData.mObjArrow.transform.localScale = Vector3.one;
                arrowData.mObjArrow.SetActive(true);
                mArrowDataDict.Add(obj, arrowData);
            }
        }
    }
    public void HideArrow(GameObject obj)
    {
        DirectionArrowData arrowData;
        if (mArrowDataDict.TryGetValue(obj, out arrowData))
        {
            arrowData.mActive = false;
            arrowData.mObjArrow.SetActive(false);
            //GameObject.Destroy(arrowData.mObjArrow);
            PoolManager.Singleton.ReleaseObj(arrowData.mObjArrow);
        }
        mArrowDataDict.Remove(obj);
    }
    public void LateUpdate()
    {
        foreach (KeyValuePair<GameObject, DirectionArrowData> pair in mArrowDataDict)
        {
            if (pair.Value.mActive)
            {
                DirectionArrowData arrowData = pair.Value;
                arrowData.mObjArrow.transform.localPosition = ActorManager.Singleton.MainActor.CenterPartWorldPos;
                Vector3 vDirection = arrowData.mObjArea.transform.position - ActorManager.Singleton.MainActor.CenterPartWorldPos;
                vDirection.y = 0.0f;
                vDirection.Normalize();
                arrowData.mObjArrow.transform.rotation = Quaternion.LookRotation(vDirection.normalized) * Quaternion.LookRotation(Vector3.forward);
            }
        }
    }
}