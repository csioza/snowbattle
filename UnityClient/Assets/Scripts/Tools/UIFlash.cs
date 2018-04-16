using UnityEngine;
using System.Collections;

public class UIFlash : MonoBehaviour
{
	public GameObject FirstObj;
	public GameObject SecondObj;
	public float FlashTime;
	private float m_startTime;
	// Use this for initialization
	void Start ()
	{
		m_startTime = Time.fixedTime;
		FirstObj.SetActive(true);
		SecondObj.SetActive(false);
	}
	
	void FixedUpdate()
	{
		if (m_startTime + FlashTime < Time.fixedTime)
		{
			FirstObj.SetActive(!FirstObj.activeSelf);
			SecondObj.SetActive(!SecondObj.activeSelf);
			m_startTime = Time.fixedTime;
		}
	}
}
