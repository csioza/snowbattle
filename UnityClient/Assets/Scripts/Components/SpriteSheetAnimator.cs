using UnityEngine;
using System.Collections;






public class SpriteSheetAnimator : MonoBehaviour
{ 
	//vars for the whole sheet
	public int colCount =  5;
	public int rowCount =  5; 
	//vars for animation
	public int totalCells = 24;
	public int hold = 0;
	public float delay =0.0f;
	public int  fps     = 12;
	public bool destoryOnEnd = true;
	public bool debug = false;
	
	private int colNumber = 0; //Zero Indexed
	private int  rowNumber  =  0; //Zero Indexed
	private Vector2 offset;

	private float time = 0;
	
	void Start(){
		//hold must be a multiple of the totalCells
		hold = (totalCells*hold);
	}
	void Awake()
	{
		time = 0.0f;
	}
	//Update
	void Update () { 

		time += Time.deltaTime;
		int modTime = (int)(time * (fps + delay)) % (totalCells + hold);
		
		if (modTime > hold){
			SetSpriteAnimation(colCount,rowCount,rowNumber,colNumber,totalCells,fps); 
		}
	}

	//SetSpriteAnimation
	void SetSpriteAnimation(int colCount ,int rowCount ,int rowNumber ,int colNumber,int totalCells,int fps ){
		
		// Calculate index
		int index  = (int)(time * (fps + delay));
		if(debug) Debug.Log (index);
		if(destoryOnEnd && index >= totalCells) {
			time=0.0f;
			GameObject.Destroy(gameObject);
		}

		// Repeat when exhausting all cells
		index = index % totalCells;
		
		// Size of every cell
		float sizeY = 1.0f / rowCount;
		float sizeX = 1.0f / colCount;
		
		Vector2 size =  new Vector2(sizeX,sizeY);
		
		// split into horizontal and vertical index
		var uIndex = index % colCount;
		var vIndex = index / colCount;
		
		// build offset
		// v coordinate is the bottom of the image in opengl so we need to invert.
		float offsetX = (uIndex+colNumber) * size.x;
		float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
		Vector2 offset = new Vector2(offsetX,offsetY);
		
		GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", offset);
		GetComponent<Renderer>().material.SetTextureScale  ("_MainTex", size);
	}
}