using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class CepTests
{
    // ============================================
    // Construtor padrão
    // ============================================

    [Fact]
    public void DefaultConstructor_ToStringReturnsNull()
    {
        Assert.Null(new Cep().ToString());
    }

    // ============================================
    // Cep(int)
    // ============================================

    [Fact]
    public void Constructor_WithMinValidValue_StoresCorrectly()
    {
        Assert.Equal("1000000", new Cep(1_000_000).ToString());
    }

    [Fact]
    public void Constructor_WithMaxValidValue_StoresCorrectly()
    {
        Assert.Equal("99999999", new Cep(99_999_999).ToString());
    }

    [Fact]
    public void Constructor_WithTypicalCep_StoresCorrectly()
    {
        Assert.Equal("1310100", new Cep(1_310_100).ToString());
    }

    [Fact]
    public void Constructor_BelowMin_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Cep(999_999));
        Assert.Contains("pelo menos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_AboveMax_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Cep(100_000_000));
        Assert.Contains("no máximo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // Cep(string)
    // ============================================

    [Fact]
    public void Constructor_WithPlainString_StoresCorrectly()
    {
        Assert.Equal("1310100", new Cep("1310100").ToString());
    }

    [Fact]
    public void Constructor_WithFormattedString_RemovesFormatting()
    {
        Assert.Equal("1310100", new Cep("01310-100").ToString());
    }

    [Fact]
    public void Constructor_WithNonNumericString_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Cep("AB-CEP"));
        Assert.Contains("caracteres não numéricos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // Factory methods
    // ============================================

    [Fact]
    public void FromString_ProducesSameResultAsConstructor()
    {
        Assert.Equal(new Cep("1310100").ToString(), Cep.FromString("1310100").ToString());
    }

    [Fact]
    public void FromInt32_ProducesSameResultAsConstructor()
    {
        Assert.Equal(new Cep(1_310_100).ToString(), Cep.FromInt32(1_310_100).ToString());
    }

    // ============================================
    // Cast operators
    // ============================================

    [Fact]
    public void ExplicitCast_FromInt_ProducesSameResultAsFromInt32()
    {
        Assert.Equal(Cep.FromInt32(5_000_000).ToString(), ((Cep)5_000_000).ToString());
    }

    [Fact]
    public void ExplicitCast_FromString_ProducesSameResultAsFromString()
    {
        Assert.Equal(Cep.FromString("5000000").ToString(), ((Cep)"5000000").ToString());
    }

    // ============================================
    // sealed / Equals / GetHashCode
    // ============================================

    [Fact]
    public void Cep_IsSealed() => Assert.True(typeof(Cep).IsSealed);

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        Assert.Equal(new Cep(1_310_100), new Cep(1_310_100));
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        Assert.NotEqual(new Cep(1_310_100), new Cep(5_000_000));
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash()
    {
        Assert.Equal(new Cep(1_310_100).GetHashCode(), new Cep(1_310_100).GetHashCode());
    }
}
