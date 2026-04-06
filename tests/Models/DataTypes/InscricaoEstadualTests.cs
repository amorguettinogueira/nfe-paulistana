using Nfe.Paulistana.Models.DataTypes;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class InscricaoEstadualTests
{
    [Fact]
    public void DefaultConstructor_ToStringReturnsNull()
    {
        Assert.Null(new InscricaoEstadual().ToString());
    }

    [Fact]
    public void Constructor_WithValidValue_StoresCorrectly()
    {
        Assert.Equal("123456789", new InscricaoEstadual(123_456_789UL).ToString());
    }

    [Fact]
    public void Constructor_WithMinValue_Accepted()
    {
        Assert.Equal("1", new InscricaoEstadual(1UL).ToString());
    }

    [Fact]
    public void Constructor_WithMaxValue_Accepted()
    {
        Assert.Equal("9999999999999999999", new InscricaoEstadual(9_999_999_999_999_999_999UL).ToString());
    }

    [Fact]
    public void Constructor_WithZero_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new InscricaoEstadual(0UL));
        Assert.Contains("pelo menos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_WithString_StoresCorrectly()
    {
        Assert.Equal("123456789", new InscricaoEstadual("123456789").ToString());
    }

    [Fact]
    public void Constructor_WithNonNumericString_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentException>(() => new InscricaoEstadual("ABC-IE"));
    }

    [Fact]
    public void FromString_ProducesSameResultAsConstructor()
    {
        Assert.Equal(new InscricaoEstadual("123456789").ToString(), InscricaoEstadual.FromString("123456789").ToString());
    }

    [Fact]
    public void FromUInt64_ProducesSameResultAsConstructor()
    {
        Assert.Equal(new InscricaoEstadual(123_456_789UL).ToString(), InscricaoEstadual.FromUInt64(123_456_789UL).ToString());
    }

    [Fact]
    public void ExplicitCast_FromUlong_ProducesSameResultAsFromUInt64()
    {
        Assert.Equal(InscricaoEstadual.FromUInt64(123_456_789UL).ToString(), ((InscricaoEstadual)123_456_789UL).ToString());
    }

    [Fact]
    public void InscricaoEstadual_IsSealed() => Assert.True(typeof(InscricaoEstadual).IsSealed);

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        Assert.Equal(new InscricaoEstadual(123_456_789UL), new InscricaoEstadual(123_456_789UL));
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        Assert.NotEqual(new InscricaoEstadual(123_456_789UL), new InscricaoEstadual(987_654_321UL));
    }
}
