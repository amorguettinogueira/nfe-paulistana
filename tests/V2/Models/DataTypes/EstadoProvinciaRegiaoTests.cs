using Nfe.Paulistana.V2.Models.DataTypes;
using System.Reflection;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="EstadoProvinciaRegiao"/>.
/// </summary>
public sealed class EstadoProvinciaRegiaoTests
{
    [Theory]
    [InlineData("São Paulo")]
    [InlineData("California")]
    [InlineData("British Columbia")]
    [InlineData("A")]
    [InlineData("A very long state name that is exactly sixty characters long")] // 60 chars
    public void Constructor_ValidValue_ShouldSetValue(string value)
    {
        // Act
        var estado = new EstadoProvinciaRegiao(value);

        // Assert
        Assert.NotNull(estado);
        Assert.Equal(
            value.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;").Replace("'", "&apos;"),
            estado.ToString()
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("This string is longer than sixty characters and should throw an exception because it exceeds the limit.")]
    public void Constructor_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        // Act
        Action act = () => new EstadoProvinciaRegiao(value!);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void FromString_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "Paraná";

        // Act
        var estado = EstadoProvinciaRegiao.FromString(value);

        // Assert
        Assert.NotNull(estado);
        Assert.Equal(
            value.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;").Replace("'", "&apos;"),
            estado.ToString()
        );
    }

    [Fact]
    public void ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        // Arrange
        var value = "Rio de Janeiro";

        // Act
        var estado = (EstadoProvinciaRegiao)value;

        // Assert
        Assert.NotNull(estado);
        Assert.Equal(
            value.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;").Replace("'", "&apos;"),
            estado.ToString()
        );
    }

    [Fact]
    public void GetMaxLength_ShouldReturnSixty()
    {
        // Act
        var estado = new EstadoProvinciaRegiao("A");
        var maxLength = estado.GetType().GetMethod("GetMaxLength", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(estado, null);

        // Assert
        Assert.Equal(60, maxLength);
    }

    // ============================================
    // ParseIfPresent
    // ============================================

    [Fact]
    public void ParseIfPresent_NullInput_ReturnsNull() =>
        Assert.Null(EstadoProvinciaRegiao.ParseIfPresent(null));

    [Fact]
    public void ParseIfPresent_EmptyStringInput_ReturnsNull() =>
        Assert.Null(EstadoProvinciaRegiao.ParseIfPresent(string.Empty));

    [Fact]
    public void ParseIfPresent_WhitespaceOnlyInput_ReturnsNull() =>
        Assert.Null(EstadoProvinciaRegiao.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_ValidInput_ReturnsInstance() =>
        Assert.Equal("São Paulo", EstadoProvinciaRegiao.ParseIfPresent("São Paulo")!.ToString());

    [Fact]
    public void ParseIfPresent_ExceedingMaxLengthInput_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => EstadoProvinciaRegiao.ParseIfPresent(new string('A', 61)));
}