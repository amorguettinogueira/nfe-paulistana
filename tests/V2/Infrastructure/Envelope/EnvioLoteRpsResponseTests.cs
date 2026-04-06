using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Infrastructure.Envelope;
using Nfe.Paulistana.V2.Models.Response;

namespace Nfe.Paulistana.Tests.V2.Infrastructure.Envelope;

/// <summary>
/// Testes unitários para <see cref="EnvioLoteRpsResponse"/>.
/// </summary>
public sealed class EnvioLoteRpsResponseTests
{
    [Fact]
    public void Construtor_MensagemXmlNula_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => new EnvioLoteRpsResponse(null!));
    }

    [Fact]
    public void Construtor_MensagemXmlValida_AtribuiPropriedade()
    {
        // Arrange
        var retorno = new RetornoEnvioLoteRps();
        var mensagemXml = new MensagemXml<RetornoEnvioLoteRps>(retorno);

        // Act
        var response = new EnvioLoteRpsResponse(mensagemXml);

        // Assert
        Assert.Same(mensagemXml, response.RetornoXml);
    }

    [Fact]
    public void ConstrutorPadrao_RetornoXmlEhNulo()
    {
        // Arrange & Act
        var response = new EnvioLoteRpsResponse();

        // Assert
        Assert.Null(response.RetornoXml);
    }

    [Fact]
    public void RetornoXml_PodeSerAlterada()
    {
        // Arrange
        var response = new EnvioLoteRpsResponse();
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
        Assert.ThrowsAny<ArgumentNullException>(() => EnvioLoteRpsResponse.FromRetornoEnvioLoteRps(null!));
    }

    [Fact]
    public void FromRetornoEnvioLoteRps_RetornoValido_CriaMensagemXmlWrapper()
    {
        // Arrange
        var retorno = new RetornoEnvioLoteRps();

        // Act
        var response = EnvioLoteRpsResponse.FromRetornoEnvioLoteRps(retorno);

        // Assert
        Assert.NotNull(response.RetornoXml);
        Assert.Same(retorno, response.RetornoXml.Payload);
    }

    [Fact]
    public void OperadorExplicito_RetornoNulo_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => (EnvioLoteRpsResponse)((RetornoEnvioLoteRps)null!));
    }

    [Fact]
    public void OperadorExplicito_RetornoValido_CriaResponseComRetornoXml()
    {
        // Arrange
        var retorno = new RetornoEnvioLoteRps();

        // Act
        var response = (EnvioLoteRpsResponse)retorno;

        // Assert
        Assert.NotNull(response.RetornoXml);
        Assert.Same(retorno, response.RetornoXml.Payload);
    }
}
