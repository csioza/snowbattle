
public enum ENNotifyEventType
{
	enNone = 0,
}

public class INotifyEvent
{
	public virtual ENNotifyEventType GetNotifyEventType() { return ENNotifyEventType.enNone; }
}
