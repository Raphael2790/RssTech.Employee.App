using RssTech.Employee.Domain.UnitTests.Common;
using RssTech.Employee.Domain.ValueObjects;

namespace RssTech.Employee.Domain.UnitTests.ValueObjects;

public class PhoneTests : TestBase
{
    [Theory]
    [InlineData("11999999999")]
    [InlineData("(11) 99999-9999")]
    [InlineData("+55 11 99999-9999")]
    [InlineData("11 9 9999-9999")]
    public void Constructor_WhenValidPhoneNumber_ThenCreatesPhone(string phoneNumber)
    {
        // Arrange & Act
        var phone = new Phone(phoneNumber);

        // Assert
        Assert.Equal(phoneNumber, phone.Number);
        Assert.NotEqual(Guid.Empty, phone.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WhenInvalidPhoneNumber_ThenCreatesPhoneWithInvalidNumber(string? phoneNumber)
    {
        // Arrange & Act
        var phone = new Phone(phoneNumber!);

        // Assert
        Assert.Equal(phoneNumber, phone.Number);
        Assert.NotEqual(Guid.Empty, phone.Id);
    }

    [Fact]
    public void Constructor_WhenCalled_ThenGeneratesUniqueId()
    {
        // Arrange & Act
        var phone1 = new Phone("11999999999");
        var phone2 = new Phone("11999999999");

        // Assert
        Assert.NotEqual(phone1.Id, phone2.Id);
        Assert.NotEqual(Guid.Empty, phone1.Id);
        Assert.NotEqual(Guid.Empty, phone2.Id);
    }

    [Fact]
    public void Phone_WhenTwoPhonesWithSameNumberAndId_ThenAreEqual()
    {
        // Arrange
        var phoneNumber = "11999999999";
        var phone1 = new Phone(phoneNumber);

        // Create a phone with same number but different id (this is a theoretical case for record equality)
        var phone2 = phone1 with { }; // Creates a copy with same values

        // Act & Assert
        Assert.Equal(phone1, phone2);
        Assert.True(phone1 == phone2);
        Assert.False(phone1 != phone2);
    }

    [Fact]
    public void Phone_WhenTwoPhonesWithDifferentNumber_ThenAreNotEqual()
    {
        // Arrange
        var phone1 = new Phone("11999999999");
        var phone2 = new Phone("11888888888");

        // Act & Assert
        Assert.NotEqual(phone1, phone2);
        Assert.False(phone1 == phone2);
        Assert.True(phone1 != phone2);
    }

    [Fact]
    public void Phone_WhenTwoPhonesWithSameNumberButDifferentId_ThenAreNotEqual()
    {
        // Arrange
        var phoneNumber = "11999999999";
        var phone1 = new Phone(phoneNumber);
        var phone2 = new Phone(phoneNumber);

        // Act & Assert
        Assert.NotEqual(phone1, phone2); // Different because of different Ids
        Assert.False(phone1 == phone2);
        Assert.True(phone1 != phone2);
    }

    [Fact]
    public void ToString_WhenCalled_ThenReturnsPhoneNumber()
    {
        // Arrange
        var phoneNumber = "11999999999";
        var phone = new Phone(phoneNumber);

        // Act
        var result = phone.ToString();

        // Assert
        Assert.Equal(phoneNumber, result);
    }

    [Fact]
    public void Phone_WhenComparedToNull_ThenIsNotEqual()
    {
        // Arrange
        var phone = new Phone("11999999999");

        // Act & Assert
        Assert.False(phone.Equals(null));
        Assert.False(phone == null);
        Assert.True(phone != null);
    }

    [Fact]
    public void Phone_WhenComparedToDifferentType_ThenIsNotEqual()
    {
        // Arrange
        var phone = new Phone("11999999999");
        var notAPhone = "11999999999";

        // Act & Assert
        Assert.False(phone.Equals(notAPhone));
    }

    [Theory]
    [InlineData("11999999999")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Phone_WhenCopiedWithWith_ThenCreatesNewInstanceWithSameValues(string? phoneNumber)
    {
        // Arrange
        var originalPhone = new Phone(phoneNumber!);

        // Act
        var copiedPhone = originalPhone with { };

        // Assert
        Assert.Equal(originalPhone, copiedPhone);
        Assert.Equal(originalPhone.Number, copiedPhone.Number);
        Assert.Equal(originalPhone.Id, copiedPhone.Id);
    }

    [Fact]
    public void Phone_WhenCopiedWithNewNumber_ThenCreatesNewInstanceWithNewNumber()
    {
        // Arrange
        var originalPhone = new Phone("11999999999");
        var newNumber = "11888888888";

        // Act
        var modifiedPhone = originalPhone with { Number = newNumber };

        // Assert
        Assert.NotEqual(originalPhone, modifiedPhone);
        Assert.Equal(newNumber, modifiedPhone.Number);
        Assert.Equal(originalPhone.Id, modifiedPhone.Id); // Id remains the same
    }
}