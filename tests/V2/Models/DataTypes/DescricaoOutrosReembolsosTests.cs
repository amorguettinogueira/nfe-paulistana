using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="DescricaoOutrosReembolsos"/>.
/// </summary>
public sealed class DescricaoOutrosReembolsosTests
{
    [Fact]
    public void DescricaoOutrosReembolsos_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new DescricaoOutrosReembolsos().ToString());

    [Theory]
    [InlineData("Reembolso válido")]
    [InlineData("A")] // mínimo
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")] // 150 chars
    public void DescricaoOutrosReembolsos_ValidValue_ShouldSetValue(string value)
    {
        var desc = new DescricaoOutrosReembolsos(value);
        Assert.Equal(value, desc.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901")] // 151 chars
    public void DescricaoOutrosReembolsos_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        Action act = () => _ = new DescricaoOutrosReembolsos(value!);
        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData("Reembolso via FromString")]
    public void FromString_ValidValue_ShouldReturnInstance(string value)
    {
        var desc = DescricaoOutrosReembolsos.FromString(value);
        Assert.Equal(value, desc.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void FromString_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        Action act = () => _ = DescricaoOutrosReembolsos.FromString(value!);
        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData("Reembolso via cast")]
    public void ExplicitOperator_ValidValue_ShouldReturnInstance(string value)
    {
        var desc = (DescricaoOutrosReembolsos)value;
        Assert.Equal(value, desc.ToString());
    }
}