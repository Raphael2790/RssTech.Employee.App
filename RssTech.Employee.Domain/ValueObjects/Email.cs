using System.Text.RegularExpressions;

namespace RssTech.Employee.Domain.ValueObjects;

public partial record class Email
{
    public string Address { get; init; }
    public Email(string address)
    {
        Address = address;
    }
}
