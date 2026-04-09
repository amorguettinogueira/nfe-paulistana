using Nfe.Paulistana.V2.Models.DataTypes;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="CodigoPaisISO"/>.
/// </summary>
public sealed class CodigoPaisISOTests
{
    [Fact]
    public void CodigoPaisISO_DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new CodigoPaisISO().ToString());

    [Theory]
    [InlineData("BR")]
    [InlineData("US")]
    [InlineData("FR")]
    public void CodigoPaisISO_ValidValue_ShouldSetValue(string value)
    {
        var cod = new CodigoPaisISO(value);
        Assert.Equal(value, cod.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("B")]
    [InlineData("BRA")]
    [InlineData("br")]
    [InlineData("1A")]
    public void CodigoPaisISO_InvalidValue_ShouldThrowArgumentException(string? value)
    {
        Action act = () => _ = new CodigoPaisISO(value!);
        Assert.ThrowsAny<ArgumentException>(act);
    }

    // ============================================
    // FromString e operador explícito
    // ============================================

    [Fact]
    public void FromString_ValorValido_CriaInstanciaCorreta()
    {
        var cod = CodigoPaisISO.FromString("BR");

        Assert.Equal("BR", cod.ToString());
    }

    [Fact]
    public void ExplicitCast_ValorValido_CriaInstanciaCorreta()
    {
        var cod = (CodigoPaisISO)"US";

        Assert.Equal("US", cod.ToString());
    }

    // ============================================
    // IsSealed
    // ============================================

    [Fact]
    public void CodigoPaisISO_DeveSerSealed() =>
        Assert.True(typeof(CodigoPaisISO).IsSealed);

    // ============================================
    // Equals e GetHashCode
    // ============================================

    [Fact]
    public void Equals_MesmoValor_SaoIguais()
    {
        var a = new CodigoPaisISO("BR");
        var b = new CodigoPaisISO("BR");

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_ValoresDiferentes_NaoSaoIguais()
    {
        var a = new CodigoPaisISO("BR");
        var b = new CodigoPaisISO("US");

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_Nulo_RetornaFalse()
    {
        var cod = new CodigoPaisISO("BR");

        Assert.False(cod.Equals(null));
    }

    [Fact]
    public void GetHashCode_MesmoValor_MesmoHash()
    {
        var a = new CodigoPaisISO("BR");
        var b = new CodigoPaisISO("BR");

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ValoresDiferentes_HashDiferente()
    {
        var a = new CodigoPaisISO("BR");
        var b = new CodigoPaisISO("US");

        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    // ============================================
    // Serialização XML (round-trip)
    // ============================================

    private static CodigoPaisISO? DeserializarDeXml(string xml)
    {
        var serializer = new XmlSerializer(typeof(CodigoPaisISO));
        using var sr = new StringReader(xml);
        return (CodigoPaisISO?)serializer.Deserialize(sr);
    }

    private static string SerializarParaXml(CodigoPaisISO cod)
    {
        var serializer = new XmlSerializer(typeof(CodigoPaisISO));
        using var sw = new StringWriter();
        serializer.Serialize(sw, cod);
        return sw.ToString();
    }

    [Fact]
    public void XmlRoundTrip_ValorValido_DeserializaCorretamente()
    {
        var original = new CodigoPaisISO("BR");
        string xml = SerializarParaXml(original);

        var deserialized = DeserializarDeXml(xml);

        Assert.Equal("BR", deserialized?.ToString());
    }

    [Fact]
    public void XmlDeserialization_ValorInvalido_LancaInvalidOperationException()
    {
        const string xml = "<CodigoPaisISO>br</CodigoPaisISO>";
        var serializer = new XmlSerializer(typeof(CodigoPaisISO));
        using var sr = new StringReader(xml);

        var ex = Assert.Throws<InvalidOperationException>(() => serializer.Deserialize(sr));

        Assert.IsType<SerializationException>(ex.InnerException);
    }
}