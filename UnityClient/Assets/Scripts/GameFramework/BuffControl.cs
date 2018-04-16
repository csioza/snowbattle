using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffControl
{
    public class BaseProperty
    {
        public float phyAttack = 0, magAttack = 0, phyDefend = 0, magDefend = 0,
            hpMax = 0, avoid = 0, hit = 0, crit = 0, critparam = 0,
            FResist = 0, moveSpeed = 0, AnitInterfere = 0, AnitInterrupt = 0, AnitRepel = 0, AnitLauncher = 0,
            WoundParam = 0, movebackSpeed = 0, animationSpeed = 0, attackAnimSpeed,
            SkillCDModifyValue = 0, SkillCDModifyPercent = 0;
        public BaseProperty(Actor target)
        {
            phyAttack = target.Props.GetProperty_Float(ENProperty.phyattack);
            magAttack = target.Props.GetProperty_Float(ENProperty.magattack);
            phyDefend = target.Props.GetProperty_Float(ENProperty.phydefend);
            magDefend = target.Props.GetProperty_Float(ENProperty.magdefend);
            hpMax = target.Props.GetProperty_Float(ENProperty.maxhp);
            avoid = target.Props.GetProperty_Float(ENProperty.avoid);
            hit = target.Props.GetProperty_Float(ENProperty.hit);
            crit = target.Props.GetProperty_Float(ENProperty.crit);
            critparam = target.Props.GetProperty_Float(ENProperty.critParam);
            FResist = target.Props.GetProperty_Float(ENProperty.FResist);
            moveSpeed = target.Props.GetProperty_Float(ENProperty.runSpeed);
            AnitInterfere = target.Props.GetProperty_Float(ENProperty.AnitInterfere);
            AnitInterrupt = target.Props.GetProperty_Float(ENProperty.AnitInterrupt);
            AnitRepel = target.Props.GetProperty_Float(ENProperty.AnitRepel);
            AnitLauncher = target.Props.GetProperty_Float(ENProperty.AnitLauncher);
            WoundParam = target.Props.GetProperty_Float(ENProperty.WoundParam);
            movebackSpeed = target.Props.GetProperty_Float(ENProperty.MovebackSpeed);
            animationSpeed = target.Props.GetProperty_Float(ENProperty.AnimationSpeed);
            attackAnimSpeed = target.Props.GetProperty_Float(ENProperty.AttackAnimSpeed);
            SkillCDModifyValue = target.Props.GetProperty_Float(ENProperty.SkillCDModifyValue);
            SkillCDModifyPercent = target.Props.GetProperty_Float(ENProperty.SkillCDModifyPercent);
        }
    }
    public List<Buff> BuffList { get; private set; }
    private Actor m_target = null;
    private BaseProperty Props
    {
        get
        {
            if (m_props == null)
            {
                m_props = new BaseProperty(m_target);
            }
            return m_props;
        }
        set
        {
            m_props = value;
        }
    }
    private BaseProperty m_props = null;
    public BuffControl(Actor target)
    {
        m_target = target;
        BuffList = new List<Buff>();
    }
    bool IsDisable(BuffInfo newInfo)
    {
        List<Buff> replaceList = new List<Buff>();
        foreach (var item in BuffList)
        {
            BuffInfo oldInfo = GameTable.BuffTableAsset.Lookup(item.BuffID);
            if (oldInfo == null)
            {
                continue;
            }
            Buff.ENRelation relation = GameTable.BuffRelationTableAsset.Lookup(newInfo.Replaceable, oldInfo.Replaceable);
            switch (relation)
            {
                case Buff.ENRelation.enUnable:
                    {
                        //不允许新buff添加
                        Debug.Log("新buff不允许添加, new id:" + newInfo.ID + ", old id:" + oldInfo.ID);
                        return true;
                    }
//                    break;
                case Buff.ENRelation.enBothCan:
                    {
                        //共存
                    }
                    break;
                case Buff.ENRelation.enReplace:
                    {
                        //替换
                        replaceList.Add(item);
                    }
                    break;
            }



            ////是否有相同效果
            //bool isSameResult = false;
            //foreach (var newResult in newInfo.BuffResultList)
            //{//效果相同的替换掉
            //    if (newResult.ID != 0)
            //    {
            //        foreach (var oldResult in oldInfo.BuffResultList)
            //        {
            //            if (newResult.ID == oldResult.ID)
            //            {
            //                isSameResult = true;
            //                break;
            //            }
            //        }
            //        if (isSameResult)
            //        {
            //            break;
            //        }
            //    }
            //}
            //if (!isSameResult)
            //{
            //    for (int i = 0; i < BuffInfo.PropertyCount; ++i)
            //    {
            //        if (newInfo.PropertyPercentList[i] != 0 && oldInfo.PropertyPercentList[i] != 0)
            //        {
            //            isSameResult = true;
            //            break;
            //        }
            //        if (newInfo.PropertyValueList[i] != 0 && oldInfo.PropertyValueList[i] != 0)
            //        {
            //            isSameResult = true;
            //            break;
            //        }
            //    }
            //}
            //if (isSameResult)
            //{//有相同效果，则判断buff关系
            //    Buff.ENRelation relation = GameTable.BuffRelationTableAsset.Lookup(newInfo.Replaceable, oldInfo.Replaceable);
            //    switch (relation)
            //    {
            //        case Buff.ENRelation.enUnable:
            //            {
            //                //不允许新buff添加
            //                Debug.Log("新buff不允许添加, new id:" + newInfo.ID + ", old id:" + oldInfo.ID);
            //                return true;
            //            }
            //            break;
            //        case Buff.ENRelation.enBothCan:
            //            {
            //                //共存
            //            }
            //            break;
            //        case Buff.ENRelation.enReplace:
            //            {
            //                //替换
            //                replaceList.Add(item);
            //            }
            //            break;
            //    }
            //}
        }
        foreach (var item in replaceList)
        {
            item.MarkRemove(BattleFactory.Singleton.GetBattleContext().CreateResultControl());
        }
        return false;
    }
    public void AddBuff(int buffID, int sourceID, float modifyTimer,bool isServerSend)
    {
        BuffInfo info = GameTable.BuffTableAsset.Lookup(buffID);
        if (null == info)
        {
            return;
        }
        if (IsDisable(info))
        {
            return;
        }
        float length = info.BuffDuration;
        if (length != 0)
        {//不是无限的
            length += modifyTimer;
        }
        Buff buff = new Buff(buffID, !info.IsNotRemoveForDead, length, info.IsFirstWork, info.Period);
        foreach (var item in info.PropertyPercentList)
        {
            if (item != 0)
            {
                buff.IsCalcProperty = true;
                break;
            }
        }
        if (!buff.IsCalcProperty)
        {
            foreach (var item in info.PropertyValueList)
            {
                if (item != 0)
                {
                    buff.IsCalcProperty = true;
                    break;
                }
            }
        }
        //play effect
        BuffEffectInfo beInfo = GameTable.BuffEffectTableAsset.Lookup(info.BuffEffectID);
        if (beInfo != null && !string.IsNullOrEmpty(beInfo.EffectName))
        {
            Vector3 offset = new Vector3(beInfo.OffsetX, beInfo.OffsetY, beInfo.OffsetZ);
            string bone = beInfo.PlayBoneT;
            if (beInfo.IsSpecialPoint)
            {
                if (m_target.Type == ActorType.enNPC)
                {
                    NPC npc = m_target as NPC;
                    if (npc.GetNpcType() == ENNpcType.enBOSSNPC)
                    {
                        bone = beInfo.SpecialPointBoneT;
                    }
                }
            }
            MainGame.Singleton.StartCoroutine(Coroutine_Load(buff, beInfo.EffectName, 0, bone, beInfo.IsAdhered, offset));
        }
        BuffList.Add(buff);
        foreach (var item in info.BuffResultList)
        {
            if (buff.IsExsit(item.ID))
            {//buff中不能存在多个相同的IBuffEffect
                continue;
            }
            IBuffEffect buffEffect = BattleFactory.Singleton.GetBattleContext().CreateBuffEffect(item.ID);
            if (null == buffEffect)
            {
                continue;
            }
            buffEffect.TargetID = m_target.ID;
            buffEffect.SourceID = sourceID;
            buffEffect.BuffID = buffID;
            buffEffect.BuffEffectID = info.BuffEffectID;
            buff.AddBuffEffect(buffEffect);
            buffEffect.OnGetBuffEffect();
        }
		buff.UpdateFastFilterFlag();
        if (buff.IsCalcProperty && !isServerSend)
        {
            BuffResult();
        }
        else
        {
            RefreshBuffGraphics();
        }
	}
    IEnumerator Coroutine_Load(Buff buff, string effectName, float effectTime, string posByBone, bool isAdhered, Vector3 offset)
    {
        GameResPackage.AsyncLoadObjectData data = new GameResPackage.AsyncLoadObjectData();
        IEnumerator e = PoolManager.Singleton.Coroutine_Load(GameData.GetEffectPath(effectName), data);
        while (true)
        {
            e.MoveNext();
            if (data.m_isFinish)
            {
                break;
            }
            yield return e.Current;
        }
        if (data.m_obj != null)
        {
            m_target.PlayEffect(data.m_obj as GameObject, effectTime, posByBone, isAdhered, offset);
            if (buff != null)
            {
                buff.EffectObj = data.m_obj as GameObject;
            }
        }
    }
	public void RemoveBuff(int buffID, IResultControl control)
    {
        Buff buff = BuffList.Find(item => item.BuffID == buffID);
        if (null == buff)
        {
            return;
        }
        buff.MarkRemove(control);
    }

    public void RemoveAll(IResultControl control)
    {
        foreach (var buff in BuffList)
        {
            buff.MarkRemove(control);
        }
    }

    public void RemoveAllForDead(IResultControl control)
    {
        foreach (var buff in BuffList)
        {
            if (buff.IsNeedRemoveForDead)
            {
                buff.MarkRemove(control);
            }
        }
    }
    private bool Temp_Remove_Buff(Buff buff)
    {
        if (buff.IsNeedRemove)
        {
            ReleaseBuff(buff);
            return true;
        }
        return false;
    }
    public void Tick(float dt, IResultControl control)
    {
        //此处用for是因为方便buff的tick中添加buff，但删除buff只能在所有buff的tick之后
        bool isCalcProperty = false;
        for (int i = 0; i < BuffList.Count; ++i)
        {
            BuffList[i].IsRemoveBuff(dt, control);
            BuffList[i].Tick(control, dt);
            if (BuffList[i].IsNeedRemove && BuffList[i].IsCalcProperty)
            {
                isCalcProperty = true;
            }
        }

        int removedBuffCount = BuffList.RemoveAll(Temp_Remove_Buff);
        if (isCalcProperty || removedBuffCount>0)
        {
            if (isCalcProperty)
            {
                BuffResult();
            }
            else
            {
                RefreshBuffGraphics();
            }
        }
    }

   

    public void OnProduceResult(IResult result, IResultControl control)
    {
        for (int i = 0; i < BuffList.Count; ++i)
        {
            if (null != BuffList[i])
            {
                BuffList[i].OnProduceResult(result, control);
                if (!result.IsEnable)
                {
                    break;
                }
            }
        }
    }

    public void OnGetResult(IResult result, IResultControl control)
    {
        for (int i = 0; i < BuffList.Count; ++i)
        {
            if (null != BuffList[i])
            {
                BuffList[i].OnGetResult(result, control);
                if (!result.IsEnable)
                {
                    break;
                }
            }
        }
    }
    private void CalcFloat(float src, float multiply, float add, int id)
    {
        float value = (1.0f + multiply) * src + add;
        if (value < 0.0f)
        {
            value = 0.0f;
        }
        m_target.Props.SetProperty_Float(id, value);
        //Debug.LogWarning("buff result calc id=" + id.ToString() + "  value=" + value.ToString() + "   target id="+m_target.ID.ToString());
    }
    private void CalcInt(int src, float multiply, float add, int id)
    {
        int value = (int)((1.0f + multiply) * src + add);
        if (value < 0)
        {
            value = 0;
        }
        m_target.Props.SetProperty_Int32(id, value);
        //Debug.LogWarning("buff result calc id=" + id.ToString() + "  value=" + value.ToString() + "   target id=" + m_target.ID.ToString());
    }
    int m_buffIdForShader = 0;
    string m_colorNameForShader;
    void RefreshBuffGraphics()
    {
        int buffEffectID = 0, buffEffectLevel = 0;
        if (m_buffIdForShader != 0 && null == BuffList.Find(item => item.BuffID == m_buffIdForShader))
        {
            AnimationShaderParamCallback callback = m_target.GetBodyParentObject().GetComponent<AnimationShaderParamCallback>();
            if (callback != null)
            {
                //恢复shader的color
                callback.RestoreShader(m_colorNameForShader, AnimationShaderParamCallback.ENParamType.enColor);
            }
            else
            {
                Debug.LogWarning("BuffControl BuffResult change shader, AnimationShaderParamCallback is null");
            }
        }
        foreach (var item in BuffList)
        {
            if (null != item && !item.IsNeedRemove)
            {
                BuffInfo info = GameTable.BuffTableAsset.Lookup(item.BuffID);
                if (null != info)
                {
                    BuffEffectInfo buffEInfo = GameTable.BuffEffectTableAsset.Lookup(info.BuffEffectID);
                    if (buffEInfo != null)
                    {
                        if (buffEInfo.EffectLevel >= buffEffectLevel)
                        {
                            buffEffectLevel = buffEInfo.EffectLevel;
                            buffEffectID = info.BuffEffectID;
                            m_buffIdForShader = info.ID;
                        }
                    }
                }
            }
        }

        BuffEffectInfo beInfo = GameTable.BuffEffectTableAsset.Lookup(buffEffectID);
        if (beInfo != null)
        {
            if (beInfo.IsChangeShaderColor)
            {
                AnimationShaderParamCallback callback = m_target.GetBodyParentObject().GetComponent<AnimationShaderParamCallback>();
                if (callback != null)
                {
                    //改变shader的color
                    callback.ChangeShaderColor(beInfo.ShaderColorName, beInfo.ShaderColorParam);
                    m_colorNameForShader = beInfo.ShaderColorName;
                }
                else
                {
                    Debug.LogWarning("BuffControl BuffResult change shader, AnimationShaderParamCallback is null");
                }
            }
            else
            {
                m_buffIdForShader = 0;
            }
        }
        m_target.NotifyChangeModel();
    }
    //所有buff结算
    private void BuffResult()
    {
        float MultiplyFHPMax = 0.0f, MultiplyFPhyAttack = 0.0f, MultiplyFMagAttack = 0.0f, MultiplyFPhyDefend = 0.0f,
            MultiplyFMagDefend = 0.0f, MultiplyFavoid = 0.0f, MultiplyFhit = 0.0f, MultiplyFcrit = 0.0f,
            MultiplyFcritparam = 0.0f, MultiplyFResist = 0.0f, MultiplyMoveSpeed = 0.0f,
            MultiplyAnitInterfereRate = 0, MultiplyAnitInterruptRate = 0, MultiplyAnitRepelRate = 0, MultiplyAnitLauncherRate = 0,
            MultiplyWoundParam = 0, MultiplyMovebackSpeed = 0, MultiplyAnimationSpeed = 0, MultiplyAttackAnimSpeed = 0, MultiplySkillCD = 0,

            AddFHPMax = 0, AddFPhyAttack = 0, AddFMagAttack = 0, AddFPhyDefend = 0,
            AddFMagDefend = 0, AddFavoid = 0, AddFhit = 0, AddFcrit = 0, AddFcritparam = 0, AddFResist = 0, AddMoveSpeed = 0,
            AddAnitInterfere = 0, AddAnitInterrupt = 0, AddAnitRepel = 0, AddAnitLauncher = 0,
            AddWoundParam = 0, AddMovebackSpeed = 0, AddAnimationSpeed = 0, AddAttackAnimSpeed = 0, AddSkillCD = 0;

        int buffEffectID = 0, buffEffectLevel = 0;
        if (m_buffIdForShader != 0 && null == BuffList.Find(item => item.BuffID == m_buffIdForShader))
        {
            AnimationShaderParamCallback callback = m_target.GetBodyParentObject().GetComponent<AnimationShaderParamCallback>();
            if (callback != null)
            {
                //恢复shader的color
                callback.RestoreShader(m_colorNameForShader, AnimationShaderParamCallback.ENParamType.enColor);
            }
            else
            {
                Debug.LogWarning("BuffControl BuffResult change shader, AnimationShaderParamCallback is null");
            }
        }
        foreach (var item in BuffList)
        {
            if (null != item && !item.IsNeedRemove && item.IsCalcProperty)
            {
                BuffInfo info = GameTable.BuffTableAsset.Lookup(item.BuffID);
                if (null != info)
                {
                    int index = 0;
                    MultiplyFHPMax += info.PropertyPercentList[index++];
                    MultiplyFPhyAttack += info.PropertyPercentList[index++];
                    MultiplyFMagAttack += info.PropertyPercentList[index++];
                    MultiplyFPhyDefend += info.PropertyPercentList[index++];
                    MultiplyFMagDefend += info.PropertyPercentList[index++];
                    MultiplyFavoid += info.PropertyPercentList[index++];
                    MultiplyFhit += info.PropertyPercentList[index++];
                    MultiplyFcrit += info.PropertyPercentList[index++];
                    MultiplyFcritparam += info.PropertyPercentList[index++];
                    MultiplyFResist += info.PropertyPercentList[index++];
                    MultiplyAnitInterfereRate += info.PropertyPercentList[index++];
                    MultiplyAnitInterruptRate += info.PropertyPercentList[index++];
                    MultiplyAnitRepelRate += info.PropertyPercentList[index++];
                    MultiplyAnitLauncherRate += info.PropertyPercentList[index++];
                    MultiplyMoveSpeed += info.PropertyPercentList[index++];
                    MultiplyWoundParam += info.PropertyPercentList[index++];
                    MultiplyAnimationSpeed += info.PropertyPercentList[index++];
                    MultiplyMovebackSpeed += info.PropertyPercentList[index++];
                    MultiplyAttackAnimSpeed += info.PropertyPercentList[index++];
                    MultiplySkillCD += info.PropertyPercentList[index++];

                    index = 0;
                    AddFHPMax += info.PropertyValueList[index++];
                    AddFPhyAttack += info.PropertyValueList[index++];
                    AddFMagAttack += info.PropertyValueList[index++];
                    AddFPhyDefend += info.PropertyValueList[index++];
                    AddFMagDefend += info.PropertyValueList[index++];
                    AddFavoid += info.PropertyValueList[index++];
                    AddFhit += info.PropertyValueList[index++];
                    AddFcrit += info.PropertyValueList[index++];
                    AddFcritparam += info.PropertyValueList[index++];
                    AddFResist += info.PropertyValueList[index++];
                    AddAnitInterfere += info.PropertyValueList[index++];
                    AddAnitInterrupt += info.PropertyValueList[index++];
                    AddAnitRepel += info.PropertyValueList[index++];
                    AddAnitLauncher += info.PropertyValueList[index++];
                    AddMoveSpeed += info.PropertyValueList[index++];
                    AddWoundParam += info.PropertyValueList[index++];
                    AddAnimationSpeed += info.PropertyValueList[index++];
                    AddMovebackSpeed += info.PropertyValueList[index++];
                    AddAttackAnimSpeed += info.PropertyValueList[index++];
                    AddSkillCD += info.PropertyValueList[index++];

                    BuffEffectInfo buffEInfo = GameTable.BuffEffectTableAsset.Lookup(info.BuffEffectID);
                    if (buffEInfo != null)
                    {
                        if (buffEInfo.EffectLevel >= buffEffectLevel)
                        {
                            buffEffectLevel = buffEInfo.EffectLevel;
                            buffEffectID = info.BuffEffectID;
                            m_buffIdForShader = info.ID;
                        }
                    }
                }
            }
        }
        CalcFloat(Props.phyAttack, MultiplyFPhyAttack, AddFPhyAttack, ENProperty.phyattack);
        CalcFloat(Props.magAttack, MultiplyFMagAttack, AddFMagAttack, ENProperty.magattack);
        CalcFloat(Props.phyDefend, MultiplyFPhyDefend, AddFPhyDefend, ENProperty.phydefend);
        CalcFloat(Props.magDefend, MultiplyFMagDefend, AddFMagDefend, ENProperty.magdefend);
        CalcFloat(Props.hpMax, MultiplyFHPMax, AddFHPMax, ENProperty.maxhp);
        CalcFloat(Props.avoid, MultiplyFavoid, AddFavoid, ENProperty.avoid);
        CalcFloat(Props.hit, MultiplyFhit, AddFhit, ENProperty.hit);
        CalcFloat(Props.crit, MultiplyFcrit, AddFcrit, ENProperty.crit);
        CalcFloat(Props.critparam, MultiplyFcritparam, AddFcritparam, ENProperty.critParam);
        CalcFloat(Props.FResist, MultiplyFResist, AddFResist, ENProperty.FResist);
        CalcFloat(Props.moveSpeed, MultiplyMoveSpeed, AddMoveSpeed, ENProperty.runSpeed);
        CalcFloat(Props.AnitInterfere, MultiplyAnitInterfereRate, AddAnitInterfere, ENProperty.AnitInterfere);
        CalcFloat(Props.AnitInterrupt, MultiplyAnitInterruptRate, AddAnitInterrupt, ENProperty.AnitInterrupt);
        CalcFloat(Props.AnitRepel, MultiplyAnitRepelRate, AddAnitRepel, ENProperty.AnitRepel);
        CalcFloat(Props.AnitLauncher, MultiplyAnitLauncherRate, AddAnitLauncher, ENProperty.AnitLauncher);
        CalcFloat(Props.WoundParam, MultiplyWoundParam, AddWoundParam, ENProperty.WoundParam);
        CalcFloat(Props.movebackSpeed, MultiplyMovebackSpeed, AddMovebackSpeed, ENProperty.MovebackSpeed);
        float animSpeed = m_target.AnimationSpeed;
        CalcFloat(Props.animationSpeed, MultiplyAnimationSpeed, AddAnimationSpeed, ENProperty.AnimationSpeed);
        float attackAnimSpeed = m_target.AttackAnimSpeed;
        CalcFloat(Props.attackAnimSpeed, MultiplyAttackAnimSpeed, AddAttackAnimSpeed, ENProperty.AttackAnimSpeed);
        if (animSpeed != m_target.AnimationSpeed || attackAnimSpeed != m_target.AttackAnimSpeed)
        {//anim速度改变
            m_target.AnimationSpeedChanged();
        }
        m_target.SkillCDModifyValue = AddSkillCD;
        m_target.SkillCDModifyPercent = MultiplySkillCD;

        BuffEffectInfo beInfo = GameTable.BuffEffectTableAsset.Lookup(buffEffectID);
        if (beInfo != null)
        {
            if (beInfo.IsChangeShaderColor)
            {
                AnimationShaderParamCallback callback = m_target.GetBodyParentObject().GetComponent<AnimationShaderParamCallback>();
                if (callback != null)
                {
                    //改变shader的color
                    callback.ChangeShaderColor(beInfo.ShaderColorName, beInfo.ShaderColorParam);
                    m_colorNameForShader = beInfo.ShaderColorName;
                }
                else
                {
                    Debug.LogWarning("BuffControl BuffResult change shader, AnimationShaderParamCallback is null");
                }
            }
            else
            {
                m_buffIdForShader = 0;
            }
        }
        m_target.NotifyChangeModel();
    }
    //释放buff所占的资源，重新放入对象池
    private void ReleaseBuff(Buff buff)
    {
        List<IBuffEffect> effs = buff.EffectList;
        for (int i = 0; i < effs.Count;i++ )
        {
            IBuffEffect eff = effs[i];
            BattleFactory.Singleton.GetBattleContext().ReleaseBuffEffect(eff);
        }
    }
    //释放所有资源
    public void ReleaseAll()
    {
        for (int i = 0; i < BuffList.Count; ++i)
        {
            Buff buff = BuffList[i];
            ReleaseBuff(buff);
        }
        BuffList.Clear();
    }
}