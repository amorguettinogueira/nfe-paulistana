using Nfe.Paulistana.V2.Models.DataTypes;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="CodigoOperacaoFornecimento"/>.
/// </summary>
public sealed class CodigoOperacaoFornecimentoTests
{
    [Fact]
    public void CodigoOperacaoFornecimento_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new CodigoOperacaoFornecimento().ToString());

    [Theory]
    [InlineData("123456")]
    [InlineData("000001")]
    [InlineData("999999")]
    public void CodigoOperacaoFornecimento_ValidValue_ShouldSetValue(string value)
    {
        var cod = new CodigoOperacaoFornecimento(value);
        Assert.Equal(value, cod.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("12345")] // menos de 6
    [InlineData("1234567")] // mais de 6
    [InlineData("12A456")] // caractere inválido
    public void CodigoOperacaoFornecimento_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        Action act = () => _ = new CodigoOperacaoFornecimento(value!);
        Assert.ThrowsAny<ArgumentException>(act);
    }

    [Theory]
    [InlineData("123456")]
    public void FromString_ValidValue_ShouldReturnInstance(string value)
    {
        var cod = CodigoOperacaoFornecimento.FromString(value);
        Assert.Equal(value, cod.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12345")]
    public void FromString_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        Action act = () => _ = CodigoOperacaoFornecimento.FromString(value!);
        Assert.ThrowsAny<ArgumentException>(act);
    }
}