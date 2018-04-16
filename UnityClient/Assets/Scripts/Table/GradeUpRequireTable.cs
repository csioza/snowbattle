using System;
using System.Collections.Generic;
using UnityEngine;

public class FormulaInfo 
{
    public int Formula_Cost { get; set; }//公式消耗金币
    public int Param_Num { get; set; }//公式参数个数
    public List<FormulaParam> ParamList = new List<FormulaParam>();//参数列表
}

public class FormulaParam
{
    public int paramType { get; set; }//参数类型
    public int paramId { get;  set; }//指定ID
    public int paramLevel { get;  set; }//需要的卡牌等级
    public int paramRarity { get;  set; }//需要的卡牌星级
    public int paramOccu { get;  set; }//需要的卡牌职业
    public int paramRingNum { get;  set; }//需要的荣誉戒指数量
}

public class GradeUpRequireInfo
{
    public int ID { get; private set; }//卡牌ID

    public int GradeUpTime { get; private set; }//升段次数

    public int LevelLimitUp { get; private set; }//每次升段提升等级上限

    public int HpUp { get; private set; }//每次升段提升生命值上限

    public int AttackUp { get; private set; }//每次升段提升攻击力上限

    public int MagicAttackUp { get; private set; }//每次升段提升魔导力上限

    public int Formula1_Cost { get; private set; }//升段公式一消耗金币数量

    public int FormulaNum { get; private set; }//升段公式数量

    public List<FormulaInfo> FormulaList = new List<FormulaInfo>();//升段公式列表


    public void Load(BinaryHelper helper) 
    {
        ID = helper.ReadInt();
        GradeUpTime = helper.ReadInt();
        LevelLimitUp = helper.ReadInt();
        HpUp = helper.ReadInt();
        AttackUp = helper.ReadInt();
        MagicAttackUp = helper.ReadInt();
        Formula1_Cost = helper.ReadInt();
        FormulaNum = helper.ReadInt();
        for (int index = 1; index <= (FormulaNum-1); ++index)
        {
            FormulaInfo formulainfo = new FormulaInfo();
            formulainfo.Formula_Cost = helper.ReadInt();
            formulainfo.Param_Num = helper.ReadInt();
            for (int temp = 1; temp <= 5; ++temp)
            {
                FormulaParam formulaParam = new FormulaParam();
                formulaParam.paramType = helper.ReadInt();
                //if (ID == 29)
                //{
                //    int aaa = 0;
                //}
                formulaParam.paramId = helper.ReadInt();
                formulaParam.paramLevel = helper.ReadInt();
                formulaParam.paramRarity = helper.ReadInt();
                formulaParam.paramOccu = helper.ReadInt();
                formulaParam.paramRingNum = helper.ReadInt();
                formulainfo.ParamList.Add(formulaParam);
            }
            FormulaList.Add(formulainfo);
        }
    }
    public void Save(BinaryHelper helper) 
    {
        helper.Write(ID);
        helper.Write(GradeUpTime);
        helper.Write(LevelLimitUp);
        helper.Write(HpUp);
        helper.Write(AttackUp);
        helper.Write(MagicAttackUp);
        helper.Write(Formula1_Cost);
        helper.Write(FormulaNum);

        foreach (FormulaInfo item in FormulaList)
        {
            helper.Write(item.Formula_Cost);
            helper.Write(item.Param_Num);
            foreach (FormulaParam temp in item.ParamList)
            {
                helper.Write(temp.paramType);
                helper.Write(temp.paramId);
                helper.Write(temp.paramLevel);
                helper.Write(temp.paramRarity);
                helper.Write(temp.paramOccu);
                helper.Write(temp.paramRingNum);
            }
        }
    }
}

public class GradeUpRequireTable 
{
    public GradeUpRequireInfo Lookup(int id)
    {
        return GradeUpInfoList.Find(item => item.ID == id);;
    }
    public List<GradeUpRequireInfo> GradeUpInfoList { get; protected set; }

    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        GradeUpInfoList = new List<GradeUpRequireInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            GradeUpRequireInfo info = new GradeUpRequireInfo();
            info.Load(helper);
            GradeUpInfoList.Add(info);
        }
    }
}
