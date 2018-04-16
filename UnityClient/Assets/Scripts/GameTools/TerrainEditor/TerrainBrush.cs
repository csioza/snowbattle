
using UnityEngine;
using System.Collections;

public class TerrainBrush : MonoBehaviour
{
	public Vector3	mDrawPos;
	

	// Use this for initialization
	void Start ()
	{
		mDrawPos = new Vector3(0.0f, 1.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	void FixedUpdate()
	{
	}
	
	// Draw the Brush
	void OnDrawGizmos()
	{
		//Gizmos.color= new Color(1.0f, 1.0f, 0.0f, 1.0f);
			
		//Vector3 szv = new Vector3(0.1f, 0.0f, 0.1f);
		//Gizmos.DrawWireCube(mDrawPos, szv);
		TexMixBrush.NtfDrawBrush(mDrawPos);
	}
	
}
