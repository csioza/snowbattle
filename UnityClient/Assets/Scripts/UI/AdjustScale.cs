using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AdjustScale : MonoBehaviour
{
	public enum Direction
	{
		Horizontal,
		Vertical,
	}
	
	public Direction m_direction = Direction.Horizontal;
	public int  m_leftOrUpPadding;
	public int  m_rightOrDownPadding;
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
            if (UIRoot.Scaling.Flexible == root.scalingStyle)
			{
				screenSize = new Vector2(Screen.width, Screen.height);
			}
			else
			{
				int width = root.manualHeight * Screen.width / Screen.height;
				screenSize = new Vector2(width, root.manualHeight);
			}
		}
		else
		{
			int width = 750 * Screen.width / Screen.height;
			screenSize = new Vector2(width, 750);
		}
		
		UIWidget widget = GetComponent<UIWidget>();
		UILabel   label = widget as UILabel;
		UISprite sprite = widget as UISprite;
		UIInput   input = GetComponent<UIInput>();
		UIPanel   panel = GetComponent<UIPanel>();
		if (null != label)
		{
			if (Direction.Horizontal == m_direction)
			{
				label.width = (int)screenSize.x - m_leftOrUpPadding - m_rightOrDownPadding;
			}
			else if (Direction.Vertical == m_direction)
			{
				label.maxLineCount = (int)((screenSize.y - m_leftOrUpPadding - m_rightOrDownPadding) / transform.localScale.y);
			}
		}
		else if (null != sprite)
		{
			Vector3 localScale = transform.localScale;
			if (Direction.Horizontal == m_direction)
			{
				localScale.x = screenSize.x - m_leftOrUpPadding - m_rightOrDownPadding;
			}
			else if (Direction.Vertical == m_direction)
			{
				localScale.y = screenSize.y - m_leftOrUpPadding - m_rightOrDownPadding;
			}
			transform.localScale = localScale;
		}
		else if (null != input)
		{
			BoxCollider boxCollider = GetComponent<BoxCollider>();
			Vector3 center = boxCollider.center;
			Vector3 size = boxCollider.size;
			if (Direction.Horizontal == m_direction)
			{
				size.x = screenSize.x - m_leftOrUpPadding - m_rightOrDownPadding;
				center.x = size.x / 2.0f;
			}
			else if (Direction.Vertical == m_direction)
			{
				size.y = screenSize.y - m_leftOrUpPadding - m_rightOrDownPadding;
			//	center.y = size.y / 2.0f;
			}
			boxCollider.center = center;
			boxCollider.size = size;
		}
		else if (null != panel)
		{
            Vector4 clipRange = panel.baseClipRegion;
			if (Direction.Horizontal == m_direction)
			{
				clipRange.z = screenSize.x - m_leftOrUpPadding - m_rightOrDownPadding;
				clipRange.x = clipRange.z / 2.0f;
			}
			else if (Direction.Vertical == m_direction)
			{
				clipRange.w = screenSize.y - m_leftOrUpPadding - m_rightOrDownPadding;
				clipRange.y = -clipRange.w / 2.0f;
			}
            panel.baseClipRegion = clipRange;
		}
		else if (null != GetComponent<Collider>())
		{
			BoxCollider boxCollider = GetComponent<Collider>() as BoxCollider;
			Vector3 center = boxCollider.center;
			Vector3 size = boxCollider.size;
			if (Direction.Horizontal == m_direction)
			{
				size.x = screenSize.x - m_leftOrUpPadding - m_rightOrDownPadding;
				center.x = size.x / 2.0f;
			}
			else if (Direction.Vertical == m_direction)
			{
				size.y = screenSize.y - m_leftOrUpPadding - m_rightOrDownPadding;
				center.y = size.y / 2.0f;
			}
			boxCollider.center = center;
			boxCollider.size = size;
		}
	}
}
