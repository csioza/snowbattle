using UnityEngine;
using System.Collections;

public class UVAnimation : MonoBehaviour 
{
	private float UVHorizontal;
	private float UVVertical;
	private float CurrentUVH;
	private float CurrentUVV;
	private bool Inv;
	// Use this for initialization
	public void SetUVType(float Horizontal,float Vertical)
	{
		UVHorizontal=Horizontal;
		UVVertical=Vertical;

		CurrentUVH=0.0f;
		CurrentUVV=0.0f;
		Inv=false;
	}

	void OnBecameInvisible()
	{
		Inv=false;
		GetComponent<Renderer>().enabled=false;
	}
	
	void OnBecameVisible()
	{
		Inv=true;
		GetComponent<Renderer>().enabled=true;
	}

	// Update is called once per frame
	void Update () 
	{
		if(Inv==false){return;}

		if(gameObject.GetComponent<Renderer>().material!=null)
		{
			CurrentUVH+=UVHorizontal*Time.deltaTime;
			CurrentUVV+=UVVertical*Time.deltaTime;
			if(CurrentUVH>1.0f){CurrentUVH=0.0f;}
			if(CurrentUVV>1.0f){CurrentUVV=0.0f;}
			gameObject.GetComponent<Renderer>().material.SetFloat("Hani",CurrentUVH);
			gameObject.GetComponent<Renderer>().material.SetFloat("Vani",CurrentUVV);
		}
	}
}
