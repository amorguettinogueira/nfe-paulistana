using Nfe.Paulistana.Models.DataTypes;
using System.Xml.Serialization;

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

    // ============================================
    // XML Serialização / Desserialização
    // ============================================

    private static string SerializarParaXml(Aliquota aliquota)
    {
        var serializer = new XmlSerializer(typeof(Aliquota));
        using var writer = new StringWriter();
        serializer.Serialize(writer, aliquota);
        return writer.ToString();
    }

    private static Aliquota? DesserializarDeXml(string xml)
    {
        var serializer = new XmlSerializer(typeof(Aliquota));
        using var reader = new StringReader(xml);
        return (Aliquota?)serializer.Deserialize(reader);
    }

    [Theory]
    [InlineData(0.05)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(9.9999)]
    public void XmlSerialization_RoundTrip_PreservesValue(double rawValue)
    {
        var aliquota = new Aliquota((decimal)rawValue);
        var xml = SerializarParaXml(aliquota);
        var desserializado = DesserializarDeXml(xml);
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
        var desserializado = DesserializarDeXml(xml);
        Assert.Equal(xmlValue, desserializado!.ToString());
    }

    [Fact]
    public void XmlDeserialization_ElementoVazio_ToStringRetornaNull()
    {
        var xml = """<?xml version="1.0" encoding="utf-16"?><Aliquota xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" />""";
        var desserializado = DesserializarDeXml(xml);
        Assert.Null(desserializado!.ToString());
    }
}