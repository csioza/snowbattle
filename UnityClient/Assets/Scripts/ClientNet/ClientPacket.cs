using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NGE.Network;
using System.IO;

public struct GamePacketID
{
	public const short ENMsgPlayerData_S2D2C = 115;
	public const short ENMsgSyncItemBag_S2C = 117;
    public const short ENMsgSyncResultTree_S2C = 118;
	public const short ENMsgAskPlayerData_C2S = 121;
    public const short ENMsgAskInitCard_C2S = 122;
    public const short ENMsgLogin_C2S = 150;
    //登录
    public const short msgLogin_C2S = 101;
    public const short msgLogin_S2C = 102;

    public const short ENMsgKickSelf_C2S = 120;
    //创建角色
    public const short msgPlayerCreate_S2C = 103;
    public const short msgPlayerCreate_C2S = 104;
    //初始化角色数据
    public const short msgPlayerInit_S2C = 105;
    //场景切换
    public const short msgSceneSwitch_C2S = 159;
    public const short msgSceneSwitch_S2C = 160;
    //朋友相关 201开始
    //初始化朋友列表
    public const short msgFriendList_S2C = 201;
    //朋友-添加
    public const short msgFriendAdd_C2S = 202;
    public const short msgFriendAdd_S2C = 203;
    //朋友-同意添加
    public const short msgFriendAdd_Agree_C2S = 204;
    public const short msgFriendAdd_Agree_S2C = 205;
    //朋友-拒绝添加
    public const short msgFriendAdd_Refuse_C2S = 206;
    public const short msgFriendAdd_Refuse_S2C = 207;
    //朋友-删除
    public const short msgFriendDel_C2S = 208;
    public const short msgFriendDel_S2C = 209;
    //朋友-被删除
    public const short msgFriendBeDel_S2C = 210;
    //朋友-聊天
    public const short msgFriendChat_C2S = 211;
    public const short msgFriendChat_S2C = 212;
    //朋友-登录、登出
    public const short msgFriendLogin_S2C = 213;
    public const short msgFriendLogout_S2C = 214;
	public const short ENMsgServerLog_S2C = 222;

	public const short ENMsgMessageRespond_S2C = 999;
    public const short ENMsgSessionError_S2C = 1000;
	public const short ENMsgAccountLogin_C2S = 1001;

    public const short ENMsgAccountOperateChar_C2S = 1003;
    public const short ENMsgAccountOperateChar_S2C = 1004;
    public const short ENMsgAccountLoginResult_S2F2C = 1005;



	public const short ENMsgServerPlayerGetReady_S2Ce2Fe2C = 1015;

	public const short ENMsgSyncBagItem_S2C = 1018;
	public const short ENMsgDelBagItem_S2C = 1019;
	public const short ENMsgSyncFloorInst_S2C = 1021;	//同步单个关卡实例给客户端 	
	
	public const short ENMsgHerocardAddexp_C2S = 1023;  //加经验
	public const short ENMsgHerocardMerge_C2S = 1024;   //合成
	public const short ENMsgHerocardEvolve_C2S = 1025;  //进化
	public const short ENMsgSetRepresentativeCard_C2S = 1026;	//请求改变代表卡牌
	public const short ENMsgSetRepresentativeCard_S2C = 1027;	//返回改变代表卡牌
	public const short ENMsgSellHeroCard_C2S = 1028;			//卖卡牌
    public const short ENMsgExpandCardBagCapacity_C2S = 1030; 		//扩背包
    public const short ENMsgSendShopItemInfo_C2S = 1033;        //发送请求商店道具信息
    public const short ENMsgSendMagicStoneInfo_S2C = 1034;      //接受服务器魔法石商店数据
    public const short ENMsgSendBuyMagicStone_C2S = 1035;       //发送确认购买魔法石消息
    public const short ENMsgExpandCardBagCapacity_S2C = 1036;	//通知客户端扩展卡牌背包格子
    public const short ENMsgRecruitment_C2S = 1039;//发送到服务器 招募消息
    //public const short ENMsgRecoverStaminaSucc_S2C = 1042;//接受服务器发送的恢复体力成功
    //public const short ENMsgRecoverEnergySucc_S2C = 1043;//接受服务器发送的恢复军资成功
    public const short ENMsgRecoverSucc_S2C = 1043;//接受服务器发送的成功消息
    //public const short ENMsgMagicStoneRecruitmentSucc_S2C = 1044; //发送到客户端魔法石招募成功
    public const short ENMsgFriendShipMoreRecruitmentSucc_S2C = 1045; //发送多次友情点数招募成功
    //public const short ENMsgFriendShipOnceRecruitmentSucc_S2C = 1046; //发送一次友情点数招募成功
    public const short ENMsgSendRingOfHonorInfo_S2C = 1048; //发送到客户端的荣誉戒指列表
    public const short ENMsgSendBuyRingOfHonorCard_C2S = 1049;//发送到服务器购买荣誉戒指卡片
    public const short ENMsgSendChangeName_C2S = 1051;//发送到服务器更改姓名
    public const short ENMsgSendChangeNameSucc_S2C = 1052;//发送到客户端改名成功

