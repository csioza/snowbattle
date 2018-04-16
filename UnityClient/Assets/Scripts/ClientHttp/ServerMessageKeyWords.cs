using UnityEngine;
using System.Collections;

//服务端发来的消息,jason中关键字
public struct ServerMessageKeyWords
{
	public const string KEYWORD_GLOBAL_CONFIGS = "global_configs";
	public const string KEYWORD_SERVER_CONFIGS = "server_configs";
	
	public const string KEYWORD_INIT_HEROCARDS = "init_herocards";
	public const string KEYWORD_ZONES = "zones";
	public const string KEYWORD_SESSION = "session";
	public const string KEYWORD_ACCOUNTINFO = "account_info";
	public const string KEYWORD_USERINFO = "user_info";
	public const string KEYWORD_HEROCARD_BAG = "herocard_bag";
	public const string KEYWORD_HEROCARD_GAIN = "herocard_gain";
    public const string KEYWORD_HEROCARD_LOSE = "herocard_lose";
    public const string KEYWORD_HEROCARD_UPDATE = "herocard_update";
}