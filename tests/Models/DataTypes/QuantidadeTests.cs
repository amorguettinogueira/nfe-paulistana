using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class QuantidadeTests
{
    [Fact]
    public void DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new Quantidade().ToString());

    [Fact]
    public void Constructor_WithValidValue_StoresCorrectly() =>
        Assert.Equal("10", new Quantidade(10).ToString());

    [Fact]
    public void Constructor_WithMinValue_Accepted() =>
        Assert.Equal("1", new Quantidade(1).ToString());

    [Fact]
    public void Constructor_WithMaxValue_Accepted() =>
        Assert.Equal("999999999999999", new Quantidade(999_999_999_999_999L).ToString());

    [Fact]
    public void Constructor_WithZero_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new Quantidade(0));
        Assert.Contains("pelo menos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_AboveMax_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new Quantidade(1_000_000_000_000_000L));
        Assert.Contains("no máximo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // Construtor string — novo
    // ============================================

    [Fact]
    public void Constructor_WithString_StoresCorrectly() =>
        Assert.Equal("10", new Quantidade("10").ToString());

    [Fact]
    public void Constructor_WithNonNumericString_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Quantidade("QTD-X"));

    [Fact]
    public void Constructor_StringAndLong_ProduceSameResult() =>
        Assert.Equal(new Quantidade(50L).ToString(), new Quantidade("50").ToString());

    [Fact]
    public void FromInt64_ProducesSameResultAsConstructor() =>
        Assert.Equal(new Quantidade(50L).ToString(), Quantidade.FromInt64(50L).ToString());

    [Fact]
    public void FromString_ProducesSameResultAsConstructor() =>
        Assert.Equal(new Quantidade("50").ToString(), Quantidade.FromString("50").ToString());

    [Fact]
    public void ExplicitCast_FromLong_ProducesSameResultAsFromInt64() =>
        Assert.Equal(Quantidade.FromInt64(50L).ToString(), ((Quantidade)50L).ToString());

    [Fact]
    public void ExplicitCast_FromString_ProducesSameResultAsFromString() =>
        Assert.Equal(Quantidade.FromString("50").ToString(), ((Quantidade)"50").ToString());

    [Fact]
    public void Quantidade_IsSealed() =>
        Assert.True(typeof(Quantidade).IsSealed);

    [Fact]
    public void Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new Quantidade(10), new Quantidade(10));

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new Quantidade(10), new Quantidade(20));
}