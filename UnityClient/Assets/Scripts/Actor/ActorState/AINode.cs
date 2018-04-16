using System;
using UnityEngine;
using System.Xml;
using System.Collections.Generic;


public class FunctionalFuncBase
{
    public enum ValueType
    {
        enNone = -1,
        enFloat,
        enTime,
        enInt,
        enBool,
        enString,
    }
    public enum ComepareType
    {
        enNone=-1,
        enHigh,
        enLow,
        enEqual,
    }
    /*
    public static bool CompareValue(string compareType, AIBaseData valueSour, string valueType, string valueTar)
    {
        switch (valueType)
        {
            case "Float":
            case "Time":
                return CompareValue_Float(compareType, valueSour.GetValue_Float(), float.Parse(valueTar));
            case "Bool":
                return valueSour.GetValue_Bool() == (valueTar == "True" ? true : false);
        }
        return false;
    }
    public static bool CompareValue_Float(string compareType, float valueSour, float valueTar)
    {
        switch (compareType)
        {
            case "ValueLow":
                return valueSour < valueTar;
            case "ValueHigh":
                return valueSour > valueTar;
            case "Equal":
                return valueSour == valueTar;
        }
        return false;
    }
    public static float OperateValue(string optType, float valueSour, float valueTar)
    {
        switch (optType)
        {
            case "Add":
                return valueSour + valueTar;
            case "Sub":
                return valueSour - valueTar;
            default:
                return -1;
        }
    }
    public static void SetValue(Actor actor, string objType, string valueName, string valueType, string setType, string value)
    {
        switch (objType)
        {
            case "Actor":
                SetActorValue(actor, valueName, valueType, setType, value);
                break;
            case "AI":
                SetAIValue(actor, valueName, valueType, setType, value);
                break;
            default:
                break;
        }
    }
    public static void SetActorValue(Actor actor, string valueName, string valueType, string setType, string value)
    {
        AIBaseData actorValue = GetActorValue(actor, valueName, valueType);
        switch (setType)
        {
            case "Add":
            case "Sub":
                SetActorValue(actor, valueName, OperateValue(setType, actorValue.GetValue_Float(), float.Parse(value)).ToString());
                break;
            case "None":
                SetActorValue(actor, valueName, value);
                break;
            default:
                break;
        }
    }

    public static void SetActorValue(Actor actor, string valueName, string value)
    {
        if (actor.Type == ActorType.enNPCTrap)
        {
            SetActorValue(actor as Trap, valueName, value);
        }
        else
        {
            switch (valueName)
            {
                case "HP":
                    if (actor.MaxHP <= int.Parse(value))
                    {
                        actor.MaxHP = int.Parse(value);
                    }
                    actor.HP = int.Parse(value);
                    break;
                default:
                    break;
            }
        }
        
    }
    public static void SetActorValue(Trap actor, string valueName, string value)
    {
        switch (valueName)
        {
            case "TrapIsActive":
                actor.m_trapActivate = value == "True" ? true : false;
                break;
            case "TrapIsAble":
                actor.m_trapAble = value == "True" ? true : false;
                break;
            case "TrapMaxAttackCount":
                actor.mMaxAttackCount = int.Parse(value);
                break;
            case "TrapMinAttackTime":
                actor.mMinAttackTime = float.Parse(value);
                break;
            default:
                break;
        }
    }

    public static void SetActorValue(Actor actor, string valueName, float value)
    {
        if (actor.Type == ActorType.enNPCTrap)
        {
            SetActorValue(actor as Trap, valueName, value);
        }
        switch (valueName)
        {
            case "HP":
                actor.MaxHP = (int)value;
                actor.HP = (int)value;
                break;
            default:
                break;
        }
    }
    public static void SetActorValue(Trap actor, string valueName, float value)
    {
        switch (valueName)
        {
            case "TrapMaxAttackCount":
                actor.mMaxAttackCount = (int)value;
                break;
            case "TrapMinAttackTime":
                actor.mMinAttackTime = value;
                break;
            default:
                break;
        }
    }
    public static void SetAIValue(Actor actor, string valueName, string valueType, string setType, string value)
    {
        AIBaseData curBaseData = actor.SelfAI.GetBaseData(valueName, valueType);
        switch (setType)
        {
            case "Add":
                curBaseData.SetValue(curBaseData.GetValue_Float() + float.Parse(value));
                break;
            case "Sub":
                curBaseData.SetValue(curBaseData.GetValue_Float() - float.Parse(value));
                break;
            case "Reset":
                curBaseData.ResetValue();
                break;
            case "None":
                curBaseData.SetValue(valueType, value);
                break;
            default:
                break;
        }
    }

    public static AIBaseData GetValue(Actor actor, string objType, string valueName, string valueType)
    {
        switch (objType)
        {
            case "Actor":
                return GetActorValue(actor, valueName, valueType);
            case "AI":
                return GetAIValue(actor, valueName, valueType);
        }
        return null;
    }
    public static AIBaseData GetActorValue(Actor actor, string valueName, string valueType = "Float")
    {
        AIBaseData tmpAiBaseData = new AIBaseData(valueName, valueType);
        switch (valueName)
        {
            case "HPValue":
                tmpAiBaseData.SetValue(actor.HP);
                break;
            case "HPPercent":
                tmpAiBaseData.SetValue(actor.HP / actor.MaxHP);
                break;
            case "TargetDistance":
                tmpAiBaseData.SetValue(ActorTargetManager.GetTargetDistance(actor.RealPos, actor.CurrentTarget.RealPos));
                break;
            case "CurTarget":
                tmpAiBaseData.SetValue(actor.CurrentTargetIsDead);
                break;
        }
        return tmpAiBaseData;
    }
    public static AIBaseData GetActorValue(Trap actor, string valueName, string valueType = "Float")
    {
        AIBaseData tmpAiBaseData = new AIBaseData(valueName, valueType);
        switch (valueName)
        {
            case "TrapIsActive":
                tmpAiBaseData.SetValue(actor.m_trapActivate);
                break;
            case "TrapIsAble":
                tmpAiBaseData.SetValue(actor.m_trapAble);
                break;
            case "TrapMaxAttackCount":
                tmpAiBaseData.SetValue(actor.mMaxAttackCount);
                break;
            case "TrapMinAttackTime":
                tmpAiBaseData.SetValue(actor.mMinAttackTime);
                break;
            case "IsActorTouch":
                tmpAiBaseData.SetValue(actor.mEnterActorIDList.Count > 0);
                break;
        }
        return tmpAiBaseData;
    }
    public static AIBaseData GetAIValue(Actor actor, string valueName, string valueType)
    {
        AIBaseData tmpBaseData = null;
        switch (valueName)
        {
            case "IsInitBaseData":
                tmpBaseData = new AIBaseData(valueName, valueType);
                tmpBaseData.SetValue(actor.SelfAI.mIsInitBaseData);
                break;
            case "BattleStartTime":
                tmpBaseData = new AIBaseData(valueName, valueType);
                tmpBaseData.SetValue(Time.time - actor.SelfAI.m_baseDataDic[valueType].GetValue_Float());
                break;
            default:
                tmpBaseData = actor.SelfAI.GetBaseData(valueName,valueType);
                break;
        }
        return tmpBaseData;
    }
    public static AIBaseData GetAIValue(Actor actor, string valueName, ValueType valueType)
    {
        AIBaseData tmpBaseData = null;

        return tmpBaseData;
    }
    */
    public static ENSkillDistanceType ChangeStrToSkillDistanceType(string distanceType)
    {
        ENSkillDistanceType tmpDistanceType = ENSkillDistanceType.enNone;
        switch (distanceType)
        {
            case "Far":
                tmpDistanceType = ENSkillDistanceType.enFar;
                break;
            case "Near":
                tmpDistanceType = ENSkillDistanceType.enNear;
                break;
            case "Middle":
                tmpDistanceType = ENSkillDistanceType.enMiddle;
                break;
            default:
                break;
        }
        return tmpDistanceType;
    }
    public static ENSkillLevelType ChangeStrToSkillLevelType(string levelType)
    {
        ENSkillLevelType tmpLevelType = ENSkillLevelType.enNone;
        switch (levelType)
        {
            case "Base":
                tmpLevelType = ENSkillLevelType.enBase;
                break;
            case "Middle":
                tmpLevelType = ENSkillLevelType.enMiddle;
                break;
            case "High":
                tmpLevelType = ENSkillLevelType.enHigh;
                break;
            default:
                break;
        }
        return tmpLevelType;
    }
    public static int GetSkillID(Actor actor, ENSkillDistanceType disType, ENSkillLevelType levelType)
    {
        int skillId = ((AINpcBoss)actor.SelfAI).GetSkillID(disType, levelType);
        return skillId;
    }
    public static bool AIArises(Actor actor)
    {
        return false;
    }


