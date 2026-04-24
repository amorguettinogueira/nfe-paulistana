using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Infrastructure.Envelope;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;

namespace Nfe.Paulistana.V2.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de consulta de NFS-e recebidas ao webservice
/// da NF-e Paulistana v02.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class ConsultaNFeRecebidasService(HttpClient httpClient)
    : SoapServiceBase<PedidoConsultaNFePeriodo, ConsultaNFeRecebidasRequest, ConsultaNFeRecebidasResponse, RetornoConsulta>(
          httpClient,
          "http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFeRecebidas",
          "Os dados do Pedido de Consulta de NFS-e Recebidas não foram validados com sucesso. Detalhes: {0}"),
      IConsultaNFeRecebidasService
{
    /// <inheritdoc/>
    protected override ConsultaNFeRecebidasRequest CreateEnvelope(PedidoConsultaNFePeriodo request) =>
        (ConsultaNFeRecebidasRequest)request;

    /// <inheritdoc/>
    protected override RetornoConsulta? ExtractPayload(ConsultaNFeRecebidasResponse response) =>
        response.RetornoXml?.Payload;

    /// <inheritdoc/>
    public Task<RetornoConsulta> SendAsync(PedidoConsultaNFePeriodo pedidoConsultaNFePeriodo, CancellationToken cancellationToken = default) =>
        base.SendAsync(pedidoConsultaNFePeriodo, cancellationToken);
}
