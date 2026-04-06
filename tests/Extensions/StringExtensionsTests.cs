using Nfe.Paulistana.Extensions;

namespace Nfe.Paulistana.Tests.Extensions;

/// <summary>
/// Testes unitários para a classe StringExtensions.
///
/// Cobre todos os métodos de extensão com cenários normais, edge cases e casos de erro.
/// </summary>
public class StringExtensionsTests
{
    // ============================================
    // RemoveFormatting Tests
    // ============================================

    #region RemoveFormatting Tests

    [Theory]
    [InlineData("123.456.789-01", "12345678901")]         // CPF formatted
    [InlineData("12.345.678/0001-90", "12345678000190")]  // CNPJ formatted
    [InlineData("12345-678", "12345678")]                 // CEP formatted
    [InlineData("(11) 98765-4321", "11987654321")]        // Phone formatted
    [InlineData("123", "123")]                            // Already unformatted
    [InlineData("0123456789", "0123456789")]              // Only digits
    [InlineData("1", "1")]                                // Single digit
    [InlineData("0", "0")]                                // Zero
    [InlineData("00000000000", "00000000000")]            // All zeros
    public void RemoveFormatting_WithFormattedString_ReturnsDigitsOnly(string formatted, string expected)
    {
        // Act
        string result = formatted.RemoveFormatting();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]           // Empty string
    [InlineData("   ")]        // Only spaces
    [InlineData("\t")]         // Tab
    [InlineData("\n")]         // Newline
    [InlineData("...---///")]  // Only formatting chars
    [InlineData("()")]         // Parentheses
    [InlineData("ABC")]        // Only letters
    [InlineData("!@#$%^&*")]   // Special characters
    public void RemoveFormatting_WithNoDigits_ReturnsEmptyString(string value)
    {
        // Act
        string result = value.RemoveFormatting();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void RemoveFormatting_WithNullValue_ReturnsEmptyString()
    {
        // Act
        string result = ((string?)null).RemoveFormatting();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void RemoveFormatting_WithWhitespaceOnlyString_ReturnsEmptyString()
    {
        // Act
        string result = "   \t\n   ".RemoveFormatting();

        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("1a2b3c", "123")]      // Letters interspersed
    [InlineData("1 2 3", "123")]       // Spaces interspersed
    [InlineData("1.2.3", "123")]       // Dots interspersed
    [InlineData("(1)(2)(3)", "123")]   // Parentheses interspersed
    public void RemoveFormatting_WithInterspersedNonDigitCharacters_ExtractsDigitsInOrder(string formatted, string expected)
    {
        // Act
        string result = formatted.RemoveFormatting();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RemoveFormatting_PreservesDigitOrder()
    {
        // Arrange
        string formatted = "9.8.7.6.5.4.3.2.1.0";

        // Act
        string result = formatted.RemoveFormatting();

        // Assert
        Assert.Equal("9876543210", result);
    }

    [Theory]
    [InlineData("123.456.789-01")]      // CPF format
    [InlineData("12.345.678/0001-90")]  // CNPJ format
    [InlineData("12345-678")]           // CEP format
    public void RemoveFormatting_IsIdempotent_ProducesConsistentResults(string input)
    {
        // Act
        string first = input.RemoveFormatting();
        string second = first.RemoveFormatting();

        // Assert - Removing formatting twice should give same result
        Assert.Equal(first, second);
    }

    #endregion RemoveFormatting Tests

    // ============================================
    // Format Tests
    // ============================================

    #region Format Tests

    [Theory]
    [InlineData("O valor é {0}", "O valor é 42", 42)]
    [InlineData("Entre {0} e {1}", "Entre 1 e 2", 1, 2)]
    [InlineData("{0} + {1} = {2}", "1 + 1 = 2", 1, 1, 2)]
    [InlineData("String: {0}", "String: teste", "teste")]
    public void Format_WithValidFormatString_ReturnsFormattedString(string format, string expected, params object[] args)
    {
        // Act
        string result = format.Format(args);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Format_WithSingleArgument_ReturnsFormattedString()
    {
        // Arrange
        string format = "Valor: {0}";
        string arg = "teste";

        // Act
        string result = format.Format(arg);

        // Assert
        Assert.Equal("Valor: teste", result);
    }

    [Fact]
    public void Format_WithDecimalFormatSpecifier_AppliesFormattingCorrectly()
    {
        // Arrange
        string format = "Valor: {0}";
        decimal value = 123.456m;

        // Act
        string result = format.Format(value);

        // Assert
        Assert.Contains("123", result);
    }

    [Fact]
    public void Format_WithIntegerFormatSpecifier_AppliesFormattingCorrectly()
    {
        // Arrange
        string format = "Inteiro: {0}";
        int value = 42;

        // Act
        string result = format.Format(value);

        // Assert
        Assert.Equal("Inteiro: 42", result);
    }

    [Fact]
    public void Format_WithDecimalSeparator_UsesInvariantCulture()
    {
        // Arrange
        string format = "Valor: {0}";
        decimal value = 123.45m;

        // Act
        string result = format.Format(value);

        // Assert - InvariantCulture uses . as separator
        Assert.Contains(".", result);
        Assert.DoesNotContain(",", result);
    }

    [Fact]
    public void Format_WithDateFormatting_UsesInvariantCulture()
    {
        // Arrange
        var date = new DateTime(2023, 12, 25);
        string format = "Data: {0:yyyy-MM-dd}";

        // Act
        string result = format.Format(date);

        // Assert
        Assert.Equal("Data: 2023-12-25", result);
    }

    [Theory]
    [InlineData("Teste {{0}}", "Teste {0}")]  // Escaped brace
    [InlineData("{{", "{")]                   // Escaped opening brace
    [InlineData("}}", "}")]                   // Escaped closing brace
    public void Format_WithEscapedBraces_HandlesProperly(string format, string expected)
    {
        // Act
        string result = format.Format();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Faltando argumento {0}")]
    [InlineData("Índice inválido {5}")]
    public void Format_WithInvalidFormatString_ThrowsFormatException(string format)
    {
        // Act & Assert
        Assert.Throws<FormatException>(() => format.Format());
    }

    [Fact]
    public void Format_WithNullArgument_RendersAsEmpty()
    {
        // Arrange
        string format = "Valor: {0}";
        object? arg = null;

        // Act
        string result = format.Format(arg);

        // Assert
        Assert.Equal("Valor: ", result);
    }

    [Fact]
    public void Format_WithMultipleNullArguments_RendersAsEmpty()
    {
        // Arrange
        string format = "A: {0}, B: {1}, C: {2}";
        object?[] args = [null, null, null];

        // Act
        string result = format.Format(args);

        // Assert
        Assert.Equal("A: , B: , C: ", result);
    }

    [Fact]
    public void Format_UsesInvariantCultureForConsistency()
    {
        // Arrange
        string format = "Número: {0}";
        decimal number = 123.456m;

        // Act
        string result = format.Format(number);

        // Assert - Should use . as decimal separator (InvariantCulture)
        Assert.Contains("123.456", result);
    }

    #endregion Format Tests

    #region Integration Tests

    [Fact]
    public void Format_WithRemoveFormattingResult_WorksCorrectly()
    {
        // Arrange
        string formatted = "123.456.789-01";
        string format = "CPF sem formatação: {0}";

        // Act
        string unformatted = formatted.RemoveFormatting();
        string result = format.Format(unformatted);

        // Assert
        Assert.Equal("CPF sem formatação: 12345678901", result);
    }

    [Theory]
    [InlineData("123.456.789-01", 11)]      // CPF length
    [InlineData("12.345.678/0001-90", 14)]  // CNPJ length
    [InlineData("12345-678", 8)]            // CEP length
    public void RemoveFormatting_OutputLength_MatchesExpectedDigitCount(string formatted, int expectedLength)
    {
        // Act
        string result = formatted.RemoveFormatting();

        // Assert
        Assert.Equal(expectedLength, result.Length);
    }

    #endregion Integration Tests

    // ============================================
    // Edge Cases and Boundary Tests
    // ============================================

    #region Edge Cases

    [Fact]
    public void Format_WithManyArguments_HandlesCorrectly()
    {
        // Arrange
        string format = "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}";
        object[] args = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

        // Act
        string result = format.Format(args);

        // Assert
        Assert.Equal("0 1 2 3 4 5 6 7 8 9", result);
    }

    #endregion Edge Cases
}