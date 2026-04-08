using Nfe.Paulistana.V2.Models.Response;

namespace Nfe.Paulistana.Tests.V2.Models.Response;

/// <summary>
/// Testes unitários para <see cref="ChaveRps"/>.
/// </summary>
public sealed class ChaveRpsTests
{
    private static string SerializarParaXml(ChaveRps obj)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ChaveRps));
        using var sw = new System.IO.StringWriter();
        serializer.Serialize(sw, obj);
        return sw.ToString();
    }

    private static ChaveRps? DesserializarDeXml(string xml)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ChaveRps));
        using var sr = new System.IO.StringReader(xml);
        return (ChaveRps?)serializer.Deserialize(sr);
    }

    [Fact]
    public void Constructor_SemArgumentos_CriaInstancia() =>
        Assert.NotNull(new ChaveRps());

    [Fact]
    public void Propriedades_QuandoDefinidas_RetornamMesmosValores()
    {
        var chave = new ChaveRps
        {
            InscricaoPrestador = "1234567",
            SerieRps = "A",
            NumeroRps = "1"
        };

        Assert.Equal("1234567", chave.InscricaoPrestador);
        Assert.Equal("A", chave.SerieRps);
        Assert.Equal("1", chave.NumeroRps);
    }

    [Fact]
    public void Propriedades_QuandoNaoDefinidas_RetornamNull()
    {
        var chave = new ChaveRps();

        Assert.Null(chave.InscricaoPrestador);
        Assert.Null(chave.SerieRps);
        Assert.Null(chave.NumeroRps);
    }

    [Fact]
    public void XmlSerialization_RoundTrip_PreservaPropriedades()
    {
        // Arrange
        var chave = new ChaveRps
        {
            InscricaoPrestador = "1234567",
            SerieRps = "A",
            NumeroRps = "1"
        };

        // Act
        var desserializado = DesserializarDeXml(SerializarParaXml(chave))!;

        // Assert
        Assert.Equal(chave.InscricaoPrestador, desserializado.InscricaoPrestador);
        Assert.Equal(chave.SerieRps, desserializado.SerieRps);
        Assert.Equal(chave.NumeroRps, desserializado.NumeroRps);
    }

    [Fact]
    public void XmlSerialization_SerieRpsENumeroRpsUsamElementosEmMaiusculas()
    {
        // Arrange
        var chave = new ChaveRps { SerieRps = "A", NumeroRps = "1" };

        // Act
        var xml = SerializarParaXml(chave);

        // Assert
        Assert.Contains("<SerieRPS>A</SerieRPS>", xml);
        Assert.Contains("<NumeroRPS>1</NumeroRPS>", xml);
    }
}
