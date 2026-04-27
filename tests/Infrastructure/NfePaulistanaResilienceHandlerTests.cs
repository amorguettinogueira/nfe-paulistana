using Nfe.Paulistana.Infrastructure;
using System.Net;

namespace Nfe.Paulistana.Tests.Infrastructure;

public sealed class NfePaulistanaResilienceHandlerTests
{
    private static readonly TimeSpan FastTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan[] NoDelay = [TimeSpan.Zero, TimeSpan.Zero];

    // ============================================
    // Helpers
    // ============================================

    private static HttpClient BuildClient(Func<int, Task<HttpResponseMessage>> respond)
    {
        int calls = 0;
        var handler = new NfePaulistanaResilienceHandler(FastTimeout, NoDelay);
        handler.InnerHandler = new FakeHttpMessageHandler(_ => respond(++calls));
        return new HttpClient(handler) { BaseAddress = new Uri("http://test/") };
    }

    private static HttpResponseMessage Ok() => new(HttpStatusCode.OK);

    private static HttpResponseMessage Transient(HttpStatusCode code = HttpStatusCode.ServiceUnavailable)
        => new(code);

    // ============================================
    // Sucesso sem retry
    // ============================================

    [Fact]
    public async Task SendAsync_SuccessOnFirstAttempt_MakesSingleAttempt()
    {
        // Arrange
        int calls = 0;
        using var client = BuildClient(_ => { calls++; return Task.FromResult(Ok()); });

        // Act
        using var response = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, calls);
    }

    // ============================================
    // Retry em respostas transitórias
    // ============================================

    [Theory]
    [InlineData(408)]
    [InlineData(429)]
    [InlineData(500)]
    [InlineData(502)]
    [InlineData(503)]
    [InlineData(504)]
    public async Task SendAsync_TransientStatusCode_RetriesUntilSuccess(int statusCode)
    {
        // Arrange
        using var client = BuildClient(call =>
            Task.FromResult(call < 3 ? Transient((HttpStatusCode)statusCode) : Ok()));

        // Act
        using var response = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SendAsync_TransientThenSuccess_ReturnsSuccessOnSecondAttempt()
    {
        // Arrange
        int calls = 0;
        using var client = BuildClient(call =>
        {
            calls++;
            return Task.FromResult(call == 1 ? Transient() : Ok());
        });

        // Act
        using var response = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, calls);
    }

    [Fact]
    public async Task SendAsync_AllAttemptsTransient_ReturnsLastTransientResponse()
    {
        // Arrange
        using var client = BuildClient(_ => Task.FromResult(Transient(HttpStatusCode.ServiceUnavailable)));

        // Act
        using var response = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    // ============================================
    // Sem retry em respostas não transitórias
    // ============================================

    [Theory]
    [InlineData(200)]
    [InlineData(201)]
    [InlineData(400)]
    [InlineData(401)]
    [InlineData(403)]
    [InlineData(404)]
    [InlineData(422)]
    public async Task SendAsync_NonTransientStatusCode_DoesNotRetry(int statusCode)
    {
        // Arrange
        int calls = 0;
        using var client = BuildClient(_ =>
        {
            calls++;
            return Task.FromResult(new HttpResponseMessage((HttpStatusCode)statusCode));
        });

        // Act
        using var response = await client.GetAsync("/");

        // Assert
        Assert.Equal((HttpStatusCode)statusCode, response.StatusCode);
        Assert.Equal(1, calls);
    }

    // ============================================
    // Retry em HttpRequestException (falha de rede)
    // ============================================

    [Fact]
    public async Task SendAsync_HttpRequestExceptionThenSuccess_Retries()
    {
        // Arrange
        int calls = 0;
        using var client = BuildClient(call =>
        {
            calls++;
            return call < 3
                ? Task.FromException<HttpResponseMessage>(new HttpRequestException("Falha de rede simulada."))
                : Task.FromResult(Ok());
        });

        // Act
        using var response = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(3, calls);
    }

    [Fact]
    public async Task SendAsync_AllAttemptsHttpRequestException_Throws()
    {
        // Arrange
        using var client = BuildClient(_ =>
            Task.FromException<HttpResponseMessage>(new HttpRequestException("Falha de rede simulada.")));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync("/"));
    }

    // ============================================
    // Timeout por tentativa
    // ============================================

    [Fact]
    public async Task SendAsync_TimeoutOnFirstAttemptsThenSuccess_Retries()
    {
        // Arrange
        var shortTimeout = TimeSpan.FromMilliseconds(50);
        int calls = 0;
        var handler = new NfePaulistanaResilienceHandler(shortTimeout, NoDelay);
        handler.InnerHandler = new FakeHttpMessageHandler(async ct =>
        {
            calls++;
            if (calls < 3)
                await Task.Delay(TimeSpan.FromSeconds(10), ct); // força o timeout por tentativa
            return Ok();
        });
        using var client = new HttpClient(handler) { BaseAddress = new Uri("http://test/") };

        // Act
        using var response = await client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(3, calls);
    }

    [Fact]
    public async Task SendAsync_AllAttemptsTimeout_ThrowsOperationCanceledException()
    {
        // Arrange
        var shortTimeout = TimeSpan.FromMilliseconds(50);
        var handler = new NfePaulistanaResilienceHandler(shortTimeout, NoDelay);
        handler.InnerHandler = new FakeHttpMessageHandler(
            async ct => { await Task.Delay(TimeSpan.FromSeconds(10), ct); return Ok(); });
        using var client = new HttpClient(handler) { BaseAddress = new Uri("http://test/") };

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.GetAsync("/"));
    }

    // ============================================
    // Cancelamento pelo chamador
    // ============================================

    [Fact]
    public async Task SendAsync_CallerCancels_DoesNotRetryAndThrows()
    {
        // Arrange
        int calls = 0;
        using var cts = new CancellationTokenSource();
        var handler = new NfePaulistanaResilienceHandler(FastTimeout, NoDelay);
        handler.InnerHandler = new FakeHttpMessageHandler(async ct =>
        {
            calls++;
            cts.Cancel(); // simula o chamador cancelando durante a requisição
            await Task.Delay(TimeSpan.FromSeconds(1), ct);
            return Ok();
        });
        using var client = new HttpClient(handler) { BaseAddress = new Uri("http://test/") };

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.GetAsync("/", cts.Token));

        Assert.Equal(1, calls);
    }

    // ============================================
    // Fake handler para testes
    // ============================================

    private sealed class FakeHttpMessageHandler(
        Func<CancellationToken, Task<HttpResponseMessage>> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken) =>
            respond(cancellationToken);
    }
}
