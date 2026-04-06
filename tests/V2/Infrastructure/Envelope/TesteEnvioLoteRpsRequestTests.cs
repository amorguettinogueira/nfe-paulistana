using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Infrastructure.Envelope;
using Nfe.Paulistana.V2.Models.Operations;

namespace Nfe.Paulistana.Tests.V2.Infrastructure.Envelope;

/// <summary>
/// Testes unitários para <see cref="TesteEnvioLoteRpsRequest"/>.
/// </summary>
public sealed class TesteEnvioLoteRpsRequestTests
{
    [Fact]
    public void Construtor_MensagemXmlNula_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => new TesteEnvioLoteRpsRequest(null!));
    }

    [Fact]
    public void Construtor_MensagemXmlValida_AtribuiPropriedade()
    {
        // Arrange
        var pedido = new PedidoEnvioLote();
        var mensagemXml = new MensagemXml<PedidoEnvioLote>(pedido);

        // Act
        var request = new TesteEnvioLoteRpsRequest(mensagemXml);

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
        var request = new TesteEnvioLoteRpsRequest(mensagemXml);

        // Assert
        Assert.Equal(2, request.VersaoSchema);
    }

    [Fact]
    public void ConstrutorPadrao_InicializaVersaoSchemaComoV2()
    {
        // Arrange & Act
        var request = new TesteEnvioLoteRpsRequest();

        // Assert
        Assert.Equal(2, request.VersaoSchema);
    }

    [Fact]
    public void ConstrutorPadrao_MensagemXmlEhNulo()
    {
        // Arrange & Act
        var request = new TesteEnvioLoteRpsRequest();

        // Assert
        Assert.Null(request.MensagemXml);
    }

    [Fact]
    public void VersaoSchema_PodeSerAlterada()
    {
        // Arrange
        var request = new TesteEnvioLoteRpsRequest();

        // Act
        request.VersaoSchema = 3;

        // Assert
        Assert.Equal(3, request.VersaoSchema);
    }

    [Fact]
    public void MensagemXml_PodeSerAlterada()
    {
        // Arrange
        var request = new TesteEnvioLoteRpsRequest();
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
        Assert.ThrowsAny<ArgumentNullException>(() => TesteEnvioLoteRpsRequest.FromPedidoEnvioLote(null!));
    }

    [Fact]
    public void FromPedidoEnvioLote_PedidoValido_CriaMensagemXmlWrapper()
    {
        // Arrange
        var pedido = new PedidoEnvioLote();

        // Act
        var request = TesteEnvioLoteRpsRequest.FromPedidoEnvioLote(pedido);

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
        var request = TesteEnvioLoteRpsRequest.FromPedidoEnvioLote(pedido);

        // Assert
        Assert.Equal(2, request.VersaoSchema);
    }

    [Fact]
    public void OperadorExplicito_PedidoNulo_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAny<ArgumentNullException>(() => (TesteEnvioLoteRpsRequest)((PedidoEnvioLote)null!));
    }

    [Fact]
    public void OperadorExplicito_PedidoValido_CriaRequestComMensagemXml()
    {
        // Arrange
        var pedido = new PedidoEnvioLote();

        // Act
        var request = (TesteEnvioLoteRpsRequest)pedido;

        // Assert
        Assert.NotNull(request.MensagemXml);
        Assert.Same(pedido, request.MensagemXml.Payload);
        Assert.Equal(2, request.VersaoSchema);
    }
}