	public const short ENMsgSyncDirtyPropertys_S2C = 1099;	//同步dirtyProps
	public const short ENMsgSyncServerTime_S2C	= 1100; 	//同步服务器时间
	public const short ENMsgSelectTeamIndex = 1101;
// 	public const short ENMsgSyncLevel_S2C		= 1101; 	//同步等级， 经验
// 	public const short ENMsgSyncMoney_S2C		= 1102;		//金币
// 	public const short ENMsgSyncBindMoney_S2C	= 1103;		//钻石
// 	public const short ENMsgSyncRing_S2C		= 1104;		//戒指
// 	public const short ENMsgSyncFriendshipPoint_S2C = 1105;	//友情
// 	public const short ENMsgSyncStamina_S2C		= 1106;		//体力
	public const short ENMsgSaveOneTeamData_C2S = 1107;		//向服务器发送队伍信息
// 	public const short ENMsgOneTeamData_S2C		= 1108;		//服务器返回某队伍信息
// 	public const short ENMsgSyncEnergy_S2C		= 1109; 	//军资

	public const short ENMsgReqBattleHelperList_C2CH = 1110; 	//客户端请求战友列表
	public const short ENMsgSyncBattleHalper_CH2C = 1111;		//发送战友卡
	public const short ENMsgSelectBattleHelper_C2CH = 1112;		//客户端选择战友卡进入副本。
	//副本相关
	public const short ENMsgEnterDugeon_C2S		= 1120;			//进副本
	public const short ENMsgEnterDugeon_S2C		= 1121;			//进副本回复
	public const short ENMsgDungeonSettlement_C2S = 1122;		//副本结算
	public const short ENMsgDungeonEnd_S2C		= 1123;			//结束副本
	public const short ENMsgExitDungeon_C2S		= 1124;			//退出副本
	public const short ENMsgFloorReliveOption_C2S = 1125;		//关卡复活选项
	public const short ENMsgFloorReliveReslut_S2C = 1126; 		//复活结果
    public const short ENMsgCardDivisionUpdate_C2S = 1127;      //发送到服务器卡牌段位升级
    public const short ENMsgSendUpdateCardSucc_S2C = 1128;      //服务器发送到客户端的 卡牌升级成功消息
	//chat消息
	public const short ENMsgSyncPlayerDataToChat_CE2CH	= 2001; 	//登陆chat
	public const short ENMsgLookupPlayer_C2CE			= 2002;		//查找玩家。
//	public const short ENMsgLookupPlayerResult_CH2C		= 2003;		//查找玩家结果
	public const short ENMsgSocialityItem_CH2C			= 2004; 	//一个玩家的信息
	public const short ENMsgSocialityItemList_CH2C		= 2005;		//玩家信息的列表
	public const short ENMsgGetSocialityList_C2CH		= 2006;		//获取社交列表（好友）
	public const short ENMsgAddFriend_C2CH				= 2007;		//加好友
	public const short ENMsgDelFriend_C2CH				= 2008;		//删除好友	
	public const short ENMsgRemoveSocialityItem_CH2C	= 2009;		//通知客户端删除一条
    public const short ENMsgPassFriendRequest_C2CH		= 2010;     //通过加好友的验证
	public const short ENMsgSendMail_C2S				= 2022;		// 发送邮件。
	public const short ENMsgInitCard_S2C				= 2100;		// 初始化卡牌

