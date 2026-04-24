using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Infrastructure.Envelope;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;

namespace Nfe.Paulistana.V2.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de envio de RPS unitário ao webservice
/// da NF-e Paulistana v02.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class EnvioRpsService(HttpClient httpClient)
    : SoapServiceBase<PedidoEnvio, EnvioRpsRequest, EnvioRpsResponse, RetornoEnvioRps>(
          httpClient,
          "http://www.prefeitura.sp.gov.br/nfe/ws/envioRPS",
          "Os dados do Pedido de Envio de RPS não foram validados com sucesso. Detalhes: {0}"),
      IEnvioRpsService
{
    /// <inheritdoc/>
    protected override EnvioRpsRequest CreateEnvelope(PedidoEnvio request) =>
        (EnvioRpsRequest)request;

    /// <inheritdoc/>
    protected override RetornoEnvioRps? ExtractPayload(EnvioRpsResponse response) =>
        response.RetornoXml?.Payload;

    /// <inheritdoc/>
    public new Task<RetornoEnvioRps> SendAsync(PedidoEnvio pedidoEnvio, CancellationToken cancellationToken = default) =>
        base.SendAsync(pedidoEnvio, cancellationToken);
}