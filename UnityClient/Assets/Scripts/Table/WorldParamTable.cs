using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

public enum ENWorldParamIndex
{
    enNone = 0,
    enTeamValueMax = 1,//组队最大值
    enAddPhysicalPowerTime = 2,//回复体力时间
    enAddPhysicalPowerValue = 3,//回复体力值
    enNindMoneyValueMax = 4,//携带绑定金钱最大上限
    enAptitudeFloatValue = 5,//资质浮动值
    enBindMoneyType = 6,//绑定金钱类型
    enRMBMoneyType = 7,//人民币金钱类型
    en1LevelDifferentType = 8,//1级异类
    en1LevelSameType = 9,//1级同类
    en1LevelSameID = 10,//1级同ID
    enEquipLevelMax = 11,//装备等级上限
    enEquipBuffRandomNum = 12,//装备Buff随机个数
    enStartSkillbarTimeMax = 13,//技能条最大读条时间
    enStartSkillbarDelayTime = 14,//读条时延迟时间
    enSkillbarPartNum = 15,//蓄力技能最大段数
    enSkillbarSkillCountDown = 16,//蓄力技能倒计时
    enSkillbarSkillPartNumMin = 17,//蓄力技最少段数
    enSkillbarSkillType = 18,//蓄力技技能类型
    enSkillbarSkillEND = 19,//蓄力技能结束提示语
    enPlayerChatMsgNumMax = 20,//玩家聊天保存的最大消息数
    enSkillbarSkillFirstPartTime = 21,//蓄力技第一段需要时间
    enSkillbarSkillSecondPartTime = 22,//蓄力技第二段需要时间
    enSkillbarSkillThirdPartTime = 23,//蓄力技第三段需要时间
    enUIPrivateChatNum = 24,//私聊气泡显示个数
    enUIPrivateChatMaxMessage = 25,//私聊消息上限

    enChatUIHigh        = 26,
    enPrivateChatUIHigh = 27,
    enChatUISpace       = 28,
    enPrivateChatUISpace = 29,
    enSpSkillBuffID = 30,
    enPrivatePanelButtonSpace = 31,
    enSpSkillAccountID = 32,//怒气技结算ID
    enPrivateChatNameLength = 33,
    enSkillbarLengthScale = 34,//蓄力条长度比例
    enMoneyDropIcon        =35,
    enRMBMoneyDropIcon      = 36,
    enTeamVolumeDropIcon    = 37,
    enPlayerGuideSceneID = 38,
    enAttackActionName      = 39,
    enAttackedActionName    = 40,
    enArrowHideTime         = 41,
    enNewPlayerGuideDungeonID = 43,
    enBlockDeleteAnimation = 44,//栅栏碎裂动作
    enBlockFragment = 45, //栅栏碎片
    enDungeonOverGradeTime = 46,//副本结算弹出延迟时间
	enClickGroundEffectDurationTime = 47,//鼠标点击地面产生的光圈持续时间
	//enComboParam = 48,//combo参数
    enUrgentEventParam              = 49, //紧急事件触发百分比
    enUrgentEventDurationParam      = 50, //紧急事件持续时间，单位：秒
    //combo评价
    enComboNice = 51,
    enComboCool = 52,
    enComboGreat = 53,
    enComboBravo = 54,
    enComboPerfect = 55,
    enComboEvaluationHideTime = 56, //combo评价消失时间
    enChangeTargetDistance = 57, //切怪范围
    enComboScaleTime = 58, //combo放大的时间
    enClickGroundEffectName = 59, //点击地面的特效名字
    enCriAttackParam = 60, //暴击参数
    enSwitchExitTimer = 61, //切入技退场时间
    enComboCountHideTime = 62, //combo count消失时间
    enCardBagID          = 63,//卡牌背包ID
    enEquipBagID         = 64, // 装备背包ID
    enMaterialBagID       = 65,// 材料背包ID
    enMainPlayerRestoreHP = 66, //切换角色恢复hp百分比
    enMainPlayerSwitchBuffID = 67, //切换角色添加的buff

    enThrowGoldStartAngle   = 68,//投掷金币随机角度的初始角度
    enThrowGoldEndAngle     = 69,//投掷金币随机角度的初始角度
    enThrowGoldAnimationMax = 70,//随机金币动画的动画名最大值

