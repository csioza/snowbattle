using UnityEngine;
using System.Collections;




public static class UI_Extensions
{
	public static void Visible(this UIWidget widget, bool enable)
	{
		widget.enabled = enable;
		widget.gameObject.SetActive(enable);
	}
	

#region Transform position scale


	public static void LocalPositionX(this MonoBehaviour mono, float x)
	{
		LocalPositionX(mono.transform, x);
	}
	public static void LocalPositionX(this GameObject obj, float x)
	{
		LocalPositionX(obj.transform, x);
	}
	public static void LocalPositionX(this Transform transform, float x)
	{
		Vector3 pos = transform.localPosition;
		pos.x = x;
		transform.localPosition = pos;
	}



	public static void LocalPositionY(this MonoBehaviour mono, float y)
	{
		LocalPositionY(mono.transform, y);
	}
	public static void LocalPositionY(this GameObject obj, float y)
	{
		LocalPositionY(obj.transform, y);
	}
	public static void LocalPositionY(this Transform transform, float y)
	{
		Vector3 pos = transform.localPosition;
		pos.y = y;
		transform.localPosition = pos;
	}



	public static void LocalPositionZ(this MonoBehaviour mono, float z)
	{
		LocalPositionZ(mono.transform, z);
	}
	public static void LocalPositionZ(this GameObject obj, float z)
	{
		LocalPositionZ(obj.transform, z);
	}
	public static void LocalPositionZ(this Transform transform, float z)
	{
		Vector3 pos = transform.localPosition;
		pos.z = z;
		transform.localPosition = pos;
	}


	public static void LocalScaleX(this Transform transform, float x)
	{
		Vector3 pos = transform.localScale;
		pos.x = x;
		transform.localScale = pos;
	}
	public static void LocalScaleY(this Transform transform, float y)
	{
		Vector3 pos = transform.localScale;
		pos.y = y;
		transform.localScale = pos;
	}
	public static void LocalScaleZ(this Transform transform, float z)
	{
		Vector3 pos = transform.localScale;
		pos.z = z;
		transform.localScale = pos;
	}

#endregion

}
