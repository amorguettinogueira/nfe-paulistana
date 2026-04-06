using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="CodigoEndPostal"/>.
/// </summary>
public sealed class CodigoEndPostalTests
{
    // =============================
    // CodigoEndPostal
    // =============================

    [Fact]
    public void CodigoEndPostal_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new CodigoEndPostal().ToString());

    [Theory]
    [InlineData("12345678901")]
    [InlineData("A1B2C3D4E5F")]
    [InlineData("S&>")]
    [InlineData("<\"")]
    [InlineData("N'")]
    public void CodigoEndPostal_ValidValue_ShouldSetValue(string value)
    {
        var cep = new CodigoEndPostal(value);
        Assert.NotNull(cep);
        Assert.Equal(
            value.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;").Replace("'", "&apos;"),
            cep.ToString()
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123456789012")] // 12 chars
    public void CodigoEndPostal_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        Action act = () => new CodigoEndPostal(value!);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void CodigoEndPostal_FromString_ValidValue_ShouldReturnInstance()
    {
        const string value = "12345678901";
        var cep = CodigoEndPostal.FromString(value);
        Assert.NotNull(cep);
        Assert.Equal(value, cep.ToString());
    }

    [Fact]
    public void CodigoEndPostal_ExplicitOperator_ValidValue_ShouldReturnInstance()
    {
        const string value = "12345678901";
        var cep = (CodigoEndPostal)value;
        Assert.NotNull(cep);
        Assert.Equal(value, cep.ToString());
    }
}