    //Get/SetActor
    public static AIBaseData GetActorHPValue(Actor actor, string name = "")
    {
        AIBaseData tmpData = AIBaseData.CreateAIBaseData();
        tmpData.SetFloatValue(actor.HP);
        return tmpData;
    }
    public static void SetActorHPValue(Actor actor, AIBaseData value)
    {
        actor.HP = value.GetValue_Float();
    }
    public static AIBaseData GetActorHPPercent(Actor actor, string name = "")
    {
        AIBaseData tmpData = AIBaseData.CreateAIBaseData();
        tmpData.SetFloatValue(actor.HP / actor.MaxHP);
        return tmpData;
    }
	public static AIBaseData GetActorTargetDis(Actor actor, string name = "")
    {
        AIBaseData tmpData = AIBaseData.CreateAIBaseData();
        tmpData.SetFloatValue(ActorTargetManager.GetTargetDistance(actor.RealPos, actor.CurrentTarget.RealPos));
        return tmpData;
    }
	public static AIBaseData GetActorCurTarget(Actor actor, string name = "")
    {
        AIBaseData tmpData = AIBaseData.CreateAIBaseData(AIBaseData.DataType.enBool);
        tmpData.SetBoolValue(actor.CurrentTargetIsDead);
        return tmpData;
    }
	public static AIBaseData GetActorIsInitAIDataList(Actor actor, string name = "")
    {
        AIBaseData tmpData = AIBaseData.CreateAIBaseData(AIBaseData.DataType.enBool);
        tmpData.SetBoolValue(actor.IsInitAIDataList);
        return tmpData;
    }
    public static void SetActorIsInitAIDataList(Actor actor, AIBaseData val)
    {
        actor.IsInitAIDataList = val.GetValue_Bool();
    }
	public static AIBaseData GetTrapIsActive(Actor actor, string name = "")
    {
        Trap tmpTrap = actor as Trap;
        AIBaseData tmpData = AIBaseData.CreateAIBaseData(AIBaseData.DataType.enBool);
        tmpData.SetBoolValue(tmpTrap.m_trapActivate);
        return tmpData;
    }
    public static void SetTrapIsActive(Actor actor, AIBaseData val)
    {
        Trap tmpTrap = actor as Trap;
        tmpTrap.m_trapActivate = val.GetValue_Bool();
    }
	public static AIBaseData GetTrapIsAble(Actor actor, string name = "")
    {
        Trap tmpTrap = actor as Trap;
        AIBaseData tmpData = AIBaseData.CreateAIBaseData(AIBaseData.DataType.enBool);
        tmpData.SetBoolValue(tmpTrap.m_trapAble);
        return tmpData;
    }
    public static void SetTrapIsAble(Actor actor, AIBaseData val)
    {
        Trap tmpTrap = actor as Trap;
        tmpTrap.m_trapAble = val.GetValue_Bool(); 
    }
    public static void SetTrapState(Actor actor, AIBaseData val)
    {
        Trap tmpTrap = actor as Trap;
        tmpTrap.SetTrapState((Trap.TrapState)val.GetValue_Int());
    }
	public static AIBaseData GetTrapState(Actor actor, string name = "")
    {
        Trap tmpTrap = actor as Trap;
        AIBaseData tmpData = AIBaseData.CreateAIBaseData(AIBaseData.DataType.enInt);
        tmpData.SetIntValue((int)tmpTrap.mTrapState);
        return tmpData;
    }
	public static AIBaseData GetTrapMaxAttackCount(Actor actor, string name = "")
    {
        Trap tmpTrap = actor as Trap;
        AIBaseData tmpData = AIBaseData.CreateAIBaseData(AIBaseData.DataType.enInt);
        tmpData.SetIntValue(tmpTrap.mMaxAttackCount);
        return tmpData;
    }
    public static void SetTrapMaxAttackCount(Actor actor, AIBaseData val)
    {
        Trap tmpTrap = actor as Trap;
        tmpTrap.mMaxAttackCount = val.GetValue_Int();
    }
	public static AIBaseData GetTrapMinAttackTime(Actor actor, string name = "")
    {
        Trap tmpTrap = actor as Trap;
        AIBaseData tmpData = AIBaseData.CreateAIBaseData();
        tmpData.SetFloatValue(tmpTrap.mMinAttackTime);
        return tmpData;
    }
    public static void SetTrapMinAttackTime(Actor actor, AIBaseData val)
    {
        Trap tmpTrap = actor as Trap;
        tmpTrap.mMinAttackTime = val.GetValue_Float();
    }
	public static AIBaseData GetTrapIsActorTouch(Actor actor, string name = "") 
    {
        Trap tmpTrap = actor as Trap;
        AIBaseData tmpData = AIBaseData.CreateAIBaseData(AIBaseData.DataType.enBool);
        tmpData.SetBoolValue(tmpTrap.mEnterActorIDList.Count > 0);
        return tmpData;
    }

