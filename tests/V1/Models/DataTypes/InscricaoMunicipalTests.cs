using Nfe.Paulistana.V1.Models.DataTypes;

namespace Nfe.Paulistana.Tests.V1.Models.DataTypes;

public class InscricaoMunicipalTests
{
    [Fact]
    public void DefaultConstructor_ToStringReturnsNull()
    {
        Assert.Null(new InscricaoMunicipal().ToString());
    }

    // ============================================
    // Zero-padding — comportamento central da classe
    // ============================================

    [Fact]
    public void Constructor_WithFullValue_NoPaddingNeeded()
    {
        Assert.Equal("39616924", new InscricaoMunicipal(39_616_924).ToString());
    }

    [Fact]
    public void Constructor_WithMinValue_PadsToEightDigits()
    {
        Assert.Equal("00000001", new InscricaoMunicipal(1).ToString());
    }

    [Fact]
    public void Constructor_WithSingleDigit_ProducesEightCharString()
    {
        Assert.Equal(8, new InscricaoMunicipal(5).ToString()!.Length);
    }

    [Fact]
    public void Constructor_WithMaxValidValue_StoresCorrectly()
    {
        Assert.Equal("99999999", new InscricaoMunicipal(99_999_999).ToString());
    }

    [Fact]
    public void Constructor_WithZero_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new InscricaoMunicipal(0));
        Assert.Contains("pelo menos", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_AboveMax_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new InscricaoMunicipal(100_000_000));
        Assert.Contains("no máximo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // Construtor string
    // ============================================

    [Fact]
    public void Constructor_WithString_AppliesPaddingCorrectly()
    {
        Assert.Equal("00000001", new InscricaoMunicipal("1").ToString());
    }

    [Fact]
    public void Constructor_WithFormattedString_RemovesFormatting()
    {
        Assert.Equal("39616924", new InscricaoMunicipal("39.616.924").ToString());
    }

    [Fact]
    public void Constructor_WithNonNumericString_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentException>(() => new InscricaoMunicipal("ABC-IM"));
    }

    [Fact]
    public void FromString_ProducesSameResultAsConstructor()
    {
        Assert.Equal(new InscricaoMunicipal("39616924").ToString(), InscricaoMunicipal.FromString("39616924").ToString());
    }

    [Fact]
    public void FromInt32_ProducesSameResultAsConstructor()
    {
        Assert.Equal(new InscricaoMunicipal(39_616_924).ToString(), InscricaoMunicipal.FromInt32(39_616_924).ToString());
    }

    [Fact]
    public void ExplicitCast_FromInt_ProducesSameResultAsFromInt32()
    {
        Assert.Equal(InscricaoMunicipal.FromInt32(39_616_924).ToString(), ((InscricaoMunicipal)39_616_924).ToString());
    }

    [Fact]
    public void InscricaoMunicipal_IsSealed() => Assert.True(typeof(InscricaoMunicipal).IsSealed);

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        Assert.Equal(new InscricaoMunicipal(39_616_924), new InscricaoMunicipal(39_616_924));
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        Assert.NotEqual(new InscricaoMunicipal(39_616_924), new InscricaoMunicipal(12_345_678));
    }
}
