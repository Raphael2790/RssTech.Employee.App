namespace RssTech.Employee.Common.Notification;

public abstract class NotifiableObject
{
    private readonly List<string> _notifications = [];
    public IReadOnlyCollection<string> Notifications => _notifications.AsReadOnly();
    public bool IsValid => !(_notifications.Count > 0);

    protected void AddNotification(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            _notifications.Add(message);
        }
    }

    protected void AddNotifications(IEnumerable<string> messages)
    {
        foreach (var message in messages)
        {
            AddNotification(message);
        }
    }

    protected void ClearNotifications()
    {
        _notifications.Clear();
    }
}
