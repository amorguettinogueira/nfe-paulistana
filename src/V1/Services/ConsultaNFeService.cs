using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Infrastructure.Envelope;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de consulta de NFS-e ao webservice
/// da NF-e Paulistana.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class ConsultaNFeService(HttpClient httpClient)
    : SoapServiceBase<PedidoConsultaNFe, ConsultaNFeRequest, ConsultaNFeResponse, RetornoConsulta>(
          httpClient,
          "http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFe",
          "Os dados do Pedido de Consulta de NFS-e não foram validados com sucesso. Detalhes: {0}"),
      IConsultaNFeService
{
    /// <inheritdoc/>
    protected override ConsultaNFeRequest CreateEnvelope(PedidoConsultaNFe request) =>
        (ConsultaNFeRequest)request;

    /// <inheritdoc/>
    protected override RetornoConsulta? ExtractPayload(ConsultaNFeResponse response) =>
        response.RetornoXml?.Payload;

    /// <inheritdoc/>
    public new Task<RetornoConsulta> SendAsync(PedidoConsultaNFe pedidoConsultaNFe, CancellationToken cancellationToken = default) =>
        base.SendAsync(pedidoConsultaNFe, cancellationToken);
}