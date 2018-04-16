using UnityEngine;
using System.Collections;

public class AISwitchPlayer : AIBase
{
    float m_enterTimer = 0;//入场时间
    float m_exitDuration = float.MinValue;//从入场到退场的间隔
    public override void Update() 
    {
        Player self = Owner as Player;
        if (null != self.CurrentCmd)
        {
            switch ((ENSwitchStep)BattleArena.Singleton.SwitchStep)
            {
                case ENSwitchStep.enNone:
                    {
                        JumpinAction action = self.ActionControl.AddAction(ActorAction.ENType.enJumpinAction) as JumpinAction;
                        if (null == action)
                        {
                            Debug.LogWarning("JumpinAction add fail");
                        }
                        else
                        {
                            BattleArena.Singleton.SwitchStep = ENSwitchStep.enJumpin;
							m_enterTimer = Time.time;
                        }
                    }
                    break;
                case ENSwitchStep.enJumpin:
                    {
                        if (!self.ActionControl.IsActionRunning(ActorAction.ENType.enJumpinAction))
                        {//入场action已完成，释放切入技
                            float now = Time.time;
                            if (m_enterTimer == 0.0f)
                            {
                                m_enterTimer = now;
                            }
                            else
                            {
                                if (m_exitDuration == float.MinValue)
                                {
                                    WorldParamInfo info = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enSwitchExitTimer);
                                    if (info != null)
                                    {
                                        m_exitDuration = info.FloatTypeValue;
                                    }
                                }
                                if (now - m_enterTimer > m_exitDuration)
                                {
                                    BattleArena.Singleton.SwitchStep = ENSwitchStep.enJumpout;
                                    return;
                                }
                            }
                            //直接释放技能
                            if (ActionTryAttack(self.CurrentCmd.m_skillID, self.TargetManager.CurrentTarget))
                            {
                                BattleArena.Singleton.SwitchStep = ENSwitchStep.enAttack;
                            }
                            //GetRangeTargetList(ENTargetType.enEnemy);
                            //m_curTargetID = Owner.TargetManager.CurrentTarget.ID;
                            //if (ActionTryFireSkill(self.CurrentCmd.m_skillID))
                            //{
                            //    self.SwitchStep = Player.ENSwitchStep.enAttack;
                            //}
                        }
                    }
                    break;
                case ENSwitchStep.enAttack:
                    {
                        if (!self.ActionControl.IsActionRunning(ActorAction.ENType.enAttackAction))
                        {//攻击action已完成
                            JumpoutAction action = self.ActionControl.AddAction(ActorAction.ENType.enJumpoutAction) as JumpoutAction;
                            if (null == action)
                            {
                                Debug.LogWarning("JumpoutAction add fail");
                            }
                            else
                            {
                                BattleArena.Singleton.SwitchStep = ENSwitchStep.enJumpout;
                            }
                        }
                    }
                    break;
                case ENSwitchStep.enJumpout:
                    {
                        if (!self.ActionControl.IsActionRunning(ActorAction.ENType.enJumpoutAction))
                        {//退场action已完成
                            self.CurrentCmd = null;
                            self.HideMe();
                            BattleArena.Singleton.SwitchStep = ENSwitchStep.enNone;
                            //将切入技角色从ActorManager列表中删除，但不销毁
                            ActorManager.Singleton.ReleaseActor(self.ID, false);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
