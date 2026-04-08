using Nfe.Paulistana.V2.Models.DataTypes;
using System.Reflection;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="InscricaoImobiliaria"/>.
/// </summary>
public sealed class InscricaoImobiliariaTests
{
    [Theory]
    [InlineData("123456789012345678901234567890")] // 30 chars
    [InlineData("A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5")] // 30 chars
    [InlineData("INCRA1234567890123456789012345")] // 30 chars
    public void Constructor_ValidValue_ShouldSetValue(string value)
    {
        // Act
        var inscricao = new InscricaoImobiliaria(value);

        // Assert
        Assert.NotNull(inscricao);
        Assert.Equal(
            value.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;").Replace("'", "&apos;"),
            inscricao.ToString()
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234567890123456789012345678901")] // 31 chars
    public void Constructor_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        // Act
        Action act = () => new InscricaoImobiliaria(value!);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void FromString_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "SQL123456789012345678901234567";

        // Act
        var inscricao = InscricaoImobiliaria.FromString(value);

        // Assert
        Assert.NotNull(inscricao);
        Assert.Equal(
            value.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;").Replace("'", "&apos;"),
            inscricao.ToString()
        );
    }

    [Fact]
    public void ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "INCRA1234567890123456789012345";

        // Act
        var inscricao = (InscricaoImobiliaria)value;

        // Assert
        Assert.NotNull(inscricao);
        Assert.Equal(
            value.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;").Replace("'", "&apos;"),
            inscricao.ToString()
        );
    }

    [Fact]
    public void GetMaxLength_ShouldReturnThirty()
    {
        // Act
        var inscricao = new InscricaoImobiliaria("A");
        var maxLength = inscricao.GetType().GetMethod("GetMaxLength", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(inscricao, null);

        // Assert
        Assert.Equal(30, maxLength);
    }

    // ============================================
    // ParseIfPresent
    // ============================================

    [Fact]
    public void ParseIfPresent_NullInput_ReturnsNull() =>
        Assert.Null(InscricaoImobiliaria.ParseIfPresent(null));

    [Fact]
    public void ParseIfPresent_EmptyStringInput_ReturnsNull() =>
        Assert.Null(InscricaoImobiliaria.ParseIfPresent(string.Empty));

    [Fact]
    public void ParseIfPresent_WhitespaceOnlyInput_ReturnsNull() =>
        Assert.Null(InscricaoImobiliaria.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_ValidInput_ReturnsInstance() =>
        Assert.Equal("InscricaoValida", InscricaoImobiliaria.ParseIfPresent("InscricaoValida")!.ToString());

    [Fact]
    public void ParseIfPresent_ExceedingMaxLengthInput_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => InscricaoImobiliaria.ParseIfPresent(new string('A', 31)));
}
