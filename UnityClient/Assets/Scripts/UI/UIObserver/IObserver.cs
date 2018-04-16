using System.Collections.Generic;

public class IObserver
{
	public virtual void OnNotifyChange(INotifier notify, INotifyEvent e) { }

	public void Detach() { }
	public void AddNotifier(INotifier val)
	{
		m_notifierList.Add(val);
	}
	public void RemoveNotifier(INotifier val) { }

	public List<INotifier> m_notifierList;
};
