using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Infrastructure.Envelope;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de informações de lote ao webservice
/// da NF-e Paulistana.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class ConsultaInformacoesLoteService(HttpClient httpClient)
    : SoapServiceBase<PedidoInformacoesLote, ConsultaInformacoesLoteRequest, ConsultaInformacoesLoteResponse, RetornoInformacoesLote>(
          httpClient,
          "http://www.prefeitura.sp.gov.br/nfe/ws/consultaInformacoesLote",
          "Os dados do Pedido de Informações de Lote não foram validados com sucesso. Detalhes: {0}"),
      IConsultaInformacoesLoteService
{
    /// <inheritdoc/>
    protected override ConsultaInformacoesLoteRequest CreateEnvelope(PedidoInformacoesLote request) =>
        (ConsultaInformacoesLoteRequest)request;

    /// <inheritdoc/>
    protected override RetornoInformacoesLote? ExtractPayload(ConsultaInformacoesLoteResponse response) =>
        response.RetornoXml?.Payload;

    /// <inheritdoc/>
    public new Task<RetornoInformacoesLote> SendAsync(PedidoInformacoesLote pedidoInformacoesLote, CancellationToken cancellationToken = default) =>
        base.SendAsync(pedidoInformacoesLote, cancellationToken);
}