using RssTech.Employee.Domain.UnitTests.Common;
using RssTech.Employee.Domain.ValueObjects;

namespace RssTech.Employee.Domain.UnitTests.ValueObjects;

public class EmployeeDocumentTests : TestBase
{
    [Theory]
    [InlineData("12345678901")]
    [InlineData("123.456.789-01")]
    [InlineData("12345678000195")]
    [InlineData("98765432109")]
    public void Constructor_WhenValidDocumentNumber_ThenCreatesDocument(string documentNumber)
    {
        // Arrange & Act
        var document = new EmployeeDocument(documentNumber);

        // Assert
        Assert.Equal(documentNumber, document.DocumentNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WhenInvalidDocumentNumber_ThenCreatesDocumentWithInvalidNumber(string? documentNumber)
    {
        // Arrange & Act
        var document = new EmployeeDocument(documentNumber!);

        // Assert
        Assert.Equal(documentNumber, document.DocumentNumber);
    }

    [Fact]
    public void EmployeeDocument_WhenTwoDocumentsWithSameNumber_ThenAreEqual()
    {
        // Arrange
        var documentNumber = "12345678901";
        var document1 = new EmployeeDocument(documentNumber);
        var document2 = new EmployeeDocument(documentNumber);

        // Act & Assert
        Assert.Equal(document1, document2);
        Assert.True(document1 == document2);
        Assert.False(document1 != document2);
        Assert.Equal(document1.GetHashCode(), document2.GetHashCode());
    }

    [Fact]
    public void EmployeeDocument_WhenTwoDocumentsWithDifferentNumber_ThenAreNotEqual()
    {
        // Arrange
        var document1 = new EmployeeDocument("12345678901");
        var document2 = new EmployeeDocument("98765432109");

        // Act & Assert
        Assert.NotEqual(document1, document2);
        Assert.False(document1 == document2);
        Assert.True(document1 != document2);
    }

    [Fact]
    public void EmployeeDocument_WhenComparedToNull_ThenIsNotEqual()
    {
        // Arrange
        var document = new EmployeeDocument("12345678901");

        // Act & Assert
        Assert.False(document.Equals(null));
        Assert.False(document == null);
        Assert.True(document != null);
    }

    [Fact]
    public void EmployeeDocument_WhenComparedToDifferentType_ThenIsNotEqual()
    {
        // Arrange
        var document = new EmployeeDocument("12345678901");
        var notADocument = "12345678901";

        // Act & Assert
        Assert.False(document.Equals(notADocument));
    }

    [Fact]
    public void EmployeeDocument_WhenCaseIsDifferent_ThenAreNotEqual()
    {
        // Arrange
        var document1 = new EmployeeDocument("ABC123");
        var document2 = new EmployeeDocument("abc123");

        // Act & Assert
        Assert.NotEqual(document1, document2);
    }

    [Fact]
    public void EmployeeDocument_WhenCopiedWithWith_ThenCreatesNewInstanceWithSameValues()
    {
        // Arrange
        var originalDocument = new EmployeeDocument("12345678901");

        // Act
        var copiedDocument = originalDocument with { };

        // Assert
        Assert.Equal(originalDocument, copiedDocument);
        Assert.Equal(originalDocument.DocumentNumber, copiedDocument.DocumentNumber);
    }

    [Fact]
    public void EmployeeDocument_WhenCopiedWithNewNumber_ThenCreatesNewInstanceWithNewNumber()
    {
        // Arrange
        var originalDocument = new EmployeeDocument("12345678901");
        var newNumber = "98765432109";

        // Act
        var modifiedDocument = originalDocument with { DocumentNumber = newNumber };

        // Assert
        Assert.NotEqual(originalDocument, modifiedDocument);
        Assert.Equal(newNumber, modifiedDocument.DocumentNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void EmployeeDocument_WhenCopiedWithInvalidNumber_ThenCreatesInstanceWithInvalidNumber(string? invalidNumber)
    {
        // Arrange
        var originalDocument = new EmployeeDocument("12345678901");

        // Act
        var modifiedDocument = originalDocument with { DocumentNumber = invalidNumber };

        // Assert
        Assert.Equal(invalidNumber, modifiedDocument.DocumentNumber);
        Assert.NotEqual(originalDocument, modifiedDocument);
    }

    [Fact]
    public void EmployeeDocument_WhenWhitespaceInNumber_ThenPreservesWhitespace()
    {
        // Arrange
        var documentWithSpaces = " 123 456 789 01 ";

        // Act
        var document = new EmployeeDocument(documentWithSpaces);

        // Assert
        Assert.Equal(documentWithSpaces, document.DocumentNumber);
    }
}