	public static AIBaseData GetAIData(Actor actor, string valueName)
	{
		AIBaseData aiData = actor.SelfAI.GetBaseData(valueName);
		return aiData;
	}
}


public class AINodeManager
{
    public static AINode CreateBossAI(string filename, string AIStr)
    {
        XmlDocument xml = new XmlDocument();
        TextAsset asset = PoolManager.Singleton.LoadWithoutInstantiate<TextAsset>("BossAIXml" + "/" + filename + ".xml");
        xml.LoadXml(asset.text);
        //xml.Load(Application.dataPath + "/Resources/BossAIXml" + "/" + filename + ".xml");
        XmlNode root = xml.SelectSingleNode(AIStr);
        AINode aiNode = CreateNodeByTypeName(((XmlElement)root).GetAttribute("Type"));
        aiNode.Deserialize(root);
        return aiNode;
    }
    static public T CreateAINode<T>() where T : AINode, new()
    {
        T ui = new T();
        return ui;
    }

    static public AINode CreateNodeByTypeName(string typeName)
    {
        Type uiNameObj = Type.GetType(typeName);
        object instanceFunc = System.Activator.CreateInstance(uiNameObj);
        System.Reflection.MethodInfo instance = uiNameObj.GetMethod("GetInstance");
        AINode aiNode = instance.Invoke(instanceFunc, null) as AINode;
        return aiNode;
    } 
}

