using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

public class MainPlayer : Player
{
    public bool m_autoAttackAll = false;
    public bool m_autoUseSkill = false;
    //切换角色后是否移动
    public bool IsMoveAfterSwitch = false;
    //soul
    public int SoulCount = 0;

    AIPlayer m_player = new AIPlayer();
    AIAutoBattle m_autoPlayer = new AIAutoBattle();

    //死亡通知
    public bool m_isDeadNotify = false;
	//死亡初始时间
	public float m_isDeadTime = 0f;

    //优先选择的目标类型
    public ActorType m_firstTargetType = ActorType.enNone;
    static public int SortByAttackRange(ActorSkillInfo infoA, ActorSkillInfo infoB)
    {
        if (infoA.SkillTableInfo.SkillType == (int)ENSkillType.enSkillNormalType)
        {//普攻在最后
            return 1;
        }
        if (infoB.SkillTableInfo.SkillType == (int)ENSkillType.enSkillNormalType)
        {//普攻在最后
            return -1;
        }
        return (int)(infoB.SkillTableInfo.AttackDistance - infoA.SkillTableInfo.AttackDistance);
    }
    public bool IsCanSelectTarget(Actor actor)
    {
        if (mCurRoomGUID == -1)
        {
            return false;
        }
        if (actor.Type == ActorType.enFollow)
        {//不能选中跟随角色
            return false;
        }
        if (!actor.IsCanSelected())
        {
            return false;
        }


        if (actor.Type == ActorType.enNPC)
        {
            NPC npc = actor as NPC;
            if (npc.GetNpcType() == ENNpcType.enBlockNPC)
            {
                return false;
            }
            if (npc.GetNpcType() != ENNpcType.enBoxNPC)
            {
                return true;
            }
			if (null != npc.CurRoom && !npc.CurRoom.IsBattleState())
            {
                return true;
            }
        }
        return true;
//         if (actor.Type == ActorType.enNPC)
//         {
//             NPC npc = actor as NPC;
//             if (npc.GetNpcType() == ENNpcType.enBlockNPC)
//             {
//                 return false;
//             }
//             if (null != npc.CurRoom)
//             {
//                 if (!npc.CurRoom.IsBattleState())
//                 {
//                     return true;
//                 }
//             }
//         }
//         return true;
    }
    public MainPlayer(ActorType type, int id, int staticID, CSItemGuid guid)
		:base(type, id, staticID,guid)
    {
        ItemBag = new CSItemBag();

        m_player.Owner = this;
        m_autoPlayer.Owner = this;
        SelfAI = m_player;

        SetPropertyObjectID((int)MVCPropertyID.enMainPlayer);

        Combo = BattleArena.Singleton.Combo;

	}
    //通知战斗ui的类型
    public enum ENBattleBtnNotifyType
    {
        enShowChange,//显示改变
        enSkillChange,//技能改变
        enAddSkillHighlight,//添加技能高亮
        enDelSkillHighlight,//删除技能高亮
        enTargetChanged,//目标改变
        enSkillEnabled,//可释放技能
        enQteShow,//显示qte
        enQteHide,//隐藏qte
        enUpdataSkillLevelUp,//更新技能升级UI
    }
    public void NotifyChangedButton(Actor actor)
    {
        if (actor == null)
        {//目标为空时，重置伤害来源
            DamageSource = null;
            StartCaution();
        }
        if (!IsActorExit)
        {
            NotifyChanged((int)ENPropertyChanged.enBattleBtn_D, ENBattleBtnNotifyType.enTargetChanged);
        }
    }
    public override void OnInitSkillBag(bool isNotifyUI = false)
    {
        base.OnInitSkillBag(isNotifyUI);
        //通知UIBattleBtn
        if (isNotifyUI)
        {
            NotifyChanged((int)ENPropertyChanged.enBattleBtn_D, ENBattleBtnNotifyType.enSkillChange);
        }

        //按攻击距离重新排列技能背包
        //this.SkillBag.Sort(SortByAttackRange);
    }
    //public void InitMainHead()
    //{
    //    NotifyChanged((int)Actor.ENPropertyChanged.enMainHead, EnMainHeadType.enInitActor);
    //}
    public override void Destroy()
    {
        base.Destroy();
        //this.NotifyChanged((int)Actor.ENPropertyChanged.enBattleBtn, MainPlayer.ENBattleBtnNotifyType.enDelSkillHighlight);
        //this.NotifyChanged((int)Actor.ENPropertyChanged.enBattleBtn_L, MainPlayer.ENBattleBtnNotifyType.enDelSkillHighlight);
        this.NotifyChanged((int)Actor.ENPropertyChanged.enBattleBtn_D, MainPlayer.ENBattleBtnNotifyType.enDelSkillHighlight);
    }
	float m_minRot = float.MaxValue;
	private void CheckTarget(Actor target)
	{
		Actor CurrentActor = this as Actor;
		if (CurrentActor == target)
		{
			return;
		}
		if (target.IsDead)
		{
			return;
		}
        if (!IsCanSelectTarget(target))
        {
            return;
        }
		if (!ActorTargetManager.IsEnemy(CurrentActor, target))
		{
            if (target.Type == ActorType.enNPC)
            {
                NPC npc = target as NPC;
                if (npc.GetNpcType() != ENNpcType.enBoxNPC)
                {
                    return;
                }
            }
            else
            {
                return;
            }
		}
        float distance = ActorTargetManager.GetTargetDistance(CurrentActor.RealPos, target.RealPos);
		//Vector3 targetDirection = target.RealPos - CurrentActor.RealPos;
		//targetDirection.y = 0.0f;
        WorldParamInfo info = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enChangeTargetDistance);
        if (info == null)
        {
            return;
        }
        //if (targetDirection.magnitude > info.FloatTypeValue)
        if (distance > info.FloatTypeValue)
		{
			return;
		}
		//targetDirection.Normalize();
		//Vector3 direction = CurrentActor.MainObj.transform.forward;
		
