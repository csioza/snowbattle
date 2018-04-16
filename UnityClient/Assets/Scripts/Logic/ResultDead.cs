using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NGE.Network;

public class ResultDead : IResult
{
    public bool m_isRelive = false;
    //int m_currentCorroutineIndex = 0;
    public bool m_offExec = false;

    
    public ResultDead()
		:base((int)ENResult.Dead)
	{
        m_isRelive = false;
        m_offExec = false;
	}
    public static IResult CreateNew()
    {
        return new ResultDead();
    }

    public override void Deserialize(PacketReader stream)
    {
        base.Deserialize(stream);
        m_isRelive = (stream.ReadInt32()==1)?true:false;
        m_offExec = (stream.ReadInt32() == 1) ? true : false;
    }


    public override void Exec(IResultControl control)
    {
        if (m_offExec)
        {
            return;
        }
        base.Exec(control);
        Actor actor = ActorManager.Singleton.Lookup(TargetID);
        if (null == actor)
        {
            return;
        }
        actor.Props.SetProperty_Int32(ENProperty.islive, 0);
        {//clear
            if (actor.Type == ActorType.enMain)
            {
                MainPlayer player = actor as MainPlayer;
                //清除命令
                player.CurrentCmd = null;
                //通知头像
                player.NotifyChanged((int)Actor.ENPropertyChanged.enMainHead, EnMainHeadType.enActorDead);
            }
            else if (actor.Type == ActorType.enNPC)
            {
                NPC npc = actor as NPC;
                //隐藏警戒
                npc.HideWarnTip();
                //恢复优先攻击标记
                npc.m_priorityAttack = false;
                if (npc.GetNpcType() != ENNpcType.enBOSSNPC)
                {
                    //npc.EnableCollider(false);
                }
                else
                {
                    if (!m_isRelive)
                    {
                        ++BattleArena.Singleton.KillBossCount;
                    }
                    //Debug.LogWarning("boss dead, source id:" + SourceID + ",self id:" + TargetID);
                }
                Actor source = ActorManager.Singleton.Lookup(SourceID);
                if (source != null && source.Type == ActorType.enMain)
                {//只增加主控角色击杀的npc
                    //增加击杀npc的数量
                    ++BattleArena.Singleton.KillNpcCount;
                }
                //清除命令
                npc.CurrentCmd = null;

                //恢复强韧度
                npc.Props.SetProperty_Float(ENProperty.stamina, npc.Props.GetProperty_Float(ENProperty.maxStamina));
                
                //test
                npc.TestBossStamina();

                // 掉落表现 开始
                DropItemPerformance.Singleton.Start(npc);

                if (!m_isRelive)
                {//清除目标
                    actor.TargetManager.CurrentTarget = null;
                }
            }

            //连击数清零
            actor.ComboClear();

            //清除buffer
            actor.MyBuffControl.RemoveAllForDead(control);
        }
        {//dead action
            //是否真的死亡
            //actor.IsRealDead = !m_isRelive;
            if (!m_isRelive)
            {
                actor.OnDead();
            }
            
            //通知所有人自己死亡
            ActorManager.Singleton.NotifyAll_ClearTarget(actor.ID);

            DeadAction action = actor.ActionControl.AddAction(ActorAction.ENType.enDeadAction) as DeadAction as DeadAction;
            if (null != action)
            {
                action.Init(m_isRelive);
            }
        }
    }

    public override void ResultServerExec(IResultControl control)
    {
        if (m_offExec)
        {
            return;
        }

        base.ResultServerExec(control);
        Actor targetActor = ActorManager.Singleton.Lookup(TargetID);
        if (null == targetActor)
        {
            return;
        }
        Actor source = ActorManager.Singleton.Lookup(SourceID);

      

        targetActor.Props.SetProperty_Int32(ENProperty.islive, 0);
        {//clear
            if (targetActor.Type == ActorType.enMain)
            {
                MainPlayer player = targetActor as MainPlayer;
                //清除命令
                player.CurrentCmd = null;
                //通知头像
                player.NotifyChanged((int)Actor.ENPropertyChanged.enMainHead, EnMainHeadType.enActorDead);

                // 目标如果是自己 则 显示自己被谁杀死
                if (null == source)
                {
                    return;
                }
                if (source.Type ==  ActorType.enPlayer)
                {
                    string tips = "[ff0000] " + source.Name + "[-] 杀死了你! ";
                    KillTips.Singleton.Update(tips);
                }
                
                
            }
            else if (targetActor.Type == ActorType.enNPC)
            {
                NPC npc = targetActor as NPC;
                //隐藏警戒
                npc.HideWarnTip();
                //恢复优先攻击标记
                npc.m_priorityAttack = false;
                if (npc.GetNpcType() != ENNpcType.enBOSSNPC)
                {
                    //npc.EnableCollider(false);
                }
                else
                {
                    if (!m_isRelive)
                    {
                        ++BattleArena.Singleton.KillBossCount;
                    }
                    //Debug.LogWarning("boss dead, source id:" + SourceID + ",self id:" + TargetID);
                }
               
                if (source != null && source.Type == ActorType.enMain)
                {//只增加主控角色击杀的npc
                    //增加击杀npc的数量
                    ++BattleArena.Singleton.KillNpcCount;

                }
                //清除命令
                npc.CurrentCmd = null;

                //恢复强韧度
                npc.Props.SetProperty_Float(ENProperty.stamina, npc.Props.GetProperty_Float(ENProperty.maxStamina));

                //test
                npc.TestBossStamina();

                // 掉落表现 开始
                DropItemPerformance.Singleton.Start(npc);

                if (!m_isRelive)
                {//清除目标
                    targetActor.TargetManager.CurrentTarget = null;
                }
            }
            else if (targetActor.Type == ActorType.enPlayer)
            {
               // Debug.Log("targetActor.Type == ActorType.enPlayer:" + targetActor.Name);
                 if (source != null && source.Type == ActorType.enMain)
                {
                    // 源是自己 则 自己显示 杀死了谁
                    string tips = "你杀死了 [ff0000]" + targetActor.Name + "[-]!";
                    KillTips.Singleton.Update(tips);
                }
            }

            //连击数清零
            targetActor.ComboClear();

            //清除buffer
            targetActor.MyBuffControl.RemoveAllForDead(control);
        }
        {//dead action
            //是否真的死亡
            //actor.IsRealDead = !m_isRelive;
            if (!m_isRelive)
            {
                targetActor.OnDead();
            }

            //通知所有人自己死亡
            ActorManager.Singleton.NotifyAll_ClearTarget(targetActor.ID);
			//设置头顶血条
			targetActor.SetCurHP(0);
            DeadAction action = targetActor.ActionControl.AddAction(ActorAction.ENType.enDeadAction) as DeadAction as DeadAction;
            if (null != action)
            {
                action.Init(m_isRelive);
            }
        }
    }


}
