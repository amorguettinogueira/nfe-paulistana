using Nfe.Paulistana.Tests.Helpers;
using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.Tests.V1.Models.Response;

/// <summary>
/// Testes unitários para <see cref="ChaveNFe"/>.
/// </summary>
public sealed class ChaveNFeTests
{

    [Fact]
    public void Constructor_SemArgumentos_CriaInstancia() =>
        Assert.NotNull(new ChaveNFe());

    [Fact]
    public void Propriedades_QuandoDefinidas_RetornamMesmosValores()
    {
        var chave = new ChaveNFe
        {
            InscricaoPrestador = "1234567",
            NumeroNFe = "0000001",
            CodigoVerificacao = "ABCD1234"
        };

        Assert.Equal("1234567", chave.InscricaoPrestador);
        Assert.Equal("0000001", chave.NumeroNFe);
        Assert.Equal("ABCD1234", chave.CodigoVerificacao);
    }

    [Fact]
    public void Propriedades_QuandoNaoDefinidas_RetornamNull()
    {
        var chave = new ChaveNFe();

        Assert.Null(chave.InscricaoPrestador);
        Assert.Null(chave.NumeroNFe);
        Assert.Null(chave.CodigoVerificacao);
    }

    [Fact]
    public void XmlSerialization_RoundTrip_PreservaPropriedades()
    {
        // Arrange
        var chave = new ChaveNFe
        {
            InscricaoPrestador = "1234567",
            NumeroNFe = "0000001",
            CodigoVerificacao = "ABCD1234"
        };

        // Act
        var desserializado = XmlTestHelper.DesserializarDeXml<ChaveNFe>(XmlTestHelper.SerializarParaXml(chave))!;

        // Assert
        Assert.Equal(chave.InscricaoPrestador, desserializado.InscricaoPrestador);
        Assert.Equal(chave.NumeroNFe, desserializado.NumeroNFe);
        Assert.Equal(chave.CodigoVerificacao, desserializado.CodigoVerificacao);
    }

    [Fact]
    public void XmlSerialization_UsaNomesCorretosDosElementos()
    {
        // Arrange
        var chave = new ChaveNFe
        {
            InscricaoPrestador = "1234567",
            NumeroNFe = "0000001",
            CodigoVerificacao = "ABCD1234"
        };

        // Act
        var xml = XmlTestHelper.SerializarParaXml(chave);

        // Assert
        Assert.Contains("<InscricaoPrestador>1234567</InscricaoPrestador>", xml);
        Assert.Contains("<NumeroNFe>0000001</NumeroNFe>", xml);
        Assert.Contains("<CodigoVerificacao>ABCD1234</CodigoVerificacao>", xml);
    }
}
