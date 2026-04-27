using Nfe.Paulistana.Extensions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
/// <para><b>Corpo da resposta:</b> em respostas de sucesso (2xx) o corpo não é capturado por este handler;
/// <see cref="SoapExchange.ResponseXml"/> será uma string vazia nesses casos. Em respostas de erro
/// (4xx/5xx) o corpo é capturado e re-embalado para que o serviço interno consiga lê-lo novamente.</para>
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
    /// Intercepta a chamada HTTP, mede o tempo decorrido, captura o XML de requisição e (em caso de erro)
    /// o XML de resposta, e notifica via callback antes de retornar a resposta ao caller.
    /// </summary>
    /// <param name="request">Mensagem HTTP da requisição SOAP.</param>
    /// <param name="cancellationToken">Token de cancelamento propagado ao handler interno.</param>
    /// <returns>A <see cref="HttpResponseMessage"/> recebida do handler interno.</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "As exceções lançadas pelo callback de diagnóstico são descartadas intencionalmente.")]
    [SuppressMessage("Roslynator", "RCS1075:Avoid empty catch clause that catches System.Exception", Justification = "As exceções lançadas pelo callback de diagnóstico são descartadas intencionalmente.")]
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

        _ = activity?.SetTag("http.response.status_code", (int)response.StatusCode);

        string responseXml = string.Empty;

        if (!response.IsSuccessStatusCode)
        {
            responseXml = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            // Re-embala o conteúdo de erro para que SoapClient consiga lê-lo novamente (streams HTTP são consumíveis apenas uma vez)
            response.Content = new StringContent(responseXml, Encoding.UTF8, "text/xml");

            _ = activity?.SetStatus(ActivityStatusCode.Error, $"HTTP {(int)response.StatusCode}");
            _ = activity?.SetTag("error.type", $"HTTP_{(int)response.StatusCode}");
        }
        else
        {
            _ = activity?.SetStatus(ActivityStatusCode.Ok);
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
