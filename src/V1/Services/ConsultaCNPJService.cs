using Nfe.Paulistana.Infrastructure;
using Nfe.Paulistana.V1.Infrastructure.Envelope;
using Nfe.Paulistana.V1.Models.Operations;
using Nfe.Paulistana.V1.Models.Response;

namespace Nfe.Paulistana.V1.Services;

/// <summary>
/// Serviço responsável por validar e enviar pedidos de consulta de CNPJ ao webservice
/// da NF-e Paulistana.
/// </summary>
/// <param name="httpClient">
/// Instância de <see cref="HttpClient"/> configurada pelo <see cref="IHttpClientFactory"/>,
/// com <see cref="HttpClient.BaseAddress"/> e certificado mTLS já configurados.
/// </param>
internal sealed class ConsultaCNPJService(HttpClient httpClient)
    : SoapServiceBase<PedidoConsultaCNPJ, ConsultaCNPJRequest, ConsultaCNPJResponse, RetornoConsultaCNPJ>(
          httpClient,
          "http://www.prefeitura.sp.gov.br/nfe/ws/consultaCNPJ",
          "Os dados do Pedido de Consulta de CNPJ não foram validados com sucesso. Detalhes: {0}"),
      IConsultaCNPJService
{
    /// <inheritdoc/>
    protected override ConsultaCNPJRequest CreateEnvelope(PedidoConsultaCNPJ request) =>
        (ConsultaCNPJRequest)request;

    /// <inheritdoc/>
    protected override RetornoConsultaCNPJ? ExtractPayload(ConsultaCNPJResponse response) =>
        response.RetornoXml?.Payload;

    /// <inheritdoc/>
    public Task<RetornoConsultaCNPJ> SendAsync(PedidoConsultaCNPJ pedidoConsultaCNPJ, CancellationToken cancellationToken = default) =>
        base.SendAsync(pedidoConsultaCNPJ, cancellationToken);
}
