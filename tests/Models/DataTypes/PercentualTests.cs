using Nfe.Paulistana.Models.DataTypes;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.Models.DataTypes;

public class PercentualTests
{
    // ============================================
    // Percentual() — Construtor padrão
    // ============================================

    [Fact]
    public void DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new Percentual().ToString());

    // ============================================
    // Percentual(decimal) — Construtor com valor
    // ============================================

    [Fact]
    public void Constructor_WithFiftyPercent_SerializesAsInteger() =>
        // ConstrainedDecimal: value % 1 == 0 → formato "F0" (sem casas decimais)
        Assert.Equal("50", new Percentual(50m).ToString());

    [Fact]
    public void Constructor_WithZero_SerializesAsZero() =>
        Assert.Equal("0", new Percentual(0m).ToString());

    [Fact]
    public void Constructor_WithHundredPercent_Accepted()
    {
        // ConstrainedDecimal: value % 1 == 0 → formato "F0" (sem casas decimais)
        var percentual = new Percentual(100m);
        Assert.Equal("100", percentual.ToString());
    }

    [Fact]
    public void Constructor_WithFractionalPercent_SerializesWithFourDecimalPlaces() =>
        Assert.Equal("15.5000", new Percentual(15.5m).ToString());

    [Fact]
    public void Constructor_WithMaxFractionalPrecision_SerializesCorrectly() =>
        Assert.Equal("33.3333", new Percentual(33.3333m).ToString());

    [Fact]
    public void Constructor_WithNegativeValue_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Percentual(-0.01m));

    [Fact]
    public void Constructor_WithValueAbove100_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Percentual(100.0001m));

    [Fact]
    public void Constructor_WithFarAbove100_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Percentual(150m));

    [Theory]
    [InlineData(0)]
    [InlineData(0.5)]
    [InlineData(50)]
    [InlineData(99.9999)]
    [InlineData(100)]
    public void Constructor_WithValidValues_DoesNotThrow(double rawValue)
    {
        Exception ex = Record.Exception(() => new Percentual((decimal)rawValue));
        Assert.Null(ex);
    }

    // ============================================
    // FromDecimal — Factory method
    // ============================================

    [Fact]
    public void FromDecimal_ProducesSameResultAsConstructor() =>
        Assert.Equal(new Percentual(75m).ToString(), Percentual.FromDecimal(75m).ToString());

    [Fact]
    public void FromDecimal_WithNegativeValue_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Percentual.FromDecimal(-1m));

    [Fact]
    public void FromDecimal_WithValueAbove100_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Percentual.FromDecimal(100.0001m));

    // ============================================
    // Explicit cast — (Percentual)decimal
    // ============================================

    [Fact]
    public void ExplicitCast_ProducesSameResultAsFromDecimal()
    {
        var via = Percentual.FromDecimal(25m);
        var cast = (Percentual)25m;
        Assert.Equal(via.ToString(), cast.ToString());
    }

    [Fact]
    public void ExplicitCast_WithValueAbove100_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => { var _ = (Percentual)101m; });

    // ============================================
    // Boundary — limites exatos
    // ============================================

    [Fact]
    public void Boundary_Zero_IsAccepted()
    {
        Exception ex = Record.Exception(() => new Percentual(0m));
        Assert.Null(ex);
    }

    [Fact]
    public void Boundary_ExactlyOneHundred_IsAccepted()
    {
        Exception ex = Record.Exception(() => new Percentual(100m));
        Assert.Null(ex);
    }

    [Fact]
    public void Boundary_JustAboveOneHundred_IsRejected() =>
        Assert.Throws<ArgumentException>(() => new Percentual(100.0001m));

    // ============================================
    // ToString / GetMaxLimit
    // ============================================

    [Fact]
    public void ToString_IntegerPercent_HasNoDecimalSeparator() =>
        // base ConstrainedDecimal: sem parte fracionária → sem "."
        Assert.DoesNotContain(".", new Percentual(0m).ToString());

    [Fact]
    public void ToString_FractionalPercent_HasUpToFourDecimalPlaces()
    {
        string[] parts = new Percentual(33.3333m).ToString()!.Split('.');
        Assert.Equal(2, parts.Length);
        Assert.True(parts[1].Length <= 4);
    }

    // ============================================
    // sealed / Equals / GetHashCode
    // ============================================

    [Fact]
    public void Percentual_IsSealed() =>
        Assert.True(typeof(Percentual).IsSealed);

    [Fact]
    public void Equals_SameValue_ReturnsTrue() =>
        Assert.Equal(new Percentual(50m), new Percentual(50m));

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse() =>
        Assert.NotEqual(new Percentual(50m), new Percentual(75m));

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash() =>
        Assert.Equal(new Percentual(50m).GetHashCode(), new Percentual(50m).GetHashCode());

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHash() =>
        Assert.NotEqual(new Percentual(50m).GetHashCode(), new Percentual(75m).GetHashCode());

    [Fact]
    public void Equals_NullObject_ReturnsFalse() =>
        Assert.False(new Percentual(50m).Equals(null));

    [Fact]
    public void Equals_DifferentType_SameSerializedValue_ReturnsFalse()
    {
        // Percentual(1m) e Aliquota(1m) serializam como "1", mas são tipos distintos
        object percentual = new Percentual(1m);
        object aliquota = new Aliquota(1m);
        Assert.False(percentual.Equals(aliquota));
    }

    // ============================================
    // Validação antes do base() — Value nunca é setado com dado inválido
    // ============================================

    [Fact]
    public void Constructor_WithValueAbove100_ValueNeverSet()
    {
        // Com a correção (ValidateRange antes de base()), o construtor lança
        // ArgumentException antes de qualquer atribuição a Value.
        // O objeto parcialmente iniciado nunca é acessível pelo chamador.
        ArgumentException ex = Assert.Throws<ArgumentException>(() => new Percentual(101m));
        Assert.NotNull(ex.Message);
    }

    // ============================================
    // FromDouble
    // ============================================

    [Fact]
    public void FromDouble_WithValidValue_ProducesSameResultAsFromDecimal() =>
        Assert.Equal(new Percentual(75m).ToString(), Percentual.FromDouble(75.0).ToString());

    [Fact]
    public void FromDouble_WithZero_SerializesAsZero() =>
        Assert.Equal("0", Percentual.FromDouble(0.0).ToString());

    [Fact]
    public void FromDouble_WithNegativeValue_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Percentual.FromDouble(-1.0));

    [Fact]
    public void FromDouble_WithValueAbove100_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Percentual.FromDouble(101.0));

    [Fact]
    public void FromDouble_WithNaN_ThrowsOverflowException() =>
        Assert.Throws<OverflowException>(() => Percentual.FromDouble(double.NaN));

    [Fact]
    public void FromDouble_WithPositiveInfinity_ThrowsOverflowException() =>
        Assert.Throws<OverflowException>(() => Percentual.FromDouble(double.PositiveInfinity));

    [Fact]
    public void FromDouble_WithNegativeInfinity_ThrowsOverflowException() =>
        Assert.Throws<OverflowException>(() => Percentual.FromDouble(double.NegativeInfinity));

    // ============================================
    // Explicit cast — (Percentual)double
    // ============================================

    [Fact]
    public void ExplicitCastDouble_ValidValue_ProducesSameResultAsFromDouble()
    {
        var via = Percentual.FromDouble(25.0);
        var cast = (Percentual)25.0;
        Assert.Equal(via.ToString(), cast.ToString());
    }

    [Fact]
    public void ExplicitCastDouble_WithValueAbove100_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => { var _ = (Percentual)101.0; });

    [Fact]
    public void ExplicitCastDouble_WithNegativeValue_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => { var _ = (Percentual)(-1.0); });

    // ============================================
    // XML Serialização / Desserialização
    // ============================================

    private static string SerializarParaXml(Percentual percentual)
    {
        var serializer = new XmlSerializer(typeof(Percentual));
        using var writer = new StringWriter();
        serializer.Serialize(writer, percentual);
        return writer.ToString();
    }

    private static Percentual? DesserializarDeXml(string xml)
    {
        var serializer = new XmlSerializer(typeof(Percentual));
        using var reader = new StringReader(xml);
        return (Percentual?)serializer.Deserialize(reader);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(33.3333)]
    public void XmlSerialization_RoundTrip_PreservesValue(double rawValue)
    {
        var percentual = new Percentual((decimal)rawValue);
        var xml = SerializarParaXml(percentual);
        var desserializado = DesserializarDeXml(xml);
        Assert.Equal(percentual.ToString(), desserializado!.ToString());
    }

    [Theory]
    [InlineData("50")]
    [InlineData("33.3333")]
    [InlineData("0")]
    [InlineData("100")]
    public void XmlDeserialization_ValorValido_PreservaCadeia(string xmlValue)
    {
        var xml = $"""<?xml version="1.0" encoding="utf-16"?><Percentual xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">{xmlValue}</Percentual>""";
        var desserializado = DesserializarDeXml(xml);
        Assert.Equal(xmlValue, desserializado!.ToString());
    }

    [Fact]
    public void XmlDeserialization_ElementoVazio_ToStringRetornaNull()
    {
        var xml = """<?xml version="1.0" encoding="utf-16"?><Percentual xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" />""";
        var desserializado = DesserializarDeXml(xml);
        Assert.Null(desserializado!.ToString());
    }

    // ============================================
    // ParseIfPresent — double?
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNullDouble_ReturnsNull() =>
        Assert.Null(Percentual.ParseIfPresent((double?)null));

    [Fact]
    public void ParseIfPresent_WithValidDouble_ReturnsPercentual() =>
        Assert.Equal("50", Percentual.ParseIfPresent((double?)50.0)!.ToString());

    [Fact]
    public void ParseIfPresent_WithNegativeDouble_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Percentual.ParseIfPresent((double?)-1.0));

    [Fact]
    public void ParseIfPresent_WithValueAbove100Double_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Percentual.ParseIfPresent((double?)101.0));

    [Fact]
    public void ParseIfPresent_WithNaNDouble_ThrowsOverflowException() =>
        Assert.Throws<OverflowException>(() => Percentual.ParseIfPresent((double?)double.NaN));

    // ============================================
    // ParseIfPresent — decimal?
    // ============================================

    [Fact]
    public void ParseIfPresent_WithNullDecimal_ReturnsNull() =>
        Assert.Null(Percentual.ParseIfPresent((decimal?)null));

    [Fact]
    public void ParseIfPresent_WithValidDecimal_ReturnsPercentual() =>
        Assert.Equal("75", Percentual.ParseIfPresent((decimal?)75m)!.ToString());

    [Fact]
    public void ParseIfPresent_WithNegativeDecimal_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Percentual.ParseIfPresent((decimal?)-1m));

    [Fact]
    public void ParseIfPresent_WithDecimalAbove100_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => Percentual.ParseIfPresent((decimal?)100.0001m));
}