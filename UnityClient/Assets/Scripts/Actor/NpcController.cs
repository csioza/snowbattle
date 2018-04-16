using UnityEngine;
using System.Collections;

//如果NPC是由玩家控制的,就添加一个controller
public class NpcController
{
	public Actor controlActor { get; set; }
	public void SendMoveTo(int npcID, Vector3 targetPos)
	{
	}

	public void SendFireSkill(NPC thisNpc,int skillID, Vector3 direction)
	{
        /*modify by luozj
		StartSkillActionPacket packet = new StartSkillActionPacket(skillID, thisNpc.RealPos.x, thisNpc.RealPos.z, direction.x, direction.z);
		ClientNet.Singleton.SendPacket(packet);*/
	}
}
