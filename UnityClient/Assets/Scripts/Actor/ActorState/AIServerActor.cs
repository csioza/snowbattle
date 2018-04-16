using UnityEngine;

//只处理服务器消息命令的ai
public class AIServerActor : AIBase
{
    public override void Update()
    {
        Actor.Cmd cmd = Owner.CurrentCmd;
        if (cmd != null)
        {
            switch (cmd.m_type)
            {
                case Actor.ENCmdType.enMove:
                    {
                        if (ActionMoveTo(cmd.m_moveTargetPos,true,false,cmd.IsSyncPosValidate,cmd.m_syncPos))
                        {
                            Owner.CurrentCmd = null;
                        }
                    }
                    break;
                case Actor.ENCmdType.enSkill:
                    {
                        Owner.CurrentTarget = ActorManager.Singleton.Lookup(cmd.m_targetID);
                        //ActionForwardTo(cmd.m_targetID);
                        if (ActionTryAttack(cmd.m_skillID, Owner.CurrentTarget,cmd.IsSyncPosValidate,cmd.m_syncPos))
                        {
                            Owner.CurrentCmd = null;
                        }
                        else
                        {
                            ActionForwardTo(cmd.m_targetID);
                        }
                    }
                    break;
				case Actor.ENCmdType.enRoll:
					{
						RollAction rollAction = Owner.ActionControl.AddAction(ActorAction.ENType.enRollAction) as RollAction;
						if (rollAction != null)
						{
                            rollAction.Init(Owner.CurrentCmd.m_syncPos, cmd.m_moveTargetPos);
							Owner.CurrentCmd = null;
						}
						break;
					}
				case Actor.ENCmdType.enActorEnter:
					{
						Player player = Owner as Player;
                        if (player != null)
                        {
                            if (player.SwitchActorEnter(cmd.m_syncPos, cmd.m_forward))
                            {
                                Owner.CurrentCmd = null;
                            }
                        }
                        else
                        {
                            Owner.CurrentCmd = null;
                        }
						break;
					}
				case Actor.ENCmdType.enActorExit:
					{
						Owner.IsActorExit = true;
						ActorExitAction actorExitAction = Owner.ActionControl.AddAction(ActorAction.ENType.enActorExitAction) as ActorExitAction;
						if (actorExitAction != null)
						{
							Owner.CurrentCmd = null;
						}
						break;
					}
				case Actor.ENCmdType.enInteruptAction:
					{
                        Owner.ActionControl.InterruptAction((ActorAction.ENType)cmd.m_interruptedActionType);
						Owner.CurrentCmd = null;
						break;
					}
				case Actor.ENCmdType.enBeHitAction:
					{ 
						BeAttackAction beAttackAction = Owner.ActionControl.AddAction(ActorAction.ENType.enBeAttackAction) as BeAttackAction;
						if (beAttackAction != null)
						{
							Actor srcActor = ActorManager.Singleton.Lookup(cmd.m_srcActorID);
							beAttackAction.Init(srcActor, cmd.m_isBack, cmd.m_isFly);
							Owner.CurrentCmd = null;
						}
						break;
					}
				case Actor.ENCmdType.enJumpInAction:
					{
						Owner.CurrentTarget = ActorManager.Singleton.Lookup(cmd.m_targetID);
						JumpinAction jumpInAction = Owner.ActionControl.AddAction(ActorAction.ENType.enJumpinAction) as JumpinAction;
						if (jumpInAction != null)
						{
							Owner.CurrentCmd = null;
						}
						break;
					}
				case Actor.ENCmdType.enJumpOutAction:
					{
						JumpoutAction jumpOutAction = Owner.ActionControl.AddAction(ActorAction.ENType.enJumpoutAction) as JumpoutAction;
						if (jumpOutAction != null)
						{
							Owner.CurrentCmd = null;
						}
						break;
					}
                case Actor.ENCmdType.enAttackingMoveAction:
                    {
                        AttackingMoveAction action = Owner.ActionControl.AddAction(ActorAction.ENType.enAttackingMoveAction) as AttackingMoveAction;
                        if (action != null)
                        {
                            action.InitImpl(Owner.CurrentCmd.m_skillID, Owner.CurrentCmd.m_targetID,cmd.IsSyncPosValidate,cmd.m_syncPos);
                            Owner.CurrentCmd = null;
                        }
                    }
                    break;
            }
        }
    }
}