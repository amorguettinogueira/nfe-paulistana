using Nfe.Paulistana.Constants;
using System.Net.Http.Headers;

namespace Nfe.Paulistana.Integration.Sample.Infrastructure;

/// <summary>
/// Realiza o download do WSDL do endpoint de produção da NF-e Paulistana via HTTP.
/// O <see cref="HttpClient"/> injetado é configurado em
/// <see cref="Host.ServiceConfigurator.RegisterServices"/> com mTLS (certificado A1)
/// e pipeline de resiliência padrão (<c>AddStandardResilienceHandler</c>).
/// </summary>
/// <param name="http">Cliente HTTP pré-configurado com mTLS e resiliência.</param>
internal sealed class HttpWsdlFetcher(HttpClient http)
{
    private readonly HttpClient _http = http;

    /// <summary>
    /// Faz GET em <c>NfePaulistanaEndpoints.Producao?WSDL</c> com header
    /// <c>Accept: text/xml</c> e retorna o corpo da resposta como string XML bruta.
    /// <para><b>Endpoint fixo:</b> sempre aponta para produção; não há opção de sandbox.</para>
    /// <para><b>Exceções:</b>
    /// <list type="bullet">
    ///   <item><see cref="System.Net.Http.HttpRequestException"/> — resposta não-2xx
    ///         (<see cref="System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode"/>),
    ///         falha de rede ou pipeline de resiliência esgotado.</item>
    ///   <item><see cref="System.OperationCanceledException"/> — token cancelado antes
    ///         ou durante a requisição.</item>
    /// </list></para>
    /// </summary>
    /// <param name="cancellationToken">Token propagado para <c>SendAsync</c> e <c>ReadAsStringAsync</c>.</param>
    /// <returns>Conteúdo WSDL como XML bruto.</returns>
    public async Task<string> GetWsdlAsync(CancellationToken cancellationToken = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"{Endpoints.Producao}?WSDL");
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

        var resp = await _http.SendAsync(req, cancellationToken);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync(cancellationToken);
    }
}