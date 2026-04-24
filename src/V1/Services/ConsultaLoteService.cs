using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Infrastructure.Envelope;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de consulta de lote ao webservice
/// da NF-e Paulistana.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class ConsultaLoteService(HttpClient httpClient)
    : SoapServiceBase<PedidoConsultaLote, ConsultaLoteRequest, ConsultaLoteResponse, RetornoConsulta>(
          httpClient,
          "http://www.prefeitura.sp.gov.br/nfe/ws/consultaLote",
          "Os dados do Pedido de Consulta de Lote não foram validados com sucesso. Detalhes: {0}"),
      IConsultaLoteService
{
    /// <inheritdoc/>
    protected override ConsultaLoteRequest CreateEnvelope(PedidoConsultaLote request) =>
        (ConsultaLoteRequest)request;

    /// <inheritdoc/>
    protected override RetornoConsulta? ExtractPayload(ConsultaLoteResponse response) =>
        response.RetornoXml?.Payload;

    /// <inheritdoc/>
    public new Task<RetornoConsulta> SendAsync(PedidoConsultaLote pedidoConsultaLote, CancellationToken cancellationToken = default) =>
        base.SendAsync(pedidoConsultaLote, cancellationToken);
}