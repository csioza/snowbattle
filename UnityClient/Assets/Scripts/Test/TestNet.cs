using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestNet : MonoBehaviour 
{
    private Rect windowRect0 = new Rect();

    string username;
    string userpwd;
    public void Start()
    {
    }

    public void OnGUI()
    {
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;

        var windowWidth = 300;
        var windowHeight = 180;
        var windowX = (screenWidth - windowWidth) / 2;
        var windowY = (screenHeight - windowHeight) / 2;
        //将窗口放置到屏幕中间  
        windowRect0 = new Rect(windowX, windowY, windowWidth, windowHeight);
        GUILayout.Window(0, windowRect0, TestBtn, "test");
    }


    public void TestBtn(int windowid)
    {
        GUILayout.BeginVertical();

        //first name  
//         GUILayout.BeginHorizontal();
//         GUILayout.Label("username", GUILayout.Width(80));
//         username = GUILayout.TextField(username);
//         GUILayout.EndHorizontal();
// 
//         //last name  
//         GUILayout.BeginHorizontal();
//         GUILayout.Label("userpwd", GUILayout.Width(80));
//         userpwd = GUILayout.TextField(userpwd);
//         GUILayout.EndHorizontal();


        if (GUILayout.Button("Conn"))
        {
            TestConn();
        }
        //if (ClientNet.Singleton.NeedCreateChar == true)
        {
            if (GUILayout.Button("createChar"))
            {
                TestCreateChar();
            }
        }
        if (GUILayout.Button("Login"))
        {
            TestLogin();
        }
        if (GUILayout.Button("test"))
        {
            TestTest();
        }
//         if (GUILayout.Button("GetCard"))
//         {
//             TestGetCard();
//         }
		if (GUILayout.Button("AskPlayerData"))
		{
			TestAskPlayerData();
		}
		if (GUILayout.Button("AddCard"))
		{
			TestCPPGetCardCard();
		}
		if (GUILayout.Button("AskItemBag"))
		{
			TestAskItemBag();
		}

		if (GUILayout.Button("ADD_EXP"))
		{
			TestCard_Addexp();
		}
		if (GUILayout.Button("合成"))
		{
			TestCard_merge();
		}
		if (GUILayout.Button("进化"))
		{
			TestCard_evolve();
		}
        GUILayout.EndVertical();
    }

    public void TestConn()
    {
        MessageProcess.Singleton.Init();
        ClientNet.Singleton.Start();
    }
    public void TestLogin()
    {
//        MainGame.Singleton.StartCoroutine(TestTestTest());
//         Loginpacket login = new Loginpacket("aaab", "aaab");
//         ClientNet.Singleton.SendPacket(login);
        MiniServer.Singleton.user_login("mylogin","mylogin");
    }

    public void TestTest()
    {
        if (ClientNet.Singleton.IsConnected)
        {
            Debug.Log("yes");
            TestTestPacket test = new TestTestPacket(5);
            ClientNet.Singleton.SendPacket(test);
        }
        else
        {
            Debug.Log("no");
        }

    }

    public void TestCreateChar()
    {
        OperateCharPacket login = new OperateCharPacket(1, 1, 1, "哈哈");
        ClientNet.Singleton.SendPacket(login);
    }

//     IEnumerator TestTestTest()
//     {
//         while (true)
//         {
//             yield return new WaitForSeconds(0.1f);
//             Loginpacket login = new Loginpacket("test", "test");
//             ClientNet.Singleton.SendPacket(login);
//         }
//     }

//     public void TestGetCard()
//     {
//         MiniServer.Singleton.user_get_card(5, 1);
//     }

	public void TestAskPlayerData()
	{
		MiniServer.Singleton.user_ask_playerData();
	}

	public void TestCPPGetCardCard()
	{
		MiniServer.Singleton.player_get_card(5,1);
	}

	public void TestAskItemBag()
	{
		MiniServer.Singleton.user_ask_bagData();
	}
	
	public void TestCard_Addexp()
	{
		CSItem item = User.Singleton.ItemBag.GetItem(0);
		if (item == null)
		{
			Debug.Log("item == null");
			return;
		}
		MiniServer.Singleton.herocard_addExp(item.m_guid,1000000);
	}
	public void TestCard_merge()
	{
		CSItem item = User.Singleton.ItemBag.GetItem(0);
		if (item == null)
		{
			Debug.Log("item == null");
			return;
		}
		List<CSItemGuid> ilist = new List<CSItemGuid>();
		ilist.Add(User.Singleton.ItemBag.GetItem(1).m_guid);
		ilist.Add(User.Singleton.ItemBag.GetItem(2).m_guid);

		MiniServer.Singleton.req_herocardMerge(item.m_guid, ilist);
	}
	public void TestCard_evolve()
	{
		CSItem item = User.Singleton.ItemBag.GetItem(0);
		if (item == null)
		{
			Debug.Log("item == null");
			return;
		}

		MiniServer.Singleton.req_herocardEvolve(item.m_guid);

	}
}
