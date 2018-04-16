using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UIFriendCfgItem
{
    public bool isUsePrefab = true;
    public string yesIcon;
    public string noIcon;
    public string acceptIcon;
    public string refuserIcon;
    public int buttonNum = 2;
    public List<string> textPrefabList = new List<string>();
    public List<string> itemPrefabList = new List<string>();
}

public class UIFriendMsgBoxCfg : MonoBehaviour
{
    public List<UIFriendCfgItem> ItemList = new List<UIFriendCfgItem>();

    // Use this for initialization
  
}
