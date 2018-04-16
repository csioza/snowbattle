using System.Collections.Generic;
using System.Linq;

public class INotifier
{
	static bool NotifyMode;
	public List<IObserver> m_observerList;
	public void NotifyChange(INotifyEvent e)
	{
		for (int index = 0; index < m_observerList.Count(); index++)
		{
			m_observerList[index].OnNotifyChange(this, e);
		}
	}
	public void AttachObserver(IObserver observer)
	{
		for (int index = 0; index < m_observerList.Count; index++)
		{
			if (m_observerList[index] == observer)
			{
				return;
			}
		}
		m_observerList.Add(observer);
		observer.AddNotifier(this);
	}
	public void DetachObserver(IObserver observer)
	{
		for (int index = 0; index < m_observerList.Count; index++)
		{
			if (m_observerList[index] == observer)
			{
				return;
			}
		}
		m_observerList.Remove(observer);
		observer.RemoveNotifier(this);
	}

	public int GetObserverCount()
	{
		return m_observerList.Count();
	}
};
