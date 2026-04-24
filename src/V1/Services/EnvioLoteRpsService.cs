using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Infrastructure.Envelope;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;

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
internal sealed class EnvioLoteRpsService(HttpClient httpClient)
    : DualModeSoapServiceBase<PedidoEnvioLote, EnvioLoteRpsRequest, EnvioLoteRpsResponse, TesteEnvioLoteRpsRequest, TesteEnvioLoteRpsResponse, RetornoEnvioLoteRps>(
          httpClient,
          "http://www.prefeitura.sp.gov.br/nfe/ws/envioLoteRPS",
          "http://www.prefeitura.sp.gov.br/nfe/ws/testeenvio",
          "Os dados do Pedido de Envio em Lote não foram validados com sucesso. Detalhes: {0}"),
      IEnvioLoteRpsService
{
    /// <inheritdoc/>
    protected override EnvioLoteRpsRequest CreateEnvelope(PedidoEnvioLote request) =>
        (EnvioLoteRpsRequest)request;

    /// <inheritdoc/>
    protected override RetornoEnvioLoteRps? ExtractPayload(EnvioLoteRpsResponse response) =>
        response.RetornoXml?.Payload;

    /// <inheritdoc/>
    protected override TesteEnvioLoteRpsRequest CreateTesteEnvelope(PedidoEnvioLote request) =>
        (TesteEnvioLoteRpsRequest)request;

    /// <inheritdoc/>
    protected override RetornoEnvioLoteRps? ExtractTestePayload(TesteEnvioLoteRpsResponse response) =>
        response.RetornoXml?.Payload;

    /// <inheritdoc/>
    public Task<RetornoEnvioLoteRps> SendAsync(PedidoEnvioLote pedidoEnvioLote, bool modoTeste = false, CancellationToken cancellationToken = default) =>
        base.SendAsync(pedidoEnvioLote, modoTeste, cancellationToken);
}
