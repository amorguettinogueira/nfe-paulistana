using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Infrastructure.Envelope;
using Nfe.Paulistana.V2.Models.Operations;

namespace Nfe.Paulistana.Tests.V2.Infrastructure.Envelope;

/// <summary>
/// Testes unitários para <see cref="EnvioLoteRpsRequest"/>.
/// </summary>
public sealed class EnvioLoteRpsRequestTests
{
    [Fact]
    public void Construtor_MensagemXmlNula_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => new EnvioLoteRpsRequest(null!));
    }

    [Fact]
    public void Construtor_MensagemXmlValida_AtribuiPropriedade()
    {
        // Arrange
        var pedido = new PedidoEnvioLote();
        var mensagemXml = new MensagemXml<PedidoEnvioLote>(pedido);

        // Act
        var request = new EnvioLoteRpsRequest(mensagemXml);

        // Assert
        Assert.Same(mensagemXml, request.MensagemXml);
    }

    [Fact]
    public void Construtor_MensagemXmlValida_DefineVersaoSchemaComoV2()
    {
        // Arrange
        var pedido = new PedidoEnvioLote();
        var mensagemXml = new MensagemXml<PedidoEnvioLote>(pedido);

        // Act
        var request = new EnvioLoteRpsRequest(mensagemXml);

        // Assert
        Assert.Equal(2, request.VersaoSchema);
    }

    [Fact]
    public void ConstrutorPadrao_InicializaVersaoSchemaComoV2()
    {
        // Arrange & Act
        var request = new EnvioLoteRpsRequest();

        // Assert
        Assert.Equal(2, request.VersaoSchema);
    }

    [Fact]
    public void ConstrutorPadrao_MensagemXmlEhNulo()
    {
        // Arrange & Act
        var request = new EnvioLoteRpsRequest();

        // Assert
        Assert.Null(request.MensagemXml);
    }

    [Fact]
    public void VersaoSchema_PodeSerAlterada()
    {
        // Arrange
        var request = new EnvioLoteRpsRequest();

        // Act
        request.VersaoSchema = 3;

        // Assert
        Assert.Equal(3, request.VersaoSchema);
    }

    [Fact]
    public void MensagemXml_PodeSerAlterada()
    {
        // Arrange
        var request = new EnvioLoteRpsRequest();
        var pedido = new PedidoEnvioLote();
        var mensagemXml = new MensagemXml<PedidoEnvioLote>(pedido);

        // Act
        request.MensagemXml = mensagemXml;

        // Assert
        Assert.Same(mensagemXml, request.MensagemXml);
    }

    [Fact]
    public void FromPedidoEnvioLote_PedidoNulo_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => EnvioLoteRpsRequest.FromPedidoEnvioLote(null!));
    }

    [Fact]
    public void FromPedidoEnvioLote_PedidoValido_CriaMensagemXmlWrapper()
    {
        // Arrange
        var pedido = new PedidoEnvioLote();

        // Act
        var request = EnvioLoteRpsRequest.FromPedidoEnvioLote(pedido);

        // Assert
        Assert.NotNull(request.MensagemXml);
        Assert.Same(pedido, request.MensagemXml.Payload);
    }

    [Fact]
    public void FromPedidoEnvioLote_PedidoValido_DefineVersaoSchemaComoV2()
    {
        // Arrange
        var pedido = new PedidoEnvioLote();

        // Act
        var request = EnvioLoteRpsRequest.FromPedidoEnvioLote(pedido);

        // Assert
        Assert.Equal(2, request.VersaoSchema);
    }

    [Fact]
    public void OperadorExplicito_PedidoNulo_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => (EnvioLoteRpsRequest)((PedidoEnvioLote)null!));
    }

    [Fact]
    public void OperadorExplicito_PedidoValido_CriaRequestComMensagemXml()
    {
        // Arrange
        var pedido = new PedidoEnvioLote();

        // Act
        var request = (EnvioLoteRpsRequest)pedido;

        // Assert
        Assert.NotNull(request.MensagemXml);
        Assert.Same(pedido, request.MensagemXml.Payload);
        Assert.Equal(2, request.VersaoSchema);
    }
}
