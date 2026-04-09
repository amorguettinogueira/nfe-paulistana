using Nfe.Paulistana.Tests.V2.Helpers;
using Nfe.Paulistana.V2.Models.DataTypes;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="Cnpj"/> (V2 — alfanumérico, padrão [0-9A-Z]{12}[0-9]{2}).
/// </summary>
public class CnpjTests
{
    private static Cnpj? DeserializarDeXml(string xml)
    {
        var serializer = new XmlSerializer(typeof(Cnpj));
        using var sr = new StringReader(xml);
        return (Cnpj?)serializer.Deserialize(sr);
    }

    private static string SerializarParaXml(Cnpj cnpj)
    {
        var serializer = new XmlSerializer(typeof(Cnpj));
        using var sw = new StringWriter();
        serializer.Serialize(sw, cnpj);
        return sw.ToString();
    }

    // ============================================
    // Construtor padrão (desserialização)
    // ============================================

    [Fact]
    public void DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new Cnpj().ToString());

    // ============================================
    // Construtor — happy path
    // ============================================

    [Theory]
    [ClassData(typeof(ValidCnpjStrings))]
    public void Constructor_FromFormattedString_ArmazenaUnformatted(string formatted, string unformatted)
    {
        var cnpj = new Cnpj(formatted);

        Assert.Equal(unformatted, cnpj.ToString());
    }

    [Theory]
    [ClassData(typeof(ValidCnpjStrings))]
    public void Constructor_FromUnformattedString_ArmazenaCorretamente(string _, string unformatted)
    {
        var cnpj = new Cnpj(unformatted);

        Assert.Equal(unformatted, cnpj.ToString());
    }

    // ============================================
    // Construtor — erros de validação
    // ============================================

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_NuloOuVazio_LancaArgumentException(string? value) =>
        Assert.ThrowsAny<ArgumentException>(() => new Cnpj(value!));

    [Fact]
    public void Constructor_ComLetrasMinusculas_NormalizaEValida()
    {
        // lowercase é normalizado para uppercase antes de validar
        var cnpj = new Cnpj("bx.5s4.x0c/0001-76");

        Assert.Equal("BX5S4X0C000176", cnpj.ToString());
    }

    [Fact]
    public void Constructor_ComDigitoVerificadorInvalido_LancaArgumentException()
    {
        // "XA412263000171" — último dígito alterado de 0→1
        var ex = Assert.Throws<ArgumentException>(() => new Cnpj("XA412263000171"));
        Assert.Contains("módulo 11", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_MenosDe14Caracteres_LancaArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Cnpj("XA41226300017"));

    [Fact]
    public void Constructor_MaisDe14Caracteres_LancaArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Cnpj("XA4122630001700"));

    [Fact]
    public void Constructor_UltimoDoisDigitosNaoNumericos_LancaArgumentException() =>
        Assert.Throws<ArgumentException>(() => new Cnpj("XA412263000A70"));

    // ============================================
    // Factory method e operador explícito
    // ============================================

    [Fact]
    public void FromString_ProducesMesmoResultadoQueConstructor()
    {
        Assert.Equal(new Cnpj("XA412263000170").ToString(), Cnpj.FromString("XA412263000170").ToString());
    }

    [Fact]
    public void ExplicitOperator_ProducesMesmoResultadoQueFromString()
    {
        Assert.Equal(Cnpj.FromString("XA412263000170").ToString(), ((Cnpj)"XA412263000170").ToString());
    }

    // ============================================
    // ParseIfPresent(string?)
    // ============================================

    [Fact]
    public void ParseIfPresent_ComNulo_RetornaNull() =>
        Assert.Null(Cnpj.ParseIfPresent(null));

    [Fact]
    public void ParseIfPresent_ComEspacoBranco_RetornaNull() =>
        Assert.Null(Cnpj.ParseIfPresent("   "));

    [Fact]
    public void ParseIfPresent_ComValorValido_RetornaInstancia() =>
        Assert.Equal(new Cnpj("XA412263000170"), Cnpj.ParseIfPresent("XA412263000170"));

    [Fact]
    public void ParseIfPresent_ComValorInvalido_LancaArgumentException() =>
        Assert.Throws<ArgumentException>(() => Cnpj.ParseIfPresent("XA412263000171"));

    // ============================================
    // Sealed / Equals / GetHashCode
    // ============================================

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(Cnpj).IsSealed);

    [Theory]
    [ClassData(typeof(ValidCnpjStrings))]
    public void Equals_MesmoValor_RetornaTrue(string _, string unformatted) =>
        Assert.Equal(new Cnpj(unformatted), new Cnpj(unformatted));

    [Fact]
    public void Equals_ValorDiferente_RetornaFalse() =>
        Assert.NotEqual(new Cnpj("XA412263000170"), new Cnpj("CP34NHRC000108"));

    [Fact]
    public void Equals_ObjetoNulo_RetornaFalse() =>
        Assert.False(new Cnpj("XA412263000170").Equals(null));

    [Theory]
    [ClassData(typeof(ValidCnpjStrings))]
    public void GetHashCode_MesmoValor_RetornaMesmoHash(string _, string unformatted) =>
        Assert.Equal(new Cnpj(unformatted).GetHashCode(), new Cnpj(unformatted).GetHashCode());

    [Fact]
    public void GetHashCode_ValorDiferente_RetornaHashDiferente() =>
        Assert.NotEqual(new Cnpj("XA412263000170").GetHashCode(), new Cnpj("CP34NHRC000108").GetHashCode());

    // ============================================
    // Imutabilidade
    // ============================================

    [Fact]
    public void ValueProperty_SemSetterPublico()
    {
        var valueProperty = typeof(Cnpj).GetProperty("Value",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        var publicSetter = valueProperty?.GetSetMethod(nonPublic: false);

        Assert.True(valueProperty == null || publicSetter == null);
    }

    // ============================================
    // Serialização XML — round-trip
    // ============================================

    [Theory]
    [ClassData(typeof(ValidCnpjStrings))]
    public void XmlRoundTrip_PreservesValue(string _, string unformatted)
    {
        var original = new Cnpj(unformatted);

        var xml = SerializarParaXml(original);
        var deserialized = DeserializarDeXml(xml);

        Assert.Equal(unformatted, deserialized?.ToString());
    }

    [Fact]
    public void XmlDeserialization_DigitosVerificadoresInvalidos_LancaSerializationException()
    {
        const string xml = """<?xml version="1.0" encoding="utf-16"?><Cnpj xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">XA412263000171</Cnpj>""";

        var ex = Assert.Throws<InvalidOperationException>(() => DeserializarDeXml(xml));
        Assert.IsType<SerializationException>(ex.InnerException);
    }

    [Fact]
    public void XmlDeserialization_FormatoInvalido_LancaSerializationException()
    {
        const string xml = """<?xml version="1.0" encoding="utf-16"?><Cnpj xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">ABC/XYZ-XXXX</Cnpj>""";

        var ex = Assert.Throws<InvalidOperationException>(() => DeserializarDeXml(xml));
        Assert.IsType<SerializationException>(ex.InnerException);
    }

    [Fact]
    public void XmlDeserialization_ValorVazio_LancaSerializationException()
    {
        const string xml = """<?xml version="1.0" encoding="utf-16"?><Cnpj xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"></Cnpj>""";

        var ex = Assert.Throws<InvalidOperationException>(() => DeserializarDeXml(xml));
        Assert.IsType<SerializationException>(ex.InnerException);
    }
}
