using System.Net;

namespace Nfe.Paulistana.Tests.Helpers;

/// <summary>
/// Auxiliar de testes que cria um <see cref="HttpClient"/> com handler falso,
/// evitando chamadas HTTP reais durante os testes unitários.
/// </summary>
internal static class FakeHttpClient
{
    /// <summary>
    /// Cria um <see cref="HttpClient"/> que sempre retorna <paramref name="responseXml"/>
    /// com status <see cref="HttpStatusCode.OK"/> e content-type <c>text/xml</c>.
    /// </summary>
    internal static HttpClient Create(string responseXml)
    {
        var handler = new FakeHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseXml, System.Text.Encoding.UTF8, "text/xml")
            });
        return new HttpClient(handler) { BaseAddress = new Uri("https://fake-nfe/") };
    }

    internal sealed class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(response);
    }
}