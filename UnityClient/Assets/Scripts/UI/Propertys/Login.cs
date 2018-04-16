using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;  

public  class Login : IPropertyObject  
{
    public string m_playrerName    = "";
    public int m_curCardId      = 0;

    public int m_checkErrorID = 0;// 检查账户时的错误返回 世界参数表ID

    public enum ENPropertyChanged
    {
        enInitChooseCard    = 1,
        enShowChooseCard,
        enCreateActor  ,
        enChooseCardHide,
        enCreateActorSuccess,
        enLoginHide,
        enChangePWD,
        enRegistSetData,
        enServerListUpdate,
        enShowLogin,
        enShowEnterName,
        enCheckUserNameOk,
    }

    public Login()
    {
        SetPropertyObjectID((int)MVCPropertyID.enLogin);
 
        InitChooseCard();

        PreResourceLoad.Singleton.Read();
    }

   
    public void OnInit()
    {
       
    }

    // 显示选择角色界面
    public void InitChooseCard()
    {
        NotifyChanged((int)ENPropertyChanged.enInitChooseCard,null);
    }

    // 显示选择角色界面
    public void ShowChooseCard()
    {
        NotifyChanged((int)ENPropertyChanged.enShowChooseCard, null);
    }

    // 显示选择角色界面
    public void ShowEnterName()
    {
        NotifyChanged((int)ENPropertyChanged.enShowEnterName, null);
    }


    // 登陆 返回的结果
    public  void OnLogin()
    {
        NotifyChanged((int)ENPropertyChanged.enShowLogin, null);
  
    }

    public void OnChangePwd(int respond, ServerMsgStructAccountInfo msg)
    {
        NotifyChanged((int)ENPropertyChanged.enChangePWD, respond);
  
    }

    // 获取服务器列表
    public void OnGetZone(ServerMsgStructZone msg)
    {

        NotifyChanged((int)ENPropertyChanged.enServerListUpdate, msg);
    }

    #region Singleton
    static Login m_singleton;
    static public Login Singleton
    {
        get
        {
            if (m_singleton == null)
            {
                m_singleton = new Login();
            }
            return m_singleton;
        }
    }
    #endregion


    // 像服务器 请求初始卡牌
    public void OnAskInitCard(int newPlayer)
    {
        Debug.Log("OnAskInitCard:" );
        // 如果不是新手
        if (newPlayer != 0)
        {
            // 请求数据
            MiniServer.Singleton.user_ask_playerData();
            return;
        }
       
        // 如果没有选卡
        if ( m_curCardId == 0 )
        {
            Loading.Singleton.Hide();

            // 如果当前状态是登陆状态
            if (MainGame.Singleton.CurrentState is StateLogin)
            {
                // 如果没有显示 则显示
                if (!UIGameStart.GetInstance().IsVisiable() && !UIChooseCard.GetInstance().IsVisiable())
                {
                    ShowChooseCard();
                }
            }
        }
    }


    // 检查 返回错误标识 标识是从 世界参数表中读取的字符串 然后显示在公共提示框上 
    public void CheckUserName(string userName,string password,int type=0)
    {
        if ( userName.Length < 6 )
        {
            m_checkErrorID = (int)ENWorldParamIndex.enUserNameTooShort;
            ShowWarnTips();
            return;
        }

        if (userName.Length > 12)
        {
            m_checkErrorID = (int)ENWorldParamIndex.enUserNameIllegal;
            ShowWarnTips();
            return;
        }

        if (type == 1)
        {
            if (password.Length < 6)
            {
                m_checkErrorID = (int)ENWorldParamIndex.enPasswordTooShort;
                ShowWarnTips();
                return;
            }

            if (password.Length > 12)
            {
                m_checkErrorID = (int)ENWorldParamIndex.enPasswordIllegal;
                ShowWarnTips();
                return;
            }
        }

        MainGame.Singleton.StartCoroutine(CoroutineCallBack(userName, password,type));
    }

