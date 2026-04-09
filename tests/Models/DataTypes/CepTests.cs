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

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHash()
    {
        Assert.NotEqual(new Cep(1_310_100).GetHashCode(), new Cep(5_000_000).GetHashCode());
    }

    [Fact]
    public void Equals_NullObject_ReturnsFalse()
    {
        Assert.False(new Cep(1_310_100).Equals(null));
    }

    [Fact]
    public void Equals_DifferentType_SameSerializedValue_ReturnsFalse()
    {
        var cep = new Cep(1_310_100);
        var ibge = new CodigoIbge(1_310_100);

        Assert.False(cep.Equals(ibge));
    }

    [Fact]
    public void ExplicitCast_FromInt_WithInvalidInput_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => { var _ = (Cep)999_999; });
        Assert.Contains("pelo menos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExplicitCast_FromString_WithInvalidInput_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => { var _ = (Cep)"AB-CEP"; });
        Assert.Contains("caracteres não numéricos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // ParseIfPresent(string?)
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNull_ReturnsNull() =>
        Assert.Null(Cep.ParseIfPresent((string?)null));

    [Fact]
    public void ParseIfPresent_WithWhiteSpace_ReturnsNull() =>
        Assert.Null(Cep.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_WithValidString_ReturnsCep() =>
        Assert.Equal(new Cep(1_310_100), Cep.ParseIfPresent("1310100"));

    [Fact]
    public void ParseIfPresent_WithInvalidString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Cep.ParseIfPresent("AB-CEP"));

    // ============================================
    // ParseIfPresent(int?)
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNullInt_ReturnsNull() =>
        Assert.Null(Cep.ParseIfPresent((int?)null));

    [Fact]
    public void ParseIfPresent_WithValidInt_ReturnsCep() =>
        Assert.Equal(new Cep(1_310_100), Cep.ParseIfPresent((int?)1_310_100));

    [Fact]
    public void ParseIfPresent_WithInvalidInt_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Cep.ParseIfPresent((int?)999_999));
}
