using UnityEngine;
using System.Collections;
//拾取掉落
public class PickupDropCallback : MonoBehaviour
{
    short     mDropID = -1;
    short     mItemID = -1;
    DropHolderTrigger mDropTrigger;
    public void AttachDropValue(DropHolderTrigger trig, short dropID, short itmID)
    {
        mDropTrigger = trig;
        mDropID = dropID;
        mItemID = itmID;
    }
	void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger)
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
		if (ClientNet.Singleton.IsConnected && prop.Type == ActorType.enMain)
		{
            if ((mDropID > 0) && (mItemID > 0))
            {
                ////拾取掉落 modify by luozj
                //ClientNet.Singleton.SendPacket(new PickupDropPacket(mDropID, mItemID));
            }
            if (mDropTrigger != null)
                mDropTrigger.DisableDropHolder();
            mDropID = -1;
            mItemID = -1;
		}
	}
}