    enBattleFinishTime      = 71,// 战斗结束后 结算界面出现的延迟时间
    enHpBarFlashJudgeTime = 72,//血条flash判定的时间
    enComboCountOffset = 73,//连击数字显示的偏移
    enSkillConnectionDuration = 74,//技能连接间隔
    enComboTimeOfSwitchActor = 75, //切换角色时，combo的增加时间
    enKillNPC_DoubleKill = 76,//多重击杀-双杀
    enKillNPC_TripleKill = 77,//多重击杀-3杀
    enKillNPC_QuadraKill = 78,//多重击杀-4杀
    enKillNPC_KillingSpree = 79,//多重击杀-5杀
    enKillNPC_Rampage = 80,//多重击杀-6杀
    enKillNPC_Unstoppable = 81,//多重击杀-7杀及以上
    enKillNPC_ShowDuration = 82,//多重击杀-显示存在时间
    enKillNPC_JudgeTime = 83,//多重击杀-判定时间
    enActroState_FightDuration = 84,//角色状态-战斗状态解除时的持续时间
    enAdventurerFriendPoint = 85,// 战友冒险者 所获得友情点数
    enFriendPoint = 86,// 好友友情点数
	enStaminaRecoverTime = 87,//体力恢复时间
    enBagShowItemNum = 88,// 背包一直显示的 格子数目
    enMagicStoneRecruitmentNum = 89,//魔法石招募消耗数量
    enFrinendShipRecruitmentNum = 90,//友情点数招募消耗数量
    enRecoverStaminaMagicStoneNum = 91,//恢复体力消耗魔法石数量
    enRecoverStaminaNum = 92,//恢复体力数量
    enRecoverEnergyMagicStoneNum = 93,//恢复能量消耗魔法石数量
    enRecoverEnergyNum = 94,//恢复军资数量
    enTeamRoleMaxNum = 95, //每个队列里边最大人数
    enActorLevel_Basic = 96,//初级玩家等级上限
    enActorLevel_Middle = 97,//中级玩家等级上限
    enActorLevel_High = 98,//高级玩家等级上限
    enQTECDTime_Basic = 99,//初级玩家QTE冷却时间
    enQTECDTime_Middle = 100,//中级玩家QTE冷却时间
    enQTECDTime_High = 101,//高级玩家QTE冷却时间
    enQTETableIDList_Basic = 102,//初级玩家QTE序列id列表
    enQTETableIDList_Middle = 103,//中级玩家QTE序列id列表
    enQTETableIDList_High = 104,//高级玩家QTE序列id列表
    enQTEDuration_NoSkill = 105,//处于战斗状态后几秒内没有释放技能
    enQTESkillCount_Basic = 106,//初级玩家QTE触发所需技能数量
    enQTESkillCount_Middle = 107,//中级玩家QTE触发所需技能数量
    enQTESkillCount_High = 108,//高级玩家QTE触发所需技能数量
    enSkillConnectionTime = 109,//连接技时间
    enSkillSpecialTime = 110,//特殊技时间
    enSkillComboHideTime = 111,//SkillCombo隐藏的时间
    enSkillComboAddTime = 112,//SkillCombo补充时间
    enNextQteModifyTime = 113,//下个qte按键和cd被显示时的修正时间
    enQteSingleCDTime = 114,//qte指令的计时时间
    enExtendBagTipsIconId = 115,//成功扩展背包提示界面IconID
    enExtendFrindTipsIconId = 116,//成功扩展好友提示界面IconID
    enRecoverStaminaIconId  = 117,//成功恢复体力提示界面IconID
    enRecoverEnergyIconId   = 118,//成功恢复能量提示界面IconID
    enQteSequenceHideTime = 119,//UIQTESequence隐藏的时间
    enSoulChargeHideTime = 120,//UISoulCharge隐藏的时间
    enQteExtraReward = 121,//qte额外奖励
    enSoulChargeLevelup = 123,//剑魂级别经验
    enRecruitTimesMax      = 122,//友情点数招募最大值
	enEnergyRecoverTime		= 124,//军资恢复一点，需要的时间（秒）
    enMinComboNumber = 127,//combo，最小连击数
    enMinComboStepModify = 128,//combo,修正最小combo步长
    enMorePercent = 129,//combo,多段轻攻击提升百分比
    enSomePercent = 130,//combo,中段中攻击提升百分比
    enLittlePercent = 131,//combo,少段重攻击提升百分比
    enZoneAnimationDelayTime = 132, // zone界面 点击显示名称的延迟时间
    enCreatCardList = 133, // zone界面 点击显示名称的延迟时间
    enPartnerFollowDistance = 134,//战友角色 跟随距离
    enPartnerStopFollowDistance = 135,//战友角色 停止跟随距离
	enInitFriendCount         = 136, //初始好友数量
	enMaxFriendCount          = 137, //游戏中好友数量上限
    enTeleportHideEffect = 139,//瞬移消失时特效
    enTeleportShowEffect = 140,//瞬移显示时特效
    enExpandFriendsNeedMagicStone = 143, //扩充好友所需魔法石
    enExpandFriendsNum = 144, //扩充好友数量
    enWillValue = 146,//战斗意志-最大值和最小值
    enTargetExistGet = 147,//战斗意志-注视增益
    enBeAttackedGet = 148,//战斗意志-受击增益
    enTimeflowGet = 149,//战斗意志-正常时间减益
    enFightWillLose = 150,//战斗意志-意志燃烧减益
    enFightWillParam = 151,//战斗意志-y值
    enFightWillBuffid = 152,//战斗意志-意志燃烧添加的buffid
    enGetTargetDuration = 153,//同伴AI-主角选中目标的时长
    enConnectFailed = 154,//连接超时时长
    enbattleBtnNormal = 155, //技能ICON普通状态下的材质名称及路径
    enbattleBtnDisable = 156, //技能ICON灰色状态下的材质名称及路径
    enDamagedCounterParam = 157, //受到伤害后反击的参数
    enBagGridBgIconId = 159, //头像背景图片ICON ID
    enBagExpandIconId = 160, //扩充营地按钮图片Icon ID
    enUnlockHeadImgIconId = 161, //未解锁图鉴头像ICON ID
    enPhyAttackMinValue = 162,//物理攻击力最小值
    enMagAttackMinValue = 163,//魔法攻击力最小值
    enPhyHealthMinValue = 164,//物理恢复力最小值
    enMagHealthMinValue = 165,//魔法恢复力最小值
    enEvadingValueId = 166,//检测的避让距离ID
    enEvadingDistanceId = 167,//避让距离
    enDefaultCardId   = 168, //招募时容错的卡牌ID

