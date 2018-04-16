using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIFriendMessageBox : UIWindow
{

    public delegate void OnButtonCallbacked(object sender, EventArgs e);
    OnButtonCallbacked m_yesCallbacked;
    OnButtonCallbacked m_noCallbacked;
    //OnButtonCallbacked m_acceptCallbacked;
    OnButtonCallbacked m_refuserCallbacked;
    GameObject m_yesButton;
    GameObject m_noButton;
    GameObject m_textLabel;
    List<UIMsgBoxMainText> UIMainTextList = new List<UIMsgBoxMainText>();
    UIFriendItem m_insertUIWnd;

    //GameObject m_messageInfo1 = null; 
    //GameObject m_messageInfo2 = null;
    //GameObject m_messageInfo3 = null;
    List<GameObject> m_messageInfoList = new List<GameObject>();
    GameObject m_InsertItemPrefa = null;

    GameObject m_oneButton = null;
    GameObject m_twoButton = null;
    GameObject m_threeButton = null;
    //GameObject m_okBtn = null;
    //GameObject m_cancelBtn = null;
    //GameObject m_acceptBtn = null;
    GameObject m_refuserBtn = null;


    public UILabel m_messageInfo = null;
    static public UIFriendMessageBox GetInstance()
    {
        UIFriendMessageBox self = UIManager.Singleton.GetUIWithoutLoad<UIFriendMessageBox>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UIFriendMessageBox>("UI/UIFriendMessageBox", UIManager.Anchor.Center);
        return self;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    public override void OnInit()
    {
        base.OnInit();
        for (int i = 1; i < 3; i++ )
        {
            GameObject messageInfo = FindChild("MessageInfo"+i);
            m_messageInfoList.Add(messageInfo);
        }
        m_InsertItemPrefa = FindChild("MessageInfo");

        m_oneButton = FindChild("ButtonTypeOne");
        m_twoButton = FindChild("ButtonTypeTwo");
        m_threeButton = FindChild("ButtonTypeThree");
        
//        m_okBtn = null;
//        m_cancelBtn = null;
//        m_acceptBtn = null;
        m_refuserBtn = null;
        //AddPropChangedNotify((int)MVCPropertyID.enMainPlayer, OnPropertyChanged);
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("confirm", OnButtonYes);
        AddChildMouseClickEvent("cancel", OnButtonNo);
        AddChildMouseClickEvent("OneConfirm", OnButtonYes);
    }
    public void OnButtonAccept(object sender, EventArgs e)
    {
        //if (null != m_acceptCallbacked)
        //{
        //    m_acceptCallbacked(sender, e);
        //}
        HideMsgBox();
    }

    public void OnButtonRefuserBtn(object sender, EventArgs e)
    {
        if (null != m_refuserCallbacked)
        {
            m_refuserCallbacked(sender, e);
        }
        HideMsgBox();
    }

    public void OnButtonYes(object sender, EventArgs e)
    {
        if (null != m_yesCallbacked)
        {
            m_yesCallbacked(sender, e);
        }

        HideMsgBox();
    }
    public void OnButtonNo(object sender, EventArgs e)
    {
        if (null != m_noCallbacked)
        {
            m_noCallbacked(sender, e);
        }

        HideMsgBox();
    }
    public List<UIMsgBoxMainText> GetMainText()
    {
        return UIMainTextList;
    }
    public void InsertOneButton()
    {
        m_oneButton.SetActive(true);
        m_twoButton.SetActive(false);
        m_threeButton.SetActive(false);
        m_yesButton = m_oneButton.transform.Find("Button_OK").gameObject;
        AddMouseClickEvent(m_yesButton, OnButtonYes);
    }

    public void InsertTwoButton()
    {
        m_oneButton.SetActive(false);
        m_twoButton.SetActive(true);
        m_threeButton.SetActive(false);
        m_yesButton = m_twoButton.transform.Find("Button_OK").gameObject;
        m_noButton = m_twoButton.transform.Find("Button_Cancel").gameObject;
        AddMouseClickEvent(m_yesButton, OnButtonYes);
        AddMouseClickEvent(m_noButton, OnButtonNo);
    }

    public void InsertThreeButton()
    {
        m_oneButton.SetActive(false);
        m_twoButton.SetActive(false);
        m_threeButton.SetActive(true);
        m_yesButton = m_threeButton.transform.Find("AcceptBtn").gameObject;
        m_noButton = m_threeButton.transform.Find("CancelBtn").gameObject;
        m_refuserBtn = m_threeButton.transform.Find("RefuserBtn").gameObject;
        AddMouseClickEvent(m_yesButton, OnButtonYes);
        AddMouseClickEvent(m_noButton, OnButtonNo);
        AddMouseClickEvent(m_refuserBtn, OnButtonRefuserBtn);
    }

    public void ShowMsgBox(OnButtonCallbacked yesCallbacked, OnButtonCallbacked noCallbacked, OnButtonCallbacked refuserCallbacked, UIFriendCfgItem boxCfg)
    {
        m_yesCallbacked = yesCallbacked;
        m_noCallbacked = noCallbacked;
        m_refuserCallbacked = refuserCallbacked;
        if (!string.IsNullOrEmpty(boxCfg.yesIcon))
        {
            UISprite sprite = m_yesButton.GetComponent<UISprite>();
            sprite.spriteName = boxCfg.yesIcon;
        }
        if (!string.IsNullOrEmpty(boxCfg.noIcon))
        {
            UISprite sprite = m_noButton.GetComponent<UISprite>();
            sprite.spriteName = boxCfg.noIcon;
        }

        // 显示按钮
        switch (boxCfg.buttonNum)
        {
            case 1:
                InsertOneButton();
                break;
            case 2:
                InsertTwoButton();
                break;
            case 3:
                InsertThreeButton();
                break;
            default:
                InsertOneButton();
                break;
        }
        if (boxCfg.isUsePrefab)
        {
            int tmpIndex = 0;
            foreach (string prefabStr in boxCfg.textPrefabList)
            {
                UIMsgBoxMainText UITmpText = UIMsgBoxMainText.Load(prefabStr);
                UITmpText.SetParent(m_messageInfoList[tmpIndex++]);
                UITmpText.WindowRoot.transform.localPosition = Vector3.zero; 
                UITmpText.ShowWindow();
            }
        }
        else
        {
            int tmpIndex = 0;
            foreach (string prefabStr in boxCfg.textPrefabList)
            {
                UILabel label = m_messageInfoList[tmpIndex++].GetComponent<UILabel>();
                if (null != label)
                {
                    label.text = prefabStr;
                }
            }


//             foreach (GameObject tmpMsg in m_messageInfoList)
//             {
//                 UILabel label = tmpMsg.GetComponent<UILabel>();
//                 if (null != label)
//                 {
//                     label.text = boxCfg.textPrefabList[tmpIndex++];
//                 }
//             }
        }
        if (null != m_insertUIWnd)
        {
            m_insertUIWnd.SetParent(m_InsertItemPrefa);
            m_insertUIWnd.WindowRoot.transform.localPosition = new Vector3(0, 0, 0);
            m_insertUIWnd.WindowRoot.SetActive(true);
            m_insertUIWnd.SetLocalScale(Vector3.one);
            UIPanel panel = m_insertUIWnd.WindowRoot.GetComponent<UIPanel>();
            panel.depth = this.WindowRoot.GetComponent<UIPanel>().depth + 1;
            m_insertUIWnd.ShowWindow();
        }
        ShowWindow();
    }
    
    public void InsertUIWnd(UIFriendItem boxCfg)
    {
        m_insertUIWnd = boxCfg;

        //boxCfg.WindowRoot.transform.parent = m_textLabel;
    }
    public void HideMsgBox()
    {
        HideWindow();
        if (UIMainTextList != null)
        {
            foreach (UIMsgBoxMainText tmpText in UIMainTextList)
            {
                tmpText.HideWindow();
                tmpText.WindowRoot.transform.parent = null;
                tmpText.Destroy();
            }
        }
        if (null != m_insertUIWnd)
        {
            m_insertUIWnd.HideWindow();
            m_insertUIWnd.WindowRoot.transform.parent = null;
            m_insertUIWnd.Destroy();
            m_insertUIWnd = null;
        }
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
        //         if (eventType == (int)Actor.ENPropertyChanged.enRelive)
        //         {
        //             ShowWindow();
        //         }
    }

	public void SendAddFriend(object sender, EventArgs e)
	{
		if (null == m_insertUIWnd)
			return;
		FriendItem item = m_insertUIWnd.GetFriendItem();
		int friendID = item.m_id;
		MiniServer.Singleton.SendAddFriend_C2CH(friendID);

	}
}