	//--------------------------测试消息-------------------------
	public const short ENMsgTestTest = 1002;
	public const short ENMsgPlayerGetCard_C2S = 1020;
    public const short ENMsgAskItemBag_C2S = 1022;
    public const short ENMsgGMCmd_C2S = 5000;

    #region//长连接消息（消息id从10003开始）
    public const short ENMsgEnterLCDugeon_S2C = 10000;    //进入副本时，回复是进入长连接的副本(跟长连接有关，故放在这里)
    //BS==BattleServer
    public const short ENMsgLogin_C2BS = 10003;//登录到battleServer
    public const short ENMsgMessageRespond_BS2C = 10004;//battleServer统一回复的消息
    public const short ENMsgEnterDugeon_BS2C = 10005;//进入副本
    public const short ENMsgTest_C2BS = 10006;//测试（客户端向BattleServer）
    public const short ENMsgCreateResultC2BS = 10008;// 创建result的消息
    public const short ENMsgSyncActor_BS2C = 119;//广播消息
    public const short ENMsgSyncPropertySet_S2C = 116;//同步服务器脏属性
    public const short ENMsgError_BS2S = 10009;//battleServer发给zoneServer的错误信息
    public const short ENMsgDepositNpcAi_BS2C = 10010;//托管npcai
    public const short ENMsgLeaveDugeon_C2BS = 10011;//离开场景
    public const short ENMsgTick_C2BS = 10012;//tick
    public const short ENMsgSyncPosition_C2BS = 10013;//同步位置
	public const short ENMsgDotaMapInfo_BS2C = 10014;//Dota地形配置文件信息
    public const short ENMsgAction_Move_C2BS = 10015;//移动action
    public const short ENMsgSyncAction_Move_BS2C = 10016;//同步移动action
    public const short ENMsgAction_Attack_C2BS = 10017;//攻击action
    public const short ENMsgSyncAction_Attack_BS2C = 10018;//同步攻击action
    public const short ENMsgAction_BeHit_C2BS = 10019;//受击action
    public const short ENMsgSyncAction_BeHit_BS2C = 10020;//同步受击action
    public const short ENMsgAction_Roll_C2BS = 10021;//翻滚action
    public const short ENMsgSyncAction_Roll_BS2C = 10022;//同步翻滚action
    public const short ENMsgAction_ActorEnter_C2BS = 10023;//主控角色进入action
    public const short ENMsgSyncAction_ActorEnter_BS2C = 10024;//同步主控角色进入action
    public const short ENMsgAction_ActorExit_C2BS = 10025;//主控角色退出action
    public const short ENMsgSyncAction_ActorExit_BS2C = 10026;//同步主控角色退出action
    public const short ENMsgAction_JumpIn_C2BS = 10027;//跳入action
    public const short ENMsgSyncAction_JumpIn_BS2C = 10028;//同步跳入action
    public const short ENMsgAction_JumpOut_C2BS = 10029;//跳出action
    public const short ENMsgSyncAction_JumpOut_BS2C = 10030;//同步跳出action
    public const short ENMsgActionInterupt_C2BS = 10031;//action被打断
    public const short ENMsgSyncActionInterupt_BS2C = 10032;//同步action被打断
    public const short ENMsgSceneFinished_BS2C = 10033;//广播场景结束
    public const short ENMsgPlayerLeaveScene_BS2C = 10034;//广播player离开场景
    public const short ENMsgComboNum_BS2C = 10035;  // 更新combo数量到服务器
    public const short ENMsgAction_AttackingMove_C2BS = 10036;//attacking move action
    public const short ENMsgSyncAction_AttackingMove_BS2C = 10037;//attacking move action
    public const short ENMsgTestAllSkill_C2BS = 10038;//测试所有技能
    public const short ENMsgLevelUpSkill_C2BS = 10039;//发送技能升级消息
    public const short ENMsgRecoverLevelUpSucc_BS2C = 10040;//服务器发送到客户端的升级技能成功
    #endregion
}
//同步信息的类型
public enum ENSyncActorType
{
    enSyncViewable = 0,//别人-可见
    enSyncRemoveView = 1,//别人-不可见
    enSyncSelf = 2,//自己
}
//
public enum ENSobTag
{
    enSobNpc = 2,
    enChiefPlayer = 3, //卡牌-主角色
    enDeputyPlayer = 4, //卡牌-副角色
    enSupportPlayer	= 5, //卡牌-支援角色
    enComradePlayer	= 6, //卡牌-同伴
}

