using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//备注 (php server)
// 一 发送:
//①所有的消息,在这里写一份虚函数,在MiniServer中实现.
//②在HttpReqTypes里加入ID和地址的映射
// 二 接收
//①在ServerMsgAnalyzer中注册解析回调.并将消息解析成ServerMsgStruct.
//②在ServerMsgHandler中注册解析完之后的回调.具体的执行函数

/// <summary>
/// 发送消息的ID在HttpReqType中定义映射
/// 收到的消息在ServerMsgHandler定义回调
/// </summary>
public abstract class IMiniServer
{
	#region singleton
	private static IMiniServer m_miniServer;
	public static IMiniServer Singleton
	{
		get
		{
			if (m_miniServer == null)
			{
				m_miniServer = new MiniServer();
			}
			return m_miniServer;
		}
	}
	#endregion
	public abstract int gateserver_globalconfig();
	public abstract int account_serverconfig();
	public abstract int account_get_zone();
	public abstract int account_register(string accname, string accpwd);
	public abstract int account_check_local(string accname, string accpwd);
	public abstract int account_login(string accname,string token);
	public abstract int account_change_pwd(string accname, string oldpwd, string newpwd);
	public abstract int user_create(string name, int portrait,int herocard_id);
	public abstract int herocard_carete(int card_id);
	public abstract int herocard_merge(int maincard, string material_cards);
	public abstract int herocard_evolve(int maincard, string material_cards);
    
	//登陆，c++和php都用。。
	public abstract int user_login(string accname, string accpwd);

	//---下面是C++服务端目前的消息
    public abstract int user_create_char(string name);
	public abstract int user_ask_playerData();
	public abstract int player_get_card(int cardID,int cardLevel);
	public abstract int user_ask_bagData();

	public abstract int herocard_addExp(CSItemGuid cardGUID,int exp);
	public abstract int req_herocardMerge(CSItemGuid cardGUID, List<CSItemGuid> foodCards);
	public abstract int req_herocardEvolve(CSItemGuid cardGUID);
	public abstract int req_setRepresentativeCard(CSItemGuid cardGUID);
	public abstract int req_battleHelperList();

	public abstract int req_expandBagCapacity(int cost);
    //public abstract int req_recoverStamina();
    public abstract int SendEnterDungeon(int dungeonID, int myStamina, int camp, string key, bool isRolling);
	
	public abstract int SendDungeonSettlement(int dungeonID, int time, int combo, int killBoss, int reSpawn, int score, int exp, int money, int awardID ,List<int> dropList = null );
	public abstract int SendExitDungeon(int dungeonID);
	public abstract int SendBuyMagicStone_C2S(int magicStoneId);

	public abstract int SendSelectTeamIndex(int teamIndex);
	public abstract int SendSaveOneTeamData(int teamIndex);
    public abstract int SendExpandCardBag_C2S(int expandType);
    //public abstract int SendMsgRecoverStamina_C2S();
    //public abstract int SendMsgRecoverEnergy_C2S();
	public abstract int SendBuyRingOfHonorCard_C2S(int ringInfo, int cardId);
	public abstract int SendSellHeroCard_C2S(List<CSItemGuid> cardList);
	public abstract int SendLookupPlayer_C2CE(int charID);
	public abstract int SendGetSocialityList_C2CH(int getType = (int)FriendItemType.enFriend);
	public abstract int SendAddFriend_C2CH(int charID);
	public abstract int SendDelFriend_C2CH(int charID);
    public abstract int SendChangeName_C2S(string changeName);
	public abstract int InitCard_C2S();
	public abstract int SendPassFriendRequest(int targetID);
    public abstract int SendRecruitment_C2S(int RecruitmentType);
    public abstract int SendShopItemInfo_C2S(int shopType);
    public abstract int SendGetCardById_C2S(int RecruitmentType, int cardId);

	public abstract int SendFloorReliveOption(int floorID, int option);

	public abstract int SendSelectBattleHelper(int charID);
    public abstract int SendGMCmd(string cmd);
	public abstract int MsgSendMail_C2S(MailItem mail);
    public abstract int SendCardDivisionUpdate(CSItem card, int formulaId, List<CSItem> cardList);


    //--------------长连接消息---------------
    //登录到BattleServer
    public abstract int SendLogin_C2BS(string tokenID);
    public abstract int SendTest_C2BS();
    public abstract int SendCreateResult_C2BS(int resultId, int source, int target, int skillResultID = 0, int skillID = 0, float[] param = null);
    public abstract int Tick();
    public abstract int SendSyncPosition_C2BS(int id, float x, float z);
    public abstract int SendAction_Move_C2BS(int sobID, float startx, float startz, float x, float z);
    public abstract int SendAction_Attack_C2BS(int sobID, int skillID, int targetID, float x, float z);
    public abstract int SendAction_AttackingMove_C2BS(int sobID, int skillID, int targetID, float x, float z);
	public abstract int SendAction_BeHit_C2BS(int sobID, int srcActorID, bool isBack, bool isFly);
	public abstract int SendAction_Roll_C2BS(int sobID, Vector3 curPos, Vector3 targetPos);
	public abstract int SendAction_ActorEnter_C2BS(int sobID, Vector3 curPos, Vector3 forward);
    public abstract int SendAction_ActorExit_C2BS(int sobID, Vector3 curPos);
    public abstract int SendAction_JumpIn_C2BS(int sobID, int targetID);
    public abstract int SendAction_JumpOut_C2BS(int sobID, float x, float z);
    public abstract int SendActionInterupt_C2BS(int sobID, int actionType);
    public abstract int SendComboNum_C2BS(int type, int skillResultId=0);
    public abstract int SendTestAllSkill_C2BS(int sobID, List<int> skillIDList);
    public abstract int SendLevelUpSkill_C2BS(int sobID, int skillIndex, bool isSkill);

    public abstract int user_LogOut();
}	
