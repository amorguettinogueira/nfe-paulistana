using Microsoft.Extensions.DependencyInjection;
using Nfe.Paulistana.Diagnostics;

namespace Nfe.Paulistana.Extensions;

/// <summary>
/// Extensões de <see cref="IHttpClientBuilder"/> para funcionalidades diagnósticas da biblioteca Nfe.Paulistana.
/// </summary>
public static class HttpClientBuilderExtensions
{
    /// <summary>
    /// Adiciona um <see cref="SoapDiagnosticsHandler"/> ao pipeline do <see cref="System.Net.Http.HttpClient"/>,
    /// notificando via <paramref name="onExchange"/> a cada intercâmbio SOAP capturado.
    /// </summary>
    /// <remarks>
    /// <para>O handler é inserido como o mais externo da cadeia: por padrão, cada tentativa de retry
    /// do pipeline de resiliência gera uma notificação separada. Para observar apenas o resultado final,
    /// adicione <em>após</em> o handler de resiliência:</para>
    /// <code>b.AddStandardResilienceHandler().AddNfePaulistanaDiagnostics(onExchange)</code>
    /// </remarks>
    /// <param name="builder">O builder do HttpClient ao qual o handler será adicionado.</param>
    /// <param name="onExchange">
    /// Callback invocado a cada intercâmbio SOAP capturado.
    /// Recebe um <see cref="SoapExchange"/> com os XMLs, tempo decorrido e status HTTP.
    /// Não pode ser <see langword="null"/>.
    /// </param>
    /// <returns>O mesmo <paramref name="builder"/> para encadeamento fluente.</returns>
    /// <exception cref="ArgumentNullException">
    /// Lançado quando <paramref name="builder"/> ou <paramref name="onExchange"/> é <see langword="null"/>.
    /// </exception>
    public static IHttpClientBuilder AddNfePaulistanaDiagnostics(
        this IHttpClientBuilder builder,
        Action<SoapExchange> onExchange)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(onExchange);

        return builder.AddHttpMessageHandler(() => new SoapDiagnosticsHandler(onExchange));
    }

    /// <summary>
    /// Adiciona um <see cref="SoapDiagnosticsHandler"/> ao pipeline do <see cref="System.Net.Http.HttpClient"/>,
    /// resolvendo o callback via <see cref="IServiceProvider"/> no momento em que o handler é instanciado.
    /// </summary>
    /// <remarks>
    /// <para>Use esta sobrecarga para acessar serviços do container de DI no callback — por exemplo,
    /// para registrar os intercâmbios via <c>ILogger</c>:</para>
    /// <code>
    /// configureClient: b => b.AddNfePaulistanaDiagnostics(sp =>
    /// {
    ///     var logger = sp.GetRequiredService&lt;ILogger&lt;MyClass&gt;&gt;();
    ///     return exchange => logger.LogDebug(
    ///         "[{SoapAction}] {ElapsedMs}ms\nReq: {Req}\nRes: {Res}",
    ///         exchange.SoapAction, (long)exchange.Elapsed.TotalMilliseconds,
    ///         exchange.RequestXml, exchange.ResponseXml);
    /// })
    /// </code>
    /// <para>A fábrica <paramref name="onExchangeFactory"/> é invocada uma vez por instância criada pelo
    /// <see cref="System.Net.Http.IHttpClientFactory"/>, não a cada requisição.</para>
    /// </remarks>
    /// <param name="builder">O builder do HttpClient ao qual o handler será adicionado.</param>
    /// <param name="onExchangeFactory">
    /// Fábrica que recebe o <see cref="IServiceProvider"/> e retorna o callback a ser usado pelo handler.
    /// Não pode ser <see langword="null"/>.
    /// </param>
    /// <returns>O mesmo <paramref name="builder"/> para encadeamento fluente.</returns>
    /// <exception cref="ArgumentNullException">
    /// Lançado quando <paramref name="builder"/> ou <paramref name="onExchangeFactory"/> é <see langword="null"/>.
    /// </exception>
    public static IHttpClientBuilder AddNfePaulistanaDiagnostics(
        this IHttpClientBuilder builder,
        Func<IServiceProvider, Action<SoapExchange>> onExchangeFactory)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(onExchangeFactory);

        return builder.AddHttpMessageHandler(sp => new SoapDiagnosticsHandler(onExchangeFactory(sp)));
    }
}
