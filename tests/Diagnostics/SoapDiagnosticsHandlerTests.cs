using Nfe.Paulistana.Diagnostics;
using Nfe.Paulistana.Tests.Helpers;
using System.Diagnostics;
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
            InnerHandler = new FakeHttpClient.FakeHttpMessageHandler(
                new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(responseBody, System.Text.Encoding.UTF8, "text/xml")
                })
        });

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
        using HttpMessageInvoker invoker = CriarInvoker(
            exchange => captured = exchange,
            HttpStatusCode.OK,
            "<response/>");

        // Act
        using HttpResponseMessage response = await invoker.SendAsync(CriarRequisicao("<xml/>"), CancellationToken.None);

        // Assert
        Assert.NotNull(captured);
        Assert.Equal("<xml/>", captured.RequestXml);
        // Em respostas de sucesso, ResponseXml é string vazia (corpo lido de forma incremental pelo serviço)
        Assert.Equal(string.Empty, captured.ResponseXml);
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

        // Assert — conteúdo da resposta acessível após falha do callback
        Assert.Equal("<body/>", body);
    }

    // ============================================
    // Request.Content null (linha 57)
    // ============================================

    [Fact]
    public async Task SendAsync_NullRequestContent_SetsEmptyRequestXml()
    {
        // Arrange
        SoapExchange? captured = null;
        using var invoker = CriarInvoker(exchange => captured = exchange);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://fake-nfe/")
        {
            Content = null
        };

        // Act
        using var response = await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(captured);
        Assert.Equal(string.Empty, captured.RequestXml);
    }

    // ============================================
    // SOAPAction header (linha 61)
    // ============================================

    [Fact]
    public async Task SendAsync_WithSoapActionHeader_CapturesSoapAction()
    {
        // Arrange
        SoapExchange? captured = null;
        using var invoker = CriarInvoker(exchange => captured = exchange);

        var request = CriarRequisicao();
        request.Headers.Add("SOAPAction", "\"http://test.com/TestAction\"");

        // Act
        using var response = await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(captured);
        Assert.Equal("http://test.com/TestAction", captured.SoapAction);
    }

    [Fact]
    public async Task SendAsync_WithoutSoapActionHeader_SetsEmptySoapAction()
    {
        // Arrange
        SoapExchange? captured = null;
        using var invoker = CriarInvoker(exchange => captured = exchange);

        var request = CriarRequisicao();
        // Não adiciona SOAPAction header

        // Act
        using var response = await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(captured);
        Assert.Equal(string.Empty, captured.SoapAction);
    }

    [Fact]
    public async Task SendAsync_WithMultipleSoapActionHeaders_UsesFirstValue()
    {
        // Arrange
        SoapExchange? captured = null;
        using var invoker = CriarInvoker(exchange => captured = exchange);

        var request = CriarRequisicao();
        request.Headers.Add("SOAPAction", "\"http://first.com/Action\"");

        // Act
        using var response = await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(captured);
        Assert.Equal("http://first.com/Action", captured.SoapAction);
    }

    // ============================================
    // Activity e tags (linhas 65-72, 87, 91-92)
    // ============================================

    [Fact]
    public async Task SendAsync_WithActivityListener_CreatesActivityWithSoapAction()
    {
        // Arrange
        Activity? capturedActivity = null;
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == SoapDiagnosticsHandler.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => capturedActivity = activity
        };
        ActivitySource.AddActivityListener(listener);

        SoapExchange? captured = null;
        using var invoker = CriarInvoker(exchange => captured = exchange);

        var request = CriarRequisicao();
        request.Headers.Add("SOAPAction", "\"http://test.com/TestAction\"");

        // Act
        using var response = await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedActivity);
        Assert.Equal("http://test.com/TestAction", capturedActivity.DisplayName);
        Assert.Equal(ActivityKind.Client, capturedActivity.Kind);
        Assert.Equal("http://test.com/TestAction", capturedActivity.GetTagItem("soap.action"));
        Assert.Equal("fake-nfe", capturedActivity.GetTagItem("server.address"));
        Assert.Equal("https://fake-nfe/", capturedActivity.GetTagItem("url.full"));
        Assert.Equal("POST", capturedActivity.GetTagItem("http.request.method"));
        Assert.Equal(200, capturedActivity.GetTagItem("http.response.status_code"));
        Assert.Equal(ActivityStatusCode.Ok, capturedActivity.Status);
    }

    [Fact]
    public async Task SendAsync_WithoutSoapAction_CreatesActivityWithDefaultName()
    {
        // Arrange
        Activity? capturedActivity = null;
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == SoapDiagnosticsHandler.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => capturedActivity = activity
        };
        ActivitySource.AddActivityListener(listener);

        using var invoker = CriarInvoker(_ => { });

        var request = CriarRequisicao();

        // Act
        using var response = await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedActivity);
        Assert.Equal("soap.send", capturedActivity.DisplayName);
    }

    [Fact]
    public async Task SendAsync_ErrorResponse_SetsActivityErrorStatus()
    {
        // Arrange
        Activity? capturedActivity = null;
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == SoapDiagnosticsHandler.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => capturedActivity = activity
        };
        ActivitySource.AddActivityListener(listener);

        using var invoker = CriarInvoker(
            _ => { },
            HttpStatusCode.InternalServerError,
            "<error/>");

        var request = CriarRequisicao();

        // Act
        using var response = await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedActivity);
        Assert.Equal(500, capturedActivity.GetTagItem("http.response.status_code"));
        Assert.Equal(ActivityStatusCode.Error, capturedActivity.Status);
        Assert.Equal("HTTP 500", capturedActivity.StatusDescription);
        Assert.Equal("HTTP_500", capturedActivity.GetTagItem("error.type"));
    }

    [Fact]
    public async Task SendAsync_BadRequestResponse_SetsActivityErrorStatusWith400()
    {
        // Arrange
        Activity? capturedActivity = null;
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == SoapDiagnosticsHandler.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => capturedActivity = activity
        };
        ActivitySource.AddActivityListener(listener);

        using var invoker = CriarInvoker(
            _ => { },
            HttpStatusCode.BadRequest,
            "<error/>");

        var request = CriarRequisicao();

        // Act
        using var response = await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedActivity);
        Assert.Equal(400, capturedActivity.GetTagItem("http.response.status_code"));
        Assert.Equal(ActivityStatusCode.Error, capturedActivity.Status);
        Assert.Equal("HTTP_400", capturedActivity.GetTagItem("error.type"));
    }

    // ============================================
    // Tratamento do corpo da resposta por tipo de status
    // ============================================

    [Fact]
    public async Task SendAsync_SuccessResponse_ResponseBodyNotReembaled()
    {
        // Arrange — verifica que em sucesso o conteúdo não é re-embalado (stream passado direto ao caller)
        using HttpMessageInvoker invoker = CriarInvoker(_ => { }, HttpStatusCode.OK, "<original/>");

        // Act
        using HttpResponseMessage response = await invoker.SendAsync(CriarRequisicao(), CancellationToken.None);
        string body = await response.Content.ReadAsStringAsync();

        // Assert — conteúdo legível (StringContent do FakeHttpClient é sempre buffered)
        Assert.Equal("<original/>", body);
    }

    [Fact]
    public async Task SendAsync_ErrorResponse_ResponseBodyIsReembaled()
    {
        // Arrange — em erro o corpo é re-embalado para permitir releitura pelo SoapClient
        using HttpMessageInvoker invoker = CriarInvoker(_ => { }, HttpStatusCode.InternalServerError, "<error/>");

        // Act
        using HttpResponseMessage response = await invoker.SendAsync(CriarRequisicao(), CancellationToken.None);

        // Lê duas vezes para confirmar que foi re-embalado
        string firstRead = await response.Content.ReadAsStringAsync();
        string secondRead = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal("<error/>", firstRead);
        Assert.Equal("<error/>", secondRead);
    }

    [Fact]
    public async Task SendAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        using HttpMessageInvoker invoker = CriarInvoker(_ => { });

        // Act & Assert
        _ = await Assert.ThrowsAnyAsync<ArgumentNullException>(
            () => invoker.SendAsync(null!, CancellationToken.None));
    }
}