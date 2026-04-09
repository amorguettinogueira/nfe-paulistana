using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class NumeroTests
{
    [Fact]
    public void DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new Numero().ToString());

    [Fact]
    public void Constructor_WithValidValue_StoresCorrectly() =>
        Assert.Equal("4105", new Numero(4105).ToString());

    [Fact]
    public void Constructor_WithMinValue_Accepted() =>
        Assert.Equal("1", new Numero(1).ToString());

    [Fact]
    public void Constructor_WithMaxValue_Accepted() =>
        Assert.Equal("999999999999", new Numero(999_999_999_999L).ToString());

    [Fact]
    public void Constructor_WithZero_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new Numero(0));
        Assert.Contains("pelo menos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_AboveMax_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new Numero(1_000_000_000_000L));
        Assert.Contains("no máximo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // Construtor string — novo
    // ============================================

    [Fact]
    public void Constructor_WithString_StoresCorrectly() =>
        Assert.Equal("4105", new Numero("4105").ToString());

    [Fact]
    public void Constructor_WithNonNumericString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Numero("NUM-X"));

    [Fact]
    public void Constructor_StringAndLong_ProduceSameResult() =>
        Assert.Equal(new Numero(4105L).ToString(), new Numero("4105").ToString());

    [Fact]
    public void FromInt64_ProducesSameResultAsConstructor() =>
        Assert.Equal(new Numero(4105L).ToString(), Numero.FromInt64(4105L).ToString());

    [Fact]
    public void FromString_ProducesSameResultAsConstructor() =>
        Assert.Equal(new Numero("4105").ToString(), Numero.FromString("4105").ToString());

    [Fact]
    public void ExplicitCast_FromLong_ProducesSameResultAsFromInt64() =>
        Assert.Equal(Numero.FromInt64(4105L).ToString(), ((Numero)4105L).ToString());

    [Fact]
    public void ExplicitCast_FromString_ProducesSameResultAsFromString() =>
        Assert.Equal(Numero.FromString("4105").ToString(), ((Numero)"4105").ToString());

    [Fact]
    public void Numero_IsSealed() =>
        Assert.True(typeof(Numero).IsSealed);

    [Fact]
    public void Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new Numero(4105), new Numero(4105));

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new Numero(4105), new Numero(9999));

    [Fact]
    public void Equals_NullObject_ReturnsFalse() =>
        Assert.False(new Numero(4105).Equals(null));

    [Fact]
    public void Equals_DifferentType_SameSerializedValue_ReturnsFalse()
    {
        var numero = new Numero(10);
        var quantidade = new Quantidade(10);

        Assert.False(numero.Equals(quantidade));
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash() =>
        Assert.Equal(new Numero(4105).GetHashCode(), new Numero(4105).GetHashCode());

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHash() =>
        Assert.NotEqual(new Numero(4105).GetHashCode(), new Numero(9999).GetHashCode());

    // ============================================
    // ParseIfPresent(string?)
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNull_ReturnsNull() =>
        Assert.Null(Numero.ParseIfPresent((string?)null));

    [Fact]
    public void ParseIfPresent_WithWhiteSpace_ReturnsNull() =>
        Assert.Null(Numero.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_WithValidString_ReturnsNumero() =>
        Assert.Equal(new Numero(4105), Numero.ParseIfPresent("4105"));

    [Fact]
    public void ParseIfPresent_WithInvalidString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Numero.ParseIfPresent("NUM-X"));

    // ============================================
    // ParseIfPresent(long?)
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNullLong_ReturnsNull() =>
        Assert.Null(Numero.ParseIfPresent((long?)null));

    [Fact]
    public void ParseIfPresent_WithValidLong_ReturnsNumero() =>
        Assert.Equal(new Numero(4105), Numero.ParseIfPresent((long?)4105L));

    [Fact]
    public void ParseIfPresent_WithInvalidLong_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Numero.ParseIfPresent((long?)0L));
}