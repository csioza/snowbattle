using UnityEngine;
using System.Collections;

public class GrassAnimation : MonoBehaviour 
{
	private float TimeRange;
	private float RotateRange;
	private bool  Grass;
	private Vector3 LocalScale;
	private bool Show;

	// Use this for initialization
	public void SetGrassType(bool GrassT)
	{
		Grass=GrassT;
		Show=false;

		TimeRange=3.0f+UnityEngine.Random.Range(-0.5f,0.5f);
		RotateRange=2.5f+UnityEngine.Random.Range(-1.0f,1.0f);
		
		LocalScale=new Vector3();
		LocalScale.x=gameObject.transform.localScale.x;
		LocalScale.y=gameObject.transform.localScale.y;
		LocalScale.z=gameObject.transform.localScale.z;
		
		gameObject.transform.eulerAngles=new Vector3(0,Camera.main.transform.eulerAngles.y,0);
	}
		
	void OnBecameInvisible()
	{
		Show=false;
	}
	
	void OnBecameVisible()
	{
		Show=true;
	}

	// Update is called once per frame
	void Update () 
	{
		if(Show==false){return;}

		float Change=Mathf.Sin(Time.time*TimeRange)*RotateRange;
		if(Grass==true)
		{
			gameObject.transform.eulerAngles=new Vector3(0,Camera.main.transform.eulerAngles.y,Change);
		}
		else
		{
			transform.rotation = Camera.main.transform.rotation;
			gameObject.transform.localScale = new Vector3(LocalScale.x+Change*0.05f,LocalScale.y+Change*0.05f,LocalScale.z+Change*0.05f);
		}
	}
}
