using Nfe.Paulistana.Models.DataTypes;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class ValorTests
{
    // ============================================
    // Valor() — Construtor padrão
    // ============================================

    [Fact]
    public void DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new Valor().ToString());

    // ============================================
    // Valor(decimal) — Construtor com valor
    // ============================================

    [Fact]
    public void Constructor_WithIntegerValue_SerializesWithoutDecimals() =>
        Assert.Equal("1000", new Valor(1000m).ToString());

    [Fact]
    public void Constructor_WithFractionalValue_SerializesWithTwoDecimalPlaces() =>
        Assert.Equal("1000.50", new Valor(1000.50m).ToString());

    [Fact]
    public void Constructor_WithZero_SerializesAsZero() =>
        Assert.Equal("0", new Valor(0m).ToString());

    [Fact]
    public void Constructor_WithOneCent_SerializesCorrectly() =>
        Assert.Equal("0.01", new Valor(0.01m).ToString());

    [Fact]
    public void Constructor_WithNegativeValue_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Valor(-0.01m));

    [Fact]
    public void Constructor_WithValueExceedingMax_ThrowsArgumentException() =>
        // 10^15 pode colidir com imprecisão de double em GetDecimalLimits()
        // Usamos 2x o máximo para garantir rejeição inequívoca
        Assert.Throws<ArgumentException>(() => new Valor(2_000_000_000_000_000m));

    [Theory]
    [InlineData(0)]
    [InlineData(100.50)]
    [InlineData(20500)]
    public void Constructor_WithValidValues_DoesNotThrow(double rawValue)
    {
        Exception ex = Record.Exception(() => new Valor((decimal)rawValue));
        Assert.Null(ex);
    }

    // ============================================
    // FromDecimal — Factory method
    // ============================================

    [Fact]
    public void FromDecimal_ProducesSameResultAsConstructor() =>
        Assert.Equal(new Valor(500.75m).ToString(), Valor.FromDecimal(500.75m).ToString());

    [Fact]
    public void FromDecimal_WithNegativeValue_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Valor.FromDecimal(-1m));

    // ============================================
    // FromValor — Conversão para decimal
    // ============================================

    [Fact]
    public void FromValor_WithValidValor_ReturnsCorrectDecimal()
    {
        var valor = new Valor(1500.25m);
        Assert.Equal(1500.25m, Valor.FromValor(valor));
    }

    [Fact]
    public void FromValor_WithNull_ReturnsDecimalZero() =>
        Assert.Equal(decimal.Zero, Valor.FromValor(null));

    [Fact]
    public void FromValor_WithDefaultConstructor_ReturnsDecimalZero() =>
        Assert.Equal(decimal.Zero, Valor.FromValor(new Valor()));

    [Fact]
    public void FromValor_WithZero_ReturnsZero() =>
        Assert.Equal(0m, Valor.FromValor(new Valor(0m)));

    // ============================================
    // Explicit cast — (Valor)decimal
    // ============================================

    [Fact]
    public void ExplicitCast_ProducesSameResultAsFromDecimal()
    {
        var via = Valor.FromDecimal(750m);
        var cast = (Valor)750m;
        Assert.Equal(via.ToString(), cast.ToString());
    }

    [Fact]
    public void ExplicitCast_WithNegativeValue_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => { var _ = (Valor)(-1m); });

    // ============================================
    // Implicit cast — decimal = Valor?
    // ============================================

    [Fact]
    public void ImplicitCast_ValidValor_ReturnsCorrectDecimal()
    {
        Valor valor = new(250.99m);
        decimal result = valor;
        Assert.Equal(250.99m, result);
    }

    [Fact]
    public void ImplicitCast_NullValor_ReturnsDecimalZero()
    {
        Valor? valor = null;
        decimal result = valor;
        Assert.Equal(decimal.Zero, result);
    }

    // ============================================
    // Round-trip — decimal → Valor → decimal
    // ============================================

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100.50)]
    [InlineData(9999.99)]
    public void RoundTrip_DecimalToValorAndBack_PreservesValue(double rawValue)
    {
        decimal original = (decimal)rawValue;
        Valor valor = new(original);
        decimal restored = valor;
        Assert.Equal(original, restored);
    }

    [Fact]
    public void RoundTrip_IntegerValue_PreservesValue()
    {
        const decimal original = 5000m;
        decimal restored = new Valor(original);
        Assert.Equal(original, restored);
    }

    // ============================================
    // sealed / Equals / GetHashCode
    // ============================================

    [Fact]
    public void Valor_IsSealed() =>
        Assert.True(typeof(Valor).IsSealed);

    [Fact]
    public void Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new Valor(100m), new Valor(100m));

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new Valor(100m), new Valor(200m));

    [Fact]
    public void Equals_NullObject_ReturnsFalse() =>
        Assert.False(new Valor(100m).Equals(null));

    [Fact]
    public void Equals_DifferentType_SameSerializedValue_ReturnsFalse()
    {
        // Valor(1m) e Aliquota(1m) serializam como "1", mas são tipos distintos
        object valor = new Valor(1m);
        object Aliquota = new Aliquota(1m);
        Assert.False(valor.Equals(Aliquota));
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash() =>
        Assert.Equal(new Valor(100m).GetHashCode(), new Valor(100m).GetHashCode());

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHash() =>
        Assert.NotEqual(new Valor(100m).GetHashCode(), new Valor(200m).GetHashCode());

    // ============================================
    // XML Serialização / Desserialização
    // ============================================

    private static string SerializarParaXml(Valor valor)
    {
        var serializer = new XmlSerializer(typeof(Valor));
        using var writer = new StringWriter();
        serializer.Serialize(writer, valor);
        return writer.ToString();
    }

    private static Valor? DesserializarDeXml(string xml)
    {
        var serializer = new XmlSerializer(typeof(Valor));
        using var reader = new StringReader(xml);
        return (Valor?)serializer.Deserialize(reader);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100.50)]
    [InlineData(1000)]
    public void XmlSerialization_RoundTrip_PreservesValue(double rawValue)
    {
        var valor = new Valor((decimal)rawValue);
        var xml = SerializarParaXml(valor);
        var desserializado = DesserializarDeXml(xml);
        Assert.Equal(valor.ToString(), desserializado!.ToString());
    }

    [Theory]
    [InlineData("1000")]
    [InlineData("1000.50")]
    [InlineData("0")]
    [InlineData("0.01")]
    public void XmlDeserialization_ValorValido_PreservaCadeia(string xmlValue)
    {
        var xml = $"""<?xml version="1.0" encoding="utf-16"?><Valor xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">{xmlValue}</Valor>""";
        var desserializado = DesserializarDeXml(xml);
        Assert.Equal(xmlValue, desserializado!.ToString());
    }

    [Fact]
    public void XmlDeserialization_ElementoVazio_ToStringRetornaNull()
    {
        var xml = """<?xml version="1.0" encoding="utf-16"?><Valor xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" />""";
        var desserializado = DesserializarDeXml(xml);
        Assert.Null(desserializado!.ToString());
    }

    [Fact]
    public void XmlDeserialization_ElementoVazio_FromValorRetornaZero()
    {
        var xml = """<?xml version="1.0" encoding="utf-16"?><Valor xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" />""";
        var desserializado = DesserializarDeXml(xml);
        Assert.Equal(decimal.Zero, Valor.FromValor(desserializado));
    }
}