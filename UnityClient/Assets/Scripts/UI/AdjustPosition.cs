using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AdjustPosition : MonoBehaviour
{
	public enum ScreenAlignment
	{
		TopLeft = 0,
		Top,
		TopRight,
		Left,
		Center,
		Right,
		BottomLeft,
		Bottom,
		BottomRight
	}
	
	public ScreenAlignment m_align = ScreenAlignment.Center;
	public Vector2 m_offset = Vector2.zero;
	public bool m_isResponseOnStart = true;
	public bool m_response = false;
	
	void Start()
	{
		if (m_isResponseOnStart)
		{
			response();
		}
	}
	
	void Update()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			response();
		}
		
		if (m_response)
		{
			m_response = false;
			response();
		}
	}
	
	public void response()
	{
		UIRoot root = NGUITools.FindInParents<UIRoot>(this.gameObject);
		
		Vector2 screenSize = Vector2.zero;
		if (null != root)
		{
 			//if (root.automatic)
 			//{
 			//	screenSize = new Vector2(Screen.width, Screen.height);
 			//}
			//else
			{
				int width = root.manualHeight * Screen.width / Screen.height;
				screenSize = new Vector2(width, root.manualHeight);
			}
		}
		else
		{
			Debug.LogError("The Game Object <" + gameObject.name + "> Not Attach UIRoot.");
		}
		
		Vector3 localPos = transform.localPosition;
		switch (m_align)
		{
		case ScreenAlignment.TopLeft:
			localPos.x = m_offset.x - screenSize.x / 2;
			localPos.y = screenSize.y / 2 - m_offset.y;
			break;
			
		case ScreenAlignment.Top:
			localPos.x = m_offset.x;
			localPos.y = screenSize.y / 2 - m_offset.y;
			break;
			
		case ScreenAlignment.TopRight:
			localPos.x = screenSize.x / 2 - m_offset.x;
			localPos.y = screenSize.y / 2 - m_offset.y;
			break;
			
		case ScreenAlignment.Left:
			localPos.x = m_offset.x - screenSize.x / 2;
			localPos.y = m_offset.y;
			break;
			
		case ScreenAlignment.Center:
			localPos.x = m_offset.x;
			localPos.y = m_offset.y;
			break;
			
		case ScreenAlignment.Right:
			localPos.x = screenSize.x / 2 - m_offset.x;
			localPos.y = m_offset.y;
			break;
			
		case ScreenAlignment.BottomLeft:
			localPos.x = m_offset.x - screenSize.x / 2;
			localPos.y = m_offset.y - screenSize.y / 2;
			break;
			
		case ScreenAlignment.Bottom:
			localPos.x = m_offset.x;
			localPos.y = m_offset.y - screenSize.y / 2;
			break;
			
		case ScreenAlignment.BottomRight:
			localPos.x = screenSize.x / 2 - m_offset.x;
			localPos.y = m_offset.y - screenSize.y / 2;
			break;
			
		default:
			break;
		}
		transform.localPosition = localPos;
	}
}
