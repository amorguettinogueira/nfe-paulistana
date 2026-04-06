using Nfe.Paulistana.Extensions;
using System.Diagnostics;
using System.Text;

namespace Nfe.Paulistana.Diagnostics;

/// <summary>
/// <see cref="DelegatingHandler"/> que intercepta cada chamada SOAP ao webservice da NF-e Paulistana,
/// captura os XMLs de requisição e resposta e notifica via callback (<see cref="Action{SoapExchange}"/>).
/// </summary>
/// <remarks>
/// <para><b>Posicionamento no pipeline:</b> inserido como o handler mais externo da cadeia por
/// <see cref="HttpClientBuilderExtensions.AddNfePaulistanaDiagnostics(Microsoft.Extensions.DependencyInjection.IHttpClientBuilder, Action{SoapExchange})"/>.
/// Em conjunto com <c>AddStandardResilienceHandler</c>, cada tentativa de retry gera uma
/// notificação separada. Para observar apenas o resultado final, adicione o handler de diagnóstico
/// <em>após</em> o de resiliência:
/// <code>b.AddStandardResilienceHandler().AddNfePaulistanaDiagnostics(onExchange)</code></para>
/// <para><b>Re-leitura do conteúdo da resposta:</b> após capturar o XML, o conteúdo da resposta é
/// substituído por um novo <see cref="StringContent"/> equivalente, garantindo que o serviço interno
/// (<c>SoapClient</c>) consiga lê-lo novamente sem erros de stream já consumido.</para>
/// <para><b>Erros HTTP:</b> o callback é invocado inclusive em respostas com status 4xx/5xx
/// (<see cref="SoapExchange.IsSuccess"/> = <see langword="false"/>). A
/// <see cref="Exceptions.NfeRequestException"/> é lançada pelo serviço APÓS o retorno deste handler.</para>
/// </remarks>
/// <param name="onExchange">
/// Callback invocado a cada intercâmbio SOAP capturado. Não pode ser <see langword="null"/>.
/// </param>
public sealed class SoapDiagnosticsHandler(Action<SoapExchange> onExchange) : DelegatingHandler
{
    /// <summary>
    /// Nome do <see cref="ActivitySource"/> utilizado para publicar atividades de tracing distribuído.
    /// Consumidores do OpenTelemetry devem registrar este nome via <c>AddSource(ActivitySourceName)</c>.
    /// </summary>
    public const string ActivitySourceName = "Nfe.Paulistana";

    private static readonly ActivitySource _activitySource = new(ActivitySourceName);

    private readonly Action<SoapExchange> _onExchange = onExchange
        ?? throw new ArgumentNullException(nameof(onExchange));

    /// <summary>
    /// Intercepta a chamada HTTP, mede o tempo decorrido, captura os XMLs de requisição e resposta
    /// e notifica via callback antes de retornar a resposta ao caller.
    /// </summary>
    /// <param name="request">Mensagem HTTP da requisição SOAP.</param>
    /// <param name="cancellationToken">Token de cancelamento propagado ao handler interno.</param>
    /// <returns>A <see cref="HttpResponseMessage"/> recebida do handler interno, com conteúdo re-embalado.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        string requestXml = request.Content is not null
            ? await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)
            : string.Empty;

        string soapAction = request.Headers.TryGetValues("SOAPAction", out IEnumerable<string>? values)
            ? values.First().Trim('"')
            : string.Empty;

        using Activity? activity = _activitySource.StartActivity(
            string.IsNullOrEmpty(soapAction) ? "soap.send" : soapAction,
            ActivityKind.Client);

        _ = activity?.SetTag("soap.action", soapAction);
        _ = activity?.SetTag("server.address", request.RequestUri?.Host);
        _ = activity?.SetTag("url.full", request.RequestUri?.ToString());
        _ = activity?.SetTag("http.request.method", "POST");

        var sw = Stopwatch.StartNew();
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        sw.Stop();

        string responseXml = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        // Re-embala o conteúdo para que SoapClient consiga lê-lo novamente (streams HTTP são consumíveis apenas uma vez)
        response.Content = new StringContent(responseXml, Encoding.UTF8, "text/xml");

        _ = activity?.SetTag("http.response.status_code", (int)response.StatusCode);

        if (response.IsSuccessStatusCode)
        {
            _ = activity?.SetStatus(ActivityStatusCode.Ok);
        }
        else
        {
            _ = activity?.SetStatus(ActivityStatusCode.Error, $"HTTP {(int)response.StatusCode}");
            _ = activity?.SetTag("error.type", $"HTTP_{(int)response.StatusCode}");
        }

        try
        {
            _onExchange(new SoapExchange(
                SoapAction: soapAction,
                RequestXml: requestXml,
                ResponseXml: responseXml,
                Elapsed: sw.Elapsed,
                IsSuccess: response.IsSuccessStatusCode));
        }
        catch (Exception)
        {
            // Exceções lançadas pelo callback de diagnóstico são descartadas intencionalmente.
            // O handler é puramente observacional: o resultado HTTP deve sempre ser retornado
            // ao caller, independente de falhas no código do consumidor.
        }

        return response;
    }
}