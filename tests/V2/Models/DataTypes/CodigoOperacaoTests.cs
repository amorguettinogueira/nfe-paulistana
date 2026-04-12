using Nfe.Paulistana.V2.Models.DataTypes;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Nfe.Paulistana.Tests.V2.Models.DataTypes;

/// <summary>
/// Testes unitários para <see cref="CodigoOperacao"/>.
/// </summary>
public sealed class CodigoOperacaoTests
{
    // ============================================
    // Construtor padrão
    // ============================================

    [Fact]
    public void DefaultConstructor_ToStringReturnsNull() =>
        Assert.Null(new CodigoOperacao().ToString());

    // ============================================
    // Construtor com valor válido
    // ============================================

    [Theory]
    [InlineData("010101")]
    [InlineData("000000")]
    [InlineData("999999")]
    public void Constructor_ValorValido_DefineValor(string value)
    {
        var codigo = new CodigoOperacao(value);

        Assert.Equal(value, codigo.ToString());
    }

    // ============================================
    // Construtor com valor inválido
    // ============================================

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("12345")]       // 5 chars — muito curto
    [InlineData("1234567")]     // 7 chars — muito longo
    [InlineData("ABCDEF")]      // não numérico
    [InlineData("01010!")]      // caractere especial
    public void Constructor_ValorInvalido_ThrowsArgumentException(string? value) =>
        Assert.ThrowsAny<ArgumentException>(() => new CodigoOperacao(value!));

    // ============================================
    // Fábrica estática
    // ============================================

    [Fact]
    public void FromString_ValorValido_RetornaInstancia()
    {
        var codigo = CodigoOperacao.FromString("010101");

        Assert.Equal("010101", codigo.ToString());
    }

    // ============================================
    // Operador explícito
    // ============================================

    [Fact]
    public void ExplicitOperator_ValorValido_RetornaInstancia()
    {
        var codigo = (CodigoOperacao)"010101";

        Assert.Equal("010101", codigo.ToString());
    }

    // ============================================
    // IsSealed
    // ============================================

    [Fact]
    public void IsSealed() =>
        Assert.True(typeof(CodigoOperacao).IsSealed);

    // ============================================
    // Equals / GetHashCode
    // ============================================

    [Fact]
    public void Equals_MesmoValor_SaoIguais()
    {
        var a = new CodigoOperacao("010101");
        var b = new CodigoOperacao("010101");

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_ValoresDiferentes_NaoSaoIguais()
    {
        var a = new CodigoOperacao("010101");
        var b = new CodigoOperacao("020202");

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_Nulo_RetornaFalse()
    {
        var codigo = new CodigoOperacao("010101");

        Assert.False(codigo.Equals(null));
    }

    [Fact]
    public void GetHashCode_MesmoValor_MesmoHash()
    {
        var a = new CodigoOperacao("010101");
        var b = new CodigoOperacao("010101");

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ValoresDiferentes_HashDiferente()
    {
        var a = new CodigoOperacao("010101");
        var b = new CodigoOperacao("020202");

        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    // ============================================
    // Serialização XML (round-trip)
    // ============================================

    private static CodigoOperacao? DeserializarDeXml(string xml)
    {
        var serializer = new XmlSerializer(typeof(CodigoOperacao));
        using var sr = new StringReader(xml);
        return (CodigoOperacao?)serializer.Deserialize(sr);
    }

    private static string SerializarParaXml(CodigoOperacao codigo)
    {
        var serializer = new XmlSerializer(typeof(CodigoOperacao));
        using var sw = new StringWriter();
        serializer.Serialize(sw, codigo);
        return sw.ToString();
    }

    [Fact]
    public void XmlRoundTrip_ValorValido_DeserializaCorretamente()
    {
        var original = new CodigoOperacao("010101");
        string xml = SerializarParaXml(original);

        var deserialized = DeserializarDeXml(xml);

        Assert.Equal("010101", deserialized?.ToString());
    }

    [Fact]
    public void XmlDeserialization_ValorInvalido_LancaInvalidOperationException()
    {
        const string xml = "<CodigoOperacao>ABCDEF</CodigoOperacao>";
        var serializer = new XmlSerializer(typeof(CodigoOperacao));
        using var sr = new StringReader(xml);

        var ex = Assert.Throws<InvalidOperationException>(() => serializer.Deserialize(sr));

        Assert.IsType<SerializationException>(ex.InnerException);
    }
}
