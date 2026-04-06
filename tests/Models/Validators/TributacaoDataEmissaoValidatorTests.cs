using Nfe.Paulistana.Models.DataTypes;
using Nfe.Paulistana.Models.Validators;

namespace Nfe.Paulistana.Tests.Models.Validators;

public class TributacaoDataEmissaoValidatorTests
{
    [Theory]
    [InlineData('T')]
    [InlineData('F')]
    [InlineData('I')]
    [InlineData('J')]
    public void ThrowIfInvalid_ValidBefore2015_AllowedCodes_DoesNotThrow(char code)
    {
        var exception = Record.Exception(() =>
            TributacaoDataEmissaoValidator.ThrowIfInvalid(new TributacaoNfe(code), new DataXsd(new DateTime(2015, 2, 22))));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData('A')]
    [InlineData('B')]
    [InlineData('D')]
    [InlineData('M')]
    [InlineData('N')]
    [InlineData('R')]
    [InlineData('S')]
    [InlineData('X')]
    [InlineData('V')]
    [InlineData('P')]
    public void ThrowIfInvalid_InvalidBefore2015_DisallowedCodes_ThrowsArgumentException(char code)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            TributacaoDataEmissaoValidator.ThrowIfInvalid(new TributacaoNfe(code), new DataXsd(new DateTime(2015, 2, 22))));

        Assert.Contains("T, F, I, J", exception.Message);
    }

    [Theory]
    [InlineData('T')]
    [InlineData('F')]
    [InlineData('A')]
    [InlineData('B')]
    [InlineData('D')]
    [InlineData('M')]
    [InlineData('N')]
    [InlineData('R')]
    [InlineData('S')]
    [InlineData('X')]
    [InlineData('V')]
    [InlineData('P')]
    public void ThrowIfInvalid_ValidAfter2015_AllowedCodes_DoesNotThrow(char code)
    {
        var exception = Record.Exception(() =>
            TributacaoDataEmissaoValidator.ThrowIfInvalid(new TributacaoNfe(code), new DataXsd(new DateTime(2015, 2, 23))));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData('I')]
    [InlineData('J')]
    public void ThrowIfInvalid_InvalidAfter2015_DisallowedCodes_ThrowsArgumentException(char code)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            TributacaoDataEmissaoValidator.ThrowIfInvalid(new TributacaoNfe(code), new DataXsd(new DateTime(2015, 2, 23))));

        Assert.Contains("T, F, A, B, D, M, N, R, S, X, V, P", exception.Message);
    }

    [Fact]
    public void ThrowIfInvalid_NullArguments_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => TributacaoDataEmissaoValidator.ThrowIfInvalid(null!, new DataXsd(DateTime.Now)));
        Assert.Throws<ArgumentNullException>(() => TributacaoDataEmissaoValidator.ThrowIfInvalid(new TributacaoNfe('T'), null!));
    }
}