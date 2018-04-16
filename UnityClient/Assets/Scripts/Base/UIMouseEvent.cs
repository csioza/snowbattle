using UnityEngine;
using System;

public abstract class UIMouseEvent : MonoBehaviour
{
	public EventHandler<EventArgs> MouseEvent { get; set; }
}