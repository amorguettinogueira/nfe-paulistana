using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class TributacaoNfeTests
{
    // ============================================
    // Construtor padrão
    // ============================================

    [Fact]
    public void DefaultConstructor_ToStringReturnsNull()
    {
        Assert.Null(new TributacaoNfe().ToString());
    }

    // ============================================
    // TributacaoNfe(char) — letras válidas
    // ============================================

    [Theory]
    [InlineData('T')]
    [InlineData('F')]
    [InlineData('A')]
    [InlineData('B')]
    [InlineData('M')]
    [InlineData('t')]
    [InlineData('f')]
    public void Constructor_WithValidLetter_StoresCorrectly(char value)
    {
        Assert.Equal(value.ToString(), new TributacaoNfe(value).ToString());
    }

    // ============================================
    // TributacaoNfe(char) — caracteres inválidos
    // ============================================

    [Theory]
    [InlineData('1')]
    [InlineData('9')]
    [InlineData('0')]
    [InlineData(' ')]
    [InlineData('!')]
    [InlineData('@')]
    [InlineData('\0')]
    public void Constructor_WithNonLetter_ThrowsArgumentException(char invalid)
    {
        var ex = Assert.Throws<ArgumentException>(() => new TributacaoNfe(invalid));
        Assert.Contains("letra", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // Factory / operador de cast
    // ============================================

    [Fact]
    public void FromChar_WithValidLetter_StoresCorrectly()
    {
        Assert.Equal("T", TributacaoNfe.FromChar('T').ToString());
    }

    [Fact]
    public void CastOperator_WithValidLetter_StoresCorrectly()
    {
        Assert.Equal("T", ((TributacaoNfe)'T').ToString());
    }

    [Fact]
    public void FromChar_WithNonLetter_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TributacaoNfe.FromChar('1'));
    }

    // ============================================
    // Sealed / Equals / GetHashCode
    // ============================================

    [Fact]
    public void IsSealed()
    {
        Assert.True(typeof(TributacaoNfe).IsSealed);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        Assert.Equal(new TributacaoNfe('T'), new TributacaoNfe('T'));
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        Assert.NotEqual(new TributacaoNfe('T'), new TributacaoNfe('F'));
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsEqualHash()
    {
        Assert.Equal(new TributacaoNfe('T').GetHashCode(), new TributacaoNfe('T').GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHash()
    {
        Assert.NotEqual(new TributacaoNfe('T').GetHashCode(), new TributacaoNfe('F').GetHashCode());
    }

    [Fact]
    public void Equals_NullObject_ReturnsFalse()
    {
        Assert.False(new TributacaoNfe('T').Equals(null));
    }

    // ============================================
    // FromString
    // ============================================

    [Fact]
    public void FromString_WithSingleValidLetter_StoresFirstChar()
    {
        Assert.Equal("T", TributacaoNfe.FromString("T").ToString());
    }

    [Fact]
    public void FromString_WithMultipleChars_UsesFirstChar()
    {
        Assert.Equal("T", TributacaoNfe.FromString("TAF").ToString());
    }

    [Fact]
    public void FromString_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => TributacaoNfe.FromString(null!));
    }

    [Fact]
    public void FromString_WithEmptyString_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TributacaoNfe.FromString(string.Empty));
    }

    [Fact]
    public void FromString_WithNonLetterFirstChar_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TributacaoNfe.FromString("1XY"));
    }

    [Fact]
    public void ExplicitCast_FromString_ProducesSameResultAsFromString()
    {
        Assert.Equal(TributacaoNfe.FromString("F").ToString(), ((TributacaoNfe)"F").ToString());
    }
}
