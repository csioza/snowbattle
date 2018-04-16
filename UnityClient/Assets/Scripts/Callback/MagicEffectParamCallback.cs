/********************************************************************
	created:	2013/12/24
	created:	24:12:2013   17:50
	filename: 	E:\work\Monster\trunk\Client\Assets\Scripts\Callback\MagicEffectParamCallback.cs
	file path:	E:\work\Monster\trunk\Client\Assets\Scripts\Callback
	file base:	MagicEffectParamCallback
	file ext:	cs
	author:		luozj
	
	purpose:	特效的脚本，存放特效的参数
*********************************************************************/

using UnityEngine;
using System.Collections;

public class MagicEffectParamCallback : MonoBehaviour
{
	//命中目标时的特效
	public GameObject m_effectObj = null;
	public float m_effectFloat = 0.0f;
	public string m_effectString = "";
	//时间缩放
	public string m_timeScaleString = "";
	//摄像机震动
	public string m_CameraFieldString = "";
	//目标
	GameObject m_mineObj = null;
	public void Init(GameObject obj)
	{
		m_mineObj = obj;
	}

    public void EffectInTarget(AnimationEvent animEvent)
	{
		if (m_mineObj == null) return;
		AnimationShaderParamCallback shader = m_mineObj.GetComponent<AnimationShaderParamCallback>();
        if (null == shader)
        {
            return;
        }
        shader.EffectInTarget(animEvent);
    }

    public void TimeScale(AnimationEvent animEvent)
	{
		if (m_mineObj == null) return;
        AnimationShaderParamCallback shader = m_mineObj.GetComponent<AnimationShaderParamCallback>();
        if (null == shader)
        {
            return;
        }
        shader.TimeScale(animEvent);
    }

    public void CameraFieldOfView(AnimationEvent animEvent)
    {
		if (m_mineObj == null) return;
        AnimationShaderParamCallback shader = m_mineObj.GetComponent<AnimationShaderParamCallback>();
        if (null == shader)
        {
            return;
        }
        shader.CameraFieldOfView(animEvent);
    }
}
