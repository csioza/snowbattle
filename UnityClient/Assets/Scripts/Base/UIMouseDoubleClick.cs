using UnityEngine;
using System;

public class UIMouseDoubleClick : UIMouseEvent
{
	void OnDoubleClick()
	{
		if (null != MouseEvent)
		{
            try
            {
                MouseEvent(gameObject, null);
            }
            catch (Exception e)
            {
                DebugLog.Singleton.OnShowLog("[UIMouseDoubleClick OnDoubleClick ] " + e.Message + " " + e.StackTrace.ToString());
            }
		}
	}
}