using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class DiscriminacaoTests
{
    [Fact]
    public void Discriminacao_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new Discriminacao().ToString());

    [Fact]
    public void Discriminacao_Constructor_WithValidValue_StoresCorrectly() =>
        Assert.Equal("Desenvolvimento de software.", new Discriminacao("Desenvolvimento de software.").ToString());

    [Fact]
    public void Discriminacao_Constructor_WithMaxLengthValue_StoresCorrectly()
    {
        string value = new('D', 2000);
        Assert.Equal(value, new Discriminacao(value).ToString());
    }

    [Fact]
    public void Discriminacao_Constructor_ExceedingMaxLength_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Discriminacao(new string('D', 2001)));

    [Fact]
    public void Discriminacao_Constructor_WithEmptyString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Discriminacao(string.Empty));

    [Fact]
    public void Discriminacao_Constructor_WithLineBreaks_ReplacesWithPipe()
    {
        var input = "Linha1\r\nLinha2\nLinha3\rLinha4";
        var expected = "Linha1|Linha2|Linha3|Linha4";
        Assert.Equal(expected, new Discriminacao(input).ToString());
    }

    [Fact]
    public void Discriminacao_Constructor_WithWhitespaceOnly_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Discriminacao("   "));

    [Fact]
    public void Discriminacao_FromString_WithValidValue_ReturnsInstance()
        => Assert.Equal("Teste", Discriminacao.FromString("Teste").ToString());

    [Fact]
    public void Discriminacao_FromString_WithInvalidValue_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => Discriminacao.FromString(""));

    [Fact]
    public void Discriminacao_ExplicitCast_FromString_ValidValue_ReturnsInstance()
        => Assert.Equal("Teste", ((Discriminacao)"Teste").ToString());

    [Fact]
    public void Discriminacao_ExplicitCast_FromString_InvalidValue_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => { var _ = (Discriminacao)string.Empty; });

    [Fact]
    public void Discriminacao_IsSealed() =>
        Assert.True(typeof(Discriminacao).IsSealed);

    [Fact]
    public void Discriminacao_Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new Discriminacao("Serviço X"), new Discriminacao("Serviço X"));

    [Fact]
    public void Discriminacao_Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new Discriminacao("Serviço X"), new Discriminacao("Serviço Y"));
}