using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIMsgBoxMainText : UIWindow
{
    UILabel m_label = null;
    UILabel m_hintLabel = null;
    public UIMsgBoxMainText()
    {

    }
    static public UIMsgBoxMainText Load(string strPrefab)
    {
       return UIManager.Singleton.LoadUI<UIMsgBoxMainText>("UI/UIMsg/" + strPrefab, UIManager.Anchor.Center);
    }
    public override void OnInit()
    {
        base.OnInit();
        m_label = FindChild("Label").GetComponent<UILabel>();
        GameObject hintObj = FindChild("Hint");
        if (hintObj != null)
        {
            m_hintLabel = hintObj.GetComponent<UILabel>();
        }
        WindowRoot.transform.localPosition = Vector3.zero;        
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
    }
    public void SetText(string text)
    {
        m_labelText = text;
        if (null != m_label)
        {
            m_label.text = m_labelText;
        }
    }
    public void SetHintText(string text)
    {
        m_hintLabelText = text;
    }
    public void SetHintTextColor(Color color) 
    {
        if (null != m_hintLabel)
        {
            m_hintLabel.color = color;
        }
    }
    string m_labelText = "";
    string m_hintLabelText = "";
    public override void OnShowWindow()
    {
        base.OnShowWindow();
        if (m_hintLabel != null && !string.IsNullOrEmpty(m_hintLabelText))
        {
            m_hintLabel.text = m_hintLabelText;
        }
        if (null != m_label && !string.IsNullOrEmpty(m_labelText))
        {
            m_label.text = m_labelText;
        }
    }
}
public class UICommonMsgBox : UIWindow
{
    public delegate void OnButtonCallbacked(object sender, EventArgs e);

    OnButtonCallbacked m_yesCallbacked;
    OnButtonCallbacked m_noCallbacked;

    GameObject m_yesButton;
    GameObject m_noButton;
    GameObject m_textLabel;
    UIMsgBoxMainText UIMainText;

    GameObject m_oneButton = null;
    GameObject m_twoButton = null;

    static public UICommonMsgBox GetInstance()
    {
        UICommonMsgBox self = UIManager.Singleton.GetUIWithoutLoad<UICommonMsgBox>();
        if (self != null)
        {
            return self;
        }
        self = UIManager.Singleton.LoadUI<UICommonMsgBox>("UI/UICommonMsgBox", UIManager.Anchor.Center);
        return self;
    }
    public override void OnInit()
    {
        base.OnInit();
        UIMainText      = null;
        m_yesButton     = FindChild("confirm");
        m_noButton      = FindChild("cancel");
        m_textLabel     = FindChild("textLabel");

        m_oneButton     = FindChild("OneButton");
        m_twoButton     = FindChild("TwoButton");
        //AddPropChangedNotify((int)MVCPropertyID.enMainPlayer, OnPropertyChanged);
    }
    public override void AttachEvent()
    {
        base.AttachEvent();
        AddChildMouseClickEvent("confirm", OnButtonYes);
        AddChildMouseClickEvent("cancel", OnButtonNo);
        AddChildMouseClickEvent("OneConfirm", OnButtonYes);
    }
    public void OnButtonYes(object sender, EventArgs e)
    {
        HideMsgBox();
        if (null != m_yesCallbacked)
        {
            m_yesCallbacked(sender, e);
        }
    }
    public void OnButtonNo(object sender, EventArgs e)
    {
        HideMsgBox();
        if (null != m_noCallbacked)
        {
            m_noCallbacked(sender, e);
        }
    }
    public UIMsgBoxMainText GetMainText()
    {
        return UIMainText;
    }
    void Reset()
    {
        UILabel label = m_textLabel.GetComponent<UILabel>();
        if (null != label)
        {
            label.text = "";
        }
        if (null != UIMainText)
        {
            UIMainText.HideWindow();
            UIMainText.Destroy();
            UIMainText = null;
        }
    }
    public void ShowMsgBox(OnButtonCallbacked yesCallbacked, OnButtonCallbacked noCallbacked, UICommonMsgBoxCfg boxCfg)
    {
        Reset();
        m_yesCallbacked = yesCallbacked;
        m_noCallbacked = noCallbacked;
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

        // 显示一个按钮
        if (boxCfg.buttonNum == 1)
        {
            m_oneButton.SetActive(true);
            m_twoButton.SetActive(false);
        }
        // 显示两个按钮
        else
        {
            m_oneButton.SetActive(false);
            m_twoButton.SetActive(true);
        }
       
        if (boxCfg.isUsePrefab)
        {
            UIMainText = UIMsgBoxMainText.Load(boxCfg.mainTextPrefab);
            UIMainText.SetParent(m_textLabel);
            UIMainText.ShowWindow();
        }
        else
        {
            UILabel label = m_textLabel.GetComponent<UILabel>();
            if (null != label)
            {
                label.text = boxCfg.mainTextPrefab;
            }
        }
        ShowWindow();
        
    }
    public void ShowMsgBox(OnButtonCallbacked yesCallbacked, OnButtonCallbacked noCallbacked, UICommonMsgBoxCfg boxCfg, int strIndex)
    {
        Reset();
        m_yesCallbacked = yesCallbacked;
        m_noCallbacked = noCallbacked;
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

        // 显示一个按钮
        if (boxCfg.buttonNum == 1)
        {
            m_oneButton.SetActive(true);
            m_twoButton.SetActive(false);
        }
        // 显示两个按钮
        else
        {
            m_oneButton.SetActive(false);
            m_twoButton.SetActive(true);
        }
        if (boxCfg.isUsePrefab)
        {
            UIMainText = UIMsgBoxMainText.Load(boxCfg.textPrefabList[strIndex]);
            UIMainText.SetParent(m_textLabel);
            UIMainText.ShowWindow();
        }
        else
        {
            UILabel label = m_textLabel.GetComponent<UILabel>();
            if (null != label)
            {
                label.text = boxCfg.textPrefabList[strIndex];
            }
        }
        ShowWindow();
    }
    public void HideMsgBox()
    {
        HideWindow();
        Reset();
    }
    void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
    {
//      
    }
}
