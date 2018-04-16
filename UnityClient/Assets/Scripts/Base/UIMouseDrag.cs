using UnityEngine;
using System;

public class UIMouseDrag : UIMouseEvent
{
	bool isPressd = true;
	void OnDrag(Vector2 delta)
	{
		if (null != MouseEvent && isPressd)
		{
			isPressd = false;
			
            try
            {
                MouseEvent(gameObject, null);
            }
            catch (Exception e)
            {
                DebugLog.Singleton.OnShowLog("[UIMouseDrag OnDrag ] " + e.Message + " " + e.StackTrace.ToString());
            }
		}
	}
	void OnPress(bool isDown)
	{
		if(isDown == false) isPressd = true;
	}
}