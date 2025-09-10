using RssTech.Employee.Common.Notification;

namespace RssTech.Employee.Domain.Entities;

public abstract class Entity : NotifiableObject
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected Entity() 
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    protected abstract void Validate();
}