    enJoinLoveImageName = 169,//加入最爱图片名称
    enCancelLoveImageName = 170,//取消最爱图片名称
    enPartnerForceMoveDistance = 171,//同伴的瞬移距离
    enPartnerMaxFindPathTime = 172,//同伴无法寻路最大时间间隔
    enSwordSoulMinNormal = 173,//剑魂常规结算中的最小值（常规结算必须大于等于这个值）
    enSwordSoulMaxValue = 174,//剑魂最大值
    enDropGoldMinMax = 175,//宝箱中弹出金币数量的区间，包含最大最小数值
    enConTimeOutCounts = 176,//在战斗中弹出的“连接超时”提示次数超过此次数后弹出可选择的二次确认界面

    enCardDivisionNullMaterialIcon = 179,//卡牌升段界面空素材图片
    enCardDivisionUnAppointIcon = 180,//非指定素材图片
    enCardDivisionRingHonorIcon = 181,//荣誉戒指图片
    enCardDivisionMaterialStarLevel = 188,//稀有素材卡牌星级等级

    enCardDetailSkillFrame = 197, // 卡牌详情的 技能边框ICONID
    enCardDetailSpecilSkillFrame = 198,// 卡牌详情的 切入技能框ICONID

    enActiveSkill = 199,// 终结技TIPS中激活时显示的文字本地化Key
    enConectSkill = 200,// 连接技名称本地化Key
    enSpecialSkill = 201,// 特殊技名称本地化Key
    enFinalSkill = 202,// 终结技名称本地化Key
    enMaxCombo = 203,// 终结技Tips额外Combo效果最大显示数量（包括激活技能Combo显示）

   
    enDamageRandomParam           = 204, //--     伤害随机值
    enDamageReduceParam           = 205,// --     减伤系数
    enHighestDRParam              = 206 ,//--     最高减伤系数
    enMaxLevelAttrParam           = 207 ,//--     属性满级系数

    enUserNameAlreadyExist  = 208,//--    用户名已存在
    enUserNameIllegal       = 209,//--    用户名不合法
    enUserNameTooShort      = 210,//--    用户名太短
    enEnsurePasswordWrong   = 211,//--    确认密码不正确
    enPasswordTooShort      = 212,//--    密码太短
    enPasswordIllegal       = 213,//--    密码内含有无效字符
    enUserNameNotExist      = 214,//--    用户名不存在
    enPasswordWrong         = 215,//--    密码错误，请重新输入密码

    Count,
}



public class WorldParamInfo : IDataBase
{
    public int ID { get; protected set; }
    //Int类型
    public int IntTypeValue { get; protected set; }
    //Float类型
    public float FloatTypeValue { get; protected set; }
    //String类型
    public string StringTypeValue { get; protected set; }
    //描述
    public string Describe { get; protected set; }
   
   
}

public class WorldParamTable
{
    public WorldParamInfo Lookup(int id)
    {
        WorldParamInfo info;
        WorldParamInfoList.TryGetValue(id, out info);
        return info;
    }
    public SortedList<int, WorldParamInfo> WorldParamInfoList { get; protected set; }
    public void Load(byte[] bytes)
    {
        BinaryHelper helper = new BinaryHelper(bytes);
        int length = helper.ReadInt();
        WorldParamInfoList = new SortedList<int, WorldParamInfo>(length);
        for (int index = 0; index < length; ++index)
        {
            WorldParamInfo info = new WorldParamInfo();
            info.Load(helper);
            WorldParamInfoList.Add(info.ID, info);
        }
    }
}

