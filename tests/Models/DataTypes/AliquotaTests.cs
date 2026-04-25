using Nfe.Paulistana.Models.DataTypes;

using Nfe.Paulistana.Tests.Helpers;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class AliquotaTests
{
    // ============================================
    // Aliquota() — Construtor padrão
    // ============================================

    [Fact]
    public void DefaultConstructor_ToStringReturnsNull() =>
        // Após correção do base(default) → base(), o construtor padrão
        // deixa Value = null, consistente com Valor e Percentual
        Assert.Null(new Aliquota().ToString());

    // ============================================
    // Aliquota(decimal) — Construtor com valor
    // ============================================

    [Fact]
    public void Constructor_WithFivePercent_SerializesWithFourDecimalPlaces() =>
        // FractionalMaxLength = 4: fracionários sempre com 4 casas decimais
        Assert.Equal("0.0500", new Aliquota(0.05m).ToString());

    [Fact]
    public void Constructor_WithZero_SerializesAsZero() =>
        Assert.Equal("0", new Aliquota(0m).ToString());

    [Fact]
    public void Constructor_WithMaxValue_Accepted()
    {
        // Máximo = 10^1 - 10^-4 = 9.9999
        var Aliquota = new Aliquota(9.9999m);
        Assert.Equal("9.9999", Aliquota.ToString());
    }

    [Fact]
    public void Constructor_WithIntegerAliquota_SerializesWithoutDecimals() =>
        Assert.Equal("1", new Aliquota(1m).ToString());

    [Fact]
    public void Constructor_WithFourDecimalPlaces_SerializesCorrectly() =>
        Assert.Equal("0.1234", new Aliquota(0.1234m).ToString());

    [Fact]
    public void Constructor_WithNegativeValue_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Aliquota(-0.01m));

    [Fact]
    public void Constructor_WithValueExceedingMax_ThrowsArgumentException() =>
        // 10 > 9.9999 (máximo permitido)
        Assert.Throws<ArgumentException>(() => new Aliquota(10m));

    [Theory]
    [InlineData(0)]
    [InlineData(0.05)]
    [InlineData(0.5)]
    [InlineData(9.9999)]
    public void Constructor_WithValidValues_DoesNotThrow(double rawValue)
    {
        Exception ex = Record.Exception(() => new Aliquota((decimal)rawValue));
        Assert.Null(ex);
    }

    // ============================================
    // FromDecimal — Factory method
    // ============================================

    [Fact]
    public void FromDecimal_ProducesSameResultAsConstructor() =>
        Assert.Equal(new Aliquota(0.05m).ToString(), Aliquota.FromDecimal(0.05m).ToString());

    [Fact]
    public void FromDecimal_WithNegativeValue_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Aliquota.FromDecimal(-0.01m));

    [Fact]
    public void FromDecimal_WithValueExceedingMax_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Aliquota.FromDecimal(10m));

    // ============================================
    // Explicit cast — (Aliquota)decimal
    // ============================================

    [Fact]
    public void ExplicitCast_ProducesSameResultAsFromDecimal()
    {
        var via = Aliquota.FromDecimal(0.03m);
        var cast = (Aliquota)0.03m;
        Assert.Equal(via.ToString(), cast.ToString());
    }

    [Fact]
    public void ExplicitCast_WithNegativeValue_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => { var _ = (Aliquota)(-0.01m); });

    // ============================================
    // ToString / GetMaxLimit
    // ============================================

    [Fact]
    public void ToString_IntegerValue_HasNoDecimalSeparator() =>
        Assert.DoesNotContain(".", new Aliquota(1m).ToString());

    [Fact]
    public void ToString_FractionalValue_HasUpToFourDecimalPlaces()
    {
        string[] parts = new Aliquota(0.1234m).ToString()!.Split('.');
        Assert.Equal(2, parts.Length);
        Assert.True(parts[1].Length <= 4);
    }

    // ============================================
    // sealed / Equals / GetHashCode
    // ============================================

    [Fact]
    public void Aliquota_IsSealed() =>
        Assert.True(typeof(Aliquota).IsSealed);

    [Fact]
    public void Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new Aliquota(0.05m), new Aliquota(0.05m));

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new Aliquota(0.05m), new Aliquota(0.1m));

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash() =>
        Assert.Equal(new Aliquota(0.05m).GetHashCode(), new Aliquota(0.05m).GetHashCode());

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHash() =>
        Assert.NotEqual(new Aliquota(0.05m).GetHashCode(), new Aliquota(0.1m).GetHashCode());

    [Fact]
    public void Equals_NullObject_ReturnsFalse() =>
        Assert.False(new Aliquota(0.05m).Equals(null));

    [Fact]
    public void Equals_DifferentType_SameSerializedValue_ReturnsFalse()
    {
        // Aliquota(1m) e Valor(1m) serializam como "1", mas são tipos distintos
        object aliquota = new Aliquota(1m);
        object valor = new Valor(1m);
        Assert.False(aliquota.Equals(valor));
    }

    // ============================================
    // XML Serialização / Desserialização
    // ============================================

    [Theory]
    [InlineData(0.05)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(9.9999)]
    public void XmlSerialization_RoundTrip_PreservesValue(double rawValue)
    {
        var aliquota = new Aliquota((decimal)rawValue);
        var xml = XmlTestHelper.SerializarParaXml(aliquota);
        var desserializado = XmlTestHelper.DesserializarDeXml<Aliquota>(xml);
        Assert.Equal(aliquota.ToString(), desserializado!.ToString());
    }

    [Theory]
    [InlineData("0.0500")]
    [InlineData("1")]
    [InlineData("9.9999")]
    [InlineData("0")]
    public void XmlDeserialization_ValorValido_PreservaCadeia(string xmlValue)
    {
        var xml = $"""<?xml version="1.0" encoding="utf-16"?><Aliquota xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">{xmlValue}</Aliquota>""";
        var desserializado = XmlTestHelper.DesserializarDeXml<Aliquota>(xml);
        Assert.Equal(xmlValue, desserializado!.ToString());
    }

    [Fact]
    public void XmlDeserialization_ElementoVazio_ToStringRetornaNull()
    {
        var xml = """<?xml version="1.0" encoding="utf-16"?><Aliquota xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" />""";
        var desserializado = XmlTestHelper.DesserializarDeXml<Aliquota>(xml);
        Assert.Null(desserializado!.ToString());
    }

    // ============================================
    // FromDouble
    // ============================================

    [Fact]
    public void FromDouble_WithValidValue_ProducesSameResultAsFromDecimal() =>
        Assert.Equal(Aliquota.FromDecimal(0.05m).ToString(), Aliquota.FromDouble(0.05).ToString());

    [Fact]
    public void FromDouble_WithZero_SerializesAsZero() =>
        Assert.Equal("0", Aliquota.FromDouble(0.0).ToString());

    [Fact]
    public void FromDouble_WithNegativeValue_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Aliquota.FromDouble(-0.01));

    [Fact]
    public void FromDouble_WithValueExceedingMax_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Aliquota.FromDouble(10.0));

    [Fact]
    public void FromDouble_WithNaN_ThrowsOverflowException() =>
        Assert.Throws<OverflowException>(() => Aliquota.FromDouble(double.NaN));

    [Fact]
    public void FromDouble_WithPositiveInfinity_ThrowsOverflowException() =>
        Assert.Throws<OverflowException>(() => Aliquota.FromDouble(double.PositiveInfinity));

    [Fact]
    public void FromDouble_WithNegativeInfinity_ThrowsOverflowException() =>
        Assert.Throws<OverflowException>(() => Aliquota.FromDouble(double.NegativeInfinity));

    [Fact]
    public void ExplicitCastDouble_ValidValue_ProducesSameResultAsFromDouble()
    {
        var via = Aliquota.FromDouble(0.03);
        var cast = (Aliquota)0.03;
        Assert.Equal(via.ToString(), cast.ToString());
    }

    [Fact]
    public void ExplicitCastDouble_WithNegativeValue_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => { var _ = (Aliquota)(-0.01); });

    // ============================================
    // ParseIfPresent — double?
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNullDouble_ReturnsNull() =>
        Assert.Null(Aliquota.ParseIfPresent((double?)null));

    [Fact]
    public void ParseIfPresent_WithValidDouble_ReturnsAliquota() =>
        Assert.Equal("0.0500", Aliquota.ParseIfPresent((double?)0.05)!.ToString());

    [Fact]
    public void ParseIfPresent_WithNegativeDouble_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Aliquota.ParseIfPresent((double?)-0.01));

    [Fact]
    public void ParseIfPresent_WithValueExceedingMaxDouble_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Aliquota.ParseIfPresent((double?)10.0));

    [Fact]
    public void ParseIfPresent_WithNaNDouble_ThrowsOverflowException() =>
        Assert.Throws<OverflowException>(() => Aliquota.ParseIfPresent((double?)double.NaN));

    // ============================================
    // ParseIfPresent — decimal?
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNullDecimal_ReturnsNull() =>
        Assert.Null(Aliquota.ParseIfPresent((decimal?)null));

    [Fact]
    public void ParseIfPresent_WithValidDecimal_ReturnsAliquota() =>
        Assert.Equal("0.0500", Aliquota.ParseIfPresent((decimal?)0.05m)!.ToString());

    [Fact]
    public void ParseIfPresent_WithNegativeDecimal_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Aliquota.ParseIfPresent((decimal?)-0.01m));

    [Fact]
    public void ParseIfPresent_WithDecimalExceedingMax_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Aliquota.ParseIfPresent((decimal?)10m));
}