    IEnumerator CoroutineCallBack(string userName,string password,int type )
    {

        WWW url = new WWW(string.Format("http://115.28.220.134:8083/server/game.php?cmd=check&op=username&username={0}", userName));
        yield return url;

       
        // 
        if (url.text.Contains("CheckFail"))
        {
            m_checkErrorID = (int)ENWorldParamIndex.enUserNameAlreadyExist;
            ShowWarnTips();
            NotifyChanged((int)ENPropertyChanged.enCheckUserNameOk, false);
        }
        // 
        else if (url.text.Contains("CheckOK"))
        {
            
            //用户名可用 并且要登陆
            if (type!=0)
            {
                MiniServer.Singleton.user_login(userName, password);

                PlayerPrefs.SetString("XUserName", userName);
                PlayerPrefs.SetString("XPassword", password);

                PlayerPrefs.Save();

                NotifyChanged((int)ENPropertyChanged.enLoginHide, null);
            }
            else
            {
                // 显示 检查完成 可以使用
                //用户名可用
                NotifyChanged((int)ENPropertyChanged.enCheckUserNameOk, true);
            }
        }

    }

    void ShowWarnTips()
    {
        if (m_checkErrorID <=0)
        {
            return;
        }
        WorldParamInfo info = GameTable.WorldParamTableAsset.Lookup(m_checkErrorID);
        if (null == info)
        {
            return;
        }

        UICommonMsgBoxCfg boxCfg    = new UICommonMsgBoxCfg();
        boxCfg.mainTextPrefab       = info.StringTypeValue;
        boxCfg.buttonNum            = 1;
        UICommonMsgBox.GetInstance().ShowMsgBox(null, null, boxCfg);
    }


    // 检查 返回错误标识 标识是从 世界参数表中读取的字符串 然后显示在公共提示框上 
    public void CheckLogin(string userName, string password)
    {
        if (userName.Length < 6)
        {
            m_checkErrorID = (int)ENWorldParamIndex.enUserNameTooShort;
            ShowWarnTips();
            return;
        }

        if (userName.Length > 12)
        {
            m_checkErrorID = (int)ENWorldParamIndex.enUserNameIllegal;
            ShowWarnTips();
            return;
        }

        if (password.Length < 6)
        {
            m_checkErrorID = (int)ENWorldParamIndex.enPasswordTooShort;
            ShowWarnTips();
            return;
        }

        if (password.Length > 12)
        {
            m_checkErrorID = (int)ENWorldParamIndex.enPasswordIllegal;
            ShowWarnTips();
            return;
        }

       

        MainGame.Singleton.StartCoroutine(CoroutineCheckLoginCallBack(userName, password));
    }

    IEnumerator CoroutineCheckLoginCallBack(string userName, string password)
    {

        WWW url = new WWW(string.Format("http://115.28.220.134:8083/server/game.php?cmd=check&op=login&username={0}&password={1}", userName, password));
        yield return url;

        Debug.Log("CoroutineCheckLoginCallBack:" + userName + ",password:" + password + ",url:" + url.text);

        //  登陆检验成功
        if (url.text.Contains("OK"))
        {
            // 登陆
            MiniServer.Singleton.user_login(userName, password);


         
            PlayerPrefs.SetString("XUserName", userName);
            PlayerPrefs.SetString("XPassword", password);
          
           

            PlayerPrefs.Save();

            NotifyChanged((int)ENPropertyChanged.enLoginHide, null);

        }

        else if (url.text.Contains("PasswordERROR"))
        {
            m_checkErrorID = (int)ENWorldParamIndex.enPasswordWrong;
            ShowWarnTips();

        }
     
        else if (url.text.Contains("NoUserNameERROR"))
        {
            m_checkErrorID = (int)ENWorldParamIndex.enUserNameNotExist;
            ShowWarnTips();

        }

    }
}