		//float yRot = Vector3.Angle(targetDirection, direction);
		//if (yRot < 360.0f)
		{
			CurrentActor.TargetManager.AddTarget(target);
			//if (yRot < m_minRot)
            if (distance < m_minRot)
			{
				if (1 == CurrentActor.TargetManager.GetTargetListValue(target))
				{
					return;
				}
				//m_minRot = yRot;
                m_minRot = distance;
				CurrentActor.TargetManager.CurrentTarget = target;
			}
		}
	}
	public bool ChangeTarget()
	{
		m_minRot = float.MaxValue;
		Actor lastActor = TargetManager.CurrentTarget;
		TargetManager.CurrentTarget = null;
		ActorManager.Singleton.ForEach(CheckTarget);
		if (TargetManager.CurrentTarget == null)
		{
			TargetManager.ClearTargetListValue(lastActor);
			m_minRot = float.MaxValue;
			ActorManager.Singleton.ForEach(CheckTarget);
			if (TargetManager.CurrentTarget == null)
			{
				return false;
			}
		}
        TargetManager.ModifyTargetListValue(1, TargetManager.CurrentTarget);
        if (lastActor != TargetManager.CurrentTarget)
        {
            AttackAction action = this.ActionControl.LookupAction(ActorAction.ENType.enAttackAction) as AttackAction;
            if (action != null && action.IsNormalAttack())
            {//普攻，移除AttackAction
                this.ActionControl.RemoveAction(ActorAction.ENType.enAttackAction);
            }
        }
        else
        {
            if (TargetManager.TargetList.Count > 1)
            {
                ChangeTarget();
            }
        }
        FireNormalSkill();
        return true;
	}
    public override void UpdateHPBar()
    {
        NotifyChanged((int)ENPropertyChanged.enMainHead, EnMainHeadType.enHpChanged);

		base.UpdateHPBar();
    }
    public void Teleport(Transform teleport)
    {
        this.TargetManager.ClearTarget();
        CursorEffectFunc.Singleton.Destroy();
        if (null != teleport)
        {
            this.ActionControl.RemoveAll();
            this.ActionControl.AddAction(ActorAction.ENType.enStandAction);
            this.mAnimControl.Tick();
            this.ForceMoveToPosition(teleport.position, teleport.rotation);
            //this.MainObj.transform.localRotation = teleport.localRotation;
            MainGame.Singleton.MainCamera.MoveAtOnce(this);
            SM.RandomRoomLevel.Singleton.NotifyChanged((int)SM.RandomRoomLevel.ENPropertyChanged.enUIBossRoomWarning, null);
            
            //MainGame.Singleton.MainCamera.Update();
            //MainGame.Singleton.StartCoroutine(CoroutineAnimationEnd(this));
            //{//跟随角色
            //    Actor followActor = ActorManager.Singleton.Lookup((int)EnMyPlayers.enComrade);
            //    if (null != followActor && !followActor.IsDead)
            //    {
            //        followActor.ActionControl.RemoveAll();
            //        followActor.ActionControl.AddAction(ActorAction.ENType.enStandAction);
            //        followActor.mAnimControl.Tick();
            //        Vector3 targetPos = this.MainPos + this.MainObj.transform.forward.normalized * -0.5f;
            //        followActor.UnhideMe(targetPos);
            //    }
            //    //followActor.SelfAI.ActionForwardTo(this.ID);
            //}
            if (ActorManager.Singleton.Comrade != null && !ActorManager.Singleton.Comrade.IsDead)
            {//传送同伴
                ActorManager.Singleton.Comrade.InitPartnerPosition();
            }
        }
        CursorEffectFunc.Singleton.Active();
    }
    public int m_curBattleBtn = (int)ENPropertyChanged.enBattleBtn_D;
    public int m_curHighlightIndex = 0;
    public void ChangeBattleBtn()
    {
        return;
        //++m_curBattleBtn;
        //if (m_curBattleBtn > (int)ENPropertyChanged.enBattleBtn_D)
        //{
        //    m_curBattleBtn = (int)ENPropertyChanged.enBattleBtn_L;
        //}
        //NotifyChanged((int)ENPropertyChanged.enBattleBtn_L, ENBattleBtnNotifyType.enShowChange);
        //NotifyChanged((int)ENPropertyChanged.enBattleBtn, ENBattleBtnNotifyType.enShowChange);
        //NotifyChanged((int)ENPropertyChanged.enBattleBtn_D, ENBattleBtnNotifyType.enShowChange);
    }
	public void CloseSetCamera()//heizTest
	{
		NotifyChanged((int)ENPropertyChanged.enSetCamera, true);
	}
    public override void OnHpChanged(int hp, bool isCrit, float multiple, bool isHeal)
    {
        /*if (hp > 0)
        {
            ReduceHP widget = MainGame.Singleton.CurrentWidgets.AddWidget<ReduceHP>();
            widget.SetValue(hp, isCrit);
            Vector3 vpos = this.RealPos;
            vpos.y += mCapsulYSize;
            //widget.AttachObj(this.CenterPart);
            widget.m_obj.transform.position = vpos;
            widget.m_obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            widget.m_obj.transform.parent = this.MainObj.transform;

            m_reduceHPList.Add(widget as IWidget);
        }*/
        base.OnHpChanged(hp, isCrit, multiple, isHeal);
    }
    public void UpDataSkillLevelUp() 
    {
        NotifyChanged((int)Actor.ENPropertyChanged.enBattleBtn_D, MainPlayer.ENBattleBtnNotifyType.enUpdataSkillLevelUp);
    }
    public void UpdataMainHeadLevel() 
    {
        NotifyChanged((int)ENPropertyChanged.enMainHead, EnMainHeadType.enUpdataMainHeadLevel);
    }
    public override bool OnFireSkill(int skillID)
    {
        if (IsDead)
        {
            return false;
        }
        Actor.ActorSkillInfo info = SkillBag.Find(item => item.SkillTableInfo.ID == skillID);
        if (info == null)
        {
            Debug.LogError("skill bag is not contains,skillID:"+skillID+",actorID:"+ID+",type:"+Type);
            return false;
        }
        if (!info.IsCanFire(this, skillID != 0))
        {
            return false;
        }
        if (info.SkillTableInfo.SkillType == (int)ENSkillType.enSkillNormalType)
        {
            return FireNormalSkill();
        }
        this.CurrentCmd = new Player.Cmd(skillID);
        return true;
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    float m_switchActorTime = 0;
    public void ActorExit()
    {
        IsActorExit = true;
        //隐藏collider
        EnableCollider(false);
        //通知所有，清除目标
        ActorManager.Singleton.NotifyAll_ClearTarget(ID);
        //清除命令
        CurrentCmd = null;
        //清除目标
        TargetManager.CurrentTarget = null;
        //增加combo时间
        if (m_switchActorTime == 0)
        {
            m_switchActorTime = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enComboTimeOfSwitchActor).FloatTypeValue;
        }
        Combo.SwitchActorTime = m_switchActorTime;

        //add buff
        if (CurrentTableInfo.SwitchBuffIDList.Count != 0)
        {
            IResult r = BattleFactory.Singleton.CreateResult(ENResult.AddBuff, ID, ID, 0, 0, CurrentTableInfo.SwitchBuffIDList.ToArray());
            if (r != null)
            {
                r.ResultExpr(CurrentTableInfo.SwitchBuffIDList.ToArray());
                BattleFactory.Singleton.GetBattleContext().CreateResultControl().DispatchResult(r);
            }
        }
    }
    public void ActorEnter(Vector3 pos, Vector3 forward, int targetID, Vector3 targetPos, bool isAttack)
    {
        IsActorExit = false;
        //隐藏collider
        EnableCollider(false);
        //设置位置
        UnhideMe(pos);
        //设置朝向
        MainObj.transform.forward = forward;

        IsMoveAfterSwitch = false;
        if (targetPos != Vector3.zero)
        {//move
            IsMoveAfterSwitch = true;
            CurrentCmd = new Player.Cmd(targetPos);
        }
        else if (isAttack)
        {//attack
            FireNormalSkill();
        }
        //初始化技能UI
        OnInitSkillBag(true);
        //通知headUI
        NotifyChanged((int)Actor.ENPropertyChanged.enMainHead, EnMainHeadType.enHpChanged);
        NotifyChanged((int)Actor.ENPropertyChanged.enMainHead, EnMainHeadType.enSwitchActor);

        //target
        TargetManager.CurrentTarget = ActorManager.Singleton.Lookup(targetID);
		//入场action
		ActionControl.AddAction(ActorAction.ENType.enActorEnterAction);


        //remove buff
        if (CurrentTableInfo.SwitchBuffIDList.Count != 0)
        {
            List<float> paramList = new List<float>(1 + CurrentTableInfo.SwitchBuffIDList.Count);
            paramList.Add((float)ResultRemoveBuff.ENRemoveBuffType.enBuffID);
            paramList.AddRange(CurrentTableInfo.SwitchBuffIDList);

            IResult r = BattleFactory.Singleton.CreateResult(ENResult.RemoveBuff, ID, ID, 0, 0, paramList.ToArray());
            if (r != null)
            {
                r.ResultExpr(paramList.ToArray());
                BattleFactory.Singleton.DispatchResult(r);
            }
        }
        //camera
        MainGame.Singleton.MainCamera.MoveAtOnce(this);
    }

    public override void OnSkillSilence(int silenceType, int type, bool isSilence)
    {
        base.OnSkillSilence(silenceType, type, isSilence);
        if (ActorManager.Singleton.Support != null && ActorManager.Singleton.Support.SkillBag.Count != 0)
        {
            switch ((ENSkillSilenceType)silenceType)
            {
                case ENSkillSilenceType.enSkill:
                    {
                        if (ActorManager.Singleton.Support.SkillBag[0].SkillTableInfo.SkillType == type)
                        {
                            ActorManager.Singleton.Support.SkillBag[0].IsSilence = isSilence;
                        }
                    }
                    break;
                case ENSkillSilenceType.enSkillSpecial:
                    {
                        if (ActorManager.Singleton.Support.SkillBag[0].SkillTableInfo.SkillSpecialType == type)
                        {
                            ActorManager.Singleton.Support.SkillBag[0].IsSilence = isSilence;
                        }
                    }
                    break;
            }
        }
        NotifyChanged((int)ENPropertyChanged.enBattleBtn_D, ENBattleBtnNotifyType.enSkillEnabled);
    }
	public bool SwitchAI()
    {
        if (SelfAI == m_player)
        {
            SelfAI = m_autoPlayer;
            return true;
        }
        else
        {
            SelfAI = m_player;
            return false;
        }
	}
}