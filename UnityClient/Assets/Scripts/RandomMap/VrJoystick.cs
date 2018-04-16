using UnityEngine;
using System.Collections;

public class VrJoystick : MonoBehaviour
{
	public  static VrJoystick VrJoy;
	
	public  Camera      UICamere;
	public  Vector2     VJRvector;          // Joystick's controls in Screen-Space.
	public  Vector2     VJRnormals;         // Joystick's normalized controls.
	
	public GameObject   JoyOBJ;          // joystick Object
	public GameObject   BackOBJ;         // Background's Object.
	public GameObject   BackZoneOBJ;     // Background Zone's Object.
	public GameObject   BackLightOBJ;    // Background Light's Object.
	
	private Color       ActiveColor;	// Joystick's color when active.
	private Color       InActiveColor;	// Joystick's color when inactive.
	private Color       InBoundColor;	// Joystick's color in BoundZone.

	private float       Length;			// The maximum distance the Joystick can be pushed.
//	private float       TapTimer;		// Double-tap's timer.
	
	private int         PickFirst;
	
	private bool        TouchDevice;
	
	public  bool        InPick;
	
	public  bool        Bt01Pick;
	public  bool        Bt02Pick;
	
	public  bool        BottomBt01Pick;
	public  bool        BottomBt02Pick;
	public  bool        BottomBt03Pick;
	public  bool        BottomBt04Pick;
	
	public  GameObject  Bt01Obj;
	public  GameObject  Bt02Obj;
	
	public  GameObject  BottomBt01Obj;
	public  GameObject  BottomBt02Obj;
	public  GameObject  BottomBt03Obj;
	public  GameObject  BottomBt04Obj;

	public  Vector2     PickScenePoint;

	private RaycastHit  hit;
	
	public void Reset()
	{
	  JoyOBJ.transform.localPosition=new Vector3(0,0,0);
	  BackOBJ.transform.localPosition=new Vector3(0,0,0);
	  BackLightOBJ.transform.localPosition=new Vector3(0,0,0);
	  ResetJoystick();
	}
	
	private void ResetJoystick() 
	{
		VJRvector.x=0;
		VJRvector.y=0;
		VJRnormals.x = 0;
		VJRnormals.y = 0;
//		TapTimer = 0.3f;
	    PickFirst=-1;
		InPick=false;
		JoyOBJ.transform.position=BackOBJ.transform.position;
		JoyOBJ.GetComponent<UISprite>().color = InActiveColor;
		BackOBJ.GetComponent<UISprite>().color = InActiveColor;
		BackLightOBJ.GetComponent<UISprite>().color = InActiveColor;
		BackZoneOBJ.GetComponent<UISprite>().color = InBoundColor;
	}
	
	void Awake()
	{
		VrJoy = this;
		hit=new RaycastHit();
		if((Application.platform == RuntimePlatform.IPhonePlayer)|| (Application.platform == RuntimePlatform.Android))
		{
		 TouchDevice=true;
		}
		else
		{
	     TouchDevice=false;
		}
		
		VJRvector  = new Vector2(0,0);
		VJRnormals = new Vector2(0,0);
		ActiveColor   = new Color(0.8f,0.8f,0.8f,0.4f);
		InActiveColor = new Color(0.6f,0.6f,0.6f,0.2f);
		InBoundColor  = new Color(0.8f,0.8f,0.8f,0.6f);

		JoyOBJ.GetComponent<UISprite>().color = InActiveColor;
		BackOBJ.GetComponent<UISprite>().color = InActiveColor;
		BackLightOBJ.GetComponent<UISprite>().color = InActiveColor;
		BackZoneOBJ.GetComponent<UISprite>().color = InBoundColor;
//		TapTimer = 0;
		ResetJoystick();
	    PickFirst=-1;
		InPick=false;
		
	    Bt01Pick=false;
	    Bt02Pick=false;
		
	    BottomBt01Pick=false;
	    BottomBt02Pick=false;
	    BottomBt03Pick=false;
	    BottomBt04Pick=false;
		
		PickScenePoint= new Vector2(0,0);
		Length=gameObject.GetComponent<SphereCollider>().radius;
		

	    Bt01Obj.SetActive(false);
	    Bt02Obj.SetActive(false);
	    BottomBt01Obj.SetActive(false);
	    BottomBt02Obj.SetActive(false);
	    BottomBt03Obj.SetActive(false);
	    BottomBt04Obj.SetActive(false);
	}

