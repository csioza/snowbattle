using UnityEngine;
using System;

public class UIMouseClick : UIMouseEvent
{
	public void OnClick()
	{
//         if (MainGame.Singleton.IsAppLogicPause())
//         {
//             return;
//         }
		if (null != MouseEvent)
		{
            try
            {
                MouseEvent(gameObject, null);
            }
            catch (Exception e)
            {
                DebugLog.Singleton.OnShowLog("[UIMouseClick OnClick ] " + e.Message + " " + e.StackTrace.ToString());
            }
		}
	}
}