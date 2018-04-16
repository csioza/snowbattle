// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System;
// 
// public class UIMsgBoxMainText : UIWindow
// {
//     UILabel m_label = null;
//     UILabel m_hintLabel = null;
//     public UIMsgBoxMainText()
//     {
// 
//     }
//     static public UIMsgBoxMainText Load(string strPrefab)
//     {
//         return UIManager.Singleton.LoadUI<UIMsgBoxMainText>("UI/UIMsg/" + strPrefab, UIManager.Anchor.Center);
//     }
//     public override void OnInit()
//     {
//         base.OnInit();
//         m_label = FindChild("Label").GetComponent<UILabel>();
//         if (null != FindChild("Hint"))
//         {
//             m_hintLabel = FindChild("Hint").GetComponent<UILabel>();
//         }
// 
//     }
//     public override void AttachEvent()
//     {
//         base.AttachEvent();
//     }
//     public void SetText(string text)
//     {
//         m_label.text = text;
//     }
//     public void SetHintText(string text)
//     {
//         if (null != m_hintLabel)
//         {
//             m_hintLabel.text = text;
//         }
//     }
// }
// public class UICommonMsgBox : UIWindow
// {
//     public delegate void OnButtonCallbacked(object sender, EventArgs e);
// 
//     OnButtonCallbacked m_yesCallbacked;
//     OnButtonCallbacked m_noCallbacked;
//     GameObject m_yesButton;
//     GameObject m_noButton;
//     GameObject m_textLabel;
//     UIMsgBoxMainText UIMainText;
//     UIWindow m_insertUIWnd;
// 
//     GameObject m_oneButton = null;
//     GameObject m_twoButton = null;
// 
// 
//     static public UICommonMsgBox GetInstance()
//     {
//         UICommonMsgBox self = UIManager.Singleton.GetUIWithoutLoad<UICommonMsgBox>();
//         if (self != null)
//         {
//             return self;
//         }
//         self = UIManager.Singleton.LoadUI<UICommonMsgBox>("UI/UICommonMsgBox", UIManager.Anchor.Center);
//         return self;
//     }
//     public override void OnInit()
//     {
//         base.OnInit();
//         m_yesButton = FindChild("confirm");
//         m_noButton = FindChild("cancel");
//         m_textLabel = FindChild("textLabel");
// 
//         m_oneButton = FindChild("OneButton");
//         m_twoButton = FindChild("TwoButton");
//         //AddPropChangedNotify((int)MVCPropertyID.enMainPlayer, OnPropertyChanged);
//     }
//     public override void AttachEvent()
//     {
//         base.AttachEvent();
//         AddChildMouseClickEvent("confirm", OnButtonYes);
//         AddChildMouseClickEvent("cancel", OnButtonNo);
//         AddChildMouseClickEvent("OneConfirm", OnButtonYes);
//     }
//     public void OnButtonYes(object sender, EventArgs e)
//     {
//         if (null != m_yesCallbacked)
//         {
//             m_yesCallbacked(sender, e);
//         }
// 
//         //HideMsgBox();
//     }
//     public void OnButtonNo(object sender, EventArgs e)
//     {
//         if (null != m_noCallbacked)
//         {
//             m_noCallbacked(sender, e);
//         }
// 
//         HideMsgBox();
//     }
//     public UIMsgBoxMainText GetMainText()
//     {
//         return UIMainText;
//     }
//     public void ShowMsgBox(OnButtonCallbacked yesCallbacked, OnButtonCallbacked noCallbacked, UICommonMsgBoxCfg boxCfg)
//     {
//         m_yesCallbacked = yesCallbacked;
//         m_noCallbacked = noCallbacked;
//         if (!string.IsNullOrEmpty(boxCfg.yesIcon))
//         {
//             UISprite sprite = m_yesButton.GetComponent<UISprite>();
//             sprite.spriteName = boxCfg.yesIcon;
// 
//         }
//         if (!string.IsNullOrEmpty(boxCfg.noIcon))
//         {
//             UISprite sprite = m_noButton.GetComponent<UISprite>();
//             sprite.spriteName = boxCfg.noIcon;
//         }
// 
//         // 显示一个按钮
//         if (boxCfg.buttonNum == 1)
//         {
//             m_oneButton.SetActive(true);
//             m_twoButton.SetActive(false);
//         }
//         // 显示两个按钮
//         else
//         {
//             m_oneButton.SetActive(false);
//             m_twoButton.SetActive(true);
//         }
//         ShowWindow();
//         if (boxCfg.isUsePrefab)
//         {
//             UIMainText = UIMsgBoxMainText.Load(boxCfg.mainTextPrefab);
//             UIMainText.SetParent(m_textLabel);
//             UIMainText.WindowRoot.transform.localPosition = Vector3.zero;
//             UIMainText.ShowWindow();
//         }
//         else
//         {
//             UILabel label = m_textLabel.GetComponent<UILabel>();
//             if (null != label)
//             {
//                 label.text = boxCfg.mainTextPrefab;
//             }
//         }
// 
// 
//     }
//     public void ShowMsgBox(OnButtonCallbacked yesCallbacked, OnButtonCallbacked noCallbacked, UICommonMsgBoxCfg boxCfg, int strIndex)
//     {
//         m_yesCallbacked = yesCallbacked;
//         m_noCallbacked = noCallbacked;
//         if (!string.IsNullOrEmpty(boxCfg.yesIcon))
//         {
//             UISprite sprite = m_yesButton.GetComponent<UISprite>();
//             sprite.spriteName = boxCfg.yesIcon;
// 
//         }
//         if (!string.IsNullOrEmpty(boxCfg.noIcon))
//         {
//             UISprite sprite = m_noButton.GetComponent<UISprite>();
//             sprite.spriteName = boxCfg.noIcon;
//         }
// 
//         // 显示一个按钮
//         if (boxCfg.buttonNum == 1)
//         {
//             m_oneButton.SetActive(true);
//             m_twoButton.SetActive(false);
//         }
//         // 显示两个按钮
//         else
//         {
//             m_oneButton.SetActive(false);
//             m_twoButton.SetActive(true);
//         }
//         ShowWindow();
//         if (boxCfg.isUsePrefab)
//         {
//             UIMainText = UIMsgBoxMainText.Load(boxCfg.textPrefabList[strIndex]);
//             UIMainText.SetParent(m_textLabel);
//             UIMainText.WindowRoot.transform.localPosition = Vector3.zero;
//             UIMainText.ShowWindow();
//         }
//         else
//         {
//             UILabel label = m_textLabel.GetComponent<UILabel>();
//             if (null != label)
//             {
//                 label.text = boxCfg.textPrefabList[strIndex];
//             }
//         }
// 
// 
//     }
//     public void ShowMsgBox(UIWindow boxCfg)
//     {
//         //ShowWindow();
//         m_insertUIWnd = boxCfg;
//         boxCfg.SetParent(m_textLabel);
//         boxCfg.SetLocalScale(Vector3.one);
//         UIPanel panel = boxCfg.WindowRoot.GetComponent<UIPanel>();
//         panel.depth = this.WindowRoot.GetComponent<UIPanel>().depth + 1;
//         boxCfg.WindowRoot.SetActive(true);
//         boxCfg.WindowRoot.transform.position = new Vector3(1, 2, 3);
//         //friendItemcc.SetLocalScale(Vector3.one);
// 
//         //boxCfg.WindowRoot.transform.parent = m_textLabel;
//     }
//     //     public void ShowMsgBox(OnButtonCallbacked yesCallbacked, OnButtonCallbacked noCallbacked, string mainTextPrefab, string yesIcon, string noIcon,int buttonNum = 2)
//     //     {
//     //         m_yesCallbacked = yesCallbacked;
//     //         m_noCallbacked = noCallbacked;
//     //         if (!string.IsNullOrEmpty(yesIcon))
//     //         {
//     //             UISprite sprite = m_yesButton.GetComponent<UISprite>();
//     //             sprite.spriteName = yesIcon;
//     //   
//     //         }
//     //         if (!string.IsNullOrEmpty(noIcon))
//     //         {
//     //             UISprite sprite = m_noButton.GetComponent<UISprite>();
//     //             sprite.spriteName = noIcon;
//     //         }
//     // 
//     //         // 显示一个按钮
//     //         if ( buttonNum == 1 )
//     //         {
//     //             m_oneButton.SetActive(true);
//     //             m_twoButton.SetActive(false);
//     //         }
//     //         // 显示两个按钮
//     //         else
//     //         {
//     //             m_oneButton.SetActive(false);
//     //             m_twoButton.SetActive(true);
//     //         }
//     //         ShowWindow();
//     //         UIMainText = UIMsgBoxMainText.Load(mainTextPrefab);
//     //         UIMainText.SetParent(m_textLabel);
//     //         UIMainText.WindowRoot.transform.localPosition = Vector3.zero;
//     //         UIMainText.ShowWindow();
//     //     }
//     public void HideMsgBox()
//     {
//         HideWindow();
//         if (UIMainText != null)
//         {
//             UIMainText.HideWindow();
//             UIMainText.WindowRoot.transform.parent = null;
//             UIMainText.Destroy();
//         }
//         if (m_insertUIWnd != null)
//         {
//             m_insertUIWnd.HideWindow();
//             m_insertUIWnd.WindowRoot.transform.parent = null;
//             m_insertUIWnd.Destroy();
//         }
//     }
//     void OnPropertyChanged(int objectID, int eventType, IPropertyObject obj, object eventObj)
//     {
//         //         if (eventType == (int)Actor.ENPropertyChanged.enRelive)
//         //         {
//         //             ShowWindow();
//         //         }
//     }
// }
