using UnityEngine;
using System.Collections;

public class PlayerCtrl : MonoBehaviour 
{
	SceneEffect SceneE;
	
	public  GameObject MainPlayer;
	
	float CharactorSpeed=3.0f;

	bool Reset=false;

	void Start () 
	{
	   SceneE=GetComponent<SceneEffect>();
	   SceneE.InitSceneObject();

	   MainPlayer.name="MainPlayer";
	   MainPlayer.transform.position=SceneE.SceneNpcPos.StartPos;
	   Camera.main.transform.position=new Vector3(MainPlayer.transform.position.x+3.5f,MainPlayer.transform.position.y+5.0f,MainPlayer.transform.position.z+3.5f);
	   Camera.main.transform.eulerAngles=new Vector3(45.0f,225.0f,0);
	}
	
	// Update is called once per frame
	void Update() 
	{
	  if(VrJoystick.VrJoy.Bt01Pick==true)
	  {
	   if(Reset==false)
	   {
	    SceneE.InitSceneObject();
	    MainPlayer.transform.position=SceneE.SceneNpcPos.StartPos;
		Reset=true;
	   }
	  }
	  else
	  {
	   Reset=false;
	   if(VrJoystick.VrJoy.InPick==false && VrJoystick.VrJoy.Bt01Pick==false && VrJoystick.VrJoy.Bt02Pick==false && Input.GetMouseButtonDown(0))
	   {
		 SceneE.GetPathFind().StartFindPath(MainPlayer,VrJoystick.VrJoy.PickScenePoint);
		 //SceneE.GetPathFind().StartFindPath(MainPlayer,new Vector2(Input.mousePosition.x,Input.mousePosition.y));
	   }
		
		SceneE.GetPathFind().FindPathUpdate(MainPlayer,VrJoystick.VrJoy.VJRnormals,CharactorSpeed);
	  }
		
	  Camera.main.transform.position=new Vector3(MainPlayer.transform.position.x+3.5f,MainPlayer.transform.position.y+5.0f,MainPlayer.transform.position.z+3.5f);			
	}
}
