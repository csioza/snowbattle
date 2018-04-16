using UnityEngine;
using System.Collections;

public class HTTPSetting  
{
	public static string REQ_Website = "http://192.168.199.182/xProjectServer/?q=";
 
    public static void ChangeWebsite(string website)
    {
        REQ_Website     = website;
    }
}
