using RssTech.Employee.Domain.UnitTests.Common;
using RssTech.Employee.Domain.ValueObjects;

namespace RssTech.Employee.Domain.UnitTests.ValueObjects;

public class EmailTests : TestBase
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("firstname+lastname@company.org")]
    [InlineData("admin@localhost")]
    public void Constructor_WhenValidEmail_ThenCreatesEmail(string emailAddress)
    {
        // Arrange & Act
        var email = new Email(emailAddress);

        // Assert
        Assert.Equal(emailAddress, email.Address);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WhenInvalidEmail_ThenCreatesEmailWithInvalidAddress(string? emailAddress)
    {
        // Arrange & Act
        var email = new Email(emailAddress!);

        // Assert
        Assert.Equal(emailAddress, email.Address);
    }

    [Fact]
    public void Email_WhenTwoEmailsWithSameAddress_ThenAreEqual()
    {
        // Arrange
        var address = "test@example.com";
        var email1 = new Email(address);
        var email2 = new Email(address);

        // Act & Assert
        Assert.Equal(email1, email2);
        Assert.True(email1 == email2);
        Assert.False(email1 != email2);
        Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
    }

    [Fact]
    public void Email_WhenTwoEmailsWithDifferentAddress_ThenAreNotEqual()
    {
        // Arrange
        var email1 = new Email("test1@example.com");
        var email2 = new Email("test2@example.com");

        // Act & Assert
        Assert.NotEqual(email1, email2);
        Assert.False(email1 == email2);
        Assert.True(email1 != email2);
    }

    [Fact]
    public void Email_WhenComparedToNull_ThenIsNotEqual()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act & Assert
        Assert.False(email.Equals(null));
        Assert.False(email == null);
        Assert.True(email != null);
    }

    [Fact]
    public void Email_WhenComparedToDifferentType_ThenIsNotEqual()
    {
        // Arrange
        var email = new Email("test@example.com");
        var notAnEmail = "test@example.com";

        // Act & Assert
        Assert.False(email.Equals(notAnEmail));
    }

    [Fact]
    public void Email_WhenCaseIsDifferent_ThenAreNotEqual()
    {
        // Arrange
        var email1 = new Email("Test@Example.Com");
        var email2 = new Email("test@example.com");

        // Act & Assert
        Assert.NotEqual(email1, email2);
    }
}