using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="CodigoPaisISO"/>.
/// </summary>
public sealed class CodigoPaisISOTests
{
    [Fact]
    public void CodigoPaisISO_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new CodigoPaisISO().ToString());

    [Theory]
    [InlineData("BR")]
    [InlineData("US")]
    [InlineData("FR")]
    public void CodigoPaisISO_ValidValue_ShouldSetValue(string value)
    {
        var cod = new CodigoPaisISO(value);
        Assert.Equal(value, cod.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("B")]
    [InlineData("BRA")]
    [InlineData("br")]
    [InlineData("1A")]
    public void CodigoPaisISO_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        Action act = () => _ = new CodigoPaisISO(value!);
        Assert.ThrowsAny<ArgumentException>(act);
    }
}