using UnityEngine;
using System;

//Actor的类型标示符，仅做记录数据使用，代码生成
public class ActorProp : MonoBehaviour
{
	public ActorType Type;
	public int ID;
	public Actor ActorLogicObj { get; set; }
}