public class AINode
{
    public List<AINode> mAINodeList = new List<AINode>();
    public virtual void Deserialize(XmlNode xmlNode)
    {
        Debug.Log("AINode public virtual void Deserialize");
        foreach (XmlNode item in xmlNode)
		{
            AINode aiNode = AINodeManager.CreateNodeByTypeName(((XmlElement)item).GetAttribute("Type"));
            aiNode.Deserialize(item);
            mAINodeList.Add(aiNode);
		}
    }
    public virtual bool Exec(Actor actor)
    {
        return true;
    }
}

public class AINodeSeq : AINode
{
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<AINodeSeq>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
    }
    public override bool Exec(Actor actor)
    {
        foreach(AINode item in mAINodeList)
        {
            if (!item.Exec(actor))
            {
                return false;
            }
        }
        return true;
    }
}

public class AINodeSel : AINode
{
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<AINodeSel>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {
        //foreach (XmlNode item in xmlNode)
        //{

        //}
        base.Deserialize(xmlNode);

    }
    public override bool Exec(Actor actor)
    {

        foreach (AINode item in mAINodeList)
        {
            if (item.Exec(actor))
            {
                return true;
            }
        }
        return false;
    }
}

public class DecoratorNot : AINode
{
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<DecoratorNot>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
    }
    public override bool Exec(Actor actor)
    {
        bool tmpBool = false;
        foreach (AINode item in mAINodeList)
        {
            if (item.Exec(actor))
            {
                tmpBool = true;
            }
        }
        if (tmpBool)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}


