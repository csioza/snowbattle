using UnityEngine;
using System;

public abstract class ObjectCallback : MonoBehaviour
{
	public EventHandler<EventArgs> CallbackEvent { get; set; }
}