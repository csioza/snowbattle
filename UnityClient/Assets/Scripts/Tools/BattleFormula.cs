
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleFormula
{

    static public int GetHp(int id,int level,int yellowPoint = 1)
    {
        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(id);
        if (null == info)
        {
            return 0;
        }

        RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(info.Rarity);

        if (rarityInfo == null)
        {
            return 0;
        }

        QualityInfo qualityInfo = GameTable.qualityRelativeAsset.LookUp(info.Quality);
        if (qualityInfo == null)
        {
            return 0;
        }

        CardTypeInfo cardTypeInfo = GameTable.cardTypeVariationAsset.LookUp((int)info.AttrTypeID);

        if (cardTypeInfo == null)
        {
            return 0;
        }

        CardLevelInfo cardLevelInfo = GameTable.cardLevelVariationAsset.LookUp((int)info.levelTypeID);

        if (cardLevelInfo == null)
        {
            return 0;
        }

        YellowPointInfo yellowPointInfo = GameTable.yellowPointParamAsset.LookUp(yellowPoint);

        if (yellowPointInfo == null)
        {
            return 0;
        }

        float maxLevelAttrParam = GameTable.WorldParamTableAsset.Lookup((int)ENWorldParamIndex.enMaxLevelAttrParam).FloatTypeValue;

        float hp = (info.FHPMax + cardTypeInfo.m_hp) * (1 + cardLevelInfo.m_hpParam * level + yellowPointInfo.m_hpParam) * (rarityInfo.m_mainAttrMutiply + qualityInfo.m_mainAttrMutiply);

        return (int)hp;
    }

    static public int GetPhyAttack(int id, int level, int yellowPoint = 1)
    {

        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(id);
        if (null == info)
        {
            return 0;
        }

        RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(info.Rarity);

        if (rarityInfo == null)
        {
            return 0;
        }

        QualityInfo qualityInfo = GameTable.qualityRelativeAsset.LookUp(info.Quality);
        if (qualityInfo == null)
        {
            return 0;
        }

        CardTypeInfo cardTypeInfo = GameTable.cardTypeVariationAsset.LookUp((int)info.AttrTypeID);

        if (cardTypeInfo == null)
        {
            return 0;
        }

        LevelUpInfo levelUpInfo = GameTable.LevelUpTableAsset.LookUp(level);
        if (levelUpInfo == null)
        {
            return 0;
        }
         CardLevelInfo cardLevelInfo = GameTable.cardLevelVariationAsset.LookUp((int)info.levelTypeID);

        if (cardLevelInfo == null)
        {
            return 0;
        }

        YellowPointInfo yellowPointInfo = GameTable.yellowPointParamAsset.LookUp(yellowPoint);

        if (yellowPointInfo == null)
        {
            return 0;
        }

        //Debug.Log("(" + info.FPhyAttack + "+" + cardTypeInfo.m_phyAtt + ") * (1 +" + cardLevelInfo.m_phyAttParam + "*" + level + "+" + yellowPointInfo.m_phyAttParam + ")*(" + rarityInfo.m_mainAttrMutiply +"+"+ qualityInfo.m_mainAttrMutiply+")");
        // 计算物理攻击力
        float attack = (info.FPhyAttack + cardTypeInfo.m_phyAtt) * (1 + cardLevelInfo.m_phyAttParam * level + yellowPointInfo.m_phyAttParam) * (rarityInfo.m_mainAttrMutiply + qualityInfo.m_mainAttrMutiply);
        return (int)attack;
    }

    // 获得当前魔法攻击力
    static public int GetMagAttack(int id, int level, int yellowPoint = 1)
    {
        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(id);
        if (null == info)
        {
            return 0;
        }

        RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(info.Rarity);

        if (rarityInfo == null)
        {
            return 0;
        }

        QualityInfo qualityInfo = GameTable.qualityRelativeAsset.LookUp(info.Quality);
        if (qualityInfo == null)
        {
            return 0;
        }

        CardTypeInfo cardTypeInfo = GameTable.cardTypeVariationAsset.LookUp((int)info.AttrTypeID);

        if (cardTypeInfo == null)
        {
            return 0;
        }

        LevelUpInfo levelUpInfo = GameTable.LevelUpTableAsset.LookUp(level);
        if (levelUpInfo == null)
        {
            return 0;
        }

        CardLevelInfo cardLevelInfo = GameTable.cardLevelVariationAsset.LookUp((int)info.levelTypeID);

        if (cardLevelInfo == null)
        {
            return 0;
        }

        YellowPointInfo yellowPointInfo = GameTable.yellowPointParamAsset.LookUp(yellowPoint);

        if (yellowPointInfo == null)
        {
            return 0;
        }

        // 获得当前魔法攻击力
        float attack = (info.FMagAttack + cardTypeInfo.m_magAtt) * (1 + cardLevelInfo.m_magAttParam * level + yellowPointInfo.m_magAttParam) * (rarityInfo.m_mainAttrMutiply + qualityInfo.m_mainAttrMutiply);

        return (int)attack;
    }

    // 获得当前魔法防御力
    static public int GetMagDefend(int id, int level, int yellowPoint = 1)
    {
        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(id);
        if (null == info)
        {
            return 0;
        }

        RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(info.Rarity);

        if (rarityInfo == null)
        {
            return 0;
        }

        QualityInfo qualityInfo = GameTable.qualityRelativeAsset.LookUp(info.Quality);
        if (qualityInfo == null)
        {
            return 0;
        }

        CardTypeInfo cardTypeInfo = GameTable.cardTypeVariationAsset.LookUp((int)info.AttrTypeID);

        if (cardTypeInfo == null)
        {
            return 0;
        }

        LevelUpInfo levelUpInfo = GameTable.LevelUpTableAsset.LookUp(level);
        if (levelUpInfo == null)
        {
            return 0;
        }

        CardLevelInfo cardLevelInfo = GameTable.cardLevelVariationAsset.LookUp((int)info.levelTypeID);

        if (cardLevelInfo == null)
        {
            return 0;
        }

        YellowPointInfo yellowPointInfo = GameTable.yellowPointParamAsset.LookUp(yellowPoint);

        if (yellowPointInfo == null)
        {
            return 0;
        }

        // 获得当前魔法 防御力
        float attack = (info.FMagDefend + cardTypeInfo.m_magDef) * (1 + cardLevelInfo.m_magDefParam * level + yellowPointInfo.m_magDefParam) * (rarityInfo.m_mainAttrMutiply + qualityInfo.m_mainAttrMutiply);

        return (int)attack;
    }

    // 获得当前物理防御力
    static public int GetPhyDefend(int id, int level, int yellowPoint = 1)
    {
        HeroInfo info = GameTable.HeroInfoTableAsset.Lookup(id);
        if (null == info)
        {
            return 0;
        }

        RarityRelativeInfo rarityInfo = GameTable.RarityRelativeAsset.LookUp(info.Rarity);

        if (rarityInfo == null)
        {
            return 0;
        }

        QualityInfo qualityInfo = GameTable.qualityRelativeAsset.LookUp(info.Quality);
        if (qualityInfo == null)
        {
            return 0;
        }

        CardTypeInfo cardTypeInfo = GameTable.cardTypeVariationAsset.LookUp((int)info.AttrTypeID);

        if (cardTypeInfo == null)
        {
            return 0;
        }

        LevelUpInfo levelUpInfo = GameTable.LevelUpTableAsset.LookUp(level);
        if (levelUpInfo == null)
        {
            return 0;
        }

        CardLevelInfo cardLevelInfo = GameTable.cardLevelVariationAsset.LookUp((int)info.levelTypeID);

        if (cardLevelInfo == null)
        {
            return 0;
        }

        YellowPointInfo yellowPointInfo = GameTable.yellowPointParamAsset.LookUp(yellowPoint);

        if (yellowPointInfo == null)
        {
            return 0;
        }

        // 获得当前魔法 防御力
        float attack = (info.FPhyDefend + cardTypeInfo.m_phyDef) * (1 + cardLevelInfo.m_phyDefParam * level + yellowPointInfo.m_phyDefParam) * (rarityInfo.m_mainAttrMutiply + qualityInfo.m_mainAttrMutiply);
        return (int)attack;
    }
}