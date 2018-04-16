using System;
using UnityEngine;

//actor模型下的附着点脚本，用来改变附着点位置
public class ActorAdherentPoints : MonoBehaviour
{
    public Transform FollowedT = null;

    // Update is called once per frame
    void LateUpdate()
    {
        if (FollowedT != null)
        {//跟随FollowT的x、z
            transform.position = new Vector3(FollowedT.position.x, transform.position.y, FollowedT.position.z);
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }
}
