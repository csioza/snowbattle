using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour 
{
	public bool Show=false;

	void OnBecameInvisible()
	{
		Show=false;
	}
	
	void OnBecameVisible()
	{
		Show=true;
	}
}
