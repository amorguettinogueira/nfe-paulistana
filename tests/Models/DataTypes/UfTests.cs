using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class UfTests
{
    // ============================================
    // Construtor padrão
    // ============================================

    [Fact]
    public void DefaultConstructor_ToStringReturnsNull()
    {
        Assert.Null(new Uf().ToString());
    }

    // ============================================
    // Uf(string) — valores válidos
    // ============================================

    [Theory]
    [InlineData("SP")]
    [InlineData("RJ")]
    [InlineData("MG")]
    [InlineData("RS")]
    [InlineData("DF")]
    public void Constructor_WithValidUf_StoresCorrectly(string uf)
    {
        Assert.Equal(uf, new Uf(uf).ToString());
    }

    [Fact]
    public void Constructor_WithLeadingTrailingSpaces_TrimsAndStoresCorrectly()
    {
        Assert.Equal("SP", new Uf("SP").ToString());
    }

    // ============================================
    // Uf(string) — comprimento inválido
    // ============================================

    [Theory]
    [InlineData("S")]
    [InlineData("SPA")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("SPSP")]
    public void Constructor_WithInvalidLength_ThrowsArgumentException(string invalid)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Uf(invalid));
        Assert.Contains("exatamente", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // Factory / operador de cast
    // ============================================

    [Fact]
    public void FromString_WithValidUf_StoresCorrectly()
    {
        Assert.Equal("RJ", Uf.FromString("RJ").ToString());
    }

    [Fact]
    public void CastOperator_WithValidUf_StoresCorrectly()
    {
        Assert.Equal("MG", ((Uf)"MG").ToString());
    }

    // ============================================
    // Sealed / Equals / GetHashCode
    // ============================================

    [Fact]
    public void IsSealed()
    {
        Assert.True(typeof(Uf).IsSealed);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        Assert.Equal(new Uf("SP"), new Uf("SP"));
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        Assert.NotEqual(new Uf("SP"), new Uf("RJ"));
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsEqualHash()
    {
        Assert.Equal(new Uf("SP").GetHashCode(), new Uf("SP").GetHashCode());
    }
}