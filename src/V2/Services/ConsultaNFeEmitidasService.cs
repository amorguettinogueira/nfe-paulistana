using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V2.Infrastructure.Envelope;
using Nfe.Paulistana.V2.Models.Operations;
using Nfe.Paulistana.V2.Models.Response;

namespace Nfe.Paulistana.V2.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de consulta de NFS-e emitidas ao webservice
/// da NF-e Paulistana v02.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class ConsultaNFeEmitidasService(HttpClient httpClient)
    : SoapServiceBase<PedidoConsultaNFePeriodo, ConsultaNFeEmitidasRequest, ConsultaNFeEmitidasResponse, RetornoConsulta>(
          httpClient,
          "http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFeEmitidas",
          "Os dados do Pedido de Consulta de NFS-e Emitidas não foram validados com sucesso. Detalhes: {0}"),
      IConsultaNFeEmitidasService
{
    /// <inheritdoc/>
    protected override ConsultaNFeEmitidasRequest CreateEnvelope(PedidoConsultaNFePeriodo request) =>
        (ConsultaNFeEmitidasRequest)request;

    /// <inheritdoc/>
    protected override RetornoConsulta? ExtractPayload(ConsultaNFeEmitidasResponse response) =>
        response.RetornoXml?.Payload;

    /// <inheritdoc/>
    public Task<RetornoConsulta> SendAsync(PedidoConsultaNFePeriodo pedidoConsultaNFePeriodo, CancellationToken cancellationToken = default) =>
        base.SendAsync(pedidoConsultaNFePeriodo, cancellationToken);
}
