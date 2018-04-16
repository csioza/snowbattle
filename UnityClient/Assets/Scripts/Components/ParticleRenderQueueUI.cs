using UnityEngine;
using System.Collections;

public class ParticleRenderQueueUI : MonoBehaviour {
	public int renderQueue = 30000;  
	public bool runOnlyOnce = true;  
	// Use this for initialization
	void Start () {
		Update();
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<Renderer>() != null && GetComponent<Renderer>().sharedMaterial != null)  
		{  
			GetComponent<Renderer>().sharedMaterial.renderQueue = renderQueue;  
		}  
		if (runOnlyOnce && Application.isPlaying)  
		{  
			this.enabled = false;  
		}  
	}
}
