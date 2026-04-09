using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class CodigoIbgeTests
{
    [Fact]
    public void DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new CodigoIbge().ToString());

    [Fact]
    public void Constructor_WithValidCode_StoresCorrectly() =>
        Assert.Equal("3550308", new CodigoIbge(3_550_308).ToString());

    [Fact]
    public void Constructor_WithMinValidValue_Accepted() =>
        Assert.Equal("1000000", new CodigoIbge(1_000_000).ToString());

    [Fact]
    public void Constructor_WithMaxValidValue_Accepted() =>
        Assert.Equal("9999999", new CodigoIbge(9_999_999).ToString());

    [Fact]
    public void Constructor_BelowMin_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new CodigoIbge(999_999));
        Assert.Contains("pelo menos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_AboveMax_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new CodigoIbge(100_000_000));
        Assert.Contains("no máximo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // Construtor string — inclui fix RemoveFormatting
    // ============================================

    [Fact]
    public void Constructor_WithPlainString_StoresCorrectly() =>
        Assert.Equal("3550308", new CodigoIbge("3550308").ToString());

    [Fact]
    public void Constructor_WithFormattedString_RemovesFormatting() =>
        Assert.Equal("3550308", new CodigoIbge("35.50308").ToString());

    [Fact]
    public void Constructor_WithNonNumericString_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new CodigoIbge("ABC-DEF"));
        Assert.Contains("caracteres não numéricos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FromString_ProducesSameResultAsConstructor() =>
        Assert.Equal(new CodigoIbge("3550308").ToString(), CodigoIbge.FromString("3550308").ToString());

    [Fact]
    public void FromString_WithInvalidInput_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => CodigoIbge.FromString("ABC-DEF"));
        Assert.Contains("caracteres não numéricos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FromInt32_ProducesSameResultAsConstructor() =>
        Assert.Equal(new CodigoIbge(3_550_308).ToString(), CodigoIbge.FromInt32(3_550_308).ToString());

    [Fact]
    public void FromInt32_WithInvalidInput_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => CodigoIbge.FromInt32(999_999));
        Assert.Contains("pelo menos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExplicitCast_FromInt_ProducesSameResultAsFromInt32() =>
        Assert.Equal(CodigoIbge.FromInt32(3_550_308).ToString(), ((CodigoIbge)3_550_308).ToString());

    [Fact]
    public void ExplicitCast_FromString_WithInvalidInput_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => { var _ = (CodigoIbge)"ABC-DEF"; });
        Assert.Contains("caracteres não numéricos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExplicitCast_FromInt_WithInvalidInput_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => { var _ = (CodigoIbge)999_999; });
        Assert.Contains("pelo menos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CodigoIbge_IsSealed() =>
        Assert.True(typeof(CodigoIbge).IsSealed);

    [Fact]
    public void Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new CodigoIbge(3_550_308), new CodigoIbge(3_550_308));

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new CodigoIbge(3_550_308), new CodigoIbge(3_304_557));

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash() =>
        Assert.Equal(new CodigoIbge(3_550_308).GetHashCode(), new CodigoIbge(3_550_308).GetHashCode());

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHash() =>
        Assert.NotEqual(new CodigoIbge(3_550_308).GetHashCode(), new CodigoIbge(3_304_557).GetHashCode());

    [Fact]
    public void Equals_NullObject_ReturnsFalse() =>
        Assert.False(new CodigoIbge(3_550_308).Equals(null));

    [Fact]
    public void Equals_DifferentType_SameSerializedValue_ReturnsFalse()
    {
        var ibge = new CodigoIbge(1_310_100);
        var cep = new Cep(1_310_100);

        Assert.False(ibge.Equals(cep));
    }

    [Fact]
    public void ExplicitCast_FromString_ProducesSameResultAsFromString() =>
        Assert.Equal(CodigoIbge.FromString("3550308").ToString(), ((CodigoIbge)"3550308").ToString());

    // ============================================
    // ParseIfPresent(string?)
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNull_ReturnsNull() =>
        Assert.Null(CodigoIbge.ParseIfPresent((string?)null));

    [Fact]
    public void ParseIfPresent_WithWhiteSpace_ReturnsNull() =>
        Assert.Null(CodigoIbge.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_WithValidString_ReturnsCodigoIbge() =>
        Assert.Equal(new CodigoIbge(3_550_308), CodigoIbge.ParseIfPresent("3550308"));

    [Fact]
    public void ParseIfPresent_WithInvalidString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CodigoIbge.ParseIfPresent("ABC-DEF"));

    // ============================================
    // ParseIfPresent(int?)
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNullInt_ReturnsNull() =>
        Assert.Null(CodigoIbge.ParseIfPresent((int?)null));

    [Fact]
    public void ParseIfPresent_WithValidInt_ReturnsCodigoIbge() =>
        Assert.Equal(new CodigoIbge(3_550_308), CodigoIbge.ParseIfPresent((int?)3_550_308));

    [Fact]
    public void ParseIfPresent_WithInvalidInt_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => CodigoIbge.ParseIfPresent((int?)999_999));
}