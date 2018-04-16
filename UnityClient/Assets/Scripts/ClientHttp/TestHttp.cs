using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestHttp : MonoBehaviour
{
    private string username = "2";
    private string userpwd = "2";
    //private bool submitted = false;
	//private bool charSubmitted = false;

	//private string selectCharID = "1";

	//private string selectStageID = "1";
	//private string selectLevelID = "1";

	//bool hasCharList = false;

	int nowWindow = 1;

    private Rect windowRect0 = new Rect();

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
		if (nowWindow == 1)
		{

			GUILayout.Window(0, windowRect0, UserForm, "User information");
		}


    }

    public void UserForm(int windowid)
    {
        GUILayout.BeginVertical();

        //first name  
        GUILayout.BeginHorizontal();
        GUILayout.Label("username", GUILayout.Width(80));
        username = GUILayout.TextField(username);
        GUILayout.EndHorizontal();

        //last name  
        GUILayout.BeginHorizontal();
        GUILayout.Label("userpwd", GUILayout.Width(80));
        userpwd = GUILayout.TextField(userpwd);
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Submit"))
        {
            TestLogin(username, userpwd);
        }
        if (GUILayout.Button("Reset"))
        {
			TestTest();
        }
        GUILayout.EndVertical();
    }

	//本地账号登陆.先验证.
    public void TestLogin(string username, string userpwd)
    {
		IMiniServer.Singleton.account_serverconfig();
    }

	public void OnMsgAccCheck(int respond, ServerMsgStructAccountInfo accinfo)
	{
		//账号验证成功之后,再登陆.如有选区.往这里加.
		if (respond == 1)
		{
			//用token登陆
			Debug.Log(accinfo.accname + " " + accinfo.token);
			IMiniServer.Singleton.account_login(accinfo.accname, accinfo.token);
		}
	}


	public void TestTest()
	{
		MiniServer.Singleton.user_create("ttt", 1, 5);
	}
}
