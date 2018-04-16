using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICommonMsgBoxCfg : MonoBehaviour
{

	// Use this for initialization
    public bool     isUsePrefab = true;
    public string   mainTextPrefab;
    public string   yesIcon;
    public string   noIcon;
    public int      buttonNum = 2;
    public List<string> textPrefabList = new List<string>();
}
