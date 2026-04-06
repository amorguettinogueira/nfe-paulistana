using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Infrastructure.Envelope;
using Nfe.Paulistana.V2.Models.Response;

namespace Nfe.Paulistana.Tests.V2.Infrastructure.Envelope;

/// <summary>
/// Testes unitários para <see cref="TesteEnvioLoteRpsResponse"/>.
/// </summary>
public sealed class TesteEnvioLoteRpsResponseTests
{
    [Fact]
    public void Construtor_MensagemXmlNula_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => new TesteEnvioLoteRpsResponse(null!));
    }

    [Fact]
    public void Construtor_MensagemXmlValida_AtribuiPropriedade()
    {
        // Arrange
        var retorno = new RetornoEnvioLoteRps();
        var mensagemXml = new MensagemXml<RetornoEnvioLoteRps>(retorno);

        // Act
        var response = new TesteEnvioLoteRpsResponse(mensagemXml);

        // Assert
        Assert.Same(mensagemXml, response.RetornoXml);
    }

    [Fact]
    public void ConstrutorPadrao_RetornoXmlEhNulo()
    {
        // Arrange & Act
        var response = new TesteEnvioLoteRpsResponse();

        // Assert
        Assert.Null(response.RetornoXml);
    }

    [Fact]
    public void RetornoXml_PodeSerAlterada()
    {
        // Arrange
        var response = new TesteEnvioLoteRpsResponse();
        var retorno = new RetornoEnvioLoteRps();
        var mensagemXml = new MensagemXml<RetornoEnvioLoteRps>(retorno);

        // Act
        response.RetornoXml = mensagemXml;

        // Assert
        Assert.Same(mensagemXml, response.RetornoXml);
    }

    [Fact]
    public void FromRetornoEnvioLoteRps_RetornoNulo_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => TesteEnvioLoteRpsResponse.FromRetornoEnvioLoteRps(null!));
    }

    [Fact]
    public void FromRetornoEnvioLoteRps_RetornoValido_CriaMensagemXmlWrapper()
    {
        // Arrange
        var retorno = new RetornoEnvioLoteRps();

        // Act
        var response = TesteEnvioLoteRpsResponse.FromRetornoEnvioLoteRps(retorno);

        // Assert
        Assert.NotNull(response.RetornoXml);
        Assert.Same(retorno, response.RetornoXml.Payload);
    }

    [Fact]
    public void OperadorExplicito_RetornoNulo_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => (TesteEnvioLoteRpsResponse)((RetornoEnvioLoteRps)null!));
    }

    [Fact]
    public void OperadorExplicito_RetornoValido_CriaResponseComRetornoXml()
    {
        // Arrange
        var retorno = new RetornoEnvioLoteRps();

        // Act
        var response = (TesteEnvioLoteRpsResponse)retorno;

        // Assert
        Assert.NotNull(response.RetornoXml);
        Assert.Same(retorno, response.RetornoXml.Payload);
    }
}
