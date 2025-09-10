namespace RssTech.Employee.Domain.ValueObjects;

public record class Phone
{
    public Guid Id { get; init; }
    public string Number { get; init; }

    public Phone(string number)
    {
        Number = number;
        Id = Guid.NewGuid();
    }

    public override string ToString() => Number;
}
