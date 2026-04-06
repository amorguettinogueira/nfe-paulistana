using Nfe.Paulistana.Extensions;
using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Infrastructure.Envelope;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;
using Nfe.Paulistana.Xml;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de envio em lote de RPS ao webservice
/// da NF-e Paulistana, em modo de produção ou de teste.
/// </summary>
/// <remarks>
/// Em modo de produção, o pedido é transmitido envelopado como <c>EnvioLoteRPSRequest</c>.
/// Em modo de teste, é transmitido como <c>TesteEnvioLoteRPSRequest</c>, sem impacto nos registros fiscais.
/// </remarks>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class EnvioLoteRpsService(HttpClient httpClient) : IEnvioLoteRpsService
{
    private const string InvalidPayload = "Os dados do Pedido de Envio em Lote não foram validados com sucesso. Detalhes: {0}";
    private const string EmptyResponse = "O webservice retornou uma resposta vazia ou inválida.";
    private const string SoapActionEnvioLote = "http://www.prefeitura.sp.gov.br/nfe/ws/envioLoteRPS";
    private const string SoapActionTesteEnvioLote = "http://www.prefeitura.sp.gov.br/nfe/ws/testeenvio";

    private readonly SoapClient _soapClient = new(httpClient ??
        throw new ArgumentNullException(nameof(httpClient)));

    /// <inheritdoc/>
    public async Task<RetornoEnvioLoteRps> SendAsync(
        PedidoEnvioLote pedidoEnvioLote,
        bool modoTeste = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pedidoEnvioLote);

        if (!pedidoEnvioLote.IsValidXsd(out string? error))
        {
            throw new InvalidOperationException(InvalidPayload.Format(error));
        }

        return modoTeste
            ? await SendTesteAsync(pedidoEnvioLote, cancellationToken).ConfigureAwait(false)
            : await SendProducaoAsync(pedidoEnvioLote, cancellationToken).ConfigureAwait(false);
    }

    private async Task<RetornoEnvioLoteRps> SendProducaoAsync(PedidoEnvioLote pedidoEnvioLote, CancellationToken cancellationToken)
    {
        var envelope = new SoapEnvelope<EnvioLoteRpsRequest>((EnvioLoteRpsRequest)pedidoEnvioLote);
        string responseXml = await _soapClient.SendRequestAsync(envelope, SoapActionEnvioLote, cancellationToken).ConfigureAwait(false);
        SoapEnvelope<EnvioLoteRpsResponse> responseEnvelope = SoapClient.DeserializeEnvelope<EnvioLoteRpsResponse>(responseXml);

        return responseEnvelope.Body?.Request?.RetornoXml?.Payload
            ?? throw new InvalidOperationException(EmptyResponse);
    }

    private async Task<RetornoEnvioLoteRps> SendTesteAsync(PedidoEnvioLote pedidoEnvioLote, CancellationToken cancellationToken)
    {
        var envelope = new SoapEnvelope<TesteEnvioLoteRpsRequest>((TesteEnvioLoteRpsRequest)pedidoEnvioLote);
        string responseXml = await _soapClient.SendRequestAsync(envelope, SoapActionTesteEnvioLote, cancellationToken).ConfigureAwait(false);
        SoapEnvelope<TesteEnvioLoteRpsResponse> responseEnvelope = SoapClient.DeserializeEnvelope<TesteEnvioLoteRpsResponse>(responseXml);

        return responseEnvelope.Body?.Request?.RetornoXml?.Payload
            ?? throw new InvalidOperationException(EmptyResponse);
    }
}
