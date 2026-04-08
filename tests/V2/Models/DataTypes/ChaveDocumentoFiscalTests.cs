using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="ChaveDocumentoFiscal"/>.
/// </summary>
public sealed class ChaveDocumentoFiscalTests
{
    [Theory]
    [InlineData("12345678901234567890123456789012345678901234567890")]
    [InlineData("A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6Q7R8S9T0U1V2W3X4Y5")] // 50 chars
    [InlineData("12345")]
    public void Constructor_ValidValue_ShouldSetValue(string value)
    {
        // Act
        var chave = new ChaveDocumentoFiscal(value);

        // Assert
        Assert.NotNull(chave);
        Assert.Equal(value, chave.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123456789012345678901234567890123456789012345678901")] // 51 chars
    public void Constructor_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        // Act
        Action act = () => _ = new ChaveDocumentoFiscal(value!);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void FromString_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "1234567890";

        // Act
        var chave = ChaveDocumentoFiscal.FromString(value);

        // Assert
        Assert.NotNull(chave);
        Assert.Equal(value, chave.ToString());
    }

    [Fact]
    public void ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "1234567890";

        // Act
        var chave = (ChaveDocumentoFiscal)value;

        // Assert
        Assert.NotNull(chave);
        Assert.Equal(value, chave.ToString());
    }

    // ============================================
    // ParseIfPresent
    // ============================================

    [Fact]
    public void ParseIfPresent_NullInput_ReturnsNull() =>
        Assert.Null(ChaveDocumentoFiscal.ParseIfPresent(null));

    [Fact]
    public void ParseIfPresent_EmptyStringInput_ReturnsNull() =>
        Assert.Null(ChaveDocumentoFiscal.ParseIfPresent(string.Empty));

    [Fact]
    public void ParseIfPresent_WhitespaceOnlyInput_ReturnsNull() =>
        Assert.Null(ChaveDocumentoFiscal.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_ValidInput_ReturnsInstance() =>
        Assert.Equal("12345", ChaveDocumentoFiscal.ParseIfPresent("12345")!.ToString());

    [Fact]
    public void ParseIfPresent_ExceedingMaxLengthInput_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => ChaveDocumentoFiscal.ParseIfPresent(new string('A', 51)));
}