	void Start()
	{
	   Reset();
	   Bt01Obj.SetActive(true);
	   Bt02Obj.SetActive(true);
	}

	private Vector3 GetRadius(Vector3 midPoint, Vector3 endPoint, float maxDistance,out float Dist) 
	{
		Dist=Vector3.Distance(midPoint,endPoint);
		if (Dist > maxDistance)
		{
			Vector2 midPoint2=new Vector2(midPoint.x,midPoint.y);
			Vector2 endPoint2=new Vector2(endPoint.x,endPoint.y);
			Vector2 distance2= endPoint2-midPoint2;
			
			distance2.Normalize();
			distance2=distance2*maxDistance+midPoint2;
			return new Vector3(distance2.x,distance2.y,endPoint.z);
		}
		return endPoint;
	}

	private Vector2 GetControls(Vector3 midPoint, Vector3 endPoint)
	{
		Vector2 vector = new Vector3();
		
		if (Vector3.Distance(midPoint,endPoint) > 0) 
		{
			vector = new Vector2(endPoint.x-midPoint.x,endPoint.y-midPoint.y);
		}
		
		return vector;
	}
	
	private void TouchFreeJoy()
	{
		if(PickFirst>=0 )
		{
           if(Input.touchCount>0)
		   {
			float dist=0.0f;
		    if(Physics.Raycast(UICamere.ScreenPointToRay(Input.GetTouch(0).position),out hit,Length))
			{
			  JoyOBJ.transform.position=GetRadius(BackOBJ.transform.position,hit.point,0.2f,out dist);
			}

			VJRvector = GetControls(BackOBJ.transform.localPosition,JoyOBJ.transform.localPosition); 
			VJRnormals.x = VJRvector.x/(Length*0.4f);
			VJRnormals.y = VJRvector.y/(Length*0.4f);
			
		    float angela =Vector2.Angle(new Vector3(0,1),VJRvector);
			
	        if(VJRvector.x>0.0f)
	        {
		     angela=(180-angela)+180.0f;
	        }
		
	        if(angela>=360.0f){angela=0.0f;}
			
			BackLightOBJ.transform.eulerAngles=new Vector3(0,0,angela);//(angela,0,0);
			
			BackLightOBJ.GetComponent<UISprite>().color = new Color(1,1,1,dist/0.2f);
	       }				
		}
		
		/*
		if (TapTimer > 0) 
		{
			TapTimer -= Time.deltaTime;	
		}

		
		for(int i=0;i<Input.touchCount;i++)
		{
		  if(Input.GetTouch(i).phase == TouchPhase.Ended)
		  {
			 ResetJoystick();
		  }


		  if(Input.GetTouch(i).phase == TouchPhase.Began)
		  {
			if(GetPickInZone(Input.GetTouch(i).position,BoundTouchPoint,BoundTouchradius*1.5f))
			{		
			 Origin = GetRadius(BoundTouchPoint,Input.mousePosition,BoundTouchradius);					
		     Position.x = Origin.x;		
	         Position.y = Origin.y;	
							
		     Joystick.color = ActiveColor;
				
		     GetConstraints();	
	         PickFirst=i;		
			}
			else
			{
			  InPick=true;
		      PickScenePoint= new Vector2(Input.GetTouch(i).position.x,Input.GetTouch(i).position.y);
			}
		  }
			
		  if(Input.GetTouch(i).phase == TouchPhase.Stationary|| Input.GetTouch(i).phase == TouchPhase.Moved)
		  {		
		    if(PickFirst==i)
			{
			 Position = GetRadius(Origin,Input.GetTouch(i).position,Length);
			 VJRvector = GetControls(Position,Origin);
				
			 VJRnormals.x = VJRvector.x/Length;
			 VJRnormals.y = VJRvector.y/Length;
			
			 if(TapTimer <= 0) 
		     {
			  if (Background.color != ActiveColor)
			  {
				Background.color = ActiveColor;
			  }
		     }
			}
		  }
		}
		*/
	}

