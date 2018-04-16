using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ENStringIndex
{
    //装备替换界面
    enNone = 0,
    UIEquipChangeAttackSpeed = 1,                           //攻击速度
    UIEquipChangeHPMax = 2,                                 //HP
    UIEquipChangePhyAttack = 3,                             //物理攻击
    UIEquipChangeMagAttack = 4,                             //魔法攻击
    UIEquipChangePhyDefend = 5,                             //物理防御
    UIEquipChangeMagDefend = 6,                             //魔法防御
    UIEquipChangeHit = 7,                                   //命中
    UIEquipChangeAvoid = 8,                                 //闪避
    UIEquipChangeCriAttack = 9,                             //暴击
    UIEquipChangeStillAttack = 10,                          //连击
    UIEquipChangeStillDefend = 11,                          //韧性
    //角色属性界面
    UIEquipmentPhyAttack = 12,                              //物理攻击
    UIEquipmentPhyDefend = 13,                              //物理防御
    UIEquipmentMagAttack = 14,                              //魔法攻击
    UIEquipmentMagDefend = 15,                              //魔法防御
    UIEquipmentHit = 16,                                    //命中
    UIEquipmentAvoid = 17,                                  //闪避
    UIEquipmentCriAttack = 18,                              //暴击
    UIEquipmentStillDefend = 19,                            //韧性
    //技能界面
    UISkillNeedLevel = 20,                                  //需要等级
    UISkillNeedMoney = 21,                                  //需要金钱
    UISKillCD = 22,                                         //技能CD
    //技能升级界面
    UISkillLevelUpTitle = 23,                               //提示
    UISkillLevelUpSkillName = 24,                           //技能名称
    UISkillLevelUpNeedMoney = 25,                           //需要金钱
    UISkillLevelUpLevel = 26,                               //升级等级
    UISkillLevelUpCD = 27,                                  //技能CD
    //任务界面
    UIMissionExp = 28,                                      //任务经验
    UIMissionBindMoney = 29,                                //绑定金币
    UIMissionRMBMoney = 30,                                 //RMB
    //玩家身上任务界面
    UIPlayerMissionContent = 31,                            //任务内容描述
    UIPlayerMissionTarget = 32,                             //任务目标描述
    UIPlayerMissionExp = 33,                                //任务经验
    UIPlayerMissionBindMoney = 34,                          //绑定金钱
    UIPlayerMissionRMBMoney = 35,                           //RMB
    //装备强化界面
    UIEquipstrengthenAttackSpeed = 36,                      //攻击速度
    UIEquipstrengthenHPMax = 37,                            //血量上限
    UIEquipstrengthenPhyAttack = 38,                        //物理攻击
    UIEquipstrengthenMagAttack = 39,                        //魔法攻击
    UIEquipstrengthenPhyDefend = 40,                        //物理防御
    UIEquipstrengthenMagDefend = 41,                        //魔法防御
    UIEquipstrengthenHit = 42,                              //命中
    UIEquipstrengthenAvoid = 43,                            //闪避
    UIEquipstrengthenCriAttack = 44,                        //暴击
    UIEquipstrengthenStillAttack = 45,                      //连击
    UIEquipstrengthenStillDefend = 46,                      //韧性
    UIChatMessageWorldChannel = 47,                         //世界
    UIChatMessageSystemChannel = 48,                        //系统
    UIChatMessageCityChannel = 49,                          //城市
    UIChatMessageTeamChannel = 50,                          //组队
    UIPrivateChatChoosePrivate = 51,                        //私聊
    UIPrivateChatChooseTeam = 52,                        //组队
    UIPrivateChatChooseExit = 53,                        //取消
    UIPrivateChatButtonNameAdd = 54,                    //私聊按钮名字太长截取添加
    //UIEquipment 
    UIEquipmentBindMoney = 55,                          //金锭
    UIEquipmentRMBMoney =56,                            //勾玉

    UISameNameAffirmStr = 57,
    UIChatWorldChannelNameStr = 58,
    UIChatCityChannelNameStr = 59,
    UIChatTeamChannelNameStr = 60,
    UILevelUpStr              = 61,                     //恭喜升级X级
    UIStudySkillStr           = 62,                     //学习X技能
    UILevelUpSkillStr           = 63,                   //X技能升级到X级
    UIDeadStr                   = 64,                   //您已经阵亡了
    UIEquitStrengSussStr        = 65,                   //装备强化成功
    UIEquitStrengFailStr        = 66,                   //装备强化失败
    UIBuyItemShowStr            = 67,                   //购买了X
    UIGetItemShowStr            = 68,                   //获得了X
    UINotEnoughBindMoneyStr     = 69,                   //金锭不足
    UINotEnoughRMBMoneyStr      = 70,                   //勾玉不足
    UIAddInTeamStr              = 71,                   //X加入了队伍
    UIExitTeamStr               = 72,                   //X离开了队伍
    UIDisBandTeamStr            = 73,                   //X解散了队伍
    UICompleteCopyStr           = 74,                   //完成X难度X副本
    UIGetBuffStr                = 75,                   //获得Xbuff
    UIKillNpcStr                = 76,                   //已杀死X
    UIAcceptQuestStr            = 77,                   //接受任务
    UICompleteQuestStr          = 78,                   //完成任务
    UIAbandonQuestStr           = 79,                   //放弃任务
    UIGetExpStr                 = 80,                   //获得X经验
    UIGetBindMoneyStr           = 81,                   //获得X金锭
    UIGetRMBMoneyStr            = 82,                   //获得X勾玉
    UINumPerStr                 = 83,                   //数字比例显示X/X
    UINotEnoughEnergyStr        = 84,                   //体力值不够
    UITeamInviteStr             = 85,                   //X邀请你加入队伍
    UIRequireJoinTeamStr        = 86,                   //X申请加入队伍
    UITeamDungeonErrorStr       = 87,                   //这个难度必须组队
    UINotEnougnTeamVolume       = 88,                   //X组队卷不足
    UISkillNeedLevel2           = 89,                   //需要等级:
    UISkillCanLearn             = 90,                   //可学习
    UISkillLevel                = 91,                   //等级
    UISkillLevelNotEnough       = 92,                   //等级不足
	UISkillSkillLevel			= 93,                   //技能等级:
	UIConnectClosed				= 94,                   //你已断开连接
    ConnectClosed               = 95,
    UINpcShopUseGold            = 96,                   //优先使用金锭
    UINpcShopUseRMB             = 97,                   //优先使用勾玉
    TeamVolume                  = 98,                   //组队券
    Adventurers                 = 99,                   //冒险者
    Friend                      = 100,                  //好友
    GroupMember                 = 101,                  //公会成员
    MAX,
}

// 装备资质表
public class StringInfo : IDataBase
{
    public int ID { get; protected set; }            //ID
    public string Name { get; protected set; }       //名称
}

public class StringTable
{
/*    public Dictionary<int, StringInfo> StringInfoList { get; protected set; }*/
    public StringInfo[] m_stringInfoList;
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        m_stringInfoList = new StringInfo[(int)ENStringIndex.MAX];
        for (int index = 0; index < length; ++index)
        {
            StringInfo info = new StringInfo();
            info.Load(helper);
            m_stringInfoList[info.ID] = info;
        }
    }

    public string GetString(ENStringIndex index)
    {
        if (index == ENStringIndex.enNone || index == ENStringIndex.MAX)
        {
            return "";
        }
        if (m_stringInfoList[(int)index] == null)
        {
            return "";
        }
        return m_stringInfoList[(int)index].Name;
    }
}