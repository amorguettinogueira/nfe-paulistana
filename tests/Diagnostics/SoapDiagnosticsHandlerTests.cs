using Nfe.Paulistana.Diagnostics;
using System.Net;

namespace Nfe.Paulistana.Tests.Diagnostics;

/// <summary>
/// Testes unitários para <see cref="SoapDiagnosticsHandler"/>:
/// guard clauses do construtor, invocação do callback e isolamento de exceções do callback.
/// </summary>
public sealed class SoapDiagnosticsHandlerTests
{
    private static HttpRequestMessage CriarRequisicao(string body = "<xml/>") =>
        new(HttpMethod.Post, "https://fake-nfe/")
        {
            Content = new StringContent(body, System.Text.Encoding.UTF8, "text/xml")
        };

    private static HttpMessageInvoker CriarInvoker(
        Action<SoapExchange> onExchange,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        string responseBody = "<response/>") =>
        new(new SoapDiagnosticsHandler(onExchange)
        {
            InnerHandler = new FakeHttpMessageHandler(
                new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(responseBody, System.Text.Encoding.UTF8, "text/xml")
                })
        });

    private sealed class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(response);
    }

    // ============================================
    // Construtor
    // ============================================

    [Fact]
    public void Constructor_NullCallback_ThrowsArgumentNullException()
    {
        _ = Assert.Throws<ArgumentNullException>(() => new SoapDiagnosticsHandler(null!));
    }

    // ============================================
    // Happy path — callback invocado
    // ============================================

    [Fact]
    public async Task SendAsync_ValidRequest_InvokesCallbackWithCorrectData()
    {
        // Arrange
        SoapExchange? captured = null;
        using var invoker = CriarInvoker(
            exchange => captured = exchange,
            HttpStatusCode.OK,
            "<response/>");

        // Act
        using var response = await invoker.SendAsync(CriarRequisicao("<xml/>"), CancellationToken.None);

        // Assert
        Assert.NotNull(captured);
        Assert.Equal("<xml/>", captured.RequestXml);
        Assert.Equal("<response/>", captured.ResponseXml);
        Assert.True(captured.IsSuccess);
        Assert.True(captured.Elapsed >= TimeSpan.Zero);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SendAsync_HttpErrorResponse_InvokesCallbackWithIsSuccessFalse()
    {
        // Arrange
        SoapExchange? captured = null;
        using var invoker = CriarInvoker(
            exchange => captured = exchange,
            HttpStatusCode.InternalServerError,
            "<error/>");

        // Act
        using var response = await invoker.SendAsync(CriarRequisicao(), CancellationToken.None);

        // Assert
        Assert.NotNull(captured);
        Assert.False(captured.IsSuccess);
        Assert.Equal("<error/>", captured.ResponseXml);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // ============================================
    // Isolamento de exceções do callback
    // ============================================

    [Fact]
    public async Task SendAsync_CallbackThrows_DoesNotPropagateException()
    {
        // Arrange — callback sempre lança exceção
        using var invoker = CriarInvoker(
            _ => throw new InvalidOperationException("Falha simulada no callback de diagnóstico"));

        // Act — não deve lançar, apesar do callback falhar
        using var response = await invoker.SendAsync(CriarRequisicao(), CancellationToken.None);

        // Assert — resposta HTTP retornada normalmente
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SendAsync_CallbackThrows_ResponseBodyRemainsReadable()
    {
        // Arrange
        using var invoker = CriarInvoker(
            _ => throw new Exception("Callback inválido"),
            HttpStatusCode.OK,
            "<body/>");

        // Act
        using var response = await invoker.SendAsync(CriarRequisicao(), CancellationToken.None);
        var body = await response.Content.ReadAsStringAsync();

        // Assert — conteúdo re-embalado pelo handler ainda é legível após falha do callback
        Assert.Equal("<body/>", body);
    }
}
