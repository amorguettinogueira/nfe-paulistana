using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Infrastructure.Envelope;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de cancelamento de NFS-e ao webservice
/// da NF-e Paulistana.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class CancelamentoNFeService(HttpClient httpClient)
    : SoapServiceBase<PedidoCancelamentoNFe, CancelamentoNFeRequest, CancelamentoNFeResponse, RetornoCancelamentoNFe>(
          httpClient,
          "http://www.prefeitura.sp.gov.br/nfe/ws/cancelamentoNFe",
          "Os dados do Pedido de Cancelamento de NFS-e não foram validados com sucesso. Detalhes: {0}"),
      ICancelamentoNFeService
{
    /// <inheritdoc/>
    protected override CancelamentoNFeRequest CreateEnvelope(PedidoCancelamentoNFe request) =>
        (CancelamentoNFeRequest)request;

    /// <inheritdoc/>
    protected override RetornoCancelamentoNFe? ExtractPayload(CancelamentoNFeResponse response) =>
        response.RetornoXml?.Payload;

    /// <inheritdoc/>
    public Task<RetornoCancelamentoNFe> SendAsync(PedidoCancelamentoNFe pedidoCancelamentoNFe, CancellationToken cancellationToken = default) =>
        base.SendAsync(pedidoCancelamentoNFe, cancellationToken);
}