	private void FreeJoy()
	{
		if(PickFirst>=0 )
		{
			float dist=0.0f;
		    if(Physics.Raycast(UICamere.ScreenPointToRay(Input.mousePosition),out hit,Length))
			{
			  JoyOBJ.transform.position=GetRadius(BackOBJ.transform.position,hit.point,0.2f,out dist);
			}
			
			VJRvector = GetControls(BackOBJ.transform.localPosition,JoyOBJ.transform.localPosition); 
			VJRnormals.x = VJRvector.x/(Length*0.4f);
			VJRnormals.y = VJRvector.y/(Length*0.4f);
			
		    float angela =Vector2.Angle(new Vector3(0,1),VJRvector);
			
	        if(VJRvector.x>0.0f)
	        {
		     angela=(180-angela)+180.0f;
	        }
		
	        if(angela>=360.0f){angela=0.0f;}
			
			BackLightOBJ.transform.eulerAngles=new Vector3(0,0,angela);//(angela,0,0);
			
			BackLightOBJ.GetComponent<UISprite>().color = new Color(1,1,1,dist/0.2f);
		}
	}

	void OnPress(bool Pressed) 
	{	
	   if(Pressed)
	   {
		 InPick=true;
	     if(TouchDevice==true)
	     {
          if(Input.touchCount>0)
		  {
		    Physics.Raycast(UICamere.ScreenPointToRay(Input.GetTouch(0).position),out hit,Length);
		  }
		 }
		 else
		 {
		  Physics.Raycast(UICamere.ScreenPointToRay(Input.mousePosition),out hit,Length);				
		 }
		 JoyOBJ.transform.position=hit.point;
		 BackOBJ.transform.position=hit.point;
		 BackLightOBJ.transform.position=hit.point;
		 JoyOBJ.GetComponent<UISprite>().color = ActiveColor;
		 BackOBJ.GetComponent<UISprite>().color = ActiveColor;

		 PickFirst=0; 
	   }
	}
	
	void OnB1Press() 
	{
      Bt01Pick=true;
	}
	
	void OnB2Press() 
	{
      Bt02Pick=true;
	}
	
	void OnB1Up() 
	{
      Bt01Pick=false;
	}
	
	void OnB2Up() 
	{
      Bt02Pick=false;
	}
	
	void OnBottomB1Press() 
	{
      BottomBt01Pick=true;
	}
	
	void OnBottomB1Up() 
	{
      BottomBt01Pick=false;
	}
	
	void OnBottomB2Press() 
	{
      BottomBt02Pick=true;
	}
	
	void OnBottomB2Up() 
	{
      BottomBt02Pick=false;
	}

	void OnBottomB3Press() 
	{
      BottomBt03Pick=true;
	}
	
	void OnBottomB3Up() 
	{
      BottomBt03Pick=false;
	}
	
	void OnBottomB4Press() 
	{
      BottomBt04Pick=true;
	}
	
	void OnBottomB4Up() 
	{
      BottomBt04Pick=false;
	}
	
	void Update()
	{
      if(TouchDevice==true)
	  {
       if(Input.touchCount>0)
	   {
	    if(Input.GetTouch(0).phase==TouchPhase.Ended)
	    {
	     ResetJoystick();
	    }			
	   }
	   else
	   {
         ResetJoystick();	
	   }
	  }
	  else
	  {
	   if(Input.GetMouseButtonUp(0))
	   {
	    ResetJoystick();
	   } 			
	  }

	  if(Bt01Pick)
	  {
			
	  }
		
	  if(Bt02Pick)
	  {
			
	  }
		
	  if(BottomBt01Pick)
	  {
	  }
		
	  if(BottomBt02Pick)
	  {
			
	  }
		
	  if(BottomBt03Pick)
	  {
			
	  }
		
	  if(BottomBt04Pick)
	  {
			
	  }
		
	  if(InPick)
	  {
        if(TouchDevice==true)
	    {
         TouchFreeJoy();
	    }
	    else
	    {
         FreeJoy();
	    }
	  }
      else
	  {
        if(TouchDevice==true)
	    {
         if(Input.touchCount>0)
	     {
	      if(Input.GetTouch(0).phase==TouchPhase.Began)
	      {
	       PickScenePoint= new Vector2(Input.GetTouch(0).position.x,Input.GetTouch(0).position.y);
	      }			
	     }
	     else
	     {
           PickScenePoint= new Vector2(Input.mousePosition.x,Input.mousePosition.y);
	     }
	    }
	    else
	    {
	     if(Input.GetMouseButtonDown(0))
	     {
	      PickScenePoint= new Vector2(Input.mousePosition.x,Input.mousePosition.y);
	     } 			
	    }			
	  }
	}
}