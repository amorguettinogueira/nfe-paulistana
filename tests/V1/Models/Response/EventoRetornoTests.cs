using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.Tests.V1.Models.Response;

/// <summary>
/// Testes unitários para <see cref="EventoRetorno"/>.
/// </summary>
public sealed class EventoRetornoTests
{
    private static string SerializarParaXml(EventoRetorno obj)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(EventoRetorno));
        using var sw = new System.IO.StringWriter();
        serializer.Serialize(sw, obj);
        return sw.ToString();
    }

    private static EventoRetorno? DesserializarDeXml(string xml)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(EventoRetorno));
        using var sr = new System.IO.StringReader(xml);
        return (EventoRetorno?)serializer.Deserialize(sr);
    }

    [Fact]
    public void Constructor_SemArgumentos_CriaInstancia() =>
        Assert.NotNull(new EventoRetorno());

    [Fact]
    public void Codigo_QuandoDefinido_RetornaMesmoValor()
    {
        var evento = new EventoRetorno { Codigo = 101 };

        Assert.Equal((short)101, evento.Codigo);
    }

    [Fact]
    public void Descricao_QuandoDefinida_RetornaMesmoValor()
    {
        var evento = new EventoRetorno { Descricao = "Alerta de teste" };

        Assert.Equal("Alerta de teste", evento.Descricao);
    }

    [Fact]
    public void ChaveRPS_QuandoDefinido_RetornaMesmaReferencia()
    {
        var chaveRps = new ChaveRps { NumeroRps = "1" };
        var evento = new EventoRetorno { ChaveRPS = chaveRps };

        Assert.Same(chaveRps, evento.ChaveRPS);
    }

    [Fact]
    public void ChaveNFe_QuandoDefinida_RetornaMesmaReferencia()
    {
        var chaveNFe = new ChaveNFe { NumeroNFe = "0000001" };
        var evento = new EventoRetorno { ChaveNFe = chaveNFe };

        Assert.Same(chaveNFe, evento.ChaveNFe);
    }

    [Fact]
    public void Propriedades_QuandoNaoDefinidas_RetornamValoresPadrao()
    {
        var evento = new EventoRetorno();

        Assert.Equal((short)0, evento.Codigo);
        Assert.Null(evento.Descricao);
        Assert.Null(evento.ChaveRPS);
        Assert.Null(evento.ChaveNFe);
    }

    [Fact]
    public void XmlSerialization_RoundTrip_PreservaPropriedades()
    {
        // Arrange
        var evento = new EventoRetorno
        {
            Codigo = 500,
            Descricao = "Erro de teste",
            ChaveRPS = new ChaveRps { InscricaoPrestador = "1234567", SerieRps = "A", NumeroRps = "1" }
        };

        // Act
        var desserializado = DesserializarDeXml(SerializarParaXml(evento))!;

        // Assert
        Assert.Equal(evento.Codigo, desserializado.Codigo);
        Assert.Equal(evento.Descricao, desserializado.Descricao);
        Assert.Equal(evento.ChaveRPS.NumeroRps, desserializado.ChaveRPS!.NumeroRps);
    }

    [Fact]
    public void XmlSerialization_UsaNomesCorretosDosElementos()
    {
        // Arrange
        var evento = new EventoRetorno
        {
            Codigo = 100,
            Descricao = "Teste",
            ChaveRPS = new ChaveRps { NumeroRps = "1" }
        };

        // Act
        var xml = SerializarParaXml(evento);

        // Assert
        Assert.Contains("<Codigo>100</Codigo>", xml);
        Assert.Contains("<Descricao>Teste</Descricao>", xml);
        Assert.Contains("<ChaveRPS>", xml);
    }
}
