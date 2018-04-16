using UnityEngine;
using System.Collections;
using System;

public class AnimationDeleteBlockCallback : MonoBehaviour
{
    public Action<GameObject, AnimationEvent> Callback { get; set; }
    private bool isOver = false;
    //private string m_areaName;
    //public void SetCurrentArea(string areaName)
    //{
    //    m_areaName = areaName;
    //}
    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if (isOver)
        {
            return;
        }
        Transform targetObj = other.transform;
        while (null != targetObj && targetObj.name != "body")
        {
            targetObj = targetObj.parent;
        }
        if (null == targetObj)
        {
            return;
        }
        ActorProp prop = targetObj.parent.GetComponent<ActorProp>();
        if (null == prop)
        {
            return;
        }
        if (prop.ActorLogicObj.IsDead)
        {
            return;
        }
        if (prop.Type == ActorType.enMain)
        {
            WorldParamInfo WorldParamList;
            WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enBlockDeleteAnimation);
            gameObject.GetComponent<Animation>().Play(WorldParamList.StringTypeValue);
            //WorldParamList = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enBlockFragment);
            //targetObj.gameObject.animation.Play(WorldParamList.StringTypeValue);
            isOver = true;
            GameObject block = null;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform child = gameObject.transform.GetChild(i);
                if (null != child.GetComponent<Collider>())
                {
                    block = child.gameObject;
                    break;
                }
            }
            if (null != block)
            {
                block.SetActive(false);
            }
            GameObject prefabObj = GameObject.Find("SceneAreas");
            if (prefabObj == null)
            {
                return;
            }
            for (int areaIndex = 0; areaIndex < prefabObj.transform.childCount; areaIndex++)
            {
                Transform tarArea = prefabObj.transform.GetChild(areaIndex);
                if (tarArea == null)
                {
                    continue;
                }
                for (int pointIndex = 0; pointIndex < tarArea.childCount; pointIndex++)
                {
                    Transform tarPointTrans = tarArea.GetChild(pointIndex);
                    if (tarPointTrans == null)
                    {
                        continue;
                    }
                    if (tarPointTrans.gameObject.name == "green")
                    {
                        if (tarPointTrans.gameObject.activeSelf == true)
                        {
                            tarPointTrans.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}