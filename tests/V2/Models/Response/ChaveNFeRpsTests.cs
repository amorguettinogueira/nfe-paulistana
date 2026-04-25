using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V2.Models.Response;

namespace Nfe.Paulistana.Tests.V2.Models.Response;

/// <summary>
/// Testes unitários para <see cref="ChaveNFeRps"/>.
/// </summary>
public sealed class ChaveNFeRpsTests
{

    [Fact]
    public void Constructor_SemArgumentos_CriaInstancia() =>
        Assert.NotNull(new ChaveNFeRps());

    [Fact]
    public void ChaveNFe_QuandoDefinida_RetornaMesmaReferencia()
    {
        var chaveNFe = new ChaveNFe { NumeroNFe = "0000001" };
        var par = new ChaveNFeRps { ChaveNFe = chaveNFe };

        Assert.Same(chaveNFe, par.ChaveNFe);
    }

    [Fact]
    public void ChaveRPS_QuandoDefinido_RetornaMesmaReferencia()
    {
        var chaveRps = new ChaveRps { NumeroRps = "1" };
        var par = new ChaveNFeRps { ChaveRPS = chaveRps };

        Assert.Same(chaveRps, par.ChaveRPS);
    }

    [Fact]
    public void Propriedades_QuandoNaoDefinidas_RetornamNull()
    {
        var par = new ChaveNFeRps();

        Assert.Null(par.ChaveNFe);
        Assert.Null(par.ChaveRPS);
    }

    [Fact]
    public void XmlSerialization_RoundTrip_PreservaPropriedades()
    {
        // Arrange
        var par = new ChaveNFeRps
        {
            ChaveNFe = new ChaveNFe { InscricaoPrestador = "1234567", NumeroNFe = "0000001" },
            ChaveRPS = new ChaveRps { InscricaoPrestador = "1234567", SerieRps = "A", NumeroRps = "1" }
        };

        // Act
        var desserializado = XmlTestHelper.DesserializarDeXml<ChaveNFeRps>(XmlTestHelper.SerializarParaXml(par))!;

        // Assert
        Assert.Equal(par.ChaveNFe.NumeroNFe, desserializado.ChaveNFe!.NumeroNFe);
        Assert.Equal(par.ChaveRPS.NumeroRps, desserializado.ChaveRPS!.NumeroRps);
    }

    [Fact]
    public void XmlSerialization_UsaNomesCorretosDosElementosFilhos()
    {
        // Arrange
        var par = new ChaveNFeRps
        {
            ChaveNFe = new ChaveNFe { NumeroNFe = "0000001" },
            ChaveRPS = new ChaveRps { NumeroRps = "1" }
        };

        // Act
        var xml = XmlTestHelper.SerializarParaXml(par);

        // Assert
        Assert.Contains("<ChaveNFe>", xml);
        Assert.Contains("<ChaveRPS>", xml);
    }
}