public class ValueCompareCondition : AINode
{
    AIBaseData mValue = null;
	string mSourValueType = null;
	string mSourValueName = null;
	string mTarValueType = null;
	string mTarValueName = null;
    AIBaseData.DataType mValueType = AIBaseData.DataType.enFloat;
    AIBaseData.CompareType mCompareType = AIBaseData.CompareType.enNone;

    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<ValueCompareCondition>();
        return self;
    }
    public delegate AIBaseData _GetValueFunction(Actor actor, string name);
    _GetValueFunction m_funcGetSourValue;
	_GetValueFunction m_funcGetTarValue;
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
        XmlElement xmlElement = xmlNode as XmlElement;
		mSourValueType = xmlElement.GetAttribute("SourValueType");
        mSourValueName = xmlElement.GetAttribute("SourValueName");
		m_funcGetSourValue = GetValueFunction(mSourValueType, mSourValueName);
		mTarValueType = xmlElement.GetAttribute("TarValueType");
		mTarValueName = xmlElement.GetAttribute("TarValueName");
		m_funcGetTarValue = GetValueFunction(mTarValueType, mTarValueName);
        mValueType = AIBaseData.ChangeStrToDataType(xmlElement.GetAttribute("ValueType"));
        mCompareType = AIBaseData.ChangeStrToCompareType(xmlElement.GetAttribute("CompareType"));
        mValue = AIBaseData.CreateAIBaseData(mValueType);
        mValue.SetValue(xmlElement.GetAttribute("Value"));
    }
    public override bool Exec(Actor actor)
    {
        bool tmpBool = false;
		AIBaseData sourData = null;
		AIBaseData tarData = null;
        if (m_funcGetSourValue != null)
        {
			sourData = m_funcGetSourValue(actor, "");
        }
		if (m_funcGetTarValue != null)
		{
			tarData = m_funcGetTarValue(actor, "");
			tmpBool = sourData.CompareValue(mCompareType, tarData);
		}
        else
        {
			tmpBool = sourData.CompareValue(mCompareType, mValue);
        }
        return tmpBool;
    }
	public _GetValueFunction GetValueFunction(string valueType, string valueName)
	{
		_GetValueFunction funcGetValue = null;
		if (valueType == "Actor")
		{
			switch (valueName)
			{
				case "HPValue":
					funcGetValue = FunctionalFuncBase.GetActorHPValue;
					break;
				case "HPPercent":
					funcGetValue = FunctionalFuncBase.GetActorHPPercent;
					break;
				case "TargetDistance":
					funcGetValue = FunctionalFuncBase.GetActorTargetDis;
					break;
				case "CurTarget":
					funcGetValue = FunctionalFuncBase.GetActorCurTarget;
					break;
				case "IsInitAIDataList":
					funcGetValue = FunctionalFuncBase.GetActorIsInitAIDataList;
					break;
				case "TrapIsActive":
					funcGetValue = FunctionalFuncBase.GetTrapIsActive;
					break;
				case "TrapIsAble":
					funcGetValue = FunctionalFuncBase.GetTrapIsAble;
					break;
				case "TrapState":
					funcGetValue = FunctionalFuncBase.GetTrapState;
					break;
				case "TrapMaxAttackCount":
					funcGetValue = FunctionalFuncBase.GetTrapMaxAttackCount;
					break;
				case "TrapMinAttackTime":
					funcGetValue = FunctionalFuncBase.GetTrapMinAttackTime;
					break;
				case "IsActorTouch":
					funcGetValue = FunctionalFuncBase.GetTrapIsActorTouch;
					break;
			}
			return funcGetValue;
		}
		else
		{

			funcGetValue = FunctionalFuncBase.GetAIData;
			return funcGetValue;
		}
	}
}

