using System.Net;

namespace Nfe.Paulistana.Infrastructure;

/// <summary>
/// Handler de resiliência embutido que aplica retry com backoff exponencial e timeout por tentativa
/// em todas as requisições ao webservice da NF-e Paulistana.
/// </summary>
/// <remarks>
/// <para>
/// Registrado automaticamente na pipeline de cada <c>HttpClient</c> quando o consumidor não fornece
/// um delegate <c>configureClient</c> na configuração dos serviços. Ao fornecer <c>configureClient</c>,
/// este handler é ignorado por completo — a responsabilidade de resiliência passa integralmente ao consumidor.
/// </para>
/// <para>
/// Comportamento padrão:
/// <list type="bullet">
/// <item>Até 3 tentativas com backoff exponencial de 1 s e 2 s entre elas.</item>
/// <item>Timeout individual por tentativa configurável via <see cref="Options.NfeOptions.TimeoutPorTentativa"/>.</item>
/// <item>Retry em respostas transitórias: 408, 429, 500, 502, 503 e 504.</item>
/// <item>Retry em falhas de rede (<see cref="HttpRequestException"/>).</item>
/// <item>Cancelamento pelo chamador interrompe imediatamente, sem nova tentativa.</item>
/// </list>
/// </para>
/// </remarks>
internal sealed class NfePaulistanaResilienceHandler : DelegatingHandler
{
    private const int MaxTentativas = 3;

    private static readonly TimeSpan[] DefaultBackoff =
    [
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
    ];

    private readonly TimeSpan _timeout;
    private readonly TimeSpan[] _backoff;

    internal NfePaulistanaResilienceHandler(TimeSpan timeout, TimeSpan[]? backoff = null)
    {
        _timeout = timeout;
        _backoff = backoff ?? DefaultBackoff;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Content != null)
        {
            // Bufferiza o corpo da requisição uma única vez para suportar retry em chamadas POST.
            // CA2016: LoadIntoBufferAsync não expõe sobrecarga pública com CancellationToken no .NET 8/9/10.
#pragma warning disable CA2016
            await request.Content.LoadIntoBufferAsync().ConfigureAwait(false);
#pragma warning restore CA2016
        }

        for (int tentativa = 0; tentativa < MaxTentativas; tentativa++)
        {
            bool isUltimaTentativa = tentativa == MaxTentativas - 1;

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_timeout);

            HttpResponseMessage response;
            try
            {
                response = await base.SendAsync(request, cts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                if (isUltimaTentativa)
                {
                    throw;
                }
                await Task.Delay(_backoff[tentativa], cancellationToken).ConfigureAwait(false);
                continue;
            }
            catch (HttpRequestException) when (!isUltimaTentativa)
            {
                await Task.Delay(_backoff[tentativa], cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (!EhTransiente(response.StatusCode) || isUltimaTentativa)
            {
                return response;
            }

            response.Dispose();
            await Task.Delay(_backoff[tentativa], cancellationToken).ConfigureAwait(false);
        }

        throw new InvalidOperationException("Código inalcançável.");
    }

    private static bool EhTransiente(HttpStatusCode statusCode) =>
        (int)statusCode is 408 or 429 or 500 or 502 or 503 or 504;
}