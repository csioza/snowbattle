using UnityEngine;
using System.Collections;
using NGE.Network;

#region disableWarning
#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#endregion

public class ResultHealth : IResult
{
    //治疗result类型
    public enum ENHealthResultType
    {
        enNone,
        enFixed,//固定恢复
        enPercent,//百分比恢复
        enPhy,//物理攻击结算恢复
        enMag,//魔法攻击结算恢复
        enHot,//hot恢复
    }
    //治疗类型
    public enum HealthType
    {
        enNone,
        enFixed,//固定恢复
        enPercent,//百分比恢复
        enPhy,//物理攻击结算恢复
        enMag,//魔法攻击结算恢复
    }
    //恢复result类型
    private ENHealthResultType m_healthResultType = ENHealthResultType.enNone;
    public ENHealthResultType HealthResultType { get { return m_healthResultType; } protected set { m_healthResultType = value; } }
    //治疗数值
    float m_healthValue = 0;
    public float HealthValue { get { return m_healthValue; } set { m_healthValue = value; } }
    public ResultHealth()
		: base((int)ENResult.Health)
    {
        m_healthValue = 0;
    }
    public static IResult CreateNew()
    {
        return new ResultHealth();
    }

    public override void Deserialize(PacketReader stream)
    {
        base.Deserialize(stream);
        HealthValue = stream.ReadInt32();
        //Debug.Log(" ResultHealth Deserialize:" + ",HealthValue:" + HealthValue + ",TargetID:" + TargetID);
    }


    public override void ResultExpr(float[] param)
    {
        base.ResultExpr(param);
        Actor target = ActorManager.Singleton.Lookup(TargetID);
        if (null == target || target.IsDead)
        {
            return;
        }
        Actor source = ActorManager.Singleton.Lookup(SourceID);
        if (null == source)
        {
            return;
        }
        {//结算
            float hp = 0;
            switch ((HealthType)param[0])
            {
                case HealthType.enFixed:
                    {
                        float value = param[1];
                        if (0 != value)
                        {
                            hp = value;
                            if (0 != param[2])
                            {//百分比上下浮动
                                hp = UnityEngine.Random.Range(1 - param[2], 1 + param[2]);
                            }
                        }
                    }
                    break;
                case HealthType.enPercent:
                    {
                        float percent = param[1];
                        if (0 != percent)
                        {
                            hp = target.MaxHP * percent;
                        }
                    }
                    break;
                case HealthType.enPhy:
                    {
                        if (0 != param[1])
                        {
                            hp = source.Props.GetProperty_Float(ENProperty.phyattack) * param[1];
                            if (0 != param[2])
                            {//百分比上下浮动
                                hp *= UnityEngine.Random.Range(1 - param[2], 1 + param[2]);
                            }
                        }
                    }
                    break;
                case HealthType.enMag:
                    {
                        if (0 != param[1])
                        {
                            hp = source.Props.GetProperty_Float(ENProperty.magattack) * param[1];
                            if (0 != param[2])
                            {//百分比上下浮动
                                hp *= UnityEngine.Random.Range(1 - param[2], 1 + param[2]);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            HealthValue = hp;
        }
    }
    public override void Exec(IResultControl control)
    {
        base.Exec(control);
        Actor target = ActorManager.Singleton.Lookup(TargetID);
        if (null == target || target.IsDead)
        {
            return;
        }
        if (HealthValue > 0)
        {
            float value = target.AddHp(HealthValue);
            target.OnHpChanged((int)value, false, 1, true);
        }
        else
        {
            Debug.LogWarning("health result is error, hp is " + HealthValue.ToString());
        }
    }

    public override void ResultServerExec(IResultControl control)
    {
        base.ResultServerExec(control);

        Actor target = ActorManager.Singleton.Lookup(TargetID);

        if (HealthValue > 0)
        {
            float value = target.AddHp(HealthValue);
            target.OnHpChanged((int)value, false, 1, true);
        }
        else
        {
            Debug.LogWarning("health result is error, hp is " + HealthValue.ToString());
        }
    }
}