public class Loginpacket : Packet
{
    /*  登录
     *	user:用户名
     *	pass:密码
     */
    public Loginpacket(string user, string pass)
		: base(GamePacketID.ENMsgAccountLogin_C2S)
    {
        m_writer.WriteAscii(user, 64);
        m_writer.WriteAscii(pass, 32);
    }
}
public class OperateCharPacket : Packet
{
    public OperateCharPacket(short operatorType, short charIndex,int charCount,string name)
        : base(GamePacketID.ENMsgAccountOperateChar_C2S)
    {
        m_writer.Write(operatorType);
        m_writer.Write(charIndex);
        m_writer.Write(charCount);
        m_writer.WriteUTF8(name, 16);
    }

}

public class TestTestPacket : Packet
{
    public TestTestPacket(int testNum)
        : base(GamePacketID.ENMsgTestTest)
    {
        m_writer.Write(testNum);
    }
}

public class PlayerCreatePacket : Packet
{
    /*  创建角色
     *	name:创建的角色名称
     */
    public PlayerCreatePacket(string name)
        : base(GamePacketID.msgPlayerCreate_C2S)
    {
        m_writer.WriteUTF8(name, 64);
    }
}
public class FriendAddPacket : Packet
{
    /* 朋友-添加
     * myName：角色的名字
     * friendName：朋友的名字
     */
    public FriendAddPacket(string myName, string friendName)
        : base(GamePacketID.msgFriendAdd_C2S)
    {
        m_writer.WriteUTF8(myName, 64);
        m_writer.WriteUTF8(friendName, 64);
    }
}
public class FriendAddAgreePacket : Packet
{
    /* 朋友-同意添加
     * myName：角色的名字
     * friendName：朋友的名字
     */
    public FriendAddAgreePacket(string myName, string friendName)
        : base(GamePacketID.msgFriendAdd_Agree_C2S)
    {
        m_writer.WriteUTF8(myName, 64);
        m_writer.WriteUTF8(friendName, 64);
    }
}
public class FriendAddRefusePacket : Packet
{
    /* 朋友-拒绝添加
     * myName：角色的名字
     * friendName：朋友的名字
     * reason：失败的原因
     */
    public FriendAddRefusePacket(string myName, string friendName, string reason)
        : base(GamePacketID.msgFriendAdd_Refuse_C2S)
    {
        m_writer.WriteUTF8(myName, 64);
        m_writer.WriteUTF8(friendName, 64);
        m_writer.WriteUTF8(reason, 256);
    }
}
public class FriendDelPacket : Packet
{
    /* 朋友-删除
     * myName：角色的名字
     * friendName：朋友的名字
     */
    public FriendDelPacket(string myName, string friendName)
        : base(GamePacketID.msgFriendDel_C2S)
    {
        m_writer.WriteUTF8(myName, 64);
        m_writer.WriteUTF8(friendName, 64);
    }
}
public class FriendChatPacket : Packet 
{
    /* 朋友-聊天
     * myName：角色的名字
     * friendName：朋友的名字
     * info：聊天的内容
     */
    public FriendChatPacket(string myName, string friendName, string info)
        : base(GamePacketID.msgFriendChat_C2S)
    {
        m_writer.WriteUTF8(myName, 64);
        m_writer.WriteUTF8(friendName, 64);
        m_writer.WriteUTF8(info, 256);
    }
}


public class TestPacket : Packet
{
    /*  登录
     *	user:用户名
     *	pass:密码
     */
    public TestPacket()
        : base(GamePacketID.ENMsgTestTest)
    {
        string session = "abc1abc2";
        m_writer.WriteAscii(session, 64);
    }
}