public class ValueSetCondition : AINode
{
    string mObjType = null;
    AIBaseData.SetType mSetType = AIBaseData.SetType.enNone;
    string mValueName = null;
    AIBaseData.DataType mValueType = AIBaseData.DataType.enFloat;
    AIBaseData mValue = null;
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<ValueSetCondition>();
        return self;
    }
    public delegate AIBaseData _GetValueFunction(Actor actor, string name);
    _GetValueFunction m_funcGetValue;
    public delegate void _SetValueFunction(Actor actor, AIBaseData val);
    _SetValueFunction m_funcSetValue;
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
        XmlElement xmlElement = xmlNode as XmlElement;
        mObjType = xmlElement.GetAttribute("ObjectType");
        mValueName = xmlElement.GetAttribute("ValueName");
        if (mObjType == "Actor")
        {
            switch (mValueName)
            {
                case "HP":
                    m_funcGetValue = FunctionalFuncBase.GetActorHPValue;
                    m_funcSetValue = FunctionalFuncBase.SetActorHPValue;
                    break;
                case "IsInitAIDataList":
                    m_funcGetValue = FunctionalFuncBase.GetActorIsInitAIDataList;
                    m_funcSetValue = FunctionalFuncBase.SetActorIsInitAIDataList;
                    break;
                case "TrapIsActive":
                    m_funcGetValue = FunctionalFuncBase.GetTrapIsActive;
                    m_funcSetValue = FunctionalFuncBase.SetTrapIsActive;
                    break;
                case "TrapIsAble":
                    m_funcGetValue = FunctionalFuncBase.GetTrapIsAble;
                    m_funcSetValue = FunctionalFuncBase.SetTrapIsAble;
                    break;
                case "TrapState":
                    m_funcGetValue = FunctionalFuncBase.GetTrapState;
                    m_funcSetValue = FunctionalFuncBase.SetTrapState;
                    break;
                case "TrapMaxAttackCount":
                    m_funcGetValue = FunctionalFuncBase.GetTrapMaxAttackCount;
                    m_funcSetValue = FunctionalFuncBase.SetTrapMaxAttackCount;
                    break;
                case "TrapMinAttackTime":
                    m_funcGetValue = FunctionalFuncBase.GetTrapMinAttackTime;
                    m_funcSetValue = FunctionalFuncBase.SetTrapMinAttackTime;
                    break;
                default:
                    break;
            }
        }
        else if (mObjType == "AI")
        {
            m_funcGetValue = null;
            m_funcSetValue = null;
        }
        mValueType = AIBaseData.ChangeStrToDataType(xmlElement.GetAttribute("ValueType"));
        mSetType = AIBaseData.ChangeStrToSetType(xmlElement.GetAttribute("SetType"));
        mValue = AIBaseData.CreateAIBaseData(mValueType);

        if (mSetType != AIBaseData.SetType.enReset)
        {
            mValue.SetValue(xmlElement.GetAttribute("Value"));
        }
    }
    public override bool Exec(Actor actor)
    {
        if (m_funcGetValue != null)
        {
            AIBaseData actorData = m_funcGetValue(actor, "");
            mValue.SetValue(mSetType, actorData);
            m_funcSetValue(actor, mValue);
        }
        else
        {
            AIBaseData actorData = actor.SelfAI.GetBaseData(mValueName, mValueType);
            mValue.SetValue(mSetType, actorData);
            actorData.SetValue(mValue);   
        }
        //FunctionalFuncBase.SetValue(actor, mObjType, mValueName, mValueType, mSetType, mValue);
        return true;
    }
}

public class SetBloodBarAction : AINode
{
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<SetBloodBarAction>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
    }
    public override bool Exec(Actor actor)
    {
        return ((AINpcBoss)actor.SelfAI).SetBossBloodBar();
    }
}

public class SetCurrentTargetAction : AINode
{
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<SetCurrentTargetAction>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
    }
    public override bool Exec(Actor actor)
    {
        return ((AINpcBoss)actor.SelfAI).GetRangeAndSetCurTarget();
    }
}

/*
public class SkillAction : AINode
{
    string mOperateType = null;
    string mSkillDistanceType = null;
    string mSkillLevelType = null;
    int mSkillID = -1;
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<SkillAction>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
        XmlElement xmlElement = xmlNode as XmlElement;
        mOperateType = xmlElement.GetAttribute("OperateType");
        mSkillDistanceType = xmlElement.GetAttribute("DistanceType");
        mSkillLevelType = xmlElement.GetAttribute("LevelType");
        mSkillID = int.Parse(xmlElement.GetAttribute("SkillID"));
    }
    public override bool Exec(Actor actor)
    {
        switch (mOperateType)
        {
            case "FireSkill":
                if (actor.SelfAI.ActionTryFireSkill(mSkillID))
                {
                    return true;
                }
                break;
            case "DistanceLevelSkill":
                mSkillID = FunctionalFuncBase.GetSkillID(actor, mSkillDistanceType, mSkillLevelType);
                if (mSkillID != 0 && actor.SelfAI.ActionTryFireSkill(mSkillID))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                return false;
        }
        return false;
    }
}
*/
public class FireSkillAction : AINode
{
    int mSkillID = 0;
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<FireSkillAction>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
        XmlElement xmlElement = xmlNode as XmlElement;
        mSkillID = int.Parse(xmlElement.GetAttribute("SkillID"));
    }
    public override bool Exec(Actor actor)
    {
        if (actor.SelfAI.ActionTryFireSkill(mSkillID))
        {
            return true;
        }
        return false;
    }
}

public class DistanceLevelSkillAction : AINode
{
    ENSkillDistanceType mSkillDistanceType = ENSkillDistanceType.enNone;
    ENSkillLevelType mSkillLevelType = ENSkillLevelType.enNone;
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<DistanceLevelSkillAction>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
        XmlElement xmlElement = xmlNode as XmlElement;
        mSkillDistanceType = FunctionalFuncBase.ChangeStrToSkillDistanceType(xmlElement.GetAttribute("DistanceType"));
        mSkillLevelType =  FunctionalFuncBase.ChangeStrToSkillLevelType(xmlElement.GetAttribute("LevelType"));
    }
    public override bool Exec(Actor actor)
    {
        int skillId = FunctionalFuncBase.GetSkillID(actor, mSkillDistanceType, mSkillLevelType);
        if (skillId != 0 && actor.SelfAI.ActionTryFireSkill(skillId))
        {
            return true;
        }
        return false;
    }
}

public class BreakStaminaAction : AINode
{
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<BreakStaminaAction>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {

    }
    public override bool Exec(Actor actor)
    {
        if (((AINpcBoss)actor.SelfAI).BreakStamina())
        {
            return true;
        }
        return false;
    }
}

public class GetTargetAction : AINode
{
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<GetTargetAction>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
    }
    public override bool Exec(Actor actor)
    {
        return ((AINpcBoss)actor.SelfAI).GetRangeAndSetCurTarget();
    }
}

public class ForwardToAction : AINode
{
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<ForwardToAction>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
    }
    public override bool Exec(Actor actor)
    {
        AINpcBoss npcBossAI = actor.SelfAI as AINpcBoss;
        return npcBossAI.ActionForwardTo(npcBossAI.m_curTargetID);
    }
}

public class TrapOnAttackAction : AINode
{
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<TrapOnAttackAction>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
    }
    public override bool Exec(Actor actor)
    {
        AINpcTrap trapAI = actor.SelfAI as AINpcTrap;
        return trapAI.OnAttackAction();
    }
}
public class TrapOnContinueDamageAction : AINode
{
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<TrapOnContinueDamageAction>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
    }
    public override bool Exec(Actor actor)
    {
        AINpcTrap trapAI = actor.SelfAI as AINpcTrap;
        return trapAI.OnContinueDamageAction();
    }
}
public class TrapOnStandByAction : AINode
{
    static public AINode GetInstance()
    {
        AINode self = AINodeManager.CreateAINode<TrapOnStandByAction>();
        return self;
    }
    public override void Deserialize(XmlNode xmlNode)
    {
        base.Deserialize(xmlNode);
    }
    public override bool Exec(Actor actor)
    {
        AINpcTrap trapAI = actor.SelfAI as AINpcTrap;
        return trapAI.OnStandByAction();
    }
}
